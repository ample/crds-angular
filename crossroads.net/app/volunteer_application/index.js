'use strict()';

var app = require("angular").module("crossroads");
require("./volunteer_application_form.html");
require('../profile/personal/profile_personal.html');
require('../profile');
// require('./volunteer_application.config.js');

app.controller("VolunteerApplicationController", require("./volunteer.application.controller"));
app.factory("Opportunity", ["$resource", "Session", require('../services/opportunity_service')]);
// app.factory("Opportunity", ["$resource", "Session", require('./opportunity_service')]);
// app.factory("ServeOpportunities", require('../services/serveOpportunities.service'));
// app.factory('Profile', ['$resource',require('./services/profile_service')]);
