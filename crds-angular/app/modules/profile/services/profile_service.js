﻿(function(){
    angular.module('crdsProfile').factory('Profile', ['$resource', ProfileService]);

    function ProfileService($resource) {
        return {
            Personal: $resource('api/profile'),
            Account: $resource('api/account')
        }
    }
})()
