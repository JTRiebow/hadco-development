import * as angular from 'angular';

angular
	.module('adminModule')
	.factory('DowntimeReasons', DowntimeReasons);

DowntimeReasons.$inject = [ 'Restangular' ];

function DowntimeReasons(Restangular) {
	Restangular.configuration.routeToIdMappings['DowntimeReasons'] = 'downtimeReasonID';
	return Restangular.service('DowntimeReasons');
}