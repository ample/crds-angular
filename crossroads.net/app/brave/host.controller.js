(function(){
  'use strict';

  module.exports = HostCtrl;

  HostCtrl.$inject = ['$scope', '$log', '$http', '$cookies', '$stateParams', 'questions', 'ResponseService'];

  function HostCtrl($scope, $log, $http, $cookies, $stateParams, questions, ResponseService) {

    // Properties
    $scope.questions = questions;
    $scope.total_questions = _.size(questions);
    $scope.currentQuestion = parseInt($stateParams.step) || 1;
    $scope.responses = ResponseService.data;

    // Methods
    $scope.previous = function(){
      $scope.currentQuestion--;
    };

    $scope.next = function(){
      $scope.currentQuestion++;
    };

    $scope.start_over = function(){
      $scope.currentQuestion = 1;
    };

  }

})();
