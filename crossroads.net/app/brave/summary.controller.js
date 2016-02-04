(function () {
    'use strict';

    module.exports = SummaryCtrl;

    SummaryCtrl.$inject = ['$scope', '$log', '$state'];

    function SummaryCtrl ($scope, $log, $state) {
        $log.debug('summary.controller.js');

        var vm = this;

        vm.totalSlides = 5;
        vm.currentSlide = 1;
        vm.nextButton = 'Next';

        vm.nextSlide = function () {
            if (vm.currentSlide < vm.totalSlides) {
                vm.currentSlide++;
                if (vm.onLastSlide()) {
                    vm.nextButton = 'Choose a Role';
                }
            }
            if (vm.onLastSlide()) {
                $state.go('brave.welcome');
            }
        };

        vm.previousSlide = function () {
            if (vm.currentSlide > 1) {
                vm.currentSlide--;
            }
        };

        vm.showSlide = function (index) {
            return index === vm.currentSlide;
        };

        vm.onLastSlide = function () {
            return vm.currentSlide === vm.totalSlides;
        };


    }

})();
