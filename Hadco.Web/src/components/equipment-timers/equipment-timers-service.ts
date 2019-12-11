import * as angular from 'angular';

angular.module('equipmentTimersModule').factory('EquipmentTimers', [
	'Restangular',
	function(Restangular) {
		Restangular.configuration.routeToIdMappings['EquipmentTimers'] = 'equipmentTimerID';
		return Restangular.service('EquipmentTimers');
	},
]);