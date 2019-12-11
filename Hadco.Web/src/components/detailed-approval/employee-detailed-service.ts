import * as angular from 'angular';
import * as moment from 'moment';
import * as _ from 'lodash';
import { IDateTimeFormatsService } from '../Shared/date-time-formats-factory';

angular
	.module('detailedApprovalModule')
	.factory("EmployeeDetailedService", EmployeeDetailedService);

EmployeeDetailedService.$inject = [
	'DowntimeTimers',
	'$routeParams',
	'$location',
	'TimesheetsService',
	'EmployeeTimers',
	'Users',
	'NotificationFactory',
	'$q',
	'EmployeeTimerEntries',
	'DateTimeFormats',
];

function EmployeeDetailedService(
	DowntimeTimers,
	$routeParams,
	$location,
	TimesheetsService,
	EmployeeTimers,
	Users,
	NotificationFactory,
	$q,
	EmployeeTimerEntries,
	DateTimeFormats: IDateTimeFormatsService,
) {

	var service = {} as IEmployeeDetailedService;
	service.returnToApprovalPage = returnToApprovalPage;
	service.day = day;
	service.setHistory = setHistory;
	service.saveChanges = saveChanges;
	service.saveEntries = saveEntries;
	service.getTimers = getTimers;
	service.hasOverlap = hasOverlap;

	return service;

	function returnToApprovalPage() {
		if (sessionStorage.getItem("cachedReturnToPage") === "accounting") {
			$location.path("accounting/timer/approval");
		}
		else if (sessionStorage.getItem("cachedReturnToPage") === "foreman-timesheet") {
			$location.path(sessionStorage.getItem("cachedForemanUrl"));
		}
		else if (sessionStorage.getItem("cachedReturnToPage") === "employee-search") {
			$location.path(sessionStorage.getItem("cachedEmployeeSearchUrl"));
		}
		else if (sessionStorage.getItem("cachedReturnToPage") === "billing") {
			$location.path("billing/timer/approval");
		}
		else {
			$location.path("supervisor/timer/approval");
		}
	}


	function day() {
		return DateTimeFormats.getShortDateFormat($routeParams.dayId, DateTimeFormats.urlDateFormat());
	}

	function setHistory(timerEntry) {
		
		var newHistory = [];
		var histories = timerEntry.employeeTimerEntryHistories;
		_.each(histories, function(history, index) {
			newHistory[index] = {
				name: history.changedBy.name,
				previousClockIn: _formatTime(history.previousClockIn),
				currentClockIn: _formatTime(history.currentClockIn),
				previousClockOut: _formatTime(history.previousClockOut),
				currentClockOut: _formatTime(history.currentClockOut),
				date: DateTimeFormats.getDateTimeFormat(history.changedTime),
				employeeTimerEntryID: history.employeeTimerEntryID, //to use for filtering selected timer entry

			};
		});
		return newHistory;
		
	}

	function _formatTime(time) {
		time = time ? DateTimeFormats.getTimeFormat(time) : "n/a";
		return time;
	}

	function saveEntries(employeeTimerEntries) {
		var defer = $q.defer();
		var promises = [];

		employeeTimerEntries.forEach(function(entry) {
			var entryPromise;
			entry.clockIn = moment(entry.clockIn).format();
			entry.clockOut = moment(entry.clockOut).format();
			if (_.isString(entry.employeeTimerEntryID)) {
				entryPromise = EmployeeTimerEntries
				.post(entry);
			}
			else {
				entryPromise = EmployeeTimerEntries.one(entry.employeeTimerEntryID)
				.patch(entry);
			}
			promises.push(entryPromise);

		});

		$q.all(promises).then(function(response) {
			defer.resolve(response);
		}, function(error) {
			defer.reject(error);
		});
		return defer.promise;
	}

	function saveChanges(employeeTimers, timesheet) {
		/*1. modify each timer entry
		2. and save each timer entry
		3. then modify each employee timer
		4. and save each employee timer
		5. then update each employee timer with new totalHours value.
		*/
		
		return saveEntries(employeeTimers)
		.then(function(response) {
			return saveTimers(employeeTimers);
		});
	}

	function saveTimers(employeeTimers) {
		var defer = $q.defer();
		var promises = [];
		employeeTimers.forEach(function(timer) {
			var foremanTimesheetID = 0;
			if (sessionStorage.getItem("cachedReturnToPage") === "foreman-timesheet") {
				foremanTimesheetID = +sessionStorage.getItem("cachedTimesheet");
			}

			timer.equipmentID = timer.equipment ? timer.equipment.equipmentID : null;
			timer.timesheetID = timer.timesheetID || foremanTimesheetID || null;

			var timerPromise = EmployeeTimers
			.one(timer.employeeTimerID)
			.patch(timer);
			promises.push(timerPromise);
		}, function(error) {
			NotificationFactory.error('Error: Timer Entries could not be saved');
		});

		$q.all(promises).then(function(response) {
			NotificationFactory.success('Success: Timer saved');
			defer.resolve(response);
		}, function(error) {
			NotificationFactory.error('Error: Timers could not be saved');
			defer.reject(error);
		});

		return defer.promise;
	}

	function getTimers(attemptsLeft = 1) {
		return Users.one($routeParams.employeeId)
			.one('Timesheets', $routeParams.dayId)
			.get({ departmentID: $routeParams.departmentId })
			.then(function(data) {
				return data;
			}, function(error) {
				return TimesheetsService.createNewTimesheet($routeParams.dayId, $routeParams.employeeId, $routeParams.departmentId)
					.then(function(data) {
						return getTimers(attemptsLeft - 1);
					});
			});
	}

	function hasOverlap(timer, startTimeKey, endTimeKey, list, timerID) {
		var isOverlapping;

		angular.forEach(list, function(gridTimer) {
			gridTimer.startTimeOverlap;
			gridTimer.endTimeOverlap;

			//check timer starttime is before endtime
			var isStartTimeAfterEndTime = moment(gridTimer[startTimeKey]).isAfter(moment(gridTimer[endTimeKey]));
			if (isStartTimeAfterEndTime) {
				isOverlapping = true;
				gridTimer.startTimeOverlap = true;
				gridTimer.endTimeOverlap = true;
			}

			if ((gridTimer[timerID] !== timer[timerID]) && _isTimerOverLapping(gridTimer, timer, startTimeKey, endTimeKey)
				) {
				isOverlapping = true;
				if (new Date(timer.startTime).getTime() < new Date(timer.startTime).getTime()) {
					timer.startTimeOverlap = true;
					gridTimer.endTimeOverlap = true;
				}
				else {
					timer.endTimeOverlap = true;
					gridTimer.startTimeOverlap = true;
				}
			}
		});
		return isOverlapping;
	}

	function _isTimerOverLapping(gridTimer, timer, startTimeKey, endTimeKey) {
		var timerStartTimeBeforeRowEndTime = moment(gridTimer[startTimeKey]).isBefore(moment(timer[endTimeKey]));
		var timerEndTimeAfterRowStartTime = moment(gridTimer[endTimeKey]).isAfter(moment(timer[startTimeKey]));
		var isOverlapping = timerStartTimeBeforeRowEndTime && timerEndTimeAfterRowStartTime;

		return isOverlapping;
	}
	
	
}

interface IEmployeeDetailedService {
	returnToApprovalPage(): void;
	day(): string;
	setHistory(timerEntry): any[];
	saveChanges(employeeTimers, timesheet): ng.IPromise<any>;
	saveEntries(employeeTimerEntries): ng.IPromise<any>;
	getTimers(attemptsLeft?): ng.IPromise<any[]>;
	hasOverlap(timer, startTimeKey, endTimeKey, list, timerID): boolean;
}

export {
	IEmployeeDetailedService,
};