import * as angular from 'angular';

angular.module('employeeJobTimersModule').factory('EmployeeJobTimers', [
	'Restangular',
	function(Restangular) {
		Restangular.configuration.routeToIdMappings['EmployeeJobTimers'] = 'employeeJobTimerID';
		return Restangular.service('EmployeeJobTimers');
	},
]);