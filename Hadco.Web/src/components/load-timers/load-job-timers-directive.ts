import * as angular from 'angular';

import * as template from '../detailed-approval/detailed-load-job-timer-template.html';

angular.module('loadTimersModule').directive('loadJobTimersDirective', loadJobTimersDirective);

loadJobTimersDirective.$inject = [ 'DetailedConcreteService' ];

function loadJobTimersDirective(DetailedConcreteService) {
	return {
		restrict: 'A, E',
		scope: {
			timersArray: "=",
			columns: "=",
			department: "=",
			timerType: '@',
		},
		template,
	};
}