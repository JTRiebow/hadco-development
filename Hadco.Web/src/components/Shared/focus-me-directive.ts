import * as angular from 'angular';

angular.module('employeeModule').directive('focusMe', function($timeout) {
  return {
    link(scope, element, attrs) {
        (element[0] as HTMLInputElement).autofocus = true;
      },
  };
});