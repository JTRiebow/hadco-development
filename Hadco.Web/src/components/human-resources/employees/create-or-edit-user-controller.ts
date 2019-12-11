import * as angular from 'angular';

import * as template from './create-or-edit-user.html';
import * as newPasswordTemplate from '../../Shared/modal-templates/new-password-modal.html';

angular
	.module('userModule')
	.component('htCreateOrEditUser', {
		controller: createOrEditUserController,
		controllerAs: 'vm',
		template,
	});

createOrEditUserController.$inject = [
	'$scope',
	'$location',
	'Pagination',
	'NotificationFactory',
	'DepartmentsHelper',
	'$routeParams',
	'Users',
	'Roles',
	'$modal',
	'Supervisors',
	'PermissionService',
];

function createOrEditUserController(
	$scope,
	$location,
	Pagination,
	NotificationFactory,
	DepartmentsHelper,
	$routeParams,
	Users,
	Roles,
	$modal,
	Supervisors,
	PermissionService,
) {
	const vm = this;
	
	vm.can = PermissionService.can;
	vm.canDoAny = PermissionService.canDoAny;

	$scope.employee = {};
	$scope.user = {};
	$scope.old = {};

	$scope.submitted = false;
	var employeeId = $routeParams.userId;

	init();

	function init() {
		if ($routeParams.userId == 'create') {
			return PermissionService.redirectIfUnauthorized('createEmployee');
		}
		PermissionService.getEmployeePermissions($routeParams.userId)
			.then(() => {
				PermissionService.redirectIfUnauthorized('viewCurrentEmployeeDetails');
			});
	}
	
	$scope.getUser = function() {
		return Users.one(employeeId).get()
		.then(function(response) {
			return $scope.employee = response;
		});
	};

	$scope.getSupervisors = function() {
		Supervisors.getList()
		.then(function(data) {
			$scope.supervisors = data;
		});
	};
	$scope.getDepartments = function() {
		DepartmentsHelper.getList()
		.then(function(data) {
			$scope.departments = data;
		});
	};
	$scope.getRoles = function() {
		Roles.getList()
		.then(function(data) {
			$scope.roles = data;
		});
	};

	$scope.getUser();

	$scope.getSupervisors();

	$scope.getDepartments();

	$scope.getRoles();

	$scope.removeItem = function(item) {
	};

	$scope.returnToPreviousPage = function() {
		if (sessionStorage.getItem("cachedReturnToPage") === "employee-search") {
			$location.path(sessionStorage.getItem("cachedEmployeeSearchUrl"));
		}
		else {
			$location.path('/human-resources/employees');
		}
	};

	$scope.newPassword = function(key) {
		var modalInstance = $modal.open({
			template: newPasswordTemplate,
			controller: 'NewPasswordController',
			resolve: {
				key: function() {
					return key;
				},
			},
		});

		modalInstance.result.then(function(employee) {
			if (employee.password1 === employee.password2 && employee.password1 && key === 'Username') {
				Users.one($scope.employee.employeeID).patch({ username: employee.password1 })
				.then(function(data) {
					$scope.getUser();
					NotificationFactory.success('Success: Username Changed');
				}, function(data) {
					NotificationFactory.error("Error");
				});
			}

			if (employee.password1 === employee.password2 && employee.password1 && key === 'Pin') {
				Users.one($scope.employee.employeeID).patch({ pin: employee.password1 })
				.then(function(data) {
					$scope.getUser();
					NotificationFactory.success('Success: Pin Changed');
				}, function(data) {
					NotificationFactory.error("Error");
				});
			}

			if (employee.password1 === employee.password2 && employee.password1 && key === 'Password') {
				Users.one($scope.employee.employeeID).patch({ password: employee.password1 })
				.then(function(data) {
					$scope.getUser();
					NotificationFactory.success('Success: Password Changed');
				}, function(data) {
					NotificationFactory.error("Error: Password requires 8 characters.");
				});
			}

				if (employee.name && key === 'Employee Name') {
				Users.one($scope.employee.employeeID).patch({ name: employee.name })
				.then(function(data) {
					$scope.getUser();
					NotificationFactory.success('Success: Employee Name Changed');
				}, function(data) {
					NotificationFactory.error("Error: Employee Name not saved.");
				});
			}

		});
	};

	$scope.$watch('employee.supervisors', function(newValue, oldValue) {
		if (!oldValue) return;
		if (newValue.length > oldValue.length) {
			// Users.one($scope.employee.employeeID, 'Roles').post({roleID: newValue[newValue.length - 1].roleId})
			var url = $scope.employee.employeeID + '/Supervisors?supervisorID=' + newValue[newValue.length - 1].employeeID;
			Users.one().post(url)
			.then(function(data) {
				NotificationFactory.success('Supervisor added');
			}, function(data) {
				NotificationFactory.error("Couldn't add supervisor");
			});
		}
	});

	$scope.$watch('employee.roles', function(newValue, oldValue) {
		if (!oldValue) return;
		if (newValue.length > oldValue.length) {
			// Users.one($scope.employee.employeeID, 'Roles').post({roleID: newValue[newValue.length - 1].roleId})
			var url = $scope.employee.employeeID + '/Roles?roleID=' + newValue[newValue.length - 1].roleId;
			Users.one().post(url)
			.then(function(data) {
				NotificationFactory.success('Role added');
			}, function(data) {
				NotificationFactory.error("Couldn't add role");
			});
		}
	});

	$scope.$watch('employee.departments', function(newValue, oldValue) {
		if (!oldValue) return;
		if (newValue.length > oldValue.length) {
			// Users.one($scope.employee.employeeID, 'Roles').post({roleID: newValue[newValue.length - 1].roleId})
			var url = $scope.employee.employeeID + '/Departments?departmentID=' + newValue[newValue.length - 1].departmentID;
			Users.one().post(url)
			.then(function(data) {
				NotificationFactory.success('Department added');
			}, function(data) {
				NotificationFactory.error("Couldn't add department");
			});
		}
	});

	$scope.saveNewUser = function(user, form) {
		$scope.submitted = true;
		if (form.$invalid) return;

		Users
		.post(user)
		.then(function(data) {
			NotificationFactory.success('Success: Employee created');
			$scope.employee = data;
			$location.path('/human-resources/employee/' + data.employeeID);
		}, function(error) {
			NotificationFactory.error('Error: Employee not created. Reason:' + error.data.exceptionMessage);
		});
	};
}
							
angular.module('employeeModule').factory('deleteRole', [ 'Roles', 'Users', '$routeParams', 'NotificationFactory',
	function(Roles, Users, $routeParams, NotificationFactory) {
		return function(removedValue, lastQuery, getLabel) {
			Users.one($routeParams.userId).get()
			.then(function(data) {
				Users.one(data.employeeID).one('Roles', removedValue.roleId).remove()
				.then(function(data) {
					NotificationFactory.success('Role removed');
				}, function(data) {
					NotificationFactory.error("Couldn't remove role");
				});
				
			});
		};
	} ]);

angular.module('employeeModule').factory('deleteDepartment', [ 'Users', '$routeParams', 'NotificationFactory',
	function(Users, $routeParams, NotificationFactory) {
		return function(removedValue, lastQuery, getLabel) {
			Users.one($routeParams.userId).get()
			.then(function(data) {
				Users.one(data.employeeID).one('Departments', removedValue.departmentID).remove()
				.then(function(data) {
					NotificationFactory.success('Department removed');
				}, function(data) {
					NotificationFactory.error("Couldn't remove role");
				});
				
			});
		};
	} ]);

angular.module('employeeModule').factory('deleteSupervisors', [ 'Users', '$routeParams', 'NotificationFactory',
	function(Users, $routeParams, NotificationFactory) {
		return function(removedValue, lastQuery, getLabel) {
			Users.one($routeParams.userId).get()
			.then(function(data) {
				Users.one(data.employeeID).one('Supervisors', removedValue.employeeID).remove()
				.then(function(data) {
					NotificationFactory.success('Supervisor removed');
				}, function(data) {
					NotificationFactory.error("Couldn't remove role");
				});
				
			});
		};
	} ]);
// var path = $scope.employeeId + 'Roles/' newValue[j].roleId;

// Users.one().getList(employeeId + '/Roles')
// .then(function(response) {
// 	$scope.employee = response;
// })