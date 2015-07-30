(function(){
  module.exports = Trip;

  Trip.$inject = ['$resource'];

  function Trip($resource){
    return {
      Search: $resource(__API_ENDPOINT__ + 'api/trip/search/:query'),
      MyTrips: $resource(__API_ENDPOINT__ + 'api/trip/mytrips/:contact')
    };
  }
})();
