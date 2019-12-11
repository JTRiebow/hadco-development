import * as angular from 'angular';

angular
	.module('adminModule')
	.factory('Materials', MaterialsFactory);

MaterialsFactory.$inject = [ 'Restangular' ];

function MaterialsFactory(Restangular) {
	Restangular.configuration.routeToIdMappings['Materials'] = 'materialID';
	return Restangular.service('Materials');
}