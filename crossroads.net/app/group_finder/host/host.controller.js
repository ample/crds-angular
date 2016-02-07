(function(){
  'use strict';

  module.exports = HostCtrl;

  HostCtrl.$inject = [
                      '$scope',
                      '$log',
                      '$http',
                      '$cookies',
                      '$stateParams',
                      '$state',
                      '$window',
                      'QuestionDefinitions',
                      'Responses'
                     ];

  function HostCtrl($scope, $log, $http, $cookies, $stateParams, $state, $window, QuestionDefinitions, Responses) {

    var vm = this;

    // Properties
    vm.questions = QuestionDefinitions.questions;
    vm.total_questions = _.size(vm.questions);
    vm.currentIteration = parseInt($stateParams.step) || 1;
    vm.responses = Responses.data;

    // Methods
    vm.previous = function(){
      vm.currentIteration--;
      $state.go('brave.host', { step: vm.currentIteration });
    };

    vm.next_question = function(){
      var req = vm.currentQuestion().required === true;
      if (req && vm.responses[vm.currentQuestion().model][vm.currentKey()] === undefined) {
        $window.alert('required');
      } else {
        vm.currentIteration++;
        $state.go('brave.host', { step: vm.currentIteration });
      }
    };

    vm.currentKey = function() {
      return _.pluck(vm.questions, 'key')[vm.currentIteration - 1];
    };

    vm.currentQuestion = function() {
      return vm.questions[vm.currentIteration - 1];
    };

    vm.startOver = function(){
      vm.currentIteration = 1;
    };

  }

})();
