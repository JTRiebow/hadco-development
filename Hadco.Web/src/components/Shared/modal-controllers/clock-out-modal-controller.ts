import * as angular from 'angular';

angular.module('modalModule').controller('ClockoutModalContentController', [
    '$scope', '$modalInstance', 'injured', 'CurrentUser',
    function($scope, $modalInstance, injured, CurrentUser) {
        $scope.clock = {
            injured: injured,
        };
        $scope.confirm = function(clock) {
            // if(clock.passcode)
            $modalInstance.close(clock);
        };

        $scope.cancel = function() {
            $modalInstance.dismiss('cancel');
        };
    },
]);