(function() {
  'use strict';

  module.exports = TripRoutes;

  TripRoutes.$inject = ['$stateProvider', '$urlMatcherFactoryProvider'];

  function TripRoutes($stateProvider, $urlMatcherFactory) {

    $stateProvider
      .state('tripsearch', {
        parent: 'noSideBar',
        url: '/trips/search',
        controller: 'TripSearchCtrl as tripSearch',
        templateUrl: 'tripsearch/tripsearch.html',
        resolve: {
          Page: 'Page',
          CmsInfo: function(Page, $stateParams) {
            return Page.get({
              url: '/tripgiving/'
            }).$promise;
          }
        }
      })
      .state('tripgiving', {
        parent: 'noSideBar',
        url: '/trips/giving/:eventParticipantId',
        controller: 'TripGivingController as tripGiving',
        templateUrl: 'tripgiving/tripgiving.html',
        resolve: {
          Trip: 'Trip',
          $stateParams: '$stateParams',
          TripParticipant: function(Trip, $stateParams) {
            return Trip.TripParticipant.get({
              tripParticipantId: $stateParams.eventParticipantId
            }).$promise;
          }
        }
      })
      .state('tripgiving.amount', {
        templateUrl: 'tripgiving/amount.html'
      })
      .state('tripgiving.login', {
        controller: 'LoginCtrl',
        templateUrl: 'tripgiving/login.html'
      })
      .state('tripgiving.register', {
        controller: 'RegisterCtrl',
        templateUrl: 'tripgiving/register.html'
      })
      .state('tripgiving.confirm', {
        templateUrl: 'tripgiving/confirm.html'
      })
      .state('tripgiving.account', {
        templateUrl: 'tripgiving/account.html'
      })
      .state('tripgiving.change', {
        templateUrl: 'tripgiving/change.html'
      })
      .state('tripgiving.thank-you', {
        templateUrl: 'tripgiving/thank_you.html'
      })
      .state('mytrips', {
        parent: 'noSideBar',
        url: '/trips/mytrips',
        controller: 'MyTripsController as tripsController',
        templateUrl: 'mytrips/mytrips.html',
        data: {
          isProtected: true
        },
        resolve: {
          loggedin: crds_utilities.checkLoggedin,
          Trip: 'Trip',
          $cookies: '$cookies',
          MyTrips: function(Trip, $cookies) {
            return Trip.MyTrips.get({
              contact: $cookies.get('userId')
            }).$promise;
          }
        }
      })
      .state('tripsignup', {
        parent: 'noSideBar',
        url: '/trips/:campaignId/signup',
        templateUrl: 'signup/signupPage.html',
        controller: 'TripsSignupController as tripsSignup',
        data: {
          isProtected: true,
        },
        resolve: {
          loggedin: crds_utilities.checkLoggedin,
          Trip: 'Trip',
          $stateParams: '$stateParams',
          Campaign: function(Trip, $stateParams) {
            return Trip.Campaign.get({campaignId: $stateParams.campaignId}).$promise;
          },

          WorkTeams: function(Trip) {
            return Trip.WorkTeams.query().$promise;
          },
        },
      })
      ;
  }

})();
