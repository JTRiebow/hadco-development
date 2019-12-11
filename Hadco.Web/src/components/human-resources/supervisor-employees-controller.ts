import * as angular from 'angular';

angular
    .module('humanResourcesModule')
    .controller('htSupervisorEmployees', supervisorEmployeesController);

supervisorEmployeesController.$inject = [
    '$scope',
    '$location',
    'Pagination',
    '$modal',
    'NotificationFactory',
    'Users',
    '$routeParams',
];

function supervisorEmployeesController(
    $scope,
    $location,
    Pagination,
    $modal,
    NotificationFactory,
    Users,
    $routeParams,
) {

    // $scope.search = $location.search().search
    
    // $scope.employees = [
    //     {name: 'Justin Calder', employeeId: 1, day1: {id: 1, hours: 5}, day2: {id: 2, hours: 5}, day3: {id: 3, hours: 5}, day4: {id: 4, hours: 5}, day5: {id: 5, hours: 5}, day6: {id: 6, hours: 5}, weeklyTotal: 30, division: 'TMCrushing', notes: 'Duis eu commodo risus, quis finibus risus. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Duis '},
    //     {name: 'Eric Winegar', employeeId: 2, day1: {id: 1, hours: 5}, day2: {id: 2, hours: 5}, day3: {id: 3, hours: 5}, day4: {id: 4, hours: 5}, day5: {id: 5, hours: 5}, day6: {id: 6, hours: 5}, weeklyTotal: 30, division: 'Concrete', notes: 'Tri-tip kevin porchetta, filet mignon swine spare ribs shank picanha jerky sirloin short ribs leberkas beef. Tail shankle chicken short ribs.'},
    //     {name: 'Kade Brewer', employeeId: 3, day1: {id: 1, hours: 5}, day2: {id: 2, hours: 5}, day3: {id: 3, hours: 5}, day4: {id: 4, hours: 5}, day5: {id: 5, hours: 5}, day6: {id: 6, hours: 5}, weeklyTotal: 30, division: 'Concrete', notes: 'Proin tristique lorem id turpis elementum, in venenatis mi sollicitudin. Aliquam finibus diam sed vehicula condimentum. Donec ex risus, posuere semper pulvinar ac, molestie eget felis.'},
    //     {name: 'Ryan Shipp', employeeId: 4, day1: {id: 1, hours: 5}, day2: {id: 2, hours: 5}, day3: {id: 3, hours: 5}, day4: {id: 4, hours: 5}, day5: {id: 5, hours: 5}, day6: {id: 6, hours: 5}, weeklyTotal: 30, division: 'Shop', notes: 'Cras vulputate, lectus vel pellentesque pellentesque, felis nisl ullamcorper purus, iaculis pulvinar justo libero quis tellus. Proin consequat purus in eleifend faucibus.'},
    //     {name: 'Logan Bunker', employeeId: 5, day1: {id: 1, hours: 5}, day2: {id: 2, hours: 5}, day3: {id: 3, hours: 5}, day4: {id: 4, hours: 5}, day5: {id: 5, hours: 5}, day6: {id: 6, hours: 5}, weeklyTotal: 30, division: 'Shop', notes: 'Tri-tip kevin porchetta, filet mignon swine spare ribs shank picanha jerky sirloin short ribs leberkas beef. Tail shankle chicken short ribs.'},
    //     {name: 'Claudia Oliva', employeeId: 6, day1: {id: 1, hours: 5}, day2: {id: 2, hours: 5}, day3: {id: 3, hours: 5}, day4: {id: 4, hours: 5}, day5: {id: 5, hours: 5}, day6: {id: 6, hours: 5}, weeklyTotal: 30, division: 'Office', notes: 'Fusce dapibus nisl mauris, eget venenatis felis fermentum in. In at ante luctus, venenatis orci non, laoreet lacus. Nam rhoncus neque sed nunc aliquam vulputate.'},
    //     {name: 'Justin Calder', employeeId: 7, day1: {id: 1, hours: 5}, day2: {id: 2, hours: 5}, day3: {id: 3, hours: 5}, day4: {id: 4, hours: 5}, day5: {id: 5, hours: 5}, day6: {id: 6, hours: 5}, weeklyTotal: 30, division: 'TMCrushing', notes: 'Duis eu commodo risus, quis finibus risus. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Duis '},
    //     {name: 'Eric Winegar', employeeId: 8, day1: {id: 1, hours: 5}, day2: {id: 2, hours: 5}, day3: {id: 3, hours: 5}, day4: {id: 4, hours: 5}, day5: {id: 5, hours: 5}, day6: {id: 6, hours: 5}, weeklyTotal: 30, division: 'Concrete', notes: 'Tri-tip kevin porchetta, filet mignon swine spare ribs shank picanha jerky sirloin short ribs leberkas beef. Tail shankle chicken short ribs.'},
    //     {name: 'Kade Brewer', employeeId: 9, day1: {id: 1, hours: 5}, day2: {id: 2, hours: 5}, day3: {id: 3, hours: 5}, day4: {id: 4, hours: 5}, day5: {id: 5, hours: 5}, day6: {id: 6, hours: 5}, weeklyTotal: 30, division: 'Concrete', notes: 'Proin tristique lorem id turpis elementum, in venenatis mi sollicitudin. Aliquam finibus diam sed vehicula condimentum. Donec ex risus, posuere semper pulvinar ac, molestie eget felis.'},
    //     {name: 'Ryan Shipp', employeeId: 10, day1: {id: 1, hours: 5}, day2: {id: 2, hours: 5}, day3: {id: 3, hours: 5}, day4: {id: 4, hours: 5}, day5: {id: 5, hours: 5}, day6: {id: 6, hours: 5}, weeklyTotal: 30, division: 'Shop', notes: 'Cras vulputate, lectus vel pellentesque pellentesque, felis nisl ullamcorper purus, iaculis pulvinar justo libero quis tellus. Proin consequat purus in eleifend faucibus.'},
    //     {name: 'Logan Bunker', employeeId: 11, day1: {id: 1, hours: 5}, day2: {id: 2, hours: 5}, day3: {id: 3, hours: 5}, day4: {id: 4, hours: 5}, day5: {id: 5, hours: 5}, day6: {id: 6, hours: 5}, weeklyTotal: 30, division: 'Shop', notes: 'Tri-tip kevin porchetta, filet mignon swine spare ribs shank picanha jerky sirloin short ribs leberkas beef. Tail shankle chicken short ribs.'},
    //     {name: 'Claudia Oliva', employeeId: 18, day1: {id: 1, hours: 5}, day2: {id: 2, hours: 5}, day3: {id: 3, hours: 5}, day4: {id: 4, hours: 5}, day5: {id: 5, hours: 5}, day6: {id: 6, hours: 5}, weeklyTotal: 30, division: 'Office', notes: 'Fusce dapibus nisl mauris, eget venenatis felis fermentum in. In at ante luctus, venenatis orci non, laoreet lacus. Nam rhoncus neque sed nunc aliquam vulputate.'},
    //     {name: 'Justin Calder', employeeId: 19, day1: {id: 1, hours: 5}, day2: {id: 2, hours: 5}, day3: {id: 3, hours: 5}, day4: {id: 4, hours: 5}, day5: {id: 5, hours: 5}, day6: {id: 6, hours: 5}, weeklyTotal: 30, division: 'TMCrushing', notes: 'Duis eu commodo risus, quis finibus risus. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Duis '},
    //     {name: 'Eric Winegar', employeeId: 20, day1: {id: 1, hours: 5}, day2: {id: 2, hours: 5}, day3: {id: 3, hours: 5}, day4: {id: 4, hours: 5}, day5: {id: 5, hours: 5}, day6: {id: 6, hours: 5}, weeklyTotal: 30, division: 'Concrete', notes: 'Tri-tip kevin porchetta, filet mignon swine spare ribs shank picanha jerky sirloin short ribs leberkas beef. Tail shankle chicken short ribs.'},
    //     {name: 'Kade Brewer', employeeId: 21, day1: {id: 1, hours: 5}, day2: {id: 2, hours: 5}, day3: {id: 3, hours: 5}, day4: {id: 4, hours: 5}, day5: {id: 5, hours: 5}, day6: {id: 6, hours: 5}, weeklyTotal: 30, division: 'Concrete', notes: 'Proin tristique lorem id turpis elementum, in venenatis mi sollicitudin. Aliquam finibus diam sed vehicula condimentum. Donec ex risus, posuere semper pulvinar ac, molestie eget felis.'},
    //     {name: 'Logan Bunker', employeeId: 35, day1: {id: 1, hours: 5}, day2: {id: 2, hours: 5}, day3: {id: 3, hours: 5}, day4: {id: 4, hours: 5}, day5: {id: 5, hours: 5}, day6: {id: 6, hours: 5}, weeklyTotal: 30, division: 'Shop', notes: 'Tri-tip kevin porchetta, filet mignon swine spare ribs shank picanha jerky sirloin short ribs leberkas beef. Tail shankle chicken short ribs.'},
    //     {name: 'Claudia Oliva', employeeId: 36, day1: {id: 1, hours: 5}, day2: {id: 2, hours: 5}, day3: {id: 3, hours: 5}, day4: {id: 4, hours: 5}, day5: {id: 5, hours: 5}, day6: {id: 6, hours: 5}, weeklyTotal: 30, division: 'Office', notes: 'Fusce dapibus nisl mauris, eget venenatis felis fermentum in. In at ante luctus, venenatis orci non, laoreet lacus. Nam rhoncus neque sed nunc aliquam vulputate.'},
    // ];

    // $scope.pagination = {
    //     itemsPerPage: 10,
    //     totalItems: $scope.supervisors.length,
    //     currentPage: $location.search().page || 1
    // };

    // var pageChanged = function(page, itemsPerPage) {
    //     $location.search('page', page);
    //     var start = Pagination.skip(page, itemsPerPage)
    //     $scope.showSupervisors = $scope.supervisors.slice(start, start + itemsPerPage)
    //     // Claims.getList({
    //     //     search: $scope.search,
    //     //     searchFields: 'CompanyName',
    //     //     skip: Pagination.skip(page, itemsPerPage),
    //     //     take: itemsPerPage,
    //     //     orderBy: 'CompanyName'
    //     // })
    //     // .then(function (response) {
    //     //     $scope.totalClaims = response;
    //     //     $scope.pagination.totalItems = response.meta.totalResultCount;
    //     // });
    // };

    // $scope.$watch('pagination.currentPage', function(newValue, oldValue) {
    //     pageChanged(newValue, $scope.pagination.itemsPerPage);
    // });




}