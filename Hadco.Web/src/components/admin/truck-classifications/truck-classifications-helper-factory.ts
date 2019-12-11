import * as angular from 'angular';
import * as _ from 'lodash';

angular
    .module('adminModule')
    .factory('TruckClassificationsHelper', TruckClassificationsHelper);

TruckClassificationsHelper.$inject = [ 'TruckClassifications', '$q' ];

function TruckClassificationsHelper(TruckClassifications, $q) {
    var _list;
    var _params;
    var deferredCalls = [];
    var querying = false;

    var service = {} as ITruckClassificationsHelperService;
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
            TruckClassifications.getList(params).then(function(response) {
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
        return TruckClassifications.one(ID).get();
    }

    function post(x) {
        var deferred = $q.defer();
        if (x) {
            TruckClassifications.post(x).then(function(response) {
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
            TruckClassifications.one(x.truckClassificationID).remove().then(function() {
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
        if (x && x.truckClassificationID) {
            TruckClassifications.one(x.truckClassificationID).patch(x).then(function() {
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

interface ITruckClassificationsHelperService {
    getList(params): ng.IPromise<any[]>;
    clearCache(): void;
    get(ID): ng.IPromise<any>;
    post(x): ng.IPromise<any>;
    remove(x): ng.IPromise<any>;
    patch(x): ng.IPromise<any>;
}

export {
    ITruckClassificationsHelperService,
};