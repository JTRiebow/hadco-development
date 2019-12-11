import * as angular from 'angular';

angular
    .module('authorizationModule')
    .config(authInterceptConfig);

authInterceptConfig.$inject = [ '$httpProvider' ];

function authInterceptConfig($httpProvider) {
    $httpProvider.interceptors.push('authInterceptor');
}