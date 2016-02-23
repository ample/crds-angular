(function() {
  'use strict';

  module.exports = GroupCardDirective;

  require('./group_card.html');

  GroupCardDirective.$inject = [];

  function GroupCardDirective() {
    return {
      restrict: 'AE',
      scope: {
        display: '@',
        group: '=',
        host: '='
      },
      controller: require('./group_card.controller'),
      templateUrl: 'group_card/group_card.html'
    };
  }
})();
