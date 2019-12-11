import * as angular from 'angular';

angular
    .module('configModule')
    .config(restangular);

restangular.$inject = [ 'RestangularProvider' ];

function restangular(RestangularProvider) {
    RestangularProvider.setBaseUrl('/api/');

    RestangularProvider.addResponseInterceptor(function(response, operation) {
        var newResponse;

        // Ensure casing and object format are correct
        if (operation == 'getList') {
            if (response.items != null) {
                newResponse = response.items;
                newResponse.meta = {
                    resultCount: response.resultCount,
                    totalResultCount: response.totalResultCount,
                };
            }
        }
        else {
            if (response != null) {
                newResponse = response;
            }
        }

        return newResponse;
    });
    
    var config = RestangularProvider.configuration;
    
    config.routeToIdMappings = {};

    config.getIdFromElem = function(elem) {
        var idFieldName = config.routeToIdMappings[elem.route];
        return elem[idFieldName];
    };

    config.setIdToElem = function(elem, value, route) {
        route = route || elem.route;
        var idFieldName = config.routeToIdMappings[route];
        elem[idFieldName] = value;
    };
}