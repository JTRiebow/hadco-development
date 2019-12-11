import * as angular from 'angular';

angular
    .module('authorizationModule')
    .factory('authInterceptor', authInterceptor);

authInterceptor.$inject = [ '$injector', 'appConfig', '$q' ];

function authInterceptor($injector, appConfig, $q) {
    return {
        request(config) {
            config.headers.Authorization = `Bearer ${localStorage.token}`;
            
            return config;
        },
        responseError(rejection) {
            if (rejection.status === 401 && rejection.config.url != appConfig.authUrl) {
                // 401, stall requests until successful login
                // Let app know to login is needed.
                var deferred = $q.defer();
                var authorization = $injector.get('authorization');
                var $http = $injector.get('$http');
                
                authorization.loginNeeded().then(retry);

                return deferred.promise;
            }
            else {
                return $q.reject(rejection);
            }

            function retry(updatedHeaders) {
                angular.extend(rejection.config.headers, updatedHeaders);
                $http(rejection.config).then(function(response) {
                    deferred.resolve(response);
                }, deferred.reject);
            }
        },
    };

}