﻿'use strict';
(function () {
    angular.module('crossroads').controller('LoginCtrl', ['$scope', '$rootScope', 'AUTH_EVENTS', 'MESSAGES', 'AuthService', '$cookieStore', '$state','$log', "Session", LoginController]);

    function LoginController($scope, $rootScope, AUTH_EVENTS, MESSAGES, AuthService, $cookieStore, $state, $log, Session) {


        $rootScope.showLoginButton = $rootScope.username === null || $rootScope.username === undefined;
  
        $scope.loginShow = false;

        $scope.toggleDesktopLogin = function () {
            $scope.loginShow = !$scope.loginShow;
            if ($scope.registerShow)
                $scope.registerShow = !$scope.registerShow;
        }
          
        $scope.logout = function () {
            AuthService.logout();
            if ($scope.credentials !== undefined) {
                $scope.credentials.username = undefined;
                $scope.credentials.password = undefined;
            }
            $rootScope.username = null;
            $rootScope.showLoginButton = true;
        }

        $scope.login = function () {           
            if (($scope.credentials === undefined) || ($scope.credentials.username === undefined || $scope.credentials.password === undefined)) {
                $scope.pending = true;
                $scope.loginFailed = false;
            } else {
                $scope.processing = true;
                AuthService.login($scope.credentials).then(function (user) {
                    $log.debug("got a 200 from the server ");
                    $log.debug(user);
                    $scope.processing = false;
                    $scope.loginShow = false;
                    $scope.loginFailed = false;
                    $rootScope.showLoginButton = false;
                    $scope.navlogin.$setPristine();
                }, function () {
                    $log.debug("Bad password");
                    $scope.pending = false;
                    $scope.processing = false;
                    $scope.loginFailed = true;
                });
            }
        };
    }
})()