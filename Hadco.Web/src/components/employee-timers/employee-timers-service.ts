import * as angular from 'angular';

angular.module('employeeTimerModule').factory('EmployeeTimers', [
	'Restangular',
	function(Restangular) {
		Restangular.configuration.routeToIdMappings['EmployeeTimers'] = 'employeeTimerId';
		return Restangular.service('EmployeeTimers');
	},
]);