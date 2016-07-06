"use strict";
require('reflect-metadata');
require('zone.js/dist/zone');
require('zone.js/dist/long-stack-trace-zone');
var upgrade_adapter_1 = require('./upgrade-adapter');
require('./downgrades');
upgrade_adapter_1.upgradeAdapter.bootstrap(document.documentElement, ['crossroads'], { strictDi: false });
