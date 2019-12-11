export const truckerDailyColumnDefs = [
    {
        name: 'Date',
        field: 'date',
        width: 100,
        type: 'date',
        enableFiltering: false,
    },
    {
        name: 'Name',
        field: 'name',
        width: 200,
        type: 'string',
    },
    {
        name: 'Location',
        field: 'location',
        width: 110,
        type: 'string',
    },
    {
        name: 'Truck',
        field: 'truck',
        width: 100,
        type: 'string',
        cellTemplate:
            '<ui-grid-typeahead ' +
                'check-function="grid.appScope.vm.checkCell" ' +
                'row="row" ' +
                'required="true" ' +
                'is-cell-editable="' +
                    'row.entity.editable && ' +
                    'grid.appScope.vm.isNotTruckingReports && ' +
                    '!row.entity.downtimeTimerID' +
                '" ' +
                'col-field="COL_FIELD" focus-id="truckInput" ' +
                'typeahead-list="row.grid.appScope.vm.dropDownLists.trucks" ' +
                'keypress-events ' +
                'escape="grid.appScope.vm.confirmCancel(row)" ' +
                'model-collection="truck" ' +
                'model-identifier="equipmentNumber" ' +
                'patch-key-value="{' +
                    'key: \'truckID\',' +
                    'value: \'equipmentID\'' +
                '}"' +
            '></ui-grid-typeahead>',
    },
        {
            name: 'Trailer',
            field: 'trailer',
            width: 80,
            cellTemplate:
            '<ui-grid-typeahead ' +
                'check-function="grid.appScope.vm.checkCell" ' +
                'row="row" ' +
                'required="false" ' +
                'is-cell-editable="' +
                    'row.entity.editable && ' +
                    'grid.appScope.vm.isNotTruckingReports && ' +
                    '!row.entity.downtimeTimerID' +
                '" ' +
                'col-field="COL_FIELD" ' +
                'typeahead-list="grid.appScope.vm.dropDownLists.trailers" ' +
                'keypress-events ' +
                'escape="grid.appScope.vm.confirmCancel(row)" ' +
                'model-collection="trailer" ' +
                'model-identifier="equipmentNumber" ' +
                'patch-key-value="{' +
                    'key: \'trailerID\', ' +
                    'value: \'equipmentID\'' +
                '}"' +
            '></ui-grid-typeahead>',
        },
    {
        name: 'Pup',
        field: 'pupTrailer',
        width: 70,
        cellTemplate:
            '<ui-grid-typeahead ' +
                'check-function="grid.appScope.vm.checkCell" ' +
                'row="row" ' +
                'is-cell-editable="' +
                    'row.entity.editable && ' +
                    'grid.appScope.vm.isNotTruckingReports && ' +
                    '!row.entity.downtimeTimerID' +
                '" ' +
                'keypress-events ' +
                'escape="grid.appScope.vm.confirmCancel(row)" ' +
                'col-field="COL_FIELD" ' +
                'typeahead-list="grid.appScope.vm.dropDownLists.trailers" ' +
                'model-collection="pupTrailer" ' +
                'model-identifier="equipmentNumber" ' +
                'patch-key-value="{' +
                    'key: \'pupID\', ' +
                    'value: \'equipmentID\'' +
                '}"' +
            '></ui-grid-typeahead>',
    },
    {
        name: 'Downtime',
        field: 'downtimeCode',
        width: 100,
        cellTemplate:
            '<ui-grid-typeahead ' +
                'check-function="grid.appScope.vm.checkCell" ' +
                'row="row" ' +
                'focus-id="downtimeInput" ' +
                'is-cell-editable="' +
                    'row.entity.editable && ' +
                    'grid.appScope.vm.isNotTruckingReports && ' +
                    'row.entity.downtimeTimerID' +
                '" ' +
                'keypress-events ' +
                'escape="grid.appScope.vm.confirmCancel(row)" ' +
                'col-field="COL_FIELD" ' +
                'typeahead-list="grid.appScope.vm.dropDownLists.downtimeReasons" ' +
                'model-collection="downtimeCode" ' +
                'model-identifier="code" ' +
                'patch-key-value="{' +
                    'key: \'downtimeReasonID\', ' +
                    'value: \'downtimeReasonID\'' +
                '}"' +
            '></ui-grid-typeahead>',
    },
    {
        name: 'Ticket #',
        field: 'ticketNumber',
        width: 100,
        type: 'string',
        cellTemplate:
            '<ui-grid-text-box ' +
                'row="row" ' +
                'keypress-events ' +
                'escape="grid.appScope.vm.confirmCancel(row)" ' +
                'col-field="COL_FIELD" ' +
                'is-cell-editable="' +
                    'row.entity.editable && ' +
                    'grid.appScope.vm.isNotTruckingReports && ' +
                    '!row.entity.downtimeTimerID' +
                '" ' +
                'col-field-key="ticketNumber"' +
            '></ui-grid-text-box>',
    },
    {
        name: 'Tons',
        field: 'tons',
        width: 100,
        type: 'number',
        cellTemplate:
            '<ui-grid-text-box ' +
                'row="row" ' +
                'keypress-events ' +
                'escape="grid.appScope.vm.confirmCancel(row)" ' +
                'required="true" ' +
                'col-field="COL_FIELD" ' +
                'is-cell-editable="' +
                    'row.entity.editable && ' +
                    'grid.appScope.vm.isNotTruckingReports && ' +
                    '!row.entity.downtimeTimerID' +
                '" ' +
                'col-field-key="tons"' +
            '></ui-grid-text-box>',
    },
    {
        name: 'Load Site',
        field: 'loadSite',
        width: 200,
        type: 'string',
        cellTemplate:
            '<ui-grid-text-box ' +
                'row="row" ' +
                'keypress-events ' +
                'escape="grid.appScope.vm.confirmCancel(row)" ' +
                'required="true" ' +
                'col-field="COL_FIELD" ' +
                'is-cell-editable="' +
                    'row.entity.editable && ' +
                    'grid.appScope.vm.isNotTruckingReports && ' +
                    '!row.entity.downtimeTimerID' +
                '" ' +
                'col-field-key="loadSite"' +
            '></ui-grid-text-box>',
    },
    {
        name: 'Dump Site',
        field: 'dumpSite',
        width: 200,
        type: 'string',
        cellTemplate:
            '<ui-grid-text-box ' +
                'row="row" ' +
                'keypress-events ' +
                'escape="grid.appScope.vm.confirmCancel(row)" ' +
                'required="true" ' +
                'col-field="COL_FIELD" ' +
                'is-cell-editable="' +
                    'row.entity.editable && ' +
                    'grid.appScope.vm.isNotTruckingReports && ' +
                    '!row.entity.downtimeTimerID' +
                '" ' +
                'col-field-key="dumpSite"' +
            '></ui-grid-text-box>',
    },
    {
        name: 'Material',
        field: 'material',
        width: 230,
        cellTemplate:
            '<ui-grid-typeahead ' +
                'check-function="grid.appScope.vm.checkCell" ' +
                'row="row" ' +
                'is-cell-editable="' +
                    'row.entity.editable && ' +
                    'grid.appScope.vm.isNotTruckingReports && ' +
                    '!row.entity.downtimeTimerID' +
                '" ' +
                'keypress-events ' +
                'escape="grid.appScope.vm.confirmCancel(row)" ' +
                'col-field="COL_FIELD" ' +
                'typeahead-list="grid.appScope.vm.dropDownLists.materials" ' +
                'model-collection="material" ' +
                'model-identifier="name" ' +
                'patch-key-value="{' +
                    'key: \'materialID\', ' +
                    'value: \'materialID\'' +
                '}"' +
            '></ui-grid-typeahead>',
    },
    {
        name: 'Equipment #',
        field: 'loadEquipment',
        width: 150,
        cellTemplate:
            '<ui-grid-typeahead ' +
                'check-function="grid.appScope.vm.checkCell" ' +
                'row="row" ' +
                'is-cell-editable="' +
                    'row.entity.editable && ' +
                    'grid.appScope.vm.isNotTruckingReports && ' +
                    '!row.entity.downtimeTimerID' +
                '" ' +
                'keypress-events ' +
                'escape="grid.appScope.vm.confirmCancel(row)" ' +
                'col-field="COL_FIELD" ' +
                'typeahead-list="grid.appScope.vm.dropDownLists.equipmentList" ' +
                'model-collection="loadEquipment" ' +
                'model-identifier="equipmentNumber" ' +
                'patch-key-value="{' +
                    'key: \'loadEquipmentID\', ' +
                    'value: \'equipmentID\'' +
                '}"' +
            '></ui-grid-typeahead>',
    },
    {
        name: 'Job',
        field: 'job',
        width: 150,
        cellTemplate:
            '<ui-grid-typeahead ' +
                'row="row" ' +
                'is-cell-editable="' +
                    'row.entity.editable && ' +
                    'grid.appScope.vm.isNotTruckingReports && ' +
                    '!row.entity.downtimeTimerID' +
                '" ' +
                'keypress-events ' +
                'escape="grid.appScope.vm.confirmCancel(row)" ' +
                'col-field="COL_FIELD" ' +
                'typeahead-list="grid.appScope.vm.dropDownLists.availableJobs" ' +
                'model-collection="job" ' +
                'model-identifier="jobNumber" ' +
                'check-function="grid.appScope.vm.checkJob" ' +
                'patch-key-value="{' +
                    'key: \'jobID\', ' +
                    'value: \'jobID\'' +
                '}"' +
            '></ui-grid-typeahead>',
    },
    {
        name: 'Phase',
        field: 'phase',
        width: 100,
        cellTemplate:
            '<ui-grid-typeahead ' +
                'row="row" ' +
                'is-cell-editable="' +
                    'row.entity.editable && ' +
                    'grid.appScope.vm.isNotTruckingReports && ' +
                    '!row.entity.downtimeTimerID && ' +
                    'row.entity.phases.length > 0' +
                '" ' +
                'keypress-events ' +
                'escape="grid.appScope.vm.confirmCancel(row)" ' +
                'col-field="COL_FIELD" ' +
                'typeahead-list="row.entity.phases" ' +
                'model-collection="phase" ' +
                'model-identifier="phaseNumber" ' +
                'check-function="grid.appScope.vm.checkPhase" ' +
                'patch-key-value="{' +
                    'key: \'phaseID\', ' +
                    'value: \'phaseID\'' +
                '}"' +
            '></ui-grid-typeahead>',
    },
    {
        name: 'Category',
        field: 'category',
        width: 100,
        cellTemplate:
            '<ui-grid-typeahead ' +
                'check-function="grid.appScope.vm.checkCell" ' +
                'row="row" ' +
                'is-cell-editable="' +
                    'row.entity.editable && ' +
                    'grid.appScope.vm.isNotTruckingReports && ' +
                    '!row.entity.downtimeTimerID && ' +
                    'row.entity.categories.length > 0' +
                '" ' +
                'keypress-events ' +
                'escape="grid.appScope.vm.confirmCancel(row)" ' +
                'col-field="COL_FIELD" ' +
                'typeahead-list="row.entity.categories" ' +
                'model-collection="category" ' +
                'model-identifier="categoryNumber" ' +
                'patch-key-value="{' +
                    'key: \'categoryID\', ' +
                    'value: \'categoryID\'' +
                '}"' +
            '></ui-grid-typeahead>',
    },
    {
        name: 'Total Hours',
        field: 'totalHours',
        width: 150,
        type: 'number',
        enableFiltering: false,
    },
    {
        name: 'Invoice #',
        field: 'invoiceNumber',
        width: 100,
        type: 'string',
        cellTemplate:
            '<ui-grid-text-box ' +
                'keypress-events ' +
                'escape="grid.appScope.vm.confirmCancel(row)" ' +
                'row="row" ' +
                'col-field="COL_FIELD" ' +
                'is-cell-editable="' +
                    'row.entity.editable && ' +
                    'grid.appScope.vm.isNotTruckingReports && ' +
                    '!row.entity.downtimeTimerID' +
                '" ' +
                'col-field-key="invoiceNumber"' +
            '></ui-grid-text-box>',
    },
    {
        name: 'Note',
        field: 'note',
        cellClass: 'expand-on-hover',
        enableFiltering: false,
        width: 200,
        type: 'string',
        cellTemplate:
            '<ui-grid-text-box ' +
                'keypress-events ' +
                'escape="grid.appScope.vm.confirmCancel(row)" ' +
                'row="row" ' +
                'col-field="COL_FIELD" ' +
                'is-cell-editable="' +
                    'row.entity.editable && ' +
                    'grid.appScope.vm.isNotTruckingReports && ' +
                    '!row.entity.downtimeTimerID' +
                '" ' +
                'col-field-key="note"' +
            '></ui-grid-text-box>',
    },
    {
        name: 'BillType',
        field: 'billType',
        width: 100,
        type: 'string',
    },
    {
        name: 'Price Per Unit',
        field: 'pricePerUnit',
        width: 150,
        type: 'number',
    },
    {
        name: 'Calculated Revenue',
        field: 'calculatedRevenue',
        width: 200,
        type: 'number',
        enableFiltering: false,
    },
    {
        name: 'Edit',
        width: 100,
        pinnedRight: true,
        enableFiltering: false,
        headerCellTemplate:
            '<div><span class="ui-grid-header-cell-label">Edit</span></div><br />'+
            '<i ' +
                'class="fa fa-lg fa-pencil icon" ' +
                'data-toggle="tooltip" ' +
                'title="Edit All" ' +
                'aria-hidden="true"' +
                'ng-class="{ hide: grid.appScope.vm.editingAll || !grid.appScope.vm.canEditRow }" ' +
                'ng-click="grid.appScope.vm.editAll()" ' +
            '></i>' +
            '<i ' +
                'class="fa fa-lg fa-floppy-o icon" ' +
                'data-toggle="tooltip" ' +
                'title="Save All" ' +
                'aria-hidden="true"' +
                'ng-click="grid.appScope.vm.confirmSaveAll()" ' +
                'ng-class="{ hide: !grid.appScope.vm.editingAll || !grid.appScope.vm.canEditRow }"' +
            '></i>' +
            '<i ' +
                'class="fa fa-lg fa-ban icon" ' +
                'data-toggle="tooltip" ' +
                'title="Cancel" ' +
                'aria-hidden="true" ' +
                'ng-click="grid.appScope.vm.cancelAll()" ' +
                'ng-class="{ hide: !grid.appScope.vm.editingAll || !grid.appScope.vm.canEditRow }"' +
            '></i>',
        cellTemplate:
            '<i ' +
                'ng-if="!row.entity.isChild && grid.appScope.vm.can(\'editTruckerDaily\')" ' +
                'ng-disabled="!grid.appScope.vm.canEditRow" ' +
                'ng-class="{ hide: grid.appScope.vm.editingAll || row.entity.editable }" ' +
                'ng-click="!grid.appScope.vm.canEditRow || grid.appScope.vm.beginEdit(row)" ' +
                'class="fa fa-lg fa-pencil icon" ' +
                'data-toggle="tooltip" ' +
                'title="Edit" ' +
                'aria-hidden="true"' +
            '></i>' +
            //'<i ng-disabled="!grid.appScope.vm.canEditRow || (row.entity.loadTimerEntryID && row.treeNode.parentRow.treeNode.children.length == 1)"' +
            //    'ng-class="{ hide: row.entity.editable || row.entity.downtimeTimerID }"' +
            //    'ng-click="!grid.appScope.vm.canEditRow || (row.entity.loadTimerEntryID && row.treeNode.parentRow.treeNode.children.length == 1) || grid.appScope.vm.deleteLoad(row.entity.sequence, row)"' +
            //    'class="fa fa-lg fa-trash icon" data-toggle="tooltip" title="Delete" aria-hidden="true">' +
            //'</i>' +
            //'<i ng-if="!row.entity.isChild" ng-disabled="!grid.appScope.vm.canEditRow"' +
            //    'ng-class="{ hide: row.entity.editable || (row.entity.loadTimerEntryID || row.entity.downtimeTimerID) }"' +
            //    'ng-click="!grid.appScope.vm.canEditRow || grid.appScope.vm.cloneLoad(row.entity)" ' +
            //    'class="fa fa-lg fa-clone icon" data-toggle="tooltip" title="Clone" aria-hidden="true">' +
            //'</i>' +
            '<i ' +
                'class="fa fa-lg fa-floppy-o icon" ' +
                'data-toggle="tooltip" ' +
                'title="Save" ' +
                'aria-hidden="true"' +
                'ng-disabled="grid.appScope.vm.isInvalid(row)" ' +
                'ng-click="grid.appScope.vm.isInvalid(row) || grid.appScope.vm.saveLoad(row)" ' +
                'ng-class="{ hide: grid.appScope.vm.editingAll || !row.entity.editable || row.entity.isChild }"' +
            '></i>' +
            '<i ' +
                'class="fa fa-lg fa-ban icon" ' +
                'data-toggle="tooltip" ' +
                'title="Cancel" ' +
                'aria-hidden="true" ' +
                'ng-click="grid.appScope.vm.cancelEdit(row)" ' +
                'ng-class="{ hide: grid.appScope.vm.editingAll || !row.entity.editable || row.entity.isChild }"' +
            '></i>',
    },
];