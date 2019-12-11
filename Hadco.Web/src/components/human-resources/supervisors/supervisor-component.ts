import * as angular from 'angular';

import * as template from './supervisor.html';

angular
    .module('humanResourcesModule')
    .component('htSupervisor', {
        controller: supervisorController,
        controllerAs: 'vm',
        template,
    });

supervisorController.$inject = [
    '$scope',
    '$location',
    '$routeParams',
    'Supervisors',
];

function supervisorController(
    $scope,
    $location,
    $routeParams,
    Supervisors
) {

    $scope.search = $location.search().search;

    var supervisorId = $routeParams.supervisorId;

    Supervisors.getList()
        .then(function(data) {
            $scope.supervisors = data;
        });

    if (supervisorId === 'view') {
        $scope.allSupervisors = true;
    }
    else {
        $scope.oneSupervisor = true;
        // get request for the supervisor of supervisorId
    }
}