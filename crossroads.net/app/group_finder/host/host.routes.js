(function(){
  'use strict';

  module.exports = HostRoutes;

  HostRoutes.$inject = ['$stateProvider', 'SERIES'];

  function HostRoutes($stateProvider, SERIES) {

    $stateProvider
      .state('group_finder.host', {
        controller: 'HostCtrl as host',
        url: '/host',
        templateUrl: 'host/host.html',
        resolve: {
          AuthenticatedPerson: ['Person', function(Person) {
            return Person.getProfile();
          }],
          QuestionDefinitions: function(GroupQuestionService) {
            return GroupQuestionService.get().$promise;
          }
        },
        data: {
          isProtected: true,
          meta: { title: SERIES.title,description: '' }
        }
      })
      .state('group_finder.host.questions', {
        controller: 'HostQuestionsCtrl as host',
        url: '/questions',
        templateUrl: 'host/host_questions.html',
        data: {meta: {title: SERIES.title,description: ''}}
      })
      .state('group_finder.host.review', {
        controller: 'HostReviewCtrl as host',
        url: '/review',
        templateUrl: 'host/host_review.html',
        data: {meta: {title: SERIES.title,description: ''}}
      })
      .state('group_finder.host.confirm', {
        controller: 'HostConfirmCtrl as host',
        url: '/success',
        templateUrl: 'host/host_confirm.html',
        data: {meta: {title: SERIES.title,description: ''}}
      })
      ;

  }

})();
