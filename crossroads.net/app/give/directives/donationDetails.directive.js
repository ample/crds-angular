require('../donation-details.html');﻿

(function () {
    angular
    .module("crossroads.give")
    .directive("donationDetails", ['$log', donationDetails]);

    function donationDetails($log) {
        var directive = {
          restrict: 'EA',
          replace: true,
          scope: {
                amount: "=",
                program: "=",
                amountSubmitted: "=",
                programsIn: "=" 

            },
          templateUrl: 'give/donation-details.html',
          link: link
      };

      function link(scope, element, attrs) {
        console.log(scope.amount);
        scope.programs = scope.programsIn;
        scope.program = scope.programsIn[0];
        if (scope.amount != undefined) {
          scope.donationDetailsForm.amount = scope.amount;
        };  

        scope.amountError = function() {
          console.log('here');
            return (scope.amountSubmitted && scope.donationDetailsForm.amount.$invalid 
              && scope.donationDetailsForm.$error.naturalNumber 
              || scope.donationDetailsForm.$dirty 
              && scope.donationDetailsForm.amount.$invalid && scope.amountSubmitted)
        };

         scope.setProgramList = function(){
          console.log("fired");
          scope.ministryShow ? scope.program = '' : scope.program = scope.programs[0];
        }
      }
      return directive;
    }
})()
