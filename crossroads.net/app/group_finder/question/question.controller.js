(function(){
  'use strict';

  module.exports = QuestionCtrl;

  QuestionCtrl.$inject = ['$scope'];

  function QuestionCtrl($scope) {

    $scope.model = function() {
      return $scope.definition.model;
    };

    $scope.getTemplateUrl = function () {
      return 'question/input_'+ $scope.definition.input_type +'.html';
    };

  }

})();
