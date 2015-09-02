(function() {
  'use strict';

  module.exports = GiveTransferService;

  GiveTransferService.$inject = ['Session', 'User'];

  function GiveTransferService(Session, User) {
    var transferObject = {
      reset: function() {
        this.account = '';
        this.amount = undefined;
        this.amountSubmitted = false;
        this.brand = '';
        this.ccNumberClass = '';
        this.changeAccountInfo = false;
        this.declinedPayment = false;
        this.donor = {};
        this.email = undefined;
        this.last4 = '';
        this.processing = false;
        this.processingChange = false;
        this.program = undefined;
        this.routing = '';
        this.savedPayment = '';
        this.view = '';

        if (!Session.isActive()) {
          User.email = '';
        }

      }
    };
    transferObject.reset();

    return transferObject;
  }
})();
