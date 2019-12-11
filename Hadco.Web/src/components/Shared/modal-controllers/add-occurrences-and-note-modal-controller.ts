import * as angular from 'angular';

angular
    .module('modalModule')
    .controller('addOccurrencesAndNoteModalController', addOccurrencesAndNoteModalController);

addOccurrencesAndNoteModalController.$inject = [ '$scope', '$modalInstance', 'currentOccurrences', 'availableOccurrences' ];

function addOccurrencesAndNoteModalController($scope, $modalInstance, currentOccurrences, availableOccurrences) {
    $scope.currentOccurrences = currentOccurrences;
    $scope.availableOccurrences = availableOccurrences;
    $scope.newOccurrence = { occurrences: currentOccurrences };
    $scope.isDetailedApprovalPage = true;
        
    $scope.confirm = function(e) {
        $modalInstance.close(e);
    };

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };

}