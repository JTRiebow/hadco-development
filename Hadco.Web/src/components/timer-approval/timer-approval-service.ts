import * as angular from 'angular';
import * as moment from 'moment';

angular.module('employeeModule').factory('TimerApprovalsService', [
    'Restangular',
    function(Restangular) {
        var service = {} as ITimerApprovalsService;

        Restangular.configuration.routeToIdMappings['TimerApprovalsService'] = 'timerApprovalId';
        angular.copy(Restangular.service('TimerApprovalsService'), service);

        service.patch = patch;
        return service;

        function patch(employee, approvalsObj) {
            var formattedDay = moment(employee.day).format("YYYY-MM-DD");

            return Restangular
                .one('Employee', employee.employeeID)
                .one('Day', formattedDay)
                .one('Department', employee.departmentID)
                .one('DailyApproval')
                .patch(approvalsObj)
                .then(data => {
                    return data
                });
        }
    },
]);

interface ITimerApprovalsService {
    patch(employee, approvalsObj): ng.IPromise<{}>;
}

export {
    ITimerApprovalsService,
};