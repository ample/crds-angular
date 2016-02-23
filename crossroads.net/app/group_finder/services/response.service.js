(function(){
  'use strict';

  module.exports = ResponseService;

  ResponseService.$inject = [];

  function ResponseService() {
    this.data = {};

    this.clear = function(){
      this.data = {};
    };

    this.getResponse = function(definition) {
      return this.data[definition.key];
    };

  }

})();
