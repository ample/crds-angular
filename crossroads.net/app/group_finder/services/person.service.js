(function() {
  'use strict';

  module.exports = PersonService;

  PersonService.$inject = ['$rootScope', '$cookies', 'Profile', 'AUTH_EVENTS'];

  function PersonService($rootScope, $cookies, Profile, AUTH_EVENTS) {
    var promise = null;

    //
    // Authenticated Person Info Service
    //

    var service = {};
    service.loadProfile = loadProfile;
    service.getProfile = getProfile;

    //
    // Listen for the logout event notification to clear the data
    //

    $rootScope.$on(AUTH_EVENTS.logoutSuccess, clearData);

    //
    // Service Implementation
    //

    function loadProfile() {
      if (!promise) {
        var cid = $cookies.get('userId');
        if (cid) {
          promise = Profile.Person.get({contactId: cid}).$promise;

          promise.then(function(data) {
            service.profile = decorateProfile(data);
          });
        }
      }

      return promise;
    }

    function getProfile() {
      var loadPromise = loadProfile();
      return loadPromise.then(function() {
        return service.profile;
      });
    }

    function clearData() {
      promise = null;
      delete service.profile;
    }

    //
    // Decorate the Profile with convenience methods
    //

    function decorateProfile(profile) {
      profile.displayName = displayName;

      return profile;
    }

    function displayName() {
      var profile = service.profile;
      var name = profile.firstName || '';

      if (profile.lastName) {
        name = name + ' ' + profile.lastName[0] + '.';
      }

      return name;
    }

    // Return the service instance
    return service;
  }
})();
