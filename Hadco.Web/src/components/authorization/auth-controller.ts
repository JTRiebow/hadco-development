import * as angular from 'angular';

import * as loginTemplate from '../login.html';
import * as headerTemplate from '../header.html';
import * as footerTemplate from '../footer.html';
import * as navTemplate from '../nav.html';

angular
    .module('authorizationModule')
    .controller('authorizationController', authorizationController);

authorizationController.$inject = [
    '$window',
    '$rootScope',
    '$scope',
    '$q',
    '$timeout',
    '$location',
    'authorization',
    'CurrentUser',
    'NotificationFactory',
    'ActivityService',
    'PermissionService',
];

function authorizationController(
    $window,
    $rootScope,
    $scope,
    $q,
    $timeout,
    $location,
    authorization,
    CurrentUser,
    NotificationFactory,
    ActivityService,
    PermissionService
) {
    var vm = this;
    
    vm.loginTemplate = loginTemplate;
    vm.headerTemplate = headerTemplate;
    vm.footerTemplate = footerTemplate;
    vm.navTemplate = navTemplate;
    
    CurrentUser.clear();

    vm.can = PermissionService.can;
    vm.canAccessCSV = PermissionService.canAccessCSV;
    vm.canAccessAdmin = PermissionService.canAccessAdmin;
    vm.canSeeTimerNav = PermissionService.canSeeTimerNav;
    vm.canSeeTruckingNav = PermissionService.canSeeTruckingNav;
    vm.canSeeHrNav = PermissionService.canSeeHrNav;
    vm.currentUser = CurrentUser;
    
    $rootScope.currentUserService = CurrentUser;

    $scope.initialAuthCheckDone = false;
    $scope.$on('onAuthorizedStatusCheckDone', function() {
        $timeout(function() {
            $scope.initialAuthCheckDone = true;
        });
    });

    // The success of this request will indicate to the application the
    // the user is logged in.
    CurrentUser.isAuthorized().then(function(isAuthorized) {
        if (isAuthorized) {
            CurrentUser.setLoggedIn(true);
        }
        else {
            CurrentUser.setLoggedIn(false);
            $timeout(function() {
                $window.location = "/accessDenied.html";
            });
        }
        $rootScope.$broadcast('onAuthorizedStatusCheckDone');
    });

    $scope.$on('onLoginNeeded', function() {
        CurrentUser.setLoggedIn(false);
        $rootScope.$broadcast('onAuthorizedStatusCheckDone');
    });

    $scope.$on('onLoginSuccess', function(event) {
        CurrentUser.setLoggedIn(true);
        CurrentUser.get();
    });

    $scope.$on('onLoginFailure', function() {
        CurrentUser.setLoggedIn(false);
        NotificationFactory.error("There was a problem logging in.<br/>Check user name and password.");
    });

    $scope.logout = function() {
        CurrentUser.setLoggedIn(false);
        authorization.logout();
        $timeout(function() {
            $location.path('/');
        });
    };
}