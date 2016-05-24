(function() {
  'use strict';

  module.exports = FormBuilder;

  FormBuilder.$inject = ['$controller'];

  function FormBuilder($controller) {
    return {
      restrict: 'E',
      scope: {
        page: '=?',
      },
      templateUrl: 'templates/formBuilder.html',
      controller: getController,
      controllerAs: 'formBuilder',
      bindToController: true
    };

    function getController($scope, $attrs, $transclude) {
      var controllerName = getControllerName($scope);
      return $controller(controllerName, $scope, $attrs, $transclude);
    }

    function getControllerName($scope) {
      return 'UndividedFacilitatorCtrl';
    }
  }

})();
