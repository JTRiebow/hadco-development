import * as angular from 'angular';

angular.module('detailedApprovalModule').factory('DailyApprovals', [
	'Restangular',
	function(Restangular) {

	    return {
	        getEmployeeApprovals: getEmployeeApprovals,
	    };

	    function getEmployeeApprovals(employeeID, day, departmentID) {
	        return Restangular.one('Employee', employeeID).one('Day', day).one('Department', departmentID).one('DailyApproval').get();
	    }
	},
]);