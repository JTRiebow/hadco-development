import * as angular from 'angular';
import * as moment from 'moment';

angular
	.module('adminModule')
	.factory('Notes', NotesFactory);

NotesFactory.$inject = [ 'Restangular' ];

function NotesFactory(Restangular) {
	var service = {
		postNoteForFlaggedTimeCard,
		updateNoteDescription,
		getDailyNotesByEmployee,
		resolve,
	} as INotesService;

	return service;

	function postNoteForFlaggedTimeCard(noteDetails) {
		return Restangular.all("Notes").customPOST(noteDetails);
	}
	
	function updateNoteDescription(id, description) {
		return Restangular.one('Notes', id).customPATCH({ description });
	}

	function getDailyNotesByEmployee(employee) {
		var timerDay = moment(employee.day).format("YYYY-MM-DD");
		return Restangular.one('Employee', employee.employeeID).one('Day', timerDay).one('DepartmentID', employee.departmentID).one('Notes').get();
	}
	
	function resolve(id) {
		return Restangular.one('Notes', id).all('Resolve').customPOST({});
	}
}

interface INote {
	noteID?: number;
	employeeID?: number;
	day?: string|Date;
	departmentID?: number;
	description?: string;
	createdTime?: string|Date;
	createdEmployeeID?: number;
	resolved?: boolean;
	resolvedTime?: string|Date;
	resolvedEmployeeID?: number;
	noteTypeID?: number;
	modifiedTime?: string|Date;
}

interface INotesService {
	postNoteForFlaggedTimeCard(INote): ng.IPromise<INote>;
	updateNoteDescription(id: number, description: string): ng.IPromise<INote>;
	getDailyNotesByEmployee(employee): ng.IPromise<INote[]>;
	resolve(id: number): ng.IPromise<INote>;
}

export {
	INote,
	INotesService,
};