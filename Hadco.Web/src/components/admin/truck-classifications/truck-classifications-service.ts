import * as angular from 'angular';

angular
	.module('adminModule')
	.factory('TruckClassifications', TruckClassificationsFactory);

TruckClassificationsFactory.$inject = [ 'Restangular' ];

function TruckClassificationsFactory(Restangular) {
	Restangular.configuration.routeToIdMappings['TruckClassifications'] = 'truckClassificationID';
	return Restangular.service('TruckClassifications');
}