require('angular-module-resource');
var cms_services_module = angular.module('crdsCMS.services', ['ngResource']);

cms_services_module.factory('Message', function ($resource) {
    return $resource('http://content.crossroads.net/api/Message/:id', { id: '@_id' }, {cache: true});
});

cms_services_module.factory('Page', function ($resource) {
    return $resource('http://content.crossroads.net/api/Page/?URLSegment=:url', { url: '@_url' }, { cache: true });
});
