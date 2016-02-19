(function(){
  'use strict';

  module.exports = GroupCardCtrl;

  GroupCardCtrl.$inject = ['$scope', 'ImageService', 'GROUP_TYPES', '$state', '$modal', '$log'];

  function GroupCardCtrl($scope, ImageService, GROUP_TYPES, $state, $modal, $log) {

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
      return 'A ' + $scope.getGroupType() + ' group meeting on ' + $scope.groupTime();
    };

    $scope.getTemplateUrl = function() {
      return 'group_card/group_' + $scope.template + '.html';
    };

    $scope.mapAddress = function() {
      var address = $scope.group.address;
      var searchAddress = address.addressLine1 + ', ' + address.city + ', ' + address.state + ', ' + address.zip;
      return 'https://maps.google.com/?q=' + searchAddress.replace(/\s/g, '+');
    };

    $scope.groupTime = function() {
      var meetingTime = moment().format('YYYY-MM-DD') + ' ' + $scope.group.meetingTime;
      return moment().isoWeekday($scope.group.meetingDayId).format('dddd') + ', ' + moment(meetingTime).format('h a');
    };

  }

})();
