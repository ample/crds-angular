﻿'use strict';
(function () {
    angular.module("crdsProfile").controller("ProfilePersonalController", ['$rootScope', 'Profile', 'Lookup', '$log','MESSAGES', ProfilePersonalController]);

    function ProfilePersonalController($rootScope, Profile, Lookup, $log, MESSAGES) {
        var _this = this;

        _this.loading = true;

        _this.initProfile = function (form) {
            _this.form = form;
            _this.genders = Lookup.Genders.query();
            _this.maritalStatuses = Lookup.MaritalStatus.query();
            _this.serviceProviders = Lookup.ServiceProviders.query();
            _this.states = Lookup.States.query();
            _this.countries = Lookup.Countries.query();
            _this.crossroadsLocations = Lookup.CrossroadsLocations.query();
            _this.person = Profile.Personal.get(function () {
                _this.loading = false;
            });
            
        }

        _this.savePersonal = function () {
            $log.debug("profile controller");
            if (_this.form.personal.$invalid) {
                $log.debug("The form is invalid!");
                $rootScope.$emit('notify.error', MESSAGES.generalError);
                return 
            }
            _this.person.$save(function () {
                $log.debug("person save successful");
            }, function () {
                $log.debug("person save unsuccessful");
            });
        }

    }

})()