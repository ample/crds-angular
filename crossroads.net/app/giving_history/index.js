(function() {
  'use strict';

  require('./history.html');
  var app = angular.module('crossroads');

  app.factory('GivingHistoryService', require('./giving_history_service'));
  app.controller('GivingHistoryController', require('./giving_history_controller'));
})();
