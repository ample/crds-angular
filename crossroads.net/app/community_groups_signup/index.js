'use strict';

var app = require("angular").module('crossroads');

    app.controller('GroupSignupController', ['$scope',
     '$rootScope',
     'Profile',
     'Group',
     '$log',
     '$stateParams', require('./group_signup_controller')]);
;