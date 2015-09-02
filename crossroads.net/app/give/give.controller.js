(function () {
  'use strict';
  module.exports = GiveCtrl;

  GiveCtrl.$inject = ['$rootScope', 
                      '$state', 
                      '$timeout', 
                      'Session', 
                      'PaymentService',
                      'DonationService',
                      'programList', 
                      'GiveTransferService',
                      'GiveFlow',
                      'User', 
                      'AUTH_EVENTS',
                      'CC_BRAND_CODES'];

  function DonationException(message) {
    this.message = message;
    this.name = 'DonationException';
  }

  function GiveCtrl($rootScope, $state, $timeout, Session, PaymentService, DonationService, programList, GiveTransferService, GiveFlow, User, AUTH_EVENTS, CC_BRAND_CODES) {

    var vm = this;
    vm.activeSession = activeSession;
    vm.bankinfoSubmitted = false;
    vm.donorError = false;
    vm.donationService = DonationService;
    vm.dto = GiveTransferService;
    vm.emailAlreadyRegisteredGrowlDivRef = 1000;
    vm.emailPrefix = 'give';
    vm.giveFlow = GiveFlow;
    vm.initDefaultState = initDefaultState;
    vm.initialized = false;
    vm.dto.processing = false;
    vm.onEmailFound = onEmailFound;
    vm.onEmailNotFound = onEmailNotFound;
    vm.programsInput = programList;
    vm.showCheckClass = 'ng-hide';
    if (!vm.dto.view ){
      vm.dto.view = 'bank';
    }


    $rootScope.$on('$stateChangeStart', function (event, toState, toParams) {
       // Short-circuit this handler if we're not transitioning TO a give state
      if(toState && !/^give.*/.test(toState.name)) {
        return;
      }

       // vm.processing is used to set state and text on the "Give" button
       // Make sure to set the processing state to true whenever a state change begins
       vm.dto.processing = true;

       // If not initialized, initialize and go to default state
       if(!vm.initialized || toState.name === 'give') {
         event.preventDefault();
         vm.initDefaultState();
         return;
       }

       vm.transitionForLoggedInUserBasedOnExistingDonor(event,toState);
    });

    $rootScope.$on(AUTH_EVENTS.logoutSuccess, function(event) {
      vm.reset();
      $state.go('home');
    });


    $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams) {
      // vm.processing is used to set state and text on the "Give" button
      // Make sure to reset the processing state to false whenever state change succeeds.
      vm.dto.processing = false;
      // Force the state to reset after successfully giving
      if(toState.name === GiveFlow.thankYou) {
        vm.initialized = false;
      }
    });

    /// USE THE LOADING BUTTON DIRECTIVE TO HANDLE THIS
    $rootScope.$on('$stateChangeError', function (event, toState, toParams) {
      // vm.processing is used to set state and text on the "Give" button
      // Make sure to reset the processing state to false whenever state change fails.
      vm.dto.processing = false;
    });

   
    ////////////////////////////////
    //// IMPLEMENTATION DETAILS ////
    ////////////////////////////////

    function activeSession(){
      return (Session.isActive()); 
    }

    function initDefaultState() {
      // If we have not initialized (meaning we came in via a deep-link, refresh, etc),
      // reset state and redirect to start page (/give/amount).
      vm.reset();

      // Setup the give flow service
      GiveFlow.reset({
        amount: 'give.amount',
        account: 'give.account',
        login: 'give.login',
        register: 'give.register',
        confirm: 'give.confirm',
        change: 'give.change',
        thankYou: 'give.thank-you'
      });          

      vm.initialized = true;
      //// LEFTOVER FROM USING MULTIPLE STATE URLS LOOK AT REMOVING
      Session.removeRedirectRoute();
      $state.go(GiveFlow.amount);
    }

    // Callback from email-field on guest giver page.  Emits a growl
    // notification indicating that the email entered may already be a
    // registered user.
    function onEmailFound() {
      $rootScope.$emit(
          'notify',
          $rootScope.MESSAGES.donorEmailAlreadyRegistered,
          vm.emailAlreadyRegisteredGrowlDivRef,
          -1 // Indicates that this message should not time out
          );

      // This is a hack to keep from tabbing on the close button on the growl message.
      // There is no option in Growl to make the close button not tabbable...
      $timeout(function() {
        var closeButton = document.querySelector("#existingEmail .close");
        if(closeButton) {
          closeButton.tabIndex = -1;
        }
      }, 11);
    }

    function onEmailNotFound() {
      // There isn't a way to close growl messages in code, outside of the growl
      // directive itself.  To work around this, we'll simply trigger the "click"
      // event on the close button, which has a close handler function.
      var closeButton = document.querySelector("#existingEmail .close");
      if(closeButton !== undefined) {
          $timeout(function() {
              angular.element(closeButton).triggerHandler("click");
          }, 0);
      }
    }
    
        vm.processChange = function(){
          if (!Session.isActive()) {
            $state.go("give.login");
          }

          vm.dto.processingChange = true;
          vm.dto.amountSubmitted = false;
          $state.go("give.amount");
        };

        vm.processCreditCardChange = function (){
          if (vm.giveForm.$valid) {
            vm.dto.processing = true;
            vm.dto.declinedCard = false;
            vm.donationService.createCard();
            var pgram = _.find(vm.programsInput, { ProgramId: vm.dto.program.ProgramId });
            DonationService.processCreditCardChange(pgram, vm.donationService.card, function(){
              $state.go('give.thank-you');
            }); 
            vm.dto.processing = false;
          } else {
            vm.dto.processing = false;
            $rootScope.$emit('notify', $rootScope.MESSAGES.generalError);
          }
        };

        vm.reset = function() {
          vm.bankinfoSubmitted = false;
          vm.dto.email = undefined;
          vm.initialized = false;
          //vm.dto.processing = false;
          //vm.dto.processingChange = false;
          vm.donorError = false;
          if (!Session.isActive()) {
            User.email = "";
          };

          vm.dto.reset();
        }

        vm.submitBankInfo = function() {
          vm.bankinfoSubmitted = true;
          if (vm.giveForm.accountForm.$valid) {
            vm.dto.processing = true;
            PaymentService.getDonor(vm.giveForm.email)
            .then(function(donor){
                vm.updateDonorAndDonate(donor.id, vm.dto.program.ProgramId, vm.dto.amount, vm.dto.email, vm.dto.view);
            },
            function(error){
              vm.createDonorAndDonate(vm.dto.program.ProgramId, vm.dto.amount, vm.dto.email, vm.dto.view);
            });
          } else {
            $rootScope.$emit('notify', $rootScope.MESSAGES.generalError);
          }
        };

        vm.createDonorAndDonate = function(programId, amount, email, view) {
          // The vm.email below is only required for guest giver, however, there
          // is no harm in sending it for an authenticated user as well,
          // so we'll keep it simple and send it in all cases.
          var pgram = _.find(vm.programsInput, { ProgramId: programId });
          if (view == "cc") {
            vm.donationService.createCard();
            PaymentService.createDonorWithCard(vm.donationService.card, email)
              .then(function(donor) {
                DonationService.donate(pgram);
              }, PaymentService.stripeErrorHandler);
          } else if (view == "bank") {
            vm.donationService.createBank();
            PaymentService.createDonorWithBankAcct(vm.donationService.bank, email)
              .then(function(donor) {
                DonationService.donate(pgram);
              }, PaymentService.stripeErrorHandler);
         };
        }

        vm.updateDonorAndDonate = function(donorId, programId, amount, email, view) {
          // The vm.email below is only required for guest giver, however, there
          // is no harm in sending it for an authenticated user as well,
          // so we'll keep it simple and send it in all cases.
          var pgram = _.find(vm.programsInput, { ProgramId: programId });
          if (view == "cc") {
            vm.donationService.createCard();
            PaymentService.updateDonorWithCard(donorId, vm.donationService.card, email)
              .then(function(donor) {
                DonationService.donate(pgram);
              }, PaymentService.stripeErrorHandler);
         } else if (view == "bank") {
            vm.donationService.createBank();
            PaymentService.updateDonorWithBankAcct(donorId, vm.donationService.bank, email)
              .then(function(donor) {
                DonationService.donate(pgram);
              }, PaymentService.stripeErrorHandler);
         };
        }

        vm.submitChangedBankInfo = function() {
          if (!Session.isActive()) {
             $state.go("give.login");
          }
          vm.bankinfoSubmitted = true;
          vm.dto.amountSubmitted = true;
          if(vm.dto.amount === "") {
           $rootScope.$emit('notify', $rootScope.MESSAGES.generalError);
          } else {
            if (vm.dto.view == "cc") {
              if(vm.dto.savedPayment == 'bank') {
                vm.giveForm.creditCardForm.$setDirty();
              }
              if (!vm.giveForm.creditCardForm.$dirty){
                var pgram = _.find(vm.programsInput, { ProgramId: vm.dto.program.ProgramId });
                vm.dto.processing = true;
                DonationService.donate(pgram);
              } else {
                vm.processCreditCardChange();
              }
            } else if (vm.dto.view == "bank"){
              if(vm.dto.savedPayment == 'cc') {
                vm.giveForm.bankAccountForm.$setDirty();
              }
              if(!vm.giveForm.bankAccountForm.$dirty) {
                var pgram = _.find(vm.programsInput, { ProgramId: vm.dto.program.ProgramId });
                vm.dto.processing = true;
                DonationService.donate(pgram);
              } else {
                vm.donationService.processBankAccountChange(vm.giveForm, vm.programsInput);
              }
            };
          };
        };

      vm.transitionForLoggedInUserBasedOnExistingDonor = function(event, toState){
        if(toState.name == "give.account" && Session.isActive() && !vm.donorError ) {
          vm.dto.processing = true;
          event.preventDefault();
          PaymentService.getDonor(vm.giveForm.email)
          .then(function(donor){
            vm.dto.donor = donor;
            vm.dto.email = vm.dto.donor.email;
            if (vm.dto.donor.default_source.credit_card.last4 != null){
              vm.dto.last4 = vm.dto.donor.default_source.credit_card.last4;
              vm.dto.brand = CC_BRAND_CODES[vm.dto.donor.default_source.credit_card.brand];
              vm.expYear =  vm.dto.donor.exp_year;
              vm.exp_month = vm.dto.donor.exp_month;
            } else {
              vm.routing = vm.dto.donor.default_source.bank_account.routing;
              vm.account = vm.dto.donor.default_source.bank_account.last4
              vm.dto.last4 = vm.dto.donor.default_source.bank_account.last4;
              vm.dto.brand = '#library';
            };
            $state.go("give.confirm");
          },function(error){
            // Go forward to account info if it was a 404 "not found" error,
            // the donor service returns a 404 when a donor doesn't exist
            if(error && error.httpStatusCode == 404) {
              vm.donorError = true;
              $state.go("give.account");
            } else {
              PaymentService.stripeErrorHandler(error);
            }
          });
        }

      }
     };

})();
