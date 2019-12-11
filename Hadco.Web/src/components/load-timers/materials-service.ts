import * as angular from 'angular';

angular.module('loadTimersModule').factory("Materials", [
	"Restangular",
	function(Restangular) {
		Restangular.configuration.routeToIdMappings["Materials"] = "MaterialId";
		return Restangular.service("Materials");
	},
]);