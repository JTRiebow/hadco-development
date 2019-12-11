import * as angular from 'angular';

import * as template from './materials.html';
import * as deleteTemplate from '../../Shared/modal-templates/delete-item-modal.html';

angular
    .module('adminModule')
    .component('htMaterials', {
        controller: materialController,
        controllerAs: 'vm',
        template,
    });

materialController.$inject = [
    '$scope',
    '$location',
    'Pagination',
    '$modal',
    'NotificationFactory',
    'MaterialsHelper',
    'PermissionService',
];

function materialController(
    $scope,
    $location,
    Pagination,
    $modal,
    NotificationFactory,
    MaterialsHelper,
    PermissionService
) {
    $scope.search = $location.search().search;
    
    $scope.materials = [];
    $scope.pageOfMaterials = [];

    $scope.pagination = {
        itemsPerPage: 10,
        totalItems: $scope.materials.length,
        currentPage: $location.search().page || 1,
    };

    init();

    function init() {
        PermissionService.redirectIfUnauthorized('viewMaterials');
    }

    $scope.deleteMaterial = function(material) {
        var modalInstance = $modal.open({
            template: deleteTemplate,
            controller: 'DeleteModalContentController',
            resolve: {
                deletedItemName: function() {
                    return material.name;
                },
            },
        });

        modalInstance.result.then(function() {
            MaterialsHelper.remove(material)
            .then(function(response) {
                pageChanged($scope.pagination.currentPage, $scope.pagination.itemsPerPage);
                NotificationFactory.success("Success: Removed material");
            }, function(error) {
                NotificationFactory.error("Error: Couldn't remove material");
            });
        });
    };

    var pageChanged = function(page, itemsPerPage) {
        $location.search('page', page);
        MaterialsHelper.getList({
            skip: Pagination.skip(page, itemsPerPage),
            take: itemsPerPage,
            orderBy: 'name',
        })
        .then(function(response) {
            $scope.materials = response;
            $scope.pagination.totalItems = response.meta.totalResultCount;
        });
    };

    $scope.$watch('pagination.currentPage', function(newValue, oldValue) {
        pageChanged(newValue, $scope.pagination.itemsPerPage);
    });
}