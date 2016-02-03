(function(){
  'use strict';

  module.exports = QuestionDirective;

  require('./templates/input_radio.html');
  require('./templates/input_text.html');
  require('./templates/input_number.html');
  require('./templates/input_checkbox.html');
  require('./templates/input_select.html');

  QuestionDirective.$inject = ['$log'];

  function QuestionDirective($log) {
    return {
      restrict: 'AE',
      scope: {
        key: '@key',
        definition: '=',
        responses: '=responses',
      },
      transclude: true,
      template: '<ng-include src="getTemplateUrl()" />',
      controller: ['$scope', function($scope){
        $scope.getTemplateUrl = function () {
          return 'templates/input_'+ $scope.definition.input_type +'.html';
        };
      }]
    };
  }

})();
