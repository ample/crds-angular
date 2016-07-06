"use strict";
var upgrade_adapter_1 = require('./upgrade-adapter');
var ng2test_component_1 = require('./ng2test/ng2test.component');
var streaming_component_1 = require('./streaming/streaming.component');
angular.module('crossroads')
    .directive('ng2Test', upgrade_adapter_1.upgradeAdapter.downgradeNg2Component(ng2test_component_1.Ng2TestComponent));
angular.module('crossroads')
    .directive('streaming', upgrade_adapter_1.upgradeAdapter.downgradeNg2Component(streaming_component_1.StreamingComponent));
