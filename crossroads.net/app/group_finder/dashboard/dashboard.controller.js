(function(){
  'use strict';

  module.exports = DashboardCtrl;

  DashboardCtrl.$inject = [
    '$rootScope',
    '$scope',
    '$log',
    '$state',
    'Person',
    'AuthService',
    'Email',
    '$modal',
    'ImageService',
    'GroupInfo'
  ];

  function DashboardCtrl(
    $rootScope,
    $scope,
    $log,
    $state,
    Person,
    AuthService,
    Email,
    $modal,
    ImageService,
    GroupInfo
  ) {

    var vm = this;

    vm.profileData = { person: Person };
    vm.person = Person;
    vm.profileImageBaseUrl = ImageService.ProfileImageBaseURL;
    vm.profileImage = vm.profileImageBaseUrl + vm.person.contactId;
    vm.defaultImage = ImageService.DefaultProfileImage;

    vm.groups = {
      hosting: GroupInfo.getHosting(),
      participating: GroupInfo.getParticipating()
    };

    vm.emailGroup = function() {
      // TODO popup with text block?
      $log.debug('Sending Email to group');
      var modalInstance = $modal.open({
        templateUrl: 'templates/group_contact_modal.html',
        controller: 'GroupContactCtrl as contactModal',
        resolve: {
          fromContactId: function() {
            return vm.person.contactId;
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
        'fromContactId': vm.person.contactId,
        'fromUserId': 0,
        'toContactId': 0,
        'templateId': 0,
        'mergeData': {}
      };
    };

    vm.startOver = function() {
      $state.go('group_finder.summary');
    };

    vm.driveTime = function() {
      // TODO maps api integration to calculate this
      return '18 minute';
    };
    vm.groupType = function() {
      // TODO need lookup of available group types. Waiting on CRDS API to return this value
      return 'co-ed';
    };

    vm.displayName = function() {
      return vm.person.firstName + ' ' + vm.person.lastName[0] + '.';
    };

    $scope.setGroup = function(group) {
      vm.group = group;
    };

    $rootScope.$on('$viewContentLoading', function(event){
      vm.group = undefined;
    });

  }

})();
