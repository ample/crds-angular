(function() {
  'use strict';

  var constatns = require('crds-constants');

  require('./login.html');

  var app = angular.module('crossroads.brave')
    .controller('LoginCtrl', require('./login.controller.js'));
})();
