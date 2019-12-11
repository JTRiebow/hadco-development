import * as angular from 'angular';

import * as template from './create-or-edit-occurrence.html';

angular
	.module('adminModule')
	.component('htCreateOrEditOccurrence', {
		controller: createOrEditOccurrenceController,
		controllerAs: 'vm',
		template,
	});

createOrEditOccurrenceController.$inject = [
	'$scope',
	'$location',
	'Pagination',
	'NotificationFactory',
	'OccurrencesHelper',
	'$routeParams',
	'PermissionService',
];

function createOrEditOccurrenceController(
	$scope,
	$location,
	Pagination,
	NotificationFactory,
	OccurrencesHelper,
	$routeParams,
	PermissionService
) {
	
	$scope.occurrenceId = $routeParams.occurrenceId;

	init();

	function init() {
		PermissionService.redirectIfUnauthorized([ 'editOccurrence', 'deleteOccurrence', 'addOccurrence' ]);
	}

	if ($scope.occurrenceId === 'create') {

	}
	else {
		OccurrencesHelper.get($scope.occurrenceId)
		.then(function(response) {
			$scope.occurrence = response;
		});
	}

	$scope.saveOccurrence = function(occurrence) {
		if ($scope.occurrenceId === 'create') {
			OccurrencesHelper.post(occurrence)
			.then(function(response) {
				NotificationFactory.success('Occurrence Created');
				$location.path('/admin/occurrences');
			});
		}
		else {
			OccurrencesHelper.patch(occurrence)
			.then(function(response) {
				NotificationFactory.success('Occurrence Changed');
				$location.path('/admin/occurrences');
			});
		}
	};

	

	$scope.cancel = function() {
		NotificationFactory.error('Create User Cancelled');
		$location.path('/admin/occurrences');
	};
}