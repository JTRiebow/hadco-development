import * as angular from 'angular';

import { IAuthActivity, IActivityService } from "../../permissions/activity-service";
import { IRequestQueue, IRequestQueueService } from '../../Shared/request-queue-service';

angular
    .module('roleModule')
    .service('RoleAuthService', getService());

function getService() {
    class RoleAuthService implements IRoleAuthService {
        private roleAuthBaseUrl = '/api/roleauthactivities';
        private allowedFields = ['roleID', 'authActivityID', 'ownDepartments', 'allDepartments', 'departmentIds'];
        private departments = [];
        private bulkRoleUpdateMap = {} as {
            [key: string]: {
                all: Promise<IRoleAuthActivity>[];
                success: IRoleAuthActivity[];
                fail: IRoleAuthActivity[];
            };
        };
        private updateMapPrototype;
        private activities = [] as IAuthActivity[];
        
        constructor(
            private $http: ng.IHttpService,
            private DepartmentsHelper,
            private ActivityService: IActivityService,
            private RequestQueueService: IRequestQueueService,
        ) {
            this.ActivityService.getAuthActivities()
                .then(activities => {
                    this.activities = activities;
                });
            
            
            this.updateMapPrototype = {
                subscribe(handler) {
                    if (!this._handlers) {
                        this._handlers = [];
                    }
                    
                    this._handlers.push(handler);
                },
                listenForComplete() {
                    return this._promise;
                },
                unsubscribe(handler) {
                    this._handlers = this._handlers.filter(h => h != handler);
                },
            };
        }
        
        public getRoleAuthActivities() {
            return this.DepartmentsHelper.getList()
                .then(departments => {
                    this.departments = departments
                    return this.$http.get(this.roleAuthBaseUrl + '/all')
                })
                // .catch(err => {if (err.status == 405) return {data: []}; throw err})
                .then(
                    (response: ng.IHttpResponse<IRoleAuthActivity[]>) => response.data
                        .reduce((acc, raa) => {
                            const departmentMap = raa.departmentIds.map(i => this.departments.find(d => d.departmentID == i));
                            
                            return {
                                ...acc,
                                [`activity-${raa.authActivityID}-role-${raa.roleID}`]: {
                                    ...raa,
                                    active: true,
                                    affectedDepartments: raa.allDepartments ? 'all' : raa.ownDepartments ? 'own' : 'some',
                                    departments: departmentMap,
                                    specificDepartments: departmentMap.slice(),
                                } as AugmentedRoleAuthActivity, 
                            };
                        }, {} as IRoleAuthActivityMap)
                )
                .catch(err => {console.warn(err); throw err});
        }
        
        public enableRoleAuthActivity(roleAuth: AugmentedRoleAuthActivity) {
            return this.$http
                .post(this.roleAuthBaseUrl, this.reduceToAllowedFields(roleAuth))
                .then((response: ng.IHttpResponse<IRoleAuthActivity>) => response.data)
                .catch(err => {console.warn(err); throw err});
        }
        
        public disableAuthActivity(roleAuthId: number) {
            return this.$http
                .delete(this.roleAuthBaseUrl + '?RoleAuthActivityID=' + roleAuthId)
                .then(response => response.data)
                .catch(err => {console.warn(err); throw err});
        }
        
        public updateRoleAuthActivity(roleAuth: AugmentedRoleAuthActivity) {
            return this.$http
                .patch(this.roleAuthBaseUrl + '/' + roleAuth.roleAuthActivityID, this.reduceToAllowedFields(roleAuth))
                .then((response: ng.IHttpResponse<IRoleAuthActivity>) => response.data)
                .catch(err => {console.warn(err); throw err});
        }
        
        public bulkUpdateRoleAuthActivities(roleAuthActivities: AugmentedRoleAuthActivity[], modelRoleAuth: AugmentedRoleAuthActivity, role: IRole) {
            const enabled = modelRoleAuth.active;
            
            const queueActions = this.activities.map(activity => {
                const associatedRoleAuth = roleAuthActivities.find(r => r.authActivityID == activity.authActivityID);
                const update = {
                    ...(associatedRoleAuth || {}),
                    ...modelRoleAuth,
                    roleID: role.roleId,
                    authActivityID: activity.authActivityID,
                };
                
                if (associatedRoleAuth) {
                    if (enabled) {
                        return () => this.updateRoleAuthActivity(update).then(u => ({ ...update, ...u }));
                    }
                    
                    return () => this.disableAuthActivity(update.roleAuthActivityID).then(() => ({ roleID: role.roleId, authActivityID: activity.authActivityID }));
                }
                
                return enabled && (() => this.enableRoleAuthActivity(update).then(u => ({ ...update, ...u })));
            }).filter(Boolean);
            
            return this.RequestQueueService.create<IRoleAuthActivity>(queueActions);
        }
        
        public filterRoleAuthActivitiesByRole(map: IRoleAuthActivityMap, role: IRole) {
            return Object.keys(map)
                .filter(k => {
                    const [ roleId ] = k.match(/\d+$/);
                    return +roleId == role.roleId;
                })
                .map(k => map[k]);
        }
        
        private reduceToAllowedFields(roleAuth) {
            return Object.keys(roleAuth).reduce((o, k) => ({ ...o, ...(this.allowedFields.includes(k) ? { [k]: roleAuth[k] } : {}) }), {});
        }
    }
    
    RoleAuthService.$inject = [
        '$http',
        'DepartmentsHelper',
        'ActivityService',
        'RequestQueueService',
    ];
    
    return RoleAuthService;
}

interface IRoleAuthActivity {
    roleAuthActivityID?: number;
    id?: number;
    roleID?: number;
    role?: IRole;
    authActivityID?: number;
    authActivity?: IAuthActivity;
    ownDepartments?: boolean;
    allDepartments?: boolean;
    departmentIds?: number[];
}

type AugmentedRoleAuthActivity = IRoleAuthActivity & {
    active?: boolean;
    affectedDepartments?: 'all'|'own'|'some';
    departments?: any[];
    specificDepartments?: any[];
    loading?: boolean;
    [key: string]: any;
};

interface IRoleAuthService {
    getRoleAuthActivities(): ng.IPromise<IRoleAuthActivityMap>;
    enableRoleAuthActivity(roleAuth: AugmentedRoleAuthActivity): ng.IPromise<IRoleAuthActivity>;
    disableAuthActivity(roleAuthId: number): ng.IPromise<any>;
    updateRoleAuthActivity(roleAuth: AugmentedRoleAuthActivity): ng.IPromise<IRoleAuthActivity>;
    filterRoleAuthActivitiesByRole(map: IRoleAuthActivityMap, role: IRole): AugmentedRoleAuthActivity[];
    bulkUpdateRoleAuthActivities(roleAuthActivities: AugmentedRoleAuthActivity[], modelRoleAuth: AugmentedRoleAuthActivity, role: IRole): IRequestQueue<IRoleAuthActivity>;
}

interface IRole {
    roleId?: number;
    id?: number;
    name?: string;
}

interface IRoleAuthActivityMap {
    [key: string]: AugmentedRoleAuthActivity;
}

export {
    IRoleAuthActivity,
    AugmentedRoleAuthActivity,
    IRoleAuthService,
    IRole,
    IRoleAuthActivityMap,
};