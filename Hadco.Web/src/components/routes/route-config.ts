import * as angular from 'angular';

angular
    .module('hadcoRouting')
    .config(routeConfig)
    // This is here because the routes aren't loading immediately when the page loads initially
    // It will cause the router to check the current route, and thus it becomes aware that it needs to load the route
    // Likely a race condition somehow that's related to this module/these routes being registered before the main module
    .run([ '$route', angular.noop ]);

routeConfig.$inject = [ '$routeProvider', '$locationProvider' ];

function routeConfig($routeProvider, $locationProvider) {
    $routeProvider
        .when('/human-resources/employees', {
            template: '<ht-users></ht-users>',
        })
        .when('/human-resources/employee/:userId', {
            template: '<ht-create-or-edit-user></ht-create-or-edit-user>',
        })
        .when('/human-resources/employee/:userId', {
            template: '<ht-create-or-edit-user></ht-create-or-edit-user>',
        })
        //#region AdminRoutes
        .when('/admin/downtime-reasons/:downtimeReasonId', {
            template: '<ht-create-or-edit-downtime-reasons></ht-create-or-edit-downtime-reasons>',
        })
        .when('/admin/downtime-reasons', {
            template: '<ht-downtime-reasons></ht-downtime-reasons>',
        })
        .when('/admin/gps-settings', {
            template: '<ht-gps-settings></ht-gps-settings>',
        })
        .when('/admin/gps-settings/:departmentID', {
            template: '<ht-gps-selected-employees></ht-gps-selected-employees>',
        })
        .when('/:admin/job/:jobId', {
            template: '<ht-create-or-edit-job></ht-create-or-edit-job>',
        })
        .when('/admin/jobs', {
            template: '<ht-jobs></ht-jobs>',
        })
        .when('/admin/job/:jobId/phases', {
            template: '<ht-phases></ht-phases>',
        })
        .when('/admin/materials/:materialId', {
            template: '<ht-create-or-edit-material></ht-create-or-edit-material>',
        })
        .when('/admin/materials', {
            template: '<ht-materials></ht-materials>',
        })
        .when('/:supervisor/occurrences/:occurrenceId', {
            template: '<ht-create-or-edit-occurrence></ht-create-or-edit-occurrence>',
        })
        .when('/admin/occurrences', {
            template: '<ht-occurrences></ht-occurrences>',
        })
        .when('/admin/trucking-classifications/:listTypeId', {
            template: '<ht-create-or-edit-truck-classifications></ht-create-or-edit-truck-classifications>',
        })
        .when('/admin/trucking-classifications', {
            template: '<ht-truck-classifications></ht-truck-classifications>',
        })
        //#endregion AdminRoutes
        .when('/employee/:employeeId/department/:departmentId/day/:dayId/:timersTab?', {
            template: '<ht-employee-detailed></ht-employee-detailed>',
        })
        .when('/:supervisor/TMCrushing/report', {
            template: '<ht-tmcrushing-report></ht-tmcrushing-report>',
        })
        .when('/downloadTimers', {
            template: '<ht-download-timers></ht-download-timers>',
        })
        .when("/employee-search/:employeeID/:selectedWeek?", {
            template: '<ht-employee-search-results-page></ht-employee-search-results-page>',
            reloadOnSearch: false,
        })
        .when('/:employee/clock-in', {
            template: '<employee-clock-in></employee-clock-in>',
        })
        .when('/:employee/equipment/:id', {
            template: '<ht-employee-equipment></ht-employee-equipment>',
        })
        .when('/admin/roles/:roleId', {
            template: '<ht-create-or-edit-role></ht-create-or-edit-role>',
        })
        .when('/admin/roles', {
            template: '<ht-roles></ht-roles>',
        })
        .when('/:supervisor/:supervisorId/employees', {
            template: '<ht-supervisor-employees></ht-supervisor-employees>',
        })
        .when('/human-resources/supervisors', {
            template: '<ht-supervisor></ht-supervisor>',
        })
        .when('/:supervisor/superintendent', {
            template: '<ht-superintendent></ht-superintendent>',
        })
        .when('/:superintendent/foreman/:employeeId/department/:departmentId/day/:dayId', {
            template: '<ht-supervisor-timesheet></ht-supervisor-timesheet>',
        })
        .when('/:supervisorOrAccounting/timer/approval', {
            template: '<timer-approval></timer-approval>',
        })
        .when('/pricing', {
            template: '<ht-pricing></ht-pricing>',
        })
        .when('/reporting', {
            template: '<ht-reporting></ht-reporting>',
        })
        .when('/trucker-dailies', {
            template: '<ht-trucker-dailies></ht-trucker-dailies>',
        })
        .when('/:supervisor/units/:unitId', {
            template: '<ht-create-or-edit-unit></ht-create-or-edit-unit>',
        })
        .when('/:supervisor/units', {
            template: '<ht-units></ht-units>',
        })
        .otherwise({
            redirectTo: '/employee/clock-in',
        });
    
    $locationProvider.html5Mode(true);
}