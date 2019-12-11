import * as angular from 'angular';

angular.module('employeeTimecardsModule').factory('EmployeeTimecardsHelper', [
    'Restangular',
    function(Restangular) {
	    var service = {} as any;
	    service.approveTimecards = approveTimecards;

	    return service;

	    function approveTimecards(timecardArray, approvalType) {
	        return Restangular.all("EmployeeTimecards").customPOST(timecardArray, approvalType);
	    }
	},
]);