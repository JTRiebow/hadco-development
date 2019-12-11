import * as angular from 'angular';

angular
	.module('pitsModule')
	.factory('Pits', PitsFactory);

PitsFactory.$inject = [ 'Restangular' ];

function PitsFactory(Restangular) {
	Restangular.configuration.routeToIdMappings['Pits'] = 'pitID';
	return Restangular.service('Pits');
}