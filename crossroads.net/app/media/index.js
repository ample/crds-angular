'use strict';

var app = angular.module('crossroads');

require('./viewAll.html');
require('./viewAllMusic.html');
require('./viewAllSeries.html');
require('./viewAllVideos.html');
require('./series-single.html');
require('./series-single-lo-res.html');
require('./media-single.html');
require('./subscribe-btn-messages.html');
require('./subscribe-btn-music.html');
require('./subscribe-btn-videos.html');
require('./subscribe-btn-dropdown.html');
require('./media-list.html');
require('./message-action-buttons.html');
require('./media-details.html');
require('./templates/mediaListCard.html');
require('./templates/seriesListCard.html');


app.controller('MediaController', require('./media.controller'));
app.factory('Media', require('./services/media.service'));
app.directive("mediaListCard", require('./directives/mediaListCard.directive.js'));