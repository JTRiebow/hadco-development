import * as angular from 'angular';

angular
    .module('sharedModule')
    .directive('numbersOnly', function() {
        return {
            require: 'ngModel',
            link(scope, element, attr, ngModelCtrl: ng.INgModelController) {
                function fromUser(text) {
                    if (text) {
                        var transformedInput = text.replace(/[^0-9]/g, '');

                        if (transformedInput !== text) {
                            ngModelCtrl.$setViewValue(transformedInput);
                            ngModelCtrl.$render();
                        }
                        return transformedInput;
                    }
                    return undefined;
                }
                ngModelCtrl.$parsers.push(fromUser);
            },
        };
    });