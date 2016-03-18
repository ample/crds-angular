(function() {
  'use strict';

  module.exports = GoVolunteerGroupFindConnector;

  GoVolunteerGroupFindConnector.$inject = ['GoVolunteerService', 'GroupConnectors'];

  function GoVolunteerGroupFindConnector(GoVolunteerService, GroupConnectors) {
    return {
      restrict: 'E',
      scope: {
        onSubmit: '&' 
      },
      bindToController: true,
      controller: GoVolunteerGroupFindConnectorController,
      controllerAs: 'goGroupFindConnector',
      templateUrl: 'groupFindConnector/goVolunteerGroupFindConnector.template.html'
    };

    function GoVolunteerGroupFindConnectorController() {
      var vm = this;
      vm.activate = activate;
      vm.createGroup = createGroup;
      vm.disableCard = disableCard;
      vm.groupConnectors = [];
      vm.loaded = loaded;
      vm.organization = GoVolunteerService.organization;
 
      vm.submit = submit;
      vm.youngestInRegistration = youngestInRegistration();

      vm.activate();

      /////////////////////////

      function activate() {
        if (vm.organization.openSignup) {
          GroupConnectors.OpenOrgs.query({initiativeId: 1}, function(data) {
            vm.groupConnectors = data;
          }, handleError);
        } else {
          GroupConnectors.ByOrgId.query({orgId: vm.organization.organizationId, initiativeId: 1}, function(data) {
            vm.groupConnectors = data;
          }, handleError);
        }
      }

      function createGroup() {
        vm.onSubmit({nextState: 'unique-skills'});
      }

      function handleError(err) {
        // show error page? 
        console.log(err);
      }

      function loaded() {
        return (vm.groupConnectors !== null && vm.groupConnectors.$resolved);
      }

      function disableCard(projectMinAge) {
        if (projectMinAge === 0) {
          return false;
        }

        if (projectMinAge > vm.youngestInRegistration) {
          return true;
        }

        return false;
      }

      function submit(groupConnectorId) {
        console.log('click: ' + groupConnectorId);
      }

      function youngestInRegistration() {
        if (GoVolunteerService.childrenAttending.childTwoSeven !== 0) {
          return 2;
        }

        if (GoVolunteerService.childrenAttending.childEightTwelve !== 0) {
          return 8;
        }

        if (GoVolunteerService.childrenAttending.childThirteenEighteen !== 0) {
          return 13;
        }

        // should this really be registrant or spouse age?
        return 18;
      }
    }
  }

})();
