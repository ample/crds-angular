(function(){
    angular.module('crdsProfile').directive('crdsProfile', ['$log', crdsProfile]);
    function crdsProfile($log) {
        return {
            restrict: 'EA',
            contoller: 'crdsProfileCtrl as profile',
            templateUrl: 'app/modules/profile/templates/profile.html',
            scope: true,
            link: (function (scope, el, attr) {
        
            })
        };
    }
})()