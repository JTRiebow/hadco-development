import * as angular from 'angular';

angular.module('loadTimersModule').factory('LoadTimers', [
	'Restangular',
	function(Restangular) {
		Restangular.configuration.routeToIdMappings['LoadTimers'] = 'loadTimerDto';
		return Restangular.service('LoadTimers');
	},
]);