import * as angular from 'angular';

angular
	.module('adminModule')
	.factory('Jobs', jobsService);

jobsService.$inject = [ 'Restangular' ];

function jobsService(Restangular) {
	Restangular.configuration.routeToIdMappings['Jobs'] = 'JobID';
	return Restangular.service('Jobs');
}