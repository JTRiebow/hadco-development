import * as angular from 'angular';

import * as template from './employee-equipment.html';

angular
	.module('employeeModule')
	.component('htEmployeeEquipment', {
		controller: EmployeeEquipmentController,
		controllerAs: 'vm',
		template,
	});

EmployeeEquipmentController.$inject = [
	'$scope',
	'NotificationFactory',
	'$location',
	'EquipmentHelper',
	'EquipmentServiceTypesHelper',
];

function EmployeeEquipmentController(
	$scope,
	NotificationFactory,
	$location,
	EquipmentHelper,
	EquipmentServiceTypesHelper,
) {
	$scope.test = 'Hello are we working?';

	$scope.saveEquipment = function() {
		NotificationFactory.success('Saved!!');
		$location.path('/employee/clock-in');
	};

	$scope.inspector = {};

	$scope.typeaheadEquipment = function(search) {
		return EquipmentHelper.getList({
			search: search,
			take: 8,
		})
		.then(function(response) {
			return response;
		});
	};

	$scope.removeEquipment = function(index) {
		if ($scope.openEquipment.length === 1)
			$scope.openEquipment[0] = { equipment: '', rspd: '', hadcoShop: '', workPerformed: '', openClosed: '', workOrder: '' };
		else
			$scope.openEquipment.splice(index, 1);
	};

	$scope.saveAllEquipment = function(equip) {
		NotificationFactory.success('Equipment Saved!!');
	};

	$scope.openEquipment = [
		{
			equipmentNumber: undefined,
			rspd: '',
			hadcoShop: 'hi',
			workPerformed: '',
			openClosed: true,
			workOrder: '',
			notes: '',
		},
	];

	$scope.addEquipment = function() {
		$scope.inserted = {
			rspd: '',
			hadcoShop: '',
			workPerformed: '',
			openClosed: true,
			workOrder: '',
		};
		$scope.openEquipment.push($scope.inserted);
	};

	$scope.equipmentDrop = [
		{ name: 'LT 112' },
		{ name: 'VT 155' },
		{ name: 'JS 854' },
		{ name: 'PT 887' },
		{ name: 'KT 998' },
	];

	EquipmentServiceTypesHelper.getList()
	.then(function(data) {
		$scope.rspdDrop = data;
	});

	$scope.trueFalse = [
		{ name: 'Open', value: true },
		{ name: 'Closed', value: false },
	];
}