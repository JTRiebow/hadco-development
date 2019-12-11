import * as angular from 'angular';
import * as moment from 'moment';

angular
    .module('sharedModule')
    .factory('DateTimeFormats', DateTimeFormatsFactory);

function DateTimeFormatsFactory() {
    var service = {} as IDateTimeFormatsService;

    service.shortDateFormat = shortDateFormat;
    service.urlDateFormat = urlDateFormat;
    service.dateTimeFormat = dateTimeFormat;
    service.timeFormat = timeFormat;
    service.nullDateTimeFormat = nullDateTimeFormat;
    service.now = now;
    service.getShortDateFormat = getShortDateFormat;
    service.getDateTimeFormat = getDateTimeFormat;
    service.getTimeFormat = getTimeFormat;


    return service;

    function shortDateFormat() {
        return 'MM/DD/YYYY';
    }
    function urlDateFormat() {
        return "MM-DD-YYYY";
    }
    function dateTimeFormat() {
        return 'MM/DD/YYYY h:mm a';
    }
    function timeFormat() {
        return 'h:mm a';
    }
    function nullDateTimeFormat() {
        return "none";
    }

    function now(object, key) {
        object[key] = new Date();
    }

    function getShortDateFormat(date, format) {
        return moment(date, format).format(shortDateFormat());
    }

    function getDateTimeFormat(date) {
        return moment(date).format(dateTimeFormat());
    }

    function getTimeFormat(date) {
        return moment(date).format(timeFormat());
    }
}

interface IDateTimeFormatsService {
    shortDateFormat(): string;
    urlDateFormat(): string;
    dateTimeFormat(): string;
    timeFormat(): string;
    nullDateTimeFormat(): string;
    now(object, key: string): void;
    getShortDateFormat(date: Date, format: string): string;
    getDateTimeFormat(date: Date): string;
    getTimeFormat(date: Date): string;
}

export {
    IDateTimeFormatsService,
};