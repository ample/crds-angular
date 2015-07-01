describe('Serve Modal Controller', function() {

  var controller, 
      $scope, 
      $log, 
      $httpBackend, 
      $modalInstance, 
      mockModalInstance;

  var mockDates = {
    'fromDate': new Date(), 
    'toDate' : new Date() 
  };

  beforeEach(module('crossroads'));

  beforeEach(module(function($provide){
    mockModalInstance = jasmine.createSpyObj('$modalInstance', ['close']);
    mockModalInstance.close.and.callFake(function(obj){
      return true;
    });
    
    $provide.value('dates', mockDates); 
    $provide.value('$modalInstance', mockModalInstance); 
  }));

  beforeEach(inject(function(_$controller_, _$log_, $injector){
    $log = _$log_;
    $httpBackend = $injector.get('$httpBackend');
    $modalInstance = $injector.get('$modalInstance');
    $scope = {};
    controller = _$controller_('ServeModalController', { $scope: $scope });
    controller.filterdates = setupFormErrors(); 
  }));

  it('should have an error when the TO date is prior to the FROM date', function(){
    controller.toDate = new Date();
    controller.toDate.setDate(controller.toDate.getDate() -1);
    controller.fromDate = new Date();
    var ret = controller.readyFilterByDate();
    expect(ret).toBe(false);
    expect(controller.filterdates.todate.$error.fromDate).toBe(true);
  });

  it('should have an error when the FROM date is after the TO date', function(){
    controller.fromDate = new Date();
    controller.fromDate.setDate(controller.fromDate.getDate() +10);
    controller.toDate = new Date();
    var ret = controller.readyFilterByDate();
    expect(ret).toBe(false);
    expect(controller.filterdates.fromDate.$error.fromDateToLarge).toBe(true);
  }); 

  function setupFormErrors(){
    return {
      todate: { 
        $error : { 
          fromDate: false,
          date: false,
          required: false
        }
      },
      fromdate : {
        $error: {
          required: false, 
          date: false,
          fromDateToLarge: false
        }
      }
    };
   } 
});
