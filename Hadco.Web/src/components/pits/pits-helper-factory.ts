import * as angular from 'angular';
import * as _ from 'lodash';

angular
	.module('pitsModule')
	.factory('PitsHelper', PitsHelper);

PitsHelper.$inject = [ 'Pits', '$q' ];

function PitsHelper(Pits, $q) {
	var _list;
	var _params;
	var deferredCalls = [];
	var querying = false;

	var service = {} as IPitsHelperService;
	service.getList = getList;
	service.clearCache = clearCache;
	service.get = get;
	service.post = post;
	service.remove = remove;
	service.patch = patch;
	return service;

	function getList(params) {
		var deferred = $q.defer();

		if (_list && (JSON.stringify(_params) === JSON.stringify(params))) {
			deferred.resolve(_list);
		}
		else if (querying) {
			deferredCalls.push(deferred);
		}
		else {
			querying = true;
			Pits.getList(params).then(function(response) {
				_params = params;
				_list = response;
				querying = false;
				deferred.resolve(_list);
				_resolveDeferredCalls();
			}, deferred.reject);
		}
		return deferred.promise;
	}

	function _resolveDeferredCalls() {
		_.each(deferredCalls, function(deferredCall) {
			deferredCall.resolve(_list);
		});
	}

	function get(ID) {
		return Pits.one(ID).get();
	}

	function post(x) {
		var deferred = $q.defer();
		if (x) {
			Pits.post(x).then(function(response) {
				deferred.resolve(response);
			}, deferred.reject);
		}
		else {
			deferred.reject();
		}
		clearCache();
		return deferred.promise;
	}

	function remove(x) {
		var deferred = $q.defer();
		if (x && x.occurrenID) {
			Pits.one(x.pitID).remove().then(function() {
				deferred.resolve(x);
			}, deferred.reject);
		}
		else {
			deferred.reject();
		}
		clearCache();
		return deferred.promise;
	}

	function patch(x) {
		var deferred = $q.defer();
		if (x && x.pitID) {
			Pits.one(x.pitID).patch(x).then(function() {
				deferred.resolve();
			}, deferred.reject);
		}
		else {
			deferred.reject();
		}
		clearCache();
		return deferred.promise;
	}

	function clearCache() {
		_list = null;
	}
}

interface IPitsHelperService {
	getList(params): ng.IPromise<any[]>;
	clearCache(): void;
	get(id: number): ng.IPromise<any>;
	post(pit): ng.IPromise<any>;
	remove(pit): ng.IPromise<any>;
	patch(pit): ng.IPromise<any>;
}

export {
	IPitsHelperService,
};