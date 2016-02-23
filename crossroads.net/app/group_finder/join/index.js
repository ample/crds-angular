(function() {
  'use strict';

  var constants = require('crds-constants');

  require('./join.html');
  require('./join_review.html');
  require('./join_results.html');
  require('./join_questions.html');
  require('./templates/upsell.html');
  require('./templates/pager.html');

  angular.module(constants.MODULES.GROUP_FINDER)
    .config(require('./join.routes'))
    .controller('JoinCtrl', require('./join.controller'))
    .controller('JoinQuestionsCtrl', require('./join_questions.controller'))
    .controller('JoinModalCtrl',    require('./join_modal.controller'))
    .controller('JoinReviewCtrl', require('./join_review.controller'))
    .controller('JoinResultsCtrl', require('./join_results.controller'))
    ;

})();
