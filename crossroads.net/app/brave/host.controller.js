(function(){
  'use strict';

  module.exports = HostCtrl;

  HostCtrl.$inject = ['$scope', '$log', '$http', '$cookies', '$stateParams', 'questions', 'ResponseService', '$state', '$window'];

  function HostCtrl($scope, $log, $http, $cookies, $stateParams, questions, ResponseService, $state, $window) {

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

    vm.next_question = function(){
      if (vm.current_question().required == true && vm.responses[vm.current_index()] === undefined) {
        $window.alert('required');
      } else {
        $state.go('brave.host', {step: vm.currentQuestion+1});
      }
    };

    vm.current_index = function() {
      return Object.keys(vm.questions)[vm.currentQuestion - 1]
    }

    vm.current_question = function() {
      return vm.questions[vm.current_index()];
    }

    vm.start_over = function(){
      vm.currentQuestion = 1;
    };

  }

})();
