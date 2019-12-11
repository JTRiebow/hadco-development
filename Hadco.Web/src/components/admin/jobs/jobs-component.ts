import * as angular from 'angular';
import * as _ from 'lodash';

import * as template from './jobs.html';
import * as deleteTemplate from '../../Shared/modal-templates/delete-item-modal.html';

angular
    .module('adminModule')
    .component('htJobs', {
        controller: jobsController,
        controllerAs: 'vm',
        template,
    });

jobsController.$inject = [
    '$scope',
    '$location',
    'Pagination',
    '$modal',
    'NotificationFactory',
    'JobsHelper',
    'PermissionService',
];

function jobsController(
    $scope,
    $location,
    Pagination,
    $modal,
    NotificationFactory,
    JobsHelper,
    PermissionService
) {

    $scope.search = $location.search().search;
    
    $scope.jobs = [];

        $scope.pagination = {
        itemsPerPage: 50,
        totalItems: $scope.jobs.length,
        currentPage: $location.search().page || 1,
    };

    init();

    function init() {
        PermissionService.redirectIfUnauthorized('viewJobs');
    }

    $scope.deleteJob = function(index) {
        var modalInstance = $modal.open({
            template: deleteTemplate,
            controller: 'DeleteModalContentController',
            resolve: {
                deletedItemName: function() {
                    return $scope.jobs[index].name;
                },
            },
        });

        modalInstance.result.then(function(claim) {
            NotificationFactory.success("Thank You");
            $scope.jobs.splice(index, 1);
        });
    };

    var pageChanged = function(page, itemsPerPage) {
        JobsHelper.getList({
            search: $scope.search,
            skip: Pagination.skip(page, itemsPerPage),
            take: itemsPerPage,
            orderBy: 'name',
        })
        .then(function(response) {
            $scope.jobs = response;
            $scope.pagination.totalItems = response.meta.totalResultCount;
        });
    };

    $scope.$watch('pagination.currentPage', function(newValue, oldValue) {
        pageChanged(newValue, $scope.pagination.itemsPerPage);
    });

    $scope.$watch('search', _.debounce(function(newValue, oldValue) {
        if (newValue == oldValue)
            return;
        pageChanged($scope.pagination.currentPage, $scope.pagination.itemsPerPage);
    }, 500));
}