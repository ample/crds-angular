
'use strict';
(function () {

  module.exports = function GiveCtrl($rootScope, $scope, $state, $timeout) {

    if(document.location.hash == "#/give"){
        $state.go("give.amount");
    }

        $rootScope.$on('$stateChangeStart', function (event, toState, toParams) {

            if(toState.name =="give.thank-you" && $scope.giveForm.giveForm.routing.$error.invalidRouting || toState.name =="give.thank-you" && $scope.giveForm.giveForm.account.$error.invalidAccount){
                $rootScope.$emit('notify', $rootScope.MESSAGES.generalError);
                event.preventDefault();
            }

             if(toState.name =="give.account" && $scope.giveForm.giveForm.amount.$error.naturalNumber){
                console.log($scope.giveForm.giveForm.amount.$error);
                $rootScope.$emit('notify', $rootScope.MESSAGES.generalError);
                event.preventDefault();
            }

        });

        var vm = this;
        vm.submitted = false;
        //Credit Card RegExs
        //var visaRegEx = /^4[0-9]{2}/;
        var visaRegEx = /^4[0-9]{12}(?:[0-9]{3})?$ /;
        var mastercardRegEx = /^5[1-5][0-9]/;
        //var discoverRegEx = /^6(?:011|5[0-9]{2})/;
        var discoverRegEx =/^6(?:011|5[0-9]{2})[0-9]{12}$/;
        //var americanExpressRegEx = /^3[47]/;
        var americanExpressRegEx = /^3[47][0-9]{13}$/;

        vm.view = 'bank';
        vm.bankType = 'checking';
        vm.showMessage = "Where?";
        vm.showCheckClass = "ng-hide";

        console.log("in the controller");

        vm.alerts = [
            {
                type: 'warning',
                msg: "If it's all the same to you, please use your bank account (credit card companies charge Crossroads a fee for each gift)."
            }
        ]

       vm.toggleCheck = function() {
            if (vm.showMessage == "Where?") {
                vm.showMessage = "Close";
                vm.showCheckClass = "";
            } else {
                vm.showMessage = "Where?";
                vm.showCheckClass = "ng-hide";
            }
        }

        vm.closeAlert = function (index) {
            vm.alerts.splice(index, 1);
        }

        vm.ccCardType = function () {
            if (vm.ccNumber) {
                if (vm.ccNumber.match(visaRegEx))
                  vm.ccNumberClass = "cc-visa";
                else if (vm.ccNumber.match(mastercardRegEx))
                  vm.ccNumberClass = "cc-mastercard";
                else if (vm.ccNumber.match(discoverRegEx))
                  vm.ccNumberClass = "cc-discover";
                else if (vm.ccNumber.match(americanExpressRegEx))
                  vm.ccNumberClass = "cc-american-express";
                else
                  vm.ccNumberClass = "";
            } else
                vm.ccNumberClass = "";
        }

        vm.goToAccount = function(){
            console.log($scope.giveForm.giveForm.amount.$error.naturalNumber);
            $timeout(function(){
                vm.submitted = true;
                $state.go("give.account");
            });
        };

    };

})();
