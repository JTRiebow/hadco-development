import * as angular from 'angular';

angular
    .module('modalModule')
    .controller('editableTextModalController', editableTextModalController);

editableTextModalController.$inject = [
    '$scope',
    '$modalInstance',
    'Restangular',
    'employeeID',
    'departmentID',
    'timerDay',
    'Notes',
];

function editableTextModalController(
    $scope,
    $modalInstance,
    Restangular,
    employeeID,
    departmentID,
    timerDay,
    Notes,
) {
    var vm = this;

    init();
    function init() {
        vm.resolved = false;
        vm.employeeID = employeeID;
        vm.departmentID = departmentID;
        vm.timerDay = timerDay;
    }

    vm.save = function() {
        var noteDetails = {
            employeeID: vm.employeeID,
            departmentID: vm.departmentID,
            description: vm.textArea,
            noteType: 0,
            day: timerDay,
        };

        Notes.postNoteForFlaggedTimeCard(noteDetails);
        $modalInstance.close('save');


    };

    vm.cancel = function() {
        $modalInstance.close('cancel');
    };
}
