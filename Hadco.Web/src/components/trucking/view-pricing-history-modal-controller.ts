import * as angular from 'angular';
import * as moment from 'moment';

angular.module('truckingModule').controller('ViewPricingHistoryModalController', [ '$scope', '$modalInstance', 'Prices', '$modal', 'uiGridConstants',

function($scope, $modalInstance, Prices, $modal, uiGridConstants) {
        $scope.jobCustRun = {};
        $scope.gridOptions = {
            enableSorting: true,
            enableHorizontalScrollbar: true,
            showColumnFooter: true,
            columnFooterHeight: 30,
            enableGridMenu: true,
        };
    $scope.close = function(response) {
        $modalInstance.close(response);
    };
    
    $scope.getFooterValue = function(key) {
        // console.log(key);
        // console.log($scope.pricingHistoryToView[key]);
        if (key == 'updatedTime') {
            return moment($scope.pricingHistoryToView[key]).format('YYYY/MM/DD h:mmA');
        }
            if (key == 'startDate' || key == 'endDate') {
            return $scope.pricingHistoryToView[key];
        }
        for (var i = $scope.pricingHistoryToView.prices.length - 1; i >= 0; i--) {
            if ($scope.pricingHistoryToView.prices[i].truck == (key.charAt(0).toUpperCase() + key.slice(1))) {
                return $scope.pricingHistoryToView.prices[i].value;
            }
        }
    };

    function createColumnDefs(data) {
        var firstRow = data[0];
        var columnsToShow = 5;
        var minColLength = 100;

        $scope.gridOptions.columnDefs = [
            
            { name: 'Update', width: 200, pinnedRight: true, cellTemplate: ' <a ng-click="grid.appScope.close(row.entity.pricingID)">Update Historical Pricing</a>', footerCellTemplate: '<div class="ui-grid-cell-contents warning" style="cursor: pointer" ng-click="grid.appScope.close(grid.appScope.pricingHistoryToView.pricingID)">Update Current Pricing</div>' },
        ];

        angular.forEach(firstRow, function(value, key) {
            if (key == "pricingID" || key == "name" || value == "NA") {
            }
            else {
                if (value == null) {
                    value = "";
                }

                var colWidth = function() {
                    // console.log(value, key);
                    if (key == "startDate" || key == "endDate") {
                        return 110;
                    }
                    if (key == "updatedTime") {
                        return 190;
                    }
                    if (value.length > key.length) {
                        return value.length*15 + 10;
                    }
                    else {
                        return key.length*12 + 10;
                    }
                };
                var columnDef = { field: key, minWidth: colWidth(), footerCellTemplate: '<div class="ui-grid-cell-contents">{{grid.appScope.getFooterValue(col.field)}}</div>' } as any;

                if (key == "startDate" || key == "endDate") {
                    columnDef.pinnedLeft = true;
                }
                if (key == 'updatedTime') {
                    columnDef.sort = { direction: uiGridConstants.DESC, priority: 1 };
                    columnDef.cellFilter = "date: 'yyyy/MM/dd h:mma'";
                    columnDef.type = "date";
                }
                $scope.gridOptions.columnDefs.push(columnDef);
                // $scope.currentGridOptions.columnDefs.push(columnDef);     
            }
        });
    }

    // function getBackgroundColor(grid, row, col, rowIndex, colIndex) {
    //     if (row.entity.pricingID == $scope.pricingHistoryToView.pricingID) {
    //         return 'warning';
    //     }
    // }
    // $scope.currentGridOptions = {
    //     //data: [$scope.pricingHistoryToView],
    //     columnDefs: $scope.gridOptions.columnDefs,

    // }
    Prices.one("PriceHistory").get({ customerTypeID: $scope.pricingHistoryToView.customerTypeID, billTypeID: $scope.pricingHistoryToView.billTypeID, jobID: $scope.pricingHistoryToView.jobID, phaseID: $scope.pricingHistoryToView.phaseID, customerID: $scope.pricingHistoryToView.customerID })
        .then(function(history) {
            for (var i = history.length - 1; i >= 0; i--) {
                if (history[i].pricingID == $scope.pricingHistoryToView.pricingID) {
                    history.splice(i, 1);
                    break;
                }
            }
            //$scope.history = history;
            $scope.gridOptions.data = history;

            // $scope.currentGridOptions.data = $scope.pricingHistoryToView.prices;
            createColumnDefs(history);
        });
} ]);