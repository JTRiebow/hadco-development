import * as angular from 'angular';

angular
    .module('downloadTimersModule')
    .factory('CSV', csvFactory);

csvFactory.$inject = [ 'Restangular' ];

function csvFactory(Restangular) {
    Restangular.configuration.routeToIdMappings['CSV'] = 'CSVID';
    return Restangular.service('CSV');
}