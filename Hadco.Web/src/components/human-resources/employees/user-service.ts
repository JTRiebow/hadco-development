import * as angular from 'angular';

angular.module('userModule').factory('Users', [
    'Restangular',
    function(Restangular) {
        Restangular.configuration.routeToIdMappings['Employees'] = 'employeeId';
        return Restangular.service('Employees');
    },
]);