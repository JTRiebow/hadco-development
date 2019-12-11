import * as angular from 'angular';
import * as moment from 'moment';
import * as _ from 'lodash';

import * as template from './trucking.html';
import * as truckerPaginationTemplate from './trucker-dailies-pagination.html';
import * as cancelTemplate from '../Shared/modal-templates/cancel-modal.html';
import * as saveAllTemplate from '../Shared/modal-templates/save-all-modal.html';
import './trucker-dailies.scss';

import { truckerDailyColumnDefs } from './trucker-daily-column-defs';

angular
    .module('truckingModule')
    .component('htTruckerDailies', {
        controller: truckerDailiesController,
        controllerAs: 'vm',
        template,
    });

truckerDailiesController.$inject = [
    '$scope',
    '$timeout',
    '$q',
    '$location',
    'Pagination',
    'CurrentUser',
    '$modal',
    'LoadTimers',
    'Jobs',
    'sidebarMenuTemplate',
    'truckingGridsTemplate',
    'uiGridConstants',
    'NotificationFactory',
    'TruckingService',
    'DowntimeTimers',
    'TimesheetsService',
    'PermissionService',
];

function truckerDailiesController(
    $scope,
    $timeout,
    $q,
    $location,
    Pagination,
    CurrentUser,
    $modal,
    LoadTimers,
    Jobs,
    sidebarMenuTemplate: string,
    truckingGridsTemplate: string,
    uiGridConstants,
    NotificationFactory,
    TruckingService,
    DowntimeTimers,
    TimesheetsService,
    PermissionService,
) {
    var vm = this;
    
    vm.can = PermissionService.can;
    vm.editingAll;
    
    const phaseCache = {};
    const categoryCache = {};
    let currentlyEditedRow;

    init();
    
    function init() {
        PermissionService.redirectIfUnauthorized('viewTruckerDailies');
        
        vm.page = 'trucker';
        vm.customerType = { name: 'Trucker Dailies' };
        vm.isNotTruckingReports = !CurrentUser.roles.isTruckingReports();
        vm.gridOptions = _getGridOptions();
        vm.canEditRow = true;
        
        vm.sidebarMenuTemplate = sidebarMenuTemplate;
        vm.truckingGridsTemplate = truckingGridsTemplate;
        vm.editingAll = false;
        
        _getDropDownLists().then(function() {
            _getLoadTimers(vm.range.start, vm.range.end, vm.department.departmentID);
            _setUpColumnVisibility(vm.department);
        });
    }

    vm.departmentChange = function(department) {
        vm.department = department;
        _setUpColumnVisibility(department);
        _clearFiltersAndSorting();
        _getLoadTimers(vm.range.start, vm.range.end, department.departmentID);
    };

    vm.dateChange = function(range, type, enter) {
        var manualInput = $("#" + type + "RangeSelect").is(':focus');
        if (manualInput && !enter) {
        }
        else {
            vm[type + "Opened"] = false;
            angular.element("#" + type + "RangeSelect").blur();
            range[type] = moment(range[type]).format("MM/DD/YYYY");
            vm.rangeChange(range);
        }
    };

    vm.rangeChange = function(range, name) {
        vm.range.name = name || "";
        _getLoadTimers(range.start, range.end, vm.department.departmentID);
    };

    vm.editAll = () => {
        vm.editingAll = true;
        if (vm.can('editTruckerDaily')) {
            vm.edits = angular.copy(vm.masterLoadTimers);
            
            vm.edits.forEach((edit, i) => vm.beginEdit({ entity: edit }, true, i));
            vm.gridOptions.data = vm.edits;
        }
    };

    vm.saveAll = () => {
        vm.editingAll = false;
        vm.edits.forEach((edit, i) => {
            var row = { entity: edit };

            var before = vm.masterLoadTimers[i];
            var after = edit;
            var dirty = Boolean(
                Object.keys(before)
                    .find(key => before[key] != after[key])
            );
            
            edit.editable = true;
            if (dirty) {
                _saveLoadG(row)
                    .then(response => {
                        vm.edits[i] = response;
                    })
                    .catch(exception => {
                        NotificationFactory.error('Error: '+edit.loadTimerId+' Load Timer Not Saved');
                    });
            }
        });
        vm.masterLoadTimers = vm.edits;
    };
    
    vm.cancelAll = () => {
        vm.editingAll = false;
        vm.gridOptions.data = vm.masterLoadTimers;
    };

    vm.beginEdit = function(row, multiple = false, index?: number) {
        currentlyEditedRow = row;
        if (vm.canEditRow) {
            const { entity } = row;
            
            if (!multiple) {
                vm.canEditRow = false;
                vm.backupEntity = angular.copy(entity);
                
            }
            entity.editable = true;
            entity.sequence = !multiple ? _.indexOf(row.grid.rows, row) : index;
            entity.downtimeTimerID || _loadPhases(entity.jobID)
                .then(function(response) {
                    entity.phases = response;
                    entity.emptyPhases = response.length === 0;
                });
            entity.downtimeTimerID || _loadCategories(entity.jobID, entity.phaseID)
                .then(function(response) {
                    entity.categories = response;
                    entity.emptyCategories = response.length === 0;
                });
            $timeout(function() {
                var focusID = entity.downtimeTimerID ? "#downtimeInput" : "#truckInput";
                angular.element(focusID).focus();
                $(focusID).select();
            });
        }
    };

    vm.checkJob = function(jobObject, index, key, label, patchKeyValue, row) {
        vm.checkCell(jobObject, index, key, label, patchKeyValue, row);
        //clear and update phases and categories
        row.entity.phase = '';
        _loadPhases(jobObject.jobID)
            .then(function(response) {
                row.entity.phases = response;
                row.entity.emptyPhases = response.length == 0;
            });
        row.entity.category = '';
        row.entity.categories = [];
        row.entity.emptyCategories = true;
    };

    vm.checkPhase = function(phaseObj, index, phase, label, patchKeyValue, row) {
        vm.checkCell(phaseObj, index, phase, label, patchKeyValue, row);
        //clear and update categories
        row.entity.category = '';
        _loadCategories(row.entity.jobID, phaseObj.phaseID)
            .then(function(response) {
                row.entity.categories = response;
                row.entity.emptyCategories = response.length == 0;
            });
    };

    vm.checkCell = function(selectedObject, index, key, label, patchKeyValue, row) {
        row.entity[patchKeyValue.key] = selectedObject[patchKeyValue.value]; //0.truckID = obj[equipmentID]
        row.entity[key] = selectedObject[label]; //0.truck = obj[equipmentNumber]
    };

    vm.isInvalid = function(row) {
        var loadTimer = row.entity;
        //cycle through loadTimer children to check for and mark invalid or overlapping timers
        var isInvalid;
        if (vm.department.departmentID === 8) {
            return _validateTransport(loadTimer);
        }
        else if (vm.department.departmentID === 4) {
            return _validateTrucking(loadTimer);
        }
    };

    //Need to be on scope instead of VM for type ahead directive
    $scope.enterKeypressSave = function(row, type, shiftEnter) {
        if (!vm.isInvalid(row)) {
            vm.saveLoad(row, shiftEnter);
        }
    };

    vm.saveLoad = function(row, shiftEnter) {
        currentlyEditedRow = undefined;

        //change load and dump to start and end until backend changes
        row.entity.startLocation = row.entity.loadSite;
        row.entity.endLocation = row.entity.dumpSite;

        row.entity.editable = false;
        vm.canEditRow = true;
        _saveLoadG(row)
            .then(response => {
                response.sequence = row.entity.sequence;
                row.entity = response;
                NotificationFactory.success('Success: Load Timer Saved');
                if (shiftEnter) {
                    var startRow = row.entity.sequence + 1;
                    if (row.grid.rows[startRow]) {
                        var newRowToOpen = row.grid.rows[startRow];
                        vm.beginEdit(newRowToOpen);
                    }
                }
            })
            .catch(exception => {
                NotificationFactory.error('Error: Load Timer Not Saved');
            });
    };

    function _saveLoadG(row) {
        //patch
        var endpoint = (row.entity.downtimeTimerID) ? DowntimeTimers.one(row.entity.downtimeTimerID) : LoadTimers.one(row.entity.loadTimerID);
        return endpoint.patch(row.entity, { truckerDaily: true });
    }

    vm.confirmCancel = function(row) {
        var modalInstance = $modal.open({
            template: cancelTemplate,
            controller: 'CancelNotificationController',
            windowClass: 'default-modal',
            keyboard: false,
        });

        modalInstance.result.then(function(saveOrCancel) {
            if (saveOrCancel === "save") {
                if (row.entity.isChild) {
                    row = row.treeNode.parentRow;
                }
                if (!vm.isInvalid(row)) {
                    vm.saveLoad(row);
                }
            }
            else {
                vm.cancelEdit(row);
            }
        });
    };

    vm.confirmSaveAll = function() {
        var modalInstance = $modal.open({
            template: saveAllTemplate,
            controller: 'SaveAllNotificationController',
            windowClass: 'default-modal',
            keyboard: false,
        });

        modalInstance.result.then(function(saveOrCancel) {
            if (saveOrCancel === "save") {
                vm.saveAll();
            } else {
                vm.cancelAll();
            }
        });
    };

    vm.cancelEdit = function(row) {
        currentlyEditedRow = undefined;
        vm.canEditRow = true;
        angular.copy(vm.backupEntity, row.entity);
    };

    function _getDropDownLists() {
        var defer = $q.defer();

        var params = {
            trucks: { orderby: 'equipmentNumber' },
            trailers: { orderby: 'equipmentNumber' },
            equipment: { orderby: 'equipmentNumber' },
            downtimeReasons: { orderBy: 'code' },
            materials: { orderBy: 'name' },
            jobs: { orderBy: 'jobNumber' },
        };
        TimesheetsService.getDropDownLists(params).then(function(response) {
            vm.dropDownLists = response;

            vm.departments = [ vm.dropDownLists.departments[3], vm.dropDownLists.departments[7] ];
            vm.department = vm.departments[0];
            vm.billType = vm.dropDownLists.billTypes[0]; //hourly
            vm.ranges = TruckingService.getRanges();
            vm.range = vm.ranges[2]; //week
            defer.resolve();
        });
        return defer.promise;
    }

    function _getGridOptions() {
        return {
            paginationPageSize: 25,
            paginationTemplate: truckerPaginationTemplate,
            enableFiltering: true,
            aggregationType: uiGridConstants.aggregationTypes.sum,
            //endableCellEditOnFocus: true,
            enableSorting: true,
            enableGridMenu: true,
            exporterCsvFilename: 'Trucker Dailies.csv',
            exporterCsvLinkElement: angular.element(document.querySelectorAll(".custom-csv-link-location")),
            enableColumnMenus: false,
            exporterMenuPdf: false,
            columnDefs: truckerDailyColumnDefs,
            onRegisterApi(gridApi) {
                vm.gridApi = gridApi;
                vm.gridApi.core.on.sortChanged( $scope, function(grid, sortColumns) {
                    if (currentlyEditedRow) {
                        // scroll the bottom
                        angular.element('.ui-grid-viewport')[0].scrollTo(0, 10000000);
                        // scroll up to allow row to be visible, otherwise it becomes "visible" under the horizontal scroll bar
                        $timeout(() => vm.gridApi.core.scrollToIfNecessary(currentlyEditedRow, grid.columns[0]));
                    }
                });
            },
        };
    }
    
    function _getLoadTimers(start, end, departmentID, filteringOptions?, updatedOrderby?) {
        var formattedStart = moment(start, 'MM/DD/YYYY').format('MM-DD-YYYY');
        var formattedEnd = moment(end, 'MM/DD/YYYY').format('MM-DD-YYYY');
        LoadTimers.one(formattedStart).one(formattedEnd).get({ departmentId: departmentID })
        .then(function(response) {
            vm.gridOptions.data = response;
            vm.masterLoadTimers = angular.copy(response);
            _setUpPagination(response);
            vm.canEditRow = true;
        });
    }
    
    function _setUpPagination(loadTimers) {
        vm.gridOptions.totalItems = loadTimers.length;
        vm.gridOptions.paginationPageSizes = [
            { value: 25, label: 25 },
            { value: 50, label: 50 },
            { value: 75, label: 75 },
            { value: loadTimers.length, label: 'All' },
        ];
        vm.gridOptions.paginationPageSize = vm.gridOptions.paginationPageSizes[3].value;
    }

    function _setUpColumnVisibility(department) {
        //change columns based on department
        var isTransport = (department.departmentID == 8) ? true : false;
        //show equipment #
        vm.gridOptions.columnDefs[12].visible = isTransport;
        //hide location, pup, ticket #, tons, material, bill type
        vm.gridOptions.columnDefs[2].visible = !isTransport;
        vm.gridOptions.columnDefs[5].visible = !isTransport;
        vm.gridOptions.columnDefs[7].visible = !isTransport;
        vm.gridOptions.columnDefs[8].visible = !isTransport;
        vm.gridOptions.columnDefs[11].visible = !isTransport;
        vm.gridOptions.columnDefs[19].visible = !isTransport;
    }

    function _validateTransport(loadTimer) {
        return !loadTimer.truck ||
                !loadTimer.job ||
                !loadTimer.phase ||
                !loadTimer.category ||
                !loadTimer.loadEquipment ||
                !loadTimer.loadSite ||
                !loadTimer.dumpSite;
    }

    function _validateTrucking(loadTimer) {
        return !loadTimer.truck ||
            !loadTimer.job ||
            !loadTimer.phase ||
            !loadTimer.category ||
            //(!loadTimer.tons && !(parseInt(loadTimer.tons) === 0))||
            //!parseInt(loadTimer.tons) <= 50 ||
            !loadTimer.material ||
            !loadTimer.loadSite ||
            !loadTimer.dumpSite ||
            !loadTimer.billType;

    }

    function _clearFiltersAndSorting() {
        vm.gridApi.core.clearAllFilters();
        _.each(vm.gridApi.grid.columns, function(col) {
            col.sort = {};
        });
    }

    function _loadPhases(jobID, refreshCache = false) {
        if (phaseCache[jobID] && !refreshCache) return phaseCache[jobID];
        return phaseCache[jobID] = Jobs.one(jobID).getList('Phases', {
            orderBy: 'PhaseNumber',
        });
    }

    function _loadCategories(jobID, phaseID, refreshCache = false) {
        const key = `${jobID}-${phaseID}`;
        if (categoryCache[key] && !refreshCache) return categoryCache[key]; 
        return categoryCache[key] = Jobs.one(jobID).one('Phases', phaseID).getList('Categories', {
            orderBy: 'CategoryNumber',
        });
    }

    //disables backspace as default previous page navigation
    $(document).on("keydown", function(e) {
        if (e.which === 8 && !$(e.target).is("input, textarea")) {
            e.preventDefault();
        }
    });
}