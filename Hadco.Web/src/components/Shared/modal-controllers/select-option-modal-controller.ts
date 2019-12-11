import * as angular from 'angular';

angular
    .module('modalModule')
    .controller('selectOptionModalContentController', selectOptionModalContentController);

selectOptionModalContentController.$inject = [ '$scope', '$modalInstance', 'options', 'entity' ];

function selectOptionModalContentController($scope, $modalInstance, options, entity) {
    $scope.options = options || [];
    $scope.entity = entity || {};

    $scope.confirm = function(selected) {
        if (selected)
            $modalInstance.close(selected);
    };

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };
}