import * as angular from 'angular';

import * as template from '../nav.html';
import { IPermissionService } from '../permissions/permission-service';

angular
    .module('sharedModule')
    .component('htNav', {
        controller: getController(),
        controllerAs: 'vm',
        template,
    });

function getController() {
    class HtNavController {
        public can;
        public canSeeTimerNav;
        public canSeeHrNav;
        public canSeeTruckingNav;
        public canAccessAdmin;
        public canAccessCSV;
        
        constructor(private PermissionService: IPermissionService) {}
        
        public $onInit() {
            this.can = this.PermissionService.can;
            this.canSeeTimerNav = this.PermissionService.canSeeTimerNav;
            this.canSeeHrNav = this.PermissionService.canSeeHrNav;
            this.canSeeTruckingNav = this.PermissionService.canSeeTruckingNav;
            this.canAccessAdmin = this.PermissionService.canAccessAdmin;
            this.canAccessCSV = this.PermissionService.canAccessCSV;
        }
    }
    
    HtNavController.$inject = [ 'PermissionService' ];
    
    return HtNavController;
}