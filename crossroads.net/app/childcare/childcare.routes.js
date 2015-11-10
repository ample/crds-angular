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
          ChildCareEvents: 'ChildcareEvents',
          ChildcareService: 'ChildcareService',

          CurrentEvent: function($stateParams, EventService, ChildCareEvents) {
            return EventService.event.get({eventId: $stateParams.eventId}, function(event) {
              ChildCareEvents.setEvent(event);
            }).$promise;
          },

          ChildcareEvent: function(ChildCareEvents, ChildcareService, $stateParams) {
            return ChildcareService.ChildcareEvent.get({eventId: $stateParams.eventId}, function(event) {
              ChildCareEvents.setChildcareEvent(event);
            }).$promise;
          },

          Children: function(ChildCareEvents) {
            var children = [
              {
                id: 98765,
                firstName: 'baby',
                lastName: 'Silbernagel',
                age: 1,
                grade: 0
              },
              {
                id: 9234265,
                firstName: 'Miles',
                lastName: 'Silbernagel',
                age: 7,
                grade: 2
              }
            ];
            ChildCareEvents.setChildren(children);
            return children;
          }
        }
      })
      ;
  }

})();
