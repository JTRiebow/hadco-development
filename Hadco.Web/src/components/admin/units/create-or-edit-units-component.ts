import * as angular from 'angular';

import * as template from './create-or-edit-units.html';

angular
	.module('adminModule')
	.component('htCreateOrEditUnit', {
		controller: createOrEditUnitController,
		controllerAs: 'vm',
		template,
	});

createOrEditUnitController.$inject = [
	'$scope',
	'$location',
	'Pagination',
	'NotificationFactory',
	'Units',
	'$routeParams',
];

function createOrEditUnitController(
	$scope,
	$location,
	Pagination,
	NotificationFactory,
	Units,
	$routeParams
) {
	$scope.unitId = $routeParams.unitId;

	if ($scope.unitId === 'create') {

	}
	else {
		Units.one($scope.unitId).get()
		.then(function(response) {
			$scope.unit = response;
		});
	}

	$scope.saveUnit = function(unit) {
		if ($scope.unitId === 'create') {
			Units.post(unit)
			.then(function(response) {
				NotificationFactory.success('Unit Created');
				$location.path('/supervisor/units');
			});
		}
		else {
			Units.one(unit.unitID).patch(unit)
			.then(function(response) {
				NotificationFactory.success('Unit Created');
				$location.path('/supervisor/units');
			});
			
		}
	};

	

	$scope.cancel = function() {
		NotificationFactory.error('Create User Cancelled');
		$location.path('/supervisor/units');
	};
}