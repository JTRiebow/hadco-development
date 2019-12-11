import * as angular from 'angular';
import * as moment from 'moment';
import * as _ from 'lodash';
import * as $ from 'jquery';

import * as template from './timer-approval.html';
import * as customMessageTemplate from '../Shared/modal-templates/confirm-custom-message-modal.html';
import * as editableTextTemplate from '../Shared/modal-templates/editable-text-modal.html';
import * as newTimerTemplate from '../Shared/modal-templates/new-timer-modal.html';
import { IPermissionService } from '../permissions/permission-service';
import { ICurrentUserService } from '../current-user/current-user-factory';
import { IActivityService } from '../permissions/activity-service';

angular
    .module('timerApprovalModule')
    .component('timerApproval', {
        controller: TimerApprovalController,
        controllerAs: 'vm',
        template,
    });

TimerApprovalController.$inject = [
    '$scope',
    '$rootScope',
    '$routeParams',
    '$location',
    '$modal',
    'CurrentUser',
    'NotificationFactory',
    'Restangular',
    'EmployeeTimecards',
    'DepartmentsHelper',
    'TimerApprovalsService',
    'PermissionService',
    'ActivityService',
    'Users'
];

function TimerApprovalController(
    $scope,
    $rootScope: ng.IRootScopeService,
    $routeParams,
    $location,
    $modal,
    CurrentUser: ICurrentUserService,
    NotificationFactory,
    Restangular,
    EmployeeTimecards,
    DepartmentsHelper,
    TimerApprovalsService,
    PermissionService: IPermissionService,
    ActivityService: IActivityService,
    Users
) {
    var cachedValues = {} as any;
    const tooltipActions = [
        'viewEmployeeTimer',
        'approveEmployeeTimer',
        'flagEmployeeTimer',
    ];
    
    $scope.filteredData = {};
    $scope.selectedTimers = [];
    $scope.users = [];
    $scope.disableAddTimerButton = true;

    var vm = this;
    
    vm.dayKeys = [
        6,
        0,
        1,
        2,
        3,
        4,
        5,
    ];
    
    vm.can = PermissionService.can;
    vm.canSeeTooltip = canSeeTooltip();
    
    init()

    async function init() {
        let pageRequiredPermissions = getPageRequiredPermissions();
        await PermissionService.getManyDepartmentPermissions(pageRequiredPermissions);
        _isSupervisorOrAccountingPage();
        PermissionService.redirectIfUnauthorized([$scope.viewPagePermission])
        _getStoredData();
        _getLists();
        _getTimeCards($scope.selectedWeek, $scope.filter);
        vm.approvedTimeCards = [];
        vm.unapprovedTimeCards = [];
    }

    PermissionService.getDepartmentPermissionBreakdown(PermissionService.timerApprovalPageActions)
        .then(perms => {
            vm.departmentActivities = perms;
            vm.departmentActivityMap = perms.reduce((m, p) => ({ ...m, [p.activityID]: p }), {});
        })

    Users.getList()
        .then(function (data) {
            $scope.users = data
            $scope.disableAddTimerButton = false
        });

    $scope.getCheckmarkColor = function(employee) {
        var timeCard = _getTimeCardInformation(employee);
        
        if (!timeCard.needsPriorApproval && !timeCard.flagged && timeCard.approved) vm.approvedTimeCards.push(employee.employeeTimecardID);

        if (vm.approvedTimeCards.includes(employee.employeeTimecardID) && !timeCard.needsPriorApproval && !timeCard.flagged) return 'green-text';
        else if (timeCard.needsPriorApproval) return 'red-text';
        else if (timeCard.flagged) return 'yellow-text';
        else return 'grey-text';
    };

    $scope.approveWeekTimers = function(employee, approvingAll) {
        if (!approvingAll) {
            var modalInstance = $modal.open({
                controller: 'confirmCustomMessageController',
                controllerAs: 'vm',
                template: customMessageTemplate,
                windowClass: 'default-modal',
                resolve: {
                    customMessage: function() {
                        return "Are you sure you want to approve all time cards for this employee for this week?";
                    },
                },
            });

            return modalInstance.result
                .then(function() {
                    var timeCard = _getTimeCardInformation(employee);

                    if (vm.approvedTimeCards.includes(employee.employeeTimecardID) && !timeCard.needsPriorApproval && !timeCard.flagged) { }
                    else if (timeCard.needsPriorApproval) { }
                    else if (timeCard.flagged) { }
                    else {

                        if (!vm.timeCardNeedsPriorApproval && !vm.timeCardIsFlagged) {
                            vm.approvedTimeCards.push(employee.employeeTimecardID);

                            var days = _getValidEmployeeTimers(employee);
                            for (var i = 0; i < days.length; i++) {
                                $scope.approveTimer(employee, days[i].day);
                            }
                        }
                    }
                }, angular.noop);
        }
        
        var timeCard = _getTimeCardInformation(employee);

        if (vm.approvedTimeCards.includes(employee.employeeTimecardID) && !timeCard.needsPriorApproval && !timeCard.flagged) { }
        else if (timeCard.needsPriorApproval) { }
        else if (timeCard.flagged) { }
        else {

            if (!vm.timeCardNeedsPriorApproval && !vm.timeCardIsFlagged) {
                vm.approvedTimeCards.push(employee.employeeTimecardID);

                var days = _getValidEmployeeTimers(employee);
                for (var i = 0; i < days.length; i++) {
                    $scope.approveTimer(employee, days[i].day);
                }
            }
        }
    };
    
    $scope.getTotalHours = function(e) {
        return Object.keys(e)
            .filter(k => k.match(/day\d/) && e[k])
            .reduce((t, k) => t + +e[k].totalHours, 0)
            .toFixed(2);
    };

    $scope.getHours = function(employeeDailyTimer) {
        if (employeeDailyTimer) {
            return employeeDailyTimer.totalHours.toFixed(2);
        }
        else {
            return '0.00';
        }
    };

    $scope.getChartColor = function (employee, dayOfTheWeek) {
        if (employee === null) return;

        const dayNum = +dayOfTheWeek[dayOfTheWeek.length - 1];

        var timerDate = new Date(employee.startOfWeek);
        timerDate.setDate(timerDate.getDate() + dayNum);

        const classes = [];

        var employeeDailyTimer = employee['day' + vm.dayKeys[dayNum]];
        if (employeeDailyTimer != null) {
            employeeDailyTimer.department = employee.department
            employeeDailyTimer.departmentID = employee.departmentID
            
            if (_timerApproved(employeeDailyTimer) && !_isFlagged(employeeDailyTimer) && !_needsPriorApproval(employeeDailyTimer)) {
                classes.push("timer-approved");
            }
            else if (_isEditable(employeeDailyTimer)) {
                classes.push('timer-editable');
            }
            else if (_needsPriorApproval(employeeDailyTimer)) {
                classes.push('timer-approval-required');
            }
            else {
                classes.push('timer-ready-for-approval');
            }

            classes.push("has-occurence");
        }

        return classes.join(' ');
    };

    $scope.isInjured = function(employeeDailyTimer) {
        return employeeDailyTimer && employeeDailyTimer.injured;
    };

    $scope.isUserFlagged = function(employeeDailyTimer) {
        return employeeDailyTimer && employeeDailyTimer.userFlagged;
    };

    $scope.isSystemFlagged = function(employeeDailyTimer) {
        return employeeDailyTimer && employeeDailyTimer.systemFlagged;
    };

    $scope.searchEmployees = function(item) {
        
        if ($scope.selectedEmployee !== "") {
            return item.name === $scope.selectedEmployee;
        }
        if (!item.supervisorApproved) {
            item.supervisorApproved = false;
        }
        if ($scope.dep)
            if ($scope.dep !== 'All Departments')
                return item.department === $scope.dep;
            else
                return true;
        return true;
    };

    $scope.searchChanged = function(selectedEmployee) {
        $scope.dep = "All Departments";
        sessionStorage.setItem(cachedValues.cachedSearch, selectedEmployee);
    };

    $scope.selectedWeekChanged = function(selectedWeek, enter) {
        var manualInput = $("#selectWeek").is(':focus');
        if (enter || !manualInput) {
            $scope.opened = false;
            angular.element("#selectWeek").blur();
            selectedWeek = moment(selectedWeek).startOf('week').format('YYYY-MM-DD');
            sessionStorage.setItem(cachedValues.cachedWeek, selectedWeek);

            _getTimeCards(selectedWeek, $scope.filter);
            $scope.approveDisabled = (moment(selectedWeek) >= moment().week());
        }
    };

    $scope.filterChanged = function(filter) {
        $scope.filterString = JSON.stringify(filter);
        sessionStorage.setItem(cachedValues.cachedFilter, $scope.filterString);
        _getTimeCards($scope.selectedWeek, filter);
    };

    $scope.departmentChanged = function(dep) {
        sessionStorage.setItem(cachedValues.cachedDepartment, $scope.dep);
    };

    $scope.newTimer = function() {
        var modalInstance = $modal.open({
            template: newTimerTemplate,
            controller: 'NewTimerModalController',
            windowClass: 'default-modal',
            resolve: {
                users: function() {
                    return $scope.users
                }
            }
        });

        modalInstance.result.then(function(e) {
            var timerDate = moment(e.date).format('MM-DD-YYYY');
            var timerInfo = { employeeID: e.employee.employeeID, departmentID: e.department.departmentID };
            $scope.detailPageWithCaching(timerInfo, timerDate);
        });
    };

    $scope.clearFilters = function(e) {
        $scope.selectedEmployee = '';
        sessionStorage.setItem(cachedValues.cachedSearch, $scope.selectedEmployee);

        $scope.selectedWeek = moment().subtract(1, 'week').startOf('week').format('YYYY-MM-DD');
        sessionStorage.setItem(cachedValues.cachedWeek, $scope.selectedWeek);

        $scope.filter = { name: "All Approved" };
        $scope.filterString = JSON.stringify($scope.filter);
        sessionStorage.setItem(cachedValues.cachedFilter, $scope.filterString);

        $scope.dep = "All Departments";
        $scope.departmentChanged($scope.dep);

        _getTimeCards($scope.selectedWeek, $scope.filter);
    };

    $scope.detailPageWithCaching = function(employee, refDay) {
        sessionStorage.setItem("cachedReturnToPage", $routeParams.supervisorOrAccounting);
        $location.path("/employee/" + employee.employeeID + "/department/" + employee.departmentID + "/day/" + refDay);
    };

    $scope.flagCard = function(card) {
        Restangular.all('EmployeeTimecards').customPOST(null, card.employeeTimecardID, { 'flag': !card.flagged })
        .then(function(data) {
        });
    };

    $scope.flagTimeForApproval = function(employeeDetails, dayOfTheWeek) {
        var invalidTimerID = 0;

        if (employeeDetails.employeeTimerID !== invalidTimerID) {
            _openNotesModal(employeeDetails, dayOfTheWeek);
        }
    };

    $scope.selectTimer = function(id) {
        var index = _.indexOf($scope.selectedTimers, id);
        if (index < 0) {
            $scope.selectedTimers.push(id);
        }
        else {
            $scope.selectedTimers.splice(index, 1);
        }
    };

    $scope.selectAll = function(filteredEmployees) {
        $scope.selectedTimers = [];
        for (var i = 0; i < filteredEmployees.length; i++) {
            if (_isEmployeeReadyForApproval(filteredEmployees[i])) {
                filteredEmployees[i].selectedForApproval = !(filteredEmployees[i].selectedForApproval);
                if (filteredEmployees[i].selectedForApproval) {
                    $scope.selectedTimers.push(filteredEmployees[i].employeeTimecardID);
                }
                else {
                    $scope.selectedTimers.splice(i, 1);
                }
            }
        }
    };

    $scope.approveTimers = function() {
        var modalInstance = $modal.open({
            controller: 'confirmCustomMessageController',
            controllerAs: 'vm',
            template: customMessageTemplate,
            windowClass: 'default-modal',
            resolve: {
                customMessage: function() {
                    return "Are you sure you want to approve all time cards for all employees this week?";
                },
            },
        });
        
        modalInstance.result
            .then(function() {
                $scope.filteredData.filteredEmployees.forEach(function(e) {
                    $scope.approveWeekTimers(e, true);
                });
            }, angular.noop);
    };

    $scope.approveTimer = async function(employee, dayOfTheWeek) {
        if (employee === null) return;
        
        const dayNum = +dayOfTheWeek[dayOfTheWeek.length - 1];
        
        var timerDate = new Date(employee.startOfWeek);
        timerDate.setDate(timerDate.getDate() + dayNum);
        
        var employeeTimer = employee['day' + vm.dayKeys[dayNum]];
        employeeTimer.department = employee.department
        employeeTimer.departmentID = employee.departmentID
        
        if (!_canApprove(employeeTimer)) {
            if(vm.approvalNeeded.length > 0) {
                var errorMessage = "Approvals required:\n " + vm.approvalNeeded;
                NotificationFactory.error(errorMessage);
                return;
            } else if(vm.notPermittedToApprove.length > 0) {
                var errorMessage = "Not Permitted to Approve as: " + vm.notPermittedToApprove
            }
        }

        if (_isFlagged(employeeTimer)) {
            NotificationFactory.error('Must resolve notes prior to approving');
            return;
        }

        var timerDay = moment(timerDate).format("YYYY-MM-DD");

        var employeeObj = {
            'departmentID': employee.departmentID,
            'employeeID': employee.employeeID,
            'day': timerDay,
        };

        var approvalObj = _getApprovalRoleObjToPatch($scope.pageName);
        await TimerApprovalsService.patch(employeeObj, approvalObj)
            .then(function (response) {
                NotificationFactory.success(`Employee Timer Approved.`)
                Object.assign(employeeTimer, approvalObj);

                if ($rootScope.$$phase != '$apply' && $rootScope.$$phase != '$digest') {
                    $rootScope.$apply();
                }
            }, error => {
                NotificationFactory.error(`Approval Request Failed. ${errorMessage}`);
                return;
            });
    };

    function getPageRequiredPermissions() {
        var pageParam = $routeParams.supervisorOrAccounting;
        
        var permissions = [];
        switch (pageParam) {
            case 'accounting':
                permissions.push(3);
                break;

            case 'billing':
                permissions.push(4);
                break;

            case 'supervisor':
                permissions.push(5);
                break;

            case 'foreman':
                permissions.push(6);
                break;

            case 'employee':
                permissions.push(7);
                break;
        
            default:
                break;
        }

        return permissions;
    }

    function _openNotesModal(employeeDetails, dayOfTheWeek) {
        var timerDate = new Date(employeeDetails.startOfWeek);
        timerDate.setDate(timerDate.getDate() + +dayOfTheWeek[dayOfTheWeek.length - 1]);
        
        var timerDay = moment(timerDate).format("MM/DD/YYYY");

        var modalInstance = $modal.open({
            controller: 'editableTextModalController',
            controllerAs: 'vm',
            template: editableTextTemplate,
            windowClass: 'default-modal',
            resolve: {
                employeeID: function() {
                    return employeeDetails.employeeID;
                },
                departmentID: function() {
                    return employeeDetails.departmentID;
                },
                timerDay: function() {
                    return timerDay;
                },

            },
        });

        modalInstance.result.then(function(response) {
            if (response !== 'cancel') {
                _updateUserNotesDisplay(employeeDetails, dayOfTheWeek);
            }
        });
    }

    function _updateUserNotesDisplay(employeeDetails, dayOfTheWeek) {
        const dayNum = +dayOfTheWeek[dayOfTheWeek.length - 1];
        const dayKey = 'day' + vm.dayKeys[dayNum];
        
        if (!employeeDetails[dayKey]) employeeDetails[dayKey] = {};
        
        employeeDetails[dayKey].userFlagged = true;
    }

    function _isSupervisorOrAccountingPage() {
        var routeParam = $routeParams.supervisorOrAccounting;

        switch(routeParam) {
            case 'supervisor':
                $scope.pageName = "Supervisors";
                $scope.isSupervisorPage = true;
                $scope.viewPagePermission = 'viewSupervisorTimers';
                $scope.filters = [
                    { supervisorApproved: true, name: 'Approved Timers' },
                    { supervisorApproved: false, name: 'Not Approved' },
                    { name: 'All Approved' },
                ];
                // cachedValues.filterOptions = { name: 'All' };
                cachedValues.cachedWeek = "cachedWeek";
                cachedValues.cachedSearch = "cachedSearch";
                cachedValues.cachedDepartment = "cachedDep";
                cachedValues.cachedFilter = "cachedFilter";
                break;

            case 'billing':
                $scope.pageName = "Billing";
                $scope.isBillingPage = true;
                $scope.viewPagePermission = 'viewBillingTimers';
                $scope.filters = [
                    { billingApproved: true, supervisorApproved: true, name: 'Approved By Billing' },
                    { billingApproved: false, supervisorApproved: true, name: 'Not Approved By Billing' },
                    { billingApproved: false, supervisorApproved: false, name: 'Not Approved By Supervisor' },
                    { name: 'All Approved' },
                ];

                // cachedValues.filterOptions = { name: 'All' };
                cachedValues.cachedWeek = "cachedWeek";
                cachedValues.cachedSearch = "cachedSearch";
                cachedValues.cachedDepartment = "cachedDep";
                cachedValues.cachedFilter = "cachedFilter";
                break;

            case 'accounting':
                $scope.pageName = "Accounting";
                $scope.isAccountingPage = true;
                $scope.viewPagePermission = 'viewAccountingTimers';
                $scope.filters = [
                    { accountingApproved: true, supervisorApproved: true, billingApproved: true, name: 'Approved By Accounting' },
                    { accountingApproved: false, supervisorApproved: true, billingApproved: true, name: 'Not Approved By Accounting' },
                    { accountingApproved: false, billingApproved: false, supervisorApproved: true, name: 'Not Approved By Billing' },
                    { accountingApproved: false, billingApproved: false, supervisorApproved: false, name: 'Not Approved By Supervisor' },
                    { name: 'All Approved' },
                ];
                // cachedValues.filterOptions = { name: 'All' };
                cachedValues.cachedWeek = "accountingCachedWeek";
                cachedValues.cachedSearch = "accountingCachedSearch";
                cachedValues.cachedDepartment = "accountingCachedDep";
                cachedValues.cachedFilter = "accountingCachedFilter";
            break;
        }
        
        // cachedValues.filterOptions = { name: 'All' };
        cachedValues.cachedWeek = "cachedWeek";
        cachedValues.cachedSearch = "cachedSearch";
        cachedValues.cachedDepartment = "cachedDep";
        cachedValues.cachedFilter = "cachedFilter";
    }

    function _setPageType() {
        var pageParam = $routeParams.supervisorOrAccounting;

        switch (pageParam) {
            case 'supervisor':
                $scope.pageName = 'Supervisors';
                vm.pageType = pageParam;
                break;
            case 'billing':
                $scope.pageName = 'Billing';
                vm.pageType = pageParam;
                break;
            case 'accounting':
                $scope.pageName = 'Accounting';
                vm.pageType = pageParam;
                break;
        }

    }

    function _getStoredData() {
        if (typeof (Storage) !== "undefined") {
            if (sessionStorage[cachedValues.cachedSearch] || sessionStorage[cachedValues.cachedSearch] == "") {
                $scope.selectedEmployee = sessionStorage.getItem(cachedValues.cachedSearch);
            }
            else {
                $scope.selectedEmployee = '';
            }
            if (sessionStorage[cachedValues.cachedDepartment]) {
                $scope.dep = sessionStorage.getItem(cachedValues.cachedDepartment);
            }
            else {
                $scope.dep = "All Departments";
            }
            if (sessionStorage[cachedValues.cachedFilter]) {
                $scope.filter = JSON.parse(sessionStorage.getItem(cachedValues.cachedFilter));
            }
            else {
                $scope.filter = { name: "All Approved" };
            }

            if (sessionStorage[cachedValues.cachedWeek]) {
                $scope.selectedWeek = sessionStorage.getItem(cachedValues.cachedWeek);
            }
            else {
                $scope.selectedWeek = moment().subtract(1, 'week').startOf('week').format('YYYY-MM-DD');
            }
        }
        else {
            console.log("No Storage support");
            $scope.selectedEmployee = '';
            $scope.dep = "All Departments";
            $scope.filter = { name: "All Approved" };
            $scope.selectedWeek = sessionStorage.getItem(cachedValues.cachedWeek);
        }
    }

    function _getLists() {
        DepartmentsHelper.getList()
            .then(function(response) {
                var departmentList = response.clone();
                departmentList.unshift({ name: "All Departments", departmentID: 0 });
                $scope.departments = departmentList;
            });
    }

    function _getTimeCards(firstOfWeek, filter) {
        var filterObj = _getFilterObject(firstOfWeek, filter);
        var EmployeeTimecardsEndpoint;

        if ($scope.isBillingPage) {
            EmployeeTimecardsEndpoint = EmployeeTimecards.one("Biller").get(filterObj);
        }
        else if ($scope.isAccountingPage) {
            EmployeeTimecardsEndpoint = EmployeeTimecards.one("Accountant").get(filterObj);
        }
        else {
            EmployeeTimecardsEndpoint = EmployeeTimecards.one("Supervisor").get(filterObj);
        }

        EmployeeTimecardsEndpoint
            .then(function(data) {
                $scope.employees = data.items || data;
                
                if ($scope.employees.length) {
                    _daysOfWeek($scope.employees[0].startOfWeek);
                }
                
                $scope.employees
                    .filter(function(e) {
                        return typeof vm.dayKeys
                            .find(function(d) {
                                const day = e['day' + d];
                                
                                return day ? day['approvedBy' + $scope.pageName] : day;
                            }) == 'number';
                    });
                
                _clearPartialEmployeeSearch();
            }, angular.noop);
    }

    function _getFilterObject(firstOfWeek, filter) {
        var filterObj = filter;
        filterObj.week = firstOfWeek || moment().subtract(1, 'week').startOf('week').format('YYYY-MM-DD');
        return filterObj;
    }

    function _clearPartialEmployeeSearch() {
        if ($scope.selectedEmployee !== '') {
            var searchTermDoesntMatchAnyEmployees = !$scope.employees.find(function(e) { return e.name.match(new RegExp($scope.selectedEmployee, 'i')); });
            if (searchTermDoesntMatchAnyEmployees) {
                $scope.selectedEmployee = '';
                sessionStorage.setItem(cachedValues.cachedSearch, $scope.selectedEmployee);
            }
        }
    }

    function _daysOfWeek(day) {
        $scope.days = [];
        $scope.refDays = [];
        for (var i = 0; i < 7; i++) {
            $scope.days[i] = moment(new Date(day)).add(i, 'd').format("D MMM");
            $scope.refDays[i] = moment(new Date(day)).add(i, 'd').format('MM-DD-YYYY');
        }
    }

    function _isEmployeeReadyForApproval(employee) {
        var readyForAccountingApproval = $scope.isAccountingPage && !employee.approvedByAccounting && employee.approvedBySupervisor;
        var readyForSupervisorApproval = $scope.isSupervisorPage && !employee.approvedBySupervisor;
        return readyForAccountingApproval || readyForSupervisorApproval;
    }

    function _isFlagged(employeeDailyTimer) {
        if (employeeDailyTimer.userFlagged || employeeDailyTimer.systemFlagged) {
            return true;
        }
        return false;
    }

    function _isEditable(employeeDailyTimer) {
        if (_canApprove(employeeDailyTimer)) {
            if (_isFlagged(employeeDailyTimer)) return true;
        }
            return false;
    }

    function _needsPriorApproval(employeeDailyTimer) {
        if (employeeDailyTimer == null)
            return false

        const priorApprovalsNeeded = [];

        if ($scope.pageName == 'Billing' && !employeeDailyTimer.approvedBySupervisor) {
            priorApprovalsNeeded.push('Supervisor');
        }
        if ($scope.pageName == 'Accounting' && !employeeDailyTimer.approvedByBilling) {
            priorApprovalsNeeded.push('Billing');
        }
        
        vm.approvalNeeded = priorApprovalsNeeded.join(', ');

        return vm.approvalNeeded
    }

    function _canApprove(employeeDailyTimer) {
        if(employeeDailyTimer == null)
            return false
            
        _needsPriorApproval(employeeDailyTimer)

        const notPermittedToApproveAs = [];

        var activityId = null
        switch($scope.pageName) {

            case ('Supervisors'):
                activityId = ActivityService.getActivityId('approveAsSupervisor')
                break

            case ('Billing'):
                activityId = ActivityService.getActivityId('approveAsBilling')
                break

            case ('Accounting'):
                activityId = ActivityService.getActivityId('approveAsAccounting')
                break

        }

        var canApprove = false
        try{
            canApprove = vm.departmentActivityMap[activityId].departmentIds.includes(employeeDailyTimer.departmentID)
        } catch(error) {
            canApprove = false
        }
        if (!canApprove) {
            notPermittedToApproveAs.push('Supervisor for ' + employeeDailyTimer.department)
        }

        vm.notPermittedToApprove = notPermittedToApproveAs.join(', ')
        
        return !vm.approvalNeeded && canApprove;
    }

    function _timerApproved(employeeDailyTimer) {
        if(employeeDailyTimer == null)
            return false

        if ($scope.pageName === 'Supervisors') {
            if (employeeDailyTimer.approvedBySupervisor) return true;
        }
        else if ($scope.pageName === 'Billing') {
            if (employeeDailyTimer.approvedByBilling) return true;
        }
        else if ($scope.pageName === 'Accounting') {
            if (employeeDailyTimer.approvedByAccounting) return true;
        }

        return false;
    }

    function _getValidEmployeeTimers(employee) {
        var days = [];
        if (employee.day0 !== null) days.push({ timer: employee.day0, day: 'day1' });
        if (employee.day1 !== null) days.push({ timer: employee.day1, day: 'day2' });
        if (employee.day2 !== null) days.push({ timer: employee.day2, day: 'day3' });
        if (employee.day3 !== null) days.push({ timer: employee.day3, day: 'day4' });
        if (employee.day4 !== null) days.push({ timer: employee.day4, day: 'day5' });
        if (employee.day5 !== null) days.push({ timer: employee.day5, day: 'day6' });
        if (employee.day6 !== null) days.push({ timer: employee.day6, day: 'day0' });
        return days;
    }

    function _getTimeCardInformation(employee) {

        var approvedTimeCard = false;
        var timeCardIsFlagged = false;
        var timeCardNeedsPriorApproval = false;
        var days = _getValidEmployeeTimers(employee);
        var count = 0;
        var flaggedDays = [];

        for (var i = 0; i < days.length; i++) {
            if (_timerApproved(days[i].timer) && !_isFlagged(days[i].timer) && !_needsPriorApproval(days[i].timer)) {
                count++;
            }
            if (_needsPriorApproval(days[i].timer)) {
                timeCardNeedsPriorApproval = true;
                break;
            }

            if (_isFlagged(days[i].timer)) flaggedDays.push(days[i]);
        }

        if (flaggedDays.length > 0) timeCardIsFlagged = true;
        if (count === days.length) approvedTimeCard = true;

        return { needsPriorApproval: timeCardNeedsPriorApproval, flagged: timeCardIsFlagged, approved: approvedTimeCard };
    }
    
    function canSeeTooltip() {
        return PermissionService.canDoAny(tooltipActions);
    }

    function _getApprovalRoleObjToPatch(role) {
        var approvalsObj = {} as { [k: string]: boolean };

        if (role === "Supervisors") {
            approvalsObj.approvedBySupervisor = true;
        }
        else if (role === "Billing") {
            approvalsObj.approvedByBilling = true;
        }
        else if (role === "Accounting") {
            approvalsObj.approvedByAccounting = true;
        }

        return approvalsObj;
    }

    function mapPageToPermission() {
        return $scope.pageName == 'Supervisors' ? 'Supervisor' : $scope.pageName;
    }
}