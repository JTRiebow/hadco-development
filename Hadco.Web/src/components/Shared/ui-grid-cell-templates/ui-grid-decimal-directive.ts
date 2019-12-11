import * as angular from 'angular';

import * as template from './decimal-cell.html';

angular
    .module('sharedModule')
    .directive('uiGridDecimal', uiGridDecimal);

function uiGridDecimal() {
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
            decimalPlaces: '@',
        },
        template,
    };

    function link(scope, element, attrs) {
        scope.decimalPlaces = scope.decimalPlaces || 2;
    }
}