import * as angular from 'angular';

import * as template from './truck-classifications.html';
import * as deleteTemplate from '../../Shared/modal-templates/delete-item-modal.html';

angular
    .module('adminModule')
    .component('htTruckClassifications', {
        controller: adminListsController,
        controllerAs: 'vm',
        template,
    });

adminListsController.$inject = [
    '$scope',
    '$location',
    'Pagination',
    '$modal',
    'NotificationFactory',
    'TruckClassificationsHelper',
    'PermissionService',
];

function adminListsController(
    $scope,
    $location,
    Pagination,
    $modal,
    NotificationFactory,
    TruckClassificationsHelper,
    PermissionService,
) {
    $scope.search = $location.search().search;
    $scope.deleteListItem = deleteListItem;
    
    $scope.list = [];

        $scope.pagination = {
        itemsPerPage: 10,
        totalItems: $scope.list.length,
        currentPage: $location.search().page || 1,
    };

    init();

    function init() {
            $scope.$watch('pagination.currentPage', function(newValue, oldValue) {
            pageChanged(newValue, $scope.pagination.itemsPerPage);
        });
        PermissionService.redirectIfUnauthorized('viewTruckingClassifications');
    }

    function deleteListItem(listItem) {
        var modalInstance = $modal.open({
            template: deleteTemplate,
            controller: 'DeleteModalContentController',
            resolve: {
                deletedItemName: function() {
                    return listItem.name;
                },
            },
        });

        modalInstance.result.then(removeEquipmentItem);
    }

    function removeEquipmentItem(listItem) {
        
            TruckClassificationsHelper.remove(listItem)
            .then(function(response) {
                pageChanged($scope.pagination.currentPage, $scope.pagination.itemsPerPage);
                TruckClassificationsHelper.clearCache();
                NotificationFactory.success("Success: Removed Truck Classification");
            }, function(error) {
                NotificationFactory.error("Error: Couldn't remove Truck Classification");
            });
        
    }

    function pageChanged(page, itemsPerPage) {
        $location.search('page', page);
        TruckClassificationsHelper.getList({
            skip: Pagination.skip(page, itemsPerPage),
            take: itemsPerPage,
            orderBy: 'name',
        })
        .then(function(response) {
            $scope.truckClassifications = response;
            $scope.pagination.totalItems = response.meta.totalResultCount;
        });
    }
}