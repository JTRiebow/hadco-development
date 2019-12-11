import * as angular from 'angular';

import * as template from './confirm-modal.html';
import './confirm-modal.scss';
import { ModalService } from '../modal-service';

angular
    .module('sharedModule')
    .component('htConfirmModal', {
        bindings: {
            emitOpenTrigger: '&getOpenTrigger',
            emitCloseTrigger: '&getCloseTrigger',
            emitCancelTrigger: '&getCancelTrigger',
            emitTriggers: '&getTriggers',
        },
        controller: getController(),
        template: '',
    });

function getController() {
    class ConfirmModalController {
        public emitOpenTrigger;
        public emitCloseTrigger;
        public emitCancelTrigger;
        public emitTriggers;
        
        private baseOptions = {
            bindToController: true,
            controller: angular.noop,
            controllerAs: '$ctrl',
            locals: {
                vm: this,
                ok: 'Okay',
                cancel: 'Cancel',
                text: '',
                heading: '',
            },
            template,
        } as ng.material.IDialogOptions;
        
        constructor(
            private $mdDialog: ng.material.IDialogService,
            private ModalService: ModalService,
        ) {}
        
        public $onInit() {
            this.emitOpenTrigger({
                trigger: options => this.open(options),
            });
            this.emitCloseTrigger({
                trigger: data => this.close(data),
            });
            this.emitCancelTrigger({
                trigger: reason => this.cancel(reason),
            });
            
            this.emitTriggers({
                triggers: {
                    open: options => this.open(options),
                    close: data => this.close(data),
                    cancel: reason => this.cancel(reason),
                },
            });
        }
        
        public open(options = {} as ng.material.IDialogOptions) {
            return this.$mdDialog.show(this.ModalService.extendOptions(this.baseOptions, options));
        }
        
        public close(data) {
            return this.$mdDialog.hide(data);
        }
        
        public cancel(reason) {
            return this.$mdDialog.cancel(reason);
        }
    }
    
    ConfirmModalController.$inject = ['$mdDialog', 'ModalService'];
    
    return ConfirmModalController;
}

type ConfirmModalOpenTrigger = (options?: ng.material.IDialogOptions) => ng.IPromise<boolean>;
type ConfirmModalCloseTrigger = (data?) => void;
type ConfirmModalCancelTrigger = (reason?) => void;

export {
    ConfirmModalOpenTrigger,
    ConfirmModalCloseTrigger,
    ConfirmModalCancelTrigger,
};