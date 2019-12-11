import * as angular from 'angular';
import * as _ from 'lodash';

angular.module('sharedModule').directive('isActive', [
    '$location',
    function($location) {
        return {
            restrict: 'A',
            link: function(scope, element, attrs) {

                var isActive = function(path) {
                    var re = /([^\/]+)+/g;
                    var locationParts = $location.path().replace('index.html', '').match(re);
                    var pathParts = path.match(re);

                    if (_.isEqual(locationParts, pathParts)) {
                        return true;
                    }

                    if (locationParts === null) {
                        locationParts = [];
                    }

                    if (pathParts === null && locationParts.length > 0) {
                        return false;
                    }

                    return _.isEqual(pathParts, _.map(_.filter(_.zip(pathParts, locationParts), function(x) { return _.isEqual(x[0], x[1]); }), function(x) { return x[0]; }));
                };

                var setActiveClass = function() {
                    if (isActive(attrs.isActive)) {
                        element.addClass('active');
                    }
                    else {
                        element.removeClass('active');
                    }
                };

                scope.$watch(function() {
                    return $location.path();
                }, setActiveClass);
            },
        };
    },
]);