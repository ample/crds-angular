(function() {
  'use strict';

  module.exports = GroupDetailCtrl;

  GroupDetailCtrl.$inject = ['$scope', '$stateParams', '$modal', 'GroupInfo'];

  function GroupDetailCtrl($scope, $stateParams, $modal, GroupInfo) {
    var vm = this;

    vm.group = GroupInfo.findHosting($stateParams.groupId);

    vm.emailGroup = function() {
      var modalInstance = $modal.open({
        templateUrl: 'templates/group_contact_modal.html',
        controller: 'GroupContactCtrl as contactModal',
        resolve: {
          fromContactId: function() {
            return vm.group.host.contactId;
          },
          toContactIds: function() {
            return _.map(vm.group.members, function(member) {return member.contactId;});
          }
        }
      });

      modalInstance.result.then(function (selectedItem) {
        $scope.selected = selectedItem;
      });
    };

    $scope.$on('$viewContentLoaded', function(event){
      $scope.$parent.setGroup(vm.group);
    });
  }

})();
