import * as angular from 'angular';

import * as template from './create-or-edit-job.html';

angular
	.module('adminModule')
	.component('htCreateOrEditJob', {
		controller: createOrEditJobController,
		controllerAs: 'vm',
		template,
	});

createOrEditJobController.$inject = [
	'$scope',
	'$location',
	'NotificationFactory',
	'JobsHelper',
	'PermissionService',
];

function createOrEditJobController(
	$scope,
	$location,
	NotificationFactory,
	JobsHelper,
	PermissionService
) {

	init();

	function init() {
		// at the current time, you are not able to add or edit jobs in the system
		// the jobs come from ComputerEase instead
		PermissionService.redirectIfUnauthorized([ ]);
	}

	$scope.saveJob = function(job) {
		//save functionality goes here.
		NotificationFactory.success('Success: Job Created');
		JobsHelper.clearCache();
		$location.path('/admin/jobs');
	};

	$scope.cancel = function() {
		$location.path('/admin/jobs');
	};
}