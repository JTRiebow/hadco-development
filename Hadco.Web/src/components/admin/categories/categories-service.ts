import * as angular from 'angular';

angular
	.module('adminModule')
	.factory('Categories', CategoriesFactory);

CategoriesFactory.$inject = [ 'Restangular' ];

function CategoriesFactory(Restangular) {
	Restangular.configuration.routeToIdMappings['Categories'] = 'categoryID';
	return Restangular.service('Categories');
}