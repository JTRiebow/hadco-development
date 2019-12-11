import * as angular from 'angular';

angular
    .module('modalModule')
    .controller('SaveAllNotificationController', SaveAllNotificationController);

SaveAllNotificationController.$inject = [ '$scope', '$modalInstance' ];

function SaveAllNotificationController($scope, $modalInstance) {
    var modalInstance = $modalInstance;
    $scope.save = function() {
        modalInstance.close('save');
    };

    $scope.cancel = function() {
        modalInstance.close('cancel');
    };
}