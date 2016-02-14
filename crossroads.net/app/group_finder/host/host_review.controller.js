(function(){
  'use strict';

  module.exports = HostReviewCtrl;

  HostReviewCtrl.$inject = ['$scope', '$state', 'Responses'];

  function HostReviewCtrl($scope, $state, Responses) {
    var vm = this;

    vm.responses = Responses;

    vm.startOver = function() {
      $scope.$parent.currentStep = 2;
      $state.go('group_finder.host.questions');
    };

    vm.showDashboard = function() {
      $state.go('group_finder.dashboard');
    };

  }

})();
