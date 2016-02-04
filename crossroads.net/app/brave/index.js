(function(){
  'use strict';

  var MODULES = require('crds-constants').MODULES;

  require('./templates/layout.html');
  require('./templates/welcome.html');
  require('./templates/join.html');
  require('./templates/dashboard.html');

  angular.module('crossroads.brave', [MODULES.CORE, MODULES.COMMON])
    .config(require('./brave.routes'))
    .directive('question', require('./question.directive'))
    .factory('User', require('./services/user.service'))
    .factory('Group', require('./services/group.service'))
    .service('Responses', require('./services/response.service'))
    .controller('BraveCtrl', require('./brave.controller'))
    .controller('LoginCtrl', require('./login.controller'))
    .controller('JoinCtrl', require('./login.controller'))
    .controller('DashboardCtrl', require('./dashboard.controller'))
    ;

    require('./host');
    require('./login');
})();
