//angular.module('sharedModule').directive('datetime', [
//	'Dates',
//	function (Dates) {
//		return {
//			restrict: 'A',
//			scope: {
//				datetime: '=',
//				datetimeText: '@',
//				dateValue: "=",
//				showTime: "=",
//				hideTime: "=",
//				shortTime: "="
//			},
//			templateUrl: 'app/components/shared/date-time.html',
//			link: function (scope, element, attrs) {
//				scope.format = Dates.format;
//				scope.shortFormat = Dates.shortFormat;
//				scope.now = function () {
//					scope.datetime = new Date();
//				scope.shortTime = moment(scope.datetime).format('HH:mm');	
//				};

//			}
//		};
//	}
//]);

//angular.module('sharedModule').directive('date', [
//	'DateTimeFormats',
//	function (DateTimeFormats) {
//		return {
//			restrict: 'A',
//			scope: {
//				datetime: '=',
//				datetimeText: '@',
//				dateValue: "=",
//				showTime: "=",
//				shortTime: '=',
//				shortFormat: "=",
//				hideTime: "="
//			},
//			templateUrl: 'app/components/shared/date-time.html',
//			link: function (scope, element, attrs) {
//				scope.format = Dates.format;
//				if (shortFormat == true)
//				scope.format = Dates.shortFormat;
		
//				scope.shortTime = moment(scope.datetime).format('HH:mm');
				
//				scope.now = function () {
//					scope.datetime = new Date();

//				};
//			}
//		};
//	}
//]);