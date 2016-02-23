(function(){
  'use strict';

  module.exports = DashboardRoutes;

  DashboardRoutes.$inject = ['$stateProvider', 'SERIES'];

  function DashboardRoutes($stateProvider, SERIES) {

    $stateProvider

      .state('group_finder.dashboard', {
        url: '/dashboard',
        templateUrl: 'dashboard/dashboard.html',
        controller: 'DashboardCtrl as dashboard',
        resolve: {
          AuthenticatedPerson: ['Person', function(Person) {
            return Person.getProfile();
          }]
        },
        data: {
          isProtected: true,
          meta: {
            title: SERIES.title,
            description: ''
          }
        }
      })

      .state('group_finder.dashboard.group', {
        url: '/groups/:groupId',
        controller: 'GroupDetailCtrl as detail',
        templateUrl: 'dashboard/group_detail.html',
        data: {
          meta: {
            title: SERIES.title,
            description: ''
          }
        }
      })
      ;
  }
})();
