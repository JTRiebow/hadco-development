import * as angular from 'angular';
import * as moment from 'moment';
import * as _ from 'lodash';

import * as template from './detailed-trucking.html';
import * as deleteTemplate from '../../Shared/modal-templates/delete-item-modal.html';
import * as cancelTemplate from '../../Shared/modal-templates/cancel-modal.html';

angular
    .module('detailedApprovalModule')
    .directive('detailedTrucking', detailedTrucking);

function detailedTrucking($window) {
    detailedTruckingController.$inject = [
        '$scope',
        '$route',
        '$modal',
        '$routeParams',
        '$q',
        '$timeout',
        'TimesheetsService',
        'Jobs',
        'LoadTimers',
        'LoadTimerEntries',
        'DowntimeTimers',
        'Users',
        'uiGridConstants',
        'uiGridTreeViewConstants',
        'uiGridTreeBaseConstants',
        'NotificationFactory',
    ];
    
    return {
        link: angular.noop,
        restrict: 'E',
        controller: detailedTruckingController,
        template,
        scope: {
            timers: '=',
        },
    };
    
    function detailedTruckingController(
        $scope,
        $route,
        $modal,
        $routeParams,
        $q,
        $timeout,
        TimesheetsService,
        Jobs,
        LoadTimers,
        LoadTimerEntries,
        DowntimeTimers,
        Users,
        uiGridConstants,
        uiGridTreeBaseConstants,
        uiGridTreeViewConstants,
        NotificationFactory,
    ) {

        init();

        function init() {
            $scope.timersDisabled = false;
            $scope.newJob = [];
            $scope.department = ($routeParams.departmentId == 8) ? { transport: true } : { trucking: true };
            $scope.truckingGridOptions = truckingGridOptions();
            _setColumnVisibilityByDepartment($scope.department);
            _getLoadTimers();
            _getDropDownLists();
            $scope.canEditRow = true;
            $scope.$on('disableTimers', (_, isDisabled) => {
                $scope.timersDisabled = isDisabled;
            });
        }


        function truckingGridOptions() {
            return {
                enableHorizontalScrollbar: 1,
                enableColumnMenus: false,
                showColumnFooter: true,
                columnFooterHeight: .5, //needs to display in order to aggregate on columns[10]. 
                showGridFooter: true,
                columnDefs: _colDefs(),
                showTreeExpandNoChildren: false,
                columnVirtualizationThreshold: 25,
                onRegisterApi: function(gridApi) {
                    $scope.gridApi = gridApi;
                    $scope.gridApi.core.on.rowsVisibleChanged($scope, function() {
                        $scope.gridTableHeight = _getTableHeight($scope.gridApi.grid.renderContainers.body.visibleRowCache.length);
                    });
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

        $scope.isInvalid = function(row) {
            var loadTimer = row.entity;
            //cycle through loadTimer children to check for and mark invalid or overlapping timers
            var isInvalid;
            _.forEach(row.treeNode.children, function(child) {
                if (_validateLoadTimerEntry(child.row.entity) || _checkOverLappingEntries(child.row.entity)) {
                    isInvalid = true;
                }
            });

            var isSplitEntry = row.entity.type.name != 'System DT' && row.entity['loadTimerEntryID'] == 'new';
            if (isSplitEntry) {
                _checkOverLappingSplitEntries();
            }

            if (isInvalid) {
                return true;
            }
            else if (loadTimer.type.name == $scope.timerTypes.downtime.name) {
                return _validatedowntimeTimerEntry(loadTimer);
            }
            else if ($scope.department.transport) {
                return _validateTransport(loadTimer);
            }
            else if ($scope.department.trucking) {
                return _validateTrucking(loadTimer);
            }
        };

        $scope.checkJob = function(item, index) {
            $scope.newJob[index] = {};
            $scope.selectedJob = item;
            $scope.truckingGridOptions.data[index].job = item;
            $scope.truckingGridOptions.data[index].phase = '';
            $scope.truckingGridOptions.data[index].category = '';
            Jobs.one(item.jobID).one('Phases').get()
            .then(function(data) {
                $scope.newJob[index]['phases'] = data.items;
            });
        };

        $scope.checkPhase = function(item, index) {
            $scope.phaseRow = index;
            $scope.truckingGridOptions.data[index].phase = item;
            Jobs.one($scope.selectedJob.jobID ? $scope.selectedJob.jobID : $scope.timesheet.loadTimers[index].jobID).one('Phases', item.phaseID).one('Categories').getList()
            .then(function(data) {
                $scope.newJob[index].categories = data;
            });
        };

        $scope.checkCell = function(item, index, type) {
            $scope.truckingGridOptions.data[index][type] = item;
        };

        $scope.cloneLoad = function(entity) {
            if ($scope.timersDisabled) {
                return;
            }
            var defaultStart = new Date(_setAndFormatDefaultTime($scope.earliestClockIn));
            var defaultStop = new Date(_setAndFormatDefaultTime($scope.lastClockOut));
            var newLoad = angular.copy(entity);
            newLoad.loadTime = defaultStart;
            newLoad.dumpTime = defaultStop;
            newLoad.loadTimerID = null;
            newLoad.ticketNumber = null;
            newLoad.tons = null;
            newLoad.downtimeTimers = [];
            newLoad.loadTimerEntries = [];
            newLoad.sequence = $scope.truckingGridOptions.data.length;
            newLoad.isNewItem = true;

            $scope.addLoad(entity.type, newLoad);
        };

        $scope.addLoad = function(type, newLoad) {
            var defaultStart = new Date(_setAndFormatDefaultTime($scope.earliestClockIn));
            var defaultStop = new Date(_setAndFormatDefaultTime($scope.lastClockOut));
            if (!newLoad) {
                if (type == $scope.timerTypes.downtime) {
                    newLoad = { type: $scope.timerTypes.downtime, startTime: defaultStart, endTime: defaultStop, downtimeTimerID: "new", $$treeLevel: 0, sequence: $scope.truckingGridOptions.data.length, isNewItem: true };
                    // newLoad = { type: $scope.timerTypes.downtime, $treeLevel: 0, sequence: $scope.truckingGridOptions.data.length, isNewItem: true, startTime: $scope.earliestClockIn, stopTime: $scope.lastClockOut, downtimeTimerID: "new" }
                }
                else if (type == $scope.timerTypes.load) {
                    newLoad = { type: $scope.timerTypes.load, loadTime: defaultStart, dumpTime: defaultStop, $$treeLevel: 0, sequence: $scope.truckingGridOptions.data.length, isNewItem: true };
                }
            }
            var index = $scope.truckingGridOptions.data.length;
            $scope.truckingGridOptions.data.push(newLoad);

            if (!$scope.timesheet) {
                $scope.timesheet = {
                    loadTimers: $scope.truckingGridOptions.data,
                };
            }

            $timeout(function() {
                var rowToEdit = $scope.gridApi.grid.rows[index];
                $scope.beginEdit(rowToEdit);
            });
        };

        $scope.addEntry = function(row, index) {
            if (row.loadTimerEntryID) {
                $scope.truckingGridOptions.data.splice(index + 1, 0, { loadTimerID: row.loadTimerID, loadTimerEntryID: 'new', startTime: row.startTime, endTime: row.endTime, editable: true, isChild: true, type: { name: "Entry" } });
            }
            else if (row.downtimeTimerID) {
                $scope.truckingGridOptions.data.splice(index + 1, 0, { loadTimerID: row.loadTimerID, downtimeTimerID: 'new', startTime: row.startTime, endTime: row.endTime, editable: true, isChild: true });
            }
            angular.forEach($scope.truckingGridOptions.data, function(row, index) {
                row.sequence = index;
            });
        };

        $scope.deleteLoad = function(index, row) {
            if ($scope.timersDisabled) {
                return;
            }
            var modalInstance = $modal.open({
                template: deleteTemplate,
                controller: 'DeleteModalContentController',
                windowClass: 'default-modal',
                resolve: {
                    deletedItemName: function() {
                        var name;
                        if (row.entity.loadTimerEntryID) {
                            name = "Load Timer Entry";
                        }
                        else if (row.entity.downtimeTimerID) {
                            name = "Downtime Timer";
                        }
                        else {
                            name = row.entity.loadTimerID;
                        }
                        return name;
                    },
                },
            });

            modalInstance.result.then(function() {
                _deleteLoad(index, row);
            });
        };

        function _deleteLoad(index, row) {
            var load = row.entity;
            var rowsToRemove = 1;

            if (load.loadTimerEntryID) {
                LoadTimerEntries
                .one(load.loadTimerEntryID)
                .remove()
                .then(function(repsonse) {
                    removeDeletedRows(index, rowsToRemove);
                });
            }
            else if (load.downtimeTimerID) {
                DowntimeTimers
                    .one(load.downtimeTimerID)
                    .remove()
                .then(function(response) {
                    removeDeletedRows(index, rowsToRemove);
                    var timesheetIndex = $scope.timesheet.downtimeTimers.map(function(downtime) { return downtime.downtimeTimerID; }).indexOf(load.downtimeTimerID);
                    if (timesheetIndex > -1) $scope.timesheet.downtimeTimers.splice(index, 1);
                    $scope.totalTimesheetDowntimeMinutes = _getDowntimeTotal($scope.timesheet.downtimeTimers);
                });
            }
            else if (load.loadTimerID) {
                rowsToRemove += row.treeNode.children.length;
                LoadTimers
                    .one(load.loadTimerID)
                    .remove()
                .then(function(response) {
                    removeDeletedRows(index, rowsToRemove);
                });
            }

        }

        function removeDeletedRows(index, rowsToRemove) {
            if (index > -1) { $scope.truckingGridOptions.data.splice(index, rowsToRemove); }
            $scope.masterLoadTimers.splice(index, rowsToRemove);
            angular.forEach($scope.truckingGridOptions.data, function(row, index) {
                row.sequence = index;
            });
        }

        $scope.saveLoad = function(row, shiftEnter) {
            var promises = [];

            if (row.treeNode.children.length) {
                angular.forEach(row.treeNode.children, function(value, key) {
                    value.row.entity.editable = false;
                    value.row.entity.invalidEndTime = false;
                    saveLoad(value.row.entity, value.row.entity.sequence)
                    .then(promises.push(value.row.entity.sequence));
                });

                var rowToUpdateIndex = $scope.truckingGridOptions.data.indexOf(row.entity);
                $scope.truckingGridOptions.data[rowToUpdateIndex] = row.entity;
                $scope.timerEntrySplitAdded = true;
            }

            $q.all(promises).then(function(data) {

                saveLoad(row.entity, row.entity.sequence).then(function(response) {
                    row.entity.editable = false;
                    $scope.canEditRow = true;
                    $scope.$emit("can-edit-row");
                    $scope.$emit("emit-updated-timers");
                    row.entity.loadTime = row.entity.loadTime ? new Date(response.loadTime) : null;
                    row.entity.dumpTime = row.entity.dumpTime ? new Date(response.dumpTime) : null;
                    row.entity.invalidDumpTime = false;

                    row.entity.totalMinutes = response.totalMinutes;
                    const column: any = _.find($scope.gridApi.grid.columns, { aggregationType: 2 });
                    if (column && column.updateAggregationValue) {
                        column.updateAggregationValue();
                    }

                    if (shiftEnter) {
                        $scope.rowToEdit = row.entity.sequence + row.treeNode.children.length + 1;
                    }
                });
            });
            $scope.gridApi.core.refresh();
            
        };

        $scope.warningClassSetup = function() {

            var totalLoadTimersMinutes = _.reduce($scope.truckingGridOptions.data, function(num, timer) {
                if (!timer.downtimeTimerID && !timer.isChild) {
                    num += timer.totalMinutes;
                }
                return num;
            }, 0);

            //totalTimersMinutes is based on $scope.gridApi.grid.columns[21].aggregationValue;	        
            $scope.totalTimersTime = TimesheetsService.convertMinutesToHoursMinutes(totalLoadTimersMinutes);
            var minutesLeftToBeAllocated = $scope.totalTimeCardMinutes - totalLoadTimersMinutes - $scope.totalTimesheetDowntimeMinutes;
            $scope.totalTimeCardTime = TimesheetsService.convertMinutesToHoursMinutes($scope.totalTimeCardMinutes);
            $scope.totalTimesheetDowntime = TimesheetsService.convertMinutesToHoursMinutes($scope.totalTimesheetDowntimeMinutes);
            $scope.timeLeftToBeAllocated = TimesheetsService.convertMinutesToHoursMinutes(minutesLeftToBeAllocated);

            $scope.showWarning = minutesLeftToBeAllocated < 0;
            return $scope.showWarning ? 'warning error' : '';
        };

        $scope.$on("timers-changed", function() {
            _getLoadTimers();
            _getDropDownLists();
            $scope.gridApi.core.refresh();

        });

        $scope.convertMinutesToHoursMinutes = function(minutes) {
            return TimesheetsService.convertMinutesToHoursMinutes(minutes);
        };

        $scope.beginEdit = function(row, event) {
            if ($scope.timersDisabled) {
                return;
            }
            if ($scope.canEditRow) {
                row.entity.isChild = false;
                $scope.backupEntity = angular.copy(row.entity);
                $scope.canEditRow = false;
                $scope.$emit("cannot-edit-row");
                row.entity.editable = true;
                $scope.backupChildEntities = [];
                angular.forEach(row.treeNode.children, function(value, key) {
                    value.row.entity.isChild = true;
                    $scope.backupChildEntities.push(angular.copy(value.row.entity));
                    value.row.entity.editable = true;
                });
                $scope.expandRow(row);
                $timeout(function() {
                    var focusID = (event && event.shiftKey) ? document.querySelector(".timepicker.grid input") : "#input" + row.entity.type.name;
                    angular.element(focusID).focus();
                    $(focusID).select();
                });

                $scope.rowToEdit = undefined;
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
                        $scope.saveLoad(row);
                    }
                }
                else {
                    $scope.cancelEdit(row);
                }
            });
        };

        $scope.cancelEdit = function(row) {
            $scope.canEditRow = true;
            $scope.$emit("can-edit-row");
            if (row.entity.isNewItem) {
                $scope.truckingGridOptions.data.pop();
                return;
            }
            if (row.entity.isChild) {
                row = row.treeNode.parentRow;
            }
            angular.copy($scope.backupEntity, row.entity);
            angular.forEach(row.treeNode.children, function(value, index) {
                if (value.row.entity.loadTimerEntryID == "new") {
                    $scope.truckingGridOptions.data.splice(value.row.entity.sequence, 1);
                }
                else {
                    var tmp = $scope.backupChildEntities.shift();
                    angular.copy(tmp, value.row.entity);
                }
            });
            angular.forEach($scope.truckingGridOptions.data, function(row, index) {
                row.sequence = index;
            });
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
                $scope.saveLoad(row, shiftEnter);
            }
        };

        $scope.getRowID = function(rowEntity) {
            var downtimeDisplay = rowEntity.downtimeReason ? rowEntity.downtimeReason.code + ' - ' + rowEntity.downtimeReason.description : null;
            var rowID = rowEntity.loadTimerEntryID || downtimeDisplay || rowEntity.loadTimerID;
            return rowID;
        };

        function saveLoad(load, index) {
            var deferred = $q.defer();

            if (load.type.name == $scope.timerTypes.entry.name) {
                _saveLoadTimerEntry(load, index);
                deferred.resolve();
            }
            else if (load.type == $scope.timerTypes.downtime) {
                _saveDowntimeTimer(load, index);
                deferred.resolve(load);
            }
            else {
                if (load.tons > 50) {
                    NotificationFactory.error("Error: Can't be more than 50 tons. ");
                    deferred.reject();
                    return deferred.promise;
                }
                var hasRequiredFields = load.truck && load.job && load.phase && load.category;
                if (!hasRequiredFields) {
                    NotificationFactory.error("Error: Missing a required field");
                    deferred.reject();
                    return deferred.promise;
                }

                var newLoad = {
                    timesheetID: $scope.timesheet.timesheetID,
                    truckID: load.truck.equipmentID,
                    trailerID: load.trailer ? load.trailer.equipmentID : "",
                    pupID: load.pup ? load.pup.equipmentID : "",
                    jobID: load.job.jobID,
                    phaseID: load.phase.phaseID,
                    categoryID: load.category.categoryID,
                    tons: load.tons,
                    loadEquipmentID: load.loadEquipment ? load.loadEquipment.equipmentID : "",
                    materialID: load.material ? load.material.materialID : "",
                    endLocation: load.endLocation,
                    startLocation: load.startLocation,
                    ticketNumber: load.ticketNumber || "",
                    billTypeID: load.billType ? load.billType.billTypeID : "",
                    invoiceNumber: load.invoiceNumber,
                    note: load.note,
                } as any;

                if (load.loadTimerID) {
                    LoadTimers.one(load.loadTimerID).patch(newLoad)
                        .then(function(data) {
                            NotificationFactory.success("Success: Load Saved");
                            deferred.resolve(data);
                        }, function(error) {
                            console.log(error.data.message);
                            $scope.truckingGridOptions.data[index] = angular.copy($scope.masterLoadTimers)[index];
                            NotificationFactory.error("Error: Fix overlapping timers.");
                            deferred.reject();
                        });
                }
                else {
                    newLoad.loadTimerEntries = [{ startTime: moment(load.loadTime).format(), endTime: moment(load.dumpTime).format() }];
                    LoadTimers.post(newLoad)
                        .then(function(data) {
                            //organization after new Load Timer and Load Timer Entry are created
                            load.loadTimerID = data.loadTimerID;

                            data.loadTimerEntries[0].sequence = index + 1;
                            data.loadTimerEntries[0].isChild = true;
                            $scope.truckingGridOptions.data.push(data.loadTimerEntries[0]);

                            $scope.masterLoadTimers.push($scope.truckingGridOptions.data[index], $scope.truckingGridOptions.data[index + 1]);
                            NotificationFactory.success("Success: Load Saved");
                            if (load.isNewItem === true) {
                                $route.reload();
                            }
                            deferred.resolve(data);
                        }, function(error) {
                            $scope.truckingGridOptions.data[index] = angular.copy($scope.masterLoadTimers)[index];
                            NotificationFactory.error("Error: Fix overlapping timers");
                            console.log(error.data.message);
                            deferred.reject();
                        });
                }
            }
            return deferred.promise;

        }

        function _getLoadTimers() {
            if ($scope.timesheet && !$scope.timerEntrySplitAdded) {
                _organizeData($scope.timesheet);
                _editRow($scope.rowToEdit);
            }
            else {
                TimesheetsService.getEmployeeTimesheet($routeParams.employeeId, $routeParams.dayId, $routeParams.departmentId)
                .then(function(data) {
                    $scope.timesheet = data;
                    _organizeData(data);
                    _editRow($scope.rowToEdit);
                });
            }
        }

        function _organizeData(data) {
            $scope.totalTimeCardMinutes = data.employeeTimers[data.employeeTimers.length - 1].totalMinutes;
            $scope.totalTimesheetDowntimeMinutes = _getDowntimeTotal(data);
            var loadAndDowntimeTimers = data.loadTimers.concat(data.downtimeTimers);
            var treeBaseLoadTimers = [];
            var rowIndex = 0;

            loadAndDowntimeTimers.sort(function(a, b) {
                var c = new Date(a.startTime || a.loadTime).getTime();
                var d = new Date(b.startTime || b.loadTime).getTime();
                return c - d;
            });
            
            if (loadAndDowntimeTimers.length === 0) {
                if (data.employeeTimers[0].employeeTimerEntries[0]) {
                    $scope.earliestClockIn = data.employeeTimers[0].employeeTimerEntries[0].clockIn;
                    $scope.lastClockOut = data.employeeTimers[0].employeeTimerEntries[0].clockOut;
                }
                else {
                    $scope.earliestClockIn = new Date();
                    $scope.lastClockOut = new Date();
                }
            }
            else {
                $scope.earliestClockIn = _.first<any>(loadAndDowntimeTimers).startTime || _.first<any>(loadAndDowntimeTimers).loadTime;
                $scope.lastClockOut = _.last<any>(loadAndDowntimeTimers).stopTime || _.last<any>(loadAndDowntimeTimers).dumpTime;
            }
            angular.forEach(loadAndDowntimeTimers, function(timer, index) {
                if (timer.downtimeTimerID) {
                    timer.startTime = new Date(timer.startTime);
                    timer.endTime = new Date(timer.stopTime || timer.endTime);
                }
                else {
                    var loadTimer = timer;
                    //create Date objects, check if dumpTime is valid. 
                    loadTimer.loadTime = new Date(loadTimer.loadTime);
                    timer.startTime = loadTimer.loadTime;
                    if (loadTimer.dumpTime) {
                        loadTimer.dumpTime = new Date(loadTimer.dumpTime);
                    }
                    else {
                        loadTimer.invalidDumpTime = true;
                        loadTimer.dumpTime = loadTimer.loadTime;
                    }
                    timer.endTime = loadTimer.dumpTime;

                }
                if (timer.downtimeTimerID) {
                    if (timer.systemGenerated) {
                        timer.type = $scope.timerTypes.systemDowntime;
                    }
                    else {
                        timer.type = $scope.timerTypes.downtime;
                    }
                }
                else {
                    timer.type = $scope.timerTypes.load;
                }
                timer.sequence = rowIndex;
                rowIndex++;
                timer.$$treeLevel = 0;
                treeBaseLoadTimers.push(timer);

                if (timer.loadTimerID && !timer.downtimeTimerID) {
                    var entriesAndDowntime = loadTimer.loadTimerEntries.concat(loadTimer.downtimeTimers);

                    entriesAndDowntime.sort(function(a, b) {
                        var c = new Date(a.startTime).getTime();
                        var d = new Date(b.startTime).getTime();
                        return c - d;
                    });

                    angular.forEach(entriesAndDowntime, function(loadTimerEntry, index) {
                        if (loadTimerEntry.downtimeTimerID) {
                            if (loadTimerEntry.systemGenerated) {
                                loadTimerEntry.type = $scope.timerTypes.systemDowntime;
                            }
                            else {
                                loadTimerEntry.type = $scope.timerTypes.downtime;
                            }
                        }
                        else {
                            loadTimerEntry.type = $scope.timerTypes.entry;
                        }
                        //check for invalid endTime. 
                        if (!loadTimerEntry.stopTime && !loadTimerEntry.endTime) {
                            loadTimerEntry.stopTime = loadTimerEntry.startTime;
                            loadTimerEntry.invalidEndTime = true;
                        }
                        //use endTime instead of stopTime for downTimeTimers and create date objects.
                        loadTimerEntry.endTime = loadTimerEntry.stopTime || loadTimerEntry.endTime;
                        loadTimerEntry.endTime = new Date(loadTimerEntry.stopTime || loadTimerEntry.endTime);
                        loadTimerEntry.startTime = new Date(loadTimerEntry.startTime);
                        loadTimerEntry.isChild = true;
                        loadTimerEntry.sequence = rowIndex;
                        rowIndex++;
                        treeBaseLoadTimers.push(loadTimerEntry);
                    });
                }
                
            });

            $scope.masterLoadTimers = angular.copy(treeBaseLoadTimers);
            $scope.truckingGridOptions.data = treeBaseLoadTimers;
        }
        
        /**
         * Expands and begins editing the row if rowToEdit is defined
         *
         * @param {number} rowToEdit
         */
        function _editRow(rowToEdit: number): void {
            if (rowToEdit && $scope.gridApi.grid.rows[rowToEdit]) {
                $timeout(() => {
                    var newRowToOpen = $scope.gridApi.grid.rows[rowToEdit];
                    $scope.expandRow(newRowToOpen);
                    $scope.beginEdit(newRowToOpen);
                });
            }
        }

        function _getDowntimeTotal(data) {
            if ($scope.truckingGridOptions.data.length > 0) {
                var downtimeTimerTotal = _.reduce(
                    $scope.truckingGridOptions.data,
                    function(num, timer) {
                        if (timer.downtimeTimerID) {
                            num += timer.totalMinutes;
                        }
                        return num;
                    }, 0
                );

                return downtimeTimerTotal;
            }
            else {
                var downtimeTimerTotalInDowntimeTimers = _.reduce(
                    data.downtimeTimers,
                    function(num, timer) {
                        num += timer.totalMinutes;
                        return num;
                    }, 0
                );

                var downtimeTimerTotalInLoadTimers = _.reduce(
                    data.loadTimers,
                    function(num, timer) {
                        _.each(timer.downtimeTimers, function(downtimeTimer) {
                            num += downtimeTimer.totalMinutes;
                        });
                        
                        return num;
                    }, 0
                );

                return downtimeTimerTotalInDowntimeTimers + downtimeTimerTotalInLoadTimers;
            }


        }

        function _getDropDownLists() {

            $scope.timerTypes = {
                load: { name: "Load" },
                downtime: { name: "DT" },
                entry: { name: "Entry" },
                systemDowntime: { name: "System DT" },
            };

            TimesheetsService.getDropDownLists().then(function(response) {
                $scope.dropDownLists = response;
                console.info("Timesheet service finished");
            });
        }

        function _getTotals() {
            //employeeDetailed controller
            var timeCardTotals = _.reduce(
                $scope.timers,
                function(num, timer) {
                    var totalTimeCardMinutes = timer.totalMinutes;
                    num.totalTimeCardMinutes += totalTimeCardMinutes;
                    return num;
                }, { totalTimeCardMinutes: 0 }
            );

            return timeCardTotals;
        }

        function _gridFooterTemplate() {
            return '<div class="gridFooter" ng-class="grid.appScope.warningClassSetup()" ><span><span class="ui-grid-footer-label">Total Time:</span> {{grid.appScope.totalTimeCardTime || 0}} </span><span><span class="ui-grid-footer-label">Total Load Time:</span> {{grid.appScope.totalTimersTime || 0}} </span><span><span class="ui-grid-footer-label">Total Downtime:</span> {{grid.appScope.totalTimesheetDowntime || 0}} </span><span> <span class="ui-grid-footer-label">Time Left to Allocate:</span> {{grid.appScope.timeLeftToBeAllocated || 0}}</span> </div>';
        }

        function _setColumnVisibilityByDepartment(department) {
            var columnsToHide;
            if (department.trucking) {
                columnsToHide = [ 'Equipment #' ];
            }
            else if (department.transport) {
                columnsToHide = [ 'Trailer', 'Pup', 'Ticket No.', 'Tons', 'Material', 'Bill Type' ];
            }
            _hideTruckingTransportColumns(columnsToHide);
        }


        function _hideTruckingTransportColumns(columnsToHide) {
            _.each($scope.truckingGridOptions.columnDefs, function(colDef) {
                colDef.visible = true;
                if (columnsToHide.indexOf(colDef.name) > -1) {
                    colDef.visible = false;
                }
            });
        }

        function _colDefs() {
            var defaultCellTemplate = '<div class="ui-grid-cell-contents" ng-if="!row.entity.editable">{{COL_FIELD}}</div>';
            return [
                {
                    field: 'type.name', name: 'Type', minWidth: 110, pinnedLeft: true,
                },
                {
                    field: 'loadTimerID', name: 'Number', minWidth: 110, pinnedLeft: true,
                    cellTemplate: _loadTypeColCellTemplate(),
                },
                {
                    field: 'truck.equipmentNumber', name: 'Truck', width: 80,
                    cellTemplate: '<ui-grid-typeahead row="row" required="true" is-cell-editable="row.entity.editable" is-cell-hidden="row.entity.isChild || row.entity.downtimeTimerID" focus-id="inputLoad" col-field="COL_FIELD" check-function="grid.appScope.checkCell"' +
                        'typeahead-list="row.grid.appScope.dropDownLists.trucks"  model-collection="truck" model-identifier="equipmentNumber">' +
                        '</ui-grid-typeahead>',
                },
                {
                    name: 'Trailer', field: 'trailer.equipmentNumber', width: 80,
                    cellTemplate: '<ui-grid-typeahead row="row" required="false" is-cell-editable="row.entity.editable" is-cell-hidden="row.entity.isChild || row.entity.downtimeTimerID" col-field="COL_FIELD" check-function="grid.appScope.checkCell"' +
                        'typeahead-list="grid.appScope.dropDownLists.trailers"  model-collection="trailer" model-identifier="equipmentNumber">' +
                        '</ui-grid-typeahead>',
                },
                {
                    name: 'Pup', field: 'pup.equipmentNumber', width: 70,
                    cellTemplate: '<ui-grid-typeahead row="row" is-cell-editable="row.entity.editable" is-cell-hidden="row.entity.isChild || row.entity.downtimeTimerID" col-field="COL_FIELD" check-function="grid.appScope.checkCell"' +
                        'typeahead-list="grid.appScope.dropDownLists.trailers" model-collection="pup" model-identifier="equipmentNumber">' +
                        '</ui-grid-typeahead>',
                },
                {
                    name: 'Load Date', field: 'loadTime', type: 'date', cellFilter: 'date:"yyyy-MM-dd"', width: 120,
                },
                {
                    name: 'Load Time', field: 'loadTime', type: 'date', width: 110, cellFilter: 'date:"h:mm a"',
                    cellTemplate: '<ui-grid-time-cell required="true" row="row" is-cell-hidden="row.entity.isChild || row.entity.downtimeTimerID" is-cell-editable="row.entity.editable && row.entity.isNewItem" col-field-key="loadTime"></ui-grid-time-cell>',
                },
                {
                    name: 'Dump Date', field: 'dumpTime', type: 'date', cellFilter: 'date:"yyyy-MM-dd"', width: 120,
                },
                {
                    name: 'Dump Time', field: 'dumpTime', type: 'date', cellFilter: 'date:"h:mm a"', width: 110,
                    cellTemplate: '<ui-grid-time-cell required="true" row="row" is-cell-hidden="row.entity.isChild || row.entity.downtimeTimerID || row.entity.invalidDumpTime" is-cell-editable="row.entity.editable && row.entity.isNewItem" col-field-key="dumpTime"></ui-grid-time-cell>',
                },
                {
                    name: 'Start Time', field: 'startTime', type: 'date', cellFilter: 'date:"h:mm a"', width: 110,
                    cellTemplate: '<ui-grid-time-cell  required="true" row="row" is-cell-editable="row.entity.editable && (row.entity.downtimeTimerID || row.entity.isChild)" col-field-key="startTime"></ui-grid-time-cell>',
                },
                {
                    name: 'Stop Time', field: 'endTime', type: 'date', cellFilter: 'date:"h:mm a"', width: 110,
                    cellTemplate: '<ui-grid-time-cell required="true" row="row" is-cell-hidden="row.entity.invalidEndTime && !row.entity.editable" is-cell-editable="row.entity.editable && (row.entity.downtimeTimerID || row.entity.isChild)" col-field-key="endTime"></ui-grid-time-cell>',
                },
                {
                    name: 'Start Location', field: 'startLocation', width: 150,
                    cellTemplate: '<ui-grid-text-box row="row" required="true" col-field="COL_FIELD" is-cell-editable="row.entity.editable" is-cell-hidden="row.entity.isChild || row.entity.downtimeTimerID" col-field-key="startLocation"></ui-grid-text-box>',
                },
                {
                    name: 'End Location', field: 'endLocation', width: 150,
                    cellTemplate: '<ui-grid-text-box row="row" required="true" col-field="COL_FIELD" is-cell-editable="row.entity.editable" is-cell-hidden="row.entity.isChild || row.entity.downtimeTimerID" col-field-key="endLocation"></ui-grid-text-box>',
                },
                {
                    name: 'Ticket No.', field: 'ticketNumber', width: 110,
                    cellTemplate: defaultCellTemplate +
                        '<div class="ui-grid-cell-contents" ng-if="!row.entity.isChild && row.entity.editable && !row.entity.downtimeTimerID">' +
                        '<input enter="grid.appScope.enterKeypressSave(row)" type="text" ng-model="MODEL_COL_FIELD" />' +
                        '</div>',
                },
                {
                    name: 'Tons', field: 'tons', type: 'number', width: 70,
                    cellTemplate: defaultCellTemplate +
                        '<div class="ui-grid-cell-contents" ng-if="!row.entity.isChild && row.entity.editable && !row.entity.downtimeTimerID">' +
                        '<input ng-required="true" enter="grid.appScope.enterKeypressSave(row)" type="number" ng-model="MODEL_COL_FIELD" />' +
                        '</div>',
                },
                {
                    name: 'Material', field: 'material.name', width: 100,
                    cellTemplate: '<ui-grid-typeahead row="row" required="true" is-cell-editable="row.entity.editable" is-cell-hidden="row.entity.isChild || row.entity.downtimeTimerID" col-field="COL_FIELD" check-function="grid.appScope.checkCell"' +
                        'typeahead-list="grid.appScope.dropDownLists.materials" model-collection="material" model-identifier="name">' +
                        '</ui-grid-typeahead>',
                },
                {
                    name: 'Equipment #', field: 'loadEquipment.equipmentNumber', width: 150,
                    cellTemplate: '<ui-grid-typeahead required="true" row="row" is-cell-editable="row.entity.editable" is-cell-hidden="row.entity.isChild || row.entity.downtimeTimerID" col-field="COL_FIELD" check-function="grid.appScope.checkCell"' +
                        'typeahead-list="grid.appScope.dropDownLists.equipmentList" model-collection="loadEquipment" model-identifier="equipmentNumber">' +
                        '</ui-grid-typeahead>',
                },
                {
                    name: 'Job', field: 'job.jobNumber', width: 130,
                    cellTemplate: '<ui-grid-typeahead required="true" row="row" col-field="COL_FIELD"is-cell-editable="row.entity.editable" is-cell-hidden="row.entity.isChild || row.entity.downtimeTimerID" check-function="grid.appScope.checkJob"' +
                        'typeahead-list="row.grid.appScope.dropDownLists.availableJobs" model-collection="job" model-identifier="jobNumber">' +
                        '</ui-grid-typeahead>',
                },
                {
                    name: 'Phase', field: 'phase.phaseNumber', type: 'string', width: 80,
                    cellTemplate: '<ui-grid-typeahead required="true" row="row" col-field="COL_FIELD" is-cell-editable="row.entity.editable" is-cell-hidden="row.entity.isChild || row.entity.downtimeTimerID" check-function="grid.appScope.checkPhase"' +
                        'typeahead-list="row.grid.appScope.newJob[row.entity.sequence].phases" model-collection="phase" model-identifier="phaseNumber">' +
                        '</ui-grid-typeahead>',
                },
                {
                    name: 'Category', field: 'category.categoryNumber', width: 100,
                    cellTemplate: '<ui-grid-typeahead required="true" row="row" col-field="COL_FIELD" is-cell-editable="row.entity.editable" is-cell-hidden="row.entity.isChild || row.entity.downtimeTimerID" check-function="grid.appScope.checkCell"' +
                        'typeahead-list="row.grid.appScope.newJob[row.entity.sequence].categories" model-collection="category" model-identifier="categoryNumber">' +
                        '</ui-grid-typeahead>',
                },
                {
                    name: 'Total Hours', field: 'totalMinutes', cellTemplate: '<ui-grid-hour-minute focus-id="timerInput" row="row" original-minutes="row.entity.totalMinutes" is-cell-editable="false" is-cell-hidden="!row.entity.downtimeTimerID && row.entity.isChild">' +
                                '</ui-grid-hour-minute>', width: 120, aggregationType: uiGridConstants.aggregationTypes.sum,
                    //footerCellTemplate: '<div ng-class="grid.appScope.warningClassSetup(col.getAggregationValue()) ? \'warning error\' : \' \'" class="ui-grid-cell-contents" col-index="renderIndex"> ' +
                    //    '<div> {{ col.getAggregationText() + grid.appScope.convertMinutesToHoursMinutes(col.getAggregationValue()) }} </div></div>'
                },
                {
                    name: 'Bill Type', field: 'billType.name', width: 110,
                    cellTemplate: '<ui-grid-typeahead required="true" row="row" is-cell-editable="row.entity.editable" is-cell-hidden="row.entity.isChild || row.entity.downtimeTimerID" col-field="COL_FIELD" check-function="grid.appScope.checkCell"' +
                        'typeahead-list="grid.appScope.dropDownLists.billTypes" model-collection="billType" model-identifier="name">' +
                        '</ui-grid-typeahead>',
                },
                {
                    name: 'Invoice #', field: 'invoiceNumber', width: 100,
                    cellTemplate: '<ui-grid-text-box row="row" col-field="COL_FIELD" is-cell-editable="row.entity.editable" is-cell-hidden="row.entity.isChild || row.entity.downtimeTimerID" col-field-key="invoiceNumber"></ui-grid-text-box>',
                },
                {
                    name: 'Note', field: 'note', width: 200,
                    cellTemplate: '<ui-grid-text-box row="row" col-field="COL_FIELD" is-cell-editable="row.entity.editable" is-cell-hidden="row.entity.isChild || row.entity.downtimeTimerID" col-field-key="note"></ui-grid-text-box>',
                },
                {
                    name: 'Edit', width: 120, pinnedRight: true,
                    cellTemplate: `
                        <i ng-if="!row.entity.isChild && !row.entity.systemGenerated" ng-disabled="!grid.appScope.canEditRow || grid.appScope.timersDisabled"
                            ng-class="{ hide: row.entity.editable || row.entity.isChild }"
                            ng-click="!grid.appScope.canEditRow || grid.appScope.beginEdit(row, $event)"
                            class="fa fa-lg fa-pencil icon" data-toggle="tooltip" title="Edit" aria-hidden="true">
                        </i>
                        <i ng-disabled="!grid.appScope.canEditRow || (row.entity.loadTimerEntryID && row.treeNode.parentRow.treeNode.children.length == 1) || grid.appScope.timersDisabled"
                            ng-class="{ hide: row.entity.editable || row.entity.systemGenerated }"
                            ng-click="!grid.appScope.canEditRow || (row.entity.loadTimerEntryID && row.treeNode.parentRow.treeNode.children.length == 1) || grid.appScope.deleteLoad(row.entity.sequence, row)"
                            class="fa fa-lg fa-trash icon" data-toggle="tooltip" title="Delete" aria-hidden="true">
                        </i>
                        <i ng-if="!row.entity.isChild && !row.entity.downtimeTimerID" ng-disabled="!grid.appScope.canEditRow || grid.appScope.timersDisabled"
                            ng-class="{ hide: row.entity.editable || row.entity.isChild }"
                            ng-click="!grid.appScope.canEditRow || grid.appScope.cloneLoad(row.entity)" 
                            ng-disabled="grid.appScope.timersDisabled"
                            class="fa fa-lg fa-clone icon" data-toggle="tooltip" title="Clone" aria-hidden="true">
                        </i>
                        <i class="fa fa-lg fa-floppy-o icon" data-toggle="tooltip" title="Save" aria-hidden="true"
                            ng-disabled="grid.appScope.isInvalid(row) || grid.appScope.timersDisabled"
                            ng-click="grid.appScope.isInvalid(row) || grid.appScope.saveLoad(row)"
                            ng-class="{ hide: !row.entity.editable || row.entity.isChild }">
                        </i>
                        <i class="fa fa-lg fa-ban icon" data-toggle="tooltip" title="Cancel" aria-hidden="true"
                            ng-click="grid.appScope.cancelEdit(row)" 
                            ng-class="{ hide: !row.entity.editable || row.entity.isChild }">
                        </i>
                        <div ng-if="(row.entity.type == grid.appScope.timerTypes.entry) && row.entity.editable">
                            <i ng-disabled="grid.appScope.timersDisabled" ng-click="grid.appScope.addEntry(row.entity, row.entity.sequence)" class="fa fa-lg fa-code-fork fa-rotate-90 icon" data-toggle="tooltip" title="Split" aria-hidden="true"></i>
                        </div>
                    `,
                },
            ];
        }

        //Unable to put this in the typeahead directive because it does not follow the format row.entity[collection][identifier]
        function _loadTypeColCellTemplate() {
            return '<div class="ui-grid-cell-contents" title="{{grid.appScope.getRowID(row.entity)}}" ng-if="!row.entity.downtimeTimerID || !row.entity.editable">{{grid.appScope.getRowID(row.entity)}}</div>' +
                '<div ng-show="row.entity.downtimeTimerID && row.entity.editable">' +
                '<input enter="grid.appScope.enterKeypressSave(row)" required type="text"' +
                'id="inputDowntime" ' +
                'ng-model="row.entity.downtimeReason"' +
                'typeahead="(r.code + \' - \' + r.description) for r in grid.appScope.dropDownLists.downtimeReasons | filter: $viewValue | limitTo: 8"' +
                'typeahead-append-to-body="true"' +
                'typeahead-on-select="grid.appScope.checkCell($item, row.entity.sequence, \'downtimeReason\')" />' +
                '</div>';
        }

        function _setAndFormatDefaultTime(time) {
            time = new Date(time);
            //var date = new Date($routeParams.dayId)
            //var updatedDate = new Date(date.getFullYear(), date.getMonth(), date.getDate(), time.getHours(), time.getMinutes(), time.getSeconds())
            var date = new Date($routeParams.dayId);
            var updatedDate = new Date(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds());
            return updatedDate;
        }

        function _validateLoadTimerEntry(loadTimer) {
            return (!loadTimer.startTime || !loadTimer.endTime);
        }

        function _validatedowntimeTimerEntry(loadTimer) {
            return (!loadTimer.startTime || !loadTimer.endTime || !loadTimer.downtimeReason);
        }

        function _validateTransport(loadTimer) {
            return !loadTimer.truck ||
                    !loadTimer.job ||
                    !loadTimer.phase ||
                    !loadTimer.category ||
                    !loadTimer.loadTime ||
                    !loadTimer.dumpTime ||
                    !loadTimer.loadEquipment ||
                    !loadTimer.endLocation ||
                    !loadTimer.startLocation;


        }

        function _validateTrucking(loadTimer) {
            return !loadTimer.truck ||
                !loadTimer.job ||
                !loadTimer.phase ||
                !loadTimer.category ||
                (!loadTimer.tons && !(loadTimer.tons === 0)) ||
                !loadTimer.loadTime ||
                !loadTimer.dumpTime ||
                !loadTimer.material ||
                !loadTimer.endLocation ||
                !loadTimer.startLocation ||
                !loadTimer.billType;

        }

        function _saveLoadTimerEntry(row, index?) {
            var loadTimerEntry = {
                startTime: moment(row.startTime).format(),
                endTime: moment(row.endTime).format(),
                loadTimerEntryID: row.loadTimerEntryID,
                loadTimerID: row.loadTimerID,
            };

            if (loadTimerEntry.loadTimerEntryID === 'new') {
                LoadTimerEntries.post(loadTimerEntry)
                .then(function(response) {
                    NotificationFactory.success("Load Timer Entry Saved");
                });
            }
            else {
                LoadTimerEntries.one(loadTimerEntry.loadTimerEntryID).patch(loadTimerEntry)
                .then(function(response) {
                    NotificationFactory.success("Load Timer Entry Saved");
                });
            }
            row.edited = false;
        }

        function _saveDowntimeTimer(rowEntity, index?) {
            var downtimeTimer = {
                startTime: moment(rowEntity.startTime).format(),
                stopTime: moment(rowEntity.endTime).format(),
                downtimeReasonID: rowEntity.downtimeReason.downtimeReasonID,
                downtimeTimerID: rowEntity.downtimeTimerID,
                loadTimerID: rowEntity.loadTimerID,
                timesheetID: $scope.timesheet.timesheetID,
            };

            if (downtimeTimer.downtimeTimerID === 'new') {
                DowntimeTimers.post(downtimeTimer)
                .then(function(response) {
                    rowEntity.downtimeTimerID = response.downtimeTimerID;
                    rowEntity.totalMinutes = response.totalMinutes;
                    $scope.timesheet.downtimeTimers.push(rowEntity);
                    $scope.totalTimesheetDowntimeMinutes = _getDowntimeTotal($scope.timesheet.downtimeTimers);

                    const column: any = _.find($scope.gridApi.grid.columns, { aggregationType: 2 });
                    const updatedTotalHours = column ? column.aggregationValue : undefined;
                    $scope.warningClassSetup(updatedTotalHours);
                    NotificationFactory.success("Downtime Timer Saved");
                });
            }
            else {
                DowntimeTimers.one(downtimeTimer.downtimeTimerID).patch(downtimeTimer)
                .then(function(response) {
                    rowEntity.totalMinutes = response.totalMinutes;
                    var index = $scope.timesheet.downtimeTimers.map(function(downtime) { return downtime.downtimeTimerID; }).indexOf(downtimeTimer.downtimeTimerID);
                    if (index > -1) $scope.timesheet.downtimeTimers[index] = response;
                    $scope.totalTimesheetDowntimeMinutes = _getDowntimeTotal($scope.timesheet.downtimeTimers);
                    NotificationFactory.success("Downtime Timer Saved");
                });
            }
            //_checkOverLappingEntries(rowEntity);
            rowEntity.edited = false;
            $scope.gridApi.core.refresh();
        }

        function _checkOverLappingEntries(row) {
            var overlapping = false;
            row.startTimeOverlap = false;
            row.endTimeOverlap = false;

            //check timer starttime is before endtime
            if (row.startTime.getTime() > row.endTime.getTime()) {
                overlapping = true;
                row.startTimeOverlap = true;
                row.endTimeOverlap = true;
            }
            return overlapping;
        }

        function _checkOverLappingSplitEntries() {
            var overlapping = false;
            var additionalTimerForSplit = _getAdditionalTimerToSplitUpTimer();
            var originalTimer = $scope.truckingGridOptions.data[$scope.truckingGridOptions.data.indexOf(additionalTimerForSplit) - 1];

            if (originalTimer && additionalTimerForSplit) {
                if (!(new Date(originalTimer.startTime).getTime() < new Date(additionalTimerForSplit.startTime).getTime())) {
                    overlapping = true;
                    if (new Date(additionalTimerForSplit.startTime).getTime() < new Date(originalTimer.endTime).getTime()) {
                        additionalTimerForSplit.startTimeOverlap = true;
                        originalTimer.endTimeOverlap = true;
                    }
                }
            }

            return overlapping;
        }



        function _getAdditionalTimerToSplitUpTimer() {
            return _.find($scope.truckingGridOptions.data, function(timer) {
                var additionalTimerForSplit = (timer.type.name != 'System DT' && timer['loadTimerEntryID'] && timer['loadTimerEntryID'] == 'new');
                return additionalTimerForSplit;
            });
        }

        function _isTimerOverLapping(timer, row) {
            var timerStartTimeBeforeRowEndTime = new Date(timer.startTime).getTime() < new Date(row.endTime).getTime();
            var timerEndTimeAfterRowStartTime = new Date(timer.endTime).getTime() > new Date(row.startTime).getTime();
            var overlapping = timerStartTimeBeforeRowEndTime && timerEndTimeAfterRowStartTime;

            return overlapping;
        }

        function _getTableHeight(dataLength) {
            dataLength = dataLength || 1;
            var rowHeight = 30;
            var headerHeight = 60;
            var scrollbarHeight = 15;
            var minimizeRowMultiplier = .75;
            if (dataLength > 10) {
                return {
                    height: (dataLength * rowHeight + headerHeight + scrollbarHeight) * minimizeRowMultiplier + "px",
                };
            }

            return {
                height: (dataLength * rowHeight + headerHeight + scrollbarHeight) + "px",
            };
            
        }
    }
}