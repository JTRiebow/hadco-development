import * as angular from 'angular';

angular
    .module('authorizationModule', [])
    .constant('appConfig', {
        authUrl: '/token',
    });

import './auth-config';
import './auth-init-run';
import './auth-controller';
import './auth-interceptor';
import './authorization-factory';
import './login-controller';