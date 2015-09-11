(function() {
  'use strict';

  module.exports = TripsSignupService;

  TripsSignupService.$inject = ['$resource', '$location'];

  function TripsSignupService($resource, $location) {
    var signupService = {
      activate: activate,
      reset: reset,
    };

    function activate() {
      switch (signupService.campaign.formName) {
        case 'GO NOLA Application':
          signupService.friendlyPageTitle = 'New Orleans';
          signupService.tripName = '';
          signupService.numberOfPages = 5;
          break;
        case 'GO South Africa Application':
          signupService.friendlyPageTitle = 'South Africa';
          signupService.tripName = '';
          signupService.numberOfPages = 6;
          break;
        case 'GO India Application':
          signupService.friendlyPageTitle = 'India';
          signupService.tripName = '';
          signupService.numberOfPages = 6;
          signupService.whyPlaceholder = 'Please be specific. ' +
            'In instances where we have a limited number of spots, we strongly consider responses to this question.';
          break;
        case 'GO Nicaragua Application':
          signupService.friendlyPageTitle = 'Nicaragua';
          signupService.tripName = '';
          signupService.numberOfPages = 6;
          break;
      }
    }

    function reset(campaign) {
      signupService.campaign = campaign;
      signupService.ageLimitReached = false;
      signupService.contactId = '';
      signupService.currentPage = 1;
      signupService.numberOfPages = 0;
      signupService.pageHasErrors = true;
      signupService.privateInvite = $location.search().invite;

      signupService.page2 = resetPageTwo();

    }

    function resetPageTwo() {
      var p2 = {};
      p2.guardianFirstName = '';
      p2.guardianLastName = '';
      p2.tshirtSize = '';
      p2.scrubSize = '';
      p2.referral = '';
      p2.conditions = '';
      p2.allergies = '';
      p2.spiritualLifeSearching = '';
      p2.spiritualLifeReceived = '';
      p2.spiritualLifeObedience = '';
      p2.spiritualLifeReplicating = '';
      p2.why = '';
      return p2;
    }

    return signupService;
  }
})();
