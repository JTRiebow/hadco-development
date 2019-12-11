import * as angular from 'angular';
import * as moment from 'moment';

angular.module('truckingModule').controller('EditPricingModalContentController', [ '$scope', '$modalInstance', 'Prices', 'Pricings', 'TruckClassificationsHelper', 'MaterialsHelper', 'PricingList', 'NotificationFactory',
    function($scope, $modalInstance, Prices, Pricings, TruckClassificationsHelper, MaterialsHelper, PricingList, NotificationFactory) {

        $scope.now = moment();
        $scope.earliestDate = moment().set({ 'year': 2000, 'month': 0, 'date': 1 });
        $scope.listOfRunsOrJobsOrCustomers = [];
        $scope.isValidated = function() {
            var validated = false;
            if (!$scope.pricing.startDate) {
                validated = true;
            }
            if ($scope.historicalPricing) {
                if (!$scope.pricing.endDate) {
                    validated = true;
                }
            }
            return validated;
        };
        function getPricing(selectedPricingID, customerType) {
            if (selectedPricingID) {
                Pricings.one(JSON.stringify(selectedPricingID)).get()
                .then(function(response) {
                    $scope.pricing = response;
                    $scope.gridOptions.data = $scope.pricing.prices;
                    getList(response.billTypeID);
                });
            }
            else if (customerType.customerTypeID == 4) {
                $scope.selectedBillTypeID = 2;
                $scope.selectBillType($scope.selectedBillTypeID);
                getList($scope.selectedBillTypeID);
            }
        }

        $scope.selectBillType = function(selectedBillTypeID) {
            $scope.pricing = {};
            $scope.pricing = {
                billTypeID: selectedBillTypeID,
                customerTypeID: $scope.customerType.customerTypeID,
                startDate: $scope.now.format('YYYY-MM-DD'),
                prices: [],
            };
            PricingList.one()
                .get({ 'customerTypeID': $scope.customerType.customerTypeID, 'billTypeID': selectedBillTypeID })
                .then(function(response) {
                  response.forEach(function(item) {
                    if (!item.pricingID) {
                      $scope.listOfRunsOrJobsOrCustomers.push(item);
                    }
                  });
                    getList(selectedBillTypeID);
                });
        };

        function getList(billTypeID) {
            if (billTypeID == 1) {
                TruckClassificationsHelper.getList()
                    .then(function(response) {
                        $scope.list = {};
                        $scope.list = response;
                        filterList($scope.list);
                        $scope.gridOptions.columnDefs[0].visible = true;
                        $scope.gridOptions.columnDefs[1].visible = false;
                        $scope.gridApi.grid.refresh();
                    });
            }
            else {
                MaterialsHelper.getList()
                    .then(function(response) {
                        $scope.list = {};
                        $scope.list = response;
                        filterList($scope.list);
                        $scope.gridOptions.columnDefs[0].visible = false;
                        $scope.gridOptions.columnDefs[1].visible = true;
                        $scope.gridApi.grid.refresh();
                    });
            }
        }

        $scope.gridOptions = {
            enableFiltering: true,
            endableCellEditOnFocus: true,
            data: [],
            columnDefs: [
              { name: 'Truck', field: 'truck', enableColumnMenu: false, enableCellEdit: false, enableFiltering: true },
              { name: 'Material', field: 'material', enableColumnMenu: false, enableCellEdit: false, enableFiltering: true },
              { name: 'Price', field: 'value', enableColumnMenu: false, enableCellEdit: true, enableFiltering: true },
            ],
            onRegisterApi: function(gridApi) {
                $scope.gridApi = gridApi;
            },
        };

        getPricing($scope.selectedPricingID, $scope.customerType);

        function filterList(list) {
            var listedIDs = [];
            var newTruckOrMaterialList = [];
            $scope.gridOptions.data = $scope.pricing.prices;
            for (var i = $scope.pricing.prices.length - 1; i >= 0; i--) {
                var ID = $scope.pricing.prices[i].materialID ? $scope.pricing.prices[i].materialID : $scope.pricing.prices[i].truckClassificationID;

                listedIDs.push(ID);
            }

            $scope.list.forEach(function(item) {
                if (item.materialID) {
                    item.ID = item.materialID;
                    item.material = item.name;
                }
                else {
                    item.ID = item.truckClassificationID;
                    item.truck = item.name;
                }

                if (listedIDs.indexOf(item.ID) === -1) {
                    $scope.gridOptions.data.push(item);
                    listedIDs.push(item.ID);
                }
            });
            return newTruckOrMaterialList;
        }

        $scope.save = function(selectedKey, canOverlap) {
            $scope.viewValidateOverlappingPricing = false;
            var pricingObject = {
                customerTypeID: $scope.pricing.customerTypeID,
                billTypeID: $scope.pricing.billTypeID,
                startDate: $scope.pricing.startDate,
                endDate: $scope.pricing.endDate,
                jobID: $scope.pricing.jobID,
                phaseID: $scope.pricing.phaseID,
                customerID: $scope.pricing.customerID,
            } as any;
            if (!$scope.pricing.pricingID) {
                if ($scope.customerType.name == "Development") {
                    pricingObject.jobID = selectedKey.jobID;
                }
                else if ($scope.customerType.name == "Outside" || $scope.customerType.name == "Metro") {
                    pricingObject.phaseID = selectedKey.phaseID;
                }
                else if ($scope.customerType.name == "Residential") {
                    pricingObject.customerID = selectedKey.customerID;
                }
            }
            if (canOverlap) {
                pricingObject.canOverlap = true;
            }

            Pricings.post(pricingObject)
            .then(function(response) {
                 $scope.viewValidateOverlappingPricing = false;
                savePrices(response.pricingID);

            }, function(error) {
                //specify type of error
                $scope.viewValidateOverlappingPricing = true;
            });
        };

        function savePrices(pricingID) {
            for (var i = $scope.gridOptions.data.length - 1; i >= 0; i--) {
                if ($scope.gridOptions.data[i].value) {
                  $scope.gridOptions.data[i].pricingID = pricingID;
                  Prices.post($scope.gridOptions.data[i])
                  .then(function(response) {
                    $modalInstance.close('saved');
                  }, function(error) {
                    $scope.viewValidateOverlappingPricing = true;
                    NotificationFactory.error("Error: This Pricing was not successfully updated.");
                  });
                }
            }
        }

        $scope.cancel = function() {
            $modalInstance.dismiss('cancel');
        };


    } ]);