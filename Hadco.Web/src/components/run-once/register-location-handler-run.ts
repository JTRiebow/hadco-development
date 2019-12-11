import * as angular from 'angular';

angular
    .module('runOnceModule')
    .run(registerLocationHandler);

registerLocationHandler.$inject = [ '$rootScope', '$modalStack' ];

function registerLocationHandler($rootScope, $modalStack) {
    $rootScope.$on('$locationChangeStart', function(event) {
        var top = $modalStack.getTop();

        if (top) {
            $modalStack.dismiss(top.key);
            event.preventDefault();
        }
    });
}