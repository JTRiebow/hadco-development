import * as angular from 'angular';

angular
    .module('runOnceModule')
    .run(assignMe);

assignMe.$inject = [ '$rootScope', 'CurrentUser' ];

function assignMe($rootScope, CurrentUser) {
    $rootScope.me = {
        roles: CurrentUser.roles,
        departments: CurrentUser.departments,
    };
}