import * as angular from 'angular';

angular.module('adminModule', [ 'restangular' ]);

//units
import './units/units-service';
import './units/units-component';
import './units/create-or-edit-units-component';

// trucks
import './trucks/truck-helper-factory';
import './trucks/truck-service';

// truck classifications
import './truck-classifications/create-or-edit-truck-classifications-component';
import './truck-classifications/truck-classifications-component';
import './truck-classifications/truck-classifications-helper-factory';
import './truck-classifications/truck-classifications-service';

// categories
import './categories/categories-helper-factory';
import './categories/categories-service';

// trailers
import './trailers/trailer-helper-factory';
import './trailers/trailer-service';

// timesheets
import './timesheets/timesheets-helper-factory';
import './timesheets/timesheets-service';

// occurences
import './occurrences/create-or-edit-occurrence-component';
import './occurrences/occurrences-component';
import './occurrences/occurrences-helper-factory';
import './occurrences/occurrences-service';

// note timers
import './note-timers/timer-flag-note-service';

// materials
import './materials/create-or-edit-material-component';
import './materials/materials-component';
import './materials/materials-helper-factory';
import './materials/materials-service';

// locations
import './locations/locations-service';

// jobs
import './jobs/jobs-service';
import './jobs/jobs-helper-factory';
import './jobs/create-or-edit-job-component';
import './jobs/jobs-component';
import './jobs/job-locations-service';
import './jobs/phase/phases-component';

// gps settings
import './gps-settings/gps-settings-component';
import './gps-settings/gps-settings-service';
import './gps-settings/selected-employees-component';

// equipment service types
import './equipment-service-types/equipment-service-types-helper-factory';
import './equipment-service-types/equipment-types-service';

// equipment
import './equipment/equipment-helper-factory';
import './equipment/equipment-service';

// downtime reasons
import './downtime-reasons/create-or-edit-downtime-reasons-component';
import './downtime-reasons/downtime-reasons-component';
import './downtime-reasons/downtime-reasons-helper-factory';
import './downtime-reasons/downtime-reasons-service';

// departments
import './departments/departments-helper-factory';
import './departments/departments-service';

// categories
import './categories/categories-helper-factory';
import './categories/categories-service';

// bill types
import './bill-types/bill-types-helper-factory';
import './bill-types/bill-types-service';

// roles
import './roles';