(function() {
  'use strict';

  module.exports = ProfilePictureDirective;

  ProfilePictureDirective.$inject = [];

  function ProfilePictureDirective() {
    return {
      restrict: 'E',
      replace: true,
      scope: {

      },
      controller: 'ProfilePictureController as picture',
      bindToController: true,
      templateUrl: 'picture/profilePicture.html'
    };
  }

})();