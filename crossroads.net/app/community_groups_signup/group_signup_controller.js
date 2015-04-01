'use strict';
require('../services/group_service');
(function () {
    module.exports = function GroupSignupController($rootScope, $scope, Profile, Group, $log, $stateParams, Page, $modal) {
        $log.debug("Inside GroupSignupController");

        var vm = this;

        vm.editProfile = editProfile;

        //flags to control show/hide logic
        vm.showContent = true;
        vm.showSuccess = false;
        vm.showFull = false;
        vm.showWaitList = false;
        vm.showWaitSuccess = false;
        vm.waitListCase = false;
        vm.alreadySignedUp = false;
        vm.viewReady = false;
        vm.modalInstance = {};
        vm.response = {};


        // vm.testResponse = {
        //   "groupID": "1",
        //   "groupFullInd": "True",
        //   "waitListInd": "True",
        //   "waitListGroupId": "1",

        //   SignUpFamilyMembers:
        //   [
        //     { "nickname": "Shankar",
        //       "emailAddress": "shankx@test.com",
        //       "userInGroup": true,
        //       "Participant_ID":"1111"
        //     },
        //     { "nickname": "Luisa",
        //       "emailAddress": "Luisa@test.com",
        //       "userInGroup": false,
        //       "Participant_ID":"2222"
        //     },
        //     { "nickname": "John",
        //       "emailAddress": "john@test.com",
        //       "userInGroup": true,
        //       "Participant_ID":"3333"
        //     },
        //     { "nickname": "Bob",
        //       "emailAddress": "bob@test.com",
        //       "userInGroup": false,
        //       "Participant_ID":"4444"
        //     }
        //   ]
        // };

        vm.testSubmit = function(){
            //console.log(vm.testResponse.relationships);
            var test = hasParticipantID(vm.response);
            console.log(test);
        };

        vm.signupPage = $rootScope.signupPage;

       // Initialize Person data for logged-in user
        Profile.Personal.get(function (response) {
            vm.person = response;
            $log.debug("Person: " + JSON.stringify(vm.person));
        });

        var pageRequest = Page.get({
            url: $stateParams.link
        }, function () {
            if (pageRequest.pages.length > 0) {
                vm.signupPage = pageRequest.pages[0];
                // retrieve group id from the CMS page
                vm.groupId = vm.signupPage.group;
                $log.debug("Group ID: " + vm.groupId);
                // Get group details
                vm.groupDetails = Group.Detail.get({groupId : vm.groupId}).$promise
                .then(function(response){
                    console.log("Call for parent group");
                    console.log(response.SignUpFamilyMembers);
                    vm.response = response.SignUpFamilyMembers;
                    if(response.waitListInd === "False" || response.waitListInd === false)
                        vm.viewReady = true;
                    //if(response.SignUpFamilyMembers[0].userInGroup === true){

                    if(response.userInGroup === true){
                        vm.alreadySignedUp = true;
                    }
                    // This is the case where the group is full and there is a waitlist
                    if((response.groupFullInd === "True" && response.waitListInd === "True")  || (response.groupFullInd === true && response.waitListInd === true)){
                        vm.waitListCase = true;
                        vm.showWaitList = true;
                        //append "- WaitList" to title
                        vm.signupPage.title = vm.signupPage.title + " - Waitlist";
                        //update groupID to waitList ID
                        vm.groupId = response.waitListGroupId;
                        // I now need to get the group-detail again for the wait list, because there are are two new possible cases
                        // 1. the user is a already a member
                        // 2. the user is not yet a member
                        vm.groupDetails = Group.Detail.get({groupId : vm.groupId}).$promise
                        .then(function(response){
                            console.log("Call for waitlist group");
                            console.log(response);
                            if(response.userInGroup === true){
                                vm.alreadySignedUp = true;
                            }
                            vm.viewReady = true;
                        });

                        //this is the case where the group is full and there is NO waitlist
                    }else if((response.groupFullInd === "True" && response.waitListInd === "False") || (response.groupFullInd === true && response.waitListInd === false)){
                        vm.showFull = true;
                        vm.waitListCase = false;
                        vm.showContent = false;
                        vm.viewReady = true;
                        vm.showWaitList = false;
                        //this is the case where the group is NOT full and there IS waitlist
                    }else if((response.groupFullInd === "False" && response.waitListInd === "True") || (response.groupFullInd === false && response.waitListInd === true)){
                        vm.waitListCase = false;
                        vm.showFull = false;
                        vm.showContent = true;
                        vm.showWaitList = false;
                        vm.viewReady = true;
                    }
                });

} else {
    var notFoundRequest = Page.get({ url: "page-not-found" }, function() {
        if (notFoundRequest.pages.length > 0) {
            vm.content = notFoundRequest.pages[0].renderedContent;
        } else {
            vm.content = "404 Content not found";
        }
    });
}
});

        vm.signup = function () {
            //Add Person to group
            Group.Participant.save({
                groupId: vm.groupId
            }).$promise.then(function (response) {
                $rootScope.$emit('notify', $rootScope.MESSAGES.successfullRegistration);
                vm.showContent = false;
                vm.showSuccess = true;
                vm.showWaitList = false;
                vm.showWaitSuccess = true;
            }, function (error) {
                $rootScope.$emit('notify', $rootScope.MESSAGES.fullGroupError);
                vm.showContent = false;
                vm.showFull = true;
            });
        };

        function editProfile() {
            vm.modalInstance = $modal.open({
                templateUrl: 'editProfile.html',
                backdrop: true,
                // This is needed in order to get our scope
                // into the modal - by default, it uses $rootScope
                scope: $scope,
            });
        };

        function hasParticipantID(array){
            var result = [];
            if(array.length === 1){
                result[result.length] = array[0]['participantId'];
            }
            for (var i = 0; i < array.length; i++) {
                if(array[i]['newAdd'] !== undefined && array[i]['newAdd'] !== "")
                result[result.length] = array[i]['newAdd'];
                }
                return result;
        };
    }
})()
