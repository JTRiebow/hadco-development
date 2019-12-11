import * as angular from 'angular';

angular
	.module('adminModule')
	.factory('Settings', settingsFactory);

settingsFactory.$inject = [ 'Restangular' ];

function settingsFactory(Restangular) {
	Restangular.configuration.routeToIdMappings['Settings'] = 'SettingsID';
	return Restangular.service('Settings');
}