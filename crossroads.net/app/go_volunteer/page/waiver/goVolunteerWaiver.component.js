(function() {
  'use strict';

  module.exports = GoVolunteerWaiver;

  GoVolunteerWaiver.$inject = ['$rootScope', 'GoVolunteerService'];

  function GoVolunteerWaiver($rootScope, GoVolunteerService) {
    return {
      restrict: 'E',
      scope: {},
      bindToController: true,
      controller: GoVolunteerWaiverController,
      controllerAs: 'goWaiver',
      templateUrl: 'waiver/goVolunteerWaiver.template.html'
    };

    function GoVolunteerWaiverController() {
      var vm = this;
      vm.waiver = null;

      vm.temp = GoVolunteerService;

      activate();

      function activate() {
        var dto = GoVolunteerService.getRegistrationDto(vm.temp);
        vm.dto = dto;
        if (GoVolunteerService.cmsInfo && GoVolunteerService.cmsInfo.pages.length > 0) {
          vm.waiver = GoVolunteerService.cmsInfo.pages[0].content;
        } else {
          vm.waiver = $rootScope.MESSAGES.goVolunteerWaiverTerms.content;
        }
      }
    }
  }

})();
