import * as angular from 'angular';

angular
	.module('adminModule')
	.factory('Units', UnitsFactory);

UnitsFactory.$inject = [ 'Restangular' ];

function UnitsFactory(Restangular) {
	Restangular.configuration.routeToIdMappings['Units'] = 'unitID';
	return Restangular.service('Units');
}