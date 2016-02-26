(function(){
  'use strict';

  module.exports = GroupQuestionsService;

  GroupQuestionsService.$inject = ['Page', 'AUTH_EVENTS', '$rootScope'];

  function GroupQuestionsService(Page, AUTH_EVENTS, $rootScope) {
    var promise = null;
    var service = {};
    service.questions = [];
    service.loadQuestions = loadQuestions;
    service.getQuestions = getQuestions;

    $rootScope.$on(AUTH_EVENTS.logoutSuccess, clearData);

    function loadQuestions() {
      if (!promise) {
        promise = Page.get({url: '/bravehost/'}).$promise;
        promise.then(function(data) {
          service.questions = _.each(data.pages[0].fields, function(question) {
            question.key = question.name;
            delete question.name;

            switch (question.className) {
              case 'EditableAddressField': question.input_type = 'address'; break;
              case 'EditableCheckboxGroupField': question.input_type = 'checkbox'; break;
              case 'EditableDatetimeField': question.input_type = 'date_and_time'; break;
              case 'EditableNumericField': question.input_type = 'number'; break;
              case 'EditableRadioField': question.input_type = 'radio'; break;
              case 'EditableTextField': question.input_type = 'textarea'; break;
            }

            if (_.has(question, 'attributeType') && question.attributeType) {
              question.answers = _.map(question.attributeType.attributes, function(attribute){
                return { id: attribute.attributeId, name: attribute.name };
              });
            }
          });
        });
      }

      return promise;
    }

    function getQuestions() {
      var loadPromise = loadQuestions();
      return loadPromise.then(function() {
        return service.questions;
      });
    }

    function clearData() {
      promise = null;
      delete service.questions;
    }

    return service;
  }

})();
