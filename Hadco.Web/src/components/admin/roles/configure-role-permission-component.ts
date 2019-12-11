import * as angular from 'angular';

import './configure-role-permission.scss';

import { IAuthActivity } from '../../permissions/activity-service';

import * as template from './configure-role-permission.html';
import { AugmentedRoleAuthActivity } from './role-auth-service';

angular
    .module('roleModule')
    .component('configureRolePermission', {
        bindings: {
            activity: '<',
            departments: '<',
            disable: '<',
            input: '<roleAuth',
            role: '<',
            emitChanges: '&onChanges',
            emitRendered: '&onRendered',
        },
        controller: getController(),
        controllerAs: 'vm',
        template,
    });

function getController() {
    class ConfigureRolePermissionController {
        public roleAuth: AugmentedRoleAuthActivity = {};
        public emitChanges;
        public emitRendered;
        public departments: any[];
        public activity: IAuthActivity;
        
        private originalEmitChanges;
        
        constructor(private $timeout: ng.ITimeoutService) {}
        
        public $onInit() {
            this.departments = this.departments || [];
            
            this.originalEmitChanges = this.emitChanges;
            this.emitChanges = () => this.originalEmitChanges({ roleAuth: this.roleAuth });
            
            this.$timeout(() => this.emitRendered());
        }
        
        public $onChanges(changes) {
            if (changes.input && changes.input.currentValue) {
                const { departments = [], specificDepartments = [] } = changes.input.currentValue;
                
                this.roleAuth = {
                    active: false,
                    allDepartments: false,
                    ownDepartments: true,
                    ...changes.input.currentValue,
                    departments: [
                        ...departments,
                    ],
                    specificDepartments: [
                        ...specificDepartments,
                    ],
                } as AugmentedRoleAuthActivity;
            }
            if (changes.activity && changes.activity.currentValue) {
                const activityName = changes.activity.currentValue.name as string;
                this.activity = {
                    ...changes.activity.currentValue,
                    name: activityName[0].toLowerCase() + activityName.slice(1),
                };
            }
        }
        
        public updateDepartment() {
            const { roleAuth } = this;
            
            if (!roleAuth.departments) {
                roleAuth.departments = [];
            }
            
            requestAnimationFrame(() => {
                const allDepartments = roleAuth.affectedDepartments == 'all';
                const ownDepartments = roleAuth.affectedDepartments == 'own';
                const departmentIds = roleAuth.departments.map(d => d.departmentID);
                
                this.roleAuth = {
                    ...roleAuth,
                    allDepartments,
                    ownDepartments,
                    departmentIds,
                };
                
                this.emitChanges();
            });
        }
        
        public addDepartment(department) {
            if (!this.roleAuth.departments) {
                this.roleAuth.departments = [];
            }
            
            this.roleAuth.departments.push(department);
            
            this.updateDepartment();
        }
        
        public removeDepartment(department) {
            this.roleAuth.departments = this.roleAuth
                    .departments.filter(d => department.departmentID != d.departmentID);
            
            this.updateDepartment();
        }
        
        public filterDepartments(query: string) {
            return this.departments.filter(this.createDepartmentFilter(query));
        }
        
        private createDepartmentFilter(query: string) {
            return department => {
                const lowerCase = department.name.toLowerCase().includes(query.toLowerCase());
                const isNotFound = !this.roleAuth.departments.find(d => d.departmentID == department.departmentID);
                
                return lowerCase && isNotFound;
            };
        }
    }
    
    ConfigureRolePermissionController.$inject = ['$timeout'];
    
    return ConfigureRolePermissionController;
}