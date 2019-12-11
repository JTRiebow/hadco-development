import * as angular from 'angular';
import * as _ from 'lodash';

angular
    .module('detailedApprovalModule')
    .controller('confirmNoteResolveModalController', confirmNoteResolveModalController);

confirmNoteResolveModalController.$inject = [
    '$scope',
    '$modalInstance',
    'Restangular',
    'noteID',
    'allNotes',
    'singleNote',
    'notesArray',
    '$rootScope',
];

function confirmNoteResolveModalController(
    $scope,
    $modalInstance,
    Restangular,
    noteID,
    allNotes,
    singleNote,
    notesArray,
    $rootScope,
) {
    var vm = this;
    init();
    function init() {
        vm.allNotes = allNotes;
        vm.singleNote = singleNote;
    }


    vm.confirm = function() {
        if (allNotes) {
            _.each(notesArray, function(note) {
                if (!note.resolved) {
                    Restangular.one('Notes', note.noteID).one('Resolve').customPOST();
                }
            });
            $modalInstance.close('notesArray');
        }
        else if (singleNote) {
            Restangular.one('Notes', noteID).one('Resolve').customPOST();
            $modalInstance.close('singleNote');
        }

        $rootScope.$broadcast('notesResolved');
    };

    vm.cancel = function() {
        $modalInstance.close('cancel');
    };

}

