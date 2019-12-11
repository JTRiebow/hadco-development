import * as angular from 'angular';

angular.module('truckingModule').factory('Pricings', [
	'Restangular',
	function(Restangular) {
		Restangular.configuration.routeToIdMappings['Pricings'] = 'customerID';
		return Restangular.service('Pricings');
	},
]);