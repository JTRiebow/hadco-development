import * as angular from 'angular';

angular
    .module('detailedApprovalModule')
    .controller('downtimeTimerController', downtimeTimerController);

downtimeTimerController.$inject = [ '$scope', 'DowntimeReasons', 'DowntimeTimers' ];

function downtimeTimerController($scope, DowntimeReasons, DowntimeTimers) {

    //Downtime reasons for trucking 
    DowntimeReasons
        .getList()
        .then(function(response) {
            $scope.downtimeReasons = response;
        });

    $scope.deleteDowntime = function(index, downtime) {
        DowntimeTimers
            .one(downtime.downtimeTimerID)
            .remove();
        // console.log("this will delete the down time");
        if (index > -1) $scope.timesheet.downtimeTimers.splice(index, 1);
    };
    $scope.saveDowntime = function(index, downtime) {
        // console.log(downtime.downtimeReason);
        downtime.editDowntime = false;
        downtime.downtimeReasonID = downtime.downtimeReason.downtimeReasonID;
        downtime.downtimeReason = downtime.downtimeReason;
        downtime.timesheetID = $scope.timesheet.timesheetID;
        if (downtime.downtimeTimerID) {
            DowntimeTimers
            .one(downtime.downtimeTimerID)
            .patch(downtime);
        }
        else {
            DowntimeTimers.post(downtime)
            // 	.then(function (data) {
            // 		Users.one($routeParams.employeeId)
            // .one('Timesheets', $routeParams.dayId)
            // .get({departmentID: $routeParams.departmentId})

                .then(function(data) {
                    downtime.downtimeTimerID = data.downtimeTimerID;
                    // console.log(data.downtimeTimerID);
                    return data;

                });
        }
    };

    $scope.addDowntime = function() {
        if (!$scope.timesheet.downtimeTimers) {
            $scope.timesheet.downtimeTimers = [];
        }
        $scope.timesheet.downtimeTimers.push({ startTime: $scope.earliestClockIn, stopTime: $scope.lastClockOut });
    };

    $scope.viewEditDowntimeFields = function(downtime, index) {
        downtime.editDowntime = true;
        var storedDowntime = "downtime" + index;
        sessionStorage.setItem(storedDowntime, JSON.stringify(downtime));
        // console.log(storedDowntime, downtime);
    };

    $scope.clearDowntime = function(downtime, index) {

        // downtime.editDowntime = false
        if (downtime.downtimeTimerID == null) {
            $scope.timesheet.downtimeTimers.splice(index, 1);
        }
        else {
            var storedDowntime = "downtime" + index;
            $scope.timesheet.downtimeTimers[index] = JSON.parse(sessionStorage.getItem(storedDowntime));
            $scope.timesheet.downtimeTimers[index].editDowntime = false;
        }
        downtime.editDowntime = false;
    };

    $scope.canEditDowntime = function(id) {
        if (id <= 3) {
            return true;
        }
        return false;
    };

    $scope.displayLabel = function(code, description) {
        return code + " - " + description;
    };

    //end downtimeReasons for trucking

}