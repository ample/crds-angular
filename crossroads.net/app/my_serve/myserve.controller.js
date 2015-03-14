'use strict()';
(function(){
  module.exports = function MyServeController($log, ServeOpportunities){
    
    var vm = this;

    vm.clear = clear; 
    vm.dateOptions = { formatYear: 'yy', startingDay: 1 }; 
    vm.disabled = disabled;
    vm.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate']; 
    vm.format = vm.formats[0];
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

    $log.debug(vm.groups);

    //////////////////////////////////////////////

    function getGroups(){
      vm.groups = ServeOpportunities;
    };

    function today() {
      vm.dt = new Date();
    };

    function clear() {
      vm.dt = null;
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
