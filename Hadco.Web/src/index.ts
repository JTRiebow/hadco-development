import 'jquery';
import * as angular from 'angular';

import './project-typings';

import './vendor';
import './components';

import './site.scss';

angular.module('hadcoApp', [
    'mm.foundation',
    // 'mm.foundation.offcanvas',
    'ngRoute',
    'ui.bootstrap',
    'ui.grid',
    'ngTouch',
    'ui.grid.autoResize',
    'ui.grid.selection',
    'ui.grid.exporter',
    'ui.grid.edit',
    'ui.grid.treeBase',
    'ui.grid.treeView',
    'ui.grid.rowEdit',
    'ui.grid.cellNav',
    'ui.grid.pinning',
    'ui.grid.pagination',
    'ui.grid.cellNav',
    'ui.grid.resizeColumns',
    'ui.grid.draggable-rows',
    'ui.grid.expandable',
    // 'ngTouch',
    'angular-loading-bar',
    'rgkevin.datetimeRangePicker',
    'vr.directives.slider',
    'ngAnimate',
    'ngAria',
    'xeditable',
    //'smart-table',
    'ngMaterial',
    'restangular',
    'angularUtils.directives.dirPagination',
    'oi.select',
    'mgcrea.ngStrap.timepicker',
    'permissionsModule',
    'authorizationModule',
    'sharedModule',
    'humanResourcesModule',
    //'notificationFactoryModule',
    'currentUserModule',
    'userModule',
    'employeeModule',
    'timerApprovalModule',
    'roleModule',
    'tmcrushingModule',
    //'equipmentModule',
    //'unitModule',
    //'timeSheetsModule',
    'employeeTimerModule',
    //'equipmentTypesModule',
    //'departmentsModule',
    'employeeTimecardsModule',
    'employeeJobEquipmentTimersModule',
    'employeeJobTimersModule',
    'equipmentTimersModule',
    //'categoriesModule',
    'jobTimersModule',
    'loadTimersModule',
    'downloadTimersModule',
    'truckingModule',
    //'BillTypesModule',
    'adminModule',
    'modalModule',
    'detailedApprovalModule',
    'pitsModule',
    'hadcoRouting',
    'configModule',
    'runOnceModule',
]);

import './ht-app-component';