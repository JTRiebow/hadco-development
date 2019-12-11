import * as angular from 'angular';

angular
	.module('adminModule')
	.factory('EquipmentServiceTypes', EquipmentServiceTypes);

EquipmentServiceTypes.$inject = [ 'Restangular' ];

function EquipmentServiceTypes(Restangular) {
	Restangular.configuration.routeToIdMappings['EquipmentServiceTypes'] = 'equipmentServiceTypeID';
	return Restangular.service('EquipmentServiceTypes');
}