'use strict';

var angular = require('angular');
var stripe = require ('stripe');

require('./templates/nav.html');
require('./templates/nav-mobile.html');

require('../node_modules/angular-toggle-switch/angular-toggle-switch-bootstrap.css');
require('../node_modules/angular-toggle-switch/angular-toggle-switch.css');

require('../styles/main.scss');
require('./profile');
require('./filters');
require('./events');
require('./cms/services/cms_services_module');

require('angular-aside');
require('angular-match-media');

require('angular-stripe');

require('./third-party/angular/angular-aside.min.css');
require('./third-party/angular/angular-growl.css');
require('./give');


require('./app.core.module');
require('./mp_tools');

var _ = require('lodash');
"use strict";
(function () {

   angular.module("crossroads", [
     'crossroads.core',
     "crossroads.profile", 
     "crossroads.filters", 
     'crossroads.mptools',
     "crdsCMS.services",
     'ngAside', 
     'matchMedia',
     'crossroads.give'
     ])
    .constant("AUTH_EVENTS", {
            loginSuccess: "auth-login-success",
            loginFailed: "auth-login-failed",
            logoutSuccess: "auth-logout-success",
            sessionTimeout: "auth-session-timeout",
            notAuthenticated: "auth-not-authenticated",
            isAuthenticated: "auth-is-authenticated",
            notAuthorized: "auth-not-authorized"
    })
    //TODO Pull out to service and/or config file
    .constant("MESSAGES", {
        generalError: 1,
        emailInUse: 2,
        fieldCanNotBeBlank: 3,
        invalidEmail: 4,
        invalidPhone: 5,
        invalidData: 6,
        profileUpdated: 7,
        photoTooSmall: 8,
        credentialsBlank: 9,
        loginFailed: 10,
        invalidZip: 11,
        invalidPassword: 12,
        successfullRegistration: 13,
        succesfulResponse: 14,
        failedResponse: 15,
        successfullWaitlistSignup:17,
        noPeopleSelectedError:18,
        fullGroupError:19,
        invalidDonationAmount:22,
        invalidAccountNumber:23,
        invalidRoutingTransit:24,
        invalidCard:25,
        invalidCvv:26,
        donorEmailAlreadyRegistered:28,
        serveSignupSuccess:29,
        creditCardDiscouraged:36,
        selectSignUpAndFrequency: 31,
        selectFrequency: 32,
        invalidDateRange: 35,
        noMembers: 33,
        noServingOpportunities: 34
    }).config(function (growlProvider) {
        growlProvider.globalPosition("top-center");
        growlProvider.globalTimeToLive(6000);
        growlProvider.globalDisableIcons(true);
        growlProvider.globalDisableCountDown(true);
    })
    .filter('html', ['$sce', function ($sce) {
        return function (val) {
            return $sce.trustAsHtml(val);
        };
    }])
    .controller("appCtrl", ["$scope", "$rootScope", "MESSAGES", "$http", "Message", "growl", "$aside", "screenSize", "$payments", "$state",
        function ($scope, $rootScope, MESSAGES, $http, Message, growl, $aside, screenSize, $payments, $state) {

                console.log(__API_ENDPOINT__);
                 
                $scope.stateData = $state.current.data;

                $scope.prevent = function (evt) {
                    evt.stopPropagation();
                };

                $rootScope.mobile = screenSize.on('xs, sm', function(match){
                    $rootScope.mobile = match;
                })

                var messagesRequest = Message.get("", function () {
                    messagesRequest.messages.unshift(null); //Adding a null so the indexes match the DB
                    //TODO Refactor to not use rootScope, now using ngTemplate w/ ngMessages but also need to pull this out into a service
                    $rootScope.messages = messagesRequest.messages;
                });

                $rootScope.error_messages = '<div ng-message="required">This field is required</div><div ng-message="minlength">This field is too short</div>';

                $rootScope.$on("notify", function (event, id, refId, ttl) {
                    var parms = { };
                    if(refId !== undefined && refId !== null) {
                        parms.referenceId = refId;
                    }
                    if(ttl !== undefined && ttl !== null) {
                        parms.ttl = ttl;
                    }

                    growl[$rootScope.messages[id].type]($rootScope.messages[id].message, parms);
                });

                $rootScope.$on("context", function (event, id) {
                    var message = Message.get({
                        id: id
                    }, function () {
                        return message.message.message;
                    });
                });

                //Offcanvas menu
                $scope.asideState = {
                  open: false
                };

                $scope.openAside = function(position, backdrop) {
                  $scope.asideState = {
                    open: true,
                    position: position
                  };

                  function postClose() {
                    $scope.asideState.open = false;
                  }

                  $aside.open({
                    templateUrl: 'templates/nav-mobile.html',
                    placement: position,
                    size: 'sm',
                    controller: function($scope, $modalInstance) {
                      $scope.ok = function(e) {
                        $modalInstance.close();
                        e.stopPropagation();
                      };
                      $scope.cancel = function(e) {
                        $modalInstance.dismiss();
                        e.stopPropagation();
                      };
                    }
                  }).result.then(postClose, postClose);
                }
        }
    ])
    .directive("emptyToNull", require('./shared/emptyToNull.directive.js'))
    .directive("stopEvent", require('./shared/stopevent.directive.js'))
    .directive("svgIcon", require('./shared/svgIcon.directive.js'));

    require('./preloader'); 

    require('./apprun');
    require('./app.config');
    require('./routes');
    require('./register/register_directive');
    require('./login');
})()
