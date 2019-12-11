import * as angular from 'angular';

import * as template from './text-box-cell.html';

angular
    .module('sharedModule')
    .directive('uiGridTextBox', [ uiGridTextBox ]);

function uiGridTextBox() {
    return {
        link: link,
        restrict: 'E',
        scope: {
            row: '=',
            colFieldKey: '@',
            colField: '=',
            modelColField: '@',
            type: '@',
            maxlength: '@',
            isCellEditable: '=',
            isCellHidden: '=',
            required: '=',
        },
        template,
    };

    function link(scope, element, attrs) {
    }
}