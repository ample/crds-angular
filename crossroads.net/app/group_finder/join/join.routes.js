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
          AuthenticatedPerson: ['Person', function(Person) {
            return Person.getProfile();
          }],
          QuestionDefinitions: function(ParticipantQuestionService) {
            return ParticipantQuestionService.getQuestions();
          },
          LookupDefinitions: function(ParticipantQuestionService) {
            return ParticipantQuestionService;
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
          LoadResults: ['Results', function(Results) {
            return Results.loadResults();
          }]
        },
        data: {meta: {title: SERIES.title,description: ''}}
      })
      ;

  }

})();
