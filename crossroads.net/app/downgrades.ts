import {upgradeAdapter} from './upgrade-adapter';
import { Ng2TestComponent } from './ng2test/ng2test.component';
import { StreamingComponent } from './streaming/streaming.component';

declare let angular:any;

angular.module('crossroads')
    .directive('ng2Test', upgradeAdapter.downgradeNg2Component(Ng2TestComponent));

angular.module('crossroads')
    .directive('streaming', upgradeAdapter.downgradeNg2Component(StreamingComponent));