import * as angular from 'angular';
import * as _ from 'lodash';

angular
    .module('adminModule')
    .factory('CategoriesHelper', CategoriesHelper);

CategoriesHelper.$inject = [ 'Categories', '$q' ];

function CategoriesHelper(Categories, $q) {
    var _list;
    var _params;
    var deferredCalls = [];
    var querying = false;

    var service = {} as ICategoriesHelperService;
    
    service.getHadcoShopList = getHadcoShopList;
    service.clearCache = clearCache;

    return service;

    function getHadcoShopList(params) {
        var deferred = $q.defer();

        if (_list && (JSON.stringify(_params) === JSON.stringify(params))) {
            deferred.resolve(_list);
        }
        else if (querying) {
            deferredCalls.push(deferred);
        }
        else {
            querying = true;
            Categories.one('HadcoShop').getList(params).then(function(response) {
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
    function clearCache() {
        _list = null;
    }
}

interface ICategoriesHelperService {
    getHadcoShopList(params): ng.IPromise<any[]>;
    clearCache(): void;
}

export {
    ICategoriesHelperService,
};