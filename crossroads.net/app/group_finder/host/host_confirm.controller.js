(function(){
  'use strict';

  module.exports = HostConfirmCtrl;

  HostConfirmCtrl.$inject = ['$state', 'Responses', '$rootScope'];

  function HostConfirmCtrl($state, Responses, $rootScope) {

    var vm = this;
    vm.responses = Responses;

    vm.showDashboard = function() {
      $state.go('group_finder.dashboard');
    };

    vm.successMessage = function() {
      var message = $rootScope.MESSAGES.groupFinderHostPublicGroup.content;
      if (vm.responses.data.open_spots <= 0) {
        message = $rootScope.MESSAGES.groupFinderHostPrivateGroup.content;
      }
      return message;
    };
  }

})();
