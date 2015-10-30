(function() {
  'use strict';

  module.exports = ProfileController;

  ProfileController.$inject = [
    '$rootScope',
    '$state',
    'AttributeTypes',
    'Person',
    'Profile',
    'Lookup',
    'Locations'];

  function ProfileController($rootScope, $state, AttributeTypes, Person, Profile, Lookup, Locations) {

    var vm = this;
    vm.attributeTypes = AttributeTypes;
    vm.buttonText = 'Save';
    vm.displayLocation = displayLocation;
    vm.enforceAgeRestriction = enforceAgeRestriction;
    vm.goToTab = goToTab;
    vm.locations = Locations;
    vm.locationFocus = locationFocus;
    vm.profileData = { person: Person };
    vm.subscriptions = Profile.Subscriptions.query();
    vm.saveSubscription = saveSubscription;
    vm.tabs = getTabs();

    activate();

    ////////////
    function activate() {

      _.forEach(vm.tabs, function(tab) {
        tab.active = $state.current.name === tab.route;
      });

    }

    function displayLocation() {
      var locationName;
      if (vm.profileData.person.congregationId !== 2) {
        locationName = _.result(
          _.find(
            vm.locations,
            'dp_RecordID',
            vm.profileData.person.congregationId),
            'dp_RecordName');
      }

      if (!locationName) {
        return 'Select Crossroads Site...';
      }

      return locationName;
    }

    function enforceAgeRestriction() {
      return 13;
    }

    function getTabs() {
      return [
        { title:'Personal', active: false, route: 'profile.personal' },
        { title:'Communications', active: false, route: 'profile.communications' },
        { title:'Skills', active: false, route: 'profile.skills' },
        { title: 'Giving History', active: false, route: 'profile.giving' }
      ];
    }

    function goToTab(tab) {
      $state.go(tab.route);
    }

    function locationFocus() {
      $rootScope.$emit('locationFocus');
    }

    function saveSubscription(subscription) {
      if (subscription.Subscription) {
        subscription.Subscription.Unsubscribed = !subscription.Subscribed;
      } else {
        subscription.Subscription = {
          Publication_ID: subscription.ID,
          Publication_Title: subscription.Title,
          Unsubscribed: !subscription.Subscribed
        };

      }

      Profile.Subscriptions.save(subscription.Subscription).$promise.then(function(data) {
        subscription.Subscription.dp_RecordID = data.dp_RecordID;
      });
    }
  }
})();
