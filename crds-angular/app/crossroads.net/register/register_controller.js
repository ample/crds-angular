﻿'use strict';
(function () {
    angular.module('crossroads').controller('RegisterCtrl', ['$scope', '$rootScope', 'AUTH_EVENTS', 'MESSAGES', 'AuthService', '$cookieStore', '$state', '$log', "Session", RegisterController]);

    function RegisterController($scope, $rootScope, AUTH_EVENTS, MESSAGES, AuthService, $cookieStore, $state, $log, Session) {

        /*$scope.showLoginButton = $rootScope.username === null || $rootScope.username === undefined;
        $scope.loginShow = false;

        $scope.toggleDesktopLogin = function () {
            $scope.loginShow = !$scope.loginShow; */
        }

        //$scope.credentials = { username: '', password: '' };

       /* $scope.login = function () {
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
                    $scope.showLoginButton = false;
                    $state.go('profile.personal');
                }, function () {
                    $log.debug("Bad password");
                    $scope.pending = false;
                    $scope.processing = false;
                    $scope.loginFailed = true;
                });
            }
        };
    }*/
})()