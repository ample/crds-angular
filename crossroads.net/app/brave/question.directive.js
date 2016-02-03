(function(){
  'use strict';

  module.exports = QuestionDirective;

  QuestionDirective.$inject = ['$log'];

  function QuestionDirective($log) {
    return {
      restrict: 'EA',
      scope: {
        key: '@key',
        answers: '=',
        label: '=',
        responses: '=responses',
      },
      transclude: true,
      templateUrl: 'templates/question.html'
    };
  }

})();
