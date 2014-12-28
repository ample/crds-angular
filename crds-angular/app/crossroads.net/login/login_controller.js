﻿'use strict';
(function () {
    angular.module('crossroads').controller('LoginCtrl', ['$scope', '$rootScope', 'AUTH_EVENTS', 'AuthService', '$cookieStore', '$state','$log', "Session", LoginController]);

    function LoginController($scope, $rootScope, AUTH_EVENTS, AuthService, $cookieStore, $state, $log, Session) {

        $scope.showLoginButton = $rootScope.username === null || $rootScope.username === undefined;
        $scope.loginShow = false;

        $scope.toggleDesktopLogin = function () {
            $scope.loginShow = !$scope.loginShow;
        }

        //$scope.credentials = { username: '', password: '' };

        $scope.logout = function () {
            AuthService.logout();
        }

        $scope.login = function () {            
            if ($scope.credentials.username === '' || $scope.credentials.password === '') {
                
            } else {
                $scope.processing = true;
                AuthService.login($scope.credentials).then(function (user) {
                    $log.debug("got a 200 from the server ");
                    $log.debug(user);
                    $scope.processing = false;
                    $scope.loginShow = false;
                    $scope.showLoginButton = false;
                    $state.go('profile.personal');
                }, function () {
                    $log.debug("Bad password");    
                    $scope.processing = false;
                });
            }
        };
    }
})()