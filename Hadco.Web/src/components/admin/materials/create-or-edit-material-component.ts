import * as angular from 'angular';

import * as template from './create-or-edit-material.html';

angular
	.module('adminModule')
	.component('htCreateOrEditMaterial', {
		controller: createOrEditMaterialController,
		controllerAs: 'vm',
		template,
	});

createOrEditMaterialController.$inject = [
	'$scope',
	'$location',
	'Pagination',
	'NotificationFactory',
	'MaterialsHelper',
	'$routeParams',
	'PermissionService',
];

function createOrEditMaterialController(
	$scope,
	$location,
	Pagination,
	NotificationFactory,
	MaterialsHelper,
	$routeParams,
	PermissionService
) {
	$scope.materialId = $routeParams.materialId;

	init();

	function init() {
		PermissionService.redirectIfUnauthorized([ 'editMaterial', 'deleteMaterial', 'addMaterial' ]);
	}

	if ($scope.materialId === 'create') {

	}
	else {
		MaterialsHelper.get($scope.materialId)
		.then(function(response) {
			$scope.material = response;
		});
	}

	$scope.saveMaterial = function(material) {
		if ($scope.materialId === 'create') {
			MaterialsHelper.post(material)
			.then(function(response) {
				NotificationFactory.success('Material Created');
				$location.path('/admin/materials');
			});
		}
		else {
			MaterialsHelper.patch(material)
			.then(function(response) {
				NotificationFactory.success('Material Changed');
				$location.path('/admin/materials');
			});
		}
	};

	$scope.cancel = function() {
		NotificationFactory.error('Create Material Cancelled');
		$location.path('/admin/materials');
	};
}