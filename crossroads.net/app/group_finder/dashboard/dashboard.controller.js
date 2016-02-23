(function(){
  'use strict';

  module.exports = DashboardCtrl;

  DashboardCtrl.$inject = [
    '$rootScope',
    '$scope',
    'ImageService',
    'GroupInfo',
    'AuthenticatedPerson'
  ];

  function DashboardCtrl(
    $rootScope,
    $scope,
    ImageService,
    GroupInfo,
    AuthenticatedPerson
  ) {

    var vm = this;

    vm.person = AuthenticatedPerson;
    vm.profileImageBaseUrl = ImageService.ProfileImageBaseURL;
    vm.profileImage = vm.profileImageBaseUrl + vm.person.contactId;
    vm.defaultImage = ImageService.DefaultProfileImage;

    vm.groups = {
      hosting: GroupInfo.getHosting(),
      participating: GroupInfo.getParticipating()
    };

    $scope.setGroup = function(group) {
      vm.group = group;
    };

    $rootScope.$on('$viewContentLoading', function(event){
      vm.group = undefined;
    });
  }

})();
