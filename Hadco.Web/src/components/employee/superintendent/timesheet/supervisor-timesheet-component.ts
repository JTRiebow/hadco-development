import * as angular from 'angular';
import * as moment from 'moment';
import * as _ from 'lodash';

import * as template from './supervisor-timesheet.html';
import * as deleteTemplate from '../../../Shared/modal-templates/delete-item-modal.html';
import * as cancelTemplate from '../../../Shared/modal-templates/cancel-modal.html';
import * as customMessageTemplate from '../../../Shared/modal-templates/confirm-custom-message-modal.html';
import * as chooseRoleTemplate from '../../../Shared/modal-templates/choose-role-modal.html';
import * as addDetailedNoteTemplate from '../../../Shared/modal-templates/add-detailed-timer-note-modal.html';
import * as addOccurrenceTemplate from '../../../Shared/modal-templates/add-occurrence-modal.html';
import * as superintendentHeaderTemplate from './header-template.html';

angular
    .module('employeeModule')
    .component('htSupervisorTimesheet', {
        controller: supervisorTimesheetController,
        controllerAs: 'vm',
        template,
    });

supervisorTimesheetController.$inject = [
    '$scope',
    '$q',
    'Restangular',
    '$location',
    '$route',
    '$routeParams',
    'NotificationFactory',
    '$modal',
    'OccurrencesHelper',
    'EmployeeTimers',
    'TimesheetsService',
    '$timeout',
    '$rootScope',
    'TimerApprovalsService',
    'PermissionService',
];

function supervisorTimesheetController(
    $scope,
    $q,
    Restangular,
    $location,
    $route,
    $routeParams,
    NotificationFactory,
    $modal,
    OccurrencesHelper,
    EmployeeTimers,
    TimesheetsService,
    $timeout,
    $rootScope,
    TimerApprovalsService,
    PermissionService,
) {

    var vm = this;
    
    vm.can = PermissionService.can;

    init();

    function init() {
        PermissionService.getDepartmentPermissions($routeParams.departmentId);
        
        vm.today = moment().format("MM-DD-YYYY");
        vm.jobDate = $routeParams.dayId;
        vm.formattedDay = moment(vm.jobDate, "MM-DD-YYYY").format('MM/DD/YYYY');
        vm.disabledButton = vm.today == vm.jobDate;
        vm.employeeID = $routeParams.employeeId;
        vm.departmentID = $routeParams.departmentId;
        vm.canEditRow = true;
        vm.masterTimesheet;


        // Get employee
        TimesheetsService.getEmployee(vm.employeeID).then(function(response) {
            vm.foreman = response;
        });

        _getOccurrenceList();

        vm.foremanTimesheetGrid = {
            treeRowHeaderAlwaysVisible: true,
            headerTemplate: superintendentHeaderTemplate,
            enableGridMenu: false,
            enableColumnMenus: false,
            enableSorting: false,
            multiSelect: true,
            rowHeight: 30,
            columnVirtualizationThreshold: 50,
            onRegisterApi: function(gridApi) {
                vm.foremanTimesheetGridApi = gridApi;
            },
        };

        _getTimesheet();
    }

    vm.changeDate = function(action) {
        if (action == 'prev') {
            vm.jobDate = moment(vm.jobDate, "MM-DD-YYYY").subtract(1, 'days').format('MM-DD-YYYY');
        }
        else if (action == 'next') {
            vm.jobDate = moment(vm.jobDate, "MM-DD-YYYY").add(1, 'days').format('MM-DD-YYYY');
        }
        else if (action == 'enter') {
            vm.jobDate = moment(vm.formattedDay).format('MM-DD-YYYY');
        }
        else {
            vm.jobDate = moment(vm.formattedDay).format('MM-DD-YYYY');
        }

        $location.path("/superintendent/foreman/" + vm.employeeID + "/department/" + vm.departmentID + "/day/" + vm.jobDate);

    };



    vm.viewTimeCard = function(entity) {
        sessionStorage.setItem("cachedReturnToPage", "foreman-timesheet");
        sessionStorage.setItem("cachedForemanUrl", $location.url());

        $location.path("/employee/" + entity.employeeID + "/department/" + entity.departmentID + "/day/" + vm.jobDate);
    };

    vm.getTimeLeftToAllocate = function(entity) {
        var sum = 0;
        sum += entity.shopMinutes + entity.greaseMinutes + entity.travelMinutes + entity.dailyMinutes;
        $.each(entity.employeeJobTimers, function(i, emp) {
            sum += emp.laborMinutes;
            if (emp.employeeJobEquipmentTimers) {
                $.each(emp.employeeJobEquipmentTimers, function(i, eq) {
                    sum += eq.equipmentMinutes;
                });
            }
        });
        entity.minuteDifference = entity.totalMinutes - sum;
        return vm.convertMinutesToHours(entity.minuteDifference);
    };

    vm.beginEdit = function(row) {
        if (vm.canEditRow) {
            vm.backupEntity = angular.copy(row.entity);
            vm.canEditRow = false;
            row.entity.editable = true;
            $timeout(function() {
                angular.element("#shopMinutes").focus();
                $("#shopMinutes").select();
            });
        }
    };


    vm.cancelEdit = function(row) {
        vm.canEditRow = true;
        angular.copy(vm.backupEntity, row.entity);
    };

    vm.saveEdit = function(row) {
        _saveEmployeeTimer(row.entity).then(function() {
            row.entity.editable = false;
            vm.canEditRow = true;
        });
    };

    vm.refresh = function() {
        var deferred = $q.defer();

        $timeout(function() {
            _getTimesheet().then(function() {

                deferred.resolve();
            });
        });

        return deferred.promise;
    };

    vm.addOccurrence = function() {
        var modalInstance = $modal.open({
            template: addOccurrenceTemplate,
            controller: 'addOccurrencesModalController',
            windowClass: 'default-modal',
            resolve: {
                availableEmployees: function() {
                    var employeeList = [];
                    angular.forEach(vm.timesheet, function(employee, index) {
                        //Only push employees that don't have occurrences
                        if (!employee.occurrences) {
                            employeeList.push(employee);
                        }
                    });
                    return employeeList;
                },
                availableOccurrences: function() {
                    return vm.occurrences;
                },
            },
        });

        modalInstance.result.then(function(e) {
            for (var i = e.occurrences.length - 1; i >= 0; i--) {
                var url = e.employeeTimerID + '/Occurrences?occurrenceID=' + e.occurrences[i].occurrenceID;
                EmployeeTimers.one().post(url)
                    .then(function(data) {
                        NotificationFactory.success('Occurrence saved');
                        vm.refresh();
                    }, function(data) {
                        NotificationFactory.error("Couldn't save Occurrence");

                    });
            }
        });
    };

    vm.confirmDelete = function(employeeTimer) {
        var modalInstance = $modal.open({
            template: deleteTemplate,
            controller: 'DeleteModalContentController',
            windowClass: 'default-modal',
            resolve: {
                deletedItemName: function() {
                    return employeeTimer.employeeName + "'s occurrences";
                },
            },
        });

        modalInstance.result.then(function(data) {
            vm.deleteAllEmployeeTimerOccurrences(employeeTimer);
        });
    };

    vm.deleteAllEmployeeTimerOccurrences = function(employeeTimer) {

        for (var i = employeeTimer.occurrences.length - 1; i >= 0; i--) {
            EmployeeTimers.one(employeeTimer.employeeTimerID).one('Occurrences', employeeTimer.occurrences[i].occurrenceID).remove()
                .then(function(data) {
                    NotificationFactory.success('Occurrence removed');
                    vm.refresh();
                }, function(data) {
                    NotificationFactory.error("Couldn't remove Occurrence");
                });
        }

    };

    vm.saveOccurrences = function(employeeTimer, index) {
        var occurrencesArray = [];
        for (var i = employeeTimer.occurrences.length - 1; i >= 0; i--) {
            occurrencesArray.push(employeeTimer.occurrences[i].occurrenceID);
        }
        EmployeeTimers.one(employeeTimer.employeeTimerID).one("Occurrences").patch(occurrencesArray)
            .then(function(data) {
                NotificationFactory.success('Occurrences saved');
                vm.refresh();
            }, function(data) {
                NotificationFactory.error("Couldn't save Occurrences");
            });
    };

    function _createNewTimesheet() {
        var modalInstance = $modal.open({
            controller: 'confirmCustomMessageController',
            controllerAs: 'vm',
            template: customMessageTemplate,
            windowClass: 'default-modal',
            resolve: {
                customMessage: function() {
                    return "This employee doesn't have a timesheet for " + vm.jobDate + ". Please confirm that you'd like to create one.";
                },
            },
        });

        modalInstance.result.then(function() {
            console.log('success after modal close');
            TimesheetsService.createNewTimesheet(vm.jobDate, vm.employeeID, vm.departmentID)
                .then(function(data) {
                    //Make call to add supervisor to timesheet
                    var newEmployeeTimer = {
                        timesheetID: data.timesheetID,
                        day: vm.jobDate,
                        employeeID: vm.employeeID,
                        departmentID: vm.departmentID,
                    };
                    EmployeeTimers.post(newEmployeeTimer).then(function(response) {
                        $route.reload();
                    });
                }, function(error) {
                    console.log('error');
                });
        }, function() {
            console.log('error after modal close');
        });
    }

    vm.convertMinutesToHours = function(minutes) {
        return TimesheetsService.convertMinutesToHoursMinutes(minutes);
    };
    //Need to be on scope instead of VM for type ahead directive



    $scope.updateTotalRemainingTime = function(rowEntity) {
        rowEntity.formattedTimeLeftToAllocate = vm.getTimeLeftToAllocate(rowEntity);

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
                vm.saveEdit(row);
            }
            else {
                vm.cancelEdit(row);
            }
        });
    };

    //Need to be on scope instead of VM for type ahead directive
    $scope.enterKeypressSave = function(row, type, shiftEnter) {
        _saveEmployeeTimer(row.entity).then(function() {
            row.entity.editable = false;
            vm.canEditRow = true;

            var startRow = row.entity.sequence + 1;
            if (row.grid.rows[startRow] && shiftEnter) {
                var newRowToOpen = row.grid.rows[startRow];
                vm.beginEdit(newRowToOpen);

                $timeout(function() {
                    angular.element("#shopMinutes").focus();
                    $("#shopMinutes").select();
                });
            }
        });
    };

    function _getOccurrenceList() {
        OccurrencesHelper.getList()
            .then(function(data) {
                vm.occurrences = data;
            });
    }
    function _getTimesheet() {
        var deferred = $q.defer();
        EmployeeTimers.one('Foreman').one(vm.employeeID).one('Department').one(vm.departmentID).one('Day').one(vm.jobDate).get().then(function(response) {
            vm.timesheet = response.employeeTimers;
            vm.masterTimesheet = angular.copy(vm.timesheet);
            _getApprovals();
            vm.timesheetId = response.timesheetID;
            _setForemanEmployeeTimerID();
            angular.forEach(vm.timesheet, function(row, index) {
                row.formattedTotalMinutes = vm.convertMinutesToHours(row.totalMinutes);
                row.formattedTimeLeftToAllocate = vm.getTimeLeftToAllocate(row);
                row.sequence = index;
            });
            if (vm.timesheet[0]) {
                _getColumnDefs(vm.timesheet[0].employeeJobTimers);

            }
            vm.foremanTimesheetGrid.data = vm.timesheet;
            console.log("timesheet data");
            console.log(vm.timesheet);
            console.log("timesheet data");
            deferred.resolve();
        }, function(error) {
            vm.foremanTimesheetGrid.data = [];
            vm.timesheet = [];
            vm.foremanTimesheetGrid.columnDefs = [];
            _createNewTimesheet();
            deferred.resolve();
        });
        return deferred.promise;
    }

    function _setForemanEmployeeTimerID() {
        angular.forEach(vm.timesheet, function(value, index) {
            if (value.employeeID == vm.employeeID) {
                vm.foremanEmployeeTimerID = value.employeeTimerID;
                return;
            }
        });
    }

    function _saveEmployeeTimer(entity) {
        var masterTimesheetIndex;
        for (var i = 0; i < vm.masterTimesheet.length; i++) {
            if (vm.masterTimesheet[i].employeeID == entity.employeeID) {
                masterTimesheetIndex = i;
                break;
            }
        }

        for (var i = 0; i < entity.employeeJobTimers.length; i++) {
            var masterJobTimer = vm.masterTimesheet[masterTimesheetIndex].employeeJobTimers[i];
            var entityJobTimer = entity.employeeJobTimers[i];
            if (masterJobTimer.jobTimerID == entityJobTimer.jobTimerID) {
                if (masterJobTimer.laborMinutes == null && entityJobTimer.laborMinutes < 1) {
                    entity.employeeJobTimers[i].laborMinutes = vm.masterTimesheet[masterTimesheetIndex].employeeJobTimers[i].laborMinutes;
                }
            }
            if (entity.employeeJobTimers[i].employeeJobEquipmentTimers !== null) {
                for (var j = 0; j < entity.employeeJobTimers[i].employeeJobEquipmentTimers.length; j++) {
                    var masterJobEquipmentTimer = vm.masterTimesheet[masterTimesheetIndex].employeeJobTimers[i]
                        .employeeJobEquipmentTimers[j];
                    var entityJobEquipmentTimer = entity.employeeJobTimers[i].employeeJobEquipmentTimers[j];
                    if (masterJobEquipmentTimer.jobTimerID == entityJobEquipmentTimer.jobTimerID) {
                        if (masterJobEquipmentTimer.equipmentMinutes == null &&
                            entityJobEquipmentTimer.equipmentMinutes < 1) {
                            entity.employeeJobTimers[i].employeeJobEquipmentTimers[j].equipmentMinutes =
                                vm.masterTimesheet[masterTimesheetIndex].employeeJobTimers[i]
                                .employeeJobEquipmentTimers[j].equipmentMinutes;
                        }
                    }
                }
            }
        }
        var deferred = $q.defer();
        EmployeeTimers.one(entity.employeeTimerID).one('Foreman').patch(entity).then(function(response) {
            response.formattedTotalMinutes = vm.convertMinutesToHours(response.totalMinutes);
            response.formattedTimeLeftToAllocate = vm.getTimeLeftToAllocate(response);
            vm.foremanTimesheetGrid.data[entity.sequence] = response;
            NotificationFactory.success('Employee timer has been saved');
            deferred.resolve();
        }, function(error) {
            NotificationFactory.error('There was an error saving the employee timer');
            deferred.reject();
        });
        return deferred.promise;
    }

    vm.getCheckColor = function(employee) {
        var role = $rootScope.me.roles;
        if (employee.hasNote) return 'orange-text';

        if ((role.isSupervisor() || role.isManager()) && role.isBilling() && role.isAccounting()) {
            if (employee.approvedBySupervisor && employee.approvedByBilling && employee.approvedByAccounting) return 'green-text';

        }
        else if ((role.isSupervisor() || role.isManager()) && role.isBilling()) {
            if (employee.approvedBySupervisor && employee.approvedByBilling) return 'green-text';
        }
        else if ((role.isSupervisor() || role.isManager()) && role.isAccounting()) {
            if (employee.approvedBySupervisor && employee.approvedByBilling && employee.approvedByAccounting) return 'green-text';
            else if (!employee.approvedByBilling) return 'red-text';
        }
        else if (role.isBilling() && role.isAccounting()) {
            if (employee.approvedBySupervisor && employee.approvedByBilling && employee.approvedByAccounting) return 'green-text';
            else if (!employee.approvedBySupervisor) return 'red-text';
        }
        else if ((role.isSupervisor() || role.isManager())) {
            if (employee.approvedBySupervisor) return 'green-text';
        }
        else if (role.isBilling()) {
            if (employee.approvedBySupervisor && employee.approvedByBilling) return 'green-text';
            else if (!employee.approvedBySupervisor) return 'red-text';
        }
        else if (role.isAccounting()) {
            if (employee.approvedBySupervisor && employee.approvedByBilling && employee.approvedByAccounting) return 'green-text';
            else if (!employee.approvedBySupervisor || !employee.approvedByBilling) return 'red-text';
        }

        return 'grey-text';

    };

    function _isApprovedByAll(supervisor, billing, accounting) {
        if (supervisor && billing && accounting) {
            return true;
        }
        else if (supervisor && billing && accounting === undefined) {
            return true;
        }
        else if (supervisor && billing === undefined && accounting === undefined) {
            return true;
        }
        else if (billing && supervisor === undefined && accounting === undefined) {
            return true;
        }
        else if (accounting && supervisor === undefined && billing === undefined) {
            return true;
        }
        return false;

    }

    vm.showTooltip = function(employee) {
        vm.name = employee.employeeName;
    };

    vm.approve = function(employee, approveAll) {
        if (!employee.hasNote) {
            var timerDay = moment(employee.day).format("YYYY-MM-DD");

            if (!approveAll && _userHasMultipleRoles()) {
                _openRolesModal(employee);
            }
            else {
                var approvalsStatus = _priorApprovalsRequiredForSingleRole(employee);
                if (!approvalsStatus.approvalsRequired) {
                    var getApprovalRoleObjToPatch = _getApprovalRoleObjToPatch(employee);

                    return TimerApprovalsService.patch(employee, getApprovalRoleObjToPatch);
                }
                else {
                    NotificationFactory.error("Prior approval is required: " + approvalsStatus.rolesRequired);
                }

            }
        }
        else if (!approveAll) {
            NotificationFactory.error("Must resolve notes prior to approving");
        }
    };
    
    vm.approveAll = function() {
        if (_userHasMultipleRoles()) {
            return _openRolesModal(vm.timesheet);
        }
        
        vm.timesheet.forEach(function(e) {
            vm.approve(e, true).then(response => {
            }, error => {
                console.log(`Error approving timesheet for: ${e.employeeName}.  ${error.data.message}`)
            })
        });
    };

    vm.flagTime = function(employee, reject) {
        _openNotesModal(employee, reject);
    };

    vm.reject = function(employee, reject) {
        var modalInstance = _openNotesModal(employee, reject);

    };

    function _priorApprovalsRequiredForSingleRole(employee) {
        var priorApprovals;
        var roles = $rootScope.me.roles;

        if (roles.isSupervisor() || roles.isManager()) {
            return true;
        }
        else if (roles.isBilling()) {
            if (!employee.approvedBySupervisor) {
                priorApprovals = {
                    'approvalsRequired': true,
                    'rolesRequired': 'Supervisor',
                };
            }
        }
        else if (roles.isAccounting()) {
            if (!employee.approvedBySupervisor || !employee.approvedByBilling) {
                priorApprovals = {
                    'approvalsRequired': true,
                };
                if (!employee.approvedBySupervisor && !employee.approvedByBilling) {
                    priorApprovals['rolesRequired'] = 'Supervisor, Billing';
                }
                else if (!employee.approvedBySupervisor) {
                    priorApprovals['rolesRequired'] = 'Supervisor';
                }
                else if (!employee.approvedByBilling) {
                    priorApprovals['rolesRequired'] = 'Billing';
                }
            }
        }
        return priorApprovals;
    }

    function _priorApprovalsRequiredForMultipleRoles(employee, approvals) {

        var proposedApprovals = {} as any;
        if (approvals.supervisor) {
            proposedApprovals['approvedBySupervisor'] = true;
            //employee.approvedBySupervisor = true;
        }

        if (approvals.billing) {
            proposedApprovals['approvedByBilling'] = true;
            //employee.approvedByBilling = true;
        }

        if (approvals.accounting) {
            proposedApprovals['approvedByAccounting'] = true;
            //employee.approvedByAccounting = true;
        }

        var priorApprovals = {} as any;

        var currentUserRole = $rootScope.me.roles;

        if ((currentUserRole.isSupervisor() || currentUserRole.isManager()) && currentUserRole.isBilling() && currentUserRole.isAccounting()) {
            if ((proposedApprovals.approvedByBilling && proposedApprovals.approvedByAccounting) || proposedApprovals.approvedByBilling) {
                if (!employee.approvedBySupervisor) {
                    priorApprovals['approvalsRequired'] = true;
                    priorApprovals['rolesRequired'] = 'Supervisor';
                }
            }
            else if (proposedApprovals.approvedByAccounting) {
                if (!employee.approvedBySupervisor && !employee.approvedByBilling) {
                    priorApprovals['approvalsRequired'] = true;
                    priorApprovals['rolesRequired'] = 'Supervisor, Billing';
                }
                else if (!employee.approvedBySupervisor) {
                    priorApprovals['approvalsRequired'] = true;
                    priorApprovals['rolesRequired'] = 'Supervisor';
                }
                else if (!employee.approvedByBilling) {
                    priorApprovals['approvalsRequired'] = true;
                    priorApprovals['rolesRequired'] = 'Billing';
                }
            }
        }
        else if ((currentUserRole.isSupervisor() || currentUserRole.isManager()) && currentUserRole.isBilling()) {
            if (proposedApprovals.approvedByBilling) {
                if (!employee.approvedBySupervisor) {
                    priorApprovals['approvalsRequired'] = true;
                    priorApprovals['rolesRequired'] = 'Supervisor';
                }
            }
        }
        else if (currentUserRole.isBilling() && currentUserRole.isAccounting()) {
            if ((proposedApprovals.approvedByBilling && proposedApprovals.approvedByAccounting) || proposedApprovals.approvedByBilling) {
                if (!employee.approvedBySupervisor) {
                    priorApprovals['approvalsRequired'] = true;
                    priorApprovals['rolesRequired'] = 'Supervisor';
                }
            }
            else if (proposedApprovals.approvedByAccounting) {
                if (!employee.approvedBySupervisor && !employee.approvedByBilling) {
                    priorApprovals['approvalsRequired'] = true;
                    priorApprovals['rolesRequired'] = 'Supervisor, Billing';
                }
                else if (!employee.approvedBySupervisor) {
                    priorApprovals['approvalsRequired'] = true;
                    priorApprovals['rolesRequired'] = 'Supervisor';
                }
                else if (!employee.approvedByBilling) {
                    priorApprovals['approvalsRequired'] = true;
                    priorApprovals['rolesRequired'] = 'Billing';
                }
            }

        }
        else if ((currentUserRole.isSupervisor() || currentUserRole.isManager()) && currentUserRole.isAccounting()) {
            if (proposedApprovals.approvedByAccounting) {
                if (!employee.approvedBySupervisor && !employee.approvedByBilling) {
                    priorApprovals['approvalsRequired'] = true;
                    priorApprovals['rolesRequired'] = 'Supervisor, Billing';
                }
                else if (!employee.approvedBySupervisor && !proposedApprovals.approvedBySupervisor) {
                    priorApprovals['approvalsRequired'] = true;
                    priorApprovals['rolesRequired'] = 'Supervisor';
                }
                else if (!employee.approvedByBilling) {
                    priorApprovals['approvalsRequired'] = true;
                    priorApprovals['rolesRequired'] = 'Billing';
                }
            }
        }

        return priorApprovals;
    }

    function _getApprovalRoleObjToPatch(employee) {
        var approvalsObj = {};
        var currentUserRole = $rootScope.me.roles;

        if ((currentUserRole.isSupervisor() || currentUserRole.isManager())) {
            approvalsObj['approvedBySupervisor'] = true;
            employee.approvedBySupervisor = true;
        }
        else if (currentUserRole.isBilling()) {
            approvalsObj['approvedByBilling'] = true;
            employee.approvedByBilling = true;
        }
        else if (currentUserRole.isAccounting()) {
            approvalsObj['approvedByAccounting'] = true;
            employee.approvedByAccounting = true;
        }

        return approvalsObj;
    }

    function _userHasMultipleRoles() {
        var currentUserRole = $rootScope.me.roles;
        if ((currentUserRole.isSupervisor() || currentUserRole.isManager()) && currentUserRole.isBilling() && currentUserRole.isAccounting()) {
            return true;
        }
        else if ((currentUserRole.isSupervisor() || currentUserRole.isManager()) && currentUserRole.isBilling()) {
            return true;
        }
        else if (currentUserRole.isBilling() && currentUserRole.isAccounting()) {
            return true;
        }
        else if ((currentUserRole.isSupervisor() || currentUserRole.isManager()) && currentUserRole.isAccounting()) {
            return true;
        }

        return false;
    }

    function _getApprovals() {
        var deferred = $q.defer();
        vm.timesheetApprovals = [];

        _.each(vm.timesheet, function(timer) {
            var timerDay = moment(timer.day).format("YYYY-MM-DD");
            Restangular.one("Employee", timer.employeeID).one("Day", timerDay).one("Department", timer.departmentID).one("DailyApproval").get()
                .then(function(response) {
                    vm.timesheetApprovals.push(response);
                });
        });
        deferred.resolve();
        return vm.timesheetApprovals;
    }

    function _openRolesModal(employee) {
        var modalInstance = $modal.open({
            controller: 'chooseRoleModalController',
            controllerAs: 'vm',
            template: chooseRoleTemplate,
            windowClass: 'default-modal',
        });

        modalInstance.result.then(function(response) {
            if (response !== 'cancel') {
                if (Array.isArray(employee)) {
                    return employee.forEach(function(e) {
                        if (_isApprovalQueueValid(response, e)) {
                            _postSelectedApprovals(response, e).then(response => {
                                e.approvedBySupervisor = response.approvedBySupervisor
                                e.approvedByBilling = response.approvedByBilling
                                e.approvedByAccounting = response.approvedByAccounting
                            }, error => {
                                NotificationFactory.error(`Unable to Approve As: ${getApprovalRequests(response, e)}`)
                            })
                        }
                        else {
                            var approvalStatus = _priorApprovalsRequiredForMultipleRoles(employee, response)
                            NotificationFactory.error(`Prior Approvals Required: ${approvalStatus.rolesRequired}`)
                        }
                    });
                }
                if (_isApprovalQueueValid(response, employee)) {
                    _postSelectedApprovals(response, employee).then(response => {
                        employee.approvedBySupervisor = response.approvedBySupervisor
                        employee.approvedByBilling = response.approvedByBilling
                        employee.approvedByAccounting = response.approvedByAccounting
                    }, error => {
                        NotificationFactory.error(`Unable to Approve As: ${getApprovalRequests(response, employee)}`)
                    })
                }
                else {
                    var approvalStatus = _priorApprovalsRequiredForMultipleRoles(employee, response)
                    NotificationFactory.error(`Prior Approvals Required: ${approvalStatus.rolesRequired}`)
                }
            }
        });
    }

    function _isApprovalQueueValid(approvals, employee) {
        let supervisorValid = employee.approvedBySupervisor || (approvals.supervisor || approvals.manager)
        let billingValid = (employee.approvedByBilling || approvals.billing) && supervisorValid
        let accountingValid = (employee.approvedByAccounting || approvals.accounting) && billingValid

        var result = true
        if(approvals.accounting) {
            result = accountingValid
        } else if(approvals.billing) {
            result = billingValid
        } else if(approvals.supervisor || approvals.manager) {
            result = supervisorValid
        } 

        return result
    }

    function _postSelectedApprovals(approvals, employee) {
        var approvalsObj = {};
        let copy = Object.assign({}, employee)

        if (approvals.supervisor || approvals.manager) {
            approvalsObj['approvedBySupervisor'] = true;
            copy.approvedBySupervisor = true;
        }

        if (approvals.billing) {
            approvalsObj['approvedByBilling'] = true;
            copy.approvedByBilling = true;
        }

        if (approvals.accounting) {
            approvalsObj['approvedByAccounting'] = true;
            copy.approvedByAccounting = true;
        }

        return TimerApprovalsService.patch(copy, approvalsObj);
    }

    function getApprovalRequests(approvals, employee) {
        var approvalsRequested = []

        if(approvals.supervisor) {
            approvalsRequested.push('Supervisor')
        }
        if (approvals.manager) {
            approvalsRequested.push('Manager')
        }
        if (approvals.billing) {
            approvalsRequested.push('Billing')
        }
        if (approvals.accounting) {
            approvalsRequested.push('Accounting')
        }

        return approvalsRequested.join(', ')
    }

    function _openNotesModal(employee, reject) {
        var modalInstance = $modal.open({
            controller: 'addDetailedTimerNoteModalController',
            controllerAs: 'vm',
            template: addDetailedNoteTemplate,
            windowClass: 'default-modal',
            resolve: {
                employeeID: function() {
                    return employee.employeeID;
                },
                departmentID: function() {
                    return employee.departmentID;
                },
                timerDay: function() {
                    return employee.day;
                },
                rejectNote: function() {
                    return reject;
                },
                currentApprovals: () => employee,
            },
        });


        modalInstance.result.then(function(response) {
            if (reject) {
                var approvalsObj;
                switch (response.name) {
                    case 'Supervisor':
                        approvalsObj = { "approvedBySupervisor": false };
                        employee.approvedBySupervisor = false;
                        break;
                    case 'Biller':
                        approvalsObj = { "approvedByBilling": false };
                        employee.approvedByBilling = false;
                        break;
                    case 'Accountant':
                        approvalsObj = { "approvedByAccounting": false };
                        employee.approvedByAccounting = false;
                        break;
                }

                TimerApprovalsService.patch(employee, approvalsObj);
            }
            else {
                employee.hasNote = true;
            }
        })
        .catch(angular.noop);


    }

    function _getColumnDefs(employeeJobTimers) {
        vm.foremanTimesheetGrid.category = [];
        vm.foremanTimesheetGrid.columnDefs = [];

        vm.foremanTimesheetGrid.category = [
            { name: 'employee', displayName: 'Employee', visible: true, showCatName: false },
            { name: 'time', displayName: 'Time', visible: true, showCatName: true },
            { name: 'overhead', displayName: 'Overhead', visible: true, showCatName: true },
        ];
        vm.foremanTimesheetGrid.columnDefs.push(
            {
                name: 'Employee Name', category: 'employee', field: 'employeeName', width: 240, pinnedLeft: true,
                cellTemplate: '<div class="ui-grid-cell-contents" ng-click="grid.appScope.vm.showTooltip(row.entity)">' +
                '{{COL_FIELD}}' +
                '<div class="ellipses-tooltip" ng-if="row.entity.employeeName === grid.appScope.vm.name" >' +
                '<div class="ellipses-tooltip-text" ng-mouseleave="grid.appScope.vm.name = \'\' ">' +
                '<ul><li ng-click="grid.appScope.vm.viewTimeCard(row.entity)">View Timecard</li>' +
                '<li ng-click="grid.appScope.vm.approve(row.entity)">Approve</li>' +
                '<li ng-click="grid.appScope.vm.flagTime(row.entity, false)">Flag for Review</li>' +
                '<li ng-click="grid.appScope.vm.reject(row.entity, true)">Reject</li></ul>' +
                '</div></div>' +

                '<i class="fa fa-check pull-right" ng-class="grid.appScope.vm.getCheckColor(row.entity)" aria-hidden="true"></i>' +

                '<i ng-if="row.entity.hasNote" class="fa fa-sticky-note-o cell-note pull-right" aria-hidden="true"></i>' +
                '</div>',
            },
            { name: 'Start Time', category: 'time', field: 'startTime', type: 'date', cellFilter: 'date: "hh:mm a"', width: 150 },
            { name: 'Stop Time', category: 'time', field: 'endTime', type: 'date', cellFilter: 'date: "hh:mm a"', width: 150 },
            {
                name: 'Shop', category: 'overhead', field: 'shopMinutes', width: 100,
                cellTemplate: '<ui-grid-hour-minute row="row" focus-id="shopMinutes" is-cell-editable="row.entity.editable" original-minutes="row.entity.shopMinutes" on-change-function="grid.appScope.updateTotalRemainingTime"></ui-grid-hour-minute>',
            },
            {
                name: 'Grease', category: 'overhead', field: 'greaseMinutes', width: 100,
                cellTemplate: '<ui-grid-hour-minute row="row" original-minutes="row.entity.greaseMinutes" is-cell-editable="row.entity.editable" on-change-function="grid.appScope.updateTotalRemainingTime"></ui-grid-hour-minute>',
            },
            {
                name: 'Travel', category: 'overhead', field: 'travelMinutes', width: 100,
                cellTemplate: '<ui-grid-hour-minute row="row" original-minutes="row.entity.travelMinutes" is-cell-editable="row.entity.editable" on-change-function="grid.appScope.updateTotalRemainingTime"></ui-grid-hour-minute>',
            },
            {
                name: 'Daily', category: 'overhead', field: 'dailyMinutes', width: 100,
                cellTemplate: '<ui-grid-hour-minute row="row" original-minutes="row.entity.dailyMinutes" is-cell-editable="row.entity.editable" on-change-function="grid.appScope.updateTotalRemainingTime"></ui-grid-hour-minute>',
            }
        );

        _populateJobColumns(employeeJobTimers);
        console.log(employeeJobTimers.length);

        vm.foremanTimesheetGrid.category.push(
            { name: 'card', displayName: 'Timecard', visible: true, showCatName: true },
            { name: 'action', displayName: 'action', visible: true, showCatName: false }
        );

        if (employeeJobTimers.length < 10) {
            vm.foremanTimesheetGrid.columnDefs.push(
                {
                    name: 'Total Time', category: 'card', field: 'totalMinutes', width: 150,
                    cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.formattedTotalMinutes}}</div>',
                },
                { name: 'Injured', category: 'card', field: 'injured', width: 100 },
                {
                    name: 'Remaining', category: 'action', width: 100, pinnedRight: true,
                    cellTemplate: '<div class="ui-grid-cell-contents" ng-class="{ \'warning\': row.entity.minuteDifference < 0 }">{{row.entity.formattedTimeLeftToAllocate}}</div>',
                },
                {
                    name: 'Edit', category: 'action', width: 150, pinnedRight: true,
                    cellTemplate:
                    '<a ng-disabled="!grid.appScope.vm.canEditRow" ng-class="{ \'hide\': row.entity.editable || !row.entity.minuteDifference != 0 }" ' +
                    'class="fa fa-lg fa-flag icon" data-toggle="tooltip" title="Allocation Error" aria-hidden="true" ng-click="grid.appScope.vm.beginEdit(row)"></a>' +
                    '<i ng-disabled="!grid.appScope.vm.canEditRow" ng-class="{ \'hide\': row.entity.editable }"' +
                    'class="fa fa-lg fa-pencil icon" data-toggle="tooltip" title="Edit" aria-hidden="true" ng-click="grid.appScope.vm.beginEdit(row)"></i>' +
                    '<i ng-class="{ \'hide\': !row.entity.editable }"' +
                    'class="fa fa-lg fa-floppy-o icon" data-toggle="tooltip" title="Save" aria-hidden="true" ng-click="grid.appScope.vm.saveEdit(row)" ng-disabled="row.entity.minuteDifference < 0"></i>' +
                    '<i ng-class="{ \'hide\': !row.entity.editable }"' +
                    'class="fa fa-lg fa-ban icon" data-toggle="tooltip" title="Cancel" aria-hidden="true" ng-click="grid.appScope.vm.cancelEdit(row)"></i>',
                }
            );
        }
        else {
            vm.foremanTimesheetGrid.columnDefs.push(
                {
                    name: 'Total Time', category: 'card', field: 'totalMinutes', width: 150, pinnedRight: true,
                    cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.formattedTotalMinutes}}</div>',
                },
                { name: 'Injured', category: 'card', field: 'injured', width: 100, pinnedRight: true },
                {
                    name: 'Remaining', category: 'action', width: 100, pinnedRight: true,
                    cellTemplate: '<div class="ui-grid-cell-contents" ng-class="{ \'warning\': row.entity.minuteDifference < 0 }">{{row.entity.formattedTimeLeftToAllocate}}</div>',
                },
                {
                    name: 'Edit', category: 'action', width: 150, pinnedRight: true,
                    cellTemplate:
                    '<a ng-disabled="!grid.appScope.vm.canEditRow" ng-class="{ \'hide\': row.entity.editable || !row.entity.minuteDifference != 0 }" ' +
                    'class="fa fa-lg fa-flag icon" data-toggle="tooltip" title="Allocation Error" aria-hidden="true" ng-click="grid.appScope.vm.beginEdit(row)"></a>' +
                    '<i ng-disabled="!grid.appScope.vm.canEditRow" ng-class="{ \'hide\': row.entity.editable }"' +
                    'class="fa fa-lg fa-pencil icon" data-toggle="tooltip" title="Edit" aria-hidden="true" ng-click="grid.appScope.vm.beginEdit(row)"></i>' +
                    '<i ng-class="{ \'hide\': !row.entity.editable }"' +
                    'class="fa fa-lg fa-floppy-o icon" data-toggle="tooltip" title="Save" aria-hidden="true" ng-click="grid.appScope.vm.saveEdit(row)" ng-disabled="row.entity.minuteDifference < 0"></i>' +
                    '<i ng-class="{ \'hide\': !row.entity.editable }"' +
                    'class="fa fa-lg fa-ban icon" data-toggle="tooltip" title="Cancel" aria-hidden="true" ng-click="grid.appScope.vm.cancelEdit(row)"></i>',
                }
            );
        }
    }

    function _populateJobColumns(employeeJobTimers) {
        angular.forEach(employeeJobTimers, function(employeeJobTimer, employeeJobTimerIndex) {
            var categoryName = 'job' + employeeJobTimer.jobTimerID;
            var displayName = employeeJobTimer.jobPhaseCategory;
            var columnWidth = _getTextWidth(displayName);

            vm.foremanTimesheetGrid.category.push({ name: categoryName, displayName: displayName, visible: true, showCatName: true });

            if (employeeJobTimer.employeeJobEquipmentTimers !== undefined) {
                angular.forEach(employeeJobTimer.employeeJobEquipmentTimers, function(jobEquipmentTimer, jobEquipmentTimerIndex) {
                    var equipmentMinutes = 'employeeJobTimers[' + employeeJobTimerIndex + '].employeeJobEquipmentTimers[' + jobEquipmentTimerIndex + '].equipmentMinutes';
                    var rowEquipmentMinutes = 'row.entity.' + equipmentMinutes;
                    vm.foremanTimesheetGrid.columnDefs.push({
                        name: 'equip' + employeeJobTimerIndex + jobEquipmentTimerIndex, field: equipmentMinutes,
                        category: categoryName, displayName: jobEquipmentTimer.equipmentNumber, width: columnWidth,
                        cellTemplate: '<ui-grid-hour-minute is-cell-editable="row.entity.editable" row="row" original-minutes="' + rowEquipmentMinutes + '" on-change-function="grid.appScope.updateTotalRemainingTime"></ui-grid-hour-minute>',
                    });
                    
                        
                });
            }

            var laborMinutes = 'employeeJobTimers[' + employeeJobTimerIndex + '].laborMinutes';
            var rowLaborMinutes = 'row.entity.' + laborMinutes;
            vm.foremanTimesheetGrid.columnDefs.push({
                category: categoryName, name: 'labor' + employeeJobTimerIndex, field: laborMinutes,
                displayName: 'Labor', type: "number", width: columnWidth,
                cellTemplate: '<ui-grid-hour-minute row="row" is-cell-editable="row.entity.editable" original-minutes="' + rowLaborMinutes + '" on-change-function="grid.appScope.updateTotalRemainingTime"></ui-grid-hour-minute>',
            });

        });

    }


    function _getTextWidth(text) {
        // if given, use cached canvas for better performance
        // else, create new canvas
        var canvas = (_getTextWidth as any).canvas || ((_getTextWidth as any).canvas = document.createElement("canvas"));
        var context = canvas.getContext("2d");
        context.font = "16px Helvetica Neu bold";
        var metrics = context.measureText(text);
        return metrics.width + 40;
    }

}