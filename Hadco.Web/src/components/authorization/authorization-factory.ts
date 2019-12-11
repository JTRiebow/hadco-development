import * as angular from 'angular';

angular
    .module('authorizationModule')
    .factory('authorization', authorizationFactory);

authorizationFactory.$inject = [
    '$q',
    '$rootScope',
    'CurrentUser',
    'appConfig',
];

function authorizationFactory(
    $q,
    $rootScope,
    CurrentUser,
    appConfig
) {
    // Set URL for getting token
    var url = appConfig.authUrl;

    var loginNeededDeferred = null;

    return {
        init: function() {
            if (localStorage['token']) {
                CurrentUser.setToken(localStorage['token']);
            }
        },
        login: function(credentials) {
            var deferred = $q.defer();

            var onError = function(response) {
                deferred.reject();
                $rootScope.$broadcast('onLoginFailure');
            };

            // Request token with credentials
            CurrentUser.postCredentials(url, credentials)
            .then(function(response) {
                var token = CurrentUser.extractToken(response);
                if (token) {
                    var updatedHeader = CurrentUser.setToken(token);
                    deferred.resolve();
                    if (loginNeededDeferred) {
                        loginNeededDeferred.resolve(updatedHeader);
                        loginNeededDeferred = null;
                    }
                    $rootScope.$broadcast('onLoginSuccess');
                    $rootScope.$broadcast('onRolesSet');
                }
                else {
                    onError(response);
                }
            })
            .catch(onError);

            return deferred.promise;
        },
        loginNeeded: function() {
            if (!loginNeededDeferred) {
                loginNeededDeferred = $q.defer();
                CurrentUser.setToken(null);
                $rootScope.$broadcast('onLoginNeeded');
            }
            return loginNeededDeferred.promise;
        },
        logout: function() {
            localStorage.removeItem('token');
            CurrentUser.setToken(null);
            CurrentUser.clear();
            $rootScope.$broadcast('onLogoutSuccess');
            $rootScope.$broadcast('onLoginNeeded');
        },
    };
}