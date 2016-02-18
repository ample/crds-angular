(function(){
  'use strict';

  module.exports = JoinRoutes;

  JoinRoutes.$inject = ['$stateProvider', 'SERIES'];

  function JoinRoutes($stateProvider, SERIES) {

    $stateProvider
      .state('group_finder.join', {
        controller: 'JoinCtrl as join',
        url: '/join',
        templateUrl: 'join/join.html',
        resolve: {
          ParticipantQuestionService: 'ParticipantQuestionService',
          QuestionDefinitions: function(ParticipantQuestionService) {
            return ParticipantQuestionService.get().$promise;
          }
        },
        data: {
          isProtected: true,
          meta: { title: SERIES.title, description: '' }
        }
      })
      .state('group_finder.join.questions', {
        controller: 'JoinQuestionsCtrl as join',
        url: '/questions',
        templateUrl: 'join/join_questions.html',
        data: {meta: {title: SERIES.title,description: ''}}
      })
      .state('group_finder.join.review', {
        controller: 'JoinReviewCtrl as join',
        url: '/review',
        templateUrl: 'join/join_review.html',
        data: {meta: {title: SERIES.title,description: ''}}
      })
      .state('group_finder.join.results', {
        controller: 'JoinResultsCtrl as result',
        url: '/results',
        templateUrl: 'join/join_results.html',
        resolve: {
          Results: 'Results'
        },
        data: {meta: {title: SERIES.title,description: ''}}
      })
      .state('group_finder.join.contact', {
        controller: 'JoinContactCtrl as results',
        url: '/contact',
        templateUrl: 'join/join_contact.html',
        resolve: {},
        data: {meta: {title: SERIES.title,description: ''}}
      })
      ;

  }

})();
