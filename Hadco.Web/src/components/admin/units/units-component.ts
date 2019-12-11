import * as angular from 'angular';

import * as template from './units.html';
import * as deleteTemplate from '../../Shared/modal-templates/delete-item-modal.html';

angular
    .module('adminModule')
    .component('htUnits', {
        controller: unitsController,
        controllerAs: 'vm',
        template,
    });

unitsController.$inject = [ '$scope', '$location', 'Pagination', '$modal', 'NotificationFactory', 'Units' ];

function unitsController(
    $scope,
    $location,
    Pagination,
    $modal,
    NotificationFactory,
    Units
) {

    $scope.search = $location.search().search;
    
    $scope.units = [];

        $scope.pagination = {
        itemsPerPage: 15,
        totalItems: $scope.units.length,
        currentPage: $location.search().page || 1,
    };

    $scope.deleteUser = function(unit) {
        var modalInstance = $modal.open({
            template: deleteTemplate,
            controller: 'DeleteModalContentController',
            resolve: {
                deletedItemName: function() {
                    return unit.name;
                },
            },
        });

        modalInstance.result.then(function(unit) {
            Units.one(unit.unitID).remove()
            .then(function(response) {
                pageChanged($scope.pagination.currentPage, $scope.pagination.itemsPerPage);
                NotificationFactory.success("Removed Unit");
            });
        });
    };

    var pageChanged = function(page, itemsPerPage) {
        $location.search('page', page);
        Units.getList({
            skip: Pagination.skip(page, itemsPerPage),
            take: itemsPerPage,
            orderBy: 'name',
        })
        .then(function(response) {
            $scope.units = response;
            $scope.pagination.totalItems = response.meta.totalResultCount;
        });
    };

    $scope.$watch('pagination.currentPage', function(newValue, oldValue) {
        pageChanged(newValue, $scope.pagination.itemsPerPage);
    });

}