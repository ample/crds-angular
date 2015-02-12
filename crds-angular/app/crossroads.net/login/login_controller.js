﻿'use strict';
(function () {
  

    function LoginController($scope, $rootScope, AUTH_EVENTS, MESSAGES, AuthService, $cookieStore, $state, $log, Session, $timeout, User) {
  
        $scope.loginShow = false;
        $scope.credentials = {};
        $scope.credentials.username = User.getEmail();

        $scope.passwordPrefix = "login-page";

        $scope.checkIfUsernameValid = function() {return (
             $scope.navlogin.username.$error.required && $scope.navlogin.$submitted &&  $scope.navlogin.username.$dirty ||
          $scope.navlogin.username.$error.required && $scope.navlogin.$submitted && !$scope.navlogin.username.$touched ||
          $scope.navlogin.username.$error.required && $scope.navlogin.$submitted && $scope.navlogin.username.$touched ||
          $scope.navlogin.username.$error.unique &&  $scope.navlogin.username.$dirty ||
         ! $scope.navlogin.username.$error.required &&  $scope.navlogin.username.$dirty && ! $scope.navlogin.username.$valid)
        };

        $scope.toggleDesktopLogin = function () {
            $scope.loginShow = !$scope.loginShow;
            if ($scope.registerShow) {
                $scope.registerShow = !$scope.registerShow;
                $scope.credentials.username = User.getEmail();
                $scope.credentials.password = User.getPassword();
            }
        }
          
        $scope.logout = function () {
            AuthService.logout();
            if ($scope.credentials !== undefined) {
                $scope.credentials.username = undefined;
                $scope.credentials.password = undefined;
            }
            $rootScope.username = null;
        }

        $scope.login = function () {           
            if (($scope.credentials === undefined) || ($scope.credentials.username === undefined || $scope.credentials.password === undefined)) {
                $scope.pending = true;
                $scope.loginFailed = false;
            } else {
                $scope.processing = true;
                AuthService.login($scope.credentials).then(function (user) {             
                    $scope.processing = false;
                    $scope.loginShow = false;
                    $timeout(function() {
                        if (Session.hasRedirectionInfo()) {
                            var url = Session.exists("redirectUrl");
                            var params = Session.exists("redirectParams");
                            Session.removeRedirectRoute();
                            $state.go(url);
                        }
                    }, 500);
                    $scope.loginFailed = false;
                    $rootScope.showLoginButton = false;
                    $scope.navlogin.$setPristine();
                }, function () {
                    $scope.pending = false;
                    $scope.processing = false;
                    $scope.loginFailed = true;
                    $rootScope.$emit('notify', $rootScope.MESSAGES.generalError);
                });
            }
        };
    }

    angular.module('crossroads').controller('LoginCtrl', ['$scope', '$rootScope', 'AUTH_EVENTS', 'MESSAGES', 'AuthService', '$cookieStore', '$state', '$log', "Session", "$timeout", "User", LoginController]);
})()