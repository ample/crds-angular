require('crds-core');
require('../../../app/common/common.module');
require('../../../app/trips/trips.module');

var tripHelpers = require('../trips.helpers');

describe('GOTrip NOLA Signup Application Controller', function() {
  var controller;
  var mockSession;
  var $rootScope;
  var $scope;
  var $log;
  var $state;
  var $httpBackend;
  var Session;
  var AUTH_EVENTS;
  var MESSAGES;

  beforeEach(angular.mock.module('crossroads.trips'));

  beforeEach(angular.mock.module(function($provide) {
    $provide.value('Campaign', tripHelpers.Nola);
    $provide.value('WorkTeams', tripHelpers.WorkTeams);
    $provide.value('contactId', 123456);
    $provide.value('Person', tripHelpers.Person);
    $provide.value('$state', { go: function() {} });

    mockSession = jasmine.createSpyObj('Session', ['exists', 'isActive', 'removeRedirectRoute', 'addRedirectRoute']);
    mockSession.exists.and.callFake(function(something) {
      return '12345678';
    });

    $provide.value('Session', mockSession);
  }));

  beforeEach(inject(function(_$controller_, _$log_, $injector, _MESSAGES_) {
    $rootScope = $injector.get('$rootScope');
    MESSAGES = _MESSAGES_;
    MESSAGES.NOLASignUpThankYou = 1;
    $scope = $rootScope.$new();
    controller = _$controller_('TripsSignupController', {$scope: $scope});
    $log = _$log_;
    Session = $injector.get('Session');
    $state = $injector.get('$state');
    $httpBackend = $injector.get('$httpBackend');
    AUTH_EVENTS = $injector.get('AUTH_EVENTS');
  }));

  it('should have correct destination of NOLA', function() {
    expect(controller.destination).toBe('NOLA');
  });

  it('should set nolaRequired to required', function() {
    expect(controller.nolaRequired()).toBe(true);
  });

  it('should set nicaRequired to empty string', function() {
    expect(controller.nicaRequired()).toBe(false);
  });

  it('should set requireInternational to false', function() {
    expect(controller.requireInternational()).toBe(false);
  });

  it('should set numberOfPages to 5', function() {
    expect(controller.numberOfPages).toBe(5);
  });

});
