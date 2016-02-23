(function() {
  'use strict';

  module.exports = GroupContactCtrl;

  GroupContactCtrl.$inject = [
      '$modalInstance',
      'fromContactId',
      'toContactIds',
      '$log',
      '$scope',
      'Email'
  ];

  function GroupContactCtrl(
      $modalInstance,
      fromContactId,
      toContactIds,
      $log,
      $scope,
      Email
    ) {

    var vm = this;
    vm.cancel = cancel;
    vm.fromContactId = fromContactId;
    vm.toContactId = toContactIds;
    vm.sending = false;

    vm.subject = '';
    vm.message = '';

    vm.errors = {
      subject: false,
      message: false,
      unknown: false
    };

    vm.send = function() {
      vm.errors = {
        subject: false,
        message: false,
        unknown: false
      };
      vm.sending = true;
      // TODO add 'working' indicator, success/failure message, and validation
      var toSend = {
        fromContact: fromContactId,
        toContacts: toContactIds,
        subject: vm.subject,
        body: vm.message
      };

      if (vm.subject.length <= 0 ) {
        vm.errors.subject = true;
      }

      if (vm.message.length <= 0) {
        vm.errors.message = true;
      }

      if (vm.errors.message === false && vm.errors.subject === false) {
        Email.GroupMail.save(toSend, function() {
          $log.debug('GroupContactCtrl - message sent successfully');
          $modalInstance.close($scope.sent = true);
        }, function() {
          vm.sending = false;
          $log.debug('GroupContactCtrl - error sending message');
          // TODO general error message
          vm.errors.unknown = true;
        });
      } else {
        vm.sending = false;
      }

    };

    function cancel() {
      $modalInstance.close();
    }

  }
})();
