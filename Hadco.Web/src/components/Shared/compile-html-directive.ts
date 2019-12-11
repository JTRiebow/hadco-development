import * as angular from 'angular';

angular
    .module('sharedModule')
    .directive('compileHtml', compileHtmlDirective);

compileHtmlDirective.$inject = ['$compile'];

function compileHtmlDirective($compile) {
    return {
        restrict: 'EA',
        link(scope, elem, attrs) {
            scope.$watchGroup([
                () => scope.$eval(attrs.compileHtml || attrs.template),
                () => scope.$eval(attrs.context),
            ], ([template, context]) => {
                elem.html('');
                elem.append($compile(template)(context || scope));
            });
        }
    } as angular.IDirective;
}