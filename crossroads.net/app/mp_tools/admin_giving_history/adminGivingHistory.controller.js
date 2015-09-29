(function() {
  'use strict()';

  module.exports = AdminGivingHistoryController;

  AdminGivingHistoryController.$inject = ['$state', 'MPTools', 'GivingHistoryService'];

  function AdminGivingHistoryController($state, MPTools, GivingHistoryService) {
    var vm = this;

    activate();

    //////////////////////

    function activate() {
      var params = MPTools.getParams();

      var donorId = params.recordId;
      if (donorId !== undefined && donorId > 0) {
        goToGivingHistory(donorId);
        return;
      }

      var selectionId = params.selectedRecord;
      if (selectionId !== undefined && selectionId > 0) {
        MPTools.Selection.get({selectionId: selectionId}, function(data) {
          goToGivingHistory(data.RecordIds[0]);
        });
      }
    }

    function goToGivingHistory(donorId) {
      GivingHistoryService.impersonateDonorId = donorId;
      $state.go('tools.adminGivingHistory.view');
    }
  }
})();
