import * as angular from 'angular';

angular.module('employeeTimerModule').factory('DowntimeTimers', [
	'Restangular',
	function(Restangular) {
		Restangular.configuration.routeToIdMappings['DowntimeTimers'] = 'downtimeTimerID';
		return Restangular.service('DowntimeTimers');
	},
]);