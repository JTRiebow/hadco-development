import * as angular from 'angular';

angular
    .module('runOnceModule')
    .run(registerRoleSet);

registerRoleSet.$inject = [ '$rootScope', '$location', '$routeParams' ];

function registerRoleSet($rootScope, $location, $routeParams) {
    $rootScope.$on('onRolesSet', function() {
        var roles = $rootScope.me.roles;

        if (roles.isSupervisor() || roles.isManager() || roles.isAccounting() || roles.isSystemAdmin()) {
            if (!roles.isUser()) {
                if ($routeParams.supervisorOrAccounting !== 'supervisor') {
                    $location.path('/supervisor/timer/approval');
                }
            }
            else {
                $location.path('/employee/clock-in');
            }

        }
        else {
            if ($routeParams.employee !== 'employee')
                $location.path('/employee/clock-in');
        }
    });
}