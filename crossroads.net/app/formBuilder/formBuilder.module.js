(function() {
  'use strict';

  var MODULE = require('crds-constants').MODULES.FORM_BUILDER;

  angular.module(MODULE, ['crossroads.core', 'crossroads.common'])
    .config(require('./formBuilder.routes'))
    .factory('FormBuilderService', require('./formBuilder.service'))
    .directive('formBuilder', require('./formBuilder.directive'))
    .directive('formField', require('./formField.directive'))
    .controller('FormBuilderDefaultCtrl', require('./formBuilderDefault.controller'))
    .controller('UndividedFacilitatorCtrl', require('./undividedFacilitator.controller'))
    ;

  //Require Templates
  require('./templates/formBuilder.html');
  require('./templates/default/defaultField.html');
  require('./templates/default/editableBooleanField.html');
  require('./templates/default/editableCheckbox.html');
  require('./templates/default/editableCheckboxGroupField.html');
  require('./templates/default/editableNumericField.html');
  require('./templates/default/editableRadioField.html');
  require('./templates/default/editableTextField.html');
  require('./templates/groupParticipant/childcare.html');
  require('./templates/groupParticipant/coFacilitator.html');
  require('./templates/groupParticipant/facilitatorTraining.html');  
  require('./templates/groupParticipant/kickOffEvent.html'); 
  require('./templates/groupParticipant/preferredSession.html'); 
  require('./templates/profile/email.html');
  require('./templates/profile/ethnicity.html');  
  require('./templates/profile/gender.html'); 
  require('./templates/profile/name.html');
  
})();
