'use strict()';
(function() {
    module.exports = function() {
      return {
        restrict: 'A',
        require: 'ngModel',
        link: function($scope, element, attrs, ngModel) {
           ngModel.$validators.initiativeRequired = function(value) {
            if(value == undefined){
              return false;
            }else{
              return true;
            }
          };
        }
      }
    };
})();
