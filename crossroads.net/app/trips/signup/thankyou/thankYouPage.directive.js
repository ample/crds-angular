(function() {
  'use strict';

  module.exports = ThankYouPageDirective;

  ThankYouPageDirective.$inject = ['$stateParams', 'Trip', 'TripsSignupService'];

  function ThankYouPageDirective($stateParams, Trip, TripsSignupService) {
    return {
      restrict: 'E',
      replace: true,
      scope: {
        currentPage: '=',
        pageTitle: '=',
        numberOfPages: '=',
      },
      templateUrl: 'thankyou/thankYou.html',
      controller: 'PagesController as pages',
      bindToController: true,
      link: link,
    };

    function link(scope, el, attr, vm) {
      vm.loading = true;
      vm.thankYou = TripsSignupService.thankYouMessage;
      activate();

      function activate() {
        Trip.Family.query({pledgeCampaignId: $stateParams.campaignId}, function(data) {
          vm.thankYouFamilyMembers = data;
          vm.loading = false;
        });
      }

    }
  }
})();
