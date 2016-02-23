(function(){
  'use strict';

  module.exports = QuestionCtrl;

  var constants = require('./constants');

  QuestionCtrl.$inject = ['$timeout', '$scope', '$compile', 'ImageService', 'Person', 'GROUP_TYPES'];

  function QuestionCtrl($timeout, $scope, $compile, ImageService, Person, GROUP_TYPES) {

    $scope.initialize = function() {
      $scope.states = constants.US_STATES;
      $scope.header = $compile('<span>' + $scope.definition.header + '<span>')($scope);
      $scope.description = $compile('<span>' + $scope.definition.description + '<span>')($scope);
      $scope.body = $compile('<span>' + $scope.definition.body + '<span>')($scope);
      $scope.help = $compile('<span>' + $scope.definition.help + '<span>')($scope);
      $scope.footer = $compile('<span>' + $scope.definition.footer + '<span>')($scope);

      $scope.person = null;
      $scope.profileImage = ImageService.DefaultProfileImage;
      $scope.defaultImage = ImageService.DefaultProfileImage;

      $scope.setupSlider();

      // Load the person data
      Person.getProfile().then(function(profile) {
        $scope.person = profile;
        $scope.profileImage = ImageService.ProfileImageBaseURL + profile.contactId;
      })
    };

    $scope.setupSlider = function() {
      var key = $scope.definition.key;
      $scope.sliderOptions = {
        hideLimitLabels: true,
        showSelectionBar: true,
        floor: (key === 'open_spots' ? 0 : 3),
        ceil: (key === 'open_spots' ? 10 : 12)
      };
      $scope.sliderDefault = (key === 'open_spots' ? 5 : 7);
      $scope.refreshSlider();
    };

    $scope.getTemplateUrl = function () {
      return 'question/input_'+ $scope.definition.input_type +'.html';
    };

    $scope.checkError = function() {
      $scope.$parent.applyErrors();
    };

    $scope.render = function(el) {
      return $scope[el].html();
    };

    $scope.onKeyUp = function(e) {
      if(e.keyCode === 13 && $scope.definition.input_type !== 'textarea') {
        $scope.$parent.nextQuestion();
      }
    };

    $scope.refreshSlider = function () {
      $timeout(function () {
        $scope.$broadcast('rzSliderForceRender');
      });
    };

    $scope.$on('stepChange', function(event, step) {
      if($scope.definition.input_type === 'number' && $scope.$parent.step === step) {
        $scope.setupSlider();
      }
    });

    // ----------------------------------- //

    $scope.initialize();
  }

})();
