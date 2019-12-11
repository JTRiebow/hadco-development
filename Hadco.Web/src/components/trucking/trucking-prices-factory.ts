import * as angular from 'angular';

angular.module('truckingModule').factory('Prices', [
	'Restangular',
	function(Restangular) {
		Restangular.configuration.routeToIdMappings['Prices'] = 'pricingID';
		return Restangular.service('Prices');
	},
]);