import * as angular from 'angular';
import * as moment from 'moment';
import * as _ from 'lodash';

import * as template from './timer-notes-grid.html';
import * as resolveNoteTemplate from '../../Shared/modal-templates/confirm-note-resolve-modal.html';

angular
    .module('detailedApprovalModule')
    .directive('timerNotesGrid', notesGrid);

function notesGrid() {
    return {
        template,
        controller: timerNotesGridController,
        controllerAs: 'ctrl',
        restrict: 'E',
        scope: {
            visibleTimerEntries: '=',
            isTimecardTab: '=',
        },
    };
}

timerNotesGridController.$inject = [
    '$scope',
    'Restangular',
    '$routeParams',
    '$q',
    '$modal',
    'PermissionService',
];

function timerNotesGridController(
    $scope,
    Restangular,
    $routeParams,
    $q,
    $modal,
    PermissionService,
) {
    var vm = this;
    
    vm.can = PermissionService.can;

    init();

    function init() {
        vm.timersDisabled = false;
        vm.isTimecardTab = $scope.isTimecardTab;
        $scope.$watch("visibleTimerEntries", function(visibleTimerEntries) {
            vm.visibleTimerEntries = visibleTimerEntries;
            if (vm.visibleTimerEntries) {
                _setUpNotes();
            }
        });

        $scope.$on('addedNewNote', function(event, newNote) {
            vm.notes.push(newNote);
        });

        $scope.$on('resolved-note', function(event, resolvedNote) {
            _.each(vm.notes, function(note) {
                if (note.noteID == resolvedNote.noteID) {
                    note.resolved = true;
                }
            });
        });

        $scope.$on('resolved-multiple-notes', function(event, resolvedNotes) {
            for (var i = 0; i < vm.notes.length; i++) {
                vm.notes[i].resolved = resolvedNotes[i].resolved;
            }
        });

        $scope.$on('disableTimers', (_, isDisabled) => {
            vm.timersDisabled = isDisabled;
        });

        _setUpNotes();


    }


    vm.formatTime = function(time) {
        return moment(time).format("h:mm a");
    };

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
            }
            else if (formData === 'notesArray') {

            }
        });

    };

    function _setUpNotes() {

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
            vm.notes = response;
        });
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
            deferred.reject();
        });
        return deferred.promise;

    }

}