import * as angular from 'angular';
import * as _ from 'lodash';
import { IPermissionService } from '../permissions/permission-service';

angular
    .module('currentUserModule')
    .factory('CurrentUser', CurrentUserFactory);

CurrentUserFactory.$inject = [
    'Restangular',
    'NotificationFactory',
    '$q',
    '$http',
    'Users',
    'PermissionService',
];

function CurrentUserFactory(
    Restangular,
    NotificationFactory,
    $q,
    $http,
    Users,
    PermissionService: IPermissionService,
) {
    var service = {
        getDepartmentIds,
    } as ICurrentUserService;

    // Set id fields used with this model
    Restangular.configuration.routeToIdMappings['users'] = 'UserID';

    var deferredCurrentUser;
    var currentUser;

    service.extractToken = function(response) {
        return response.data.access_token || undefined;
    };

    service.setToken = function(token) {
        // Set correct headers for authentication
        var newDefaultHeaders = { 'Authorization': 'Bearer ' + token };
        Restangular.setDefaultHeaders(newDefaultHeaders);
        // console.info("Set Authorization in header to: Bearer " + token);
        localStorage['token'] = token;
        return newDefaultHeaders;
    };

    service.postCredentials = function(url, credentials) {
        // Credentials need extra data
        credentials.grant_type = 'password';
        credentials.scope = 'read';

        var data = '';
        var delimiter = '';

        _.each(Object.keys(credentials), function(k) {
            data += delimiter + encodeURIComponent(k) + '=' + encodeURIComponent(credentials[k]);
            delimiter = '&';
        });

        return $http({
            method: 'POST',
            url: url,
            data: data,
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        });
    };

    // Get a promise that will return the current user or fail
    // Result is cached to avoid requesting user information many times
    // If updated user information is needed, clear the current user and the
    // next requrest will fetch it again.
    // get :: () -> Promise CurrentUser
    service.get = function() {
        if (deferredCurrentUser) {
            return deferredCurrentUser.promise;
        }

        deferredCurrentUser = $q.defer();

        fetchCurrentUserFromApi()
            .then(currentUser => {
                if (currentUser) {
                    service.setLoggedIn(true);
                }
                deferredCurrentUser.resolve(currentUser);
            }, () => {
                NotificationFactory.error('There was an error fetching the current user data.');
            });

        return deferredCurrentUser.promise;
    };

    // Returns a promise that will indicate if the user is authorized for this site
    // isAuthorized :: () -> Promise Bool
    service.isAuthorized = function() {
        var deferred = $q.defer();

        isAuthorizedForSite().then((authorized) => {
            deferred.resolve(authorized);
        }, (error) => {
            NotificationFactory.error('There was an error checking authorization status.');
        });

        return deferred.promise;
    };

    var _loggedIn;

    service.loggedIn = function() {
        return _loggedIn;

    };

    // setLoggedIn :: Bool? -> ()
    service.setLoggedIn = function(loggedIn) {
        _loggedIn = loggedIn;
    };

    // Reset back to initial state
    service.clear = function() {
        _loggedIn = undefined;
        deferredCurrentUser = undefined;
        resetToInitial();
    };

    var _isSystemAdmin;
    var _isAccounting;
    var _isBilling;
    var _isManager;
    var _isSupervisor;
    var _isUser;
    var _isTruckingRole;
    var _isForemanRole;
    var _isTruckingReports;
    var _isForeman;
    var _isSuperintendent;

    service.roles = {
        isSystemAdmin: function() {
            return _isSystemAdmin;
        },

        isAccounting: function() {
            return _isAccounting;
        },

        isBilling: function() {
            return _isBilling;
        },

        isManager: function() {
            return _isManager;
        },

        isSupervisor: function() {
            return _isSupervisor;
        },

        isUser: function() {
            return _isUser;
        },
        isTruckingRole: function() {
            return _isTruckingRole;
        },
        isTruckingReports: function() {
            return _isTruckingReports;
        },
        isForemanRole: function() {
            return _isForemanRole;
        },
        isSuperintendent: function() {
            return _isSuperintendent;
        },
    };

    var _isConcrete;
    var _isDevelopment;
    var _isResidential;
    var _isTrucking;
    var _isMobileMechanic;
    var _isFrontOffice;
    var _isTMCrushing;
    var _isShop;

    service.departments = {
        isConcrete: function() {
            return _isConcrete;
        },
        isDevelopment: function() {
            return _isDevelopment;
        },
        isResidential: function() {
            return _isResidential;
        },
        isTrucking: function() {
            return _isTrucking;
        },
        isMobileMechanic: function() {
            return _isMobileMechanic;
        },
        isFrontOffice: function() {
            return _isFrontOffice;
        },
        isTMCrushing: function() {
            return _isTMCrushing;
        },
        isShop: function() {
            return true;
        },
    };
    // API specific implementations

    // Clear anything that needs to be cleared that is api specific and should be cleared when 
    // current user info is cleared (e.g. Role information and such)
    // resetToInitial :: () -> ()
    var resetToInitial = function() {
        _isUser = false;
        _isSupervisor = false;
        _isManager = false;
        _isBilling = false;
        _isAccounting = false;
        _isSystemAdmin = false;
        _isTruckingReports = false;
        _isTruckingRole = false;
        _isForemanRole = false;
        _isSuperintendent = false;
        _isConcrete = false;
        _isDevelopment = false;
        _isResidential = false;
        _isTrucking = false;
        _isMobileMechanic = false;
        _isFrontOffice = false;
        _isTMCrushing = false;
        _isShop = false;
    };

    // Make whatever calls are needed to get the current user object and return it.
    // fetchCurrentUserFromApi :: () -> Promise CurrentUser
    var fetchCurrentUserFromApi = function() {
        return Users.one('me').get().then(function(response) {
            currentUser = response;
            if (currentUser) {
                //Get image from gravatar (uses placeholder if not found)
                //If image url is ever added to system we will use that
                //instead but this helps to have something.
                //var hash = CryptoJS.MD5(currentUser.Email.trim().toLowerCase());
                //currentUser.gravatarUrl = "http://www.gravatar.com/avatar/" + hash + "?s=36&r=g&default=mm";
                _.each(currentUser.roles, function(r) {
                    if (r.name === "System Admin") {
                        _isSystemAdmin = true;
                    }
                    else if (r.name === "Accountant") {
                        _isAccounting = true;
                    }
                    else if (r.name === "Biller") {
                        _isBilling = true;
                    }
                    else if (r.name == "Manager") {
                        _isManager = true;
                    }
                    else if (r.name === "Supervisor") {
                        _isSupervisor = true;
                    }
                    else if (r.name === "User") {
                        _isUser = true;
                    }
                    else if (r.name === "Trucker") {
                        _isTruckingRole = true;
                    }
                    else if (r.name === "Trucking Reporter") {
                        _isTruckingReports = true;
                    }
                    else if (r.name === "Foreman") {
                        _isForemanRole = true;
                    }
                    else if (r.name === "Superintendent") {
                        _isSuperintendent = true;
                    }
                });

                _.each(currentUser.departments, function(d) {
                    switch (d.name) {
                        case "Concrete":
                            _isConcrete = true;
                            break;
                        case "Development":
                            _isDevelopment = true;
                            break;
                        case "Residential":
                            _isResidential = true;
                            break;
                        case "Trucking":
                            _isTrucking = true;
                            break;
                        case "MobileMechanic":
                            _isMobileMechanic = true;
                            break;
                        case "FrontOffice":
                            _isFrontOffice = true;
                            break;
                        case "TMCrushing":
                            _isTMCrushing = true;
                            break;
                        case "Shop":
                            _isShop = true;
                            break;
                    }
                });

                // $rootScope.$broadcast('onRolesSet');

                return PermissionService.getMyPermissions()
                    .then(function() { return currentUser; });
            }
            else {
                throw new Error("Unable to get current user but got successful response from API. Has model changed?");
            }
        })
        .catch(function() {
            throw new Error("Unable to get current user.");
        });
    };

    // Peform what is needed to return a boolean inside a promise indicating if the user is
    // authorized for this site (e.g. check Roles, if applicable)
    var isAuthorizedForSite = function() {
        var deferred = $q.defer();

        // In this case if a user has api access we allow use of site
        service.get().then(function(currentUser) {
            if (currentUser) {
                deferred.resolve(true);
            }
            else {
                deferred.resolve(false);
            }
        }, function() {
            deferred.reject();
        });

        return deferred.promise;
    };

    function getDepartmentIds() {
        return currentUser.departments
            .map(d => d.departmentID);
    }

    return service;
}

interface ICurrentUserService {
    departments: {
        isConcrete(): boolean;
        isDevelopment(): boolean;
        isResidential(): boolean;
        isTrucking(): boolean;
        isMobileMechanic(): boolean;
        isFrontOffice(): boolean;
        isTMCrushing(): boolean;
        isShop(): boolean;
    };
    
    roles: {
        isSystemAdmin(): boolean;
        isAccounting(): boolean;
        isBilling(): boolean;
        isManager(): boolean;
        isSupervisor(): boolean;
        isUser(): boolean;
        isTruckingRole(): boolean;
        isTruckingReports(): boolean;
        isForemanRole(): boolean;
        isSuperintendent(): boolean;
    };
    
    extractToken(response: ng.IHttpResponse<ILogInResponse>): string|undefined;
    setToken(token: string): { [key: string]: string };
    get(): ng.IPromise<ICurrentUser>;
    clear(): void;
    setLoggedIn(loggedIn: boolean): void;
    loggedIn(): boolean;
    isAuthorized(): ng.IPromise<boolean>;
    postCredentials(uri: string, credentials: any): ng.IPromise<ng.IHttpResponse<ILogInResponse>>;
    getDepartmentIds(): number[];
}

interface ILogInResponse {
    access_token: string;
}

interface ICurrentUser {
    
}

export {
    ICurrentUserService,
    ICurrentUser,
    ILogInResponse,
};