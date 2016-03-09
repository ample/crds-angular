(function(){
  'use strict';

  module.exports = ResultsService;

  ResultsService.$inject = ['Group', 'Address', 'GROUP_API_CONSTANTS', 'Responses'];

  function ResultsService(Group, Address, GROUP_API_CONSTANTS, Responses) {
    var requestPromise = null;
    var results = {};
    var groups = [];

    results.responses = Responses;
    results.loadResults = loadResults;
    results.clearData = clearData;
    results.getResults = getResults;

    function loadResults() {
      if (!requestPromise) {
        if (_.has(results.responses.data, 'completed_flow') && results.responses.data.completed_flow === true) {

          var participant = {
            singleAttributes: Responses.getSingleAttributes(),
            attributeTypes: Responses.getMultiAttributes(['date_time_week', 'date_time_weekend'])
          };
          requestPromise = Group.Search.save({groupTypeId: GROUP_API_CONSTANTS.GROUP_TYPE_ID}, participant).$promise;
          requestPromise.then(function(results) {
            clearData();

            groups = results.slice(0,12);

            _.each(groups, function(group) {
              group.time = displayTime(group.meetingDayId, group.meetingTime);
              group.description = group.groupDescription;
              group.id = group.groupId;
              group.groupTitle = groupTitle(group.contactName);
              group.mapLink = Address.mapLink(group.address);

              if (_.has(group.singleAttributes, '73' ) ) {
                group.groupType = group.singleAttributes[73].attribute.description;
              }
              group.attributes = [];

              //
              // check attributes for pets and kids
              //
              _.each(group.attributeTypes, function(attribute) {
                if (attribute.name === 'Pets') {
                  _.each(attribute.attributes, function(type) {
                    if (type.selected && type.name) {
                      if (type.name.substr('dog')) { group.attributes.push('has a dog'); }
                      if (type.name.substr('cat')) { group.attributes.push('has a cat'); }
                    }
                  });
                }
                if (attribute.name === 'Kids') {
                  _.each(attribute.attributes, function(type) {
                    if (type.selected && type.name.substr('kid')) { group.attributes.push('kids welcome'); }
                  });
                }
              });
            });
            console.log(groups);

          });
        }
      }

      return requestPromise;
    }

    function displayTime(day, time) {
      var _time = time.split(':');
      var format = 'dddd[s] @ h:mm a';
      if (parseInt(_time[1]) === 0) {
        format = 'dddd[s] @ h a';
      }
      return moment().isoWeekday(day - 1).hour(_time[0]).minute(_time[1]).format(format);
    }

    function groupTitle(name) {
      var _name = name.split(', ');
      return _name[1] + ' ' +  _name[0][0] + '.';
    }

    function clearData() {
      groups = [];
    }

    function getResults() {
      return groups;
    }

    return results;
  }

})();
