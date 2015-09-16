(function() {
  'use strict';

  module.exports = FamilySelectDirective;

  FamilySelectDirective.$inject = ['$state', 'TripsSignupService', '$rootScope'];

  function FamilySelectDirective($state, TripsSignupService, $rootScope) {
    return {
      restrict: 'E',
      replace: true,
      scope: {
        showSignUp: '@',
        familyMembers: '=',
      },
      templateUrl: 'familySelectTool/familySelect.html',
      link: link
    };

    function link(scope, el, attr) {
      scope.divClass = divClass;
      scope.goToApp = goToApp;
      scope.isOfAge = isOfAge;
      scope.isSignedUp = isSignedUp;
      scope.signUpQuestion = signUpQuestion;
      scope.signupService = TripsSignupService;
      scope.pClass = pClass;

      ////////////////////////////////
      //// Implementation Details ////
      ////////////////////////////////

      function divClass(member) {
        if (!member.signedUp && isOfAge(member)) {
          return 'col-sm-9 col-md-10';
        }

        return '';
      }

      function goToApp(contactId) {
        scope.signupService.contactId = contactId;
        $state.go('tripsignup.application', {campaignId: scope.signupService.campaign.id});
      }

      function isOfAge(member) {
        if (member.age !== 0) {
          if (member.age < TripsSignupService.campaign.ageLimit) {
            if (_.includes(TripsSignupService.campaign.ageExceptions, Number(member.contactId))) {
              return true;
            }

            return false;
          }
        }

        return true;
      }

      function isSignedUp(member) {
        return member.signedUp;
      }

      function pClass(member) {
        if (!member.signedUp) {
          return 'flush-bottom';
        }

        return '';
      }

      function signUpQuestion() {
        if (_.some(scope.familyMembers, 'signedUp', false)) {
          if (scope.showSignUp === 'page0') {
            return $rootScope.MESSAGES.TripSignupFamilySelection.content;
          }

          if (scope.showSignUp === 'thankyou') {
            return $rootScope.MESSAGES.TripSignupOtherFamily.content;
          }
        }

        return '';
      }

    }

  }

})();
