import * as angular from 'angular';
import * as moment from 'moment';

angular
    .module('adminModule')
    .factory('TimesheetsService', TimesheetsService);

TimesheetsService.$inject = [
    'Restangular',
    'Employees',
    '$q',
    'OccurrencesHelper',
    'EquipmentServiceTypesHelper',
    'PitsHelper',
    'CategoriesHelper',
    'DepartmentsHelper',
    'MaterialsHelper',
    'BillTypesHelper',
    'TrucksHelper',
    'TrailersHelper',
    'EquipmentHelper',
    'JobsHelper',
    'DowntimeReasonsHelper',
];

function TimesheetsService(
    Restangular,
    Employees,
    $q,
    OccurrencesHelper,
    EquipmentServiceTypesHelper,
    PitsHelper,
    CategoriesHelper,
    DepartmentsHelper,
    MaterialsHelper,
    BillTypesHelper,
    TrucksHelper,
    TrailersHelper,
    EquipmentHelper,
    JobsHelper,
    DowntimeReasonsHelper
) {

    Restangular.configuration.routeToIdMappings['Timesheets'] = 'timesheetID';
    // return Restangular.service('Timesheets')

    return {
        getForemenForSuperintendent: getForemenForSuperintendent,
        createNewTimesheet: createNewTimesheet,
        patchTruckerTimesheet: patchTruckerTimesheet,
        getEmployee: getEmployee,
        getEmployeeTimesheet: getEmployeeTimesheet,
        getDropDownLists: getDropDownLists,
        getDateList: getDateList,
        getFormattedDate: getFormattedDate,
        convertMinutesToHoursMinutes: convertMinutesToHoursMinutes,
        syncDateAndTime: syncDateAndTime,
        millisToMinutes: millisToMinutes,
    };

    function getForemenForSuperintendent(employeeID, week) {
        return Restangular.service('Timesheets').one("Superintendent").one(employeeID).get({ week: week });
    }

    function createNewTimesheet(day, employeeID, departmentID) {
        return Restangular.service('Timesheets').post({
            "day": day,
            "employeeID": employeeID,
            "departmentID": departmentID,
        });
    }

    function patchTruckerTimesheet(timesheet) {
        return Restangular.service('Timesheets').one(timesheet.timesheetID).patch(timesheet);
    }

    function getEmployee(employeeID) {
        return Employees.one(employeeID).get()
        .then(function(data) {
            return data;
        });
    }

    function getEmployeeTimesheet(employeeID, date, departmentID) {
        var defer = $q.defer();
        Employees.one(employeeID).one('Timesheets', date).get({ "departmentID": departmentID })
        .then(function(response) {
            defer.resolve(response);
        }, function(error) {
            defer.reject(error);
        });
        return defer.promise;
    }

    var dropDownListPromise;

    function getDropDownLists(params) {

        if (dropDownListPromise) {
            return dropDownListPromise;
        }
        else {
            var dropDownLists = {} as any;
            params = params || {};
            var deferred = $q.defer();
            dropDownListPromise = deferred.promise;

            var promises = {
                departments: DepartmentsHelper.getList(params.departments),
                occurrences: OccurrencesHelper.getList(params.occurrences),
                pits: PitsHelper.getList(params.pits),
                equipmentServiceTypes: EquipmentServiceTypesHelper.getList(params.equipmentServiceTypes),
                categories: CategoriesHelper.getHadcoShopList(params.categories),
                materials: MaterialsHelper.getList(params.materials),
                billTypes: BillTypesHelper.getList(params.billTypes),
                trucks: TrucksHelper.getList(params.trucks),
                trailers: TrailersHelper.getList(params.trailers),
                equipment: EquipmentHelper.getList(params.equipment),
                jobs: JobsHelper.getList(params.jobs),
                downtimeReasons: DowntimeReasonsHelper.getList(params.downtimeReasons),
            };

            $q.all(promises).then(function(values) {
                dropDownLists.departments = values.departments;
                dropDownLists.occurrences = values.occurrences;
                dropDownLists.pits = values.pits;
                dropDownLists.serviceTypes = values.equipmentServiceTypes;
                dropDownLists.categories = values.categories;
                dropDownLists.materials = values.materials;
                dropDownLists.billTypes = values.billTypes;
                dropDownLists.trucks = values.trucks;
                dropDownLists.trailers = values.trailers;
                dropDownLists.equipmentList = values.equipment;
                dropDownLists.availableJobs = values.jobs;
                dropDownLists.downtimeReasons = values.downtimeReasons;

                deferred.resolve(dropDownLists);
            });

            return dropDownListPromise;
        }
    }

    function getDateList(day) {
        return [
            {
                date: moment(day, "MM-DD-YYYY"),
                formattedDate: moment(day, "MM-DD-YYYY").format("MM/DD/YYYY"),
            },
            {
                date: moment(day, "MM-DD-YYYY").add(1, 'd'),
                formattedDate: moment(day, "MM-DD-YYYY").add(1, 'd').format("MM/DD/YYYY"),
            },
        ];
    }

    function getFormattedDate(day) {
        return {
            date: moment(day, "MM-DD-YYYY"),
        };
    }

    function convertMinutesToHoursMinutes(value) {
        var isNegative = false;
        if (value < 0) {
            isNegative = true;
            value *= -1;
        }
        var hours, minutes;
        hours = addZeroToValueWhenNecessary(Math.floor(value / 60).toString());
        minutes = addZeroToValueWhenNecessary((value % 60).toString());
        if (isNegative) {
            hours = "-" + hours;
        }
        return hours + ":" + minutes;
    }

    function millisToMinutes(millis) {
        var minutes = Math.floor(millis / 60000);
        var seconds = ((millis % 60000) / 1000).toFixed(0);
        return (+seconds == 60 ? (minutes + 1) : minutes);
    }

    function addZeroToValueWhenNecessary(value) {
        return (value.length > 1) ? value : "0" + value;
    }

    function syncDateAndTime(item, type, entity, defaultTime) {
        var key;
        switch (type) {
            case 'startDate':
                key = 'startTime';
                break;
            case 'stopDate':
                key = 'stopTime';
                break;
            case 'clockInDate':
                key = 'clockIn';
                break;
            case 'clockOutDate':
                key = 'clockOut';
                break;
        }

        var time = entity[key] || moment(defaultTime).format();
        return { key: key, value: moment(time).set({ 'month': item.date.get('month'), 'date': item.date.get('date') }).format() };
    }
}