(function(){
  'use strict';

  module.exports = ResponseService;

  ResponseService.$inject = ['$log'];

  function ResponseService($log) {
    this.data = {};

    this.clear = function(){
      this.data = {};
    };
  }

})();
