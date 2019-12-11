import * as angular from 'angular';
import * as moment from 'moment';
import * as _ from 'lodash';

import * as template from './notes-grid.html';
import * as subgridTemplate from './subgridTemplate.html';
import * as resolveNoteTemplate from '../../Shared/modal-templates/confirm-note-resolve-modal.html';
import * as addDetailedNoteTemplate from '../../Shared/modal-templates/add-detailed-timer-note-modal.html';
import { IPermissionService } from '../../permissions/permission-service';

angular
    .module('detailedApprovalModule')
    .directive('notesGrid', notesGrid);

function notesGrid() {
    return {
        template,
        controller: notesGridController,
        controllerAs: 'ctrl',
        restrict: 'E',
        scope: {
            visibleTimerEntries: '=',
            isTimecardTab: '=',
            currentApprovals: '=',
        },
    };
}

notesGridController.$inject = [
    '$scope',
    'Restangular',
    '$routeParams',
    '$q',
    '$modal',
    '$rootScope',
    'PermissionService',
];

function notesGridController(
    $scope: ng.IScope & { [k: string]: any },
    Restangular,
    $routeParams,
    $q,
    $modal,
    $rootScope: ng.IRootScopeService,
    PermissionService: IPermissionService,
) {
    var vm = this;
    let _openNewNote = false;

    vm.can = PermissionService.can;

    init();

    function init() {
        vm.timersDisabled = false;
        vm.isTimecardTab = $scope.isTimecardTab;
        $scope.$watch("visibleTimerEntries", function(visibleTimerEntries) {
            vm.visibleTimerEntries = visibleTimerEntries;
            if (vm.visibleTimerEntries) {
                _setUpGrid();
            }
        });

        $scope.$watch("currentApprovals", function(currentApprovals) {
            vm.currentApprovals = currentApprovals;
            if (_openNewNote) {
                vm.newNote();
            }
        });

        vm.noteGridOptions = _getNotesGridOptions();

        $scope.$on('rejected-note', function(event, newNote) {
            vm.noteGridOptions.data.push(newNote);
        });

        $scope.$on('disableTimers', (_, isDisabled) => {
            vm.timersDisabled = isDisabled;
        });
        
        $scope.$on('refresh-notes', () => {
            _setUpGrid();
        });
    }


    vm.formatTime = function(time) {
        return moment(time).format("H:mm a");
    };

    vm.newNote = function() {
        if (!vm.currentApprovals) {
            _openNewNote = true;
            return;
        }

        _openNewNote = false;
        var modalInstance = $modal.open({
            controller: 'addDetailedTimerNoteModalController',
            controllerAs: 'vm',
            template: addDetailedNoteTemplate,
            windowClass: 'default-modal',
            resolve: {
                employeeID: function() {
                    return $routeParams.employeeId;
                },
                departmentID: function() {
                    return $routeParams.departmentId;
                },
                timerDay: function() {
                    return $routeParams.dayId;
                },
                rejectNote: function() {
                    return false;
                },
                currentApprovals: () => {
                    return vm.currentApprovals;
                },
            },
        });

        modalInstance.result.then(function(response) {
            var newNote = response;
            switch (newNote.noteTypeID) {
                case 0:
                    newNote.noteType ={ description: 'Other' };
                    break;
                case 1:
                    newNote.noteType = { description: 'Clock In' };
                    break;
                case 2:
                    newNote.noteType = { description: 'Clock Out' };
                    break;
                case 5:
                    newNote.noteType = { description: 'Injury' };
                    break;
            }

            //backend error preventing proper testing of this portion of code
            //_getNoteCreatorName()
            //.then(function (currentEmployeeInfo) {
            //    newNote['createdEmployee'] = currentEmployeeInfo.name;
            //})

            vm.noteGridOptions.data.push(newNote);
            $rootScope.$broadcast('addedNewNote', newNote);
        });

        
    };

    //function _getNoteCreatorName() {
    //    var deferred = $q.defer()
    //    Restangular.one('Employees', $routeParams.employeeId).get()
    //    .then(function (currentEmployeeInfo) {
    //        deferred.resolve(currentEmployeeInfo)
        
    //    })

    //    return deferred.promise;
    //}

    vm.resolve = function(note) {
        $scope.noteInstance = note;
        var modalInstance = $modal.open({
            controller: 'confirmNoteResolveModalController',
            controllerAs: 'vm',
            template: resolveNoteTemplate,
            windowClass: 'default-modal',
            resolve: {
                noteID: function() {
                    return note.noteID;
                },
                allNotes: function() {
                    return false;
                },
                notesArray: function() {
                    return;
                },
                singleNote: function() {
                    return true;
                },
            },
        });

        modalInstance.result.then(function(formData) {
            if (formData === 'cancel') {

            }
            else if (formData === 'singleNote') {
                note.resolved = true;
                $rootScope.$broadcast('resolved-note', note);
            }
        });

    };

    vm.resolveNotes = function(note) {
        var modalInstance = $modal.open({
            controller: 'confirmNoteResolveModalController',
            controllerAs: 'vm',
            template: resolveNoteTemplate,
            windowClass: 'default-modal',
            resolve: {
                noteID: function() {
                    return;
                },
                allNotes: function() {
                    return true;
                },
                notesArray: function() {
                    return vm.notes;
                },
                singleNote: function() {
                    return false;
                },
            },
        });

        modalInstance.result.then(function(formData) {
            if (formData === 'cancel') {

            }
            else if (formData === 'singleNote') {
                note.resolved = true;
                
            }
            else if (formData === 'notesArray') {
                for (let i = 0; i < vm.notes.length; i++) {
                    vm.notes[i].resolved = true;
                }

                $rootScope.$broadcast('resolved-multiple-notes', vm.notes);
            }
        });
    };

    function _setUpGrid() {
        
        _getNotes().then(function(response) {
            _.each(response, function(note) {
                var resolvedEmployeeData = [];
                if (note.resolvedEmployee !== null) {
                    note.resolvedEmployee.action = "Approved";
                    note.resolvedEmployee.day = note.resolvedTime;
                }
                resolvedEmployeeData.push(note.resolvedEmployee);
                note.resolvedEmployee = resolvedEmployeeData;
            });

            vm.notes = _setUpExpandedNoteData(response);

            vm.noteGridOptions.data = vm.notes;
            vm.gridTableHeight = _getTableHeight(vm.noteGridOptions.data.length);
        });
    }

    function _getNotesGridOptions() {
        return {
            enableColumnMenus: false,
            columnDefs: _colDefs(),
            enableHorizontalScrollbar: 1,
            enableVerticalScrollbar: 1,
            rowHeight: 30,
            showHeader: true,
            expandableRowTemplate: subgridTemplate,
            expandableRowHeight: 85,
            expandableRowScope: {
                subGridVariable: 'subGridScopeVariable',
            },

            columnVirtualizationThreshold: 25,
            onRegisterApi: function(gridApi) {
                vm.gridApi = gridApi;
            },
        };
    }

    function _colDefs() {

        return [
            {
                field: 'noteType.description',
                displayName: 'Type',
                width: 100,
            },
            {
                field: 'createdTime',
                displayName: 'Date',
                type: 'date',
                cellFilter: 'date:\'MM/dd/yyyy\'',
                width: 100,
            },
            {
                field: 'createdTime',
                displayName: 'Time',
                type: 'date',
                cellFilter: 'date:\'h:mm a\'',
                width: 100,
            },
            {
                field: 'createdEmployee.name',
                displayName: 'Created By',
                width: 100,
                cellTemplate: '<div data-toggle="tooltip" title="{{COL_FIELD}}" class="note-creator">{{COL_FIELD}}</div>',
            },
            {
                field: 'description',
                Width: 300,
                cellTooltip: true,
            },
            {
                field: 'noteType.isSystemGenerated',
                displayName: 'Flag',
                width: 50,
                cellTemplate: `
                    <div ng-class="{notesFlagUser: !row.entity.noteType.isSystemGenerated , notesFlagSystem: row.entity.noteType.isSystemGenerated }">
                        <i class="fa fa-exclamation-triangle" aria-hidden="true"></i>
                    </div>
                `,
            },
            {
                field: 'resolved',
                width: 90,
                cellTemplate: `
                    <div class="text-center" >
                        <input
                            type="checkbox"
                            ng-checked="row.entity.resolved"
                            ng-disabled="!grid.appScope.ctrl.can('resolveEmployeeTimerFlag') || row.entity.resolved || grid.appScope.ctrl.timersDisabled"
                            ng-click="grid.appScope.ctrl.resolve(row.entity) "/>
                    </div>
                `,
            },
        ];
    }

    function _getNotes() {
        var deferred = $q.defer();
        
        var notesArray = [];

        var formattedDay = moment($routeParams.dayId).format("YYYY-MM-DD");
        Restangular.one("Employee", $routeParams.employeeId).one("Day", formattedDay).one("DepartmentID", $routeParams.departmentId).one("Notes").get()
        .then(function(response) {
            _.each(response, function(note) {
                notesArray.push(note);
            });
            deferred.resolve(notesArray);
            

        }, function(error) {
            console.log("note grid error");
            deferred.reject;
        });
        return deferred.promise;

    }

    function _getTableHeight(dataLength) {
        if (vm.isTimecardTab) {
            var rowHeight = vm.noteGridOptions.rowHeight;
            var headerHeight = 30;
            var scrollbarHeight = 15;
            dataLength = dataLength || 1;
            return {
                "height": (dataLength * rowHeight + headerHeight + scrollbarHeight)*1.5 + "px",
            };
        }
        else {
            return { "height": "150px" };
        }
    }

    function _setUpExpandedNoteData(notes) {
        var modifiedNotes = notes;

        for (let i = 0; i < notes.length; i++) {
            modifiedNotes[i].subGridOptions = {
                columnDefs: [
                    { name: 'Date', field: 'day', type: 'date', cellFilter: 'date:\'MM/dd/yyyy\'' },
                    { name: 'Time', field: 'day', type: 'date', cellFilter: 'date:\'HH:mm a\'' },
                    { name: 'Name', field: 'name' },
                    { name: 'Action', field: 'action' },
                ],
                data: modifiedNotes[i].resolvedEmployee,
            };
            
        }
        return modifiedNotes;
    }
}