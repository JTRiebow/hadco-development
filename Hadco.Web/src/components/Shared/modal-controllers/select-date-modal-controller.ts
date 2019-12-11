import * as angular from 'angular';
import * as moment from 'moment';
import * as _ from 'lodash';

angular
    .module('modalModule')
    .controller('selectDateModalController', selectDateModalController);

selectDateModalController.$inject = [ '$scope', '$modalInstance', 'date', 'foreman' ];

function selectDateModalController($scope, $modalInstance, date, foreman) {

    $scope.foreman = foreman;

    var weekDateList = [];
    for (var i = 0 ; i <= 6; i++) {
        weekDateList.push(moment(date).add(i, 'days').format("MMM DD, YYYY"));
    }

    var timeSheetDates = [];
    if (foreman.timesheets) {
        for (var i = 0; i <= foreman.timesheets.length - 1; i++) {
            timeSheetDates.push(moment(foreman.timesheets[i].day, "YYYY-MM-DD").format("MMM DD, YYYY"));
        }
    }

    //find dates not already assigned timesheets
    $scope.dateList = _.difference(weekDateList, timeSheetDates);
    $scope.date = $scope.dateList[0];

    $scope.confirm = function(date) {
        date = moment(date).format("YYYY-MM-DD");
        $modalInstance.close(date);
    };

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };
}