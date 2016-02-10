(function(){
  'use strict';

  module.exports = DashboardCtrl;

  DashboardCtrl.$inject = [
    '$scope',
    '$log',
    '$state',
    'Profile',
    'Person',
    'AuthService',
    'User',
    'SERIES',
    'Email',
    '$modal'
  ];

  function DashboardCtrl(
    $scope,
    $log,
    $state,
    Profile,
    Person,
    AuthService,
    User,
    SERIES,
    Email,
    $modal
  ) {

    var vm = this;

    if (AuthService.isAuthenticated() === false) {
      $log.debug('not logged in');
      $state.go(SERIES.permalink + '.welcome');
    }

    vm.profileData = { person: Person };
    vm.groups = User.groups;
    vm.invitee = ''; // empty invitee input text
    vm.tabs = [
      { title:'Resources', active: false, route: 'dashboard.resources' },
      { title:'My Groups', active: true, route: 'dashboard.groups'},
    ];

    vm.emailGroup = function() {
      // TODO popup with text block?
      $log.debug('Sending Email to group');
      var modalInstance = $modal.open({
        templateUrl: 'templates/group_contact_modal.html',
        controller: 'GroupContactCtrl as contactModal',
        resolve: {
          fromContactId: function() {
            return vm.profileData.person.contactId;
          },
          toContactIds: function() {
            return _.map(vm.groups[0].members, function(member) {return member.contactId;});
          }
        }
      });

      modalInstance.result.then(function (selectedItem) {
        $scope.selected = selectedItem;
      }, function () {
        $log.info('Modal dismissed at: ' + new Date());
      });
    };

    vm.inviteMember = function(email) {
      // TODO add validation. Review how to send email without `toContactId`
      $log.debug('Sending Email to: ' + email);
      var toSend = {
        'fromContactId': vm.profileData.person.contactId,
        'fromUserId': 0,
        'toContactId': 0,
        'templateId': 0,
        'mergeData': {}
      };

    };
    vm.startOver = function() {
      $state.go(SERIES.permalink + '.summary');
    };

    vm.driveTime = function() {
      // TODO maps api integration to calculate this
      return '18 minute';
    };
    vm.groupType = function() {
      // TODO need lookup of available group types. Waiting on CRDS API to return this value
      return 'co-ed';
    };
  }

})();
