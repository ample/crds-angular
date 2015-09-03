(function() {
  'use strict';

  module.exports = DonationService;

  DonationService.$inject = ['$rootScope', 'Session', 'GiveTransferService', 'PaymentService', 'GiveFlow', '$state', 'CC_BRAND_CODES'];

  function DonationService($rootScope, Session, GiveTransferService, PaymentService, GiveFlow, $state, CC_BRAND_CODES) {
    var donationService = {
      bank: {},
      card: {},
      createBank: createBank,
      createCard: createCard,
      createDonorAndDonate: createDonorAndDonate,
      confirmDonation: confirmDonation,
      donate: donate,
      processBankAccountChange: processBankAccountChange,
      processChange: processChange,
      processCreditCardChange: processCreditCardChange,
      transitionForLoggedInUserBasedOnExistingDonor: transitionForLoggedInUserBasedOnExistingDonor,
      submitBankInfo: submitBankInfo,
      submitChangedBankInfo: submitChangedBankInfo,
      updateDonorAndDonate: updateDonorAndDonate,
    };

    function createBank() {
      donationService.bank = {
        country: 'US',
        currency: 'USD',
        routing_number: GiveTransferService.donor.default_source.routing,
        account_number: GiveTransferService.donor.default_source.bank_account_number
      };
    }

    function createCard() {
      donationService.card = {
        name: GiveTransferService.donor.default_source.name,
        number: GiveTransferService.donor.default_source.cc_number,
        exp_month: GiveTransferService.donor.default_source.exp_date.substr(0,2),
        exp_year: GiveTransferService.donor.default_source.exp_date.substr(2,2),
        cvc: GiveTransferService.donor.default_source.cvc,
        address_zip: GiveTransferService.donor.default_source.address_zip
      };
    }

    function createDonorAndDonate(programsInput) {
      var pgram;
      if (programsInput !== undefined) {
        pgram = _.find(programsInput, { ProgramId: GiveTransferService.program.ProgramId });
      } else {
        pgram = GiveTransferService.program;
      }
      if (GiveTransferService.view === 'cc') {
        donationService.createCard();
        PaymentService.createDonorWithCard(donationService.card, GiveTransferService.email)
          .then(function(donor) {
            donationService.donate(pgram);
          }, PaymentService.stripeErrorHandler);
      } else if (GiveTransferService.view === 'bank') {
        donationService.createBank();
        PaymentService.createDonorWithBankAcct(donationService.bank, GiveTransferService.email)
          .then(function(donor) {
            donationService.donate(pgram);
          }, PaymentService.stripeErrorHandler);
      }
    }

    function confirmDonation(programsInput) {
      if (!Session.isActive()) {
        $state.go(GiveFlow.login);
      }

      GiveTransferService.processing = true;
      try {
        var pgram;
        if (programsInput !== undefined) {
          pgram = _.find(programsInput, { ProgramId: GiveTransferService.program.ProgramId });
        } else {
          pgram = GiveTransferService.program;
        }

        donationService.donate(pgram, function(confirmation) {

        }, function(error) {

          if (GiveTransferService.declinedPayment) {
            GiveFlow.goToChange();
          }
        });
      } catch (DonationException) {
        $rootScope.$emit('notify', $rootScope.MESSAGES.failedResponse);
      }
    }

    function donate(program, onSuccess, onFailure) {
      PaymentService.donateToProgram(program.programId,
          GiveTransferService.amount,
          GiveTransferService.donor.donorId,
          GiveTransferService.email,
          GiveTransferService.pymtType).then(function(confirmation) {
            GiveTransferService.amount = confirmation.amount;
            GiveTransferService.program = program;
            GiveTransferService.program_name = GiveTransferService.program.Name;
            GiveTransferService.email = confirmation.email;
            if (onSuccess !== undefined) {
              onSuccess(confirmation);
            }
            $state.go(GiveFlow.thankYou);
          }, function(error) {

            GiveTransferService.processing = false;
            PaymentService.stripeErrorHandler(error);
            if (onSuccess !== undefined && onFailure !== undefined) {
              onFailure(error);
            }
          });
    }

    function processBankAccountChange(giveForm, programsInput) {
      if (giveForm.$valid) {
        GiveTransferService.processing = true;
        donationService.createBank();
        PaymentService.updateDonorWithBankAcct(GiveTransferService.donor.id,donationService.bank,GiveTransferService.email)
         .then(function(donor) {
           var pgram;
           if (programsInput !== undefined) {
             pgram = _.find(programsInput, { ProgramId: GiveTransferService.program.ProgramId });
           } else {
             pgram = GiveTransferService.program;  
           }
           donationService.donate(pgram);
        }, PaymentService.stripeErrorHandler);
      } else {
         $rootScope.$emit('notify', $rootScope.MESSAGES.generalError);
       }

    }

    function processChange() {
      if (!Session.isActive()) {
        $state.go(GiveFlow.login);
      }

      GiveTransferService.processingChange = true;
      GiveTransferService.amountSubmitted = false;
      $state.go(GiveFlow.amount);
    }

    function processCreditCardChange(giveForm, programsInput) {
      if (giveForm.$valid) {
        GiveTransferService.processing = true;
        GiveTransferService.declinedCard = false;
        donationService.createCard();
        var pgram;
        if (programsInput !== undefined) {
          pgram = _.find(programsInput, { ProgramId: GiveTransferService.program.ProgramId });
        } else {
          pgram = GiveTransferService.program;
        }
        PaymentService.updateDonorWithCard(GiveTransferService.donor.id, donationService.card, GiveTransferService.email)
          .then(function(donor) {
            donate(pgram, function() {
            
            }, function(error) {
              GiveTransferService.processing = false;
              PaymentService.stripeErrorHandler(error);
            });
          },function(error) {
            GiveTransferService.processing = false;
            PaymentService.stripeErrorHandler(error);
          });

      } else {
        GiveTransferService.processing = false;
        $rootScope.$emit('notify', $rootScope.MESSAGES.generalError);
      }
    }

   function submitBankInfo(giveForm, programsInput) {
      GiveTransferService.bankinfoSubmitted = true;
      if (giveForm.accountForm.$valid) {
        GiveTransferService.processing = true;
        PaymentService.getDonor(giveForm.email)
          .then(function(donor){
            donationService.updateDonorAndDonate(donor.id, programsInput);
          },
          function(error){
            donationService.createDonorAndDonate(programsInput);
          });
      } else {
        $rootScope.$emit('notify', $rootScope.MESSAGES.generalError);
      }
    }

    function submitChangedBankInfo(giveForm, programsInput) {

      var pgram = (programsInput !== undefined) ?  
        _.find(programsInput, { ProgramId: GiveTransferService.program.ProgramId }) :
        GiveTransferService.program;

      if (!Session.isActive()) {
        $state.go(GiveFlow.login);
      }
      
      GiveTransferService.bankinfoSubmitted = true;
      GiveTransferService.amountSubmitted = true;

      if(GiveTransferService.amount === '') {
        $rootScope.$emit('notify', $rootScope.MESSAGES.generalError);
      } else {
        if (GiveTransferService.view === 'cc') {
          if(GiveTransferService.savedPayment === 'bank') {
            giveForm.creditCardForm.$setDirty();
          }
          if (!giveForm.creditCardForm.$dirty) {
            GiveTransferService.processing = true;
            donationService.donate(pgram);
          } else {
            donationService.processCreditCardChange(giveForm, programsInput);
          }
        } else if (GiveTransferService.view === 'bank'){
          if(GiveTransferService.savedPayment === 'cc') {
            giveForm.bankAccountForm.$setDirty();
          }
          if(!giveForm.bankAccountForm.$dirty) {
            GiveTransferService.processing = true;
            donationService.donate(pgram);
          } else {
            donationService.processBankAccountChange(giveForm, programsInput);
          }
        }
      }
    }
    
    function transitionForLoggedInUserBasedOnExistingDonor(event, toState) {
      if(toState.name === GiveFlow.account && Session.isActive() && !GiveTransferService.donorError ) {
        GiveTransferService.processing = true;
        event.preventDefault();
        PaymentService.getDonor(GiveTransferService.email)
        .then(function(donor){
          GiveTransferService.donor = donor;
          if (GiveTransferService.donor.default_source.credit_card.last4 != null){
            GiveTransferService.last4 = donor.default_source.credit_card.last4;
            GiveTransferService.brand = CC_BRAND_CODES[donor.default_source.credit_card.brand];
          } else {
            GiveTransferService.last4 = donor.default_source.bank_account.last4;
            GiveTransferService.brand = '#library';
          }
          $state.go(GiveFlow.confirm);
        },function(error){
          // Go forward to account info if it was a 404 "not found" error,
          // the donor service returns a 404 when a donor doesn't exist
          if(error && error.httpStatusCode === 404) {
            GiveTransferService.donorError = true;
            $state.go(GiveFlow.account);
          } else {
            PaymentService.stripeErrorHandler(error);
          }
        });
      }
    } 


    function updateDonorAndDonate(donorId, programsInput) {
      // The vm.email below is only required for guest giver, however, there
      // is no harm in sending it for an authenticated user as well,
      // so we'll keep it simple and send it in all cases.
      var pgram;
      if (programsInput !== undefined){
        pgram = _.find(programsInput, { ProgramId: GiveTransferService.program.ProgramId });
      } else {
        pgram = GiveTransferService.program
      }
      if (GiveTransferService.view === 'cc') {
        donationService.createCard();
        PaymentService.updateDonorWithCard(donorId, donationService.card, GiveTransferService.email)
          .then(function(donor) {
            donationService.donate(pgram);
          }, PaymentService.stripeErrorHandler);
      } else if (GiveTransferService.view === 'bank') {
        donationService.createBank();
        PaymentService.updateDonorWithBankAcct(donorId, donationService.bank, GiveTransferService.email)
          .then(function(donor) {
            donationService.donate(pgram);
          }, PaymentService.stripeErrorHandler);
      }
    }

    return donationService;
  }
})();
