(function(){
  'use strict';

  module.exports = QuestionsDirective;

  require('./questions.html');

  QuestionsDirective.$inject = [];

  function QuestionsDirective() {

    return {
      restrict: 'AE',
      scope: {
        mode: '@mode',
        step: '=',
        questions: '=definitions',
        responses: '='
      },
      templateUrl: 'questions/questions.html',
      controller: require('./questions.controller')
    };

  }

})();