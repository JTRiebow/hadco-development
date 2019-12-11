import * as angular from 'angular';
import * as moment from 'moment';
import * as _ from 'lodash';

import * as template from './detailed-hours.html';
import * as deleteTemplate from '../../Shared/modal-templates/delete-item-modal.html';
import * as editableTextTemplate from '../../Shared/modal-templates/editable-text-modal.html';
import * as superintendentHeaderTemplate from '../../employee/superintendent/timesheet/header-template.html'
import * as addOccurrenceTemplate from '../../Shared/modal-templates/add-occurrence-modal.html';
import * as addToTimesheetTemplate from '../../Shared/modal-templates/add-to-timesheet-modal.html';
import { INotesService, INote } from '../../admin/note-timers/timer-flag-note-service';
import { IPermissionService } from '../../permissions/permission-service';

angular
    .module('detailedApprovalModule')
    .directive('employeeDetailedHours', employeeDetailedHours);

function employeeDetailedHours($window) {
    return {
        restrict: 'E',
        template,
        controller: detailedHoursController,
        controllerAs: 'vm',
        scope: {
            timers: '=',
            department: '=',
            isTimecardTab: '=',
        },
    };
}

detailedHoursController.$inject = [
    '$scope',
    '$rootScope',
    '$routeParams',
    '$location',
    '$modal',
    '$mdDialog',
    '$timeout',
    'DetailedConcreteService',
    'TimesheetsService',
    'EmployeeDetailedService',
    'EmployeeTimerEntries',
    'EmployeeTimers',
    'NotificationFactory',
    'Users',
    'DateTimeFormats',
    'Pits',
    '$q',
    'DailyApprovals',
    'Notes',
    'PermissionService',
];

function detailedHoursController(
    $scope,
    $rootScope,
    $routeParams,
    $location,
    $modal,
    $mdDialog: angular.material.IDialogService,
    $timeout: angular.ITimeoutService,
    DetailedConcreteService,
    TimesheetsService,
    EmployeeDetailedService,
    EmployeeTimerEntries,
    EmployeeTimers,
    NotificationFactory,
    Users,
    DateTimeFormats,
    Pits,
    $q,
    DailyApprovals,
    Notes: INotesService,
    PermissionService: IPermissionService,
) {
    var vm = this;
    
    vm.timerEntryGridOptions;
    
    vm.can = PermissionService.can;
    vm.assignAddTextModalTriggers = assignAddTextModalTriggers;

    vm.$onInit = init;

    function init() {
        vm.timersDisabled = false;
        vm.department = $scope.department;
        if (vm.department.crush) {
            _getPits().then(function(pits) {
                vm.pits = pits;
            });
            
        }
        vm.pitsEditable = false;
        vm.isTimecardTab = $scope.isTimecardTab;
        $scope.$watch("timers", function(timers) {
            vm.timers = timers;
            if (vm.timers) {
                vm.getCheckStatus();
                vm.timerEntryGridOptions.data = _getCombinedTimerEntries(vm.timers);
                vm.visibleEntries = vm.timerEntryGridOptions.data;
                _.each(vm.timerEntryGridOptions.data, function(entry) {
                    entry = entry.rowEdit = false;
                });
                vm.dayTimer = vm.timers[0];
                vm.gridTableHeight = _getTableHeight(vm.timerEntryGridOptions.data.length);
                _getEmployeeNotes().then(notes => vm.notes = notes);
                
                vm.currentInjuredStatus = vm.dayTimer.injured;
            }
            vm.employeeInfo = { department: vm.department };
        });


        $scope.$on("timers-changed", function() {
            vm.timerEntryGridApi.core.refresh();
            //vm.toggleTimer(vm.activeTimer);
        });
        _getDropDownLists();
        _getOdometer();
        vm.timerEntryGridOptions = _timerEntryGridOptions();
        vm.dateList = TimesheetsService.getDateList($routeParams.dayId);
        vm.activeTimer = 0;
        vm.canEditRow = true;

        $scope.$on('addedTimecardTimer', function(event, data) {
            vm.timerEntryGridOptions.data = data;
            vm.gridTableHeight = _getTableHeight(vm.timerEntryGridOptions.data.length);
        });
        $scope.$on('notesResolved', function() {
            vm.getCheckStatus();
        });

        $scope.$on('addedNewNote', function() {
            vm.getCheckStatus();
        });

        $scope.$on('resolved-note', function() {
            vm.getCheckStatus();
        });

        $scope.$on('resolved-mulitple-notes', function() {
            vm.getCheckStatus();
        });

        $scope.$on('rejected-note', function() {
            vm.getCheckStatus();
        });

        $scope.$on('disableTimers', (_, isDisabled) => {
            vm.timersDisabled = isDisabled;
        });

        _getTruckAndTravelTimes();
        
    }

    vm.getCheckStatus = function() {
        var timerDay = moment($routeParams.dayId).format('YYYY-MM-DD');

        DailyApprovals.getEmployeeApprovals($routeParams.employeeId, timerDay, $routeParams.departmentId)
        .then(function(employeeApprovals) {
            vm.employeeApprovals = employeeApprovals;
            var role = $rootScope.me.roles;
            
                if ((role.isSupervisor() || role.isManager()) && role.isBilling() && role.isAccounting()) {
                    if (vm.employeeApprovals.approvedBySupervisor && vm.employeeApprovals.approvedByBilling && vm.employeeApprovals.approvedByAccounting) {
                        vm.checkColor = "green-text";
                    }
                    else {
                        vm.checkColor = "grey-text";
                    }

                }
                else if ((role.isSupervisor() || role.isManager()) && role.isBilling() && !role.isAccounting()) {
                    if (vm.employeeApprovals.approvedBySupervisor && vm.employeeApprovals.approvedByBilling) {
                        vm.checkColor = "green-text";
                    }
                    else {
                        vm.checkColor = "grey-text";
                    }
                }
                else if ((role.isSupervisor() || role.isManager()) && !role.isBilling() && role.isAccounting()) {
                    if (!vm.employeeApprovals.approvedByBilling) vm.checkColor = "red-text";
                    else if (vm.employeeApprovals.approvedBySupervisor && vm.employeeApprovals.approvedByAccounting) {
                        vm.checkColor = "green-text";
                    }
                    else {
                        vm.checkColor = "grey-text";
                    }
                }
                else if (!role.isSupervisor() && role.isBilling() && role.isAccounting()) {
                    if (!vm.employeeApprovals.approvedBySupervisor) {
                        vm.checkColor = "red-text";
                    }
                    else if (vm.employeeApprovals.approvedBySupervisor && vm.employeeApprovals.approvedByBilling && vm.employeeApprovals.approvedByAccounting) {
                        vm.checkColor = "green-text";
                    }
                    else {
                        vm.checkColor = "grey-text";
                    }
                }
                else if ((role.isSupervisor() || role.isManager()) && !role.isBilling() && !role.isAccounting()) {
                    if (vm.employeeApprovals.approvedBySupervisor) vm.checkColor = "green-text";
                    else vm.checkColor = "grey-text";
                }
                else if (!(role.isSupervisor() || role.isManager()) && role.isBilling() && !role.isAccounting()) {
                    if (!vm.employeeApprovals.approvedBySupervisor) vm.checkColor = "red-text";
                    else if (vm.employeeApprovals.approvedBySupervisor && vm.employeeApprovals.approvedByBilling) vm.checkColor = "green-text";
                    else vm.checkColor = "grey-text";
                }
                else if (role.isAccounting() && !role.isSupervisor() && !role.isBilling()) {
                    if (!vm.employeeApprovals.approvedBySupervisor || !vm.employeeApprovals.approvedByBilling) {
                        vm.checkColor = "red-text";
                    }
                    else if (vm.employeeApprovals.approvedBySupervisor && vm.employeeApprovals.approvedByBilling && vm.employeeApprovals.approvedByAccounting) {
                        vm.checkColor = "green-text";
                    }
                    else vm.checkColor = "grey-text";
                }
                else {
                    vm.checkColor = "grey-text";
                }

                _getEmployeeNotes().then(function(notes) {
                    _.each(notes, function(note) {
                        if (!note.resolved) vm.checkColor = "yellow-text";
                    });
                });
        });
    };

    function _getTruckAndTravelTimes() {
        var timerDate = moment($routeParams.dayId).format("YYYY-MM-DD");
        TimesheetsService.getEmployeeTimesheet($routeParams.employeeId, timerDate, $routeParams.departmentId)
        .then(function(response) {
            if (response.equipment) {
                vm.truckName = response.equipment.name;
                
            }
            else {
                vm.truckName = '';
                vm.travelMinutes = 0.00;
            }

            if (response.equipmentUseTime) {
                vm.travelMinutes = TimesheetsService.convertMinutesToHoursMinutes(response.equipmentUseTime);
            }
            else {
                vm.travelMinutes = 0.00;
            }
            
            vm.foremanName = response.employeeTimers[0].employee.name;
        })
        .catch(angular.noop);
    }

    $scope.$on("cannot-edit-row", function() {
        vm.canEditRow = false;
    });
    $scope.$on("can-edit-row", function() {
        vm.canEditRow = true;
    });

    vm.toggleTimer = function(index) {
        if (index == 'all') {
            vm.dayTimer = null;
            vm.timerEntryGridOptions.data = _getCombinedTimerEntries(vm.timers);
            vm.gridTableHeight = _getTableHeight(vm.timerEntryGridOptions.data.length);
            vm.timerEntryGridApi.core.refresh();
        }
        else {
            vm.dayTimer = vm.timers[index];
            vm.timerEntryGridOptions.data = vm.dayTimer.employeeTimerEntries || [];
            vm.gridTableHeight = _getTableHeight(vm.timerEntryGridOptions.data.length);
            vm.timerEntryGridApi.core.refresh();
        }
        vm.visibleEntries = vm.timerEntryGridOptions.data;
    };

    vm.returnToSupervisorTimesheet = function(index, event) {
        if (event.ctrlKey) {
            var supervisorID = vm.timers[index].supervisor.employeeID;
            $location.path("superintendent/foreman/" + supervisorID + "/department/" + $routeParams.departmentId + "/day/" + $routeParams.dayId);
        }
    };

    function _getEmployeeNotes() {
        vm.employee = {
            day: $routeParams.dayId,
            employeeID: $routeParams.employeeId,
            departmentID: $routeParams.departmentId,
        };

        return Notes.getDailyNotesByEmployee(vm.employee);
    }

    vm.addToTimesheet = function() {
        var modalInstance = $modal.open({
            template: addToTimesheetTemplate,
            controller: 'selectOptionModalContentController',
            windowClass: 'small',
            resolve: {
                options: function() {
                    var supervisorList = vm.supervisors ||
                        DetailedConcreteService.getSupervisors($routeParams.dayId, $routeParams.departmentId, $routeParams.employeeId)
                    .then(function(response) {
                        return response;
                    });
                    return supervisorList;
                },
                entity: function() {
                    return {};
                },
            },
        });

        modalInstance.result.then(function(supervisor) {
            var newTimer = {
                timesheetID: supervisor.timesheetID,
                departmentID: $routeParams.departmentId,
                employeeID: $routeParams.employeeId,
                day: moment(new Date($routeParams.dayId)).format(),
            };
            EmployeeTimers.post(newTimer)
            .then(function(newTimer) {
                newTimer.supervisor = supervisor;
                vm.timers.push(newTimer);

            });
        });
    };

    vm.addNote = function() {
        var modalInstance = $modal.open({
            template: editableTextTemplate,
            controller: 'selectOptionModalContentController',
            windowClass: 'small',
            resolve: {
                options: function() {
                    return [{ name: "Clock In", key: "clockInNote" }, { name: "Clock Out", key: "clockOutNote" }, { name: "Timesheet", key: "diary" }];
                },
                entity: function() {
                    return {};
                },
            },
        });

        modalInstance.result.then(function(newNote) {
            EmployeeTimerEntries.one(vm.timerEntryGridOptions.data[0].employeeTimerEntryID).patch(newNote)
            .then(function(newTimer) {
                
            });
        });
    };

    vm.beginEdit = function(row) {
        if (vm.timersDisabled) {
            return;
        }
        row.rowEdit = true;
        if (vm.canEditRow) {
            vm.backupEntity = angular.copy(vm.timerEntryGridOptions.data);
            vm.canEditRow = false;
            $scope.$emit("cannot-edit-row");
            vm.timerEntryGridOptions.data.forEach(function(timerEntry) {
                timerEntry.clockInDate = _setUpStartAndStopDatesDropDown(timerEntry.clockIn);
                timerEntry.clockOutDate = _setUpStartAndStopDatesDropDown(timerEntry.clockOut);
                timerEntry.editable = true;
            });
            vm.pitsEditable = true;

            $timeout(function() {
                angular.element("#dateInput" + row.entity.employeeTimerEntryID).focus();
                $("#dateInput" + row.entity.employeeTimerEntryID).select();
                
            });
        }
    };

    vm.syncDateAndTime = function(item, index, type, entity) {
        var syncModelAndTime = TimesheetsService.syncDateAndTime(item, type, entity);
        entity[syncModelAndTime.key] = syncModelAndTime.value;
    };

    vm.checkClockInClockOut = function(timeCardTimer) {
        if (timeCardTimer.clockOut < timeCardTimer.clockIn) {
            timeCardTimer.invalidEntries = true;
        }
        else {
            timeCardTimer.invalidEntries = false;
        }
        return timeCardTimer.invalidEntries;
    };

    vm.invalidEntries = function(timers) {
        if (vm.visibleEntries) {
            return EmployeeDetailedService.hasOverlap(vm.visibleEntries[0], 'clockIn', 'clockOut', vm.visibleEntries, 'employeeTimerEntryID');
        }
    };

    vm.getPushpin = function(time, latitude, longitude, type) {
        vm.selectedPushpin = { time: time, latitude: latitude, longitude: longitude, type: type };
    };

    vm.deleteTimer = function(timerEntry) {
        if (vm.timersDisabled) {
            return;
        }
        if (_.isString(timerEntry.employeeTimerEntryID) && timerEntry.employeeTimerEntryID.startsWith("new")) { //if timerEntryID == "new" + index
            var index = vm.timerEntryGridOptions.data.indexOf(timerEntry);
            if (index > -1) vm.timerEntryGridOptions.data.splice(index, 1);
        }
        else {

            var modalInstance = $modal.open({
                controller: 'DeleteModalContentController',
                template: deleteTemplate,
                windowClass: 'default-modal',
                resolve: {
                    deletedItemName: function() {
                        return "Timer Entry";
                    },
                },
            });

            modalInstance.result.then(function(data) {

                EmployeeTimerEntries.one(timerEntry.employeeTimerEntryID)
                .remove()
                .then(function(data) {
                    var index = vm.timerEntryGridOptions.data.indexOf(timerEntry);
                    if (index > -1) vm.timerEntryGridOptions.data.splice(index, 1);
                    vm.visibleEntries = vm.timerEntryGridOptions.data;
                    NotificationFactory.success("Timer Entry was Successfully Deleted");
                }, function() {
                    NotificationFactory.error("Timer Entry was not Deleted");

                });
            });
        }
    };

    vm.getAllHistory = function() {
        _updateHistory();
        vm.allHistory = true;
    };

    vm.setHistory = function(timerEntry) {
        var history = EmployeeDetailedService.setHistory(timerEntry);
        vm.visibleHistory = history;
        vm.allHistory = false;
        vm.historyTabActive = true;
        
    };

    function _updateHistory() {
        vm.visibleHistory = [];
        var formattedDate = moment($routeParams.dayId).format("YYYY-MM-DD");
        Users.one($routeParams.employeeId).one("EmployeeTimers", formattedDate).one("Department", $routeParams.departmentId).get()
        .then(function(data) {
            vm.foo = data.items;
            vm.visibleHistory = [];
            _.each(data.items, function(item) {
                item.employeeTimerEntries.forEach(function(timerEntry) {
                    var historyEntries = EmployeeDetailedService.setHistory(timerEntry);
                    vm.visibleHistory = vm.visibleHistory.concat(historyEntries);
                });
            });
        });
    }

    vm.toggleInjured = async (injured, employeeTimerID) => {
        if (injured == vm.currentInjuredStatus) return;
        
        try {
            if (injured) {
                let note: INote = Array.isArray(vm.notes) ? vm.notes.find(n => n.noteTypeID == 5) : null;
                const isNew = !note;
                
                const description = await vm.openAddTextModal({
                    locals: {
                        required: true,
                        label: 'Please include a note about the injury',
                        useTextarea: true,
                        title: 'Injury Note',
                        value: note ? note.description : '',
                    },
                });
                
                if (isNew) {
                    note = await Notes.postNoteForFlaggedTimeCard({
                        day: new Date(vm.dayTimer.day),
                        departmentID: vm.dayTimer.departmentID,
                        noteTypeID: 5,
                        employeeID: vm.employee.employeeID,
                        description
                    });
                }
                else {
                    note = await Notes.updateNoteDescription(note.noteID, description);
                }
                
                vm.notes = await _getEmployeeNotes();
            }
            
            await EmployeeTimers.one(employeeTimerID).patch({ injured });
            
            NotificationFactory.success('Timer marked as ' + (injured ? 'injured' : 'not injured'));
            
            $rootScope.$broadcast('refresh-notes');
            
            vm.currentInjuredStatus = injured;
        }
        catch (e) {
            if (!e) {
                vm.dayTimer.injured = !injured;
                $scope.$apply();
            }
            else {
                console.info(e);
            }
        }
    };

    vm.updateOdometer = function(odometer) {
        var timesheet = {
            odometer: odometer,
            timesheetID: vm.timers[0].timesheetID,
        };
        TimesheetsService.patchTruckerTimesheet(timesheet)
            .then(function(response) {
                NotificationFactory.success('Odometer updated');
            });
    };

    vm.addOccurrence = (dayTimer) => {
        if (vm.timersDisabled) {
            return;
        }
        const originalOccurrences = dayTimer.occurrences || [];
        const originalOccurrenceIds = originalOccurrences.map(o => o.occurrenceID);
        var modalInstance = $modal.open({
            controller: 'addOccurrencesAndNoteModalController',
            template: addOccurrenceTemplate,
            windowClass: 'default-modal',
            resolve: {
                currentOccurrences: function() {
                    return originalOccurrences.slice();
                },
                availableOccurrences: function() {
                    return vm.dropDownLists.occurrences;
                },
            },
        });

        modalInstance.result.then(async result => {
            const occurrencesList = result.occurrences.map((occurrence) => occurrence.occurrenceID);
            const occurrenceWasAdded = result.occurrences.length > originalOccurrences.length || occurrencesList.filter(id => !originalOccurrenceIds.includes(id)) > 0;
            const occurrenceWasDeleted = result.occurrences.length < originalOccurrences.length || originalOccurrenceIds.filter(id => !occurrencesList.includes(id)).length > 0;
            const noOccurrencesLeft = !result.occurrences.length;
            
            let note: INote = Array.isArray(vm.notes) ? vm.notes.find((n: INote) => n.noteTypeID == 10 && !n.resolved) : null;
            
            let noteIsNew = false;
            let noteIsUpdated = false;
            const noteIsUnresolved = note && !note.resolved;
            let noteShouldResolve = false;
            
            if (!occurrenceWasAdded && !occurrenceWasDeleted) {
                return;
            }
            else if (occurrenceWasAdded) {
                
                let description;
                try {
                    description = await vm.openAddTextModal({
                        locals: {
                            label: note ? 'If you want, you can update the occurrence note' : 'Please include a note about the occurrence (optional)',
                            useTextarea: true,
                            title: 'Occurrence Note',
                            value: note ? note.description : '',
                        },
                    });
                }
                catch (e) {}
                
                noteIsNew = description && !note;
                noteIsUpdated = note && description && description != note.description;
                
                if (noteIsNew) {
                    await Notes.postNoteForFlaggedTimeCard({
                        day: new Date(vm.dayTimer.day),
                        departmentID: vm.dayTimer.departmentID,
                        noteTypeID: 10,
                        employeeID: vm.employee.employeeID,
                        description,
                    });
                }
                else if (noteIsUpdated) {
                    await Notes.updateNoteDescription(note.noteID, description);
                }
            }
            else if (occurrenceWasDeleted && noOccurrencesLeft && noteIsUnresolved && PermissionService.can('resolveEmployeeTimerFlag')) {
                await vm.openConfirmModal({
                    locals: {
                        text: 'You are about to delete the last occurrence.  Do you want to resolve the occurrence note?',
                        cancel: 'No',
                        ok: 'Yes',
                    },
                })
                    .then(result => {
                        noteShouldResolve = result;
                        
                        if (result) {
                            return Notes.resolve(note.noteID);
                        }
                    })
                    .catch(angular.noop);
            }
            
            if (noteIsNew || noteIsUpdated || noteShouldResolve) {
                $rootScope.$broadcast('refresh-notes');
                
                vm.notes = await _getEmployeeNotes();
            }
            
            EmployeeTimers
                .one(dayTimer.employeeTimerID)
                .one('Occurrences')
                .patch(occurrencesList)
                .then(function(response) {
                    dayTimer.occurrences = response;
                });
        })
        .catch(angular.noop);
    };
    
    vm.addTimerEntry = function(index) {
        if (vm.timersDisabled) {
            return;
        }
        index = index || vm.timerEntryGridOptions.data.length;
        var defaultClockIn = moment();
        
        var newTimerEntry = {
            clockInDate: vm.dateList[0],
            clockIn: vm.dateList[0].date,
            clockOutDate: vm.dateList[0],
            clockOut: vm.dateList[0].date,
            employeeTimerID: vm.dayTimer.employeeTimerID,
            employeeTimerEntryID: 'new' + index,
            pitID: 'new',
        };

        vm.timerEntryGridOptions.data.splice(index, 0, newTimerEntry);
        vm.gridTableHeight = _getTableHeight(vm.timerEntryGridOptions.data.length);
        vm.timerEntryGridApi.core.refresh();
        var row = { entity: newTimerEntry };
        vm.beginEdit(row);
        
    };

    vm.cancelEdit = function(row) {
        vm.pitsEditable = false;
        angular.copy(vm.backupEntity, vm.timerEntryGridOptions.data);
        vm.canEditRow = true;
        vm.timerEntryGridOptions.data.forEach(function(timerEntry, index) {
            timerEntry.editable = false;
            if (_.isString(timerEntry.employeeTimerEntryID) && timerEntry.employeeTimerEntryID.startsWith("new")) { //if timerEntryID == "new" + index
                vm.timerEntryGridOptions.data.splice(index, 1);
                vm.gridTableHeight = _getTableHeight(vm.timerEntryGridOptions.data.length);
                vm.timerEntryGridApi.core.refresh();
            }
        });
        $scope.$emit("can-edit-row");
    };

    vm.saveTimerEntries = function() {
        if (vm.department.crush) {
            _.each(vm.timerEntryGridOptions.data, function(entry) {
                if (entry.pitID === 'new' || entry.rowEdit) {
                    entry.pitID = vm.selectedPit.pitID;
                    entry.rowEdit = false;
                }

            });
        }
        
        EmployeeDetailedService.saveEntries(vm.timerEntryGridOptions.data)
            .then(function(response) {
                vm.timerEntryGridOptions.data = response;
                $rootScope.$broadcast('addedTimecardTimer', vm.timerEntryGridOptions.data);
                vm.visibleEntries = vm.timerEntryGridOptions.data;
                vm.canEditRow = true;
                vm.backupEntity = angular.copy(vm.timerEntryGridOptions.data);
                NotificationFactory.success("Success: Timer Entries saved.");
                $scope.$emit("emit-updated-timers");
                $scope.$emit("can-edit-row");

                _updateHistory();

            }, function(error) {
                NotificationFactory.error("Error: Timer Entries were not saved.");
            });

        if (vm.department.crush) {
            vm.pitsEditable = false;
        }
        
        
    };
    
    vm.assignConfirmModalTriggers = ({ open, close, cancel }) => {
        vm.openConfirmModal = open;
        vm.closeConfirmModal = close;
        vm.cancelConfirmModal = cancel;
    };
    
    function _getDropDownLists() {
        TimesheetsService.getDropDownLists().then(function(response) {
            vm.dropDownLists = response;
        });
    }

    function _getOdometer() {
        if (vm.department.trucking || vm.department.transport) {
            vm.viewOdometer = true;
            Users.one($routeParams.employeeId)
                .one('Odometer', $routeParams.dayId)
                .one('DepartmentID', $routeParams.departmentId)
                .get()
                .then(function(response) {
                    vm.odometer = response;
                });
        }
    }

    function _getPits() {
        var deferred = $q.defer();
        Pits.getList().then(function(data) {
            deferred.resolve(data);
        });
        return deferred.promise;
    }

    function _setUpStartAndStopDatesDropDown(modelDateTime) {
        var matchesDayOnTimeCard = (moment(modelDateTime).format('MM-DD-YYYY') == $routeParams.dayId);
        return matchesDayOnTimeCard ? vm.dateList[0] : vm.dateList[1];
    }

    function _getCombinedTimerEntries(timers) {
        var totalMinutes = 0;
        var entries = [];
        _.each(timers, function(timer) {
            entries = entries.concat(timer.employeeTimerEntries);
            totalMinutes += timer.totalMinutes;
        });
        vm.formattedTotalMinutes = TimesheetsService.convertMinutesToHoursMinutes(totalMinutes);
        return entries;
    }

    function _timerEntryGridOptions() {
        return {
            treeRowHeaderAlwaysVisible: true,
            headerTemplate: superintendentHeaderTemplate,
            enableGridMenu: false,
            enableColumnMenus: false,
            enableSorting: false,
            enableScrollbars: false,
            enableHorizontalScrollbar: 1,
            enableVerticalScrollbar: 0,
            rowHeight: 30,
            headerHeight: 60,
            columnVirtualizationThreshold: 50,
            onRegisterApi: function(gridApi) {
                vm.timerEntryGridApi = gridApi;
            },
            useUiGridDraggableRowsHandle: true,
            rowTemplate: '<div grid="grid" class="ui-grid-draggable-row" draggable="true">' +
                '<div ng-repeat="(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name"' +
                'class="ui-grid-cell" ng-class="{ \'ui-grid-row-header-cell\': col.isRowHeader, \'custom\': true }" ui-grid-cell>' +
                '</div></div>',
            columnDefs: [
                {
                    field: 'clockIn',
                    width: 120,
                    cellFilter: 'date: "MM/dd/yyyy"',
                    displayName: 'Date',
                    category: 'clockIn',
                    cellTemplate: '<ui-grid-dropdown focus-id="dateInput{{row.entity.employeeTimerEntryID}}" row="row" type="equipment" is-cell-editable="row.entity.editable && grid.appScope.vm.isTimecardTab"  col-field="COL_FIELD | date: \'MM/dd/yyyy\'" ' +
                    'dropdown-list="row.grid.appScope.vm.dateList" drag-handle="!grid.appScope.vm.canEditRow && grid.appScope.vm.isTimecardTab" model-collection="clockInDate" model-identifier="formattedDate" check-function="grid.appScope.vm.syncDateAndTime">' +
                        '</ui-grid-dropdown>',
                },
                {
                    field: 'clockIn',
                    width: 110,
                    cellFilter: 'date: "hh:mma"',
                    displayName: 'Time',
                    category: 'clockIn',
                    cellTemplate: '<ui-grid-time-cell  required="true" row="row" is-cell-editable="row.entity.editable && grid.appScope.vm.isTimecardTab" col-field-key="clockIn"></ui-grid-time-cell>',
                },
                {
                    field: 'gps',
                    visible: vm.isTimecardTab,
                    width: 50,
                    displayName: 'GPS',
                    category: 'clockIn',
                    cellTemplate: '<div class="ui-grid-cell-contents"><i class="fa fa-lg fa-map-marker" ng-class="row.entity.clockInLongitude && \'green-text\'" aria-hidden="true" ng-click="row.entity.clockInLongitude && grid.appScope.vm.getPushpin(row.entity.clockIn, row.entity.clockInLatitude, row.entity.clockInLongitude, \'clockIn\')"></i></div>',
                },
                {
                    field: 'clockOut',
                    width: 115,
                    cellFilter: 'date: "MM/dd/yyyy"',
                    displayName: 'Date',
                    category: 'clockOut',
                    cellTemplate: '<ui-grid-dropdown row="row" type="equipment" is-cell-editable="row.entity.editable && grid.appScope.vm.isTimecardTab"  col-field="COL_FIELD | date: \'MM/dd/yyyy\'" ' +
                    'dropdown-list="row.grid.appScope.vm.dateList" model-collection="clockOutDate" model-identifier="formattedDate" check-function="grid.appScope.vm.syncDateAndTime">' +
                        '</ui-grid-dropdown>',
                },
                {
                    field: 'clockOut',
                    width: 110,
                    cellFilter: 'date: "hh:mma"',
                    displayName: 'Time',
                    category: 'clockOut',
                    cellTemplate: '<ui-grid-time-cell  required="true" row="row" is-cell-editable="row.entity.editable && grid.appScope.vm.isTimecardTab" col-field-key="clockOut"></ui-grid-time-cell>',
                },
                {
                    field: 'gps',
                    visible: vm.isTimecardTab,
                    width: 50,
                    displayName: 'GPS',
                    category: 'clockOut',
                    cellTemplate: '<div class="ui-grid-cell-contents"><i class="fa fa-lg fa-map-marker"ng-class="row.entity.clockOutLongitude && \'red-text\'" aria-hidden="true" ng-click="row.entity.clockOutLatitude && grid.appScope.vm.getPushpin(row.entity.clockOut, row.entity.clockOutLatitude, row.entity.clockOutLongitude, \'clockOut\')"></i></div>',
                },

                {
                    field: 'pitID',
                    visible: vm.department.crush !== undefined && vm.isTimecardTab,
                    pinnedRight: true,
                    width: 100,
                    name: 'Pit',
                    category: 'pit',
                    cellTemplate: '<input ng-if="grid.appScope.vm.pitsEditable && (row.entity.pitID === \'new\' || row.entity.rowEdit) "' +
                                        'placeholder="Select Pit"' +
                                        'ng-model="grid.appScope.vm.selectedPit"' +
                                        'typeahead="pit as pit.name for pit in grid.appScope.vm.pits | filter: { name:$viewValue } | limitTo: 8"' +
                                        'typeahead-append-to-body="true"' +
                                        'typeahead-editable="false"' +
                                        ' />' +
                                    '<div ng-hide="grid.appScope.vm.pitsEditable"' +
                                        'ng-bind="grid.appScope.vm.getPitName(row.entity.pitID)"' +
                                        'class="ellipsis-long-text"' +
                                        'data-toggle="tooltip" title="{{grid.appScope.vm.getPitName(row.entity.pitID)}}"></div>',
                },

                {
                    name: 'Actions',
                    visible: vm.isTimecardTab,
                    category: 'actions',
                    minWidth: 110,
                    pinnedRight: true,
                    cellTemplate: `
                        <div class="ui-grid-cell-contents">
                            <i
                                ng-class="{ hide: !grid.appScope.vm.canEditRow }"
                                class="fa  fa-pencil icon"
                                data-toggle="tooltip"
                                title="Edit"
                                aria-hidden="true"
                                ng-disabled="grid.appScope.vm.timersDisabled"
                                ng-if="grid.appScope.vm.can('editEmployeeTimerEntry')"
                                ng-click="grid.appScope.vm.beginEdit(row); row.entity.rowEdit = true"
                            ></i>
                            
                            <i
                                ng-class="{ hide: grid.appScope.vm.canEditRow || grid.appScope.vm.activeTimer == 'all' }"
                                class="fa  fa-plus icon"
                                data-toggle="tooltip"
                                title="Add Entry"
                                aria-hidden="true"
                                ng-disabled="grid.appScope.vm.timersDisabled"
                                ng-if="grid.appScope.vm.can('addEmployeeTimerEntry')"
                                ng-click="grid.appScope.vm.canEditRow = true; grid.appScope.vm.addTimerEntry(rowRenderIndex + 1)"
                            ></i>
                            
                            <i
                                ng-disabled="!grid.appScope.vm.canEditRow || grid.appScope.vm.timersDisabled"
                                data-toggle="tooltip"
                                title="Delete"
                                class="fa  fa-trash icon"
                                ng-click="!grid.appScope.vm.canEditRow || grid.appScope.vm.deleteTimer(row.entity, row.entity.sequence)"
                                ng-if="grid.appScope.vm.can('deleteEmployeeTimerEntry')"
                            ></i>
                            
                            <i
                                ng-disabled="!grid.appScope.vm.canEditRow || row.entity.employeeTimerEntryHistories.length < 1"
                                class="fa fa-clock-o icon"
                                data-toggle="tooltip"
                                title="View Edit History"
                                aria-hidden="true"
                                ng-click="grid.appScope.vm.setHistory(row.entity)"
                                ng-if="grid.appScope.vm.can('viewEmployeeTimerEditHistory')"
                            ></i>
                        </div>
                    `,
                },
            ],
            category: [
                { name: 'clockIn', displayName: 'Clock In', visible: true, showCatName: true },
                { name: 'clockOut', displayName: 'Clock Out', visible: true, showCatName: true },
                { name: 'pit', visible: true, showCatName: false },
                { name: 'actions', visible: true, showCatName: false },
            ],
        };
        
    }
    
    vm.getPitName = function(pitIdFromGrid) {
        var pitName = '';
        angular.forEach(vm.pits, function(pit) {

            if (pit.pitID === pitIdFromGrid) {
                pitName = pit.name;
            }
        });

        return pitName;
    };
    
    function assignAddTextModalTriggers(triggers) {
        vm.openAddTextModal = triggers.open;
        vm.closeAddTextModal = triggers.close;
        vm.cancelAddTextModal = triggers.cancel;
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
