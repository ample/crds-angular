var cms_services_module = require('module');﻿

cms_service_module.factory('Message', function ($resource) {
    return $resource('http://content.crossroads.net/api/Message/:id', { id: '@_id' }, {cache: true});
});