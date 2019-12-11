import * as angular from 'angular';

angular
    .module('modalModule')
    .controller('mapUnavailableModalController', mapModalController);

mapModalController.$inject = [ '$modalInstance', 'employeeName' ];

function mapModalController($modalInstance, employeeName) {
    var vm = this;

    init();

    function init() {
        vm.employeeName = employeeName;
    }

    vm.close = function() {
        $modalInstance.dismiss();
    };
}
