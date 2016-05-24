(function() {
  'use strict';

  module.exports = UndividedFacilitatorCtrl;

  UndividedFacilitatorCtrl.$inject = ['$rootScope', 'Group', 'Session', 'ProfileReferenceData', 'Profile', 'FormBuilderService'];

  function UndividedFacilitatorCtrl($rootScope, Group, Session, ProfileReferenceData, Profile, FormBuilderService) {
    var vm = this;
    var constants = require('crds-constants');
    var attributeTypeIds = require('crds-constants').ATTRIBUTE_TYPE_IDS;

    //TODO Decide if you member or leader - now always leader
    var participant = {
      capacity: 1,
      contactId: parseInt(Session.exists('userId')),
      groupRoleId: constants.GROUP.ROLES.LEADER,
      childCareNeeded: false,
      sendConfirmationEmail: false,
      singleAttributes: {},
      attributeTypes: {}
    };

    vm.data = {};
    vm.saving = false;
    vm.save = save;
    vm.viewReady = false;

    activate();

    function activate() {
      //TODO only load profile data if profile field in CMS form builder
      ProfileReferenceData.getInstance().then(function(response) {

        vm.data.genders = response.genders;
        vm.data.maritalStatuses = response.maritalStatuses;
        vm.data.serviceProviders = response.serviceProviders;
        vm.data.groupParticipant = participant;
        //vm.crossroadsLocations = response.crossroadsLocations;
        var contactId = Session.exists('userId');

        Profile.Person.get({contactId: contactId},function(data) {
          vm.data.profileData = { person: data };
          vm.data.ethnicities = vm.data.profileData.person.attributeTypes[attributeTypeIds.ETHNICITY].attributes;

          vm.viewReady = true;
        });

      });
      //TODO GroupsUndivided from   vm.field.mpField  -or- formField.field.mpField
      FormBuilderService.Groups.query({templateType: 'GroupsUndivided'})
        .$promise.then(function(data){
          vm.data.availableGroups = data;
        }
      );

      FormBuilderService.Attribute.get({attributeTypeId: attributeTypeIds.UNDIVIDED_FACILITATOR_TRAINING})
        .$promise.then(function(data){
          vm.data.availableFacilitatorTraining = data;
        }
      );

      FormBuilderService.Attribute.get({attributeTypeId: attributeTypeIds.UNDIVIDED_RSVP_KICKOFF})
        .$promise.then(function(data){
          vm.data.availableRsvpKickoff = data;
        }
      );
    }

    function save(){
      vm.saving = true;
      try {
          // TODO: Need to return promises from save methods and then wait on all to turn of vm.saving
          savePersonal();
          saveGroup();
      }
      catch (error) {
        vm.saving = false;
        throw (error);
      }
    }

    function savePersonal() {
        vm.data.profileData.person.$save(function() {
           $rootScope.$emit('notify', $rootScope.MESSAGES.successfullRegistration);
           vm.saving = false;
         },
         function() {
           $rootScope.$emit('notify', $rootScope.MESSAGES.generalError);
           $log.debug('person save unsuccessful');
           vm.saving = false;
         });
    }

    function saveGroup() {
        //var singleAttributes = _.cloneDeep(vm.responses.singleAttributes);
        var coFacilitator = vm.data[constants.CMS.FORM_BUILDER.FIELD_NAME.COFACILITATOR];

        if (coFacilitator && coFacilitator !== '') {

          var item = {
            attribute: {
              attributeId: constants.ATTRIBUTE_IDS.COFACILITATOR
            },
            notes: coFacilitator,
          };
          vm.data.groupParticipant.singleAttributes[constants.ATTRIBUTE_TYPE_IDS.COFACILITATOR] = item;
        }

        var participants = [vm.data.groupParticipant];
        //TODO groupId will change with new groups
        Group.Participant.save({
          groupId: formField.data.groupId,
        }, participants).$promise.then(function(response) {
          $rootScope.$emit('notify', $rootScope.MESSAGES.successfullRegistration);
          vm.saving = false;
        }, function(error) {
          $rootScope.$emit('notify', $rootScope.MESSAGES.generalError);
          vm.saving = false;
        });
    }
  }

})();
