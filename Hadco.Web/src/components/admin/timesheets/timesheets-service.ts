import * as angular from 'angular';

angular
    .module('adminModule')
    .factory('Timesheets', TimesheetsFactory);

TimesheetsFactory.$inject = [ 'Restangular' ];

function TimesheetsFactory(Restangular) {
    Restangular.configuration.routeToIdMappings['Timesheets'] = 'timesheetID';
    return Restangular.service('Timesheets');
}