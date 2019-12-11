import * as angular from 'angular';

angular.module('employeeModule').factory('Employees', [
	'Restangular',
	function(Restangular) {
		Restangular.configuration.routeToIdMappings['Employees'] = 'employeeId';
		return Restangular.service('Employees');
	},
]);