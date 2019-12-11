import * as angular from 'angular';

angular.module('sharedModule').factory('Pagination', [
	function() {
	    var service = {} as IPaginationService;

	    service.skip = function(page, itemsPerPage) {
	        return (page - 1) * itemsPerPage;
	    };

	    return service;
	},
]);

interface IPaginationService {
	skip(page: number, itemsPerPage: number): number;
}

export {
	IPaginationService,
};