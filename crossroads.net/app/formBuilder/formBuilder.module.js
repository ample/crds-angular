(function(global) {
  'use strict';

  var MODULE = require('crds-constants').MODULES.FORM_BUILDER;

  angular.module(MODULE, ['crossroads.core', 'crossroads.common'])
    .config(require('./formBuilder.routes'))
    .directive('formBuilder', require('./formBuilder.directive'))
    .directive('formField', require('./formField.directive'))
    ;

  //Require Templates
  require('./templates/formBuilder.html');
  require('./templates/formField.html');
  require('./templates/textField.html');
})(this);
