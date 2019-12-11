import * as angular from 'angular';

angular
    .module('authorizationModule')
    .run(authInit);

authInit.$inject = [ 'authorization' ];
    
function authInit(authorization) {
    authorization.init();
}