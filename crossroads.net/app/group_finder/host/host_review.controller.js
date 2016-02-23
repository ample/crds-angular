(function(){
  'use strict';

  module.exports = HostReviewCtrl;

  HostReviewCtrl.$inject = [
    '$window',
    '$scope',
    '$state',
    'Responses',
    'Group',
    'AuthenticatedPerson',
    'GROUP_API_CONSTANTS',
    '$log',
    'GroupInvitationService',
    'GROUP_ROLE_ID_HOST'
  ];

  function HostReviewCtrl($window,
                          $scope,
                          $state,
                          Responses,
                          Group,
                          AuthenticatedPerson,
                          GROUP_API_CONSTANTS,
                          $log,
                          GroupInvitationService,
                          GROUP_ROLE_ID_HOST) {
    var vm = this;

    vm.initialize = function() {
      vm.responses = Responses.data;
      vm.host = AuthenticatedPerson;
      $log.debug('Host profile: ', vm.host);

      if(vm.isPrivate()) {
        return $state.go('group_finder.host.confirm');
      }

      var groupTitle = AuthenticatedPerson.displayName();

      vm.group = {
        groupTitle: groupTitle,
        time: vm.getGroupTime(),
        distance: '0 miles from you',
        description: vm.responses.description,
        type: vm.responses.group_type,
        attributes: vm.getGroupAttributes(),
        host: {
          contactId: $scope.person.contactId
        }
      };
    };

    vm.startOver = function() {
      $scope.$parent.currentStep = 1;
      $state.go('group_finder.host.questions');
    };

    /*
      {
        'groupName': 'Sample Group',
        'groupDescription': 'Sample Group Description',
        'groupTypeId': 19,
        'ministryId': 8,
        'congregationId': 1,
        'contactId': 2399608,
        'startDate': '2016-02-01T10:00:00.000Z',
        'endDate': '2016-03-01T10:00:00.000Z',
        'availableOnline': true,
        'remainingCapacity': 10,
        'groupFullInd': false,
        'waitListInd': false,
        'waitListGroupId': 0,
        'childCareInd': false,
        'minAge': 0,
        'meetingDayId': 1,
        'meetingTime': '10:00 AM',
        'groupRoleId': ,
        'address': {
          'addressLine1': '5766 Pandora Ave',
          'addressLine2': '',
          'city': 'Cincinnati',
          'state': 'Oh',
          'zip': '45213',
          'foreignCountry': 'United States',
          'county': '',
        }
      }
     */
    var days = [ 'sunday', 'monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday' ];
    vm.publish = function() {
      vm.rejected = false;

      // Create the Group detail resource
      var group = new Group.Detail();

      // Set constants
      group.groupTypeId = GROUP_API_CONSTANTS.GROUP_TYPE_ID;
      group.ministryId = GROUP_API_CONSTANTS.MINISTRY_ID;
      group.startDate = GROUP_API_CONSTANTS.START_DATE;
      group.endDate = GROUP_API_CONSTANTS.END_DATE;

      // Group owner, name and description
      group.contactId = AuthenticatedPerson.contactId;
      group.groupName = AuthenticatedPerson.displayName();
      group.groupDescription = Responses.data.description;
      group.congregationId = AuthenticatedPerson.congregationId;

      // Group size and availability
      group.availableOnline = true;
      group.remainingCapacity = Responses.data.open_spots;
      group.groupFullInd = Responses.data.open_spots <= 0;
      group.waitListInd = false;
      group.childCareInd = Responses.data.kids === 1;

      // When and where does the group meet
      // TODO Handle this as ordinal in Responses instead of day name string
      group.meetingDayId = days.indexOf(Responses.data.date_and_time.day.toLowerCase());

      group.meetingTime = Responses.data.date_and_time.time + ' ' + Responses.data.date_and_time.ampm;
      group.address = {
        addressLine1: Responses.data.location.street,
        city: Responses.data.location.city,
        state: Responses.data.location.state,
        zip: Responses.data.location.zip
      };

      // Publish the group to the API and handle the response
      $log.debug('Publishing group:', group);
      Group.Detail.save(group).$promise.then(function success(group) {

        $log.debug('Group was published successfully:', group);
        // User invitation service to add person to that group
        var promise = GroupInvitationService.acceptInvitation(group.groupId,
                                                              {capacity: 1, groupRoleId: GROUP_ROLE_ID_HOST});
        promise.then(function() {
          // Invitation acceptance was successful
          vm.accepted = true;
        }, function(error) {
          // An error happened accepting the invitation
          vm.rejected = true;
        }).finally(function() {
          vm.requestPending = false;
        });

        // Created group successfully, go to confirmation page
        $state.go('group_finder.host.confirm');
      }, function error() {
        vm.rejected = true;
        $log.debug('An error occurred while publishing');
      });
    };

    vm.getGroupAttributes = function() {
      var ret = [];
      if (vm.responses.kids === '1') { ret.push('kids welcome'); }
      if (vm.responses.pets) {
        var pet_selections = _.map(Object.keys(vm.responses.pets), function(el) {
          return parseInt(el);
        });
        if (pet_selections.indexOf(0) !== -1) { ret.push('has a cat'); }
        if (pet_selections.indexOf(1) !== -1) { ret.push('has a dog'); }
      }
      return ret;
    };

    vm.getGroupTime = function() {
      var dt = vm.responses.date_and_time;
      if (dt) {
        return dt['day'] + 's @ ' + dt['time'] + dt['ampm'];
      }
    };

    vm.goBack = function() {
      $window.history.back();
    };

    vm.isPrivate = function() {
      return vm.responses && vm.responses.open_spots <= 0;
    };

    // ------------------------------- //

    vm.initialize();

  }

})();
