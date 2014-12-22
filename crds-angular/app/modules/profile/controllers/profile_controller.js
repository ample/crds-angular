'use strict';
(function () {
    angular.module("crdsProfile").controller('crdsProfileCtrl', ['Profile', 'Lookup','$log',  ProfileController]);

    function ProfileController(Profile, Lookup, $log) {
        this.genders = Lookup.Genders.query();

       // this.profile = Profile.get();

        this.person = Profile.Personal.get();

        this.maritalStatuses = Lookup.MaritalStatus.query();
        this.serviceProviders = Lookup.ServiceProviders.query();
        this.states = Lookup.States.query();
        this.countries = Lookup.Countries.query();
        this.crossroadsLocations = Lookup.CrossroadsLocations.query();
        this.account = Profile.Account.get();

        this.savePersonal = function (profile) {

            //profile.$save(function () {
            $log.debug("profile controller");
            profile.person.$save(function () {

                //on success give message
            });
        }

        this.saveAccount = function () {
            $log.debug(this.account.NewPassword);
            $log.debug(this.account.emailPrefs);
            account.$save(function () {
                $log.debug("save successful");
            }, function () {
                $log.error("save unsuccessful");
            });
        }
    }

})()