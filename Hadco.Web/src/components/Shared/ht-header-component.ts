import * as angular from 'angular';

import * as template from '../header.html';
import { IPermissionService } from '../permissions/permission-service';

angular
    .module('sharedModule')
    .component('htHeader', {
        controller: getController(),
        controllerAs: 'vm',
        template,
    });

function getController() {
    class HtHeaderController {
        public can;
        
        constructor(
            private $timeout: ng.ITimeoutService,
            private $location: ng.ILocationService,
            private authorization,
            private CurrentUser,
            private PermissionService: IPermissionService,
        ) {}
        
        public $onInit() {
            this.can = this.PermissionService.can;
        }
        
        public logout() {
            this.CurrentUser.setLoggedIn(false);
            this.authorization.logout();
            this.$timeout(() => {
                this.$location.path('/');
            });
        }
    }
    
    HtHeaderController.$inject = [
        '$timeout',
        '$location',
        'authorization',
        'CurrentUser',
        'PermissionService',
    ];
    
    return HtHeaderController;
}