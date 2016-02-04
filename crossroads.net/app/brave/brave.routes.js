(function(){
  'use strict';

  module.exports = BraveRoutes;

  BraveRoutes.$inject = ['$stateProvider', '$urlRouterProvider'];

  function BraveRoutes($stateProvider, $urlRouterProvider) {

    $stateProvider
      .state('brave', {
        url: '/brave',
        controller: 'BraveCtrl as brave',
        templateUrl: 'templates/layout.html',
        resolve: {},
        data: {
          meta: {
            title: 'Brave',
            description: ''
          }
        }
      })

      .state('brave.login', {
        controller: 'LoginCtrl as login',
        url: '/login',
        templateUrl: 'templates/login.html',
        resolve: {},
        data: {
          meta: {
            title: 'Brave',
            description: ''
          }
        }
      })

      .state('brave.welcome', {
        controller: 'BraveCtrl as welcome',
        url: '/welcome',
        templateUrl: 'templates/welcome.html',
        resolve: {},
        data: {
          meta: {
            title: 'Brave',
            description: ''
          }
        }
      })

      .state('brave.join', {
        controller: 'JoinCtrl as join',
        url: '/join',
        templateUrl: 'templates/join.html',
        resolve: {},
        data: {
          meta: {
            title: 'Brave',
            description: ''
          }
        }
      })

      .state('brave.summary', {
        controller: 'SummaryCtrl as summary',
        url: '/summary',
        templateUrl: 'templates/summary.html',
        resolve: {},
        data: {
          meta: {
            title: 'Brave',
            description: ''
          }
        }
      })

      .state('brave.choice', {
        controller: 'ChoiceCtrl as choice',
        url: '/choice',
        templateUrl: 'templates/choice.html',
        resolve: {},
        data: {
          meta: {
            title: 'Brave',
            description: ''
          }
        }
      })

      .state('brave.dashboard', {
        controller: 'DashboardCtrl as dashboard',
        url: '/dashboard',
        templateUrl: 'templates/dashboard.html',
        resolve: {},
        data: {
          meta: {
            title: 'Brave',
            description: ''
          }
        }
      })

      .state('brave.host', {
        controller: 'HostCtrl as host',
        url: '/host/:step',
        templateUrl: 'templates/host.html',
        resolve: {
          questions: function($http){
            return $http.get('/app/brave/data/host.questions.json')
              .then(function(res){
                return res.data;
              });
          }
        },
        data: {
          meta: {
            title: 'Brave',
            description: ''
          }
        }
      })
    ;

    $urlRouterProvider.otherwise('/brave/login');
  }

})();
