import * as angular from 'angular';

import * as template from './employee-search-nav.html';

angular
    .module('employeeModule')
    .directive('employeeSearch', employeeSearch);

employeeSearch.$inject = [ '$location', 'Users' ];

function employeeSearch($location, Users) {
    return {
        link: link,
        restrict: 'E',
        template,
    };

    function link(scope, element, attrs) {
        Users.getList()
            .then(function(response) {
                scope.employeeList = response;
            });

        scope.clearSearch = function() {
            scope.selectedEmployee = null;
        };


        scope.searchEmployee = function(selectedEmployee) {
            if (selectedEmployee) {
                $location.path("/employee-search/" + selectedEmployee.employeeID);
                scope.clearSearch();
            }
        };
    }
}