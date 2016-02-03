(function(){
  'use strict';

  var MODULES = require('crds-constants').MODULES;

  require('./templates/layout.html');
  require('./templates/welcome.html');
  require('./templates/login.html');
  require('./templates/join.html');
  require('./templates/question.html');
  require('./templates/host.html');
  require('./templates/dashboard.html');

  angular.module('crossroads.brave', [MODULES.CORE, MODULES.COMMON])
    .config(require('./brave.routes'))
    .directive('question', require('./question.directive'))
    .service('ResponseService', require('./response.service'))
    .controller('BraveCtrl', require('./brave.controller'))
    .controller('LoginCtrl', require('./login.controller'))
    .controller('JoinCtrl', require('./login.controller'))
    .controller('DashboardCtrl', require('./dashboard.controller'))
    .controller('HostCtrl', require('./host.controller'))
    ;

})();
