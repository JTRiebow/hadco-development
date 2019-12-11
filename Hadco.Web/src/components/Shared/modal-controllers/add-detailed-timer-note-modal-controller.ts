import * as angular from 'angular';
import * as _ from 'lodash';
import * as moment from 'moment';
import { IPermissionService } from '../../permissions/permission-service';

angular
    .module('modalModule')
    .controller('addDetailedTimerNoteModalController', addDetailedTimerNoteModalController);

addDetailedTimerNoteModalController.$inject = [
    '$scope',
    '$modalInstance',
    'employeeID',
    'departmentID',
    'timerDay',
    'currentApprovals',
    'Notes',
    'rejectNote',
    'NotificationFactory',
    'PermissionService',
];

function addDetailedTimerNoteModalController(
    $scope,
    $modalInstance,
    employeeID,
    departmentID,
    timerDay,
    { approvedBySupervisor, approvedByAccounting, approvedByBilling },
    Notes,
    rejectNote,
    NotificationFactory,
    PermissionService: IPermissionService,
) {
    var vm = this;
    
    vm.can = PermissionService.can;
    
    $scope.options = [
        { name: 'Clock In' },
        { name: 'Clock Out' },
        { name: 'Injury' },
        { name: 'Occurrence' },
        { name: 'Other' },
    ];
    $scope.rejectOptions = [
        {
            name: 'Accounting',
            canReject: vm.can('rejectAccountingApproval') &&
                approvedBySupervisor &&
                approvedByBilling &&
                approvedByAccounting,
        },
        {
            name: 'Billing',
            canReject: vm.can('rejectBillingApproval') &&
                approvedBySupervisor &&
                approvedByBilling,
        },
        {
            name: 'Supervisor',
            canReject: vm.can('rejectSupervisorApproval') && approvedBySupervisor,
        },
    ];
    
    init();
    
    function init() {
        vm.rejectToDepartments = [false, false, false];
        vm.resolved = false;
        vm.employeeID = employeeID;
        vm.departmentID = departmentID;
        vm.timerDay = moment(timerDay).format("YYYY-MM-DD");
        vm.rejectNote = rejectNote;
    }

    vm.save = function() {
        var formError = false;
        var rejectionSelected = vm.rejectToDepartments.find(rejection => rejection);

        if (!vm.type) {
            formError = true;
            NotificationFactory.error("Must select a note type");
        }

        if (!vm.textArea) {
            formError = true;
            NotificationFactory.error("Must include note");
        }

        if (!rejectionSelected && vm.rejectNote) {
            formError = true;
            NotificationFactory.error("Must select a department");
        }

        if (!formError) {
            var noteDetails = {
                employeeID: vm.employeeID,
                departmentID: vm.departmentID,
                description: vm.textArea,
                noteTypeID: _getNoteID(vm.type.name),
                day: vm.timerDay,
            };

            Notes.postNoteForFlaggedTimeCard(noteDetails).then(function(response) {
                var newNoteObject = response;
                newNoteObject.noteType = { description: vm.type.name };


                if (rejectNote) {
                    var rejectValues = {
                        accounting: approvedByAccounting && Boolean(rejectionSelected),
                        billing: approvedByBilling && rejectionSelected != 'Accounting',
                        supervisor: approvedBySupervisor && rejectionSelected == 'Supervisor',
                        newNote: newNoteObject,
                    };
                    $modalInstance.close(rejectValues);
                }
                else {
                    $modalInstance.close(newNoteObject);
                }
            });
        }
    };

    vm.cancel = function() {
        $modalInstance.dismiss();
    };

    function _getNoteID(name) {
        switch (name) {
            case 'Other':
                return 0;
            case 'Clock In':
                return 1;
            case 'Clock Out':
                return 2;
            case 'Injury':
                return 5;
            case 'Occurrence':
                return 10;
        }
    }
}

