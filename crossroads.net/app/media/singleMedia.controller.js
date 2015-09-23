(function() {
  'use strict';
  module.exports = SingleMediaController;

  SingleMediaController.$inject = [
    '$rootScope',
    '$scope',
    '$sce',
    '$location',
    '$sanitize',
    'SingleMedia',
    'ItemProperty',
    'ParentMedia',
    'ParentItemProperty',
    'ImageURL',
    'YT_EVENT',
    'ResponsiveImageService',
    'ContentSiteConfigService'
  ];

  function SingleMediaController($rootScope,
                                 $scope,
                                 $sce,
                                 $location,
                                 $sanitize,
                                 SingleMedia,
                                 ItemProperty,
                                 ParentMedia,
                                 ParentItemProperty,
                                 ImageURL,
                                 YT_EVENT,
                                 ResponsiveImageService,
                                 ContentSiteConfigService) {
    var vm = this;
    vm.imageUrl = ImageURL;
    vm.isMessage = (ItemProperty === 'message');

    vm.isSubscribeOpen = false;
    vm.media = SingleMedia[ItemProperty];
    vm.pauseVideo = pauseVideo;
    vm.playVideo = playVideo;
    vm.setAudioPlayer = setAudioPlayer;
    vm.showSwitchToAudio = showSwitchToAudio;
    vm.showSwitchToVideo = showSwitchToVideo;
    vm.showAudioSection = showAudioSection;
    vm.showVideoSection = showVideoSection;
    vm.stopVideo = stopVideo;
    vm.switchToAudio = switchToAudio;
    vm.switchToVideo = switchToVideo;
    vm.videoStillIsVisible = true;
    vm.videoPlayerIsVisible = false;
    vm.showVideoDownloadLink = showVideoDownloadLink;
    vm.showAudioDownloadLink = showAudioDownloadLink;
    vm.showProgramDownloadLink = showProgramDownloadLink;
    vm.shareUrl = $location.absUrl();
    vm.sanitizedDescription = $sanitize(vm.media.description);
    vm.mediaTags = vm.media.tags;

    if (vm.isMessage) {
      vm.videoSectionIsOpen = true;
      vm.audio = vm.media.audio;
      vm.video = vm.media.video;
      vm.programDownloadLink = _.get(vm.media, 'program.filename');
      vm.mediaTags = vm.media.combinedTags;
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
      vm.video.videoUrl = _.get(vm.video, 'serviceId');
      vm.videoDownloadLink = _.get(vm.video, 'source.filename');
      $sce.trustAsResourceUrl(vm.videoUrl);
    }

    if (vm.audio) {
      debugger;
      vm.audio.audioUrl = ContentSiteConfigService.siteconfig.soundCloudURL + _.get(vm.audio, 'serviceId');
      vm.audioDownloadLink = _.get(vm.audio, 'source.filename');
    }

    if (ParentMedia) {
      vm.parentMedia = ParentMedia[ParentItemProperty];
    } else {
      vm.parentMedia = false;
    }

    $scope.$on(YT_EVENT.STATUS_CHANGE, function(event, data) {
      if (!$scope.yt) {
        return;
      }

      $scope.yt.playerStatus = data;
    });

    $scope.$on('$destroy', function() {
      stopAudioPlayer();
      stopVideo();
    });

    function pauseVideo() {
      sendControlEvent(YT_EVENT.PAUSE);
    }

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

    function showSwitchToAudio() {
      return vm.audio && vm.showVideoSection();
    }

    function showSwitchToVideo() {
      return vm.video && vm.showAudioSection();
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
      vm.pauseVideo();
      ResponsiveImageService.updateResponsiveImages();
    }

    function switchToVideo() {
      vm.videoSectionIsOpen = true;
      stopAudioPlayer();
      ResponsiveImageService.updateResponsiveImages();
    }

    function showVideoDownloadLink() {
      return ((vm.videoDownloadLink === undefined) ? false : true);
    }

    function showAudioDownloadLink() {
      return ((vm.audioDownloadLink === undefined) ? false : true);
    }

    function showProgramDownloadLink() {
      return ((vm.programDownloadLink === undefined) ? false : true);
    }
  }
})();
