(function() {
  'use strict';

  // NOTE: If updating this list of required modules, also update the same in crds-core!
  // TODO: Figure out a better way so we don't have to duplicate this in crds-core
  require('angular');
  require('angular-resource');
  require('angular-sanitize');
  require('angular-messages');
  require('angular-cookies');
  require('angular-toggle-switch');
  require('angular-scroll');
  require('angular-stripe');
  require('angular-animate');
  require('angular-payments');
  require('angular-bootstrap-npm');
  require('angular-ui-router');
  require('angular-ui-event');
  require('angular-ui-mask');
  require('angular-addthis');
  require('angular-aside');
  require('angular-match-media');
  require('angular-mailchimp');
  require('angular-image-crop');
  require('angulartics');
  require('angulartics-gtm');
  require('angularjs-slider');

  require('../node_modules/angularjs-slider/dist/rzslider.min.css');
  require('../node_modules/angular-toggle-switch/angular-toggle-switch-bootstrap.css');
  require('../node_modules/angular-toggle-switch/angular-toggle-switch.css');
})();
