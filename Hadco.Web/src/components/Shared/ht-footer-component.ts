import * as angular from 'angular';

import * as template from '../footer.html';

angular
    .module('sharedModule')
    .component('htFooter', {
        controller: getController(),
        controllerAs: 'vm',
        template,
    });

function getController() {
    class HtFooterController {
        
    }
    
    HtFooterController.$inject = [];
    
    return HtFooterController;
}