(function(){
  'use strict';

  module.exports = JoinReviewCtrl;

  JoinReviewCtrl.$inject = ['$scope', '$state', 'Responses', 'ZipcodeService'];

  function JoinReviewCtrl($scope, $state, Responses, ZipcodeService) {
    var vm = this;

    vm.responses = Responses;
    vm.showUpsell = parseInt(vm.responses.data.prior_participation) > 2;
    vm.showResults = vm.showUpsell === false;
    vm.contactCrds = false;
    Responses.data.completedQa = true;

    if (vm.responses.data.location && vm.responses.data.location.zip) {
      vm.zipcode = parseInt(vm.responses.data.location.zip);
      if (ZipcodeService.isLocalZipcode(vm.zipcode) === false) {
        vm.showUpsell = false;
        vm.showResults = false;
        vm.contactCrds = true;
      }
    }

    if (vm.showResults === true && vm.contactCrds === false) {
      $state.go('group_finder.join.results');
    }

    if (parseInt(vm.responses.data.relationship_status) === 2) {
      $scope.showInvite = true;
    }

    vm.goToHost = function() {
      $state.go('group_finder.host');
    };

    vm.goToResults = function() {
      $state.go('group_finder.join.results');
    };

  }

})();
