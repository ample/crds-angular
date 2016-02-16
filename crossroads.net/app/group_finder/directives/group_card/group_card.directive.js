(function() {
  'use strict';

  module.exports = GroupCardDirective;

  require('./group_card.html');

  GroupCardDirective.$inject = ['$log'];

  function GroupCardDirective($log) {
    return {
      restrict: 'AE',
      scope: {
        group: '=',
        host: '='
      },
      controller: require('./group_card.controller'),
      templateUrl: 'group_card/group_card.html'
    };
  }
})();
