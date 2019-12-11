import * as angular from 'angular';

angular
    .module('modalModule')
    .controller('AddTimeModalContentController', AddTimeModalContentController);

AddTimeModalContentController.$inject = [ '$scope', '$modalInstance', 'date', 'pitList', 'dep' ];

function AddTimeModalContentController($scope, $modalInstance, date, pitList, dep) {

    $scope.showPit = dep.crush;
    $scope.pitList = pitList;
    $scope.time = {
        clockIn: new Date(date),
        clockOut: new Date(date),
    };
    $scope.confirm = function(time) {
        if (time.clockOut && time.clockIn)
            $modalInstance.close(time);
    };

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };
}
