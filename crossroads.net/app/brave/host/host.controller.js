(function(){
  'use strict';

  module.exports = HostCtrl;

  HostCtrl.$inject = [
                      '$scope',
                      '$log',
                      '$http',
                      '$cookies',
                      '$stateParams',
                      'questions',
                      '$state',
                      '$window',
                      'Responses'
                     ];

  function HostCtrl($scope, $log, $http, $cookies, $stateParams, questions, $state, $window, Responses) {

    var vm = this;

    // Properties
    vm.questions = questions;
    vm.total_questions = _.size(questions);
    vm.currentQuestion = parseInt($stateParams.step) || 1;
    vm.responses = Responses.data;

    // Methods
    vm.previous = function(){
      vm.currentQuestion--;
      $state.go('brave.host', { step: vm.currentQuestion });
    };

    vm.next_question = function(){
      var req = vm.current_question().required === true;
      if (req && vm.responses[vm.current_question().model][vm.current_index()] === undefined) {
        $window.alert('required');
      } else {
        vm.currentQuestion++;
        $state.go('brave.host', { step: vm.currentQuestion });
      }
    };

    vm.current_index = function() {
      return Object.keys(vm.questions)[vm.currentQuestion - 1];
    };

    vm.current_question = function() {
      return vm.questions[vm.current_index()];
    };

    vm.start_over = function(){
      vm.currentQuestion = 1;
    };

  }

})();
