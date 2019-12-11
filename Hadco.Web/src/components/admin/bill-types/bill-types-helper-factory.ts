import * as angular from 'angular';
import * as _ from 'lodash';

angular
    .module('adminModule')
    .factory('BillTypesHelper', BillTypesHelper);

BillTypesHelper.$inject = [ 'BillTypes', '$q' ];

function BillTypesHelper(BillTypes, $q) {
    var _list;
    var _params;
    var deferredCalls = [];
    var querying = false;

    var service = {} as IBillTypesHelperService;
    service.getList = getList;
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
            BillTypes.getList(params).then(function(response) {
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
        return BillTypes.one(ID).get();
    }
}

interface IBillTypesHelperService {
    getList(params): ng.IPromise<any[]>;
}

export {
    IBillTypesHelperService,
};