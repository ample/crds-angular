(function(){
  'use strict()'; 
  
  var _ = require('lodash');
  
  module.exports = MPTools; 

  MPTools.$inject = ['$location'];

  function MPTools($location){ 
    var params =  {}; 
    return {
      setParams : function(location) {
        console.log('saving location info to a service');
        params = {
          userGuid: location.search()['ug'],
          domainGuid: location.search()['dg'],
          pageId: location.search()['pageID'],
          recordId: location.search()['recordID'],
          recordDescription: location.search()['recordDescription'],
          selectedRecord: location.search()['s'],
          selectedCount: location.search()['sc']
        };
      },
      getParams : function() {
        if(_.isEmpty(params) || params.userGuid === undefined){
          params = { 
            userGuid: $location.search()['ug'],
            domainGuid: $location.search()['dg'],
            pageId: $location.search()['pageID'],
            recordId: $location.search()['recordID'],
            recordDescription: $location.search()['recordDescription'],
            selectedRecord: $location.search()['s'],
            selectedCount: $location.search()['sc']
          };
        }
        return params;
      }
    };
  }

})();
