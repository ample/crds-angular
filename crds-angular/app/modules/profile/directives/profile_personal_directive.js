﻿(function () {
    angular.module('crdsProfile').directive('crdsProfilePersonal', ['$log', crdsProfile]);
    function crdsProfile($log) {
        return {
            restrict: 'EA',
            contoller: 'crdsProfilePersonalCtrl',
            templateUrl: 'app/modules/profile/templates/profile_personal.html',
            scope: true,
            link: (function (scope, el, attr) {

            })
        };
    }
})()