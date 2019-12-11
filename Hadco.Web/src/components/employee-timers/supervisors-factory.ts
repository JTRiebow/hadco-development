import * as angular from 'angular';

angular.module('employeeTimerModule').factory('Supervisors', [
	'Restangular',
	function(Restangular) {
		// Restangular.configuration.routeToIdMappings['Supervisors'] = 'EmployeeTimerEntryID'
		return Restangular.service('Supervisors');
	},
]);