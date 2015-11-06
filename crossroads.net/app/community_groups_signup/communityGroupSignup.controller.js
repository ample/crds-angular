(function(){
  'use strict';

  module.exports = CommunityGroupsController;

  CommunityGroupsController.$inject = [
    '$scope',
    '$rootScope',
    'Profile',
    'Group',
    '$log',
    '$stateParams',
    'Page',
    '$modal',
    'ChildCare'
  ];

  function CommunityGroupsController(
    $scope,
    $rootScope,
    Profile,
    Group,
    $log,
    $stateParams,
    Page,
    $modal,
    ChildCare) {

    var vm = this;
    vm.allSignedUp = allSignedUp;
    vm.alreadySignedUp = false;
    vm.childCareAvailable = false;
    vm.childCareChange = childCareChange;
    vm.editProfile = editProfile;
    vm.formValid = true;
    vm.hasParticipantID = hasParticipantID;
    vm.modalInstance = {};
    vm.person = {};
    vm.response = {};
    vm.showContent = true;
    vm.showFull = false;
    vm.showSuccess = false;
    vm.showWaitList = false;
    vm.showWaitSuccess = false;
    vm.signup = signup;
    vm.signupPage = $rootScope.signupPage;
    vm.viewReady = false;
    vm.waitListCase = false;

    activate();

    ///////////////////////////

    function activate() {
      // Initialize Person data for logged-in user
      Profile.Personal.get(function(response) {
        vm.person = response;
      },

      function(err) {
        console.log('Can\'t get your profile! ' + err);
      });

      var pageRequest = Page.get({
        url: $stateParams.link
      }, function() {
        if (pageRequest.pages.length > 0) {
          vm.signupPage = pageRequest.pages[0];
          vm.groupId = vm.signupPage.group;
          vm.groupDetails = Group.Detail.get({
            groupId: vm.groupId
          }).$promise.then(function(response) {
            vm.response = response.SignUpFamilyMembers;
            vm.childCareAvailable = response.childCareInd;
            if (!response.waitListInd) {
              vm.viewReady = true;
            }

            if (allSignedUp(response)) {
              vm.alreadySignedUp = true;
            }

            if (response.groupFullInd && response.waitListInd) {
              vm.waitListCase = true;
              vm.showWaitList = true;
              vm.signupPage.title = vm.signupPage.title + ' - Waitlist';
              vm.groupId = response.waitListGroupId;

              // I now need to get the group-detail again for the wait list there are are two new possible cases
              // 1. the user is a already a member
              // 2. the user is not yet a member
              // 3. DE616 the user is already member of non-wait list group
              var originalMembers = vm.response;
              vm.groupDetails = Group.Detail.get({
                groupId: vm.groupId
              }).$promise.then(function(response) {
                var familyMembers = response.SignUpFamilyMembers;
                _.forEach(familyMembers, function(member) {
                  if (!member.userInGroup) {
                    var m = _.find(originalMembers, function(i) {
                      return i.participantId === member.participantId;
                    });

                    member.userInGroup = m.userInGroup;
                  }
                });

                vm.response = familyMembers;
                vm.childCareAvailable = response.childCareInd;
                if (allSignedUp(response)) {
                  vm.alreadySignedUp = true;
                }

                vm.viewReady = true;
              });

              //this is the case where the group is full and there is NO waitlist
            } else if (response.groupFullInd && !response.waitListInd) {
              vm.showFull = true;
              vm.waitListCase = false;
              vm.showContent = false;
              vm.showWaitList = false;
              vm.viewReady = true;

              //this is the case where the group is NOT full and there IS waitlist
            } else if (!response.groupFullInd && response.waitListInd) {
              vm.waitListCase = false;
              vm.showFull = false;
              vm.showContent = true;
              vm.showWaitList = false;
              vm.viewReady = true;
            }
          });
        } else {
          var notFoundRequest = Page.get({
            url: 'page-not-found'
          }, function() {
            if (notFoundRequest.pages.length > 0) {
              vm.content = notFoundRequest.pages[0].content;
            } else {
              vm.content = '404 Content not found';
            }
          });
        }
      });
    }

    function allSignedUp(array) {
      var result = false;
      if (array.SignUpFamilyMembers.length > 1) {
        for (var i = 0; i < array.SignUpFamilyMembers.length; i++) {
          if (array.SignUpFamilyMembers[i].userInGroup === false) {
            result = false;
            break;
          } else {
            result = true;
          }
        }
      } else {
        if (array.SignUpFamilyMembers[0].userInGroup === false) {
          result = false;
        } else {
          result = true;
        }
      }

      return result;
    }

    function childCareChange(changedRecord) {
      _.forEach(vm.response, function(found) {
        if (found.participantId === changedRecord.participantId) {
          found.childCareNeeded = changedRecord.value;
        }
      });
    }

    function editProfile() {
      vm.modalInstance = $modal.open({
        templateUrl: 'editProfile.html',
        backdrop: true,
        scope: $scope
      });
    }

    function hasParticipantID(array) {
      var result = {};
      result.partId = [];
      if (array.length > 0) {
        for (var i = 0; i < array.length; i++) {
          if (array[i].newAdd !== undefined && array[i].newAdd !== '') {
            result.partId[result.partId.length] = {
              participantId: array[i].newAdd,
              childCareNeeded: array[i].childCareNeeded
            };
          }
        }
      }

      return result;
    }

    function signup(form) {
      var participantArray = hasParticipantID(vm.response);
      var flag = false;
      for (var i = 0; i < vm.response.length; i++) {
        if (!vm.response[i].userInGroup &&
            vm.response[i].newAdd !== undefined &&
            vm.response[i].newAdd !== '') {
          flag = true;
          break;
        }
      }

      if (vm.response.length === 1) {
        flag = true;
      }

      vm.formValid = flag;
      if (!vm.formValid) {
        $rootScope.$emit('notify', $rootScope.MESSAGES.noPeopleSelectedError);
        return;
      }

      //Add Person to group
      Group.Participant.save({
        groupId: vm.groupId
      }, participantArray.partId).$promise.then(function(response) {
        if (vm.waitListCase) {
          $rootScope.$emit('notify', $rootScope.MESSAGES.successfullWaitlistSignup);
        } else {
          $rootScope.$emit('notify', $rootScope.MESSAGES.successfullRegistration);
        }

        vm.showContent = false;
        vm.showSuccess = true;
        vm.showWaitList = false;
        vm.showWaitSuccess = true;

      }, function(error) {
        // 422 indicates an HTTP "Unprocessable Entity", in this case meaning Group is Full
        // http://tools.ietf.org/html/rfc4918#section-11.2
        if (error.status === 422) {
          $rootScope.$emit('notify', $rootScope.MESSAGES.fullGroupError);
          vm.showFull = true;
          vm.showContent = false;
        } else {
          $rootScope.$emit('notify', $rootScope.MESSAGES.generalError);
          vm.showFull = false;
          vm.showContent = true;
        }

      });
    }

  }
})();
