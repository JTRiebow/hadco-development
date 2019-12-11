import * as angular from 'angular';

angular
    .module('modalModule')
    .controller('chooseRoleModalController', chooseRoleModalController);

chooseRoleModalController.$inject = [ '$scope', '$modalInstance', '$rootScope' ];

function chooseRoleModalController($scope, $modalInstance, $rootScope) {
    var vm = this;
    init();

    function init() {
        vm.isSupervisor = $rootScope.me.roles.isSupervisor();
        vm.isManager = $rootScope.me.roles.isManager();
        vm.isBilling = $rootScope.me.roles.isBilling();
        vm.isAccounting = $rootScope.me.roles.isAccounting();

    }

    vm.save = function() {
        var selectedRoles = { supervisor: false, manager: false, billing: false, accounting: false };
        if (vm.supervisor && vm.isSupervisor) {
            selectedRoles.supervisor = true;
        }
        if (vm.manager && vm.isManager) {
            selectedRoles.manager = true;
        }
        if (vm.billing && vm.isBilling) {
            selectedRoles.billing = true;
        }
        if (vm.accounting && vm.isAccounting) {
            selectedRoles.accounting = true;
        }

        $modalInstance.close(selectedRoles);


    };

    vm.cancel = function() {
        $modalInstance.close('cancel');
    };


}