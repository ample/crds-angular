(function() {
  'use strict';

  var constants = require('crds-constants');

  require('./dashboard.html');
  require('./group_detail.html');
  require('./templates/dashboard_header.html');
  require('./templates/group_resources.html');
  require('./templates/group_contact_modal.html');

  angular.module(constants.MODULES.GROUP_FINDER)
    .config(require('./dashboard.routes'))
    .controller('DashboardCtrl', require('./dashboard.controller.js'))
    .controller('GroupContactCtrl', require('./group_contact.controller.js'))
    .controller('GroupDetailCtrl', require('./group_detail.controller.js'))
    ;

})();
