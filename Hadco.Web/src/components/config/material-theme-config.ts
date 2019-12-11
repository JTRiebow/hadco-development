import * as angular from 'angular';

angular
    .module('configModule')
    .config(materialThemeConfig);

materialThemeConfig.$inject = ['$mdThemingProvider'];

function materialThemeConfig($mdThemingProvider: ng.material.IThemingProvider) {
    $mdThemingProvider
        .definePalette('hadcoPrimary', { // red
            50: 'f94a4a',
            100: 'f94a4a',
            200: 'f94a4a',
            300: 'b50a37',
            400: 'b50a37',
            500: 'b50a37',
            600: '91082c',
            700: '91082c',
            800: '91082c',
            900: '91082c',
            A100: 'e6c6c6',
            A200: 'e6c6c6',
            A400: 'e6c6c6',
            A700: 'e6c6c6',
            contrastDefaultColor: 'light',
            contrastDarkColors: ['50', '100', '200', 'A100', 'A200', 'A400', 'A700'],
        })
        .definePalette('hadcoAccent', { // blue
            50: '78b2f7',
            100: '78b2f7',
            200: '78b2f7',
            300: '476ee3',
            400: '476ee3',
            500: '476ee3',
            600: '0837c3',
            700: '0837c3',
            800: '0837c3',
            900: '0837c3',
            A100: '8caace',
            A200: '8caace',
            A400: '8caace',
            A700: '8caace',
            contrastDefaultColor: 'light',
            contrastDarkColors: ['50', '100', '200', 'A100', 'A200', 'A400', 'A700'],
        })
        .definePalette('hadcoWarn', { // yellow/orange
            50: 'f9e89e',
            100: 'f9e89e',
            200: 'f9e89e',
            300: 'f89d18',
            400: 'f89d18',
            500: 'f89d18',
            600: 'f04124',
            700: 'f04124',
            800: 'f04124',
            900: 'f04124',
            A100: 'ffb88d',
            A200: 'ffb88d',
            A400: 'ffb88d',
            A700: 'ffb88d',
            contrastDefaultColor: 'light',
            contrastDarkColors: ['50', '100', '200', 'A100', 'A200', 'A400', 'A700'],
        })
        .definePalette('hadcoBackground', { // grey
            50: 'efefef',
            100: 'efefef',
            200: 'efefef',
            300: 'efefef',
            400: 'efefef',
            500: '333333',
            600: '333333',
            700: '333333',
            800: '333333',
            900: '333333',
            A100: 'ffffff',
            A200: '333333',
            A400: '000000',
            A700: '000000',
            contrastDefaultColor: 'dark',
            contrastLightColors: ['50', '100', '200', '300', '400', 'A100'],
        });
    
    $mdThemingProvider.theme('default')
        .primaryPalette('hadcoPrimary')
        .accentPalette('hadcoAccent')
        .warnPalette('hadcoWarn')
        .backgroundPalette('hadcoBackground');
    
    $mdThemingProvider
        .enableBrowserColor({
            theme: 'default',
            palette: 'hadcoBackground',
            hue: 'A100',
        });
}