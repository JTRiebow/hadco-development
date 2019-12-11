import * as angular from 'angular';
import * as moment from 'moment';
import * as _ from 'lodash';

import * as template from './superintendent.html';
import * as selectDateTemplate from '../../Shared/modal-templates/select-date-modal.html';

angular
	.module('employeeModule')
	.component('htSuperintendent', {
		controller: superintendentController,
		controllerAs: 'vm',
		template,
	});

superintendentController.$inject = [
	'$scope',
	'$modal',
	'EmployeeTimers',
	'$location',
	'CurrentUser',
	'TimesheetsService',
	'DepartmentsHelper',
	'PermissionService',
];

function superintendentController(
	$scope,
	$modal,
	EmployeeTimers,
	$location,
	CurrentUser,
	TimesheetsService,
	DepartmentsHelper,
	PermissionService,
) {
	const vm = this;
	
	vm.can = PermissionService.can;
	
	var count = 0;
	let departmentList = [];
	
	vm.departmentFilterList = [];
	vm.onDepartmentFilterChange = onDepartmentFilterChange;
	
	DepartmentsHelper
		.getList()
		.then(list => {
			departmentList = list;
		});
	
	$scope.today = moment().format();
	$scope.week = {};
	$scope.filters = [
		{ 'supervisorApproved': true, 'name': 'Approved' },
		{ 'supervisorApproved': false, 'name': 'Not Approved' },
		{ 'name': 'All' },
	];
	$scope.detailPageWithCaching = detailPageWithCaching;
	$scope.myFilter = myFilter;
	$scope.clearFilters = clearFilters;
	$scope.jobChanged = jobChanged;
	$scope.clearJob = clearJob;
	$scope.foremanChanged = foremanChanged;
	$scope.daysOfWeek = daysOfWeek;
	$scope.filterChanged = filterChanged;
	$scope.addTimesheet = addTimesheet;

	_getSuperintendentForemen();

	$scope.$watch('selectedWeek', function(newValue, oldValue) {
		newValue = moment(newValue);
		oldValue = moment(oldValue);
		if (count > 0 || moment(newValue).week() !== moment(oldValue).week()) {
			$scope.week.week = moment(newValue).startOf('week').format('MM/DD/YYYY');
			// var weekString = JSON.stringify($scope.week.week);
			sessionStorage.setItem("superintendentCachedWeek", $scope.week.week);
			_getSuperintendentForemen($scope.week.week, $scope.filter);
			if (moment($scope.week.week).week() >= moment().week())
				$scope.approveDisabled = true;
			else
				$scope.approveDisabled = false;
		}
		else {
			count++;
		}
	});

	function addTimesheet(foreman, week) {
		var modalInstance = $modal.open({
			template: selectDateTemplate,
			controller: 'selectDateModalController',
			windowClass: 'default-modal',
			resolve: {
				foreman: function() { return foreman; },
				date: function() { return week; },
			},
		});

		modalInstance.result.then(function(day) {
			TimesheetsService.createNewTimesheet(day, foreman.employeeID, foreman.departmentID)
			.then(function(data) {
				//Make call to add supervisor to timesheet
				var newEmployeeTimer = {
					timesheetID: data.timesheetID,
					day: day,
					employeeID: foreman.employeeID,
					departmentID: foreman.departmentID,
				};
				EmployeeTimers.post(newEmployeeTimer).then(function(response) {
					$scope.detailPageWithCaching(data);
				});
			}, function(error) {
				console.log('error');
			});

		});
	}

	function clearFilters() {
		$scope.filteredForemen = angular.copy($scope.foremen);
		$scope.foreman = '';
		sessionStorage.superintendentCachedForeman = null;

		$scope.selectedJob = '';
		sessionStorage.superintendentCachedJob = null;
		sessionStorage.setItem("superintendentCachedWeek", $scope.selectedWeek);
		$scope.filter = { name: "All", $$hashKey: "object:103" };
		$scope.filterChanged($scope.filter);

		_getSuperintendentForemen($scope.selectedWeek, $scope.filter);
	}

	function clearJob() {
		$scope.allJobs = $scope.JOBS_CONST;
	}

	function daysOfWeek(day) {
		$scope.days = [];
		$scope.refDays = [];
		for (var i = 0; i < 7; i++) {
			$scope.days[i] = moment(new Date(day)).add(i, 'd').format("MM/DD/YYYY");
			$scope.refDays[i] = moment(new Date(day)).add(i, 'd').format('MM/DD/YYYY');
		}
	}

	function detailPageWithCaching(timesheet) {
		sessionStorage.setItem("cachedReturnToPage", "superintendent");
		sessionStorage.setItem("superintendentCachedWeek", $scope.selectedWeek);

		var formattedForRouteParamDay = moment(timesheet.day).format("MM-DD-YYYY");
		
		$location.path("/superintendent/foreman/" + timesheet.employeeID + "/department/" + timesheet.departmentID + "/day/" + formattedForRouteParamDay);
	}

	function filterChanged(filter) {
		// console.log('Filter Changed to: ', filter);
		$scope.filterString = JSON.stringify(filter);
		sessionStorage.setItem("superintendentCachedFilter", $scope.filterString);
		$scope.filter = filter;
		_getSuperintendentForemen($scope.week.week, filter);
	}

	function foremanChanged(selectedForeman) {
		sessionStorage.setItem("superintendentCachedForeman", JSON.stringify(selectedForeman));
		_updateFilteredForemen(selectedForeman);
		_updateFilteredTimesheets();

	}

	function jobChanged() {
		sessionStorage.setItem("superintendentCachedJob", $scope.selectedJob);
		_updateFilteredTimesheets();

	}

	function myFilter(item) {
		if ($scope.foreman !== "") {
			return item.name === $scope.foreman;
		}
		else { return true; }
	}

	function _getAllJobs(data) {
		var jobList = [];
		_.each(data, function(foreman) {
			_.each(foreman.timesheets, function(timesheet) {
				_.each(timesheet.jobNumbers, function(job) {
					if (jobList.indexOf(job) < 0) {
						jobList.push(job);
					}
				});
			});
		});

		return jobList;
	}

	function _getSuperintendentForemen(week?, filter?) {

		if (typeof (Storage) !== "undefined") {

			if (sessionStorage.superintendentCachedFilter) {
				$scope.filter = JSON.parse(sessionStorage.getItem("superintendentCachedFilter"));
			}
			else {
				$scope.filter = { name: "All" };
			}

			if (sessionStorage.superintendentCachedWeek) {
				$scope.selectedWeek = sessionStorage.getItem("superintendentCachedWeek");
			}
			else {
				var count = 0;
				$scope.week = {};
				$scope.week.week = moment().subtract(1, 'week').startOf('week').format('MM/DD/YYYY');
				$scope.selectedWeek = $scope.week.week;
			}
		}
		else {
			console.log("No Storage support");
		}

		CurrentUser.get()
			.then(function(me) {
				$scope.me = me;
				
				TimesheetsService
					.getForemenForSuperintendent("1", $scope.selectedWeek)
					.then(function(data) {
						$scope.foremen = _removeEmptyTimesheets(data);
						
						const departmentMap = { '': 'All' };
						
						_.each($scope.foremen, function(foreman) {
							foreman.filteredTimesheets = foreman.timesheets;
							if (!departmentMap[foreman.departmentID]) {
								departmentMap[foreman.departmentID] = departmentList.find(d => d.departmentID == foreman.departmentID).name;
							}
						});
						
						vm.departmentFilterList = Object.keys(departmentMap).map(id => ({ id, name: departmentMap[id] }));

						$scope.filteredForemen = angular.copy($scope.foremen);

						$scope.allJobs = _getAllJobs(data);
						$scope.JOBS_CONST = _getAllJobs(data);

						if (typeof (Storage) !== "undefined") {

							if (sessionStorage.superintendentCachedForeman && sessionStorage.superintendentCachedForeman != "null") {
								var storedForemen = JSON.parse(sessionStorage.getItem("superintendentCachedForeman"));

								_updateFilteredForemen(storedForemen);
								$scope.foreman = storedForemen;

							}

							if (sessionStorage.superintendentCachedJob && sessionStorage.superintendentCachedJob != "null") {
								$scope.selectedJob = sessionStorage.getItem("superintendentCachedJob");
								_updateFilteredTimesheets();
							}
						}
					});
			});
	}

	function _updateFilteredForemen(selectedForeman) {
		$scope.filteredForemen = _.filter($scope.foremen, function(foreman) {
			return foreman.employeeID === selectedForeman.employeeID;
		});

		$scope.filteredForemen = _removeEmptyFilteredTimesheets($scope.filteredForemen);
		$scope.allJobs = _getAllJobs($scope.filteredForemen);
	}

	function _updateFilteredTimesheets() {
		_.each($scope.filteredForemen, function(foreman) {
			foreman.filteredTimesheets = _.filter(foreman.timesheets, function(timesheet) {
				return !$scope.selectedJob || timesheet.jobNumbers.indexOf($scope.selectedJob) > -1;
			});
		});

		$scope.filteredForemen = _removeEmptyFilteredTimesheets($scope.filteredForemen);

	}

	function _removeEmptyFilteredTimesheets(data) {
		return _.filter(data, function(foreman) {
			if (foreman.filteredTimesheets.length > 0) {
				return foreman;
			}
		});
	}

	function _removeEmptyTimesheets(data) {

		return _.filter(data, function(foreman) {
			if (foreman.timesheets.length > 0) {
				return foreman;
			}
		});
	}
	
	function onDepartmentFilterChange(department) {
		$scope.filteredForemen = _removeEmptyFilteredTimesheets($scope.foremen.filter(f => !department.id || f.departmentID == department.id && f.filteredTimesheets.length));
		$scope.allJobs = _getAllJobs($scope.filteredForemen);
	}
}
