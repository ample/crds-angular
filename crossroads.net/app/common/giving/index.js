(function() {
  'use strict';

  var constants = require('../../constants');

  angular.module(constants.MODULES.COMMON)
    .directive('currencyMask', require('./currencyMask.directive'))
    .directive('initiativeRequired', require('./initiativeRequired.validation.directive'))
    .directive('invalidAccount', require('./invalidAccount.validation.directive'))
    .directive('invalidRouting', require('./invalidRouting.validation.directive'))
    .directive('invalidZip', require('./invalidZip.validation.directive'))
    .directive('naturalNumber', require('./naturalNumber.validation.directive'));

  require('./services');
  require('./bankCreditCardDetails');
  require('./donation_details');
  require('./bankInfo');
  require('./creditCardInfo');

})();