import * as angular from 'angular';

angular
    .module('runOnceModule')
    .run(detectMobile);

detectMobile.$inject = [ '$rootScope' ];

function detectMobile($rootScope) {
    //$$$ Uncomment when ready for android / ios apps
    // var userAgent = navigator.userAgent || navigator.vendor || window.opera;
    //
    //$rootScope.android = Boolean(userAgent.match(/Android/i));
    //
    //$rootScope.ios = Boolean(userAgent.match(/iPad|iPhone|iPod/i));
}