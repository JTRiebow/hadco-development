import * as angular from 'angular';
import * as moment from 'moment';
import * as _ from 'lodash';

import * as template from './detailed-concrete.html';
import * as deleteTemplate from '../../Shared/modal-templates/delete-item-modal.html';
import * as cancelTemplate from '../../Shared/modal-templates/cancel-modal.html';

angular
    .module('detailedApprovalModule')
    .directive('detailedConcrete', detailedConcrete);

function detailedConcrete($window) {
    return {
        restrict: 'E',
        controller: detailedConcreteController,
        template,
        scope: {
            timers: '=',
            dayTimer: '=',
        },
    };
}

detailedConcreteController.$inject = [
    '$scope',
    '$routeParams',
    '$location',
    'DetailedConcreteService',
    'NotificationFactory',
    'EmployeeJobTimers',
    'EmployeeJobEquipmentTimers',
    'TimesheetsService',
    'uiGridConstants',
    '$timeout',
    '$modal',
    '$q',
];

function detailedConcreteController(
    $scope,
    $routeParams,
    $location,
    DetailedConcreteService,
    NotificationFactory,
    EmployeeJobTimers,
    EmployeeJobEquipmentTimers,
    TimesheetsService,
    uiGridConstants,
    $timeout,
    $modal,
    $q,
) {

    init();

    function init() {
        $scope.timersDisabled = false;
        $scope.concreteGridOptions = concreteGridOptions();
        _getEmployeeJobTimers();
        _getDropDownLists();
        $scope.canEditRow = true;
    }

    function concreteGridOptions() {
        return {
            enableHorizontalScrollbar: 1,
            enableColumnMenus: false,
            showColumnFooter: true,
            columnFooterHeight: .5, //needs to display in order to aggregate on columns[9]. 
            showGridFooter: true,
            enableColumnResizing: false,
            columnDefs: _colDefs(),
            showTreeExpandNoChildren: true,
            columnVirtualizationThreshold: 25,
            onRegisterApi: function(gridApi) {
                $scope.gridApi = gridApi;
                $scope.gridApi.core.on.rowsVisibleChanged($scope, function() {
                    $scope.gridTableHeight = _getTableHeight($scope.concreteGridOptions.data.length);
                });
                $scope.gridApi.grid.registerRowsProcessor($scope.employeeTimerIDFilter);
            },
            gridFooterTemplate: _gridFooterTemplate(),
        };
    }

    $scope.$on("cannot-edit-row", function() {
        $scope.canEditRow = false;
    });
    $scope.$on("can-edit-row", function() {
        $scope.canEditRow = true;
    });

    $scope.$watch("dayTimer", function(dayTimer) {
        $scope.gridApi && $scope.gridApi.grid.refresh();
        // _filterByEmployeeTimer(dayTimer);
    });
    $scope.$on("timers-changed", function() {
        $scope.timeCardTotals = null;
    });

    $scope.$on('disableTimers', (_, isDisabled) => {
        $scope.timersDisabled = isDisabled;
    });

    $scope.employeeTimerIDFilter = function(renderableRows) {
        _.each(renderableRows, function(row) {
            var rowEmployeeTimerID = row.entity.employeeTimerID || row.treeNode.parentRow.entity.employeeTimerID;
            var isSameEmployeeTimerID = $scope.dayTimer ? $scope.dayTimer.employeeTimerID == rowEmployeeTimerID : true; //always return true when $scope.dayTimer is null
            row.visible =  isSameEmployeeTimerID;
        });
        return renderableRows;
    };
    
    $scope.addEmployeeJobTimer = function() {
        var row = {
            entity: {
                laborMinutes: 0,
                minutesWorked: 0,
                newJobTimer: true,
                $$treeLevel: 0,

            },
            treeNode: {
                children: [],
            },
        };
        $scope.concreteGridOptions.data.push(row.entity);
        _assignSequenceToTimers(row);
        $scope.beginEdit(row);

    };

    $scope.returnToSupervisorTimesheet = function(supervisorID) {
        $location.path("superintendent/foreman/" + supervisorID + "/department/" + $routeParams.departmentId + "/day/" + $routeParams.dayId);
    };

    $scope.addEquipmentTimer = function(row, index, children) {
        if ($scope.timersDisabled) {
            return;
        }
        var newEquipmentTimer = {
            equipmentMinutes: 0,
            minutesWorked: 0,
            isChild: true,
            editable: true,
            newEquipmentTimer: true,
        };
        var newEquipmentTimerIndex = index + children.length + 1;
        $scope.concreteGridOptions.data.splice(newEquipmentTimerIndex, 0, newEquipmentTimer);
        _assignSequenceToTimers();
        $scope.beginEdit(row, newEquipmentTimer);
    };

    $scope.removeTimer = function(row, index, children) {
        if ($scope.timersDisabled) {
            return;
        }
        if (!row.employeeJobEquipmentTimerID && !row.employeeJobTimerID) {
            $scope.concreteGridOptions.data.splice(index, 1);
            _assignSequenceToTimers();
        }
        else {
            $scope.deletedEntity = row;

            var modalInstance = $modal.open({
                template: deleteTemplate,
                controller: 'DeleteModalContentController',
                windowClass: 'default-modal',
                resolve: {
                    deletedItemName: function() {
                        return row.employeeJobEquipmentTimerID ? row.equipment.equipmentNumber : row.jobTimer.job.jobNumber;
                    },
                },
            });

            modalInstance.result.then(function(data) {
                var rowsToRemove = 1;
                if (row.employeeJobEquipmentTimerID) {
                    if (row.employeeJobEquipmentTimerID) {
                        EmployeeJobEquipmentTimers.one(row.employeeJobEquipmentTimerID).remove()
                            .then(function(data) {
                                NotificationFactory.success("Success: Employee job equipment timer deleted");
                                $scope.concreteGridOptions.data.splice(index, rowsToRemove);
                                _assignSequenceToTimers(row);
                                $scope.$emit("emit-updated-timers");
                            }, function(error) {
                                NotificationFactory.error("Error: Delete unsuccessful");
                            });
                    }
                }
                else if (row.employeeJobTimerID) {
                    EmployeeJobTimers.one(row.employeeJobTimerID).remove()
                    .then(function(data) {
                        rowsToRemove += children.length;
                        $scope.concreteGridOptions.data.splice(index, rowsToRemove);
                        _assignSequenceToTimers(row);
                        $scope.$emit("emit-updated-timers");
                        _filterAvailableJobTimers($scope.allJobTimers);
                    }, function(error) {
                        NotificationFactory.error("Error: Delete unsuccessful");
                    });
                }
            });
        }
    };
    

    $scope.isInvalid = function(row) {
        var employeeJobTimerInvalid = (!row.entity.jobTimer);
        var employeeJobEquipmentTimerInvalid;
        angular.forEach(row.treeNode.children, function(value, key) {
            if (!value.row.entity.minutesWorked || !value.row.entity.equipment) {
                employeeJobEquipmentTimerInvalid = true;
            }
        });
        return (employeeJobTimerInvalid || employeeJobEquipmentTimerInvalid);
    };

    $scope.convertMinutesToHoursMinutes = function(value) {
        return TimesheetsService.convertMinutesToHoursMinutes(value);
    };

    $scope.saveEmployeeJobTimer = function(row) {
        if ($scope.timersDisabled) {
            return;
        }
        var defer = $q.defer();

        var employeeJobTimer = row.entity;
        if (!employeeJobTimer.employeeJobTimerID) {
            var employeeTimerID = _getEmployeeTimerBySupervisorID(employeeJobTimer);
            var newEmployeeJobTimer = {
                jobTimerID: employeeJobTimer.jobTimer.jobTimerID,
                laborMinutes: employeeJobTimer.minutesWorked,
                employeeTimerID: employeeTimerID,
            };
            EmployeeJobTimers.post(newEmployeeJobTimer)
            .then(function(response) {
                NotificationFactory.success('Success: Employee Job timer created');
                _organizeNewTimer(employeeJobTimer, response);
                _saveEquipmentTimers(row)
                .then(function() {
                    $scope.$emit("emit-updated-timers");
                    defer.resolve(response);

                });
            }, function(error) {
                defer.reject();
                NotificationFactory.error('Error: Employee Job timer creation unsuccessful');
            });
        }
        else {
            _saveEquipmentTimers(row).then(function() {
                EmployeeJobTimers.one(employeeJobTimer.employeeJobTimerID).patch({ laborMinutes: employeeJobTimer.minutesWorked })
                .then(function(response) {
                    NotificationFactory.success('Success: Updated Employee Job Timer');
                    _organizeNewTimer(employeeJobTimer, response);
                    $scope.$emit("emit-updated-timers");
                    defer.resolve(response);
                }, function(error) {
                    NotificationFactory.error('Error: Labor time update unsuccessful');
                });
            });
        }
        return defer.promise;
    };

    $scope.warningClassSetup = function(rows) {
        //totalTimersMinutes is based on $scope.gridApi.grid.columns[9].aggregationValue;
        var isTimeCardOrTimersUpdated = rows !== $scope.updatedRows || !$scope.timeCardTotals;
        var isTimersLoaded = $scope.timers;

        if (isTimersLoaded && isTimeCardOrTimersUpdated) {
            $scope.updatedRows = rows.length ? rows : null;
            $scope.totalTimersMinutes = _getTotalTimersMinutes(rows);
            $scope.timeCardTotals = $scope.timeCardTotals || _getTotals();
            $scope.totalOverheadTime = TimesheetsService.convertMinutesToHoursMinutes($scope.timeCardTotals.totalOverheadMinutes);
            $scope.summaryTotalTime = TimesheetsService.convertMinutesToHoursMinutes($scope.timeCardTotals.totalOverheadMinutes + $scope.totalTimersMinutes);
            $scope.totalTimersTime = TimesheetsService.convertMinutesToHoursMinutes($scope.totalTimersMinutes);
            $scope.timeLeftToBeAllocated = TimesheetsService.convertMinutesToHoursMinutes($scope.timeCardTotals.totalTimecardMinutes - ($scope.timeCardTotals.totalOverheadMinutes + $scope.totalTimersMinutes));
            $scope.allocationError = $scope.timeLeftToBeAllocated !== '00:00';

            $scope.showWarning = ($scope.timeCardTotals.totalTimecardMinutes - ($scope.timeCardTotals.totalOverheadMinutes + $scope.totalTimersMinutes) ) < 0;
            return $scope.showWarning ? 'warning error' : '';
            
        }
    };
    
    function _getTotalTimersMinutes(rows) {
        var total = 0;
        _.each(rows, function(row) {
            total += row.entity.totalAllocatedMinutes || 0;
        });
        return total;
    }

    $scope.beginEdit = function(row, newEquipmentTimer) {
        if ($scope.timersDisabled) {
            return;
        }
        if ($scope.canEditRow) {
            row.entity.isChild = false;
            var formattedRow = _.omit(row.entity, 'rowform', 'employeeJobEquipmentTimers');
            $scope.backupEntity = angular.copy(formattedRow);
            $scope.canEditRow = false;
            $scope.$emit("cannot-edit-row");
            row.entity.editable = true;
            $scope.backupChildEntities = [];
            if (row.treeNode.children.length || newEquipmentTimer) {
                angular.forEach(row.treeNode.children, function(value, key) {
                    var formattedRow = _.omit(value.row.entity, 'rowform');
                    value.row.entity.isChild = true;
                    $scope.backupChildEntities.push(angular.copy(formattedRow));
                    value.row.entity.editable = true;
                });
                $scope.expandRow(row);
            }
            $timeout(function() {
                var inputId = newEquipmentTimer ? "#newEquipmentTimerInput" : "#timerInput";
                angular.element(inputId).focus();
                $(inputId).select();
            });
        }
    };

    $scope.confirmCancel = function(row) {
        var modalInstance = $modal.open({
            template: cancelTemplate,
            controller: 'CancelNotificationController',
            windowClass: 'default-modal',
            keyboard: false,
        });

        modalInstance.result.then(function(saveOrCancel) {
            if (saveOrCancel === "save") {
                if (row.entity.isChild) {
                    row = row.treeNode.parentRow;
                }
                if (!$scope.isInvalid(row)) {
                    $scope.saveEmployeeJobTimer(row);
                }
            }
            else {
                $scope.cancelEdit(row);
            }
        });
    };

    $scope.cancelEdit = function(row) {
        if (!row.entity.employeeJobTimerID) {
            $scope.concreteGridOptions.data.splice(row.entity.sequence, 1);
        }
        else {
            _.each(row.treeNode.children, function(child, index) {
                if (!child.row.entity.employeeJobEquipmentTimerID) {
                    $scope.concreteGridOptions.data.splice(child.row.entity.sequence, 1);
                }
                else {
                    var tmp = $scope.backupChildEntities.shift();
                    angular.copy(tmp, child.row.entity);
                }
            });
            angular.copy($scope.backupEntity, row.entity);
        }
        _assignSequenceToTimers();
        $scope.canEditRow = true;
        $scope.$emit("can-edit-row");
    };

    $scope.expandRow = function(row) {
        if (row.treeNode.state !== "expanded") {
            row.grid.api.treeBase.expandRow(row);
        }
    };

    $scope.enterKeypressSave = function(row, type, shiftEnter) {
        if (row.entity.isChild) {
            row = row.treeNode.parentRow;
        }
        if (!$scope.isInvalid(row)) {
            $scope.saveEmployeeJobTimer(row).then(function() {
                angular.forEach(row.treeNode.children, function(value, key) {
                    value.row.entity.editable = false;
                });
                var startRow = row.entity.sequence + row.treeNode.children.length + 1;
                if (row.grid.rows[startRow] && shiftEnter) {
                    var newRowToOpen = row.grid.rows[startRow];
                    $scope.expandRow(newRowToOpen);
                    $scope.beginEdit(newRowToOpen);
                }
                $timeout(function() {
                    angular.element("#timerInput").focus();
                    $("#timerInput").select();
                });
            });
        }
    };

    function _getEmployeeTimerBySupervisorID(employeeJobTimer) {
        var employeeTimer = _.find($scope.timers, function(timer) {
            return timer.supervisor.employeeID === employeeJobTimer.jobTimer.supervisor.employeeID;
        });
        return employeeTimer.employeeTimerID;
    }



    function _getEmployeeJobTimers() {
        DetailedConcreteService.getEmployeeJobTimers($routeParams.employeeId, $routeParams.dayId, $routeParams.departmentId)
        .then(function(data) {
            $scope.employeeJobTimers = data.items;
            _organizeData(data.items);
        }, function(error) {
            console.log(error.message);
            _getAvailableJobTimers();
        });
        
    }
    
    function _organizeData(data) {
        var employeeJobTimers = data;

        var employeeJobAndEquipmentTimersList = [];
        var rowIndex = 0;
        angular.forEach(data, function(employeeJobTimer, index) {
            //console.log('employeeJobTimer', employeeJobTimer);
            employeeJobTimer.minutesWorked = employeeJobTimer.laborMinutes;
            employeeJobTimer.jobTimer.displayName = employeeJobTimer.jobTimer.jobTimerID + ': ' + employeeJobTimer.jobTimer.job.jobNumber + '-' + employeeJobTimer.jobTimer.phase.phaseNumber + '-' + employeeJobTimer.jobTimer.category.categoryNumber + '-' + employeeJobTimer.supervisor.name;
            employeeJobTimer.jobTimer.supervisor = employeeJobTimer.supervisor;
            employeeJobTimer.invoiceNumber = employeeJobTimer.jobTimer.invoiceNumber;
            employeeJobTimer.formattedTotalAllocatedMinutes = TimesheetsService.convertMinutesToHoursMinutes(employeeJobTimer.totalAllocatedMinutes);
            employeeJobTimer.sequence = rowIndex;
            rowIndex++;
            employeeJobTimer.$$treeLevel = 0;
            employeeJobAndEquipmentTimersList.push(employeeJobTimer);
            angular.forEach(employeeJobTimer.employeeJobEquipmentTimers, function(equipmentTimer, index) {
                equipmentTimer.minutesWorked = equipmentTimer.equipmentMinutes;
                equipmentTimer.isChild = true;
                equipmentTimer.sequence = rowIndex;
                rowIndex++;
                employeeJobAndEquipmentTimersList.push(equipmentTimer);
            });
        });
        $scope.concreteGridOptions.data = employeeJobAndEquipmentTimersList;
        $scope.gridTableHeight = _getTableHeight($scope.concreteGridOptions.data.length);
        $scope.timeCardMinutesTotals = null;
        _getAvailableJobTimers(data);
        _getSupervisors();
        
    }

    function _assignSequenceToTimers(row?) {
        angular.forEach($scope.concreteGridOptions.data, function(row, index) {
            row.sequence = index;
        });
    }

    function _getSupervisors() {
        DetailedConcreteService.getSupervisors($routeParams.dayId, $routeParams.departmentId, $routeParams.employeeId)
        .then(function(response) {
            $scope.supervisors = response;
        });
    }

    function _getAvailableJobTimers(data?) {
        DetailedConcreteService.getJobTimers($routeParams.employeeId, $routeParams.dayId, $routeParams.departmentId)
        .then(function(response) {
            $scope.allJobTimers = response;
            _filterAvailableJobTimers(response);
        }, function(error) {
            $scope.disableAddEmployeeJobTimerButton = true;
            console.log(error);
        });
    }

    function _filterAvailableJobTimers(allJobTimers) {

        // Use only jobTimers from the timesheet that aren't already being used.
        $scope.availableJobTimers = [];
        var currentJobTimerIDArray = _.map($scope.concreteGridOptions.data, function(employeeJobTimer) {
            if (employeeJobTimer.jobTimer) {
                return employeeJobTimer.jobTimer.jobTimerID;
            }
        });

        allJobTimers.forEach(function(jobTimer, index) {
            jobTimer.displayName = jobTimer.jobTimerID + ': ' + jobTimer.job.jobNumber + '-' + jobTimer.phase.phaseNumber + '-' + jobTimer.category.categoryNumber + '-' + jobTimer.supervisor.name;
            if (currentJobTimerIDArray.indexOf(jobTimer.jobTimerID) == -1) {
                $scope.availableJobTimers.push(jobTimer);
            }
        });
    }

    function _getDropDownLists() {
        TimesheetsService.getDropDownLists().then(function(response) {
            $scope.dropDownLists = response;
            console.info("Timsheet service finished");
        });
    }

    function _getTotals() {
        //employeeDetailed controller
        var timeCardTotals = _.reduce(
            $scope.timers,
            function(num, timer) {
                var totalTimecardMinutes = timer.totalMinutes;
                num.totalTimecardMinutes += totalTimecardMinutes;

                var totalOverheadMinutes = (timer.greaseMinutes + timer.travelMinutes + timer.shopMinutes + timer.dailyMinutes);
                num.totalOverheadMinutes += totalOverheadMinutes;

                return num;
            }, { totalTimecardMinutes: 0, totalOverheadMinutes: 0 }
        );

        return timeCardTotals;
    }


    function _gridFooterTemplate() {
        return '<div class="gridFooter" ng-class="grid.appScope.warningClassSetup(grid.rows)" ><span><span class="ui-grid-footer-label">Overhead Total:</span> {{(grid.appScope.totalOverheadTime) || 0}} </span><span> <span class="ui-grid-footer-label">Job and Equipment Total:</span> {{(grid.appScope.totalTimersTime) || 0}}</span><span> <span class="ui-grid-footer-label">Summary Total: </span> {{(grid.appScope.summaryTotalTime) || 0}}</span><span> <span class="ui-grid-footer-label">Time Left to Allocate: </span>{{(grid.appScope.timeLeftToBeAllocated) || 0}}</span> </div>';
    }

    function _organizeNewTimer(employeeJobTimer, response) {
        employeeJobTimer.employeeJobTimerID = response.employeeJobTimerID;
        employeeJobTimer.laborMinutes = response.laborMinutes;
        employeeJobTimer.totalAllocatedMinutes = response.totalAllocatedMinutes;
        employeeJobTimer.formattedTotalAllocatedMinutes = TimesheetsService.convertMinutesToHoursMinutes(employeeJobTimer.totalAllocatedMinutes);
        employeeJobTimer.newJobTimer = false;
        employeeJobTimer.editable = false;
        $scope.canEditRow = true;
        $scope.$emit("can-edit-row");

        _filterAvailableJobTimers($scope.allJobTimers);
        $scope.gridApi.core.refresh();
    }

    function _saveEquipmentTimers(row) {
        var defer = $q.defer();
        if (row.treeNode.children.length) {
            angular.forEach(row.treeNode.children, function(child, index) {
                var equipmentTimer = child.row.entity;
                var newEquipmentTimer = {
                    equipmentID: equipmentTimer.equipment.equipmentID,
                    equipmentMinutes: equipmentTimer.minutesWorked,
                    employeeJobTimerID: child.parentRow.entity.employeeJobTimerID,
                };

                if (equipmentTimer.employeeJobEquipmentTimerID) {

                    EmployeeJobEquipmentTimers.one(equipmentTimer.employeeJobEquipmentTimerID).patch(newEquipmentTimer)
                    .then(function(response) {
                        _organizeNewEquipmentTimer(equipmentTimer, response);
                        if (index === row.treeNode.children.length - 1) { defer.resolve(response); }
                    }, function(error) {
                        NotificationFactory.error("Error: Equipment timer didn't save. Check for overlapping timers.");
                    });
                }
                else {
                    EmployeeJobEquipmentTimers.post(newEquipmentTimer)
                    .then(function(response) {
                        _organizeNewEquipmentTimer(equipmentTimer, response, row);
                        if (index === row.treeNode.children.length - 1) { defer.resolve(response); }
                    }, function(error) {
                        NotificationFactory.error("Error: Equipment timer didn't save. Check for overlapping timers.");
                    });
                }
            });
        }
        else {
            defer.resolve();
        }

        return defer.promise;

    }
    function _organizeNewEquipmentTimer(equipmentTimer, response, row?) {
        equipmentTimer.equipmentMinutes = response.equipmentMinutes;
        equipmentTimer.employeeJobEquipmentTimerID = response.employeeJobEquipmentTimerID;
        equipmentTimer.editable = false;
        equipmentTimer.newEquipmentTimer = false;
    }

    function _colDefs() {
        var defaultCellTemplate = '<div class="ui-grid-cell-contents" ng-if="!row.entity.editable">{{COL_FIELD}}</div>';
        return [
            {
                displayName: 'Timer ID', field: 'jobTimer.jobTimerID', width: 100,
                cellTemplate: '<ui-grid-dropdown required="true" row="row" is-cell-hidden="row.entity.isChild" is-cell-editable="row.entity.newJobTimer && row.entity.editable" focus-id="timerInput" col-field="row.entity.jobTimer.jobTimerID"' +
                    'dropdown-list="grid.appScope.availableJobTimers"  model-collection="jobTimer" model-identifier="displayName">' +
                    '</ui-grid-dropdown>',
            },
            {
                name: 'Job', field: 'jobTimer.job.jobNumber', width: 120,
                
            },
            {
                name: 'Phase', field: 'jobTimer.phase.phaseNumber', width: 80,
            },
            {
                name: 'Category', field: 'jobTimer.category.categoryNumber', width: 100,
            },
            {
                name: 'Supervisor', field: 'jobTimer.supervisor.name', width: 180, cellTemplate: '<a ng-click="grid.appScope.returnToSupervisorTimesheet(row.entity.supervisor.employeeID)">{{COL_FIELD}}</a>',
            },
            {
                name: 'Job Notes', field: 'jobTimer.diary', cellTooltip: true, minWidth: 200,
            },
                {
                    name: 'Equip. #', field: 'equipment.equipmentNumber', width: 100,
                    cellTemplate: '<ui-grid-typeahead required="true" row="row"  is-cell-editable="row.entity.editable" is-cell-hidden="!row.entity.isChild" col-field="COL_FIELD" ' +
                        'typeahead-list="grid.appScope.dropDownLists.equipmentList" focus-id="{{row.entity.newEquipmentTimer ? \'newEquipmentTimerInput\' : null}}" model-collection="equipment" model-identifier="equipmentNumber">' +
                        '</ui-grid-typeahead>',
                },
            {
                name: 'Hours', field: 'laborMinutes', width: 100, cellTemplate: '<ui-grid-hour-minute focus-id="timerInput"  is-cell-editable="row.entity.editable" row="row" original-minutes="row.entity.minutesWorked">' +
                        '</ui-grid-hour-minute>',
            },

            {
                name: 'Total Hours', field: 'totalAllocatedMinutes', cellTemplate: '<div class="ui-grid-cell-contents"  ng-hide="row.entity.equipmentID">{{row.entity.formattedTotalAllocatedMinutes}}</div>',
                pinnedRight: true, width: 120,
            },
            {
                name: 'Invoice', field: 'invoiceNumber', width: 120, pinnedRight: true,
            },
            {
                name: 'Edit', width: 125, pinnedRight: true,
                cellTemplate: '<div ng-if="!row.entity.isChild">' +
                    '<a ng-disabled="(!grid.appScope.canEditRow || grid.appScope.timersDisabled) && grid.appScope.allocationError" data-toggle="tooltip" title="Allocation Error" class="fa fa-lg fa-flag icon"' +
                        'ng-class="{ \'hide\': row.entity.editable || !grid.appScope.allocationError }"' +
                        'ng-click="!grid.appScope.canEditRow || grid.appScope.beginEdit(row)">' +
                    '</a>' +
                    '<i class="fa fa-lg fa-pencil icon" data-toggle="tooltip" title="Edit" aria-hidden="true" ' +
                        'ng-class="{ \'hide\': row.entity.editable }"' +
                        'ng-disabled="!grid.appScope.canEditRow || grid.appScope.timersDisabled" ng-click="!grid.appScope.canEditRow || grid.appScope.beginEdit(row)">' +
                        '</i>' +
                    '<i ng-disabled="!grid.appScope.canEditRow || grid.appScope.timersDisabled" data-toggle="tooltip" title="Delete" class="fa fa-lg fa-trash icon"' +
                        'ng-class="{ \'hide\': row.entity.editable }"' +
                        'ng-click="!grid.appScope.canEditRow || grid.appScope.removeTimer(row.entity, row.entity.sequence, row.treeNode.children)">' +
                    '</i>' +
                    '<img ng-disabled="!grid.appScope.canEditRow || grid.appScope.timersDisabled" data-toggle="tooltip" title="Add Equipment"' +
                        'ng-class="{ \'hide\': row.entity.editable || grid.appScope.timersDisabled }"' +
                        'ng-click="!grid.appScope.canEditRow || grid.appScope.addEquipmentTimer(row, row.entity.sequence, row.treeNode.children)" class="icon-image" src="' + require('../../../assets/images/excavator-icon.svg') + '">' +
                    '</img>' +
                    '<i class="fa fa-lg fa-floppy-o icon" data-toggle="tooltip" title="Save"' +
                        'ng-disabled="grid.appScope.isInvalid(row) || grid.appScope.timersDisabled"' +
                        'ng-click="grid.appScope.isInvalid(row) || grid.appScope.saveEmployeeJobTimer(row)"' +
                        'ng-class="{ \'hide\': !row.entity.editable }">' +
                    '</i>' +
                    '<i class="fa fa-lg fa-ban icon" ng-click="grid.appScope.cancelEdit(row)" ng-class="{ \'hide\': !row.entity.editable }"' +
                    'data-toggle="tooltip" title="Cancel"></i>' +
                    '</div>' +
                    '<i ng-if="row.entity.isChild" ng-disabled="!grid.appScope.canEditRow || grid.appScope.timersDisabled" class="fa fa-lg fa-trash icon"' +
                        'ng-class="{ \'hide\': row.entity.editable }" data-toggle="tooltip" title="Delete"' +
                        'ng-click="!grid.appScope.canEditRow || grid.appScope.removeTimer(row.entity, row.entity.sequence)" >' +
                    '</i>',
            },
        ];
    }

    function _getTableHeight(dataLength) {
        dataLength = dataLength || 1;
        var rowHeight = 30;
        var headerHeight = 60;
        var scrollbarHeight = 15;
        return {
            height: (dataLength * rowHeight + headerHeight + scrollbarHeight) + "px",
        };
    }
}