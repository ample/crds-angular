(function(){
  'use strict';

  var MODULES = require('crds-constants').MODULES;

  require('./common/layout.html');
  require('./common/welcome.html');
  require('./host/host.html');
  require('./dashboard/dashboard.html');

  angular.module('crossroads.group_finder', [MODULES.CORE, MODULES.COMMON])
    .config(require('./group_finder.routes'))
    .directive('question',          require('./question/question.directive'))
    .factory('Group',               require('./services/group.service'))
    .factory('GroupMember',         require('./services/group_member.service'))
    .service('Responses',           require('./services/response.service'))
    .controller('GroupFinderCtrl',  require('./group_finder.controller'))
    .controller('DashboardCtrl',    require('./dashboard/dashboard.controller'))
    .controller('HostCtrl',         require('./host/host.controller'))
    ;

})();
