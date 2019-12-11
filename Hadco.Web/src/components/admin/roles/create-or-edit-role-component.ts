import * as angular from 'angular';

angular
	.module('roleModule')
	.controller('htCreateOrEditRole', createOrEditRoleController);

createOrEditRoleController.$inject = [
	'$scope',
	'$location',
	'Pagination',
	'NotificationFactory',
	'Roles',
	'$routeParams',
];

function createOrEditRoleController(
	$scope,
	$location,
	Pagination,
	NotificationFactory,
	Roles,
	$routeParams,
) {
	
	$scope.roleId = $routeParams.roleId;

	if ($scope.roleId === 'create') {

	}
	else {
		Roles.one($scope.roleId).get()
		.then(function(response) {
			$scope.role = response;
		});
	}

	$scope.saveRole = function(role) {
		if ($scope.roleId === 'create') {
			Roles.post(role)
			.then(function(response) {
				NotificationFactory.success('Success: Role Created');
				$location.path('/human-resources/roles');
			});
		}
	};

	$scope.cancel = function() {
		NotificationFactory.error('Create User Cancelled');
		$location.path('/human-resources/roles');
	};
}