class RequestChildcareController {
  /*@ngInject*/
  constructor($rootScope, MPTools, CRDS_TOOLS_CONSTANTS, $log, RequestChildcareService) {
    this.allowAccess = MPTools.allowAccess(CRDS_TOOLS_CONSTANTS.SECURITY_ROLES.ChildcareRequestTool);
    this.viewReady = false;
    this.name = 'request-childcare';
    this._$rootScope = $rootScope;
    this._MPTools = MPTools;
    this._CRDS_TOOLS_CONSTANTS = CRDS_TOOLS_CONSTANTS;
    this._$log = $log;
    this.params = MPTools.getParams();
    this.congregations = RequestChildcareService.getCongregations();
    this.currentRequest = Number(this.params.recordId);
    this.viewReady = true;
  }

  getConstants() {
    return this._CRDS_TOOLS_CONSTANTS;
  }

  submit() {
    this._$log.debug('submit form');
  }

}
export default RequestChildcareController;
