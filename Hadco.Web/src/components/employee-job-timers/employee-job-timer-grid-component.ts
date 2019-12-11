import * as angular from 'angular';

import * as template from './job-timer-grid.html';
import * as deleteTemplate from '../Shared/modal-templates/delete-item-modal.html';
import * as cancelTemplate from '../Shared/modal-templates/cancel-modal.html';

angular
    .module('employeeJobTimersModule')
    .component('employeeJobTimersGrid', {
        bindings: {
            timesheet: '=',
            timesheetId: '=',
            foremanId: '=',
            foremanEmployeeTimerId: '=',
            refreshGrid: '&',
        },
        controller: EmployeeJobTimerGridController,
        controllerAs: 'vm',
        template,
    });

EmployeeJobTimerGridController.$inject = [
    '$timeout',
    '$scope',
    '$q',
    '$modal',
    'EmployeeJobTimers',
    'TimesheetsService',
    'Jobs',
    'JobTimers',
    'EmployeeJobEquipmentTimers',
    'NotificationFactory',
];


function EmployeeJobTimerGridController(
    $timeout,
    $scope,
    $q,
    $modal,
    EmployeeJobTimers,
    TimesheetsService,
    Jobs,
    JobTimers,
    EmployeeJobEquipmentTimers,
    NotificationFactory,
) {
    var vm = this;
    vm.$onInit = init;

    function init() {

        vm.canEditRow = true;
        vm.employeeJobTimerGridOptions = {
            enableHorizontalScrollbar: 1,
            enableColumnMenus: false,
            showColumnFooter: true,
            columnDefs: _getColumnDefinitions(),
            showTreeExpandNoChildren: true,
            columnVirtualizationThreshold: 25,
            onRegisterApi: function(gridApi) {
                vm.gridApi = gridApi;
                vm.gridApi.grid.registerDataChangeCallback(function() {
                    vm.gridApi.treeBase.expandAllRows();
                });
                $timeout(function(){
                    var invoiceHeader = vm.gridApi.grid.element[0].children[1].children[2].children[0].children[0].children[0].children[0].children[0].children[0].children[0].children[0].children[0].children[0];
                    var editHeader = vm.gridApi.grid.element[0].children[1].children[2].children[0].children[0].children[0].children[0].children[0].children[0].children[0].children[1].children[0].children[0];
                    invoiceHeader.tabIndex = -1;
                    editHeader.tabIndex = -1;
                });
            },
        };
        TimesheetsService.getDropDownLists().then(function(response) {
            vm.dropDownLists = response;
        });
        //Check if there is an entry on the timesheet
        if (vm.timesheet && vm.timesheet[0]) {
            _organizeData(angular.copy(vm.timesheet[0].employeeJobTimers));
        }
    }

    vm.getTotalQuantity = function(entity) {
        return entity.previousQuantity + entity.newQuantity + entity.otherNewQuantity;
    };

    vm.addNewJobTimer = function() {
        vm.canEditRow = false;
        vm.employeeJobTimerGridOptions.data.push({ isChild: false, editable: true, $$treeLevel: 0, isNewItem: true, job: null, phase: null, category: null });
        _addSequenceToRows();
        _focusInput("jobInput");
    };

    vm.addEquipmentTimer = function(row, index) {
        var indexToInsert = index + 1;
        for (var i = indexToInsert; i < vm.employeeJobTimerGridOptions.data.length; i++) {
            if (!vm.employeeJobTimerGridOptions.data[i].isChild) {
                indexToInsert = i;
                break;
            }
            ++indexToInsert;
        }
        vm.employeeJobTimerGridOptions.data.splice(indexToInsert, 0, { isChild: true, editable: true, isNewItem: true, employeeJobTimerID: row.entity.employeeJobTimerID });
        vm.canEditRow = false;

        _addSequenceToRows();
        vm.expandRow(row);
        _focusInput("equipmentInput");
    };

    vm.addEquipmentTimerValidation = function(row) {
        if (row.entity.equipmentID) {
            return true;
        }
        return false;
    };

    vm.checkJob = function(item, index) {
        vm.employeeJobTimerGridOptions.data[index].job = item;
        vm.employeeJobTimerGridOptions.data[index].jobID = item.jobID;
        vm.employeeJobTimerGridOptions.data[index].jobNumber = item.jobNumber;
        _getAvailablePhases(item.jobID);
        _clearPhase(index);
        _clearCategory(index);
    };

    vm.checkPhase = function(item, index) {
        vm.employeeJobTimerGridOptions.data[index].phase = item;
        vm.employeeJobTimerGridOptions.data[index].phaseID = item.phaseID;
        vm.employeeJobTimerGridOptions.data[index].phaseNumber = item.phaseNumber;
        _getAvailableCategories(vm.employeeJobTimerGridOptions.data[index].jobID, item.phaseID);
        _clearCategory(index);
    };

    vm.checkCategory = function(item, index) {
        vm.employeeJobTimerGridOptions.data[index].category = item;
        vm.employeeJobTimerGridOptions.data[index].categoryID = item.categoryID;
        vm.employeeJobTimerGridOptions.data[index].categoryNumber = item.categoryNumber;
    };

    vm.checkEquipment = function(item, index) {
        vm.employeeJobTimerGridOptions.data[index].equipment = item;
        vm.employeeJobTimerGridOptions.data[index].equipmentID = item.equipmentID;
        vm.employeeJobTimerGridOptions.data[index].equipmentNumber = item.equipmentNumber;
    };

    vm.expandRow = function(row) {
        if (row.treeNode.state !== "expanded") {
            row.grid.api.treeBase.expandRow(row);
        }
    };

    vm.beginEdit = function(row) {
        if (vm.canEditRow) {
            vm.backupEntity = angular.copy(row.entity);
            vm.canEditRow = false;
            row.entity.isChild = false;
            row.entity.editable = true;
            vm.backupChildEntities = [];
            _getAvailablePhases(row.entity.jobID);
            _getAvailableCategories(row.entity.jobID, row.entity.phaseID);
            row.entity.job = {
                jobID: row.entity.jobID,
                jobNumber: row.entity.jobNumber,
            };
            row.entity.phase = {
                phaseID: row.entity.phaseID,
                phaseNumber: row.entity.phaseNumber,
            };
            row.entity.category = {
                categoryID: row.entity.categoryID,
                categoryNumber: row.entity.categoryNumber,
            };
            angular.forEach(row.treeNode.children, function(value, key) {
                vm.backupChildEntities.push(angular.copy(value.row.entity));
                value.row.entity.originalEquipmentID = value.row.entity.equipmentID;
                value.row.entity.isChild = true;
                value.row.entity.editable = true;
            });
            vm.expandRow(row);
            _focusInput("jobInput");
        }
    };

    vm.saveEmployeeTimer = function(row) {
        if (row.entity.isNewItem) {
            _createJobTimer(row);
        }
        else {
            _saveJobTimer(row);
        }
    };

    vm.createEquipmentTimer = function(row) {
        var parentRow = row.treeNode.parentRow.entity;

        var newEquipmentTimer = {
            equipmentID: row.entity.equipmentID,
            equipmentMinutes: 0,
            employeeTimerID: parentRow.employeeTimerID,
            jobTimerID: parentRow.jobTimerID,
            employeeJobTimerID: row.entity.employeeJobTimerID,
        };
        

        EmployeeJobEquipmentTimers.post(newEquipmentTimer).then(function(response) {
            NotificationFactory.success('Equipment has been created');
            _refreshData().then(function() {
                vm.canEditRow = true;
            });
        });
    };

    vm.deleteEquipmentTimer = function(row) {
        var modalInstance = $modal.open({
            template: deleteTemplate,
            controller: 'DeleteModalContentController',
            windowClass: 'default-modal',
            resolve: {
                deletedItemName: function() {
                    return row.entity.equipmentNumber;
                },
            },
        });

        modalInstance.result.then(function() {
            row.entity.originalEquipmentID = row.entity.equipmentID;
            _updateOrDeleteEquipment(row, _deleteEquipment);
            _refreshData();
        });
    };

    vm.deleteJob = function(row) {
        var modalInstance = $modal.open({
            template: deleteTemplate,
            controller: 'DeleteModalContentController',
            windowClass: 'default-modal',
            resolve: {
                deletedItemName: function() {
                    return row.entity.jobPhaseCategory;
                },
            },
        });

        modalInstance.result.then(function() {
            JobTimers.one(row.entity.jobTimerID).remove().then(function() {
                NotificationFactory.success('Job has been deleted');
                _refreshData();
            });
        });
    };
    //Need to be on scope instead of VM for type ahead directive

    $scope.confirmCancel = function(row) {
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
                vm.saveEmployeeTimer(row);
            }
            else {
                vm.cancelEdit(row);
            }
        });
    };

    vm.cancelEdit = function(row) {
        vm.canEditRow = true;
        if (row.entity.isNewItem) {
            vm.employeeJobTimerGridOptions.data.splice(row.entity.sequence, 1);
            return;
        }
        if (row.entity.isChild) {
            row = row.treeNode.parentRow;
        }
        angular.copy(vm.backupEntity, row.entity);
        angular.forEach(row.treeNode.children, function(value, key) {
            var tmp = vm.backupChildEntities.shift();
            angular.copy(tmp, value.row.entity);
        });
    };

    //Need to be on scope instead of VM for type ahead directive
    $scope.enterKeypressSave = function(row, type, shiftEnter) {
        if (row.entity.isNewItem) {
            if (row.entity.isChild) {
                vm.createEquipmentTimer(row);
            }
            else {
                //Create new Job
                _createJobTimer(row);
            }
        }
        else {

            if (row.entity.isChild) {
                row = row.treeNode.parentRow;
            }
            var rowToOpenIndex = row.entity.sequence + row.treeNode.children.length + 1;
            _saveJobTimer(row).then(function() {
                $timeout(function() {
                    if (vm.employeeJobTimerGridOptions.data[rowToOpenIndex] && shiftEnter) {
                        var newRowToOpen = vm.gridApi.grid.rows[rowToOpenIndex];
                        vm.expandRow(newRowToOpen);
                        vm.beginEdit(newRowToOpen);
                        $timeout(function() {
                            _focusInput("jobInput");
                        });
                    }
                });
            });

        }
    };

    function _organizeData(data) {
        var employeeJobTimers = data;

        var employeeJobAndEquipmentTimersList = [];
        var rowIndex = 0;
        angular.forEach(data, function(employeeJobTimer, index) {
            employeeJobTimer.$$treeLevel = 0;
            employeeJobAndEquipmentTimersList.push(employeeJobTimer);
            angular.forEach(employeeJobTimer.employeeJobEquipmentTimers, function(equipmentTimer, index) {
                equipmentTimer.isChild = true;
                rowIndex++;
                employeeJobAndEquipmentTimersList.push(equipmentTimer);
            });

        });
        vm.employeeJobTimerGridOptions.data = employeeJobAndEquipmentTimersList;
        _addSequenceToRows();
    }

    function _addSequenceToRows() {
        angular.forEach(vm.employeeJobTimerGridOptions.data, function(row, index) {
            row.sequence = index;
        });
    }

    function _refreshData() {
        var deferred = $q.defer();
        $timeout(function() {
            vm.refreshGrid().then(function() {
                $timeout(function() {
                    if (vm.timesheet[0]) {
                        _organizeData(angular.copy(vm.timesheet[0].employeeJobTimers));
                    }
                    deferred.resolve();
                });
            });
        });
        return deferred.promise;
    }

    function _clearPhase(index) {
        vm.employeeJobTimerGridOptions.data[index].phase = null;
        vm.employeeJobTimerGridOptions.data[index].phaseID = null;
        vm.employeeJobTimerGridOptions.data[index].phaseNumber = null;
    }

    function _clearCategory(index) {
        vm.employeeJobTimerGridOptions.data[index].category = null;
        vm.employeeJobTimerGridOptions.data[index].categoryID = null;
        vm.employeeJobTimerGridOptions.data[index].categoryNumber = null;
    }

    function _getAvailablePhases(jobId) {
        vm.availablePhases = null;
        Jobs.one(jobId).one('Phases').get().then(function(data) {
            vm.availablePhases = data.items;
        });
    }

    function _getAvailableCategories(jobId, phaseId) {
        vm.availableCategories = null;
        Jobs.one(jobId).one('Phases', phaseId).one('Categories').getList().then(function(data) {
            vm.availableCategories = data;
        });
    }

    function _saveJobTimer(row) {
        var deferred = $q.defer();
        var patchObject = {
            phaseID: row.entity.phaseID,
            categoryID: row.entity.categoryID,
            jobID: row.entity.jobID,
            newQuantity: row.entity.newQuantity,
            diary: row.entity.diary,
            invoiceNumber: row.entity.invoiceNumber,
        };
        JobTimers.one(row.entity.jobTimerID).patch(patchObject).then(function() {
            row.entity.editable = false;
            vm.canEditRow = true;
            angular.forEach(row.treeNode.children, function(value, index) {
                value.row.entity.editable = false;
                _updateOrDeleteEquipment(value.row, _patchEquipment);
            });
            NotificationFactory.success('Timer has been saved.');
            _refreshData().then(function() {
                deferred.resolve();
            });
        }), function(error) {
            NotificationFactory.error('There was an error saving the timer.');
            deferred.reject();
        };
        return deferred.promise;
    }


    function _createJobTimer(row) {
        var jobId = row.entity.jobID
        var phaseId = row.entity.phaseID
        var categoryId = row.entity.categoryID
        if(jobId === null || phaseId === null || categoryId === null)
            return

        row.entity.editable = false;
        var newJobTimer = {
            jobID: jobId,
            phaseID: phaseId,
            categoryID: categoryId,
            diary: row.entity.diary,
            newQuantity: row.entity.newQuantity,
            timesheetID: vm.timesheetId,
        };
        if(newJobTimer.jobID == null || newJobTimer.phaseID == null || newJobTimer.categoryID == null){
            NotificationFactory.error("Please Fill All Required Fields");
            row.entity.editable = true;
        }
        JobTimers.post(newJobTimer).then(function(response) {
            var newEmployeeJobTimer = {
                jobTimerID: response.jobTimerID,
                employeeTimerID: vm.foremanEmployeeTimerId,
            };
            EmployeeJobTimers.post(newEmployeeJobTimer).then(function() {
                NotificationFactory.success("Job timer has been created.");
                _refreshData().then(function() {
                    vm.canEditRow = true;
                });
            });
        });
    }

    function _updateOrDeleteEquipment(row, crudFunction) {
        angular.forEach(vm.timesheet, function(employee) {
            angular.forEach(employee.employeeJobTimers, function(employeeJobTimer) {
                //make sure we are updating equipment for correct job timer
                if (employeeJobTimer.jobTimerID === row.entity.jobTimerID) {
                    angular.forEach(employeeJobTimer.employeeJobEquipmentTimers, function(employeeJobEquipmentTimer) {
                        //Make sure we are updating the correct equipment
                        if (employeeJobEquipmentTimer.employeeJobEquipmentTimerID && employeeJobEquipmentTimer.equipmentID === row.entity.originalEquipmentID) {
                            crudFunction(employeeJobEquipmentTimer, row);
                        }
                    });
                }
            });
        });
    }

    function _patchEquipment(employeeJobEquipmentTimer, row) {
        EmployeeJobEquipmentTimers.one(employeeJobEquipmentTimer.employeeJobEquipmentTimerID).patch({ equipmentID: row.entity.equipmentID }).then(function(response) {
            NotificationFactory.success('Equipment has been updated');
        });
    }

    function _deleteEquipment(employeeJobEquipmentTimer) {
        EmployeeJobEquipmentTimers.one(employeeJobEquipmentTimer.employeeJobEquipmentTimerID).remove().then(function(response) {
            NotificationFactory.success('Equipment has been deleted');
        });
    }

    function _focusInput(name) {
        $timeout(function() {
            angular.element("#" + name).focus();
            $("#" + name).select();
        });
    }

    function _getColumnDefinitions() {
        return [
            {
                name: 'Job', field: 'jobNumber', width: 150,
                cellTemplate: '<ui-grid-typeahead row="row" col-field="COL_FIELD" is-cell-editable="row.entity.editable" is-cell-hidden="row.entity.isChild" check-function="grid.appScope.vm.checkJob"' +
                'typeahead-list="row.grid.appScope.vm.dropDownLists.availableJobs" focus-id="jobInput" model-collection="job" model-identifier="jobNumber">' +
                '</ui-grid-typeahead>',
            },
            {
                name: 'Phase', field: 'phaseNumber', width: 100,
                cellTemplate: '<ui-grid-typeahead row="row" col-field="COL_FIELD" is-cell-editable="row.entity.editable" is-cell-hidden="row.entity.isChild" check-function="grid.appScope.vm.checkPhase"' +
                'typeahead-list="row.grid.appScope.vm.availablePhases" model-collection="phase" model-identifier="phaseNumber">' +
                '</ui-grid-typeahead>',
            },
            {
                name: 'Category', field: 'categoryNumber', width: 100,
                cellTemplate: '<ui-grid-typeahead row="row" col-field="COL_FIELD" is-cell-editable="row.entity.editable" is-cell-hidden="row.entity.isChild" check-function="grid.appScope.vm.checkCategory"' +
                'typeahead-list="row.grid.appScope.vm.availableCategories" model-collection="category" model-identifier="categoryNumber">' +
                '</ui-grid-typeahead>',
            },
            {
                name: 'Diary', field: 'diary', width: 600,
                cellTemplate: '<ui-grid-text-box row="row" col-field="COL_FIELD" is-cell-editable="row.entity.editable" is-cell-hidden="row.entity.isChild" col-field-key="diary"></ui-grid-text-box>',
            },
            {
                name: 'Equip #', field: 'equipmentNumber', width: 75,
                cellTemplate: '<ui-grid-typeahead row="row" is-cell-editable="row.entity.editable" is-cell-hidden="!row.entity.isChild" col-field="COL_FIELD" check-function="grid.appScope.vm.checkEquipment"' +
                    'typeahead-list="grid.appScope.vm.dropDownLists.equipmentList" focus-id="equipmentInput" model-collection="equipmentNumber" model-identifier="equipmentNumber">' +
                    '</ui-grid-typeahead>',
            },
            { name: 'UM', displayName: 'UM', field: 'unitsOfMeasure', width: 75 },
            { name: 'Est', field: 'plannedQuantity', cellFilter: 'number: 2', width: 110 },
            { name: 'Prev', field: 'previousQuantity', cellFilter: 'number: 2', width: 110 },
            {
                name: 'New', field: 'newQuantity', width: 110,
                cellTemplate: '<ui-grid-decimal tabindex="4" row="row" col-field-key="newQuantity" is-cell-editable="row.entity.editable" is-cell-hidden="row.entity.isChild" ></ui-grid-decimal>',
            },
            { name: 'Other New', field: 'otherNewQuantity', cellFilter: 'number: 2', width: 110 },
            {
                name: 'Total', width: 110, cellFilter: 'number: 2',
                cellTemplate: '<div ng-if="!row.entity.isChild" class="ui-grid-cell-contents">{{grid.appScope.vm.getTotalQuantity(row.entity) | number: 2}}</div>',
            },
            {
                name: 'Invoice', width: 110, field: 'invoiceNumber', pinnedRight: true,
                cellTemplate: '<ui-grid-text-box row="row" id="invoiceHeader" col-field="COL_FIELD" is-cell-editable="row.entity.editable" is-cell-hidden="row.entity.isChild" col-field-key="invoiceNumber" maxlength="50"></ui-grid-text-box>',
            },
            {
                name: 'Edit', width: 275, pinnedRight: true,
                cellTemplate: '<div tabindex="-1" ng-if="!row.entity.isChild">' +
                    '<i tabindex="-1" ng-disabled="!grid.appScope.vm.canEditRow"' +
                        'ng-class="{ \'hide\': row.entity.editable}"' +
                        'ng-click="grid.appScope.vm.beginEdit(row)" class="fa fa-lg fa-pencil icon" data-toggle="tooltip" title="Edit" aria-hidden="true">' +
                    '</i>' +
                    '<i tabindex="-1" ng-disabled="!grid.appScope.vm.canEditRow" class="fa fa-lg fa-trash icon" data-toggle="tooltip" title="Delete" aria-hidden="true"' +
                        'ng-class="{ \'hide\': row.entity.editable }"' +
                        'ng-click="grid.appScope.vm.deleteJob(row)">' +
                    '</i>' +
                    '<img tabindex="-1" ng-disabled="!grid.appScope.vm.canEditRow"' +
                        'ng-class="{ \'hide\': row.entity.editable }"' +
                        'ng-click="grid.appScope.vm.addEquipmentTimer(row, row.entity.sequence)"  class="icon-image" src="' + require('../../assets/images/excavator-icon.svg') + '" data-toggle="tooltip" title="Add Equipment">' +
                    '</img>' +
                    '<i tabindex="-1" class="fa fa-lg fa-floppy-o icon" data-toggle="tooltip" title="Save" aria-hidden="true"' +
                        'ng-click="grid.appScope.vm.saveEmployeeTimer(row)"' +
                        'ng-class="{ \'hide\': !row.entity.editable }">' +
                    '</i>' +
                    '<i tabindex="-1" class="fa fa-lg fa-ban icon" data-toggle="tooltip" title="Cancel" aria-hidden="true"' +
                        'ng-click="grid.appScope.vm.cancelEdit(row)"' +
                        'ng-class="{ \'hide\': !row.entity.editable }">' +
                    '</i>' +
                    '</div>' +
                '<div tabindex="-1" ng-if="row.entity.isChild && row.entity.editable && row.entity.isNewItem">' +
                    '<i tabindex="-1" class="fa fa-lg fa-ban icon" data-toggle="tooltip" title="Cancel" aria-hidden="true"' +
                        'ng-click="grid.appScope.vm.cancelEdit(row)" ng-class="{ \'hide\': !row.entity.editable }">' +
                    '</i>' +
                    '<i tabindex="-1" class="fa fa-lg fa-floppy-o icon" data-toggle="tooltip" title="Create Equipment"' +
                        'ng-click="grid.appScope.vm.createEquipmentTimer(row)"' +
                        'ng-disabled="!grid.appScope.vm.addEquipmentTimerValidation(row)"' +
                        'ng-class="{ \'hide\': !row.entity.editable }">' +
                    '</i>' +
                '</div>' +
                '<div tabindex="-1" ng-if="row.entity.isChild && grid.appScope.vm.canEditRow">' +
                    '<i tabindex="-1" class="fa fa-lg fa-trash icon" data-toggle="tooltip" title="Delete Equipment" aria-hidden="true"' +
                        'ng-click="grid.appScope.vm.deleteEquipmentTimer(row)">' +
                    '</i>' +
                '</div>',
            },
        ];
    }
}