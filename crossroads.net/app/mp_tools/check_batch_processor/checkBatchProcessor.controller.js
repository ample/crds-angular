(function() {
  'use strict()';

  module.exports = CheckBatchProcessor;

  CheckBatchProcessor.$inject = ['$rootScope', '$log', 'MPTools', 'CheckScannerBatches', 'getPrograms', 'AuthService', 'GIVE_PROGRAM_TYPES', 'GIVE_ROLES'];

  function CheckBatchProcessor($rootScope, $log, MPTools, CheckScannerBatches, getPrograms, AuthService, GIVE_PROGRAM_TYPES, GIVE_ROLES) {
    var vm = this;

    vm.allBatches = [];
    vm.batch = {};
    vm.batches = [];
    vm.openBatches = [];
    vm.programs = [];
    vm.program = {};
    vm.processing = false;
    vm.params = MPTools.getParams();
    vm.showClosedBatches = false;

    activate();
    //////////////////////

    function activate() {
      CheckScannerBatches.query({'onlyOpen': false}, function(data) {
        vm.allBatches = data;
        vm.openBatches = _.filter(data, {'status': 'notExported'});
        vm.batches = vm.openBatches;
      });

      getPrograms.Programs.get({'excludeTypes[]': [GIVE_PROGRAM_TYPES.NonFinancial]}, function(data) {
        vm.programs = _.sortBy(data, 'Name');
      });
    }

    vm.allowAccess = function() {
      return(AuthService.isAuthenticated() && AuthService.isAuthorized(GIVE_ROLES.StewardshipDonationProcessor));
    }

    vm.filterBatches = function() {
      vm.batches = vm.showClosedBatches ? vm.allBatches : vm.openBatches;
    }

    vm.processBatch = function() {
      vm.processing = true;

      CheckScannerBatches.save({name: vm.batch.name, programId: vm.program.ProgramId}).$promise.then(function(){
          vm.success = true;
          vm.error = false;
        }, function(){
          vm.success = false;
          vm.error=true;
      }).finally(function(){
        vm.processing = false;
      });
    };
  }
})();
