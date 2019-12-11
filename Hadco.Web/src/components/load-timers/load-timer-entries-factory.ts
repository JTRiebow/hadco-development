import * as angular from 'angular';

angular.module('loadTimersModule').factory('LoadTimerEntries', [
	'Restangular',
	function(Restangular) {
	    Restangular.configuration.routeToIdMappings['LoadTimerEntries'] = 'loadTimerEntryDto';
	    return Restangular.service('LoadTimerEntries');
	},
]);

