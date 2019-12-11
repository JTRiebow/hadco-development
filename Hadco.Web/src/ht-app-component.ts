import * as angular from 'angular';

import * as template from './ht-app.html';

angular
    .module('hadcoApp')
    .directive('htApp', () => ({
        controller: 'authorizationController',
        controllerAs: 'vm',
        require: '^offCanvasWrap',
        scope: true,
        template,
    }));