(function(){
  'use strict';

  module.exports = HostCtrl;

  HostCtrl.$inject = ['$scope', '$log', '$http', '$cookies', '$stateParams', 'questions', 'ResponseService'];

  function HostCtrl($scope, $log, $http, $cookies, $stateParams, questions, ResponseService) {

    var vm = this;

    // Properties
    vm.questions = questions;
    vm.total_questions = _.size(questions);
    vm.currentQuestion = parseInt($stateParams.step) || 1;
    vm.responses = ResponseService.data;

    // Methods
    vm.previous = function(){
      vm.currentQuestion--;
    };

    vm.next = function(){
      vm.currentQuestion++;
    };

    vm.start_over = function(){
      vm.currentQuestion = 1;
    };

  }

})();
