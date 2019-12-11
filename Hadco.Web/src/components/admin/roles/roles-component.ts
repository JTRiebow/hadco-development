/// <reference path="../../../../node_modules/css-element-queries/src/ResizeSensor.d.ts" />

import * as angular from 'angular';

const ResizeSensor = require('css-element-queries/src/ResizeSensor');

import './roles.scss';
import { IRole, IRoleAuthActivityMap, IRoleAuthService, IRoleAuthActivity, AugmentedRoleAuthActivity } from './role-auth-service';
import { IActivityService } from '../../permissions/activity-service';
import { IPermissionService } from '../../permissions/permission-service';

import * as template from './roles.html';

angular
    .module('roleModule')
    .component('htRoles', {
        bindings: {
            // bindings
        },
        controller: getController(),
        controllerAs: 'vm',
        template,
    });

function getController() {
    class RolesController {
        public roles = [];
        public search = '';
        public pagination = {
            itemsPerPage: 10,
            totalItems: this.roles.length,
            currentPage: 1,
        };
        public departments = [];
        public activities = [];
        public roleAuthActivities = {} as IRoleAuthActivityMap;
        public horizontalScrollPadding = 0;
        public verticalScrollPadding = 0;
        public activeRow: number;
        public activeRole: IRole;
        public currentRoleAuthKey: string;
        public loading = false;
        public onEditFormRendered;
        
        private roleActivityElem: HTMLElement;
        private activityElem: HTMLElement;
        private roleElem: HTMLElement;
        private formElem: HTMLElement;
        private resizeSensors = {
            width: {} as { [key: string]: any },
            height: {} as { [key: string]: any },
            scroll: null,
        };
        private shiftIsPressed = false;
        private hasEditFormRendered = false;
        private subTemplateRendered: Promise<any>;
        private scrollLeft;
        
        constructor(
            private $scope: ng.IScope,
            private $location: ng.ILocationService,
            private $element: ng.IAugmentedJQuery,
            private $timeout: ng.ITimeoutService,
            private $mdDialog: ng.material.IDialogService,
            private Pagination,
            private NotificationFactory,
            private Roles,
            private ActivityService: IActivityService,
            private RoleAuthService: IRoleAuthService,
            private DepartmentsHelper,
            private PermissionService: IPermissionService,
        ) {}
        
        public $onInit() {
            this.PermissionService.redirectIfUnauthorized('editPermissions');
            
            this.updateSubTemplatePromise();
            
            this.roleActivityElem = this.$element[0].querySelector('.role-activities');
            this.activityElem = this.$element[0].querySelector('.activities');
            this.roleElem = this.$element[0].querySelector('.roles');
            
            this.search = this.$location.search().search;
            this.pagination.currentPage = +this.$location.search().page || 1;
            
            this.loading = true;
            
            Promise.all([
                this.ActivityService.getAuthActivities(),
                this.RoleAuthService.getRoleAuthActivities(),
                this.pageChanged(this.pagination.currentPage),
                this.DepartmentsHelper.getList(),
            ])
            .then(([activities, roleAuthMap, roles, departments]) => {
                this.activities = activities;
                this.departments = departments;
                
                activities.forEach(activity => {
                    roles.forEach(role => {
                        const key = `activity-${activity.authActivityID}-role-${role.roleId}`;
                        
                        if (!roleAuthMap[key]) {
                            roleAuthMap[key] = {} as IRoleAuthActivity;
                        }
                    });
                });
                
                this.roleAuthActivities = roleAuthMap;
            })
            .catch(err => {})
            .then(() => this.loading = false);
            
            this.$timeout(() => {
                this.registerScrollListeners();
                this.registerResizeSensors();
            });

            this.scrollLeft = 0;
        }
        
        public $onDestroy() {
            this.deregisterScrollListeners();
        }
        
        public setActiveRow(id: number, isOnlyActivity = false) {
            if (this.activeRow == id) {
                return this.resetActiveRows();
            }
            
            this.activeRow = id;
            
            if (!this.activeRole) {
                return this.setActiveRole(this.roles[0], id);
            }
            
            this.updateSubTemplatePromise();
            
            this.updateRoleAuthKey();
        }
        
        public setActiveRole(role: IRole, activityId: number, e?) {
            if (this.activeRole != role || this.activeRow != activityId) {
                this.activeRole = role;
                
                this.updateRoleAuthKey();
                
                return requestAnimationFrame(async () => {
                    if (this.activeRole) {
                        await this.subTemplateRendered;
                        
                        setTimeout(() => {
                            this.selectToggleSwitch();
                        }, 500);
                    }
                });
            }
            
            if (e) {
                e.stopPropagation();
            }
            
            this.resetActiveRows();
        }
        
        public toggleRoleAuth(roleAuth: AugmentedRoleAuthActivity) {
            
            const roleAuthKey = this.currentRoleAuthKey;
            
            if (roleAuth.active && !roleAuth.id) {
                roleAuth = {
                    authActivityID: roleAuth.authActivityID,
                    roleID: this.activeRole.roleId,
                    ownDepartments: true,
                    allDepartments: false,
                    departmentIds: [],
                    affectedDepartments: 'own',
                    specificDepartments: [],
                    ...roleAuth,
                };
                
                return this.RoleAuthService
                    .enableRoleAuthActivity(roleAuth)
                    .then(savedRoleAuth => {
                        this.roleAuthActivities[roleAuthKey] = {
                            ...roleAuth,
                            ...savedRoleAuth,
                        };
                    })
                    .catch(() => {
                        roleAuth.active = false;
                        
                        this.roleAuthActivities[roleAuthKey] = roleAuth;
                    });
            }
            else {
                return this.RoleAuthService
                    .disableAuthActivity(this.roleAuthActivities[roleAuthKey].roleAuthActivityID)
                    .then(() => {
                        roleAuth.active = false;
                        
                        delete roleAuth.roleAuthActivityID;
                        
                        this.roleAuthActivities[roleAuthKey] = roleAuth;
                    })
                    .catch(err => {
                        roleAuth.active = true;
                    });
            }
        }
        
        public log(data) {
            console.log(data);
        }
        
        public showBulkEditModal(role: IRole) {
            this.$mdDialog.show({
                controller: angular.noop,
                controllerAs: 'vm',
                bindToController: true,
                locals: {
                    parent: this,
                    role,
                    departments: this.departments,
                    tempRoleAuth: { roleID: role.roleId },
                },
                template: `
                    <div class="bulk-edit-role-permissions-modal">
                        <md-button
                            class="md-icon-button h-modal-dismiss"
                            ng-click="vm.parent.$mdDialog.cancel()"
                            aria-label="Close modal without saving"
                        >
                            <md-icon class="fa fa-times"></md-icon>
                        </md-button>
                        
                        <div class="h-modal-content no-bottom-padding">
                            <h2 class="h-modal-heading">Modify all permissions for the {{ vm.role.name }} role</h2>
                            
                            <configure-role-permission
                                role="vm.role"
                                departments="vm.parent.departments"
                                role-auth="vm.tempRoleAuth"
                                on-changes="vm.tempRoleAuth = roleAuth"
                                disable="vm.parent.loading"
                            ></configure-role-permission>
                        </div>
                        
                        <div class="h-modal-controls">
                            <md-button ng-click="vm.parent.$mdDialog.hide()">Cancel</md-button>
                            
                            <md-button
                                class="md-raised md-primary"
                                ng-click="vm.parent.$mdDialog.hide(vm.tempRoleAuth)"
                            >
                                Submit
                            </md-button>
                        </div>
                    </div>
                `,
                escapeToClose: true,
                clickOutsideToClose: true,
                ariaLabel: `modify all activities for ${role.name}`,
                onComplete: () => {
                    this.selectToggleSwitch(document.querySelector<HTMLElement>('.bulk-edit-role-permissions-modal'));
                },
            })
                .then(roleAuth => {
                    if (!roleAuth) return;
                    
                    this.loading = true;
                    
                    const auths = this.RoleAuthService
                        .filterRoleAuthActivitiesByRole(this.roleAuthActivities, role)
                        .filter(r => r.roleAuthActivityID);

                    this.RoleAuthService
                        .bulkUpdateRoleAuthActivities(auths, roleAuth, role)
                        .subscribe((rAuth: IRoleAuthActivity) => {
                            const key = `activity-${ rAuth.authActivityID }-role-${ rAuth.roleID }`;
                            
                            if (!rAuth.roleAuthActivityID) {
                                this.roleAuthActivities[key] = {
                                    active: false,
                                };
                            }
                            else {
                                this.roleAuthActivities[key] = {
                                    ...this.roleAuthActivities[key],
                                    ...rAuth,
                                };
                            }
                        })
                        .then(result => {
                            if (result.failure.length) {
                                this.NotificationFactory
                                    .error(`
                                        Some or all of the permissions could not be saved.  Check the table to see which ones 
                                        need help.
                                    `);
                            }
                            else if (result.success.length) {
                                this.NotificationFactory
                                    .success('Permissions saved successfully.')
                            }
                            
                            if (!result.all.length) {
                                this.NotificationFactory.warning('No updates needed to be made');
                                
                                requestAnimationFrame(() => this.$scope.$apply());
                            }
                            
                            this.loading = false;
                        });
                })
                .catch(angular.noop);
        }
        
        public handleRoleAuthChange(roleAuth: AugmentedRoleAuthActivity, applicableKey = this.currentRoleAuthKey) {
            if (this.areRoleAuthsDifferent(this.roleAuthActivities[applicableKey], roleAuth)) {
                if (typeof roleAuth.authActivityID != 'number') {
                    roleAuth.authActivityID = this.activeRow;
                }

                this.roleAuthActivities[applicableKey].loading = true;
                
                if (this.roleAuthActivities[applicableKey].active != roleAuth.active) {
                    return this.toggleRoleAuth(roleAuth) 
                    .then(() => this.roleAuthActivities[applicableKey].loading = false);
                    
                }
                
                this.RoleAuthService.updateRoleAuthActivity(roleAuth)
                    .then(auth => {
                        this.roleAuthActivities[applicableKey] = {
                            ...roleAuth,
                            ...auth,
                        };
                    })
                    .then(() => this.roleAuthActivities[applicableKey].loading = false);
            }
        }
        
        private selectToggleSwitch(insideElem: HTMLElement|Document = document) {
            const daSwitch = insideElem.querySelector<HTMLElement>('md-switch');
            
            daSwitch.focus();
            daSwitch.classList.add('md-focused');
        }
        
        private updateRoleAuthKey() {
            this.currentRoleAuthKey = `activity-${ this.activeRow }-role-${ this.activeRole.roleId }`;
        }
        
        private resetActiveRows() {
            this.activeRow = null;
            this.activeRole = null;
        }
        
        private pageChanged(page) {
            return this.Roles
                .getList({
                    skip: this.Pagination.skip(page, this.pagination.itemsPerPage),
                    orderBy: 'name',
                })
                .then(response => {
                    this.roles = response;
                    this.pagination.totalItems = response.meta.totalResultCount;
                    
                    return response;
                });
        }

        public setEditWrapperPosition(scrollLeft?: number): void {
            if (scrollLeft) {
                this.scrollLeft = scrollLeft;
            }

            const editWrapper = this.$element.find('.edit-wrapper');
            if (editWrapper.length) {
                editWrapper[0].style.left = `${this.scrollLeft}px`;
            }
        }
        
        private roleActivityScrollListener = (e: MouseEvent) => {
            // Sets the left style of the editing component to avoid using position: sticky
            this.setEditWrapperPosition(e.srcElement.scrollLeft);
            
            this.activityElem.scrollTop = (e.target as HTMLElement).scrollTop;
            this.roleElem.scrollLeft = (e.target as HTMLElement).scrollLeft;
        };

        private activityScrollListener = (e: WheelEvent) => {
            if (this.canScroll(e)) {
                e.preventDefault();
                e.stopPropagation();
                
                requestAnimationFrame(() => {
                    if (e.deltaY && e.shiftKey) {
                        this.roleActivityElem.scrollBy({
                            left: e.deltaY,
                            behavior: 'auto',
                        });
                    }
                    else if (e.deltaY) {
                        this.roleActivityElem.scrollBy({
                            top: e.deltaY,
                            behavior: 'auto',
                        });
                        // this.roleActivityElem.scrollTop = this.roleActivityElem.scrollTop + e.deltaY;
                    }
                    else if (e.deltaX) {
                        this.roleActivityElem.scrollBy({
                            left: e.deltaX,
                            behavior: 'auto',
                        });
                        // this.roleActivityElem.scrollLeft = this.roleActivityElem.scrollLeft + e.deltaX;
                    }
                });
            }
        };
        
        private getHorizontalCellResizeListener = (id: string) => {
            return () => {
                // document.querySelector(`#${id.split('-').slice(0, 2).join('-')}`).
            };
        };
        
        private getVerticalCellResizeListener = (elem: HTMLElement) => {
            return () => {
                const id = elem.dataset.activity;
                
                document.querySelector<HTMLElement>(`#activity-${id}`).style.height = elem.offsetHeight + 'px';
            };
        };
        
        private roleActivityResizeListener = () => {
            this.verticalScrollPadding = this.roleActivityElem.offsetHeight - this.roleActivityElem.clientHeight;
            this.horizontalScrollPadding = this.roleActivityElem.offsetWidth - this.roleActivityElem.clientWidth;
            
            this.updateFormWidth();
            
            this.$scope.$apply();
        };
        
        private registerResizeSensors() {
            requestAnimationFrame(() => {
                this.resizeSensors.scroll = new ResizeSensor(this.roleActivityElem, this.roleActivityResizeListener);
            });
        }
        
        private registerScrollListeners() {
            this.roleActivityElem.addEventListener('scroll', this.roleActivityScrollListener, false);
            this.activityElem.addEventListener('wheel', this.activityScrollListener, false);
        }
        
        private deregisterScrollListeners() {
            this.roleActivityElem.removeEventListener('scroll', this.roleActivityScrollListener, false);
            this.activityElem.removeEventListener('wheel', this.activityScrollListener, false);
        }
        
        private registerVerticalResizeListeners(id) {
            if (id > -1) {
                requestAnimationFrame(() => {
                    const activity = document.querySelector<HTMLElement>(`.activity-row.resize-me[data-activity="${id}"]`);
                    
                    this.getVerticalCellResizeListener(activity)();
                    
                    this.resizeSensors.height[id] = new ResizeSensor(activity, this.getVerticalCellResizeListener(activity));
                });
                
                return;
            }
            
            let leftovers = Object.keys(this.resizeSensors.height);
            
            [].forEach.apply(document.querySelectorAll<HTMLElement>('.activity-row.resize-me'), (elem: HTMLElement) => {
                const index = elem.dataset.activity;
                
                if (!this.resizeSensors.height[index]) {
                    const handler = this.getVerticalCellResizeListener(elem)
                    this.resizeSensors.height[index] = new ResizeSensor(elem, handler);
                    
                }
            });
            
            leftovers.forEach(k => delete this.resizeSensors.height[k]);
        }
        
        // The following two methods are ignored due to the fact that addition of roles and activities aren't yet implemented
        // private addActivityToList() {
            
        // }
        
        // private addRoleToList() {
            
        // }
        
        private canScroll(e: WheelEvent) {
            return (this.roleActivityElem.scrollTop != 0 &&
                    e.deltaY < 0) ||
                (this.roleActivityElem.scrollTop + this.roleActivityElem.clientHeight != this.roleActivityElem.scrollHeight &&
                    e.deltaY > 0) ||
                (this.roleActivityElem.scrollLeft != 0 &&
                    (e.deltaX < 0 || (e.shiftKey && e.deltaY))) ||
                (this.roleActivityElem.scrollLeft + this.roleActivityElem.clientWidth != this.roleActivityElem.scrollWidth &&
                    (e.deltaX > 0 || (e.shiftKey && e.deltaY)));
        }
        
        private areRoleAuthsDifferent(original: AugmentedRoleAuthActivity, update: AugmentedRoleAuthActivity) {
            return (update.active && !original.roleAuthActivityID) ||
                (!update.active && original.roleAuthActivityID) ||
                (update.ownDepartments != original.ownDepartments) ||
                (update.allDepartments != original.allDepartments) ||
                update.departmentIds.length != original.departmentIds.length ||
                update.roleID != original.roleID ||
                (update.authActivityID ? update.authActivityID == original.authActivityID : false);
        }
        
        private updateSubTemplatePromise() {
            this.subTemplateRendered = new Promise((res, rej) => {
                this.onEditFormRendered = () => res();
            })
                .then(() => {
                    this.formElem = this.$element.find('.edit-wrapper')[0];
                    
                    this.updateFormWidth();
                });
        }
        
        private updateFormWidth() {
            if (this.formElem) {
                this.formElem.style.width = `${this.roleActivityElem.clientWidth}px`;
            }
        }
    }
    
    RolesController.$inject = [
        '$scope',
        '$location',
        '$element',
        '$timeout',
        '$mdDialog',
        'Pagination',
        'NotificationFactory',
        'Roles',
        'ActivityService',
        'RoleAuthService',
        'DepartmentsHelper',
        'PermissionService',
    ];
    
    return RolesController;
}
