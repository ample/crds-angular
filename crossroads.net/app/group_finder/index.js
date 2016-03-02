(function(){
  'use strict';

  var MODULES = require('crds-constants').MODULES;

  require('./common/layout.html');
  require('./common/header.html');
  require('./summary/summary.html');
  require('./invitation/invitation.html');

  angular.module(MODULES.GROUP_FINDER, [MODULES.CORE, MODULES.COMMON, 'rzModule'])
    .config(require('./group_finder.routes'))
    .constant('DAYS',                 require('./group_finder.constants').DAYS)
    .constant('SERIES',               require('./group_finder.constants').SERIES)
    .constant('GROUP_TYPES',          require('./group_finder.constants').GROUP_TYPES)
    .constant('GROUP_API_CONSTANTS',  require('./group_finder.constants').GROUP_API_CONSTANTS)
    .constant('GOOGLE_API_KEY',       require('./group_finder.constants').GOOGLE_API_KEY)
    .constant('GROUP_ROLE_ID_PARTICIPANT',       require('./group_finder.constants').GROUP_ROLE_ID_PARTICIPANT)
    .constant('GROUP_ROLE_ID_HOST',       require('./group_finder.constants').GROUP_ROLE_ID_HOST)
    .constant('EMAIL_TEMPLATES',      require('./group_finder.constants').EMAIL_TEMPLATES)
    .filter('humanize',               require('./filters/humanize.filter.js'))
    .controller('GroupFinderCtrl',    require('./group_finder.controller'))
    .controller('SummaryCtrl',        require('./summary/summary.controller'))
    .controller('GroupInvitationCtrl',      require('./invitation/invitation.controller'))
    ;

  require('./services');
  require('./dashboard');
  require('./host');
  require('./join');
  require('./directives');

})();
