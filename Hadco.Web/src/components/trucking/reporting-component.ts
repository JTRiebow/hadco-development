import * as angular from 'angular';
import moment from 'moment';

import * as template from './trucking.html';
import * as pricingHistoryTemplate from '../Shared/modal-templates/view-pricing-history-modal.html';
import * as editPricingTemplate from '../Shared/modal-templates/edit-pricing-modal.html';

angular
	.module('truckingModule')
	.component('htReporting', {
		controller: reportingController,
		controllerAs: 'vm',
		template,
	});

reportingController.$inject = [
	'$scope',
	'$rootScope',
	'$location',
	'Pagination',
	'CurrentUser',
	'BillTypesHelper',
	'$modal',
	'EquipmentHelper',
	'LoadTimers',
	'Prices',
	'Pricings',
	'TruckingService',
	'sidebarMenuTemplate',
	'truckingGridsTemplate',
];

function reportingController(
	$scope,
	$rootScope,
	$location,
	Pagination,
	CurrentUser,
	BillTypesHelper,
	$modal,
	EquipmentHelper,
	LoadTimers,
	Prices,
	Pricings,
	TruckingService,
	sidebarMenuTemplate,
	truckingGridsTemplate,
) {
	$scope.vm = $scope;
	
	
	
	$scope.sidebarMenuTemplate = sidebarMenuTemplate;
	$scope.truckingGridsTemplate = truckingGridsTemplate;
	
	$scope.gridOptions = {};
	$scope.page = 'reporting';
	
	$scope.reportingTypes = [
		{
			reportingTypeID: 1,
			name: 'Proj. vs Actual',
		},
		{
			reportingTypeID: 2,
			name: 'Revenue by Customer',
		},
		{
			reportingTypeID: 3,
			name: 'Revenue by Dept.',
		},
		{
			reportingTypeID: 4,
			name: 'Revenue by Truck',
		},
		{
			reportingTypeID: 5,
			name: 'Quantity',
		},
	];
	$scope.customerType = $scope.reportingTypes[0];

	$scope.customerTypeChange = function(pageTitle) {
		$scope.customerType = pageTitle;
	};

	$scope.pageToggle = function(customerType) {
		$scope.customerType = customerType;
	};

	BillTypesHelper.getList()
	.then(function(response) {
		$scope.billTypes = response;
		$scope.billType = response[0];
	});

	$scope.ranges = TruckingService.getRanges();
	$scope.range = $scope.ranges[4];

	$scope.displays = TruckingService.getDisplays();
	$scope.display = $scope.displays[2];

	//there's no endpoint for customers yet
	$scope.customers = [
		{ name: "customer1" },
		{ name: "customer2" },
		{ name: "customer3" },
	];
	
	EquipmentHelper.getList('Trucks')
	.then(function(response) {
		$scope.trucks = response;
	});
		
	

	$scope.saveCell = function( cellEntity, colDef, newValue, OldValue) {
			var editedCell;
			if (colDef.field == "note") {
				editedCell = { note: newValue };
			}
			if (colDef.field == "invoiceNumber") {
				editedCell = { invoiceNumber: newValue };
			}
			if (newValue !== OldValue) {
				LoadTimers.one(cellEntity.loadTimerID).patch(editedCell)
				.then(function(result) {
					console.log(result, cellEntity, colDef, newValue, OldValue);
					LoadTimers.one($scope.range.start).getList($scope.range.end)
			.then(function(response) {
				
				$scope.gridOptions.data = response;
					
			});
				});
			}
	};



	
	$scope.gridOptions = {
		endableCellEditOnFocus: true,
		enableSorting: true,
		enableHorizontalScrollbar: true,
		enableGridMenu: true,
		exporterCsvFilename: 'Trucker Dailies.csv',
		exporterCsvLinkElement: angular.element(document.querySelectorAll(".custom-csv-link-location")),
		enableColumnMenus: false,
		exporterMenuPdf: false,
		// columnDefs: [
	//      { name:'Date', field: 'date', width: 100, type: 'date'},
	//      { name:'Name', field: 'name',  width: 100, type: 'string'},
	//      { name:'Truck', field: 'truck',  width: 100, type: 'string'},
//         { name:'Trailer', field: 'trailer',  width: 100, type: 'string'},
	//      { name:'Pup', field: 'pupTrailer',  width: 100, type: 'string'},
	//      { name:'Ticket #', field: 'ticketNumber',  width: 100, type: 'number'},
	//      { name:'Tons', field: 'tons',  width: 100, type: 'number'},
	//      { name:'Load Site', field: 'loadSite',  width: 200, type: 'string'},
	//      { name:'Material', field: 'material',  width: 100, type: 'string'},  
	//      { name:'Job', field: 'job',  width: 100, type: 'string'},  
	//      { name:'Phase', field: 'phase',  width: 100, type: 'string'},  
	//      { name:'Category', field: 'category',  width: 100, type: 'string'},  
	//      // { name:'Hours', field: 'hours',  width: 100},  
	//      // { name:'Load', field: 'loadTimerID',  width: 100},  
	//      { name:'Total Hours', field: 'totalHours',  width: 150, type: 'number'},  
	//      // { name:'TMCPO', field: 'tmcpo',  width: 100},  
	//      // { name:'Adjusted Total Hours', field: 'adjustedTotalHours',  width: 200},  
	//      { name:'Invoice #', field: 'invoiceNumber',  enableCellEdit: true, width: 100, type: 'string'},
	//      { name:'Note', field: 'note',  enableCellEdit: true, width: 200, type: 'string'},        	          	          
	//      { name:'Calculated Total Revenue', field: 'calculatedTotalRevenue',  width: 200, type: 'number'}

		// ],

		data: [
			{ jobs: 'Report1', dumpTruck: 12, sideDump: 13, bellyDump: 41, pupTrailer: 51, doubleSidedDump: 17, doubleBellyDump: 41, endDump: 61, slinger: 71, waterTruck: 19, date: "12/04/2016" },
			{ jobs: 'Report2', dumpTruck: 7, sideDump: 18, bellyDump: 91, pupTrailer: 30, doubleSidedDump: 10, doubleBellyDump: 81, endDump: 17, slinger: 18, waterTruck: 91, date: "12/05/2016" },
			{ jobs: 'Report3', dumpTruck: 17, sideDump: 11, bellyDump: 19, pupTrailer: 10, doubleSidedDump: 11, doubleBellyDump: 18, endDump: 11, slinger: 11, waterTruck: 19, date: "12/06/2016" },
			{ jobs: 'Report3', dumpTruck: 17, sideDump: 11, bellyDump: 19, pupTrailer: 10, doubleSidedDump: 11, doubleBellyDump: 18, endDump: 11, slinger: 11, waterTruck: 19, date: "12/07/2016" },
		],

	};
	$scope.billType = "All";
	$scope.billTypeChanged = function(billType) {
		$scope.billType = billType;
	};
	$scope.myFilter = function(item) {
		if ($scope.billType) {
			if ($scope.billType !== "All")
				return item.billType === $scope.billType;
			else
				return true;
		}
		return true;
	};
	$scope.filter = true;

	$scope.rangeChange = function(range) {
		range.start = moment(range.start).format('YYYY-MM-DD');
		range.end = moment(range.end).format('YYYY-MM-DD');
		console.log(range.start, range.end);
		LoadTimers.one(range.start ||$scope.range.start).getList(range.end || $scope.range.end)
		.then(function(response) {
			$rootScope.loadTimers = response;
			$scope.gridOptions.data = response;
		});
	};
	$scope.truckerFilters = function(item) {
		if ($scope.range) {
			$scope.filter = item.timesheet.day > $scope.rangeStart && item.timesheet.day < $scope.rangeEnd;
		}
	};

	$scope.editPrice = function() {
		var modalInstance = $modal.open({
			template: editPricingTemplate,
			controller: 'EditPricingModalContentController',
			windowClass: 'default-modal',
		});

		modalInstance.result.then(function(e) {
			var path = '/supervisor/employee/' + e.employeeID + '/department/' + e.departments[0].departmentID + '/day/create';
			$location.path(path);
		});
	};

	$scope.viewHistory = function() {
		var modalInstance = $modal.open({
			template: pricingHistoryTemplate,
			controller: 'ViewHistoryModalContentController',
			windowClass: 'history-modal',
		});

		modalInstance.result.then(function(e) {
			var path = '/supervisor/employee/' + e.employeeID + '/department/' + e.departments[0].departmentID + '/day/create';
			$location.path(path);
		});
	};
}
