import * as angular from 'angular';

import * as template from './dropdown-cell.html';

angular
    .module('sharedModule')
    .directive('uiGridDropdown', [ uiGridDropdown ]);

function uiGridDropdown() {
    return {
        link: link,
        restrict: 'E',
        scope: {
            row: '=',
            colFieldKey: '@',
            colField: '=',
            isCellEditable: '=',
            isCellHidden: '=',
            required: '=',
            focusId: '@',
            type: '@',
            dropdownList: '=',
            modelCollection: '@',
            modelIdentifier: '@',
            checkFunction: '&?',
            dragHandle: '=',
        },
        template,
    };

    function link(scope, element, attrs) {
        scope.checkCell = function(item) {
            scope.checkFunction && scope.checkFunction()(item, scope.row.entity.sequence, scope.modelCollection, scope.row.entity);
        };
    }
}
