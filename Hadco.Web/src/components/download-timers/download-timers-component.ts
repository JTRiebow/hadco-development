import * as angular from 'angular';
import * as moment from 'moment';
import * as _ from 'lodash';
import * as $ from 'jquery';

import * as template from './download-timers.html';

angular
    .module('downloadTimersModule')
    .component('htDownloadTimers', {
        controller: downloadTimersController,
        controllerAs: 'vm',
        template,
    });

downloadTimersController.$inject = [
    '$scope',
    '$location',
    'Pagination',
    'NotificationFactory',
    'Departments',
    'CSV',
    'PermissionService',
    'ActivityService',
];

function downloadTimersController(
    $scope,
    $location,
    Pagination,
    NotificationFactory,
    Departments,
    CSV,
    PermissionService,
    ActivityService,
) {
    var vm = this;
    
    vm.can = PermissionService.can;
    PermissionService.redirectIfUnauthorized(PermissionService.csvPageActions);
    
    vm.availableDepartments = [];
    vm.departmentActivityMap = {};
    vm.departmentActivities = [];
    
    vm.allDepartments = [];
    
    $scope.downloadObject = {
        token: localStorage['token'],
        startDate: moment().subtract(1, 'week').startOf('week').format(),
        endDate: moment().subtract(1, 'week').endOf('week').format(),
        departmentID: null,
    };
    
    PermissionService.getDepartmentPermissionBreakdown(PermissionService.csvPageActions)
        .then(perms => {
            vm.availableDepartments = perms
                .reduce((arr, a) => [
                    ...arr,
                    ...a.departmentIds.filter(id => !arr.includes(id)),
                ], []);
            
            vm.departmentActivities = perms;
            vm.departmentActivityMap = perms.reduce((m, p) => ({ ...m, [p.activityID]: p }), {});
            return Departments.getList({ orderby: "name" });
        })
        .then(function(response) {
            vm.allDepartments = response;
            $scope.departments = response.filter(d => vm.availableDepartments.includes(d.departmentID));
            $scope.departments.unshift({ name: "All" });
            _.each($scope.departments, function(department) {
                $scope[department.name] = department;
                department.displayName = _.startCase(department.name);
            });

            $scope.types = {
                jobTimers: {
                    name: "Job Timers",
                    endpoint: "JobTimers",
                    departments: [
                        $scope.Mechanic,
                        $scope.Concrete,
                        $scope.Concrete2H,
                        $scope.ConcreteHB,
                        $scope.Development,
                        $scope.Residential,
                    ].filter(Boolean),
                    activityKey: 'downloadJobTimersCsv',
                },
                jobTimersInvoice: {
                    name: "Job Timers - Invoice",
                    endpoint: "JobTimersInvoice",
                    departments: [
                        $scope.Mechanic,
                        $scope.Concrete,
                        $scope.Concrete2H,
                        $scope.ConcreteHB,
                        $scope.Development,
                        $scope.Residential,
                    ].filter(Boolean),
                    activityKey: 'downloadJobTimersCsv',
                },
                occurrences: {
                    name: "Occurrences",
                    endpoint: "Occurrences",
                    departments: [
                        $scope.Concrete,
                        $scope.Development,
                        $scope.Residential,
                        $scope.Trucking,
                        $scope.Mechanic,
                        $scope.FrontOffice,
                        $scope.TMCrushing,
                        $scope.Transport,
                        $scope.Concrete2H,
                        $scope.ConcreteHB,
                        $scope.Scheduling
                    ].filter(Boolean),
                    activityKey: 'downloadOccurrencesCsv',
                },
                timecards: {
                    name: 'Employee Time Cards',
                    endpoint: 'EmployeeTimecards',
                    departments: [
                        $scope.TMCrushing,
                        $scope.All,
                        $scope.Concrete,
                        $scope.Concrete2H,
                        $scope.ConcreteHB,
                        $scope.Development,
                        $scope.Mechanic,
                        $scope.Residential,
                        $scope.Transport,
                        $scope.Trucking,
                        $scope.FrontOffice,
                    ].filter(Boolean),
                    activityKey: 'downloadEmployeeTimecardsCsv',
                },
                discrepancies: {
                    name: 'Discrepancies',
                    endpoint: 'Discrepancies',
                    departments: [
                        $scope.TMCrushing,
                        $scope.All,
                        $scope.Concrete,
                        $scope.Concrete2H,
                        $scope.ConcreteHB,
                        $scope.Development,
                        $scope.Mechanic,
                        $scope.Residential,
                        $scope.Transport,
                        $scope.Trucking,
                        $scope.FrontOffice,
                    ].filter(Boolean),
                    activityKey: 'downloadDiscrepanciesCsv',
                },
                loadTimers: {
                    name: 'Load Timers',
                    endpoint: 'LoadTimers',
                    departments: [
                        $scope.All,
                        $scope.Trucking,
                        $scope.Transport,
                    ].filter(Boolean),
                    activityKey: 'downloadLoadTimersCsv',
                },
                downtimeTimers: {
                    name: 'Downtime Timers',
                    endpoint: 'DowntimeTimers',
                    departments: [
                        $scope.Trucking,
                        $scope.Transport,
                    ].filter(Boolean),
                    activityKey: 'downloadDowntimeCsv',
                },
                equipmentTimers: {
                    name: "Equipment Timers",
                    endpoint: "EquipmentTimers",
                    departments: [
                        $scope.Mechanic,
                    ].filter(Boolean),
                    activityKey: 'downloadEquipmentTimersCsv',
                },
                employeeRoles: {
                    name: "Employee Roles",
                    endpoint: "EmployeeRoles",
                    departments: [
                        $scope.Concrete,
                        $scope.Development,
                        $scope.Residential,
                        $scope.Trucking,
                        $scope.Mechanic,
                        $scope.FrontOffice,
                        $scope.TMCrushing,
                        $scope.Transport,
                        $scope.Concrete2H,
                        $scope.ConcreteHB,
                        $scope.Scheduling
                    ].filter(Boolean),
                    activityKey: 'downloadEmployeeRolesCsv',
                },
                employeeClockInsOuts: {
                    name: "Employee Clock Ins/Outs",
                    endpoint: "EmployeeClockInsOuts",
                    departments: [
                        $scope.Concrete,
                        $scope.Development,
                        $scope.Residential,
                        $scope.Trucking,
                        $scope.Mechanic,
                        $scope.FrontOffice,
                        $scope.TMCrushing,
                        $scope.Transport,
                        $scope.Concrete2H,
                        $scope.ConcreteHB,
                        $scope.Scheduling,
                        $scope.All,
                    ].filter(Boolean),
                    activityKey: 'downloadEmployeeClockInsOutsCSV',
                },
                notes: {
                    name: "Notes",
                    endpoint: "Notes",
                    departments: [
                        $scope.Concrete,
                        $scope.Development,
                        $scope.Residential,
                        $scope.Trucking,
                        $scope.Mechanic,
                        $scope.FrontOffice,
                        $scope.TMCrushing,
                        $scope.Transport,
                        $scope.Concrete2H,
                        $scope.ConcreteHB,
                        $scope.Scheduling,
                        $scope.All,
                    ].filter(Boolean),
                    activityKey: 'downloadNotesCSV'
                },
                quantities: {
                    name: "Quantities",
                    endpoint: "Quantities",
                    departments: [
                        $scope.Concrete,
                        $scope.Development,
                        $scope.Residential,
                        $scope.Trucking,
                        $scope.Mechanic,
                        $scope.FrontOffice,
                        $scope.TMCrushing,
                        $scope.Transport,
                        $scope.Concrete2H,
                        $scope.ConcreteHB,
                        $scope.Scheduling,
                        $scope.All,
                    ].filter(Boolean),
                    activityKey: 'downloadQuantitiesCSV'
                }
            };
        });

    $scope.checkDatesAndDepForDisabled = function(type, downloadObject) {
        const activityId = ActivityService.getActivityId(type.activityKey);
        const allSelected = downloadObject.departmentID === null || downloadObject.departmentID === undefined;

        var departmentIncluded = type.departments.filter(department => {
            return department.departmentID === downloadObject.departmentID || allSelected
        });
        
        var hideDepartment = departmentIncluded.length <= 0;
        var endDateRequired = !$scope.types || (type === $scope.types.occurrences && !downloadObject.endDate);

        return !downloadObject.startDate || 
        hideDepartment ||
        endDateRequired;
    };

    $scope.downloadCSV = function(type, tempDownloadObject) {
        const downloadObject = { ...tempDownloadObject };
        downloadObject.startDate = moment(downloadObject.startDate).format("YYYY-MM-DD");
        downloadObject.endDate = moment(downloadObject.endDate).format("YYYY-MM-DD");

        if (!downloadObject.departmentID) {
            downloadObject.departmentID = vm.departmentActivityMap[ActivityService.getActivityId(type.activityKey)].departmentIds;
        }

        CSV.one(type.endpoint).get(downloadObject)
            .then(function(data) {
                if (data) {
                    window.location.href = 'api/CSV/' + type.endpoint +
                        '?' + $.param({
                            startDate: downloadObject.startDate,
                            endDate: downloadObject.endDate,
                            departmentID: downloadObject.departmentID,
                            access_token: downloadObject.token
                        }),
                        '_self';
                }
                else {
                    NotificationFactory.error("Error: No " + type.name + " available for that date range.");
                }
            });
    };
}