import * as angular from 'angular';

angular
    .module('authorizationModule')
    .controller('loginController', loginController);

loginController.$inject = [ '$scope', 'authorization' ];

function loginController($scope, authorization) {
    $scope.login = function() {
        authorization.login({ username: $scope.username, password: $scope.password });
    };
}