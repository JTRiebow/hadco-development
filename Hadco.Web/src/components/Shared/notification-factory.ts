import * as angular from 'angular';
import * as toastr from 'toastr';

angular.module('sharedModule').factory('NotificationFactory', [
	function() {
	    return {
	        success: function(message) {
	            toastr.success(message);
	        },
	        error: function(message) {
	            toastr.error(message);
			},
			warning(message) {
				toastr.warning(message);
			},
	        alertPrompt: function(title, message, proceed, abort) {
	            console.log(title, message);
	        },
	    };
	},
]);
