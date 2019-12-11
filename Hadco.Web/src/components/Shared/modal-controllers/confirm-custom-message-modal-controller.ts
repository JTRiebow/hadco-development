import * as angular from 'angular';

angular
    .module("modalModule")
    .controller("confirmCustomMessageController", confirmCustomMessageController);

confirmCustomMessageController.$inject = [ '$scope', '$modalInstance', 'customMessage' ];

function confirmCustomMessageController($scope, $modalInstance, customMessage) {
    var vm = this;
    vm.customMessage = customMessage;

    vm.confirm = function() {
        $modalInstance.close();
    };

    vm.cancel = function() {
        $modalInstance.dismiss();
    };
}