(function() {
  'use strict';

  module.exports = GoVolunteerProjectPrefOne;

  GoVolunteerProjectPrefOne.$inject = ['$sce'];

  function GoVolunteerProjectPrefOne() {
    return {
      restrict: 'E',
      scope: {},
      bindToController: true,
      controller: GoVolunteerProjectPrefOneController,
      controllerAs: 'goProjectPrefOne',
      templateUrl: 'projectPrefOne/goVolunteerProjectPrefOne.template.html'
    };

    function GoVolunteerProjectPrefOneController($sce) {
      var vm = this;

      vm.list = [
        { title: 'Artistic Painting', state: '', age: '13' },
        { title: 'Construction', state: '', age: '13' },
        { title: 'Gardening', state: '', age: '2' },
        { title: 'Landscaping', state: '', age: '8' },
        { title: 'Organizing and Cleaning', state: '', age: '2' },
        { title: 'Painting', state: '', age: '13' },
        { title: 'Prayer', state: '', age: '2' },
        { title: 'Serving Meals or Throw A Party', state: '', age: '8' },
        { title: 'Working with Children', state: '', age: '2' },
        { title: 'Working with the Elderly', state: '', age: '2' }
      ];

    }
  }

})();
