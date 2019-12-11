import * as angular from 'angular';

import * as template from './type-ahead.html';

angular
    .module('sharedModule')
    .directive('uiGridTypeahead', [ uiGridTypeahead ]);

function uiGridTypeahead() {
    return {
        link: link,
        restrict: 'E',
        scope: {
            row: '=',
            colFieldKey: '=',
            colField: '=',
            isCellEditable: '=',
            isCellHidden: '=',
            required: '=',
            focusId: '@',
            type: '@',
            typeaheadList: '=',
            modelCollection: '@',
            modelIdentifier: '@',
            checkFunction: '&?',
            patchKeyValue: '=',
        },
        template,
    };

    function link(scope, element, attrs) {
        scope.checkCell = function(item) {
            scope.checkFunction && scope.checkFunction()(item, scope.row.entity.sequence, scope.modelCollection, scope.modelIdentifier, scope.patchKeyValue, scope.row);
        };
    }
}
