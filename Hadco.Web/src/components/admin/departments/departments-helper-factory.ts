import * as angular from 'angular';
import * as _ from 'lodash';

angular
    .module('adminModule')
    .factory('DepartmentsHelper', DepartmentsHelper);

DepartmentsHelper.$inject = [ 'Departments', '$q' ];

function DepartmentsHelper(Departments, $q) {
    var _list;
    var _params;
    var deferredCalls = [];
    var querying = false;

    var service = {} as IDepartmentsHelperService;
    service.getList = getList;
    service.clearCache = clearCache;
    service.get = get;
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
            Departments.getList(params).then(function(response) {
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
        return Departments.one(ID).get();
    }

    function patch(x) {
        var deferred = $q.defer();
        if (x && x.departmentID) {
            Departments.one(x.departmentID).patch(x).then(function() {
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

interface IDepartmentsHelperService {
    getList(params): ng.IPromise<any[]>;
    clearCache(): void;
    get(ID): ng.IPromise<any>;
    patch(trailer): ng.IPromise<any>;
}

export {
    IDepartmentsHelperService,
};