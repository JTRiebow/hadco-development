import * as angular from 'angular';
import moment from 'moment';

import * as template from './trucking.html';
import * as pricingHistoryTemplate from '../Shared/modal-templates/view-pricing-history-modal.html';
import * as editPricingTemplate from '../Shared/modal-templates/edit-pricing-modal.html';

angular
	.module('truckingModule')
	.component('htPricing', {
		controller: pricingController,
		controllerAs: 'vm',
		template,
	});

pricingController.$inject = [
	'$scope',
	'$rootScope',
	'$location',
	'Pagination',
	'CurrentUser',
	'BillTypesHelper',
	'$modal',
	'LoadTimers',
	'Prices',
	'Pricings',
	'uiGridConstants',
	'sidebarMenuTemplate',
	'truckingGridsTemplate',
];

function pricingController(
	$scope,
	$rootScope,
	$location,
	Pagination,
	CurrentUser,
	BillTypesHelper,
	$modal,
	LoadTimers,
	Prices,
	Pricings,
	uiGridConstants,
	sidebarMenuTemplate,
	truckingGridsTemplate,
) {
	var vm = this;
	
	vm.sidebarMenuTemplate = sidebarMenuTemplate;
	vm.truckingGridsTemplate = truckingGridsTemplate;
	
	$scope.page = 'pricing';
	$scope.customerTypes = [
		{
			customerTypeID: 1,
			name: 'Development',
			key: 'Job',
		},
		{
			customerTypeID: 2,
			name: 'Residential',
			key: 'Customer',
		},
		{
			customerTypeID: 3,
			name: 'Outside',
			key: 'Customer',
		},
		{
			customerTypeID: 4,
			name: 'Metro',
			key: 'Run',
		},
	];
	
	$scope.customerType = $scope.customerTypes[0];

	vm.gridOptions = {
		cellEditableCondition: false,
		enableSorting: true,
		paginationPageSizes: [ 25, 50, 75 ],
		paginationPageSize: 25,
		enableFiltering: true,
		enableGridMenu: true,
		exporterCsvLinkElement: angular.element(document.querySelectorAll(".custom-csv-link-location")),
		enableColumnMenus: false,
		exporterMenuPdf: false,
		onRegisterApi: function(gridApi) {
			$scope.gridApi = gridApi;
		},
		
	};

	function createColumnDefs(data) {
		
		var firstRow = data[0];
		var columnsToShow = 5;
		var minColLength = 100;

		vm.gridOptions.columnDefs = [
			{ name: 'History', width: 175, enableFiltering: false, pinnedRight: true,
				cellTemplate: ' <a ng-click="grid.appScope.viewHistory(row.entity.pricingID)">View & Update</a>' },
		];
		angular.forEach(firstRow, function(value, key) {
			if (key == "pricingID" || value == "NA") {
			}
			else {
				if (value == null) {
					value = "";
				}

			var colWidth = function() {
				// console.log(value, key);
				if (key == "updatedTime") {
					return 200;
				}
				if (value.length > key.length) {
					return value.length*15 + 10;
				}
				else {
					return key.length*12 + 10;
				}
			};

			var columnDef = { field: key, minWidth: colWidth() } as any;
			if (key == "updatedTime") {
				columnDef.sort = { direction: uiGridConstants.DESC, priority: 1 };
				columnDef.cellFilter = "date: 'MM/dd/yyyy h:mma'";
				columnDef.type = "date";
			}

			vm.gridOptions.columnDefs.push(columnDef);
			
			}
		});
		
	}
		
	$scope.getPriceList = function() {
		
		Pricings.one('PriceGrid').get({ billTypeID: $scope.billType.billTypeID, customerTypeID: $scope.customerType.customerTypeID })
			.then(function(response) {
				vm.gridOptions.data = response;
				createColumnDefs(response);
				$scope.gridApi.grid.refresh();

			});
	};

	BillTypesHelper.getList()
		.then(function(response) {
			$scope.billTypes = response;
			$scope.billType = response[0];
			getCsvFilename($scope.customerType);
			$scope.getPriceList();
		
	});

	$scope.billTypeChanged = function(billType) {
		$scope.billType = billType;
		$scope.getPriceList();
	};

	$scope.customerTypeChange = function(customerType) {
		$scope.customerType = customerType;
		if (customerType.customerTypeID == 4) {
			$scope.billType = $scope.billTypes[1];
		}
		getCsvFilename(customerType);
		$scope.getPriceList();
	};

	function getCsvFilename(customerType) {
		if (customerType.customerTypeID == 1) {
			vm.gridOptions.exporterCsvFilename = 'Pricing-' + customerType.name + "-" + $scope.billType.name + '.csv';
		}
		else {
			vm.gridOptions.exporterCsvFilename = 'Pricing-' + $scope.customerType.name + '.csv';
		}
	}

	$scope.editPrice = function(pricingID, historical) {
		$scope.selectedPricingID = "";
		if (pricingID) {
			$scope.selectedPricingID = pricingID;
		}
		if (historical) {
			$scope.historicalPricing = true;
		}
		var modalInstance = $modal.open({
			template: editPricingTemplate,
			controller: 'EditPricingModalContentController',
			windowClass: 'default-modal',
			scope: $scope,
		});

		modalInstance.result.then(function(response) {
			$scope.getPriceList();
		});
	};

	$scope.viewHistory = function(pricingID) {
	// console.log(pricingID);
	
		Pricings.one(pricingID).get()
		.then(function(response) {
			$scope.pricingHistoryToView = response;
			// console.log($scope.pricingHistoryToView)
		
			var modalInstance = $modal.open({
				template: pricingHistoryTemplate,
				controller: 'ViewPricingHistoryModalController',
				windowClass: 'history-modal',
					scope: $scope,
			});

			modalInstance.result.then(function(response) {
				console.log(response, historical);
				//vm.gridOptions.data[index] = response;
				//$location.path(path);
				if (response !== "cancel") {
					var historical;
					console.log(response);
					if (response !== $scope.pricingHistoryToView.pricingID) {
						historical = true;
					}
					$scope.editPrice(response, historical);
				}
			});
		});
	};
}