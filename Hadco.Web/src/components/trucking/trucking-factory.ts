import * as angular from 'angular';
import * as moment from 'moment';

angular.module('truckingModule').factory('TruckingService', [
	'Restangular',
	function(Restangular) {

		//service
		var service = {} as ITruckingService;

		service.getRanges = getRanges;
		service.getDisplays = getDisplays;

		return service;

		//functions

		function getRanges() {
			var ranges = [
			//include or exclude end date? It's currently set to include.
				{
					name: "Today",
					start: moment().startOf('day').format('MM/DD/YYYY'),
					end: moment().startOf('day').format('MM/DD/YYYY'),
				},
				{
					name: "Yesterday",
					start: moment().subtract(1, 'day').startOf('day').format('MM/DD/YYYY'),
					end: moment().subtract(1, 'day').endOf('day').format('MM/DD/YYYY'),
				},
				{
					name: "This Week",
					start: moment().startOf('week').format('MM/DD/YYYY'),
					end: moment().endOf('week').format('MM/DD/YYYY'),
				},
				{
					name: "This Month",
					start: moment().startOf('month').format('MM/DD/YYYY'),
					end: moment().endOf('month').format('MM/DD/YYYY'),
				},
				{
					name: "This Quarter",
					start: moment().startOf('quarter').format('MM/DD/YYYY'),
					end: moment().endOf('quarter').format('MM/DD/YYYY'),
				},
				{
					name: "This Year",
					start: moment().startOf('year').format('MM/DD/YYYY'),
					end: moment().endOf('year').format('MM/DD/YYYY'),
				},
				{
					name: "Last Week",
					start: moment().subtract(1, 'week').startOf('week').format('MM/DD/YYYY'),
					end: moment().subtract(1, 'week').endOf('week').format('MM/DD/YYYY'),
				},
				{
					name: "Last Month",
					start: moment().subtract(1, 'month').startOf('month').format('MM/DD/YYYY'),
					end: moment().subtract(1, 'month').endOf('month').format('MM/DD/YYYY'),
				},
				{
					name: "Last Quarter",
					start: moment().subtract(1, 'quarter').startOf('quarter').format('MM/DD/YYYY'),
					end: moment().subtract(1, 'quarter').endOf('quarter').format('MM/DD/YYYY'),
				},
				{
					name: "Last Year",
					start: moment().subtract(1, 'year').startOf('year').format('MM/DD/YYYY'),
					end: moment().subtract(1, 'year').endOf('year').format('MM/DD/YYYY'),
				},
			];
			return ranges;
		}

		function getDisplays() {
			var displays = [
				{ name: "Day" },
				{ name: "Week" },
				{ name: "Month" },
				{ name: "Quarter" },
				{ name: "Year" },
			];
			return displays;
		}
	},
]);

interface ITruckingService {
	getRanges(): any[];
	getDisplays(): any[];
}

export {
	ITruckingService,
};