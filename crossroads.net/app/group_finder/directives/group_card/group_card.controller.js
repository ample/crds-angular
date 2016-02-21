(function(){
  'use strict';

  module.exports = GroupCardCtrl;

  GroupCardCtrl.$inject = ['$scope', 'ImageService', 'GROUP_TYPES', '$modal'];

  function GroupCardCtrl($scope, ImageService, GROUP_TYPES, $modal) {

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
      return GROUP_TYPES[$scope.group.type];
    };

    $scope.groupDescription = function() {
      return 'A ' + $scope.getGroupType() + ' group meeting on ' + $scope.group.time;
    };

    $scope.getTemplateUrl = function() {
      return 'group_card/group_' + $scope.template + '.html';
    };

    $scope.joinGroup = function(groupId) {

      $scope.groupId = groupId;

      var modalInstance = $modal.open({
        templateUrl: 'group_card/join_modal.html',
        controller: 'JoinModalCtrl as modal',
        scope: $scope
      });
    };

  }

})();
