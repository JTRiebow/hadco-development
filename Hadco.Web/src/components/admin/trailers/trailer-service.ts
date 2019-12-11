import * as angular from 'angular';

angular
	.module('adminModule')
	.factory('Trailers', TrailersFactory);

TrailersFactory.$inject = [ 'Restangular' ];

function TrailersFactory(Restangular) {
	Restangular.configuration.routeToIdMappings['Trailers'] = 'equipmentID';
	return Restangular.service('Trailers');
}