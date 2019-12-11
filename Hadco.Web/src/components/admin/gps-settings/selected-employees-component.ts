import * as angular from 'angular';

import * as template from './selected-employees.html';

angular
	.module('adminModule')
	.component('htGpsSelectedEmployees', {
		controller: selectedEmployeesController,
		controllerAs: 'vm',
		template,
	});

selectedEmployeesController.$inject = [
	'$scope',
	'NotificationFactory',
	'Users',
	'$location',
	'Pagination',
];

function selectedEmployeesController($scope, NotificationFactory, Users, $location, Pagination) {

	$scope.search = $location.search().search;
	
	$scope.employees = [];

	$scope.pagination = {
		itemsPerPage: 10,
		totalItems: $scope.employees.length,
		currentPage: $location.search().page || 1,
	};

	Users.getList()
	.then(function(response) {
		$scope.allEmployees = response;
		$scope.pagination.totalItems = response.meta.totalResultCount;
	});

	var pageChanged = function(page, itemsPerPage) {
		$location.search('page', page);
		Users.getList({
			skip: Pagination.skip(page, itemsPerPage),
			take: itemsPerPage,
			orderBy: 'name',
		})
		.then(function(response) {
			$scope.employees = response;
			$scope.pagination.totalItems = response.meta.totalResultCount;
		});
	};

	$scope.$watch('pagination.currentPage', function(newValue, oldValue) {
		pageChanged(newValue, $scope.pagination.itemsPerPage);
	});


	$scope.updateTrackingType = function(employee) {
		console.log(employee.trackingType);
		Users.one(employee.employeeID).patch({ trackingType: employee.trackingType })
		.then(function(response) {
			NotificationFactory.success('Success: Tracking Setting Saved.');
		}, function(error) {
			NotificationFactory.error("Error: Tracking Type Not Saved.");
		});
	};
}