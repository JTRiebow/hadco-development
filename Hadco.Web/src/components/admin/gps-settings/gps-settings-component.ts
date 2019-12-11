import * as angular from 'angular';
import * as _ from 'lodash';

import * as template from './gps-settings.html';

angular
    .module('adminModule')
    .component('htGpsSettings', {
        controller: gpsSettingsController,
        controllerAs: 'vm',
        template,
    });

gpsSettingsController.$inject = [
    '$scope',
    'NotificationFactory',
    'Restangular',
    'DepartmentsHelper',
    'Settings',
    'PermissionService',
];

function gpsSettingsController(
    $scope,
    NotificationFactory,
    Restangular,
    DepartmentsHelper,
    Settings,
    PermissionService
) {
    var vm = this;

    vm.intervalChanged = intervalChanged;
    vm.updateTracking = updateTracking;
    init();

    function init() {
        vm.showTable = false;
        vm.intervalOptions = _getIntervalOptions();
        _setCurrentInterval();

        PermissionService.redirectIfUnauthorized('viewGpsSettings');
    }

    DepartmentsHelper.getList({ orderby: 'name' })
        .then(function(response) {
            vm.departments = response;
        });

    function updateTracking(department) {
        //patch 
        DepartmentsHelper.patch(department)
        .then(function(result) {
            NotificationFactory.success('Success: Department GPS setting saved');
        }, function(error) {
            NotificationFactory.error('Error: Department GPS setting not saved');
        });
    }

    function intervalChanged() {
        Settings.post({ "breadCrumbSeconds": vm.selectedInterval.value * 60 });
    }

    function _getIntervalOptions() {
        var intervalOptions = [];
        for (let i = 1; i <= 240; i++) {
            var name = i + " Minutes";
            intervalOptions.push({ "value": i, "name": name });
        }
        return intervalOptions;
    }

    function _setCurrentInterval() {
        Settings.one("BreadCrumbIntervalInSeconds").get()
        .then(function(response) {
            var minutes = response / 60;
            _.each(vm.intervalOptions, function(option) {
                if (option.value === minutes) {
                    vm.selectedInterval = option;
                }
            });
        });
    }
}