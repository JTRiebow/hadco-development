import * as angular from 'angular';

angular.module('employeeJobEquipmentTimersModule').factory('EmployeeJobEquipmentTimers', [
	'Restangular',
	function(Restangular) {
		Restangular.configuration.routeToIdMappings['EmployeeJobEquipmentTimers'] = 'employeeJobEquipmentTimerID';
		return Restangular.service('EmployeeJobEquipmentTimers');
	},
]);