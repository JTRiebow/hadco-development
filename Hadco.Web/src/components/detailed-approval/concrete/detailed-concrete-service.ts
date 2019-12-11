import * as angular from 'angular';

angular
	.module('detailedApprovalModule')
	.factory('DetailedConcreteService', DetailedConcreteFactory);

DetailedConcreteFactory.$inject = [
	'Restangular',
	'$q',
	'Users',
	'Supervisors',
];

function DetailedConcreteFactory(Restangular, $q, Users, Supervisors) {

	//service
	var service = {} as IDetailedConcreteService;
	service.getJobTimers = getJobTimers;
	service.getEmployeeJobTimers = getEmployeeJobTimers;
	service.getSupervisors = getSupervisors;
	return service;

	//functions
	function getJobTimers(employeeID, dayID, departmentID) {
		var deferred = $q.defer();
		Users.one(employeeID).one('JobTimers', dayID).get({ departmentID: departmentID })
		.then(function(response) {
			deferred.resolve(response);
		}, function(error) {
			deferred.reject(error);
		});

		return deferred.promise;
	}

	function getEmployeeJobTimers(employeeID, dayID, departmentID) {
		var deferred = $q.defer();
		Users.one(employeeID).one('EmployeeJobTimers', dayID).get({ departmentId: departmentID })
			.then(function(data) {
				deferred.resolve(data);
			}, function(error) {
				deferred.reject(error);
		});
		return deferred.promise;
	}

	function getSupervisors(dayId, departmentId, employeeId) {
		var deferred = $q.defer();
		Supervisors.one(dayId).get({ departmentID: departmentId, employeeID: employeeId })
		//Users.one(employeeId).one("Supervisors").getList()
		.then(function(response) {
			deferred.resolve(response);
		}, function(error) {
			deferred.reject(error);
		});

		return deferred.promise;
	}
}

interface IDetailedConcreteService {
	getJobTimers(employeeID, dayID, departmentID): ng.IPromise<any>;
	getEmployeeJobTimers(employeeID, dayID, departmentID): ng.IPromise<any>;
	getSupervisors(dayId, departmentId, employeeId): ng.IPromise<any>;
}

export {
	IDetailedConcreteService,
};