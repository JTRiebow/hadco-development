import * as angular from 'angular';

import * as template from './time-cell.html';

angular
    .module('sharedModule')
    .directive('uiGridTimeCell', [ uiGridTimeCell ]);

function uiGridTimeCell() {
    return {
        link: link,
        restrict: 'E',
        scope: {
            row: '=',
            colFieldKey: '@',
            isCellEditable: '=',
            isCellHidden: '=',
            required: '=',
            focusId: '=',
            type: '@',

        },
        template,
    };

    function link(scope, element, attrs) {
    }
}
