import * as angular from 'angular';

import * as template from './occurrences.html';
import * as deleteTemplate from '../../Shared/modal-templates/delete-item-modal.html';

angular
    .module('adminModule')
    .component('htOccurrences', {
        controller: occurrencesController,
        controllerAs: 'vm',
        template,
    });

occurrencesController.$inject = [
    '$scope',
    '$location',
    'Pagination',
    '$modal',
    'NotificationFactory',
    'OccurrencesHelper',
    'PermissionService',
];

function occurrencesController(
    $scope,
    $location,
    Pagination,
    $modal,
    NotificationFactory,
    OccurrencesHelper,
    PermissionService
) {
    $scope.search = $location.search().search;
    
    $scope.occurrences = [];

        $scope.pagination = {
        itemsPerPage: 10,
        totalItems: $scope.occurrences.length,
        currentPage: $location.search().page || 1,
    };

    init();
    
    function init() {
        PermissionService.redirectIfUnauthorized('viewOccurrences');
    }

    $scope.deleteOccurrence = function(occurrence) {
        var modalInstance = $modal.open({
            template: deleteTemplate,
            controller: 'DeleteModalContentController',
            resolve: {
                deletedItemName: function() {
                    return occurrence.name;
                },
            },
        });

        modalInstance.result.then(function() {
            OccurrencesHelper.remove(occurrence.occurrenceID)
            .then(function(response) {
                pageChanged($scope.pagination.currentPage, $scope.pagination.itemsPerPage);
                NotificationFactory.success("Success: Removed occurrence");
            }, function(error) {
                NotificationFactory.error("Error: Couldn't remove occurrence");
            });
        });
    };

    var pageChanged = function(page, itemsPerPage) {
        $location.search('page', page);
        OccurrencesHelper.getList({
            skip: Pagination.skip(page, itemsPerPage),
            take: itemsPerPage,
            orderBy: 'name',
        })
        .then(function(response) {
            $scope.occurrences = response;
            $scope.pagination.totalItems = response.meta.totalResultCount;
        });
    };

    $scope.$watch('pagination.currentPage', function(newValue, oldValue) {
        pageChanged(newValue, $scope.pagination.itemsPerPage);
    });
}