import * as angular from 'angular';
import * as moment from 'moment';

angular.module('modalModule').controller('mapModalController', mapModalController);

mapModalController.$inject = [ '$modalInstance', 'mapData', 'employeeName' ];

function mapModalController($modalInstance, mapData, employeeName) {
    var vm = this;

    init();

    function init() {
        vm.mapData = mapData;
        vm.employeeName = employeeName;
        vm.mapData.date = moment(vm.mapData.timestamp).format("LLLL");
        vm.zoomLevel = 16;
        _getMap(vm.zoomLevel);
    }

    
    function _getMap(zoomLevel) {
        var pushpin = 'pushpin=' + vm.mapData.latitude + ',' + vm.mapData.longitude + ';66;&';
        vm.mapUrl = 'http://dev.virtualearth.net/REST/v1/Imagery/Map/Road?' + pushpin + 'zoomLevel=' + zoomLevel + '&declutter=1&key=AtynugkvBm592ahESde0X1n0a40X9FPuX-FNn0Ev1hgkMtV86fjDYjFp0_yQnO83';
    }

    vm.changeZoom = function() {
        if (vm.zoomLevel <= 12) {
            vm.zoomLevel = 16;
        }
        else {
            vm.zoomLevel = vm.zoomLevel - 1;
        }

        _getMap(vm.zoomLevel);
    };
    vm.close = function() {
        $modalInstance.dismiss();
    };


}
