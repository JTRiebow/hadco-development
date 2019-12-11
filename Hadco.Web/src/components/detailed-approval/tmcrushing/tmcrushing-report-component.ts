import * as angular from 'angular';

import * as template from './tmcrushing-report.html';

angular
	.module('tmcrushingModule')
	.component('htTmcrushingReport', {
		controller: tmcrushingReportController,
		controllerAs: 'vm',
		template,
	});
	
tmcrushingReportController.$inject  = [ '$scope', 'PermissionService' ];

function tmcrushingReportController($scope, PermissionService) {
	const vm = this;
	
	vm.can = PermissionService.can;
	
	$scope.day = {};

	$scope.day.production = [
		{ product: '', goal: '', produced: '', time: '' },
	];
	$scope.day.maintenance = [
		{ equipmentNumber: '', scheduledMaint: '', downtime: '', description: '' },
	];

	$scope.addProduction = function() {
		$scope.inserted = { product: '', goal: '', produced: '', time: '' };
		$scope.day.production.push($scope.inserted);
	};

	$scope.addMaintenance = function() {
		$scope.inserted = { equipmentNumber: '', scheduledMaint: '', downtime: '', description: '' };
		$scope.day.maintenance.push($scope.inserted);
	};

	$scope.removeProduction = function(index) {
		if ($scope.day.production.length === 1)
			$scope.day.production[0] = { product: '', goal: '', produced: '', time: '' };
		else
			$scope.day.production.splice(index, 1);
	};

	$scope.removeMaintenance = function(index) {
		if ($scope.day.maintenance.length === 1)
			$scope.day.maintenance[0] = { equipmentNumber: '', scheduledMaint: '', downtime: '', description: '' };
		else
			$scope.day.maintenance.splice(index, 1);
	};

	$scope.submitDay = function(day) {

	};
}