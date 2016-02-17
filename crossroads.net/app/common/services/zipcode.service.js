(function () {
  'use strict()';

  var _ = require('lodash');

  module.exports = ZipcodeService;

  //
  // Export a Service definition defining the Zipcode array check
  //

  ZipcodeService.$inject = ['$log'];

  function ZipcodeService($log) {
    var zipcodeArray = [40355,41001,41005,41006,41007,41011,41012,41014,41015,41016,41017,41018,41019,41022,41030,41033,41035,41040,41042,41045,41046,41048,41051,41052,41053,41054,41059,41063,41071,41072,41073,41074,41075,41076,41080,41083,41085,41086,41091,41092,41094,41095,41097,41099,45001,45002,45012,45014,45015,45018,45030,45033,45041,45051,45052,45053,45061,45063,45069,45071,45102,45103,45111,45112,45140,45147,45150,45153,45156,45157,45160,45174,45201,45202,45203,45204,45205,45206,45207,45208,45209,45211,45212,45213,45214,45215,45216,45217,45218,45219,45220,45221,45222,45223,45224,45225,45226,45227,45229,45230,45231,45232,45233,45234,45235,45236,45237,45238,45239,45240,45241,45242,45243,45244,45245,45246,45247,45248,45249,45250,45251,45252,45253,45254,45255,45258,45262,45263,45264,45267,45268,45269,45270,45271,45273,45274,45275,45277,45280,45296,45298,45299,45999,47001,47011,47017,47018,47019,47020,47021,47022,47025,47031,47032,47035,47038,47040,47041,47043,47060,45003,45004,45005,45011,45013,45032,45034,45036,45039,45040,45042,45044,45050,45054,45055,45056,45062,45064,45065,45066,45067,45068,45070,45107,45113,45114,45118,45122,45142,45146,45148,45152,45154,45158,45162,45176,45177,45301,45305,45311,45325,45327,45330,45342,45343,45345,45370,45381,45401,45402,45403,45406,45409,45410,45412,45413,45417,45419,45420,45422,45423,45428,45429,45430,45432,45434,45437,45439,45440,45441,45448,45449,45458,45459,45469,45470,45475,45479,45481,45482,45490,45106,45119,45120,45130,47016,47003,47006,47010,47012,47024,47030,47033,47036,47039];
    var service = {};

    //
    // Service API
    //

    service.isLocalZipcode = isLocalZipcode;

    //
    // Service implementation
    //

    function isLocalZipcode(zipcode) {
      var found = _.contains(zipcodeArray, zipcode);
      $log.debug('Zipcode', zipcode, 'found?', found);

      return found;
    }

    // Return the service instance
    return service;
  }

})();