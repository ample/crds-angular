(function() {
  'use strict';

  module.exports = ProfileRoutes;

  ProfileRoutes.$inject = [
    '$stateProvider',
    '$urlMatcherFactoryProvider',
    '$locationProvider'
  ];

  function ProfileRoutes(
      $stateProvider,
      $urlMatcherFactoryProvider,
      $locationProvider) {

    $stateProvider
      .state('profile', {
        parent: 'noSideBar',
        abstract: true,
        url: '/profile',
        templateUrl: 'profile/profile.html',
        controller: 'ProfileController as profile',
        resolve: {
          loggedin: crds_utilities.checkLoggedin,

          AttributeTypeService: 'AttributeTypeService',
          Profile: 'Profile',
          $stateParams: '$stateParams',
          $cookies: '$cookies',

          AttributeTypes: function(AttributeTypeService) {
            return AttributeTypeService.AttributeTypes().query().$promise;
          },

          Person: function(Profile, $stateParams, $cookies) {
            var cid = $cookies.get('userId');
            if ($stateParams.contactId) {
              cid = $stateParams.contactId;
            }

            return Profile.Person.get({contactId: cid}).$promise;
          },
        },
        data: {
          isProtected: true,
          meta: {
            title: 'Profile',
            description: ''
          }
        }
      })
      .state('profile.personal', {
        url: '/personal',
        templateUrl: 'personal/profilePersonal.html'
      })
      .state('profile.account', {
        url: '/account',
        templateUrl: 'account/profileAccount.html',
      })
      .state('profile.skills', {
        url: '/skills',
        template: '<profile-skills> </profile-skills>',
      })
      .state('profile.giving', {
        url: '/giving',
        controller: 'ProfileGivingController as giving_profile_controller',
        templateUrl: 'giving/profileGiving.html',
      })
      ;
  }
})();
