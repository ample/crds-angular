
require('./profile.module');
require('./profile.config');

require('./profile.html')
require('./personal/profile_personal.template.html');

var app = angular.module('crossroads.profile')

app.controller('crdsProfileCtrl', ['$rootScope','Profile', 'Lookup', '$q', '$log','$scope',  require("./profile_controller")]);

// Modal
require("./editProfile.html");
app.controller('ProfileModalController', require("./profileModalController"));

// Shared Services
app.factory('Lookup', ["$resource", "Session", require('./services/profile_lookup_service')]);
app.factory('Profile', ['$resource',require('./services/profile_service')]);
app.factory('ProfileReferenceData', ['Lookup', 'Profile', '$resolve', require('./services/profile_reference_data')]);

// Personal
require("./personal/profile_personal.html");
app.controller("ProfilePersonalController", ["$rootScope", "$log","$timeout","MESSAGES", "ProfileReferenceData", require('./personal/profile_personal_controller')]);
app.directive('uniqueEmail', ['$http', 'Session', 'User', require('./personal/profile_unique_email_directive') ]);
app.directive("validateDate", ["$log", require('./personal/profile_valid_date_directive')]);
app.directive('profilePersonal', ["$log", require("./personal/profile_personal.directive")]);

// Household
require('./household/profile_household.template.html');
app.controller('ProfileHouseholdController', ["$rootScope", "$log","$timeout","MESSAGES", "ProfileReferenceData", require('./household/profile_household.controller')]);
app.directive('profileHousehold', ["$log", require("./household/profile_household.directive")]);

// Skills
require('./skills/profile_skills.html');
app.controller("ProfileSkillsController", ['$rootScope', 'Skills', 'Session', '$log', require('./skills/profile_skills_controllers')]);
app.factory('Skills', ["$resource", require('./skills/profile_skills_service')]);

app.controller('crdsDatePickerCtrl', require('./profile_date_picker'));
require('./profile_account.html');
