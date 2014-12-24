'use strict';
(function () {
    angular.module("crdsProfile").controller('crdsProfileCtrl', ['Profile', 'Lookup','$log',  ProfileController]);

    function ProfileController(Profile, Lookup, $log) {
        
	var _this = this;
      
        _this.initProfile = function () {
            _this.genders = Lookup.Genders.query();
            _this.person = Profile.Personal.get();
            _this.maritalStatuses = Lookup.MaritalStatus.query();
            _this.serviceProviders = Lookup.ServiceProviders.query();
            _this.states = Lookup.States.query();
            _this.countries = Lookup.Countries.query();
            _this.crossroadsLocations = Lookup.CrossroadsLocations.query();
        }
        
        _this.initAccount = function () {
            _this.account = Profile.Account.get();
            _this.password = new Profile.Password();
        }

	_this.savePersonal = function () {

            $log.debug("profile controller");
            _this.person.$save(function () {
                $log.debug("person save successful");
            }, function () {
                $log.debug("person save unsuccessful");
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
