import * as angular from 'angular';

angular
	.module('adminModule')
	.factory('Occurrences', OccurrencesFactory);

OccurrencesFactory.$inject = [ 'Restangular' ];

function OccurrencesFactory(Restangular) {
	Restangular.configuration.routeToIdMappings['Occurrences'] = 'occurrenceID';
	return Restangular.service('Occurrences');
}