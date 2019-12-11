import * as angular from 'angular';

angular
    .module('permissionsModule')
    .service('ActivityService', getService());

function getService() {
    class ActivityService implements IActivityService {
        private activityMap: ActivityMap = {};
        private rootActivityApiPath = '/api/permissions';
        private fetchingLists = {} as { [key: string]: ng.IPromise<any> };
        
        constructor(private $http: ng.IHttpService) {
            this.getActivities();
        }
        
        public getActivities() {
            return this.$http
                .get(this.rootActivityApiPath)
                .then((response: ng.IHttpResponse<ActivityMap>) => {
                    this.activityMap = response.data;
                    
                    return {
                        ...this.activityMap,
                    };
                })
                .catch(err => {
                    console.warn(err);
                    throw err;
                });
        }
        
        public getActivityId(name: string) {
            return this.activityMap[name];
        }
        
        public getAuthActivities() {
            if (!this.fetchingLists.getAuthActivities) {
                this.fetchingLists.getAuthActivities = this.$http
                    .get('/api/authactivities')
                    .then(({ data: activities }) => {
                        return activities;
                    })
                    .catch(err => {
                        console.warn(err);
                        this.fetchingLists.getAuthActivities = undefined;
                        throw err;
                    });
            }
            
            return this.fetchingLists.getAuthActivities;
        }
    }
    
    ActivityService.$inject = ['$http'];
    
    return ActivityService;
}

type ActivityMap = {[name: string]: number} & {[id: number]: string};

interface IActivityService {
    getActivities(): ng.IPromise<ActivityMap>;
    getActivityId(name: string): number;
    getAuthActivities(): ng.IPromise<IAuthActivity[]>;
}

interface IAuthActivity {
    authActivityID?: number;
    id?: number;
    authSectionID?: number;
    authSection?: IAuthSection;
    name?: string;
    description?: string;
}

interface IAuthSection {
    id?: number;
    name?: string;
    description?: string;
}

export {
    ActivityMap,
    IActivityService,
    IAuthActivity,
    IAuthSection,
};