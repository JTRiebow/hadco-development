import * as angular from 'angular';

angular
	.module('adminModule')
	.factory('Equipment', EquipmentFactory);

EquipmentFactory.$inject = [ 'Restangular' ];

function EquipmentFactory(Restangular) {
	Restangular.configuration.routeToIdMappings['Equipment'] = 'equipmentID';
	return Restangular.service('Equipment');
}