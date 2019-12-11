import * as angular from 'angular';

angular.module('truckingModule').factory('PricingList', [
	'Restangular',
	function(Restangular) {
		Restangular.configuration.routeToIdMappings['PricingList'] = 'pricingID';
		return Restangular.service('PricingList');
	},
]);