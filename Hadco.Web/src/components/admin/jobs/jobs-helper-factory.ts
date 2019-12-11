import * as angular from 'angular';
import * as _ from 'lodash';

angular
    .module('adminModule')
    .factory('JobsHelper', jobsHelperFactory);

jobsHelperFactory.$inject = [ 'Jobs', '$q' ];

function jobsHelperFactory(Jobs, $q) {
    var _list;
    var _params;
    var deferredCalls = [];
    var querying = false;

    var service = {} as IJobsHelperService;
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
            Jobs.getList(params).then(function(response) {
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
        return Jobs.one(ID).get();
    }

    function post(x) {
        var deferred = $q.defer();
        if (x) {
            Jobs.post(x).then(function(response) {
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
            Jobs.one(x.jobID).remove().then(function() {
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
        if (x && x.jobID) {
            Jobs.one(x.jobID).patch(x).then(function() {
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

interface IJobsHelperService {
    getList(params): ng.IPromise<any[]>;
    clearCache(): void;
    get(ID): ng.IPromise<any>;
    post(trailer): ng.IPromise<any>;
    remove(trailer): ng.IPromise<any>;
    patch(trailer): ng.IPromise<any>;
}

export {
    IJobsHelperService,
};