import * as angular from 'angular';

angular.module('roleModule').factory('Roles', [
	'Restangular',
	function(Restangular) {
		Restangular.configuration.routeToIdMappings['Roles'] = 'roleId';
		return Restangular.service('Roles');
	},
]);