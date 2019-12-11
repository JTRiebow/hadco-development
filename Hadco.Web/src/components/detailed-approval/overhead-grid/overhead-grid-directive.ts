import * as angular from 'angular';

import * as template from './overhead-grid.html';
import * as cancelTemplate from '../../Shared/modal-templates/cancel-modal.html';

angular
    .module('detailedApprovalModule')
    .directive('overheadGrid', overheadGrid);

function overheadGrid() {
    overheadGridController.$inject = [
        '$scope',
        '$q',
        '$routeParams',
        '$location',
        '$modal',
        '$timeout',
        'DetailedConcreteService',
        'TimesheetsService',
        'EmployeeDetailedService',
        'EmployeeTimerEntries',
        'EmployeeTimers',
        'NotificationFactory',
        'Users',
        'DateTimeFormats',
        'PermissionService',
    ];
    
    return {
        template,
        restrict : 'E',
        controller: overheadGridController,
        controllerAs: 'vm',
        scope: {
            timers: '=',
            activeTimer: '=',
        },
    };
    
    function overheadGridController(
        $scope,
        $q,
        $routeParams,
        $location,
        $modal,
        $timeout,
        DetailedConcreteService,
        TimesheetsService,
        EmployeeDetailedService,
        EmployeeTimerEntries,
        EmployeeTimers,
        NotificationFactory,
        Users,
        DateTimeFormats,
        PermissionService,
    ) {
        var vm = this;
        
        vm.can = PermissionService.can;
        
        vm.overheadGridOptions;
    
        init();
    
        function init() {
            vm.timersDisabled = false;
            vm.timers = $scope.timers;
            vm.selectedTimers = [ $scope.activeTimer ] || vm.timers;
            //vm.selectedTimer = $scope.activeTimer == "all" ? vm.timers : [vm.timers[$scope.activeTimer]];
            vm.overheadGridOptions = _getOverheadGridOptions();
            _populateGrid(vm.selectedTimers, vm.timers);
            vm.canEditRow = true;
    
            $scope.$watch("timers", function(timers) {
                vm.timers = timers;
                _populateGrid(vm.selectedTimers, timers);
            });
            $scope.$watch("activeTimer", function(activeTimer) {
                _populateGrid(activeTimer, vm.timers);
            });
            $scope.$on('disableTimers', (_, isDisabled) => {
                vm.timersDisabled = isDisabled;
            });
        }
    
        vm.beginEdit = function(row, index) {
            if (vm.timersDisabled) {
                return;
            }
            if (vm.canEditRow) {
                vm.backupEntity = angular.copy(row.entity);
                vm.canEditRow = false;
                $scope.$emit("cannot-edit-row");
                row.entity.sequence = index;
                row.entity.editable = true;
                $timeout(function() {
                    angular.element("#overheadInput").focus();
                        $("#overheadInput").select();
                });
            }
        };
    
        vm.cancelEdit = function(row) {
            vm.canEditRow = true;
            $scope.$emit("can-edit-row");
            angular.copy(vm.backupEntity, row.entity);
        };
        //Need to be on scope instead of VM for type ahead directive
        $scope.confirmCancel = function(row) {
            var modalInstance = $modal.open({
                template: cancelTemplate,
                controller: 'CancelNotificationController',
                windowClass: 'default-modal',
                keyboard: false,
            });
    
            modalInstance.result.then(function(saveOrCancel) {
                if (saveOrCancel === "save") {
                    vm.saveOverhead(row);
                }
                else {
                    vm.cancelEdit(row);
                }
            });
        };
    
        //Need to be on scope instead of VM for type ahead directive
        $scope.enterKeypressSave = function(row, type, shiftEnter) {
            vm.saveOverhead(row).then(function() {
                var startRow = row.entity.sequence + 1;
                if (row.grid.rows[startRow] && shiftEnter) {
                    var newRowToOpen = row.grid.rows[startRow];
                    vm.beginEdit(newRowToOpen);
                }
            });
        };
    
        vm.saveOverhead = function(row) {
            if (vm.timersDisabled) {
                return;
            }
            var deferred = $q.defer();
            var overheadObj = {
                "shopMinutes": row.entity.shopMinutes,
                "travelMinutes": row.entity.travelMinutes,
                "greaseMinutes": row.entity.greaseMinutes,
                "dailyMinutes": row.entity.dailyMinutes,
            };
            EmployeeTimers.one(row.entity.employeeTimerID).patch(overheadObj)
            .then(function(response) {
                NotificationFactory.success("Success: Overhead saved.");
                vm.canEditRow = true;
                $scope.$emit("emit-updated-timers");
                row.entity.editable = false;
                deferred.resolve();
    
            }, function(error) {
                NotificationFactory.error("Error: Overhead not saved.");
                angular.copy(vm.backupEntity, row.entity);
                vm.canEditRow = true;
                row.entity.editable = false;
                deferred.reject();
            });
            return deferred.promise;
        };
    
        vm.setHistory = function(timer) {
            EmployeeDetailedService.setHistory(timer)
            .then(function(response) {
                vm.visibleHistory = response;
            });
        };
    
        function _populateGrid(selectedTimers, allTimers) {
            if (allTimers) {
                vm.selectedTimers = selectedTimers ? [ selectedTimers ] : allTimers;
                vm.overheadGridOptions.data = vm.selectedTimers;
                vm.gridTableHeight = _getTableHeight(vm.overheadGridOptions.data.length);
            }
        }
    
        function _getOverheadGridOptions() {
            return {
                enableGridMenu: false,
                enableColumnMenus: false,
                enableHorizontalScrollbar: 1,
                enableVerticalScrollbar: 0,
                columnDefs: _colDefs(),
            };
    
        }
    
        function _colDefs() {
            vm.isMechanic = false;
            if ($routeParams.departmentId == 5) {
                vm.isMechanic = true;
            }
            
            return [
                {
                    name: 'Foreman',
                    field: 'supervisor.name',
                },
                {
                    visible: !vm.isMechanic,
                    name: 'Shop',
                    field: 'shopMinutes',
                    cellTemplate: `
                        <ui-grid-decimal
                            focus-id="'overheadInput'"
                            decimal-places="0"
                            row="row"
                            col-field-key="shopMinutes"
                            is-cell-editable="row.entity.editable"
                        ></ui-grid-decimal>
                    `,
                },
                {
                    visible: !vm.isMechanic,
                    name: 'Grease',
                    field: 'greaseMinutes',
                    cellTemplate: `
                        <ui-grid-decimal
                            row="row"
                            decimal-places="0"
                            col-field-key="greaseMinutes"
                            is-cell-editable="row.entity.editable"
                        ></ui-grid-decimal>
                    `,
                },
                {
                    field: 'equipment.equipmentNumber',
                    name: 'Truck',
                },
                {
                    visible: !vm.isMechanic,
                    name: 'Travel',
                    field: 'travelMinutes',
                    cellTemplate: `
                        <ui-grid-decimal
                            row="row"
                            decimal-places="0"
                            col-field-key="travelMinutes"
                            is-cell-editable="row.entity.editable"
                        ></ui-grid-decimal>
                    `,
                },
                {
                    name: 'Daily',
                    field: 'dailyMinutes',
                    cellTemplate: `
                        <ui-grid-decimal
                            ng-if="!grid.appScope.vm.isMechanic"
                            row="row"
                            decimal-places="0"
                            col-field-key="dailyMinutes"
                            is-cell-editable="row.entity.editable"
                        ></ui-grid-decimal>
                        
                        <div
                            ng-if="grid.appScope.vm.isMechanic"
                            ng-bind="grid.appScope.vm.getTravelHoursMinutes(row.entity.travelMinutes)"
                        ></div>
                    `,
                },
                {
                    visible: !vm.isMechanic,
                    name: 'Actions',
                    field: 'time',
                    cellTemplate: `
                        <div>
                            <i
                                class="fa fa-lg fa-pencil icon"
                                data-toggle="tooltip"
                                title="Edit"
                                aria-hidden="true"
                                ng-class="{ hide: row.entity.editable }"
                                ng-disabled="!grid.appScope.vm.canEditRow || grid.appScope.vm.timersDisabled"
                                ng-click="!grid.appScope.vm.canEditRow || grid.appScope.vm.beginEdit(row, rowRenderIndex)"
                                ng-if="grid.appScope.vm.can('editTimerOverhead')"
                            ></i>
                            
                            <i
                                ng-disabled="!grid.appScope.vm.canEditRow || !row.entity.employeeTimerHistories.length || grid.appScope.vm.timersDisabled"
                                class="fa fa-clock-o icon" 
                                data-toggle="tooltip" 
                                title="View Edit History" 
                                aria-hidden="true" 
                                ng-click="!grid.appScope.vm.canEditRow || !row.entity.employeeTimerHistories.length || grid.appScope.vm.setHistory(row.entity)"
                                ng-if="grid.appScope.vm.can('viewTimerOverheadEditHistory')"
                            ></i>
                            
                            <i class="fa fa-lg fa-floppy-o icon" data-toggle="tooltip" title="Save"
                                ng-click="grid.appScope.vm.saveOverhead(row)"
                                ng-class="{ hide: !row.entity.editable }"
                            ></i>
                            
                            <i
                                class="fa fa-lg fa-ban icon"
                                ng-click="grid.appScope.vm.cancelEdit(row)"
                                ng-class="{ hide: !row.entity.editable }"
                                data-toggle="tooltip"
                                title="Cancel"
                            ></i>
                        </div>
                    `,
                },
    
            ];
        }
    
        vm.getTravelHoursMinutes = function(travelMinutes) {
            var maxSingleDigitHoursInMinutes = 600;
            var time = TimesheetsService.convertMinutesToHoursMinutes(travelMinutes);
            if (travelMinutes < maxSingleDigitHoursInMinutes) {
                time = time.substring(1);
            }
            
            return time;
        };
    
        function _getTableHeight(dataLength) {
            dataLength = dataLength || 1;
            var rowHeight = 30;
            var headerHeight = 30;
            var scrollbarHeight = 15;
            return {
                height: (dataLength * rowHeight + headerHeight + scrollbarHeight) + "px",
            };
        }
    }
}