'use strict';
(function () {
    angular.module("crdsProfile").controller('crdsProfileCtrl', ['Profile', 'Lookup','$log',  ProfileController]);

    function ProfileController(Profile, Lookup, $log) {
        var _this = this;

        
        _this.genders = Lookup.Genders.query();
        _this.person = Profile.Personal.get();
        _this.maritalStatuses = Lookup.MaritalStatus.query();
        _this.serviceProviders = Lookup.ServiceProviders.query();
        _this.states = Lookup.States.query();
        _this.countries = Lookup.Countries.query();
        _this.crossroadsLocations = Lookup.CrossroadsLocations.query();
        Profile.Account.get(function (acct) {
            _this.account = new Profile.Account();
            _this.account.EmailNotifications = acct.EmailNotifications.toString();
            _this.account.TextNotifications = acct.TextNotifications;
            _this.account.PaperlessStatements = acct.PaperlessStatements;
        });
        

        _this.password = new Profile.Password();

        _this.savePersonal = function (profile) {
            profile.person.$save(function () {
                //on success give message
            });
        }

        _this.saveAccount = function () {
            $log.debug(_this.password.password);
            $log.debug(_this.account.EmailNotifications);

            if (_this.password.password) {
                $log.debug("Save the password first!");
                _this.password.$save(function () {
                    $log.debug("password saved succesfully!");
                    _this.password.password = null;
                }, function () {
                    _this.password.password = null;
                    $log.error("did not save the password successfully. You need to have a Mobile Phone number set.");
                });
            }

            _this.account.$save(function () {
                $log.debug("save successful");
                
            }, function () {
                $log.error("save unsuccessful");
            });
        }
    }
})()