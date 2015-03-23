'use strict()';
(function(){
  var moment = require('moment');
  
  module.exports = MyServeController;

  MyServeController.$inject = ['$log', 'ServeOpportunities', 'Session'];
    
  function MyServeController($log, ServeOpportunities, Session){
    
    var vm = this;

    vm.clear = clear; 
    vm.convertToDate = convertToDate;
    vm.dateOptions = { formatYear: 'yy', startingDay: 1 }; 
    vm.disabled = disabled;
    vm.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate']; 
    vm.format = vm.formats[0];
    vm.groups = [];
    vm.isCollapsed = true;
    vm.open = open; 
    vm.today = today;
    vm.toggleMin = toggleMin; 
    vm.repeating = '2';
   
    function activate(){
      today();
      toggleMin();
      getGroups();
    }
    activate();

    ////////////////////////////
    // Implementation Details //
    ////////////////////////////
    function getGroups(){
      vm.groups = ServeOpportunities.query();
    };

    function today() {
      vm.dt = new Date();
    };

    function clear() {
      vm.dt = null;
    };
    
    function convertToDate(date){
      // date comes in as mm/dd/yyyy, convert to yyyy-mm-dd for moment to handle
      var d = new Date(date);
      return d;
    };
    
    function disabled (date, mode) {
      return ( mode === 'day' && ( date.getDay() === 0 || date.getDay() === 6 ) );
    };
        
    function toggleMin() {
      vm.minDate = vm.minDate ? null : new Date();
    };

    function open($event) {
      $event.preventDefault();
      $event.stopPropagation();
      vm.opened = true;   
    };
  }
    
})();
