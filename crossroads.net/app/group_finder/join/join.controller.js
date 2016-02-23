(function () {
  'use strict';

  module.exports = JoinCtrl;

  JoinCtrl.$inject = ['$scope', 'AuthenticatedPerson'];

  function JoinCtrl ($scope, AuthenticatedPerson) {
    $scope.currentStep = 1;
    $scope.person = AuthenticatedPerson;
  }

})();
