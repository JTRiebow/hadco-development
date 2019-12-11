import * as angular from 'angular';

angular
	.module('adminModule')
	.factory('BillTypes', BillTypesFactory);

BillTypesFactory.$inject = [ 'Restangular' ];

function BillTypesFactory(Restangular) {
	Restangular.configuration.routeToIdMappings['BillTypes'] = 'billTypeID';
	return Restangular.service('BillTypes');
}