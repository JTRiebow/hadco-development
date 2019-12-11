import * as angular from 'angular';

angular.module('employeeTimecardsModule').factory('EmployeeTimecards', [
	'Restangular',
	function(Restangular) {
	    Restangular.configuration.routeToIdMappings['EmployeeTimecards'] = 'employeeTimecarID';
	    return Restangular.service('EmployeeTimecards');
	},
]);