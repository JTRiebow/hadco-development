import * as angular from 'angular';

angular.module('jobTimersModule').factory('JobTimers', [
	'Restangular',
	function(Restangular) {
		Restangular.configuration.routeToIdMappings['JobTimers'] = 'jobTimerID';
		return Restangular.service('JobTimers');
	},
]);