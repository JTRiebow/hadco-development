import * as angular from 'angular';
import { ICollection } from 'restangular';

angular
	.module('adminModule')
	.factory('Locations', LocationsFactory);

LocationsFactory.$inject = [ 'Restangular' ];

function LocationsFactory(Restangular) {
	var service = {} as ILocationsService;

	Restangular.configuration.routeToIdMappings['Locations'] = 'locationID';
	angular.copy(Restangular.service('Locations'), service);

	service.getMostRecentLocationByEmployeeID = getMostRecentLocationByEmployeeID;
	service.getGPSCoordinatesByEmployeeID = getGPSCoordinatesByEmployeeID;

	return service;

	function getMostRecentLocationByEmployeeID(employeeID) {
		return service.one("MostRecent").one(employeeID.toString()).get();
	}

	function getGPSCoordinatesByEmployeeID(employeeID, params) {
		return service.one("Employee").one(employeeID.toString()).customGET('', params);
		
	}
}

interface ILocationsService extends ICollection {
	getMostRecentLocationByEmployeeID(employeeID): ng.IPromise<any>;
	getGPSCoordinatesByEmployeeID(employeeID, params): ng.IPromise<any>;
}

export {
	ILocationsService,
};