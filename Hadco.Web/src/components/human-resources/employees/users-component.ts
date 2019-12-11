import * as angular from 'angular';

import * as template from './users.html';
import * as mapModalTemplate from '../../Shared/modal-templates/modal-map.html';
import * as mapUnavailableTemplate from '../../Shared/modal-templates/map-unavailable-modal.html';

angular
    .module('employeeModule')
    .component('htUsers', {
        controller: usersController,
        controllerAs: 'vm',
        template,
    });

usersController.$inject = [
    '$scope',
    '$filter',
    '$location',
    'Pagination',
    '$modal',
    'NotificationFactory',
    'Users',
    'Locations',
    '$q',
    'PermissionService',
];

function usersController(
    $scope,
    $filter,
    $location,
    Pagination,
    $modal,
    NotificationFactory,
    Users,
    Locations,
    $q,
    PermissionService,
) {
    var vm = this;
    
    vm.can = PermissionService.can;

    vm.openMapModal = openMapModal;
    vm.viewEmployeePage = viewEmployeePage;
    vm.gridOptions = {
        enableRowSelection: false,
        enableFullRowSelection: false,
        enableRowHeaderSelection: false,
        exporterCsvFilename: 'vehicle-inspections.csv',
        enableSelectAll: false,
        showGridFooter: false,
        enableSorting: true,
        enableFiltering: false,
        multiSelect: false,
        enableGridMenu: false,
        exporterMenuPdf: false,
        minRowsToShow: 20,
        data: vm.users,
        columnDefs: createColumnDefs(),
        onRegisterApi: function(gridApi) {
            $scope.gridApi = gridApi;
            $scope.refreshData = refreshData;
        },
    };
    
    init();
    
    function createColumnDefs() {
        var columnDefs = [
            {
                field: 'name',
                displayName: 'Name',
                minWidth: 180,
                sort: {
                    direction: 'asc',
                },
                cellTemplate: `
                    <div>
                        <a
                            class="employee-link"
                            ng-click="grid.appScope.vm.viewEmployeePage(row.entity.employeeID)"
                            ng-if="grid.appScope.vm.can('viewEmployeeDetails')"
                        >
                            {{row.entity.name}}
                        </a>
                        <span ng-if="!grid.appScope.vm.can('viewEmployeeDetails')">
                            {{ row.entity.name }}
                        </span>
                    </div>`,
            },
            {
                field: 'employeeNumber',
                displayName: 'Username',
                minWidth: 150,
            },
            {
                field: 'departmentList',
                displayName: 'Department',
                minWidth: 150,
            },
            {
                field: 'supervisorList',
                displayName: 'Supervisor',
                minWidth: 150,
            },
            {
                field: 'roleList',
                displayName: 'Role',
                minWidth: 150,
            },
            {
                field: 'isClockedIn',
                displayName: 'Clocked In',
                minWidth: 150,
                cellTemplate: `
                    <div class="text-center">
                        <i
                            class="fa fa-lg fa-clock-o"
                            ng-class="row.entity.isClockedIn === true ? 'blue-text' : ''"
                            aria-hidden="true"
                        ></i>
                    </div>`,
            },
            {
                field: 'location',
                displayName: 'Location',
                minWidth: 150,
                cellTemplate: `
                    <div class="text-center">
                        <i
                            class="fa fa-lg fa-compass"
                            aria-hidden="true"
                            ng-class="'green-text'"
                            button="outline:none"
                            ng-click="grid.appScope.vm.openMapModal(row.entity)"
                        ></i>
                    </div>`,
            },
        ];
        
        return columnDefs;
    }

    function _getLocation(employee) {
        var deferred = $q.defer();
        Locations.getMostRecentLocationByEmployeeID(employee.employeeID)
        .then(function(response) {
            vm.mapData = response;
            vm.isMapAvailable = true;
            deferred.resolve();
        }, function() {
            vm.isMapAvailable = false;
            deferred.reject();
        });
        return deferred.promise;
    }

    function init() {
        PermissionService.redirectIfUnauthorized('viewEmployeeList');

        Users.getList().then(function(response) {
            vm.users = populateData(response);
            vm.gridOptions.data = vm.users;
        });
    }

    function openMapModal(employee) {

        _getLocation(employee).then(function() {
            setupSuccessfulMapModal(employee);
        }, function() {
            setupFailedMapModal(employee);
        });
    }

    function populateData(employeeList) {
        $.each(employeeList, function(index, employee) {
            var departments = [];
            var roles = [];
            var supervisors = [];
            $.each(employee.departments, function(i, department) {
                departments.push(department.name);
            });
            $.each(employee.roles, function(i, role) {
                roles.push(role.name);
            });
            $.each(employee.supervisors, function(i, supervisor) {
                supervisors.push(supervisor.name);
            });
            employeeList[index].departmentList = departments.join(", ");
            employeeList[index].roleList = roles.join(", ");
            employeeList[index].supervisorList = supervisors.join(", ");

        });
        return employeeList;
    }

    function refreshData() {
        var searchText = vm.searchText;
        vm.gridOptions.data = [];
        if (searchText) {
            ///$$ Removing the ability to search across multiple columns but leaving code in case requirements change
            //var searchTerms = searchText.split(" ");
            //$.each(searchTerms, function (index, value) {
            //    if (index === 0) {
            //        vm.gridOptions.data = $filter('filter')(vm.users, { name: value }, undefined);
            //    }
            //    else {
            //        vm.gridOptions.data = $filter('filter')(vm.gridOptions.data, value, undefined);
            //    }
            //});
            vm.gridOptions.data = $filter('filter')(vm.users, { name: searchText }, undefined);
        }
        else {
            vm.gridOptions.data = vm.users;
        }
    }

    function setupFailedMapModal(employee) {

        var modalInstance = $modal.open({
            controller: 'mapUnavailableModalController',
            controllerAs: 'vm',
            template: mapUnavailableTemplate,
            windowClass: 'map-unavailable-modal',
            resolve: {
                mapData: function() {
                    return vm.mapData;
                },
                employeeName: function() {
                    return employee.name;
                },
            },
        });

    }

    function setupSuccessfulMapModal(employee) {

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
                    return employee.name;
                },
            },
        });

    }

    function viewEmployeePage(employeeID) {
        sessionStorage.setItem("cachedReturnToPage", "human-resources");
        $location.path("/human-resources/employee/" + employeeID);
    }
}