(function () {
  angular.module('crossroads.give').factory('GiveTransferService',GiveTransferService);


    function GiveTransferService() {
        console.log("Inside Give GiveTransferService");
        var transferObject = {
          amount : '',
          donor : '',
          email : '',
          program : '',
          view : '',
        }

        return transferObject;
    }

})()
