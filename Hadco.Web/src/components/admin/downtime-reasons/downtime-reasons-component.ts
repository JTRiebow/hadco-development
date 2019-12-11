import * as angular from 'angular';

import * as template from './downtime-reasons.html';
import * as deleteTemplate from '../../Shared/modal-templates/delete-item-modal.html';
import './downtime-reasons.scss';

angular
	.module('adminModule')
	.component('htDowntimeReasons', {
		controller: downtimeReasonsController,
		controllerAs: 'vm',
		template,
	});

downtimeReasonsController.$inject = [
	'$scope',
	'$location',
	'Pagination',
	'$modal',
	'NotificationFactory',
	'DowntimeReasonsHelper',
	'PermissionService',
];

function downtimeReasonsController(
	$scope,
	$location,
	Pagination,
	$modal,
	NotificationFactory,
	DowntimeReasonsHelper,
	PermissionService
) {
	$scope.search = $location.search().search;

	$scope.downtimeReasons = [];

	$scope.pagination = {
		itemsPerPage: 10,
		totalItems: $scope.downtimeReasons.length,
		currentPage: $location.search().page || 1,
	};
	
	init();

	function init() {
		PermissionService.redirectIfUnauthorized('viewDowntimeReasons');
	}

	//DowntimeReasonsHelper.getList()
	//    .then(function (response) {
	//        $scope.allDowntimeReasons = response;
	//         $scope.pagination.totalItems = response.meta.totalResultCount;
			
	//    });
	$scope.deleteDowntimeReason = function(downtimeReason) {
		var modalInstance = $modal.open({
			template: deleteTemplate,
			controller: 'DeleteModalContentController',
			windowClass: 'default-modal',
			resolve: {
				deletedItemName: function() {
					return downtimeReason.code;
				},
			},
		});

		modalInstance.result.then(function() {
			DowntimeReasonsHelper.remove(downtimeReason)
			.then(function(response) {
				pageChanged($scope.pagination.currentPage, $scope.pagination.itemsPerPage);
				NotificationFactory.success("Success: Removed downtime reason");
			}, function(error) {
				NotificationFactory.error("Error: Couldn't remove downtime reason");
			});
		});
	};

	var pageChanged = function(page, itemsPerPage) {
		$location.search('page', page);
		DowntimeReasonsHelper.getList({
			skip: Pagination.skip(page, itemsPerPage),
			take: itemsPerPage,
			orderBy: 'Description',
		})
		.then(function(response) {
			$scope.downtimeReasons = response;
			$scope.pagination.totalItems = response.meta.totalResultCount;
		});
	};

	$scope.$watch('pagination.currentPage', function(newValue, oldValue) {
		pageChanged(newValue, $scope.pagination.itemsPerPage);
	});
}