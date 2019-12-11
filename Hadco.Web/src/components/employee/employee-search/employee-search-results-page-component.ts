import * as angular from 'angular';
import * as moment from 'moment';
import * as _ from 'lodash';

import * as template from './employee-search-results-page.html';
import * as mapModalTemplate from '../../Shared/modal-templates/modal-map.html';
import { IPermissionService } from '../../permissions/permission-service';

angular
    .module('employeeModule')
    .component('htEmployeeSearchResultsPage', {
        controller: employeeSearchResultsPageController,
        controllerAs: 'vm',
        template,
    });

employeeSearchResultsPageController.$inject = [
    '$route',
    '$routeParams',
    '$location',
    '$modal',
    'Employees',
    'NotificationFactory',
    'EmployeeTimecardsHelper',
    'Locations',
    'CurrentUser',
    'PermissionService',
];

function employeeSearchResultsPageController(
    $route,
    $routeParams,
    $location: ng.ILocationService,
    $modal,
    Employees,
    NotificationFactory,
    EmployeeTimecardsHelper,
    Locations,
    CurrentUser,
    PermissionService: IPermissionService,
) {
    var vm = this;
    
    
    init();
    
    function init() {
        PermissionService.redirectIfUnauthorized('searchEmployees');
        
        vm.can = PermissionService.can;
        
        vm.employeeID = $routeParams.employeeID;
        vm.selectedWeek =$routeParams.selectedWeek || moment().startOf('week').format('MM/DD/YYYY');
        vm.formattedForRouteParamDay = moment(vm.selectedWeek, 'MM/DD/YYYY').format('MM-DD-YYYY');
        _getEmployeeInformation(vm.employeeID, vm.selectedWeek);
    }

    vm.detailPageWithCaching = function(employee, refDay, employeePage) {
        sessionStorage.setItem("cachedReturnToPage", "employee-search");
        sessionStorage.setItem("cachedEmployeeSearchUrl", $location.url());
        var detailPageUrl = employeePage ? "/human-resources/employee/" + employee.employeeID : "/employee/" + employee.employeeID + "/department/" + employee.departmentID + "/day/" + refDay;
        $location.path(detailPageUrl);
    };

    vm.isSupervisorReadyToApprove = function(timer) {
        var readyToApprove = !timer.approvedBySupervisor && !timer.approvedByBilling && !timer.approvedByAccounting;
        return readyToApprove;
    };

    vm.isBillingReadyToApprove = function(timer) {
        var readyToApprove = timer.approvedBySupervisor && !timer.approvedByBilling && !timer.approvedByAccounting;
        return readyToApprove;
    };

    vm.isAccountingReadyToApprove = function(timer) {
        var readyToApprove = timer.approvedBySupervisor && !timer.approvedByAccounting; // add timer.approvedByBilling when backend supports it
        return readyToApprove;
    };

    vm.approveTimecard = function(timecard, approvalType) {
        EmployeeTimecardsHelper.approveTimecards([ timecard.employeeTimecardID ], approvalType)
        .then(function(data) {
            NotificationFactory.success('Success: Timer approved');
            _getEmployeeInformation(vm.employeeID, vm.selectedWeek);
        });
    };

    vm.viewTimesheet = function(timesheet) {
        sessionStorage.setItem("cachedReturnToPage", "employee-search");
        sessionStorage.setItem("cachedEmployeeSearchUrl", $location.url());
        var date = timesheet.day;
        var formattedForRouteParamDay = moment(date).format('MM-DD-YYYY');
        var employeeID = timesheet.employeeID;
        var departmentID = timesheet.departmentID;
        $location.path("/superintendent/foreman/" + employeeID + "/department/" + departmentID + "/day/" + formattedForRouteParamDay);
    };

    vm.selectedWeekChanged = function(enter) {
        var manualInput = $("#selectWeek").is(':focus');
        if (enter || !manualInput) {
            vm.opened = false;
            angular.element("#selectWeek").blur();
            vm.selectedWeek = moment(vm.selectedWeek).format('MM/DD/YYYY');
            vm.formattedForRouteParamDay = moment(vm.selectedWeek, 'MM/DD/YYYY').format('MM-DD-YYYY');
            _getEmployeeInformation(vm.employeeID, vm.selectedWeek);
            $route.updateParams({ selectedWeek: vm.formattedForRouteParamDay });
        }
    };

    vm.openMapModal = function() {
        var modalInstance = $modal.open({
            controller: 'mapModalController',
            controllerAs: 'vm',
            template: mapModalTemplate,
            windowClass: 'default-modal',
            resolve: {
                mapData: function() {
                    return vm.mapData;
                },
                employeeName: function() {
                    return vm.employee.name;
                },
            },
        });

        modalInstance.result.then(function(response) {
            console.log("map closed");
        });
    };

    function _getEmployeeInformation(employeeID, selectedWeek) {
        Employees.one(employeeID).one("Search", vm.formattedForRouteParamDay).get()
        .then(function(response) {
            vm.employee = response.employee;
            vm.employee.departmentList = _createNamesString(vm.employee.departments);
            vm.employee.supervisorList = _createNamesString(vm.employee.supervisors);
            vm.employee.roleList = _createNamesString(vm.employee.roles);

            vm.timers = response.timers;
            vm.employee.timers = _getEmployeeTimers(vm.timers);
            _calculateTotalHours(vm.timers);
            _formatNullTimers(vm.timers);
            vm.foremen = response.timesheets;
            _checkForemenData(response.timesheets);
            _getLocation(employeeID);
            var startOfWeek = moment(selectedWeek, 'MM/DD/YYYY').startOf('week');
            _daysOfWeek(startOfWeek);
        });
    }

    function _getEmployeeTimers(timers) {
        if (vm.employee) {
            var filteredTimers = _.filter(timers, function(timer) {
                return timer.employeeID === vm.employee.employeeID;
            });
        }

        return filteredTimers;
    }

    function _calculateTotalHours(timers) {
        _.each(timers, function(timer) {
            var totalHoursForWeek = 0;
            _.each(timer, function(day) {
                if (day && day.totalHours) {
                        totalHoursForWeek += day.totalHours;
                    }
                
                timer.total = totalHoursForWeek;
            });
        });
    }

    function _checkForemenData(foremen) {
        if (foremen.length === 0) {
            vm.foremenTimesheetEmpty = true;
        }
    }

    function _formatNullTimers(timers) {
        var newTotalHours = { 'totalHours': 0.00 };
        _.each(timers, function(timer) {
            if (!timer.day0) timer.day0 = newTotalHours;
            if (!timer.day1) timer.day1 = newTotalHours;
            if (!timer.day2) timer.day2 = newTotalHours;
            if (!timer.day3) timer.day3 = newTotalHours;
            if (!timer.day4) timer.day4 = newTotalHours;
            if (!timer.day5) timer.day5 = newTotalHours;
            if (!timer.day6) timer.day6 = newTotalHours;
        });
    }
    
    function _createNamesString(array) {
        return array.map(function(object) {
            return object.name;
        }).join(', ');
    }

    function _getLocation(employeeID) {
        Locations.getMostRecentLocationByEmployeeID(employeeID)
        .then(function(response) {
            vm.mapData = response;
            vm.isMapAvailable = true;
        }, function() {
            vm.isMapAvailable = false;
        });
    }

    function _daysOfWeek(startOfWeek) {
        vm.days = [];
        vm.refDays = [];
        for (var i = 0; i < 7; i++) {
            vm.days[i] = moment(startOfWeek).add(i, 'd').format("D MMM");
            vm.refDays[i] = moment(startOfWeek).add(i, 'd').format('MM-DD-YYYY');
        }
    }


}