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
            console.log('person:', data);
            service.profile = data;
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

    // Return the service instance
    return service;
  }
})();
