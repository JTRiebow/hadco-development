import * as angular from 'angular';
import * as moment from 'moment';
import * as _ from 'lodash';

import * as template from './employee-clock-in.html';
import * as clockInTemplate from '../Shared/modal-templates/clock-in-modal.html';
import * as clockOutTemplate from '../Shared/modal-templates/clock-out-modal.html';

angular.module('employeeModule')
    .component('employeeClockIn', {
        controller: employeeClockInController,
        controllerAs: 'vm',
        template,
    });

employeeClockInController.$inject = [
    '$scope',
    'CurrentUser',
    'authorization',
    'Users',
    '$timeout',
    '$location',
    '$modal',
    'NotificationFactory',
    'Restangular',
    'EmployeeTimers',
    'EmployeeTimerEntries',
    '$http',
    'EmployeeTimecards',
    'PermissionService',
];

function employeeClockInController(
    $scope,
    CurrentUser,
    authorization,
    Users,
    $timeout,
    $location,
    $modal,
    NotificationFactory,
    Restangular,
    EmployeeTimers,
    EmployeeTimerEntries,
    $http,
    EmployeeTimecards,
    PermissionService,
) {
    var vm = this;
    vm.allowedToClockIn = true;

    PermissionService.getMyPermissions().then(function () {
        Promise.all([
            PermissionService.can('clockInFromWeb'),
            PermissionService.getDepartmentPermissionBreakdown(['clockInFromWeb']),
        ]).then(([canClockInFromWeb, [perm]]) => {
            vm.allowedToClockIn = canClockInFromWeb && perm && Boolean(perm.departmentIds.length);
        });
    });

    $scope.timerData;
    $scope.longitude;
    $scope.latitude;

    getWeeklySUmmary();

    $scope.accessTimers = function(selectedDepartment) {
        $scope.selectedDepartment = selectedDepartment;
        getTimer(selectedDepartment)
        .then(function(timer) {
            $scope.timerData = timer;
        }, function(error) {
            EmployeeTimers.post({ departmentID: selectedDepartment.departmentID, employeeID: $scope.employee.employeeID, day: moment().format() })
            .then(function(newTimer) {
                $scope.timerData = newTimer;
            });
        });
    };

    CurrentUser.get()
        .then(function(me) {
            $scope.employee = me;

            $scope.me = CurrentUser;

            if ($scope.me.departments.isTMCrushing() && $scope.me.departments.isFrontOffice()) {
                //hard coded to include Front Office and TM Crushing on Clock in page. Default to Front office if both are present
                $scope.multipleDepartments = [{ departmentID: 7, name: 'TMCrushing' }, { departmentID: 6, name: 'FrontOffice' }];
                $scope.selectedDepartment = $scope.multipleDepartments[1];
            }
            else if ($scope.me.departments.isTMCrushing()) {
                $scope.multipleDepartments = false;
                $scope.selectedDepartment = { departmentID: 7, name: 'TMCrushing' };
            }
            else if ($scope.me.departments.isFrontOffice()) {
                $scope.multipleDepartments = false;
                $scope.selectedDepartment = { departmentID: 6, name: 'FrontOffice' };
            }


            $scope.isAuthorizedToView = $scope.me.departments.isFrontOffice() || $scope.me.departments.isTMCrushing();

            if ($scope.selectedDepartment) {
                $scope.accessTimers($scope.selectedDepartment);
            }
        });


    $scope.clockIn = function() {
        var currentTime;

        Restangular.one('CurrentTime').get().then(function(response) {
            currentTime = response;
            handleModal();
        }, function(error) {
            NotificationFactory.error("Error: Unable to get current time.");
        });

        function handleModal() {
            var modalInstance = $modal.open({
                template: clockInTemplate,
                controller: 'ClockInModalContentController',
                windowClass: 'default-modal',
                resolve: {
                    currentUser: function() {
                        return $scope.me;
                    },
                    TMCrushingOrFrontOffice: function() {
                        return $scope.selectedDepartment;
                    },
                },
            });

            modalInstance.result.then(function(clockInResponse) {
                if (!$scope.clockInTime && currentTime) {

                    var clockInData = {
                        employeeTimerID: $scope.timerData.employeeTimerID,
                        clockIn: currentTime,
                        clockInNote: clockInResponse.notes,
                        clockInLatitude: $scope.latitude,
                        clockInLongitude: $scope.longitude,
                        pitID: clockInResponse.pitID,
                    };

                    EmployeeTimerEntries.post(clockInData)
                    .then(function(data) {
                        console.log(data);
                        getTimer($scope.selectedDepartment).then(function(data) {
                            $scope.timerData = data;
                        });
                    });
                }
            });
        }
    };

    $scope.clockOut = function() {
        var len = $scope.timerData.employeeTimerEntries.length - 1;
        if ($scope.timerData.employeeTimerEntries[len].clockIn && !$scope.timerData.employeeTimerEntries[len].clockOut) {
            var modalInstance = $modal.open({
                template: clockOutTemplate,
                controller: 'ClockoutModalContentController',
                windowClass: 'default-modal',
                resolve: {
                    injured: function() {
                        return $scope.timerData.injured;
                    },
                },
            });

            modalInstance.result.then(function(time) {
                var len = $scope.timerData.employeeTimerEntries.length - 1;
                var clockOutData = {
                    employeeTimerID: $scope.timerData.employeeTimerID,
                    clockOut: moment.utc().format(),
                    clockOutNote: time.notes,
                    clockOutLatitude: $scope.latitude,
                    clockoutLongitude: $scope.longitude,
                };
                EmployeeTimerEntries.one($scope.timerData.employeeTimerEntries[len].employeeTimerEntryID)
                .patch(clockOutData)
                .then(function(data) {
                    EmployeeTimers.one(data.employeeTimerID).patch({
                        departmentID: $scope.selectedDepartment.departmentID,
                        injured: time.injured,

                    })
                    .then(function(response) {
                        getTimer($scope.selectedDepartment).then(function(data) {
                            $scope.clockInTime = false;
                            getWeeklySUmmary();
                        });
                    });
                });
            });
        }
    };

    function getTimer(selectedDepartment) {
        return Users.one($scope.employee.employeeID).one('EmployeeTimer', moment().format("YYYY-MM-DD")).get({ "departmentID": selectedDepartment.departmentID })
            .then(function(data) {
                _.each(data.employeeTimerEntries, function(t) {
                    if (t.clockOut) {
                        t.clockOut = moment(t.clockOut).format('h:mm a');
                    }
                    if (t.clockIn) {
                        t.clockIn = moment(t.clockIn).format('h:mm a');
                    }
                });
                if (data.employeeTimerEntries.length > 0) {
                    var timers = data.employeeTimerEntries[data.employeeTimerEntries.length - 1];
                    if (timers.clockIn && !timers.clockOut) {
                        $scope.clockInTime = timers.clockIn;
                    }
                }

                return $scope.timerData = data;
            });
    }

    EmployeeTimecards.one('Summary').one('Me').get({ week: moment().subtract(1, 'week').format("YYYY-MM-DD") })
    .then(function(data) {
        $scope.oldTotal = 0;
        if (data.items.length) {
            $scope.oldTime = data.items;
        }
        for (var i = 0; i < $scope.oldTime.length; i++) {
            $scope.oldTotal += $scope.oldTime[i].totalHours;
        }
    });

    function getWeeklySUmmary() {
        EmployeeTimecards.one('Summary').one('Me').get({ week: moment().format("YYYY-MM-DD") })
        .then(function(data) {
            $scope.totalTime = 0;
            if (data.items.length)
                $scope.week = data.items;
            for (var i = 0; i < $scope.week.length; i++) {
                $scope.week[i].day = moment(new Date($scope.week[i].day)).format('ddd');
                $scope.totalTime += $scope.week[i].totalHours;
            }
        });
    }


    var successFunction = function(position) {
        $scope.latitude = position.coords.latitude;
        $scope.longitude = position.coords.longitude;
    };

    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(successFunction);
    }

    $scope.logout = function() {
        CurrentUser.setLoggedIn(false);
        authorization.logout();
        $timeout(function() {
            $location.path('/');
        });
    };
}