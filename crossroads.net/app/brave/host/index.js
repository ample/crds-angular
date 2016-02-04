(function() {
  'use strict';

  var constatns = require('crds-constants');

  require('./host.html');

  var app = angular.module('crossroads.brave')
    .controller('HostCtrl', require('./host.controller.js'));
})();
