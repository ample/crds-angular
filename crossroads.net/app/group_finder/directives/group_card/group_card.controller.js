(function(){
  'use strict';

  module.exports = GroupCardCtrl;

  GroupCardCtrl.$inject = ['$scope', 'ImageService', 'GROUP_ROLE_ID_PARTICIPANT', 'Address'];

  function GroupCardCtrl($scope, ImageService, GROUP_ROLE_ID_PARTICIPANT, Address) {

    $scope.GROUP_ROLE_ID_PARTICIPANT = GROUP_ROLE_ID_PARTICIPANT;
    $scope.defaultImage = ImageService.DefaultProfileImage;

    $scope.getMemberImage = function(member) {
      return $scope.getUserImage(member.contactId);
    };

    $scope.getHostImage = function(host) {
      return $scope.getUserImage(host.contactId);
    };

    $scope.getUserImage = function(contactId) {
      return ImageService.ProfileImageBaseURL + parseInt(contactId);
    };

    $scope.getGroupType = function() {
      return $scope.group.type ? 'A group of ' + $scope.group.type : 'A group';
    };

    $scope.groupDescription = function() {
      return $scope.getGroupType() + ' meeting on ' + $scope.groupTime();
    };

    $scope.getTemplateUrl = function() {
      return 'group_card/group_' + $scope.template + '.html';
    };

    $scope.mapAddress = Address.mapLink;

    $scope.groupTime = function() {
      return $scope.group.meetingDay + ', ' + $scope.group.meetingHour;
    };

  }

})();
