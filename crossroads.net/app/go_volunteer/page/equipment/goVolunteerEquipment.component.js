(function() {
  'use strict';

  module.exports = GoVolunteerEquipment;

  GoVolunteerEquipment.$inject = ['GoVolunteerService'];

  function GoVolunteerEquipment(GoVolunteerService) {
    return {
      restrict: 'E',
      scope: {
        onSubmit: '&'
      },
      bindToController: true,
      controller: GoVolunteerEquipmentController,
      controllerAs: 'goEquipment',
      templateUrl: 'equipment/goVolunteerEquipment.template.html'
    };

    function GoVolunteerEquipmentController() {
      var vm = this;
      vm.addEquipment = addEquipment;
      vm.equipment = GoVolunteerService.availableEquipment;
      vm.otherField = false;
      vm.otherEquipment = [];
      vm.showOtherEquipment = showOtherEquipment;
      vm.submit = submit;

      function addEquipment() {
        vm.otherEquipment.push({equipment: {name: null}});
      }

      function showOtherEquipment() {
        vm.otherField = !vm.otherField;
        if (vm.otherField && vm.otherEquipment.length === 0) {
          vm.addEquipment();
        }
      }

      function submit() {
        GoVolunteerService.equipment = _.where(vm.equipment, {checked: true});
        GoVolunteerService.otherEquipment = vm.otherEquipment;

        vm.onSubmit({nextState: 'additional-info'});
      }
    }
  }

})();
