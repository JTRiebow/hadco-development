import * as angular from 'angular';

import * as template from './create-or-edit-truck-classifications.html';

angular
	.module('adminModule')
	.component('htCreateOrEditTruckClassifications', {
		controller: createOrEditTruckClassificationsController,
		controllerAs: 'vm',
		template,
	});

createOrEditTruckClassificationsController.$inject = [
	'$scope',
	'$location',
	'Pagination',
	'NotificationFactory',
	'OccurrencesHelper',
	'MaterialsHelper',
	'JobsHelper',
	'DowntimeReasonsHelper',
	'TruckClassificationsHelper',
	'$routeParams',
	'PermissionService',
	'Ocurrences',
];

function createOrEditTruckClassificationsController(
	$scope,
	$location,
	Pagination,
	NotificationFactory,
	OccurrencesHelper,
	MaterialsHelper,
	JobsHelper,
	DowntimeReasonsHelper,
	TruckClassificationsHelper,
	$routeParams,
	PermissionService,
	Occurrences,
) {
	$scope.pageTitle = 'Truck Classifications';

	$scope.pageToggle = function(pageTitle) {
		$scope.pageTitle = pageTitle;
	};

	// change name of listTypeID and pageToggle
	$scope.listTypeId = $routeParams.listTypeId;

	if ($scope.listTypeId === 'create') {

	}
	else {
		if ($scope.pageTitle = 'Downtime Reasons') {
			DowntimeReasonsHelper.get($scope.listTypeId)
			.then(function(response) {
				$scope.downtimeReason = response;
			});
		}
		if ($scope.pageTitle = 'Jobs') {
			JobsHelper.get($scope.listTypeId)
			.then(function(response) {
				$scope.job = response;
			});
		}
		if ($scope.pageTitle= 'Occurrences') {
			OccurrencesHelper.get($scope.listTypeId)
			.then(function(response) {
				$scope.occurrence = response;
			});
		}
		if ($scope.listType = 'Materials') {
			MaterialsHelper.get($scope.listTypeId)
			.then(function(response) {
				$scope.material = response;
			});
		}
		if ($scope.listType = 'Truck Classifications') {
			TruckClassificationsHelper.get($scope.listTypeId)
			.then(function(response) {
				$scope.truckClassification = response;
			});
		}
	}

	$scope.saveListItem = function(listItem) {
		if ($scope.listTypeId === 'create') {
			
			if ($scope.pageTitle = 'Downtime Reasons') {
				var downtimeReason = listItem;
				DowntimeReasonsHelper.post(downtimeReason)
				.then(function(response) {
					NotificationFactory.success('Downtime Reason Created');
					$location.path('/admin/downtimeReasons');
				});
			}
			if ($scope.pageTitle = 'Jobs') {
				var job = listItem;
				JobsHelper.post(job)
				.then(function(response) {
					NotificationFactory.success('Job Created');
					$location.path('/admin/jobs');
				});
			}
			if ($scope.pageTitle = 'Occurrences') {
				var occurrence = listItem;
				OccurrencesHelper.post(occurrence)
				.then(function(response) {
					NotificationFactory.success('Occurrence Created');
					$location.path('/admin/occurrences');
				});
			}
			if ($scope.pageTitle = 'Materials') {
				var material = listItem;
				MaterialsHelper.post(material)
				.then(function(response) {
					NotificationFactory.success('Material Created');
					$location.path('/admin/Materials');
				});
			}
			if ($scope.pageTitle = 'Truck Classifications') {
				var truckClassification = listItem;
				TruckClassificationsHelper.post(truckClassification)
				.then(function(response) {
					NotificationFactory.success('Truck Classification Created');
					$location.path('/admin/truckClassifications');
				});
			}
		}
		else {

			if ($scope.pageTitle = 'Downtime Reasons') {
				DowntimeReasonsHelper.patch(listItem)
				.then(function(response) {
					NotificationFactory.success('Downtime Reason Changed');
					$location.path('/admin/downtimeReasons');
				});
			}
			if ($scope.pageTitle = 'Jobs') {
				JobsHelper.patch(listItem)
				.then(function(response) {
					NotificationFactory.success('Job Changed');
					$location.path('/admin/jobs');
				});
			}
			if ($scope.pageTitle = 'Occurrences') {
				Occurrences.patch(listItem)
				.then(function(response) {
					NotificationFactory.success('Occurrence Changed');
					$location.path('/admin/occurrences');
				});
			}
			else if ($scope.pageTitle = 'Materials') {
				MaterialsHelper.patch(listItem)
				.then(function(response) {
					NotificationFactory.success('Material Changed');
					$location.path('/admin/Materials');
				});
			}
			else if ($scope.pageTitle = 'Truck Classifications') {
				TruckClassificationsHelper.patch(listItem)
				.then(function(response) {
					NotificationFactory.success('Truck Classification Changed');
					$location.path('/admin/trucking-classifications');
				});
			}
		}
	};

	

	$scope.cancel = function() {
		NotificationFactory.error('Create User Cancelled');
		$location.path('/admin/trucking-classifications');
	};
}