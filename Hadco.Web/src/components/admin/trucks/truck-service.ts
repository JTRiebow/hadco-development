import * as angular from 'angular';

angular
	.module('adminModule')
	.factory('Trucks', TrucksFactory);

TrucksFactory.$inject = [ 'Restangular' ];

function TrucksFactory(Restangular) {
	Restangular.configuration.routeToIdMappings['Trucks'] = 'equipmentID';
	return Restangular.service('Trucks');
}