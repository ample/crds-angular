'use strict';
(function () {
    angular.module("crdsProfile").controller('crdsProfileCtrl', ['Profile', 'Lookup', ProfileController]);

    function ProfileController(Profile, Lookup) {
        this.genders = Lookup.Genders.query();
        this.profile = Profile.get();
        this.maritalStatuses = Lookup.MaritalStatus.query();
        this.serviceProviders = Lookup.ServiceProviders.query();
        this.states = Lookup.States.query();
        this.countries = Lookup.Countries.query();
        this.crossroadsLocations = Lookup.CrossroadsLocations.query();

        this.savePersonal = function (profile) {
            profile.$save(function () {
                //on success give message
            });
        }
    }

})()