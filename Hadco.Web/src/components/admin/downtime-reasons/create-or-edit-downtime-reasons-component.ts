import * as angular from 'angular';

import * as template from './create-or-edit-downtime-reasons.html';

angular
	.module('adminModule')
	.component('htCreateOrEditDowntimeReasons', {
		controller: createOrEditdowntimeReasonsController,
		controllerAs: 'vm',
		template,
	});

createOrEditdowntimeReasonsController.$inject = [
	'$scope',
	'$location',
	'Pagination',
	'NotificationFactory',
	'DowntimeReasonsHelper',
	'$routeParams',
	'PermissionService',
];

function createOrEditdowntimeReasonsController(
	$scope,
	$location,
	Pagination,
	NotificationFactory,
	DowntimeReasonsHelper,
	$routeParams,
	PermissionService
) {

	init();

	function init() {
		PermissionService.redirectIfUnauthorized([ 'createDowntimeReasons', 'editDowntimeReasons', 'deleteDowntimeReason' ]);
	}

	$scope.downtimeReasonId = $routeParams.downtimeReasonId;
		console.log($scope.downtimeReasonId);
	if ($scope.downtimeReasonId === 'create') {
		$scope.create = true;
	}
	else {
		$scope.create = false;
		DowntimeReasonsHelper.get($scope.downtimeReasonId)
		.then(function(response) {
			$scope.downtimeReason = response;
		});
	}

	$scope.saveDowntimeReason = function(downtimeReason) {
		if ($scope.downtimeReasonId === 'create') {
			DowntimeReasonsHelper.post(downtimeReason)
			.then(function(response) {
				NotificationFactory.success('Downtime Reason Created');
				$location.path('/admin/downtime-reasons');
			});
		}
		else {
			DowntimeReasonsHelper.patch(downtimeReason)
			.then(function(response) {
				NotificationFactory.success('Downtime Reason Changed');
				DowntimeReasonsHelper.clearCache();
				$location.path('/admin/downtime-reasons');
			});
		}
	};

	$scope.cancel = function() {
		NotificationFactory.error('Create User Cancelled');
		$location.path('/admin/downtime-reasons');
	};
}