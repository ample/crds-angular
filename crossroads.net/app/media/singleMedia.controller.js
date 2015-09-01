(function() {
  'use strict';
  module.exports = SingleMediaController;

  SingleMediaController.$inject = ['$rootScope', '$scope', '$sce', 'SingleMedia', 'ItemProperty', 'ParentMedia', 'ParentItemProperty', 'ImageURL', 'YT_EVENT'];

  function SingleMediaController($rootScope, $scope, $sce, SingleMedia, ItemProperty, ParentMedia, ParentItemProperty, ImageURL, YT_EVENT) {
    var vm = this;
    vm.imageUrl = ImageURL;
    vm.isMessage = (ItemProperty === 'messages');

    vm.isSubscribeOpen = false;
    vm.media = SingleMedia[ItemProperty][0];
    vm.playVideo = playVideo;
    vm.setAudioPlayer = setAudioPlayer;
    vm.showAudioSection = showAudioSection;
    vm.showVideoSection = showVideoSection;
    vm.stopVideo = stopVideo;
    vm.switchToAudio = switchToAudio;
    vm.switchToVideo = switchToVideo;
    vm.videoStillIsVisible = true;
    vm.videoPlayerIsVisible = false;

    if (vm.isMessage) {
      vm.videoSectionIsOpen = true;
      vm.audio = vm.media.audio;
      vm.video = vm.media.video;
    } else {
      if (vm.media.className === 'Music') {
        vm.audio = vm.media;
        vm.video = null;
        vm.videoSectionIsOpen = false;
      } else if (vm.media.className === 'Video') {
        vm.audio = null;
        vm.video = vm.media;
        vm.videoSectionIsOpen = true;
      }
    }

    if (vm.video) {
      // if the video url is bound directly in the iframe at some point, it will need to be marked as
      // trusted for Strict Contextual Escaping, such as --
      // $sce.trustAsResourceUrl("https://www.youtube.com/embed/" + _.get(vm.media, 'serviceId'));
      vm.video.videoUrl = _.get(vm.video, 'serviceId');
      $sce.trustAsResourceUrl(vm.videoUrl);
    }

    if (ParentMedia) {
      vm.parentMedia = ParentMedia[ParentItemProperty][0];
    } else {
      vm.parentMedia = false;
    }

    $scope.$on(YT_EVENT.STATUS_CHANGE, function(event, data) {
      $scope.yt.playerStatus = data;
    });

    $scope.$on('$destroy', function() {
      stopAudioPlayer();
      stopVideo();
    });

    function playVideo() {
      vm.videoStillIsVisible = false;
      vm.videoPlayerIsVisible = true;
      sendControlEvent(YT_EVENT.PLAY);
    }

    function stopVideo() {
      sendControlEvent(YT_EVENT.STOP);
      vm.videoStillIsVisible = true;
      vm.videoPlayerIsVisible = false;
    }

    function setAudioPlayer(audioPlayer) {
      vm.audioPlayer = audioPlayer;
    }

    function sendControlEvent(ctrlEvent) {
      $rootScope.$broadcast(ctrlEvent);
    }

    function showAudioSection() {
      return !showVideoSection();
    }

    function showVideoSection() {
      return vm.videoSectionIsOpen;
    }

    function stopAudioPlayer() {
      if (!vm.audioPlayer) {
        return;
      }

      if (!vm.audioPlayer.playing) {
        return;
      }

      vm.audioPlayer.pause();
    }

    function switchToAudio() {
      vm.videoSectionIsOpen = false;
      vm.stopVideo();
    }

    function switchToVideo() {
      vm.videoSectionIsOpen = true;
      stopAudioPlayer();
    }
  }
})();
