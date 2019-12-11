import * as angular from 'angular';

angular
    .module('modalModule')
    .controller('CancelNotificationController', CancelNotificationController);

CancelNotificationController.$inject = [ '$scope', '$modalInstance' ];

function CancelNotificationController($scope, $modalInstance) {
    var modalInstance = $modalInstance;
    $scope.save = function() {
        modalInstance.close('save');
    };

    $scope.cancel = function() {
        modalInstance.close('cancel');
    };
}