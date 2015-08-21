"use strict()";
(function () {

  module.exports = Media;

  function Media($resource) {
    return {
      Series: function () {
        return $resource(__CMS_ENDPOINT__ + 'api/series/');
      },
      Musics: function() {
        return $resource(__CMS_ENDPOINT__ + 'api/musics/')
      },
      Videos: function() {
        return $resource(__CMS_ENDPOINT__ + 'api/videos/')
      }
    };
  }

})();