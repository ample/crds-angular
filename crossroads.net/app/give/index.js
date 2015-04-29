require('./give.html');
require('./amount.html');
require('./login.html');
require('./account.html');
require('./thank_you.html');
require('./history.html');
require('./give.module.js');

var app = require('angular').module('crossroads.give');
app.factory("getPrograms", require('./services/getPrograms.service.js'));
app.directive('invalidRouting',[require('./directives/invalidRouting.validation.directive')]);
app.directive('invalidAccount',[require('./directives/invalidAccount.validation.directive')]);
app.directive('naturalNumber',[require('./directives/naturalNumber.validation.directive')]);
app.directive('invalidZip', [require('./directives/invalidZip.validation.directive')]);

/****** DEMO PAGES ******/
require('./demo/guest_giver/give.html');
require('./demo/guest_giver/give-login.html');
require('./demo/guest_giver/give-login-guest.html');
require('./demo/guest_giver/give-confirmation.html');
require('./demo/guest_giver/give-register.html');
require('./demo/guest_giver/give-logged-in-bank-info.html');
require('./demo/guest_giver/give-confirm-amount.html');
require('./demo/guest_giver/give-change-information.html');
require('./demo/guest_giver/give-logged-in.html');
require('./demo/guest_giver/give-change-information-logged-in.html');
require('./demo/guest_giver/give-logged-in-new-giver.html');
require('./demo/trip_giving/give.html');


app.controller("GiveCtrl",require("./give_controller"));
