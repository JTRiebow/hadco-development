import * as angular from 'angular';

import * as sidebarTemplate from './side-bar-menu.html';
import * as truckingGridsTemplate from './trucking-grids.html';


angular.module('truckingModule', [])
    .constant('sidebarMenuTemplate', sidebarTemplate)
    .constant('truckingGridsTemplate', truckingGridsTemplate);

// import './trucking-service';
import './trucker-dailies-component';
import './pricing-component';
import './edit-pricing-modal-controller';
import './reporting-component';
import './view-pricing-history-modal-controller';
import './trucking-factory';
import './trucking-prices-factory';
import './trucking-pricing-list-factory';
import './trucking-pricings-factory';