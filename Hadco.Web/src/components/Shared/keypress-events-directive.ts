import * as angular from 'angular';

angular.module('sharedModule').directive('keypressEvents', function() {
    return function(scope, element, attrs) {
        element.bind("keydown keypress", function(event) {
            if (event.which === 13) { // enter
                if (event.shiftKey) {
                    scope.$apply(function() {
                        scope.$eval(attrs.shiftEnter);
                    });
                }
                else { //uncommenting this will allow "enter" to save without opening new rows in ui-grid cells       .  
                    scope.$apply(function() {
                        scope.$eval(attrs.enter);
                    });
                }

                event.preventDefault();
            }
            if (event.which === 27) { // escape
                scope.$apply(function() {
                    scope.$eval(attrs.escape);
                });

                event.preventDefault();
            }
        });
    };
});