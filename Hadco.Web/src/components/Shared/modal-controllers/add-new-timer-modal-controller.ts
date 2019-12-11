import * as angular from 'angular';

angular
    .module('modalModule')
    .controller('NewTimerModalController', NewTimerModalController);

NewTimerModalController.$inject = [ '$scope', '$modalInstance', '$location', 'users' ];

function NewTimerModalController($scope, $modalInstance, $location, users) {

    $scope.users = users;
    $scope.newTimer = {};
    $scope.submitted = false;
   
    $scope.typeaheadEmployees = function(search) {
        return filterUsers(search);
    };

    function filterUsers(search) {
        var i = 0;
        var filteredUsers = $scope.users
            .filter((user: any) => user.name.toLowerCase().includes(search.toLowerCase()))
            .filter(user => {
                i++;
                return i <= 10;
            })
        return filteredUsers
    }

    $scope.employeeSelected = function() {
        $scope.newTimer.department = null;
        var departments = $scope.newTimer.employee.departments;
        var userHasOneDepartment = departments.length === 1;
        $scope.userHasMultipleDepartments = departments.length > 1;
        if (userHasOneDepartment) {
            $scope.newTimer.department = $scope.newTimer.employee.departments[0];
        }
    };

    $scope.confirm = function(e, form) {
        $scope.submitted = true;
        if (!form.$invalid) {
            $modalInstance.close(e);
        }
    };

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };

}