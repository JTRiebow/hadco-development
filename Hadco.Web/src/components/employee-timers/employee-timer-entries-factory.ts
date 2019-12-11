import * as angular from 'angular';

angular.module('employeeTimerModule').factory('EmployeeTimerEntries', [
	'Restangular',
	function(Restangular) {
		Restangular.configuration.routeToIdMappings['EmployeeTimerEntries'] = 'EmployeeTimerEntryID';
		return Restangular.service('EmployeeTimerEntries');
	},
]);