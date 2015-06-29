(function(){
  'use strict()';

  module.exports = KCApplicantController;

  KCApplicantController.$inject = ['$rootScope', '$log', 'VolunteerApplication', 'MPTools', 'Contact', 'CmsInfo' ];

  function KCApplicantController($rootScope, $log, VolunteerApplication, MPTools, Contact, CmsInfo) {

    var vm = this;
    vm.errorMessage = $rootScope.MESSAGES.toolsError;
    vm.params = MPTools.getParams();
    vm.person = Contact;
    vm.showAdult = showAdult;
    vm.showAgeError = showAgeError;
    vm.showError = showError;
    vm.showStudent = showStudent;
    vm.showSuccess = false;
    vm.viewReady = false;


    activate();
    //////////////////////

    function activate(){
      vm.person.middleInitial = VolunteerApplication.middleInitial(vm.person);
      vm.pageInfo = (CmsInfo.pages !== undefined && CmsInfo.pages.length > 0) ? CmsInfo.pages[0] : null;

      if(vm.pageInfo !== null){
        var response = VolunteerApplication.getResponse(vm.pageInfo.opportunity, vm.params.recordId)
          .then(function(response){
          if(response !== null && response.responseId !== undefined){
            vm.responseId = response.responseId;
            vm.viewReady = true;
          } else {
            vm.error = true;
            vm.errorMessage = $rootScope.MESSAGES.noResponse;
            vm.viewReady = true;
          }
        });
      } else {
        vm.error = true;
        vm.errorMessage = $rootScope.MESSAGES.generalError;
      }
    }

    function showAdult(){
      return !vm.showInvalidResponse && VolunteerApplication.show('adult', vm.person);
    }

    function showAgeError(){
      return (vm.person.age === null || vm.person.age === undefined || vm.person.age < 10);
    }

    function showError(){
      if (vm.params.selectedCount > 1 || vm.params.recordDescription === undefined || vm.params.recordId === '-1'){
        vm.errorMessage = $rootScope.MESSAGES.toolsError;
        vm.error = true;
      } else if(showAgeError()){
        vm.errorMessage = $rootScope.MESSAGES.ageError; 
        vm.error = $rootScope.MESSAGES.generalError;
      }
      return vm.error;
    }

    function showStudent(){
      return !vm.showInvalidResponse && VolunteerApplication.show('student', vm.person);
    }

  }

})();
