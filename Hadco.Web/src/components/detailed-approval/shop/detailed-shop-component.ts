import * as angular from 'angular';
import * as moment from 'moment';
import * as _ from 'lodash';

import * as template from './detailed-shop.html';
import * as deleteTemplate from '../../Shared/modal-templates/delete-item-modal.html';
import * as cancelTemplate from '../../Shared/modal-templates/cancel-modal.html';

angular
    .module('detailedApprovalModule')
    .directive('detailedShop', detailedShop);

function detailedShop($window) {
    detailedShopController.$inject = [
        '$scope',
        '$q',
        '$timeout',
        '$routeParams',
        'EquipmentServiceTypesHelper',
        'CategoriesHelper',
        'EquipmentHelper',
        'TimesheetsService',
        'JobTimers',
        'EquipmentTimers',
        'NotificationFactory',
        'uiGridConstants',
        '$modal',
    ];
    
    return {
        link: angular.noop,
        restrict: 'E',
        controller: detailedShopController,
        template,
        scope: {
            timers: '=',
        },
    };
    
    function detailedShopController(
        $scope,
        $q,
        $timeout,
        $routeParams,
        EquipmentServiceTypesHelper,
        CategoriesHelper,
        EquipmentHelper,
        TimesheetsService,
        JobTimers,
        EquipmentTimers,
        NotificationFactory,
        uiGridConstants,
        $modal
    ) {
    
        var equipmentTimersGridOptions = {
            enableHorizontalScrollbar: 1,
            enableColumnMenus: false,
            showColumnFooter: true,
            columnFooterHeight: .5, //needs to display in order to aggregate on columns[9]. 
            columnVirtualizationThreshold: 25,
            cellEditableCondition: false,
            onRegisterApi: function(gridApi) {
                $scope.equipmentGridApi = gridApi;
                $scope.equipmentGridApi.core.on.rowsVisibleChanged($scope, function() {
                    $scope.equipmentTimersGridTableHeight = _getTableHeight($scope.equipmentTimersGridOptions.data.length);
                });
            },
        };
    
        var jobTimersGridOptions = {
            enableHorizontalScrollbar: 1,
            enableColumnMenus: false,
            showColumnFooter: true,
            showGridFooter: true,
            columnFooterHeight: .5, //needs to display in order to aggregate on columns[9]. 
            columnVirtualizationThreshold: 25,
            columnDefs: _jobTimersColDefs(),
            cellEditableCondition: false,
            onRegisterApi: function(gridApi) {
                $scope.shopGridApi = gridApi;
                $scope.shopGridApi.core.on.rowsVisibleChanged($scope, function() {
                    $scope.shopTimersGridTableHeight = _getTableHeight($scope.jobTimersGridOptions.data.length, $scope.shopGridApi.grid.footerHeight);
                });
            },
            gridFooterTemplate: _gridFooterTemplate(),
    
        };
    
        var jobAndEquipmentTimersGridOptions = {
            enableHorizontalScrollbar: 1,
            enableColumnMenus: false,
            showColumnFooter: true,
            showGridFooter: true,
            columnFooterHeight: .5, //needs to display in order to aggregate on columns[9]. 
            columnVirtualizationThreshold: 25,
            cellEditableCondition: false,
            onRegisterApi: function(gridApi) {
                $scope.jobAndEquipmentGridApi = gridApi;
                $scope.jobAndEquipmentGridApi.core.on.rowsVisibleChanged($scope, function() {
                    $scope.jobAndEquipmentTimersGridTableHeight = _getTableHeight($scope.jobAndEquipmentTimersGridOptions.data.length);
                });
            },
            gridFooterTemplate: _gridFooterTemplate(),
    
    
        };
    
        init();
            
        function init() {
            $scope.timersDisabled = false;
            $scope.showWarning = {};
            $scope.totalTimersMinutes = {};
            $scope.equipmentTimersGridOptions = equipmentTimersGridOptions;
            $scope.jobTimersGridOptions = jobTimersGridOptions;
            $scope.jobAndEquipmentTimersGridOptions = jobAndEquipmentTimersGridOptions;
            _getTimers();
            _getDropDownLists();
            $scope.canEditRow = true;
            $scope.dateList = TimesheetsService.getDateList($routeParams.dayId);
            $scope.shopTimerTypes = { equipment: 'equipment', shop: 'shop' };
            $scope.jobSaved = false;
        }
    
    
        $scope.$on("cannot-edit-row", function() {
            $scope.canEditRow = false;
        });
        $scope.$on("can-edit-row", function() {
            $scope.canEditRow = true;
        });
    
    
        $scope.$on("timers-changed", function() {
            $scope.timeCardTotals = null;
        });
    
        $scope.$on('disableTimers', (_, isDisabled) => {
            $scope.timersDisabled = isDisabled;
        });
    
        $scope.isInvalid = function(timer, type) {
            timer.totalMinutes = (moment(timer.stopTime) - moment(timer.startTime)) / 60000.0
            if (type == $scope.shopTimerTypes.equipment) {
                timer.invalid = !timer.equipment || !timer.stopTime || !timer.startTime || timer.totalMinutes < 0 || !timer.equipmentServiceType || timer.closed === undefined ;
            }
            else if (type == $scope.shopTimerTypes.shop) {
                timer.invalid = !timer.category || !timer.stopTime || !timer.startTime || timer.totalMinutes < 0;
            }
            return timer.invalid;
        };
    
    
        $scope.convertMinutesToHoursMinutes = function(value) {
            return TimesheetsService.convertMinutesToHoursMinutes(value);
        };
    
        $scope.assignClosedOpen = function(item, index, type, shopTimerType) {
            //index = index || $scope.equipmentTimersGridOptions.data.length - 1;
            $scope.jobAndEquipmentTimersGridOptions.data[index].closed = item.name;
        };
    
        $scope.syncDateAndTime = function(item, index, type, entity) {
            var syncModelAndTime = TimesheetsService.syncDateAndTime(item, type, entity);
            entity[syncModelAndTime.key] = syncModelAndTime.value;
        };
    
        $scope.addShopOrEquipmentTimer = function(type) {
            var defaultStart = _setAndFormatDefaultTime($scope.earliestClockIn);
            var defaultStop = _setAndFormatDefaultTime($scope.lastClockOut);
            var rowToEdit;
            var newTimer;
            if (type === 'equipment') {
                newTimer = { equipmentID: 1, startTime: new Date(defaultStart), stopTime: new Date(defaultStop), editable: true, isNewItem: true };
            }
            else if (type === 'shop') {
                newTimer = { jobID: 1, startTime: new Date(defaultStart), stopTime: new Date(defaultStop), editable: true, isNewItem: true };
            }
            newTimer.sequence = $scope.jobAndEquipmentTimersGridOptions.data.length;
            $scope.jobAndEquipmentTimersGridOptions.data.push(newTimer);
            $scope.jobAndEquipmentTimersGridTableHeight = _getTableHeight($scope.jobAndEquipmentTimersGridOptions.data.length);
            rowToEdit = { entity: $scope.jobAndEquipmentTimersGridOptions.data[$scope.jobAndEquipmentTimersGridOptions.data.length - 1] };
            $scope.beginEdit(rowToEdit, type, true);
        };

        function indexTimer(timer) {
            var data = $scope.jobAndEquipmentTimersGridOptions.data
            var timerReference = data.find(t => {
                if (timer.equipmentTimerID) {
                    return t.equipmentTimerID == timer.equipmentTimerID
                } else if (timer.jobTimerID) {
                    return t.jobTimerID == timer.jobTimerID
                }
            })
            var index = data.indexOf(timerReference)

            return index
        }
    
        $scope.deleteTimer = function(timer, index, type) {
            index = indexTimer(timer)

            if ($scope.timersDisabled) {
                return;
            }
            if (type === $scope.shopTimerTypes.shop) {
                _deleteJobTimer(timer, index);
            }
            else if (type ===  $scope.shopTimerTypes.equipment) {
                _deleteEquipmentTimer(timer, index);
            }
        };
    
    
        $scope.saveTimer = function(timer, index, type) {
            if ($scope.timersDisabled) {
                return;
            }
            if (type === $scope.shopTimerTypes.shop) {
                _saveJobTimer(timer, index)
                .then(function(response) {
                    _getTimers()
                    $scope.$emit("emit-updated-timers");
                    $scope.jobAndEquipmentGridApi.core.refresh();
                });
            }
            else if (type === $scope.shopTimerTypes.equipment) {
                _saveEquipmentTimer(timer, index)
                .then(function(response) {
                    _getTimers()
                    $scope.$emit("emit-updated-timers");
                    $scope.jobAndEquipmentGridApi.core.refresh();
                });
            }
        };
    
        $scope.warningClassSetup = function(totalTimersMinutes, type) {
            //totalTimersMinutes is based on $scope.gridApi.grid.columns[5].aggregationValue;
            var isTimeCardOrTimersUpdated = totalTimersMinutes !== $scope.totalTimersMinutes[type] || !$scope.timeCardTotals;
            var isTimersLoaded = $scope.timers;
    
            if (isTimersLoaded && isTimeCardOrTimersUpdated) {
                $scope.totalTimersMinutes[type] = totalTimersMinutes;
                $scope.timeCardTotals = $scope.timeCardTotals || _getTotals();
                $scope.totalOverheadTime = TimesheetsService.convertMinutesToHoursMinutes($scope.timeCardTotals.totalOverheadMinutes);
                if($scope.totalTimersMinutes.Equipment === undefined) {
                    $scope.totalTimersMinutes.Equipment = 0
                }
                if($scope.totalTimersMinutes.Shop === undefined) {
                    $scope.totalTimersMinutes.Shop = 0
                }
                $scope.summaryTotalTime = TimesheetsService.convertMinutesToHoursMinutes($scope.timeCardTotals.totalOverheadMinutes + $scope.totalTimersMinutes.Equipment + $scope.totalTimersMinutes.Shop);

                $scope.totalEquipmentTimersTime = TimesheetsService.convertMinutesToHoursMinutes($scope.totalTimersMinutes.Equipment);
                $scope.totalShopTimersTime = TimesheetsService.convertMinutesToHoursMinutes($scope.totalTimersMinutes.Shop);
                $scope.timeLeftToBeAllocated = TimesheetsService.convertMinutesToHoursMinutes($scope.timeCardTotals.totalTimecardMinutes - ($scope.timeCardTotals.totalOverheadMinutes + $scope.totalTimersMinutes.Equipment + $scope.totalTimersMinutes.Shop));
                $scope.allocationError = $scope.timeLeftToBeAllocated != "00:00"
                
                $scope.showWarning[type] = ($scope.timeCardTotals.totalOverheadMinutes + totalTimersMinutes) > $scope.timeCardTotals.totalTimecardMinutes
                $scope.warningText = type;
                
                return $scope.showWarning[type] ? 'warning error' : '';
    
            }
        };
    
        /**
         *  Sets a table row to editable, emits all other rows aren't editable, and places the cursor in the editable row.
         *
         * @param {*} row
         * @param {string} type
         * @param {boolean} [isNewRow]
         * @returns {void}
         */
        $scope.beginEdit = function(row: any, type: string, isNewRow?: boolean): void {
            if ($scope.timersDisabled) {
                return;
            }
            if ($scope.canEditRow) {
                row.entity.startDate =_setUpStartAndStopDatesDropDown(row.entity.startTime);
                row.entity.stopDate = _setUpStartAndStopDatesDropDown(row.entity.stopTime);
                
                $scope.backupEntity = isNewRow ? undefined : angular.copy(row.entity);
                $scope.canEditRow = false;
                $scope.$emit("cannot-edit-row");
                row.entity.editable = true;
                $timeout(function() {
                    if (type == $scope.shopTimerTypes.equipment) {
                        angular.element("#equipmentInput").focus();
                        $("#equipmentInput").select();
                    }
                    else if (type == $scope.shopTimerTypes.shop) {
                        angular.element("#shopInput").focus();
                        $("#shopInput").select();
                    }
                });
            }
        };
    
        function _setUpStartAndStopDatesDropDown(modelDateTime) {
            var matchesDayOnTimeCard = (moment(modelDateTime).format('MM-DD-YYYY') == $routeParams.dayId);
            return matchesDayOnTimeCard ? $scope.dateList[0] : $scope.dateList[1];
        }
    
        $scope.confirmCancel = function(row, type) {
            var modalInstance = $modal.open({
                template: cancelTemplate,
                controller: 'CancelNotificationController',
                windowClass: 'default-modal',
                keyboard: false,
            });
    
            modalInstance.result.then(function(saveOrCancel) {
                if (saveOrCancel === "save") {
                    if (!$scope.isInvalid(row)) {
                        $scope.saveTimer(row.entity, row.entity.sequence, type);
                    }
                }
                else {
                    $scope.cancelEdit(row, type);
                }
            });
        };
    
        /**
         * Emits all rows can be edited.
         * If new, the row is deleted; if not new, the previous row value is restored.
         * 
         * @param {*} row
         * @param {string} type
         * @returns {void}
         */
        $scope.cancelEdit = function(row: any, type: string): void {
            $scope.canEditRow = true;
            $scope.$emit("can-edit-row");

            if ($scope.backupEntity) {
                if (row.entity) {
                    angular.copy($scope.backupEntity, row.entity);
                }
                else {
                    angular.copy($scope.backupEntity, row);
                }
                $scope.backupEntity = undefined;
            }
            else {
                $scope.jobAndEquipmentTimersGridOptions.data.pop();
            }
        };
    
    
        $scope.enterKeypressSave = function(row, type, shiftEnter) {
            if (!$scope.isInvalid(row.entity, type)) {
                if (type == $scope.shopTimerTypes.equipment) {
                    _saveEquipmentTimer(row.entity, row.entity.sequence).then(function() {
                        var startRowIndex = row.entity.sequence + 1;
                        if (row.grid.rows[startRowIndex] && shiftEnter) {
                            $scope.beginEdit(row.grid.rows[startRowIndex], type);
                        }
                    });
                }
                else if (type == $scope.shopTimerTypes.shop) {
                    _saveJobTimer(row.entity, row.entity.sequence, type).then(function() {
                        var startRowIndex = row.entity.sequence + 1;
                        if (row.grid.rows[startRowIndex]) {
                            //begins edits for next row that isn't a downtime timer. 
                            var remainingTimerRows = row.grid.rows.slice(startRowIndex);
                            var nextEditableTimer = _.find(remainingTimerRows, function(timerRow) {
                                return timerRow.entity.category.categoryNumber !== "DOWN";
                            });
                            (nextEditableTimer && shiftEnter) ? $scope.beginEdit(nextEditableTimer, type) : $scope.cancelEdit(row, type);
                        }
                    });
    
                }
            }
        };
    
    
        function _getTimers() {
            TimesheetsService.getEmployeeTimesheet($routeParams.employeeId, $routeParams.dayId, $routeParams.departmentId)
            .then(function(data) {
                $scope.timesheet = data;
                angular.forEach(data.equipmentTimers, function(timer, index) {
                    timer.formattedTotalMinutes = TimesheetsService.convertMinutesToHoursMinutes(timer.totalMinutes);
                    timer.sequence = index;
                });
                angular.forEach(data.jobTimers, function(timer, index) {
                    timer.formattedTotalMinutes = TimesheetsService.convertMinutesToHoursMinutes(timer.totalMinutes);
                    timer.sequence = index;
                });
                $scope.equipmentTimers = data.equipmentTimers;
                $scope.masterEquipmentTimers = angular.copy(data.equipmentTimers);
                $scope.equipmentTimersGridOptions.data = data.equipmentTimers;
    
                $scope.equipmentTimersGridOptions.columnDefs = _equipmentTimersColDefs();

                $scope.jobTimers = data.jobTimers;
                $scope.jobTimersGridOptions.data = data.jobTimers;
                $scope.masterJobTimers = angular.copy(data.jobTimers);
                $scope.jobTimersGridOptions.columnDefs = _jobTimersColDefs();
    
                $scope.totalTimeCardMinutes = data.employeeTimers.reduce(function(a, b) { return a + b.totalMinutes; }, 0);
                $scope.jobAndEquipmentTimersGridOptions.data = data.equipmentTimers.concat(data.jobTimers);
                $scope.jobAndEquipmentTimersGridOptions.columnDefs = _jobAndEquipmentTimersColDefs();
                
    
    
                $scope.equipmentTimersTotalMinutes = data.equipmentTimers.reduce(function(a, b) { return a + b.totalMinutes; }, 0);
                $scope.masterEquipmentTimersTotalMinutes = angular.copy($scope.equipmentTimersTotalMinutes);
                $scope.equipmentTimersTotalMinutes = TimesheetsService.convertMinutesToHoursMinutes($scope.equipmentTimersTotalMinutes);
    
                $scope.jobTimersTotalMinutes = data.jobTimers.reduce(function(a, b) { return a + b.totalMinutes; }, 0);
                $scope.masterJobTimersTotalMinutes = angular.copy($scope.jobTimersTotalMinutes);
                $scope.jobTimersTotalMinutes = TimesheetsService.convertMinutesToHoursMinutes($scope.jobTimersTotalMinutes);
    
                $scope.minutesLeftToAllocate = $scope.totalTimeCardMinutes - ($scope.masterJobTimersTotalMinutes + $scope.masterEquipmentTimersTotalMinutes);
                $scope.timeLeftToBeAllocated = TimesheetsService.convertMinutesToHoursMinutes($scope.minutesLeftToAllocate);
    
            }, function(error) {
                $scope.jobTimersGridOptions.data = [];
                $scope.equipmentTimersGridOptions.columnDefs = _equipmentTimersColDefs();
                $scope.equipmentTimersGridOptions.data = [];
                $scope.equipmentTimersGridOptions.columnDefs = _equipmentTimersColDefs();
            });
        }
    
        function _getDropDownLists() {
            $scope.dropDownLists = {};
    
            CategoriesHelper.getHadcoShopList()
            .then(function(response) {
                $scope.dropDownLists.categories = response;
            });
            EquipmentServiceTypesHelper.getList()
            .then(function(response) {
                $scope.dropDownLists.serviceTypes = response;
            });
            EquipmentHelper.getList()
            .then(function(response) {
                $scope.dropDownLists.equipmentList = response;
            });
        }
    
        function _getTotals() {
                //employeeDetailed controller
            var timeCardTotals = _.reduce(
                $scope.timers,
                function(num, timer) {
                    var totalTimecardMinutes = timer.totalMinutes;
                    num.totalTimecardMinutes += totalTimecardMinutes;
    
                    var totalOverheadMinutes = timer.travelMinutes;
                    num.totalOverheadMinutes += totalOverheadMinutes;
    
                    return num;
                }, { totalTimecardMinutes: 0, totalOverheadMinutes: 0 }
            );
    
            return timeCardTotals;
        }
    
    
        function _gridFooterTemplate() {
            return '<div class="gridFooter" ><span> <span class="ui-grid-footer-label">Total Equipment:</span> {{(grid.appScope.equipmentTimersTotalMinutes) || 0}}</span><span> <span class="ui-grid-footer-label">Total Shop:</span> {{(grid.appScope.jobTimersTotalMinutes) || 0}}</span><span><span class="ui-grid-footer-label">Total Drive Time:</span> {{(grid.appScope.totalOverheadTime) || 0}} </span><span> <span class="ui-grid-footer-label">Total Summary: </span> {{(grid.appScope.totalEquipmentTimersTime) || 0}}</span><span> <span class="ui-grid-footer-label">Time Left to Allocate: </span>{{(grid.appScope.timeLeftToBeAllocated) || 0}}</span> </div>';
        }
    
        function _jobAndEquipmentTimersColDefs() {
            var defaultCellTemplate = '<div class="ui-grid-cell-contents" ng-if="!row.entity.editable">{{COL_FIELD}}</div>';
            return [
                {
                    name: 'type', width: 100, pinnedLeft: true,
                    cellTemplate: '<div ng-if="row.entity.equipmentID">Equipment</div>' +
                                    '<div ng-if="row.entity.jobID">Hadco.Shop</div>',
                },
                {
                    name: 'Identifier', width: 150,
                    cellTemplate: '<div ng-if="row.entity.equipmentID">' +
                                    '<ui-grid-typeahead is-cell-editable="row.entity.editable"  row="row" col-field="row.entity.equipment.equipmentNumber" type="equipment" focus-id="equipmentInput" required="true"' +
                                    'typeahead-list="row.grid.appScope.dropDownLists.equipmentList" model-collection="equipment" model-identifier="equipmentNumber">' +
                                    '</ui-grid-typeahead>' +
                                    '</div>' +
                                    '<div ng-if="row.entity.jobID">' +
                                    '<ui-grid-typeahead required="true" row="row"  type="shop" is-cell-editable="row.entity.editable" col-field="row.entity.category.categoryNumber" focus-id="shopInput"' +
                                    'typeahead-list="row.grid.appScope.dropDownLists.categories" model-collection="category" model-identifier="categoryNumber">' +
                                    '</ui-grid-typeahead>' +
                                    '</div>',
                },
                {
                    name: 'Start Date', field: 'startTime', type: 'date', width: 110,
                    cellTemplate: '<ui-grid-dropdown row="row" type="equipment" is-cell-editable="row.entity.editable"  col-field="COL_FIELD | date: \'MM/dd/yyyy\'" ' +
                    'dropdown-list="row.grid.appScope.dateList" model-collection="startDate" model-identifier="formattedDate" check-function="grid.appScope.syncDateAndTime">' +
                        '</ui-grid-dropdown>',
                },
                {
                    name: 'Start Time', field: 'startTime',
                    cellTemplate: '<ui-grid-time-cell required="true" row="row"  is-cell-editable="row.entity.editable" type="equipment" col-field-key="startTime"></ui-grid-time-cell>',
                    width: 110,
                },
                {
                    name: 'End Date', field: 'stopTime', type: 'date',
                    width: 120, cellTemplate: '<ui-grid-dropdown row="row"  is-cell-editable="row.entity.editable" type="equipment" col-field="COL_FIELD | date: \'MM/dd/yyyy\'" ' +
                    'dropdown-list="row.grid.appScope.dateList" model-collection="stopDate" model-identifier="formattedDate" check-function="grid.appScope.syncDateAndTime">' +
                        '</ui-grid-dropdown>',
                },
                {
                    name: 'End Time', field: 'stopTime',
                    cellTemplate: '<ui-grid-time-cell required="true"  row="row" is-cell-editable="row.entity.editable" type="equipment" col-field-key="stopTime"></ui-grid-time-cell>',
                    width: 110,
                },
                {
                    name: 'Total Hours', field: 'totalMinutes', cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.formattedTotalMinutes}}</div>', aggregationType: uiGridConstants.aggregationTypes.sum,
                    width: 120,
                    footerCellTemplate:
                        '<div ng-class="grid.appScope.warningClassSetup(col.getAggregationValue(), \'Equipment\') ? \'warning error\' : \' \'" class="ui-grid-cell-contents" col-index="renderIndex"> ' +
                        '<div> {{ col.getAggregationText() + grid.appScope.convertMinutesToHoursMinutes(col.getAggregationValue()) }} </div></div>',
                },
                {
                    displayName: 'RSPD', field: 'equipmentServiceType.name', width: 100,
                    cellTemplate: '<ui-grid-typeahead row="row" required="true"  is-cell-editable="row.entity.editable && row.entity.equipmentID" col-field="COL_FIELD" type="equipment"' +
                        'typeahead-list="grid.appScope.dropDownLists.serviceTypes" model-collection="equipmentServiceType" model-identifier="name">' +
                        '</ui-grid-typeahead>',
                },
                {
                    name: 'Notes / work Performed', field: 'diary', minWidth: 200,
                    cellTemplate: '<ui-grid-text-box row="row" type="equipment"  col-field="COL_FIELD" is-cell-editable="row.entity.editable" col-field-key="diary"></ui-grid-text-box>',
                },
                {
                    name: 'closed', field: 'closed', width: 100,
                    cellTemplate: '<ui-grid-typeahead required="true" is-cell-editable="row.entity.editable && row.entity.equipmentID"  row="row" col-field="COL_FIELD" check-function="grid.appScope.assignClosedOpen" ' +
                        'typeahead-list="[{ name: \'true\' }, { name: \'false\' }]" model-collection="closed" type="equipment" model-identifier="name">' +
                        '</ui-grid-typeahead>',
                },
                {
                    name: 'Edit', width: 100, pinnedRight: true,
                    cellTemplate: 
                        '<div ng-if="row.entity.equipmentID">' + 
                            '<a ng-disabled="(!grid.appScope.canEditRow || grid.appScope.timersDisabled) && grid.appScope.allocationError" data-toggle="tooltip" title="Allocation Error" class="fa fa-lg fa-flag icon"' +
                                'ng-class="{ \'hide\': row.entity.editable || !grid.appScope.allocationError }"' +
                                'ng-click="!grid.appScope.canEditRow || grid.appScope.beginEdit(row)">' +
                            '</a>' +
                            '<i ng-disabled="(!grid.appScope.canEditRow || grid.appScope.timersDisabled)" data-toggle="tooltip" title="Edit" class="fa fa-lg fa-pencil icon"' +
                                'ng-class="{ \'hide\': row.entity.editable || (row.entity.loadTimerEntryID || row.entity.downtimeTimerID) }"' +
                                'ng-click="!grid.appScope.canEditRow || grid.appScope.beginEdit(row, \'equipment\')" class="fa fa-lg fa-pencil icon" data-toggle="tooltip" title="Edit" aria-hidden="true">' +
                            '</i>' +
                            '<i ng-disabled="(!grid.appScope.canEditRow || grid.appScope.timersDisabled)" data-toggle="tooltip" title="Delete" class="fa fa-lg fa-trash icon"' +
                                'ng-class="{ \'hide\': row.entity.editable }"' +
                                'ng-click="!grid.appScope.canEditRow || grid.appScope.deleteTimer(row.entity, row.entity.sequence, grid.appScope.shopTimerTypes.equipment)">' +
                            '</i>' +
                            '<i class="fa fa-lg fa-floppy-o icon" data-toggle="tooltip" title="Save"' +
                                'ng-disabled="grid.appScope.isInvalid(row.entity, grid.appScope.shopTimerTypes.equipment) || grid.appScope.timersDisabled"' +
                                'ng-click="grid.appScope.isInvalid(row.entity, grid.appScope.shopTimerTypes.equipment) || grid.appScope.saveTimer(row.entity, rowRenderIndex, \'equipment\')"' +
                                'ng-class="{ \'hide\': !row.entity.editable }">' +
                            '</i>' +
                            '<i class="fa fa-lg fa-ban icon" data-toggle="tooltip" title="Cancel" ng-click="grid.appScope.cancelEdit(row.entity, grid.appScope.shopTimerTypes.equipment)" ng-class="{ \'hide\': !row.entity.editable }"></i>' +
                        '</div>' +
    
                        '<div ng-if="(row.entity.jobID && row.entity.category.categoryNumber !== \'DOWN\')">' +
                            '<a ng-disabled="(!grid.appScope.canEditRow || grid.appScope.timersDisabled) && grid.appScope.allocationError" data-toggle="tooltip" title="Allocation Error" class="fa fa-lg fa-flag icon"' +
                                'ng-class="{ \'hide\': row.entity.editable || !grid.appScope.allocationError }"' +
                                'ng-click="!grid.appScope.canEditRow || grid.appScope.beginEdit(row)">' +
                            '</a>' +
                            '<i ng-disabled="(!grid.appScope.canEditRow || grid.appScope.timersDisabled)" data-toggle="tooltip" title="Edit" class="fa fa-lg fa-pencil icon"' +
                                'ng-class="{ \'hide\': row.entity.editable || (row.entity.loadTimerEntryID || row.entity.downtimeTimerID) }"' +
                                'ng-click="!grid.appScope.canEditRow || grid.appScope.beginEdit(row, \'shop\')" class="fa fa-lg fa-pencil icon" data-toggle="tooltip" title="Edit" aria-hidden="true">' +
                            '</i>' +
                            '<i ng-disabled="(!grid.appScope.canEditRow || grid.appScope.timersDisabled)" data-toggle="tooltip" title="Delete" class="fa fa-lg fa-trash icon"' +
                                'ng-class="{ \'hide\': row.entity.editable }"' +
                                'ng-click="!grid.appScope.canEditRow || grid.appScope.deleteTimer(row.entity, row.entity.sequence, grid.appScope.shopTimerTypes.shop)">' +
                            '</i>' +
                            '<i class="fa fa-lg fa-floppy-o icon" data-toggle="tooltip" title="Save"' +
                                'ng-disabled="grid.appScope.isInvalid(row.entity, grid.appScope.shopTimerTypes.shop) || grid.appScope.timersDisabled"' +
                                'ng-click="grid.appScope.isInvalid(row.entity, grid.appScope.shopTimerTypes.shop) || grid.appScope.saveTimer(row.entity, rowRenderIndex, \'shop\')"' +
                                'ng-class="{ \'hide\': !row.entity.editable }">' +
                            '</i>' +
                            '<i class="fa fa-lg fa-ban icon" data-toggle="tooltip" title="Cancel" ng-click="grid.appScope.cancelEdit(row, grid.appScope.shopTimerTypes.shop)" ng-class="{ \'hide\': !row.entity.editable }"></i>' +
                        '</div>',
                    },
            ];
        }
    
        function _equipmentTimersColDefs() {
            var defaultCellTemplate = '<div class="ui-grid-cell-contents" ng-if="!row.entity.editable">{{COL_FIELD}}</div>';
            return [
                {
                    name: 'equipment', field: 'equipment.equipmentNumber', width: 150,
                    cellTemplate: '<ui-grid-typeahead is-cell-editable="row.entity.editable"  row="row" col-field="COL_FIELD" type="equipment" focus-id="equipmentInput" required="true"' +
                    'typeahead-list="row.grid.appScope.dropDownLists.equipmentList" model-collection="equipment" model-identifier="equipmentNumber">' +
                    '</ui-grid-typeahead>',
                },
                {
                    name: 'Start Date', field: 'startTime', type: 'date', width: 110,
                    cellTemplate: '<ui-grid-dropdown row="row" type="equipment" is-cell-editable="row.entity.editable"  col-field="COL_FIELD | date: \'MM/dd/yyyy\'" ' +
                    'dropdown-list="row.grid.appScope.dateList" model-collection="startDate" model-identifier="formattedDate" check-function="grid.appScope.syncDateAndTime">' +
                        '</ui-grid-dropdown>',
                },
                {
                    name: 'Start Time', field: 'startTime',
                    cellTemplate: '<ui-grid-time-cell required="true" row="row"  is-cell-editable="row.entity.editable" type="equipment" col-field-key="startTime"></ui-grid-time-cell>',
                    width: 110,
                },
                {
                    name: 'End Date', field: 'stopTime', type: 'date',
                    width: 120, cellTemplate: '<ui-grid-dropdown row="row"  is-cell-editable="row.entity.editable" type="equipment" col-field="COL_FIELD | date: \'MM/dd/yyyy\'" ' +
                    'dropdown-list="row.grid.appScope.dateList" model-collection="stopDate" model-identifier="formattedDate" check-function="grid.appScope.syncDateAndTime">' +
                        '</ui-grid-dropdown>',
                },
                {
                    name: 'End Time', field: 'stopTime',
                    cellTemplate: '<ui-grid-time-cell required="true"  row="row" is-cell-editable="row.entity.editable" type="equipment" col-field-key="stopTime"></ui-grid-time-cell>',
                    width: 110,
                },
                {
                    name: 'Total Hours', field: 'totalMinutes', cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.formattedTotalMinutes}}</div>', aggregationType: uiGridConstants.aggregationTypes.sum,
                    footerCellTemplate:
                        '<div ng-class="grid.appScope.warningClassSetup(col.getAggregationValue(), \'Equipment\') ? \'warning error\' : \' \'" class="ui-grid-cell-contents" col-index="renderIndex"> ' +
                        '<div> {{ col.getAggregationText() + grid.appScope.convertMinutesToHoursMinutes(col.getAggregationValue()) }} </div></div>', width: 120,
                },
                {
                    displayName: 'RSPD', field: 'equipmentServiceType.name', width: 100,
                    cellTemplate: '<ui-grid-typeahead row="row" required="true"  is-cell-editable="row.entity.editable" col-field="COL_FIELD" type="equipment"' +
                        'typeahead-list="grid.appScope.dropDownLists.serviceTypes" model-collection="equipmentServiceType" model-identifier="name">' +
                        '</ui-grid-typeahead>',
                },
                {
                    name: 'work Performed', field: 'diary', minWidth: 200,
                    cellTemplate: '<ui-grid-text-box row="row" type="equipment"  col-field="COL_FIELD" is-cell-editable="row.entity.editable" col-field-key="diary"></ui-grid-text-box>',
                },
                {
                    name: 'closed', field: 'closed', width: 100,
                    cellTemplate: '<ui-grid-typeahead required="true" is-cell-editable="row.entity.editable"  row="row" col-field="COL_FIELD" check-function="grid.appScope.assignClosedOpen" ' +
                        'typeahead-list="[{ name: \'true\' }, { name: \'false\' }]" model-collection="closed" type="equipment" model-identifier="name">' +
                        '</ui-grid-typeahead>',
                },
                {
                    name: 'Edit', width: 100, pinnedRight: true,
                    cellTemplate: '<div>' +
                            '<a ng-disabled="(!grid.appScope.canEditRow || grid.appScope.timersDisabled) && grid.appScope.allocationError" data-toggle="tooltip" title="Allocation Error" class="fa fa-lg fa-flag icon"' +
                                'ng-class="{ \'hide\': row.entity.editable || !grid.appScope.allocationError }"' +
                                'ng-click="!grid.appScope.canEditRow || grid.appScope.beginEdit(row)">' +
                            '</a>' +
                            '<i ng-disabled="!grid.appScope.canEditRow"' +
                                'ng-class="{ \'hide\': row.entity.editable || (row.entity.loadTimerEntryID || row.entity.downtimeTimerID) }"' +
                                'ng-click="!grid.appScope.canEditRow || grid.appScope.beginEdit(row, \'equipment\')" class="fa fa-lg fa-pencil icon" data-toggle="tooltip" title="Edit" aria-hidden="true">' +
                            '</i>' +
                            '<i ng-disabled="!grid.appScope.canEditRow" data-toggle="tooltip" title="Delete" class="fa fa-lg fa-trash icon"' +
                                'ng-class="{ \'hide\': row.entity.editable }"' +
                                'ng-click="!grid.appScope.canEditRow || grid.appScope.deleteTimer(row.entity, row.entity.sequence, grid.appScope.shopTimerTypes.equipment)">' +
                            '</i>' +
                            '<i class="fa fa-lg fa-floppy-o icon" data-toggle="tooltip" title="Save"' +
                                'ng-disabled="grid.appScope.isInvalid(row.entity, grid.appScope.shopTimerTypes.equipment)"' +
                                'ng-click="grid.appScope.isInvalid(row.entity, grid.appScope.shopTimerTypes.equipment) || grid.appScope.saveTimer(row.entity, rowRenderIndex, \'equipment\')"' +
                                'ng-class="{ \'hide\': !row.entity.editable }">' +
                            '</i>' +
                            '<i class="fa fa-lg fa-ban icon" data-toggle="tooltip" title="Cancel" ng-click="grid.appScope.cancelEdit(row, grid.appScope.shopTimerTypes.equipment)" ng-class="{ \'hide\': !row.entity.editable }"></i>' +
                            '</div>',
                    },
            ];
        }
    
        function _jobTimersColDefs() {
            var defaultCellTemplate = '<div class="ui-grid-cell-contents" ng-if="!row.entity.editable">{{COL_FIELD}}</div>';
            return [
                {
                    displayName: 'HADCO.SHOP', field: 'category.categoryNumber', width: 100,
                    cellTemplate: '<ui-grid-typeahead required="true" row="row"  type="shop" is-cell-editable="row.entity.editable" col-field="COL_FIELD" focus-id="shopInput"' +
                    'typeahead-list="row.grid.appScope.dropDownLists.categories" model-collection="category" model-identifier="categoryNumber">' +
                    '</ui-grid-typeahead>',
                    },
                {
                    name: 'Start Date', field: 'startTime', type: 'date', cellFilter: 'date:"yyyy-MM-dd"',
                    width: 120, cellTemplate: '<ui-grid-dropdown row="row"  is-cell-editable="row.entity.editable" type="shop" col-field="COL_FIELD | date: \'MM/dd/yyyy\'" ' +
                    'dropdown-list="row.grid.appScope.dateList" model-collection="startDate" model-identifier="formattedDate" check-function="grid.appScope.syncDateAndTime">' +
                        '</ui-grid-dropdown>',
                },
                {
                    name: 'Start Time', field: 'startTime', type: 'date',
                    cellTemplate: '<ui-grid-time-cell required="true" row="row" is-cell-editable="row.entity.editable"  type="shop" col-field-key="startTime"></ui-grid-time-cell>',
                    width: 110,
                },
                {
                    name: 'End Date', field: 'stopTime', type: 'date', cellFilter: 'date:"yyyy-MM-dd"',
                    width: 120, cellTemplate: '<ui-grid-dropdown row="row"  is-cell-editable="row.entity.editable" type="shop" col-field="COL_FIELD | date: \'MM/dd/yyyy\'" ' +
                    'dropdown-list="row.grid.appScope.dateList" model-collection="stopDate" model-identifier="formattedDate" check-function="grid.appScope.syncDateAndTime">' +
                        '</ui-grid-dropdown>',
                },
                {
                    name: 'End Time', field: 'stopTime', type: 'date', cellFilter: 'date:"h:mm a"',
                    cellTemplate: '<ui-grid-time-cell required="true" row="row" is-cell-editable="row.entity.editable"  type="shop" col-field-key="stopTime"></ui-grid-time-cell>',
                    width: 110,
                },
                {
                    name: 'Total Hours', field: 'totalMinutes', cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.formattedTotalMinutes}}</div>',
                    aggregationType: uiGridConstants.aggregationTypes.sum,
                    footerCellTemplate:
                        '<div ng-class="grid.appScope.warningClassSetup(col.getAggregationValue(), \'Shop\') ? \'warning error\' : \' \'" class="ui-grid-cell-contents" col-index="renderIndex"> ' +
                        '<div> {{ col.getAggregationText() + grid.appScope.convertMinutesToHoursMinutes(col.getAggregationValue()) }} </div></div>', width: 120,
                },
                { name: 'notes', field: 'diary', minWidth: 200, cellTemplate: '<ui-grid-text-box row="row" type="shop"  col-field="COL_FIELD" is-cell-editable="row.entity.editable" col-field-key="diary"></ui-grid-text-box>' },
                {
                    name: 'Edit', field: 'jobTimer', pinnedRight: true, width: 100,
                    cellTemplate: '<div ng-hide="row.entity.category.categoryNumber == \'DOWN\'">' +
                            '<a ng-disabled="(!grid.appScope.canEditRow || grid.appScope.timersDisabled) && grid.appScope.allocationError" data-toggle="tooltip" title="Allocation Error" class="fa fa-lg fa-flag icon"' +
                                'ng-class="{ \'hide\': row.entity.editable || !grid.appScope.allocationError }"' +
                                'ng-click="!grid.appScope.canEditRow || grid.appScope.beginEdit(row)">' +
                            '</a>' +
                            '<i ng-disabled="!grid.appScope.canEditRow"' +
                                'ng-class="{ \'hide\': row.entity.editable || (row.entity.loadTimerEntryID || row.entity.downtimeTimerID) }"' +
                                'ng-click="!grid.appScope.canEditRow || grid.appScope.beginEdit(row, \'shop\')" class="fa fa-lg fa-pencil icon" data-toggle="tooltip" title="Edit" aria-hidden="true">' +
                            '</i>' +
                            '<i ng-disabled="!grid.appScope.canEditRow"  data-toggle="tooltip" title="Delete" class="fa fa-lg fa-trash icon"' +
                                'ng-class="{ \'hide\': row.entity.editable }"' +
                                'ng-click="!grid.appScope.canEditRow || grid.appScope.deleteTimer(row.entity, row.entity.sequence, grid.appScope.shopTimerTypes.shop)">' +
                            '</i>' +
                            '<i class="fa fa-lg fa-floppy-o icon" data-toggle="tooltip" title="Save"' +
                                'ng-disabled="grid.appScope.isInvalid(row.entity, grid.appScope.shopTimerTypes.shop)"' +
                                'ng-click="grid.appScope.isInvalid(row.entity, grid.appScope.shopTimerTypes.shop) || grid.appScope.saveTimer(row.entity, rowRenderIndex, \'shop\')"' +
                                'ng-class="{ \'hide\': !row.entity.editable }">' +
                            '</i>' +
                            '<i class="fa fa-lg fa-ban icon" data-toggle="tooltip" title="Cancel" ng-click="grid.appScope.cancelEdit(row, grid.appScope.shopTimerTypes.shop)" ng-class="{ \'hide\': !row.entity.editable }"></i>' +
                            '</div>',
                },
    
            ];
        }
    
        function _checkTimes(timer) {
            if (moment(timer.startTime).format('YYYY') !== moment(new Date($routeParams.dayId)).format('YYYY')) {
                var date = $routeParams.dayId + ' ' + moment(timer.startTime).format('h:mm A');
                timer.startTime = moment(new Date(date));
            }
            if (moment(timer.stopTime).format('YYYY') !== moment(new Date($routeParams.dayId)).format('YYYY')) {
                var date = $routeParams.dayId + ' ' + moment(timer.stopTime).format('h:mm A');
                timer.stopTime = moment(new Date(date));
            }
    
            return timer;
        }
    
    
        function _setAndFormatDefaultTime(time) {
            return moment($routeParams.dayId, "MM/DD/YYYY").set({
                hours: moment(time).hours(),
                minutes: moment(time).minutes(),
            });
        }
    
        function _deleteEquipmentTimer(timer, index) {
            if (!timer.equipmentTimerID) {
                $scope.equipmentTimersGridOptions.data.splice(index, 1);
            }
            else {
                var modalInstance = $modal.open({
                    template: deleteTemplate,
                    controller: 'DeleteModalContentController',
                    windowClass: 'default-modal',
                    resolve: {
                        deletedItemName: function() {
                            return timer.equipment.equipmentNumber;
                        },
                    },
                });
    
                modalInstance.result.then(function(data) {
                    EquipmentTimers.one(timer.equipmentTimerID)
                        .remove()
                        .then(function (response) {
                            $scope.jobAndEquipmentTimersGridOptions.data.splice(index, 1);
                            
                            $scope.equipmentTimersGridOptions.data.splice(index, 1);
                            $scope.masterEquipmentTimers.splice(index, 1);
                            angular.forEach($scope.jobAndEquipmentTimersGridOptions.data, function(row, index) {
                                row.sequence = index;
                            });
                            
                            _updateEquipmentTimerTotals()
                            
                            $scope.jobAndEquipmentGridApi.core.refresh();
                        });
                });
            }
        }
    
    
        function _deleteJobTimer(timer, index) {
            if (!timer.jobTimerID) {
                $scope.jobTimers.splice(index, 1);
            }
            else {
                var modalInstance = $modal.open({
                    template: deleteTemplate,
                    controller: 'DeleteModalContentController',
                    windowClass: 'default-modal',
                    resolve: {
                        deletedItemName: function () {
                            return timer.category.name
                        },
                    },
                });
    
                modalInstance.result.then(function(data) {
                    JobTimers.one(timer.jobTimerID)
                        .remove()
                        .then(function (response) {
                            $scope.jobAndEquipmentTimersGridOptions.data.splice(index, 1);
                            $scope.jobTimersGridOptions.data.splice(index, 1);
                            $scope.masterJobTimers.splice(index, 1);
                            angular.forEach($scope.jobAndEquipmentTimersGridOptions.data, function(row, index) {
                                row.sequence = index;
                            });
                            
                            _updateJobTimerTotals()

                            $scope.jobAndEquipmentGridApi.core.refresh();
                        }, error => {
                            console.log(`ERROR: Unable to Delete Timer.  ${error.message}`)
                        });
                });
            }
        }

        function _updateEquipmentTimerTotals() {
            $scope.masterEquipmentTimersTotalMinutes = $scope.jobAndEquipmentTimersGridOptions.data
            .filter(timer => {
                if(timer.equipmentID)
                    return true
            })
            .map(timer => {
                return moment(timer.stopTime).diff(moment(timer.startTime), 'minutes')
            }).reduce(function (sum, time) {
                return sum += time
            })

            $scope.timeLeftToBeAllocated = TimesheetsService.convertMinutesToHoursMinutes($scope.totalTimeCardMinutes - ($scope.masterJobTimersTotalMinutes + $scope.masterEquipmentTimersTotalMinutes));
            $scope.equipmentTimersTotalMinutes = TimesheetsService.convertMinutesToHoursMinutes($scope.masterEquipmentTimersTotalMinutes);
        }
    
        function _saveEquipmentTimer(timer, index) {
            var defer = $q.defer();
            timer = _checkTimes(timer);
    
            var newTimer = {
                timesheetID: $scope.timesheet.timesheetID,
                diary: timer.diary,
                equipmentServiceTypeID: timer.equipmentServiceType.equipmentServiceTypeID,
                equipmentID: timer.equipment.equipmentID,
                startTime: moment(timer.startTime).format(),
                stopTime: moment(timer.stopTime).format(),
                closed: timer.closed,
            };
    
            if (timer.equipmentTimerID) {
                EquipmentTimers.one(timer.equipmentTimerID).patch(newTimer)
                .then(function(data) {
                    _organizeUpdatedTimer(timer, data, index);
                    angular.copy(timer, $scope.masterEquipmentTimers[index]);
                    NotificationFactory.success("Success: Equipment Timer Saved");
                    defer.resolve();
                    _updateEquipmentTimerTotals()
                }, function(error) {
                    console.log(error.data.message);
                    timer = angular.copy($scope.masterEquipmentTimers)[index];
                    NotificationFactory.error("Error: Fix overlapping equipment timers");
                    defer.reject();
                });
            }
            else {
                EquipmentTimers.post(newTimer)
                    .then(function(data) {
                        timer.equipmentTimerID = data.equipmentTimerID;
                        _organizeUpdatedTimer(timer, data, index);
                        angular.copy(timer, $scope.masterEquipmentTimers[index]);
                        NotificationFactory.success("Success: Equipment Timer Saved");
                        defer.resolve();
                        _updateEquipmentTimerTotals()
                    }, function(error) {
                        console.log(error.data.message);
                        timer = angular.copy($scope.masterEquipmentTimers)[index];
                        NotificationFactory.error("Error: Fix overlapping equipment timers");
                        defer.reject();
                    });
            }
            return defer.promise;
        }

        function _updateJobTimerTotals() {
            $scope.masterJobTimersTotalMinutes = $scope.jobAndEquipmentTimersGridOptions.data
            .filter(timer => {
                if(timer.jobTimerID) {
                    return true
                }
            })
            .map(timer => {
                return moment(timer.stopTime).diff(moment(timer.startTime), 'minutes')
            }).reduce(function (sum, time) {
                return sum += time
            })

            $scope.timeLeftToBeAllocated = TimesheetsService.convertMinutesToHoursMinutes($scope.totalTimeCardMinutes - ($scope.masterJobTimersTotalMinutes + $scope.masterEquipmentTimersTotalMinutes));
            $scope.jobTimersTotalMinutes = TimesheetsService.convertMinutesToHoursMinutes($scope.masterJobTimersTotalMinutes);
        }
    
        function _saveJobTimer(timer, index, type?) {
            var defer = $q.defer();
            timer = _checkTimes(timer);
            var newJob = {
                timesheetID: $scope.timesheet.timesheetID,
                startTime: moment(timer.startTime).format(),
                stopTime: moment(timer.stopTime).format(),
                jobID: timer.category.jobID,
                phaseID: timer.category.phaseID,
                categoryID: timer.category.categoryID,
                diary: timer.diary,
            };
    
            if (timer.jobTimerID) {
                JobTimers.one(timer.jobTimerID).patch(newJob)
                .then(function(data) {
                    _organizeUpdatedTimer(timer, data, index);
                    angular.copy(timer, $scope.masterJobTimers[index]);
                    NotificationFactory.success("Success: Job Timer Saved");
                    defer.resolve();
                    _updateJobTimerTotals()
                }, function(error) {
                    console.log(error.data.message);
                    timer = angular.copy($scope.masterJobTimers[index]);
                    NotificationFactory.error("Error: Fix overlapping shop timers");
                    defer.reject();
                });
            }
            else {
                JobTimers.post(newJob)
                    .then(function(data) {
                        timer.jobTimerID = data.jobTimerID;
                        _organizeUpdatedTimer(timer, data, index);
                        angular.copy(timer, $scope.masterJobTimers[index]);
                        NotificationFactory.success("Success: Job Timer Saved");
                        defer.resolve();
                        _updateJobTimerTotals()
                    }, function(error) {
                        console.log(error.data.message);
                        timer = angular.copy($scope.masterJobTimers[index]);
                        NotificationFactory.error("Error: Fix overlapping shop timers");
                        defer.reject();
                    });
            }
            return defer.promise;
    
        }
    
        function _organizeUpdatedTimer(timer, data, index) {
            timer.startTime = data.startTime;
            timer.stopTime = data.stopTime;
            timer.totalMinutes = data.totalMinutes;
            timer.formattedTotalMinutes = TimesheetsService.convertMinutesToHoursMinutes(timer.totalMinutes);
            timer.editable = false;
            $scope.canEditRow = true;
            $scope.$emit("can-edit-row");
            $scope.jobAndEquipmentTimersGridTableHeight = _getTableHeight($scope.jobAndEquipmentTimersGridOptions.data.length, $scope.jobAndEquipmentGridApi.grid.footerHeight);
            $scope.jobAndEquipmentGridApi.core.refresh();
    
        }
    
        function _getTableHeight(dataLength, footerHeight?) {
            if (dataLength < 3) {
                dataLength = 3;
            }
            var rowHeight = 30;
            var headerHeight = 30;
            var scrollbarHeight = 15;
            dataLength = dataLength || 1;
            footerHeight = footerHeight || 0;
            return {
                height: (dataLength * rowHeight + headerHeight + scrollbarHeight + footerHeight) * 1.25 + "px",
            };
        }
    
    }
}