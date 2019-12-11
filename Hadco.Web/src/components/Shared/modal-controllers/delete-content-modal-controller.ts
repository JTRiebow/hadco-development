import * as angular from 'angular';

angular
    .module('modalModule')
    .controller('DeleteModalContentController', DeleteModalContentController);

DeleteModalContentController.$inject = [ '$scope', '$modalInstance', 'deletedItemName' ];

function DeleteModalContentController($scope, $modalInstance, deletedItemName) {
    $scope.deletedItemName = deletedItemName;

    $scope.confirm = function() {
        $modalInstance.close();
    };

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };

}

//angular.module('modalModule').factory('deleteOccurrence', ['EmployeeTimers', 'Users', '$routeParams', 'NotificationFactory',
//    function (EmployeeTimers, Users, $routeParams, NotificationFactory) {
//        return function (removedValue, lastQuery, getLabel) {
//            Users.one($routeParams.employeeId).one('EmployeeTimer', moment(new Date($routeParams.dayId)).format('YYYY-MM-DD')).get({ "departmentID": $routeParams.departmentId })
//            .then(function (timer) {
//                EmployeeTimers.one(timer.employeeTimerID).one('Occurrences', removedValue.occurrenceID).remove()
//                .then(function (data) {
//                    NotificationFactory.success('Occurrence removed');
//                }, function (data) {
//                    NotificationFactory.error("Couldn't remove Occurrence");
//                })

//            })
//        }
//    }
//])