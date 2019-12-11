import * as angular from 'angular';

import * as template from './add-text-modal.html';

import './add-text-modal.scss';

angular
    .module('sharedModule')
    .component('addTextModal', {
        bindings: {
            emitTriggers: '&onTriggerEmit',
            emitOpenTrigger: '&onOpenTriggerEmit',
            emitCloseTrigger: '&onCloseTriggerEmit',
            emitCancelTrigger: '&onCancelTriggerEmit',
            label: '<',
            useTextarea: '<',
            title: '<',
        },
        template: '',
        controller: getController(),
    });

function getController() {
    class AddTextModalBaseController {
        public emitTriggers;
        public emitOpenTrigger;
        public emitCloseTrigger;
        public emitCancelTrigger;
        
        private defaultDialogOptions = {
            bindToController: true,
            template,
            controller: angular.noop,
            clickOutsideToClose: true,
            escapeToClose: true,
            controllerAs: 'vm',
        } as ng.material.IDialogOptions;
        
        private defaultLocals = {} as { [key: string]: any };
        
        constructor(private $mdDialog: ng.material.IDialogService) {}
        
        public $onInit(this: AddTextModalBaseController) {
            this.defaultLocals = {
                ...this,
            };
            
            this.emitTriggers({
                triggers: {
                    open: this.open,
                    close: this.close,
                    cancel: this.cancel,
                },
            });
            
            this.emitOpenTrigger({
                trigger: this.open,
            });
            
            this.emitCloseTrigger({
                trigger: this.close,
            });
            
            this.emitCancelTrigger({
                trigger: this.cancel,
            });
        }
        
        public open = (options = {} as { [key: string]: any }) => {
            const { locals: providedLocals = {} } = options;
            const { vm = {} } = providedLocals;
            
            const locals = {
                ...this.defaultLocals,
                ...providedLocals,
            };
            
            const finalOptions = {
                ...this.defaultDialogOptions,
                ...options,
                locals,
            };
            
            return this.$mdDialog.show(finalOptions);
        };
        
        public close = event => {
            return this.$mdDialog.hide(event);
        };
        
        public cancel = event => {
            return this.$mdDialog.cancel(event);
        };
    }
    
    AddTextModalBaseController.$inject = ['$mdDialog'];
    
    return AddTextModalBaseController;
}