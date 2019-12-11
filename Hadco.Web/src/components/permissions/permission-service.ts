import * as angular from 'angular';
import * as $ from 'jquery';

import { IActivityService } from './activity-service';

angular
    .module('permissionsModule')
    .service('PermissionService', getService());

function getService() {
    class PermissionService implements IPermissionService {
        public csvPageActions = [
            'downloadJobTimersCsv',
            'downloadOccurrencesCsv',
            'downloadEmployeeTimecardsCsv',
            'downloadDiscrepanciesCsv',
            'downloadLoadTimersCsv',
            'downloadDowntimeCsv',
            'downloadEquipmentTimersCsv',
            'downloadEmployeeRolesCsv',
            'downloadEmployeeClockInsOutsCSV',
            'downloadEmployeeClockInsOutsCSV',
            'downloadNotesCSV',
            'downloadQuantitiesCSV',
        ];
        public timerApprovalPageActions = [
            'approveAsSupervisor',
            'approveAsBilling',
            'approveAsAccounting'
        ]
        
        private rootPermissionsApiPath = '/api/permissions';
        private myPermissions = [];
        private currentPagePermissions = [];
        
        private adminPagesActions = [
            'viewDowntimeReasons',
            'viewJobs',
            'viewMaterials',
            'viewGPSSettings',
            'viewOccurrences',
            'viewTruckingClassifications',
        ];
        private timerNavActions = [
            'viewAccountingTimers',
            'viewBillingTimers',
            'viewSupervisorTimers',
            'viewForemenTimesheets',
        ];
        private truckingNavActions = [
            'viewTruckerDailies',
            'viewTruckingPricing',
            'viewTruckingReporting',
        ];
        private hrNavActions = [
            'viewEmployeeList',
            'viewSupervisors',
        ];
        
        constructor(private $http: ng.IHttpService,
                    private $location: ng.ILocationService,
                    private ActivityService: IActivityService) {}
        
        public getMyPermissions() {
            return this.$http
                .get(this.rootPermissionsApiPath + '/activities/me')
                .then((response: ng.IHttpResponse<number[]>) => {
                    this.myPermissions = response.data;
                    this.currentPagePermissions = [];
                    
                    return [
                        ...this.myPermissions,
                    ];
                })
                .catch(err => {
                    throw err;
                });
        }

        public getManyDepartmentPermissions(ids: number[]) {
            this.currentPagePermissions = [];

            return Promise.all<number[]>(ids.map(this.getDepartmentPermissions.bind(this)))
                .then(responses => {
                    return this.currentPagePermissions = responses
                        .reduce((agg, r) => [
                            ...agg,
                            ...r,
                        ], []);
                });
        }
        
        public getDepartmentPermissions(id) {
            this.currentPagePermissions = [];
            
            return this.$http
                .get(this.rootPermissionsApiPath + '/activities?departmentId=' + id)
                .then((response: ng.IHttpResponse<number[]>) => {
                    this.currentPagePermissions = response.data;
                    
                    return response.data;
                })
                .catch(err => {
                    throw err;
                });
        }
        
        public getEmployeePermissions(id) {
            this.currentPagePermissions = [];
            
            return this.$http
                .get(this.rootPermissionsApiPath + '/employeeeditactivities?employeeID=' + id)
                .then((response: ng.IHttpResponse<number[]>) => {
                    this.currentPagePermissions = response.data;
                    
                    return response.data;
                })
                .catch(err => {
                    throw err;
                });
        }
        
        public getDepartmentPermissionBreakdown(identifiers: (string|number)[]) {
            identifiers = identifiers.map(id => typeof id == 'number' ? id : this.ActivityService.getActivityId(id));
            
            return this.$http
                .get(this.rootPermissionsApiPath + '/activitydepartments?' + $.param({ activityId: identifiers }))
                .then((response: ng.IHttpResponse<{ activityID: number; departmentIds: number[] }[]>) => {
                    return response.data;
                })
                .catch(err => {
                    throw err;
                });
        }
        
        public can = (identifier: string|number) => {
            if (typeof identifier == 'string') {
                identifier = this.ActivityService.getActivityId(identifier);
            }
            
            return this.myPermissions.includes(identifier) || this.currentPagePermissions.includes(identifier);
        };
        
        public canDoAny = (identifiers: (string|number)[]) => {
            const found = identifiers.find(this.can) || false;
            return typeof found != 'boolean';
        };
        
        public redirectIfUnauthorized(identifiers: number|string|(number|string)[]) {
            if (Array.isArray(identifiers) ? !this.canDoAny(identifiers) : !this.can(identifiers)) {
                this.$location.path('/employee/clock-in');
            }
        }
        
        public canAccessCSV = () => {
            return this.canDoAny(this.csvPageActions);
        };
        
        public canAccessAdmin = () => {
            return this.canDoAny(this.adminPagesActions);
        };
        
        public canSeeTimerNav = () => {
            return this.canDoAny(this.timerNavActions) || this.canAccessCSV();
        };
        
        public canSeeTruckingNav = () => {
            return this.canDoAny(this.truckingNavActions);
        };
        
        public canSeeHrNav = () => {
            return this.canDoAny(this.hrNavActions);
        };
    }
    
    PermissionService.$inject = ['$http', '$location', 'ActivityService'];
    
    return PermissionService;
}

interface IPermissionService {
    getMyPermissions(): ng.IPromise<number[]>;
    getDepartmentPermissions(id): ng.IPromise<number[]>;
    getManyDepartmentPermissions(ids: number[]): Promise<number[]>;
    getEmployeePermissions(id): ng.IPromise<number[]>;
    getDepartmentPermissionBreakdown(identifiers: (string|number)[]): ng.IPromise<{ activityID: number; departmentIds: number[] }[]>;
    can(identifier: string|number): boolean;
    canDoAny(identifiers: (string|number)[]): boolean;
    canAccessCSV(): boolean;
    redirectIfUnauthorized(identifiers: number|string|(number|string)[]): void;
    canAccessAdmin(): boolean;
    canSeeTimerNav(): boolean;
    canSeeTruckingNav(): boolean;
    canSeeHrNav(): boolean;
    timerApprovalPageActions: string[]
}

export {
    IPermissionService,
};