(function() {
  'use strict';

  module.exports = RecurringGivingModals;

  RecurringGivingModals.$inject = ['$modalInstance',
      '$filter',
      'DonationService',
      'GiveTransferService',
      'donation',
      'programList'];

  function RecurringGivingModals($modalInstance,
                                 $filter,
                                 DonationService,
                                 GiveTransferService,
                                 donation,
                                 programList) {
    var vm = this;
    vm.dto = GiveTransferService;
    vm.programsInput = programList;
    vm.donation = donation;
    vm.cancel = cancel;
    vm.remove = remove;
    vm.edit = edit;

    activate($filter);

    function activate(filter) {
      vm.dto.reset();

      vm.dto.recurringGiftId = donation.recurring_gift_id;
      vm.dto.amount = vm.donation.amount;
      vm.dto.amountSubmitted = false;
      vm.dto.bankinfoSubmitted = false;
      vm.dto.changeAccountInfo = true;
      vm.dto.brand = '#' + vm.donation.source.icon;
      vm.dto.ccNumberClass = vm.donation.source.icon;
      vm.dto.givingType = vm.donation.interval;
      vm.dto.initialized = true;
      vm.dto.last4 = vm.donation.source.last4;
      vm.dto.program = filter('filter')(vm.programsInput, {ProgramId: vm.donation.program})[0];
      vm.dto.recurringStartDate = vm.donation.start_date;
      vm.dto.view = vm.donation.source.type === 'CreditCard' ? 'cc' : 'bank';
      setupInterval();
      setupDonor();
    }

    function setupDonor() {
      vm.dto.donor = {
        id: donation.donor_id,
        default_source: {
          credit_card: {
            last4: null,
            brand: null,
            address_zip: null,
            exp_date: null,
          },
          bank_account: {
            routing: null,
            last4: null,
          },
        },
      };

      if (vm.donation.source.type === 'CreditCard') {
        vm.dto.donor.default_source.credit_card.last4 = vm.donation.source.last4;
        vm.dto.donor.default_source.credit_card.brand = vm.donation.source.brand;
        vm.dto.donor.default_source.credit_card.address_zip = vm.donation.source.address_zip;
        vm.dto.donor.default_source.credit_card.exp_date = moment(vm.donation.source.exp_date).format('MMYY');
      } else {
        vm.dto.donor.default_source.bank_account.last4 = vm.donation.source.last4;
        vm.dto.donor.default_source.bank_account.routing = vm.donation.source.routing;
      }
    }

    function setupInterval() {
      if (vm.donation.interval !== null) {
        vm.dto.interval = _.capitalize(vm.donation.interval.toLowerCase()) + 'ly';
      }
    }

    function cancel() {
      $modalInstance.dismiss('cancel');
    }

    function remove() {
      DonationService.deleteRecurringGift().then(function() {
        $modalInstance.close(true);
      }, function(/*error*/) {

        $modalInstance.close(false);
      });
    }

    function edit(recurringGiveForm) {
      if ((recurringGiveForm.creditCardForm !== undefined && recurringGiveForm.creditCardForm.$dirty) ||
          (recurringGiveForm.bankAccountForm !== undefined && recurringGiveForm.bankAccountForm.$dirty)) {
        DonationService.updateRecurringGift(true).then(function() {
          $modalInstance.close(true);
        }, function(/*error*/) {

          $modalInstance.close(false);
        });
      } else if (recurringGiveForm.donationDetailsForm.$dirty) {
        DonationService.updateRecurringGift(false).then(function() {
          $modalInstance.close(true);
        }, function(/*error*/) {

          $modalInstance.close(false);
        });
      } else {
        $modalInstance.close(true);
      }
    }

  };

})();
