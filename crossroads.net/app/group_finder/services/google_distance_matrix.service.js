/* globals google */
(function () {
  'use strict';

  /*
    API Reference:  https://developers.google.com/maps/documentation/javascript/3.exp/reference#DistanceMatrixService
    API Example: https://developers.google.com/maps/documentation/javascript/examples/distance-matrix

    Service Initialization:
      The Google Maps API Javascript library is currently being loaded asynchronously by running an Init function
      provided by this library.  The init function should be executed inside an AngularJS run block:

         .run(require('./google_distance_matrix.service').init)

    Service Usage:

      GoogleDistanceMatrixService.distanceFromAddress('7915 Dream St, Florence, KY 41042', [
        '828 Heights Blvd, Florence, KY 41042',
        '3500 Montgomery Rd, Cincinnati, OH 45207',
        'completely invalid address'
      ]).then(function(result) {
        console.log('Distances', JSON.stringify(result));
      }, function(error) {
        $log.error("An error occurred");
      });

   The result is a data structure:

      [
        {
          "destination": "828 Heights Blvd, Florence, KY 41042, USA",
          "distance": {
            "text": "1.6 mi",
            "value": 2554      <- This is the distance in meters
          },
          "duration": {
            "text": "5 mins",
            "value": 313      <- This is the duration in seconds
          },
          "status": "OK"
        },
        {
          "destination": "3500 Montgomery Rd, Cincinnati, OH 45207, USA",
          "distance": {
            "text": "17.3 mi",
            "value": 27875     <- This is the distance in meters
          },
          "duration": {
            "text": "20 mins",
            "value": 1187      <- This is the duration in seconds
          },
          "status": "OK"
        },
        {
          "destination": "",
          "status": "NOT_FOUND"
        }
      ]

    Error conditions that will result in the rejection of the promise
      * The Google Maps JS library was not loaded correctly
      * The Google Maps API request failed
      * The origin address could not be resolved to a valid physical location
  */

  module.exports = {
    init: GoogleDistanceMatrixServiceInit,
    service: GoogleDistanceMatrixService
  };

  //
  // Export an init function to be used in an angular module run() block and load the Google Map API
  //

  GoogleDistanceMatrixServiceInit.$inject = ['GoogleDistanceMatrixService'];

  function GoogleDistanceMatrixServiceInit(GoogleDistanceMatrixService) {
    GoogleDistanceMatrixService.init();
  }

  //
  // Export a Service definition wrapping the Google Distance Matrix service
  //

  GoogleDistanceMatrixService.$inject = ['$q', '$log', 'GOOGLE_API_KEY'];

  function GoogleDistanceMatrixService($q, $log, GOOGLE_API_KEY) {
    var service = {};

    //
    // Service API
    //

    service.init = init;
    service.distanceFromAddress = distanceFromAddress;

    //
    // Service implementation
    //

    // If the Google maps JS api hasn't been loaded, load it now
    function init() {
      if (typeof google !== 'undefined' && google && google.maps) {
        return;
      }

      var script = document.createElement('script');
      script.type = 'text/javascript';
      script.async = true;
      script.src = 'https://maps.googleapis.com/maps/api/js?key=' + GOOGLE_API_KEY;

      var s = document.getElementsByTagName('script')[0];
      s.parentNode.insertBefore(script, s);
    }

    function handleResponse(response, status, deferred) {
      var err;

      if (status !== google.maps.DistanceMatrixStatus.OK) {
        err = 'Google maps could not process the request and resulted in status:' + status;
        $log.error(err);
        deferred.reject(err);
      }

      // If the origin address could not be found the rows array will exist but be empty
      var result = [];

      if (response.rows.length && response.rows[0].elements) {
        result = response.rows[0].elements;

        angular.forEach(response.destinationAddresses, function (value, key) {
          result[key].destination = value;
        });
      } else {
        err = 'Provided origin address could not be resolved to a physical location';
        $log.error(err);
        deferred.reject(err);
      }

      deferred.resolve(result);
    }

    function distanceFromAddress(startingAddress, destinationAddressList) {
      var deferred = $q.defer();

      if (typeof google === 'undefined' || !google || !google.maps) {
        var err = 'Google Maps API has not been loaded by init() method';
        $log.error(err);
        deferred.reject(err);
      }

      var matrixService = new google.maps.DistanceMatrixService();
      matrixService.getDistanceMatrix({
        origins: [startingAddress],
        destinations: destinationAddressList,
        travelMode: google.maps.TravelMode.DRIVING,
        unitSystem: google.maps.UnitSystem.IMPERIAL
      }, function(response, status) {
        handleResponse(response, status, deferred);
      });

      return deferred.promise;
    }

    // Return the service instance
    return service;
  }

})();
