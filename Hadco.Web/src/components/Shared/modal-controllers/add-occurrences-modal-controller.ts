import * as angular from 'angular';

angular
    .module('modalModule')
    .controller('addOccurrencesModalController', addOccurrencesModalController);

addOccurrencesModalController.$inject = [ '$scope', '$modalInstance', 'availableEmployees', 'availableOccurrences' ];

function addOccurrencesModalController($scope, $modalInstance, availableEmployees, availableOccurrences) {

    $scope.availableEmployees = availableEmployees;
    $scope.availableOccurrences = availableOccurrences;

    $scope.confirm = function(e) {
        $modalInstance.close(e);
    };

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };

}