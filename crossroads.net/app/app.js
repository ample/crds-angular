//require('./group_finder');

require('./childcare');
require('./mp_tools');
require('./group_tool');
require('ui-select/dist/select.css');

(function() {
  'use strict()';

  var MODULE = 'crossroads';
  var constants = require('./constants');

  angular.module(constants.MODULES.CROSSROADS, [
      constants.MODULES.CHILDCARE,
      constants.MODULES.CORE,
      constants.MODULES.COMMON,
      constants.MODULES.FORM_BUILDER,
      constants.MODULES.GIVE,
      constants.MODULES.GO_VOLUNTEER,
      //constants.MODULES.GROUP_FINDER,
      constants.MODULES.MEDIA,
      constants.MODULES.MPTOOLS,
      constants.MODULES.GROUP_TOOL,
      constants.MODULES.PROFILE,
      constants.MODULES.SEARCH,
      constants.MODULES.SIGNUP,
      constants.MODULES.TRIPS,
   ]);

  angular.module(constants.MODULES.CROSSROADS)
    .config(require('./routes'))
    .config(require('./routes.content'));

  require('./events');
  require('./signup');
  require('./styleguide');
  require('./superbowl');
  require('./thedaily');
  require('./explore');
  require('./gotrips');
  require('./my_serve');
  require('./brave_at_home');
  require('./volunteer_signup');
  require('./volunteer_application');
  require('./giving_history');
  require('./boot');

})();
