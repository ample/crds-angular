(function() {
  'use strict';

  module.exports = ChildcareRoutes;

  ChildcareRoutes.$inject = ['$stateProvider'];

  function ChildcareRoutes($stateProvider) {
    $stateProvider
      .state('childcare-event', {
        parent: 'noSideBar',
        url: '/childcare/:eventId',
        template: '<childcare></childcare>',
        data: {
          isProtected: true,
          meta: {
            title: 'Childcare Signup',
            description: 'Choose which of your children you want to enroll in childcare during your event'
          }
        },
        resolve: {
          loggedin: crds_utilities.checkLoggedin,
          $stateParams: '$stateParams',
          EventService: 'EventService',
          ChildCareEvents: 'ChildCareEvents',
          CurrentEvent: function($stateParams, EventService, ChildCareEvents) {
            return EventService.event.get({eventId: $stateParams.eventId}, function(event) {
              ChildCareEvents.setEvent(event);
            }).$promise;
          }
        }
      })
      ;
  }

})();
