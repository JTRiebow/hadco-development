import * as angular from 'angular';

angular
    .module('modalModule')
    .controller('NewPasswordController', NewPasswordController);

NewPasswordController.$inject = [ '$scope', '$modalInstance', 'key', 'NotificationFactory' ];

function NewPasswordController($scope, $modalInstance, key, NotificationFactory) {
    $scope.user = {};
    $scope.key = key;
    if (key === 'Password') {
        $scope.password = true;
    }
    else if (key === 'Pin') {
        $scope.pin = true;
    }
    else {
        $scope.username = true;
    }
    $scope.confirm = function(user) {
        if (user.password1 === user.password2) {
            $modalInstance.close(user);
        }
    else {
            NotificationFactory.error("Error: " + key + "s don't match");
        }
    };

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };
}