/*@ngInject*/
class ChildcareDashboardGroupController {
  constructor($rootScope, $scope, ChildcareDashboardService) {
    this.message = '';
    this.root = $rootScope;
    this.childcareService = ChildcareDashboardService;
    if(this.isEventClosed()) {
      this.message = $rootScope.MESSAGES.childcareEventClosed.content;
    }
    if(!this.hasEligibleChildren()) {
      this.message = $rootScope.MESSAGES.noEligibleChildren.content;
    }

    if (this.communityGroup !== undefined) {
      this.communityGroup.eligibleChildren.forEach( (child) => {
        $scope.$watch( () => child.rsvpness, (newval, oldval) => { 
          if(oldval !== newval) {
            this.rsvp(child, newval);
          }
        });
      });
    }
  }

  getCongregation(congregationId) {
    console.log(this.childcareService.congregations);
    const record = this.childcareService.congregations.filter((con) => {
      return con.dp_RecordID === congregationId;
    });

    if (record.length > 0) {
      return record[0].dp_RecordName;
    }
    return 'Unknown';
  }

  hasEligibleChildren() {
    if (this.communityGroup === undefined) {
      return false;
    }
    return this.communityGroup.eligibleChildren.length > 0;
  }

  hasSignedUpChild() {
    const rsvpd = this.communityGroup.eligibleChildren.filter( (child) => {
      return child.rsvpness;
    });
    return rsvpd.length > 0;
  }

  isEventCancelled() {
    if (this.cancelled !== undefined && this.cancelled) {
      this.message = this.root.MESSAGES.childcareEventCancelled.content;
      return true;
    }

    return false;
  }

  isEventClosed() {
    const today = moment();
    const otherDate = moment(this.eventDate);
    var diff = today.diff(otherDate, 'days');
    return diff >= -7;
  }

  rsvp(child, status) {

    var resp = this.childcareService.saveRSVP(child.contactId, this.communityGroup.childcareGroupId, status);
    resp.$promise.then(() => { 

    }, (err) => {
      child.rsvpness = !status;
      // display an error message...
      if (err.statusCode === 412) {
        this.root.$emit('notify', 'childcareRsvpFull');
      } else {
        this.root.$emit('notify', 'childcareRsvpError');
      }
    });
  }

  showMessage(){
    return this.message.length >0;
  }


}
export default ChildcareDashboardGroupController;

