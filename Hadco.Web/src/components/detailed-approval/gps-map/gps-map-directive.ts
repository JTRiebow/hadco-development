import * as angular from 'angular';
import * as moment from 'moment';
import * as _ from 'lodash';

import * as template from './gps-map.html';

angular
    .module('detailedApprovalModule')
    .directive('gpsMap', gpsMap);

function gpsMap() {
    return {
        template,
        controller: gpsMapController,
        controllerAs: 'ctrl',
    };
}
    
gpsMapController.$inject = [ '$scope', 'Locations', '$q', '$routeParams' ];

function gpsMapController($scope, Locations, $q, $routeParams) {

    var ctrl = this;

    init();

    function init() {
        $scope.$watch("vm.visibleEntries", function(visibleEntries) {
            if (visibleEntries && visibleEntries.length) {
                ctrl.visibleEntries = visibleEntries;
                getTimeDropDownList(ctrl.visibleEntries);
                getMap();
            }
        });

        $scope.$watch("vm.selectedPushpin", function(pushpin) {
            if (pushpin) {
                ctrl.selectPushpin(pushpin);
            }
        });
        ctrl.zoomLevel = 13;
        ctrl.hideMap = true;
    }

    ctrl.changeZoom = function() {
        ctrl.useZoom = true;
        var zoomDefaults = { min: 9, max: 13 };
        if (ctrl.selectedPushpin) {
            var zoomDefaults = { min: 12, max: 16 };
        }

        if (ctrl.zoomLevel <= zoomDefaults.min) {
            ctrl.zoomLevel = zoomDefaults.max;
        }
        else {
            ctrl.zoomLevel = ctrl.zoomLevel - 1;
        }

        getMap(ctrl.zoomLevel, ctrl.timeframe);
    };

    ctrl.selectPushpin = function(selectedPushpin) {
        var type = selectedPushpin.type == "clockIn" ? 79 : 80;
        ctrl.zoomLevel = 16;
        ctrl.selectedPushpin = 'pushpin=' + selectedPushpin.latitude + ',' + selectedPushpin.longitude + ';' + type + ';&';
        getMap(ctrl.zoomLevel, ctrl.timeframe, ctrl.selectedPushpin);
    };

    ctrl.reset = function() {
        ctrl.timeframe.startTime = _.first<any>(ctrl.dropDownTimes).date;
        ctrl.timeframe.endTime = _.last<any>(ctrl.dropDownTimes).date;
        ctrl.useZoom = false;
        ctrl.zoomLevel = 13;
        ctrl.selectedPushpin = false;
        getMap();
    };

    ctrl.timeChanged = function() {
        ctrl.zoomLevel = 13;
        getMap(ctrl.zoomLevel, ctrl.timeframe);
    };

    function getTimeDropDownList(visibleEntries) {
        getTimeframe(visibleEntries);
        var timeList = [];
        var firstTime = getFirstTime(ctrl.timeframe.earliestClockIn);

        if (ctrl.timeframe.lastClockOut > ctrl.timeframe.earliestClockIn) {
            var lastTime = getLastTime(ctrl.timeframe.lastClockOut);

            for (var time = firstTime;
                time.isBefore(lastTime);
                time.add(30, 'minutes')) {
                timeList.push({ date: time.format(), time: time.format("hh:mm A") });
            }
        }
        else {
            timeList.push({ date: firstTime.format(), time: firstTime.format("hh:mm A") });
        }

        ctrl.dropDownTimes = timeList;
        ctrl.timeframe.startTime = _.first<any>(ctrl.dropDownTimes).date;
        ctrl.timeframe.endTime = _.last<any>(ctrl.dropDownTimes).date;
        
    }

    function getFirstTime(time) {
        var remainder = moment(time).minute() % 30;
        return moment(time).subtract(remainder, "minutes");
    }

    function getLastTime(time) {
        var remainder = 60 - moment(time).minute() % 30;
        return moment(time).add(remainder, "minutes");
    }

    function getTimeframe(visibleEntries) {
        ctrl.timeframe = {
            startTime: ctrl.visibleEntries[0].clockIn,
            endTime: ctrl.visibleEntries[0].clockOut,
        };
        _.each(visibleEntries, function(timerEntry) {
            ctrl.timeframe.earliestClockIn = isEarliestClockIn(timerEntry.clockIn);
            ctrl.timeframe.lastClockOut = isLastClockOut(timerEntry.clockOut);
        });
    }

    //find earliest clockIn and last clockout for default timeframe
    function isEarliestClockIn(clockIn) {
        ctrl.timeframe.startTime = (clockIn < ctrl.timeframe.startTime) ? clockIn : ctrl.timeframe.startTime;
        return new Date(ctrl.timeframe.startTime);

    }
    function isLastClockOut(clockOut) {
        ctrl.timeframe.endTime = (clockOut > ctrl.timeframe.endTime) ? clockOut : ctrl.timeframe.endTime;
        return new Date(ctrl.timeframe.endTime);

    }

    //Load or Update Map	
    function getMap(zoomLevel?, timeframe?, pushpin?) {
        if (ctrl.visibleEntries && ctrl.visibleEntries.length) {
            getPushpins().then(function(response) {

                var pushpins = '';

                if (ctrl.selectedPushpin) {
                    pushpins = ctrl.selectedPushpin;
                }
                else {
                    pushpins = response;
                }

                if (!pushpins) {
                    ctrl.hideMap = true;
                }
                else {
                    ctrl.hideMap = false;
                    var zoom = ctrl.useZoom ? 'zoomLevel=' + zoomLevel + '&' : '&';
                    var mapUrl = 'http://dev.virtualearth.net/REST/v1/Imagery/Map/Road?mapSize=450,400&' + pushpins + zoom + 'declutter=1&key=AtynugkvBm592ahESde0X1n0a40X9FPuX-FNn0Ev1hgkMtV86fjDYjFp0_yQnO83';
                    ctrl.map = mapUrl;
                    ctrl.useZoom = false;
                }
            });
        }
    }

    function getPushpins() {
        var pushpins = "";
        var deferred = $q.defer();
        var promises = [];
        var pushpinsCount = 0;

        _.each(ctrl.visibleEntries, function(timerEntry, index) {
            if (timerEntry.clockInLatitude && timerEntry.clockOutLatitude) {
                var promise = _getIntermediaryPushpins(timerEntry, index).then(function(data) {
                    if (isWithinTimeframe(timerEntry.clockIn) && (timerEntry.clockInLatitude && timerEntry.clockInLongitude)) {
                        pushpins = pushpins + 'pushpin=' + timerEntry.clockInLatitude + ',' + timerEntry.clockInLongitude + ';79;' + (pushpinsCount) + '&';
                        pushpinsCount++;
                        //note that teardrop pushpins are 63 and 65
                    }

                    for (var i = 0; i < data.length; i++) {
                        pushpins = pushpins + 'pushpin=' + data[i].latitude + ',' + data[i].longitude + ';67;' + '&';
                    }

                    if (isWithinTimeframe(timerEntry.clockOut) && (timerEntry.clockOutLatitude && timerEntry.clockOutLongitude)) {
                        pushpins = pushpins + 'pushpin=' + timerEntry.clockOutLatitude + ',' + timerEntry.clockOutLongitude + ';80;' + (pushpinsCount) + '&';
                        pushpinsCount++;
                    }
                });
            }
            else {
                if (timerEntry.clockOutLatitude === null && timerEntry.clockInLatitude) {
                    pushpins = pushpins + 'pushpin=' + timerEntry.clockInLatitude + ',' + timerEntry.clockInLongitude + ';79;' + (pushpinsCount) + '&';
                    pushpinsCount++;
                }
                else if (timerEntry.clockInLatitude === null && timerEntry.clockOutLatitude) {
                    pushpins = pushpins + 'pushpin=' + timerEntry.clockOutLatitude + ',' + timerEntry.clockOutLongitude + ';80;' + (pushpinsCount) + '&';
                    pushpinsCount++;
                }
            }

                
                
            promises.push(promise);
            
        });
        $q.all(promises).then(function() {
            deferred.resolve(pushpins);
        });
        
        return deferred.promise;
    }

    //check to see if clock in and clock out is within timeframe, rounded to the minute.
    function isWithinTimeframe(time) {
        var momentTime = moment(time);
        return momentTime.startOf('minute') >= moment(ctrl.timeframe.startTime).startOf('minute') && momentTime.startOf('minute') <= moment(ctrl.timeframe.endTime).startOf('minute');
    }

    function _getIntermediaryPushpins(timerEntry, index?) {
        var formattedStartTime = moment(timerEntry.clockIn).format("YYYY-MM-DD HH:mm:ss");
        var formattedEndTime = moment(timerEntry.clockOut).format("YYYY-MM-DD HH:mm:ss");
        var locationsQueryParams = { endTime: formattedEndTime.toString(), startTime: formattedStartTime.toString() };
        if (timerEntry.clockInLatitude || timerEntry.clockOutLatitude) {
            return Locations.getGPSCoordinatesByEmployeeID($routeParams.employeeId, locationsQueryParams);
        }
        
    }
}