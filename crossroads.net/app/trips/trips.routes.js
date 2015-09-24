(function() {
  'use strict';

  module.exports = TripRoutes;

  TripRoutes.$inject = ['$stateProvider', '$urlMatcherFactoryProvider', '$locationProvider'];

  function TripRoutes($stateProvider, $urlMatcherFactory, $locationProvider) {

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
        },
        data: {
          meta: {
            title: 'Trip Search',
            description: ''
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
        },
        data: {
          meta: {
            title: 'Trip Giving',
            description: ''
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
          isProtected: true,
          meta: {
            title: 'My Trips',
            description: ''
          }
        },
        resolve: {
          loggedin: crds_utilities.checkLoggedin,
          Trip: 'Trip',
          $cookies: '$cookies',
          MyTrips: function(Trip, $cookies) {
            return Trip.MyTrips.get().$promise;
          }
        }
      })
      .state('tripsignup', {
        parent: 'noSideBar',
        url: '/trips/:campaignId?invite',
        templateUrl: 'page0/page0.html',
        controller: 'Page0Controller as page0',
        data: {
          isProtected: true,
          meta: {
            title: 'Trip Signup',
            description: 'Select the family member you want to signup for a trip'
          }
        },
        resolve: {
          loggedin: crds_utilities.checkLoggedin,
          $cookies: '$cookies',
          Trip: 'Trip',
          $stateParams: '$stateParams',
          Campaign: function(Trip, $stateParams) {
            return Trip.Campaign.get({campaignId: $stateParams.campaignId}).$promise;
          },

          Family: function(Trip, $stateParams) {
            return Trip.Family.query({pledgeCampaignId: $stateParams.campaignId}).$promise;
          }
        }
      })
      .state('tripsignup.application', {
        parent: 'noSideBar',
        url: '/trips/:campaignId/signup/:contactId?invite',
        templateUrl: 'signup/signupPage.html',
        controller: 'TripsSignupController as tripsSignup',
        data: {
          isProtected: true,
          meta: {
            title: 'Trip Signup',
            description: ''
          }
        },
        resolve: {
          loggedin: crds_utilities.checkLoggedin,
          $cookies: '$cookies',
          contactId: function($cookies) {
            return $cookies.get('userId');
          },

          Trip: 'Trip',
          $stateParams: '$stateParams',
          Campaign: function(Trip, $stateParams) {
            return Trip.Campaign.get({campaignId: $stateParams.campaignId}).$promise;
          },

          Profile: 'Profile',
          Person: function(Profile, $stateParams, $cookies) {
            var cid = $cookies.get('userId');
            if ($stateParams.contactId) {
              cid = $stateParams.contactId;
            }

            return Profile.Person.get({contactId: cid}).$promise;
          },

          Lookup: 'Lookup',

          WorkTeams: function(Trip) {
            return Trip.WorkTeams.query().$promise;
          },
        }
      })
      .state('tripsignup.application.page1', {
        url: '/1',
        templateUrl: 'pageTemplates/signupPage1.html'
      })
      .state('tripsignup.application.page2', {
        url: '/2',
        templateUrl: 'pageTemplates/signupPage2.html'
      })
      .state('tripsignup.application.page3', {
        url: '/3',
        templateUrl: 'pageTemplates/signupPage3.html'
      })
      .state('tripsignup.application.page4', {
        url: '/4',
        templateUrl: 'pageTemplates/signupPage4.html'
      })
      .state('tripsignup.application.page5', {
        url: '/5',
        templateUrl: 'pageTemplates/signupPage5.html'
      })
      .state('tripsignup.application.page6', {
        url: '/6',
        templateUrl: 'pageTemplates/signupPage6.html'
      })
      .state('tripsignup.application.thankyou', {
        url: '/thankyou',
        templateUrl: 'pageTemplates/thankYou.html',
      });

  }

})();
