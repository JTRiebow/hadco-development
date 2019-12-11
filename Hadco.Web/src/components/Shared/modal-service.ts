import * as angular from 'angular';

class ModalService {
    extendOptions(base: ng.material.IDialogOptions, provided = {} as ng.material.IDialogOptions): ng.material.IDialogOptions {
        const baseLocals = base.locals || {};
        const providedLocals = provided.locals || {};
        
        return {
            ...base,
            ...provided,
            locals: {
                ...baseLocals,
                ...providedLocals,
            },
        };
    }
}

ModalService.$inject = [];

angular
    .module('sharedModule')
    .service('ModalService', ModalService);

export {
    ModalService,
};