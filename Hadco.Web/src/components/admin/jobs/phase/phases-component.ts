import * as angular from 'angular';

import * as template from './phases.html';

angular
	.module('adminModule')
	.component('htPhases', {
		controller: phasesController,
		controllerAs: 'vm',
		template,
	});

phasesController.$inject = [ '$scope', 'Jobs', '$routeParams' ];

function phasesController($scope, Jobs, $routeParams) {

	$scope.params = $routeParams.jobId;
	$scope.phases = [];

	Jobs.one($routeParams.jobId)
		.getList('Phases')
		.then(function(response) {
			$scope.phases = response;
		});
}