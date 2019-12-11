import * as angular from 'angular';

angular
    .module('modalModule')
    .controller('ClockInModalContentController', ClockInModalContentController);

ClockInModalContentController.$inject = [ '$scope', '$modalInstance', 'currentUser', 'PitsHelper', 'TMCrushingOrFrontOffice' ];

function ClockInModalContentController($scope, $modalInstance, currentUser, PitsHelper, TMCrushingOrFrontOffice) {
    $scope.currentUser = currentUser;
    $scope.clockInResponse = {};
    $scope.TMCrushingOrFrontOffice = TMCrushingOrFrontOffice;

    if ($scope.TMCrushingOrFrontOffice.name == 'TMCrushing') {
        $scope.isTMCrushing = true;
    }

    if ($scope.isTMCrushing) {
        PitsHelper.getList().then(function(response) {
            $scope.pits = response;
        });
    }

    $scope.confirm = function() {
        if ($scope.isTMCrushing && !$scope.clockInResponse.pitID) {
            return;
        }

        $modalInstance.close($scope.clockInResponse);
    };

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };
}