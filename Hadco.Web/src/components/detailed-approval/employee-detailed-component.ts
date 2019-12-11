import * as angular from 'angular';
import * as moment from 'moment';
import * as _ from 'lodash';

import * as template from './employee-overview.html';
import * as customMessageTemplate from '../Shared/modal-templates/confirm-custom-message-modal.html';
import * as addDetailedNoteTemplate from '../Shared/modal-templates/add-detailed-timer-note-modal.html';
import * as addTimeTemplate from '../Shared/modal-templates/add-time-modal.html';
import { ITimerApprovalsService } from '../timer-approval/timer-approval-service';
import { IPermissionService } from '../permissions/permission-service';

angular
    .module('detailedApprovalModule')
    .component('htEmployeeDetailed', {
        controller: employeeDetailedController,
        controllerAs: 'vm',
        template,
    });

employeeDetailedController.$inject = [
    '$scope',
    '$rootScope',
    'NotificationFactory',
    '$location',
    '$modal',
    '$routeParams',
    'DailyApprovals',
    'TimerApprovalsService',
    'Users',
    'EmployeeTimers',
    '$q',
    'TimesheetsService',
    'EmployeeDetailedService',
    'Notes',
    'PermissionService',
    'EmployeeTimerEntries',
];

function employeeDetailedController(
    $scope,
    $rootScope: ng.IRootScopeService & { [key: string]: any },
    NotificationFactory,
    $location,
    $modal,
    $routeParams,
    DailyApprovals,
    TimerApprovalsService: ITimerApprovalsService,
    Users,
    EmployeeTimers,
    $q,
    TimesheetsService,
    EmployeeDetailedService,
    Notes,
    PermissionService: IPermissionService,
    EmployeeTimerEntries,
) {
    var vm = this;
    
    Object.defineProperty(vm, 'canReject', {
        get() {
            return vm.employeeApprovals && (
                (
                    vm.employeeApprovals.approvedBySupervisor &&
                        vm.can('rejectSupervisorApproval')
                ) || (
                    vm.employeeApprovals.approvedByBilling &&
                        vm.can('rejectBillingApproval')
                ) || (
                    vm.employeeApprovals.approvedByAccounting &&
                        vm.can('rejectAccountingApproval')
                )
            );
        },
    });
    
    Object.defineProperty(vm, 'canApprove', {
        get() {
            const {
                approvedBySupervisor,
                approvedByBilling,
                approvedByAccounting,
            } = vm.employeeApprovals || {} as { [k: string]: any };
            
            const noteIsUnresolved = vm.notes ? _isNoteUnresolved(vm.notes) : true;
            
            return (
                _canApproveSupervisor() ||
                _canApproveBiller() ||
                _canApproveAccountant()
            ) && !noteIsUnresolved;
        }
    });
    
    vm.can = PermissionService.can;

    init();

    function init() {
        PermissionService.getDepartmentPermissions($routeParams.departmentId)
            .then(() => {
                PermissionService.redirectIfUnauthorized('viewEmployeeTimer');
            });
        
        $scope.$on("emit-updated-timers", function() {
            vm.updateEditedTimerTotals();
        });

        $scope.$on('notesResolved', function() {
                _getApprovals();
        });

        $scope.$on('addedNewNote', function() {
            //_getApprovals();
        });

        $scope.$on('rejected-note', function() {
            _getApprovals();
        });

        vm.department = {};
        vm.keys = {};
        vm.timesheetID;
        vm.canEditRow = true;
        vm.timersTabActive = $routeParams.timersTab == "timers";
        vm.timecardTabActive = $routeParams.timersTab == "timecard";
        vm.day = EmployeeDetailedService.day();
        vm.dateList = TimesheetsService.getDateList($routeParams.dayId);
        _setupDepartments();
        _getTimeCard();
        _getApprovals();

        vm.employee = {
            day: $routeParams.dayId,
            employeeID: $routeParams.employeeId,
            departmentID: $routeParams.departmentId,
        };
        
        _getNotes()
            .then(notes => vm.notes = notes)
            .catch(e => console.info(e));
    }

    function _setupDepartments() {
        switch ($routeParams.departmentId) {
            case '1':
                vm.department.concrete = true;
                vm.department.CDR = true;
                vm.timerTabName = "Job";
                break;
            case '2':
                vm.department.development = true;
                vm.department.CDR = true;
                vm.timerTabName = "Job";
                break;
            case '3':
                vm.department.residential = true;
                vm.department.CDR = true;
                vm.timerTabName = "Job";
                break;
            case '4':
                vm.department.trucking = true;
                vm.timerTabName = "Trucking";
                break;
            case '5':
                vm.department.shop = true;
                vm.timerTabName = "Mechanics";
                break;
            case '6':
                vm.department.office = true;
                break;
            case '7':
                vm.department.crush = true;
                break;
            case '8':
                vm.department.transport = true;
                vm.timerTabName = "Transport";
                break;
            case '9':
                vm.department.concrete = true;
                vm.timerTabName = "Job";
                break;
            case '10':
                vm.department.concrete = true;
                vm.timerTabName = "Job";
                break;
        }
    }

    $(document.body).keydown(function(event) {
        // save status of the button 'pressed' == 'true'
        if (event.keyCode == 16) {
            vm.keys["shift"] = true;
        }
        if (event.keyCode == 17) {
            vm.keys["ctrl"] = true;
        }
        if (event.keyCode == 49) {
            vm.keys["one"] = true;
        }
        if (event.keyCode == 50) {
            vm.keys["two"] = true;
        }
        if (vm.keys["shift"] && vm.keys["ctrl"] && vm.keys["one"]) {
            $("#timecardtab").trigger("click");
        }
        if (vm.keys["shift"] && vm.keys["ctrl"] && vm.keys["two"]) {
            $("#timerstab").trigger("click");
        }
    });

    $(document.body).keyup(function(event) {
        // reset status of the button 'released' == 'false'
        if (event.keyCode == 16) {
            vm.keys["shift"] = false;
        }
        else if (event.keyCode == 17) {
            vm.keys["ctrl"] = false;
        }
        else if (event.keyCode == 49) {
            vm.keys["one"] = false;
        }
        else if (event.keyCode == 50) {
            vm.keys["two"] = false;
        }
    });


    $scope.$on("cannot-edit-row", function() {
        vm.canEditRow = false;
    });

    $scope.$on("can-edit-row", function() {
        vm.canEditRow = true;
    });

    vm.changeTab = function(tab) {
        if (tab === 'timecard') {
            vm.timecardTabActive = true;
        }
        else if (tab === 'timers') {
            vm.timersTabActive = true;
        }

        var newPath = $location.path();

        if (vm.timersTabActive) {
            if ($location.path() && newPath.indexOf('/timers') < 0) {
                newPath = $location.path() + "/timers";
                $location.path(newPath);
            }
        }
        else {
            if (newPath.indexOf('/timers') > 0) {
                $location.path(newPath.substring(0, newPath.indexOf('/timers')));
            }
        }
    };

    vm.reject = function() {
        _openNotesModal();
    };

    vm.approveTimer = function() {
        _getApprovals().then(function(employee) {
            var approvalRole = getApprovalRole(employee);

            openApproveTimerModal(approvalRole)
                .result
                .then(async () => {
                    var approval = _getApprovalRoleObjToPatch(approvalRole);
                    await TimerApprovalsService.patch(vm.employee, approval);
                    
                    Object.assign(employee, approval);

                    _setDisabledStatus();
                    _setRejectButton();
                    
                    if ($rootScope.$$phase != '$apply' && $rootScope.$$phase != '$digest') {
                        $rootScope.$apply();
                    }
                })
                .catch(() => {
                    console.log("cancelled");
                });

        });
    };
    
    vm.getApprovalRole = getApprovalRole;
    
    function getApprovalRole(employee = vm.employeeApprovals) {
        const { approvedBySupervisor, approvedByBilling, approvedByAccounting } = employee;
        
        if (!approvedBySupervisor && vm.can('approveAsSupervisor')) {
            return 'Supervisor';
        }
        else if (approvedBySupervisor && !approvedByBilling && vm.can('approveAsBilling')) {
            return 'Biller';
        }
        else if (approvedBySupervisor && approvedByBilling && !approvedByAccounting && vm.can('approveAsAccounting')) {
            return 'Accountant';
        }
    }

    function _getApprovalRoleObjToPatch(role) {
        var approvalsObj = {} as { [k: string]: boolean };
        var currentUserRole = $rootScope.me.roles;

        if (role === "Supervisor") {
            approvalsObj.approvedBySupervisor = true;
        }
        else if (role === "Biller") {
            approvalsObj.approvedByBilling = true;
        }
        else if (role === "Accountant") {
            approvalsObj.approvedByAccounting = true;
        }

        return approvalsObj;
    }

    function _openNotesModal() {
        var reject = true;
        var modalInstance = $modal.open({
            controller: 'addDetailedTimerNoteModalController',
            controllerAs: 'vm',
            template: addDetailedNoteTemplate,
            windowClass: 'default-modal',
            resolve: {
                employeeID: function() {
                    return vm.employee.employeeID;
                },
                departmentID: function() {
                    return vm.employee.departmentID;
                },
                timerDay: function() {
                    return vm.employee.day;
                },
                rejectNote: function() {
                    return reject;
                },
                currentApprovals: () => vm.employeeApprovals,
            },
        });


        modalInstance.result.then(function(response) {
            if (reject) {
                var approvalsObj = {} as any;
                if (response.supervisor) {
                    approvalsObj.approvedBySupervisor = false;
                }
                if (response.billing) {
                    approvalsObj.approvedByBilling = false;
                }
                if (response.accounting) {
                    approvalsObj.approvedByAccounting = false;
                }
                
                TimerApprovalsService.patch(vm.employee, approvalsObj);
            }
            else {
                vm.employee.hasNote = true;
            }
            $scope.$broadcast('rejected-note', response.newNote);
            vm.notes.push(response.newNote);
            _setDisabledStatus();
        });

        
    }
    
    vm.returnToApprovalPage = function() {
        return EmployeeDetailedService.returnToApprovalPage();
    };

    vm.changeDate = function(action) {
        var manualInput = $("#selectDate").is(':focus');
        if (manualInput && action !== 'enter') {
        }
        else {
            if (action == 'prev') {
                vm.day = moment(vm.day).subtract(1, 'days');
            }
            else if (action == 'next') {
                vm.day = moment(vm.day).add(1, 'days');
            }
            else if (action == 'enter') {
                vm.opened = false;
                angular.element("#selectDate").blur();
            }

            vm.day = moment(vm.day).format('MM/DD/YYYY');
            var urlDate = moment(vm.day).format('MM-DD-YYYY');

            Users.one($routeParams.employeeId)
                .one('EmployeeTimers', urlDate)
                .head({ "DepartmentID": $routeParams.departmentId })
            .then(function(response) {
                $location.path("/employee/" + $routeParams.employeeId + "/department/" + $routeParams.departmentId + "/day/" + urlDate);
            }, function(error) {
                console.log(error);
                createEmployeeTimerPrompt(vm.day, urlDate);
            });
        }
        
    };
    
    function createEmployeeTimerPrompt(day, urlDate) {
        var modalInstance = $modal.open({
            controller: 'confirmCustomMessageController',
            controllerAs: 'vm',
            template: customMessageTemplate,
            windowClass: 'default-modal',
            resolve: {
                customMessage: function() {
                    return "This employee doesn't have a timesheet for " + day + ". Please confirm that you'd like to create one.";
                },
            },
        });
        modalInstance.result.then(function() {
            $location.path("/employee/" + $routeParams.employeeId + "/department/" + $routeParams.departmentId + "/day/" + urlDate);
        }, function() {
            vm.day = EmployeeDetailedService.day();
        });
    }

    function openApproveTimerModal(approvalRole) {
        var modalInstance = $modal.open({
            controller: 'confirmCustomMessageController',
            controllerAs: 'vm',
            template: customMessageTemplate,
            windowClass: 'default-modal',
            resolve: {
                customMessage: function() {
                    return `
                        Are you sure you want to approve this timer as 
                        a${ approvalRole == 'Accounting' ? 'n' : '' } ${
                            approvalRole
                        }?`;
                },
            },
        });

        return modalInstance;
    }

    vm.updateEditedTimerTotals = function() {
        Users.one($routeParams.employeeId)
        .one('EmployeeTimers', $routeParams.dayId).one("Department", $routeParams.departmentId)
        .get()
        .then(function(timers) {
            var updatedTimers = timers.items;
            _.each(vm.timers, function(timer, index) {
                var updatedTimerIndex = _.findIndex(updatedTimers, { employeeTimerID: timer.employeeTimerID });
                timer.totalMinutes = updatedTimers[updatedTimerIndex].totalMinutes;
                timer.formattedTotalMinutes = TimesheetsService.convertMinutesToHoursMinutes(updatedTimers[updatedTimerIndex].totalMinutes);
                _.each(timer.employeeTimerEntries, function(timerEntry, index) {
                    var updatedTimerEntryIndex = _.findIndex(updatedTimers[updatedTimerIndex].employeeTimerEntries, { employeeTimerEntryID: timerEntry.employeeTimerEntryID });
                    var newTimerEntryHistories = updatedTimers[updatedTimerIndex].employeeTimerEntries[updatedTimerEntryIndex].employeeTimerEntryHistories;
                    timerEntry.employeeTimerEntryHistories = newTimerEntryHistories;
                });
                timer.employeeTimerEntries = _.sortBy(timer.employeeTimerEntries, 'clockIn');
            });
            $scope.$broadcast("timers-changed");
        });
    };

    vm.addTime = function(CDRemployeeTimerID) {
        var modalInstance = $modal.open({
            template: addTimeTemplate,
            controller: 'AddTimeModalContentController',
            resolve: {
                date: function($routeParams) {
                    return moment(new Date($routeParams.dayId));
                },
                pitList: function() {
                    if (vm.department.crush) {
                        return vm.dropDownLists.pits;
                    }
                    else { return null; }
                },
                dep: function() {
                    return vm.department;
                },
            },
        });

        modalInstance.result.then(function(time) {
            getOrCreateEmployeeTimerID(time, CDRemployeeTimerID)
            .then(function(employeeTimerID) {
                employeeTimerID = employeeTimerID;
                var newTime = {
                    clockIn: moment(time.clockIn).format(),
                    clockOut: moment(time.clockOut).format(),
                    employeeTimerID: employeeTimerID,
                    pitID: time.pitID,
                };
                EmployeeTimerEntries.post(newTime)
                .then(function(data) {
                    
                    Users.one($routeParams.employeeId)
                    .one('EmployeeTimers', $routeParams.dayId).one("Department", $routeParams.departmentId)
                    .get()
                    .then(function(timers) {
                        NotificationFactory.success("Timer Entry was Successfully Created");
                        vm.timers = timers.items;
                        vm.timers.forEach(organizeTimer);
                    });
                }, function(error) {
                    console.log(error);
                    NotificationFactory.error("Check for overlapping timer entries.");
                });
            });
        });
    };

    function getOrCreateEmployeeTimerID(time, employeeTimerID) {
        var deferred = $q.defer();
        if (!vm.timers) {
            EmployeeTimers.post({ departmentID: $routeParams.departmentId, employeeID: $routeParams.employeeId, day: moment(new Date($routeParams.dayId)).format() })
            .then(function(newTimer) {
                Users.one($routeParams.employeeId)
                .one('EmployeeTimers', $routeParams.dayId).one("Department", $routeParams.departmentId)
                .get()
                .then(function(data) {
                    vm.timers = data.items;
                    vm.timers.forEach(organizeTimer);
                    employeeTimerID = newTimer.employeeTimerID;
                    deferred.resolve(employeeTimerID);
                }, function(error) {
                    deferred.reject(error);
                });
            }, function(error) {
                deferred.reject(error);
            });
        }
        else {
            var employeeTimerID = employeeTimerID || vm.timers[0].employeeTimerID;
            deferred.resolve(employeeTimerID);
        }
        return deferred.promise;
    }

    vm.checkBadDate = function(timerStart, timesheetDate) {
        if (moment(timerStart).format('YYYY-MM-DD') < timesheetDate) {
            timerStart = timesheetDate;
        }
        return timerStart;
    };

    vm.checkClockInClockOut = function(timeCardTimer) {
        if (timeCardTimer.clockOut < timeCardTimer.clockIn) {
            timeCardTimer.invalidEntries = true;
            return true;
        }
        else {
            timeCardTimer.invalidEntries = false;
            return false;
        }
    };

    vm.invalidEntries = function(timers) {
        var disabled;
        if (timers) {
            for (var i = timers.length - 1; i >= 0; i--) {
                var dayTimer = timers[i];
                if (dayTimer.employeeTimerEntries) {
                    vm.invalidClockInClockOut = dayTimer.employeeTimerEntries.some(vm.checkClockInClockOut);
                }

                if (vm.invalidClockInClockOut || vm.invalidTimesheet) {
                    disabled = true;
                }
                return disabled;
            }
        }
    };

    vm.syncDateAndTime = function(item, type, entity, defaultTime) {
        var syncDateAndTime = TimesheetsService.syncDateAndTime(item, type, entity, defaultTime);
        entity[syncDateAndTime.key] = syncDateAndTime.value;
    };

    function _setUpStartAndStopDatesDropDown(modelDateTime) {
        var matchesDayOnTimeCard = (moment(modelDateTime).format('MM-DD-YYYY') == $routeParams.dayId);
        return matchesDayOnTimeCard ? vm.dateList[0] : vm.dateList[1];
    }

    //gets clockin and clock outs
    function _getTimeCard() {

            Users.one($routeParams.employeeId)
            .one('EmployeeTimers', $routeParams.dayId).one("Department", $routeParams.departmentId)
            .get()
            .then(function(timers) {
                vm.timers = timers.items;

                if (vm.timers.length > 0) {
                    vm.timers.forEach(organizeTimer);
                }
                else {
                    var employeeTimerToPost = {
                        departmentID: $routeParams.departmentId,
                        employeeID: $routeParams.employeeId,
                        day: moment(new Date($routeParams.dayId)).format(),
                    };
                    var promises = [
                        EmployeeTimers.post(employeeTimerToPost),
                        Users.one($routeParams.employeeId).get(),
                    ];

                    $q.all(promises).then(function(results) {
                        console.log('new timer created');
                        var newTimer = results[0];
                        newTimer.employee = results[1];
                        vm.timers = [ newTimer ];
                        organizeTimer(newTimer);
                    });
                    
                }
            }, function(error) {
                if (error.exceptionMessage == "The employee timer does not exist in the system.") {
                    var employeeTimerToPost = {
                        departmentID: $routeParams.departmentId,
                        employeeID: $routeParams.employeeId,
                        day: moment(new Date($routeParams.dayId)).format(),
                    };
                    EmployeeTimers.post(employeeTimerToPost)
                    .then(function(newTimer) {
                        vm.timers = [ newTimer ];
                        organizeTimer(newTimer);
                    });
                }
            });

    }

    function _getApprovals() {
        var timerDay = moment($routeParams.dayId).format('YYYY-MM-DD');
        var deferred = $q.defer();

        DailyApprovals.getEmployeeApprovals($routeParams.employeeId, timerDay, $routeParams.departmentId)
        .then(function(employeeApprovals) {
            vm.employeeApprovals = employeeApprovals;
            
            _getNotes()
            .then(function(notes) {
                vm.notes = notes;
                if (_isNoteUnresolved(notes)) {
                }
                else {
                    _setDisabledStatus();
                    _setRejectButton();
                }
            });
            
            deferred.resolve(vm.employeeApprovals);
        });
        
        return deferred.promise;
    }

    function _getNotes() {

        var deferred = $q.defer();
        Notes.getDailyNotesByEmployee(vm.employee)
        .then(function(response) {
            deferred.resolve(response);
        });

        return deferred.promise;
    }

    function _isNoteUnresolved(notes) {
        var hasUnresolvedNotes = false;
        _.each(notes, function(note) {
            if (!note.resolved) hasUnresolvedNotes = true;
        });

        if (hasUnresolvedNotes) {
            $rootScope.notesResolved = false;
        }

        return hasUnresolvedNotes;
    }
    
    function _canApproveSupervisor() {
        return vm.employeeApprovals &&
            vm.can('approveAsSupervisor') &&
            !vm.employeeApprovals.approvedBySupervisor;
    }
    
    function _canApproveBiller() {
        return vm.employeeApprovals &&
            vm.can('approveAsBilling') &&
            vm.employeeApprovals.approvedBySupervisor &&
            !vm.employeeApprovals.approvedByBilling;
    }
    
    function _canApproveAccountant() {
        return vm.employeeApprovals &&
            vm.can('approveAsAccounting') &&
            vm.employeeApprovals.approvedBySupervisor &&
            vm.employeeApprovals.approvedByBilling &&
            !vm.employeeApprovals.approvedByAccounting;
    }

    function _setRejectButton() {
    }

    function _setDisabledStatus() {
        const disableTimers = !(
            _canApproveSupervisor() ||
            _canApproveBiller() ||
            _canApproveAccountant()
        ) && _isNoteUnresolved(vm.notes);
        
        $rootScope.$broadcast('disableTimers', disableTimers);
    }

    function organizeTimer(timer) {
        timer.formattedTotalMinutes = TimesheetsService.convertMinutesToHoursMinutes(timer.totalMinutes);
        //assign timesheetID if it's null
        if (!timer.timesheetID) {
            if (sessionStorage.getItem("cachedReturnToPage") === "foreman-timesheet") {
                const foremanTimesheetID = sessionStorage.getItem("cachedTimesheet");
                EmployeeTimers.one(timer.employeeTimerID)
                .patch({ timesheetID: foremanTimesheetID || null })
                .then(function(timer) {
                    timer.timesheetID = timer.timesheetID;
                }, function(error) {
                    console.log(error);
                });
            }
        }
        //formatDates and sort entries
        timer.employeeTimerEntries =_.sortBy(timer.employeeTimerEntries, function(timerEntry) {
            if (timerEntry.clockOut) {
                timerEntry.clockOut = new Date(timerEntry.clockOut);
                timerEntry.clockOutDate = _setUpStartAndStopDatesDropDown(timerEntry.clockOut);

            }
            if (timerEntry.clockIn) {
                timerEntry.clockIn = new Date(timerEntry.clockIn);
                timerEntry.clockInDate = _setUpStartAndStopDatesDropDown(timerEntry.clockIn);
            }
            return timerEntry.clockIn;
        });
    }
}