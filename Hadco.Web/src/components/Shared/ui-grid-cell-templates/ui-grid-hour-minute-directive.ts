import * as angular from 'angular';

import * as template from './hour-minute-cell.html';

angular
    .module('sharedModule')
    .directive('uiGridHourMinute', uiGridHourMinute);

function uiGridHourMinute() {
    return {
        link: link,
        restrict: 'E',
        scope: {
            originalMinutes: '=',
            row: '=',
            focusId: "@",
            isCellEditable: '=',
            isCellHidden: '=',
            onChangeFunction: '&?',
        },
        template,
    };

    function link(scope, element, attrs) {
        scope.originalMinutes = scope.originalMinutes || 0;

        scope.hours = Math.floor(scope.originalMinutes / 60);

        scope.minutes = displayTwoDigits(scope.originalMinutes % 60);

        scope.$watch('originalMinutes', function(newValue, oldValue) {
            if (newValue != oldValue) {
                scope.hours = Math.floor(newValue / 60);
                scope.minutes = displayTwoDigits(newValue % 60);
                scope.onChangeFunction && scope.onChangeFunction()(scope.row.entity);
            }
        });

        scope.updateMinutes = function(hours, minutes) {
            hours = hours || 0;
            minutes = minutes || 0;
            scope.originalMinutes = parseInt(hours) * 60 + parseInt(minutes);
        };

        function displayTwoDigits(n) {
            return n > 9 ? "" + n : "0" + n;
        }
    }
}
