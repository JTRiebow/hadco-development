import * as angular from 'angular';

angular
	.module('adminModule')
	.factory('Departments', DepartmentsFactory);

DepartmentsFactory.$inject = [ 'Restangular' ];

function DepartmentsFactory(Restangular) {
	Restangular.configuration.routeToIdMappings['Departments'] = 'departmentID';
	return Restangular.service('Departments');
}