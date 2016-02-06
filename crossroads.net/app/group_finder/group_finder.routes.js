(function(){
  'use strict';

  module.exports = GroupFinderRoutes;

  GroupFinderRoutes.$inject = ['$stateProvider', '$urlRouterProvider'];

  function GroupFinderRoutes($stateProvider, $urlRouterProvider) {

    var seriesPermalink = 'brave';
    var seriesTitle = 'Brave';

    $stateProvider
      .state(seriesPermalink, {
        url: '/' + seriesPermalink,
        controller: 'GroupFinderCtrl as ctrl',
        templateUrl: 'common/layout.html',
        resolve: {},
        data: {
          meta: {
            title: seriesTitle,
            description: ''
          }
        }
      })

      .state(seriesPermalink + '.welcome', {
        controller: 'GroupFinderCtrl as ctrl',
        url: '/welcome',
        templateUrl: 'common/welcome.html',
        resolve: {},
        data: {
          meta: {
            title: seriesTitle,
            description: ''
          }
        }
      })

      .state(seriesPermalink + '.dashboard', {
        controller: 'DashboardCtrl as dashboard',
        url: '/host/dashboard',
        templateUrl: 'dashboard/dashboard.html',
        resolve: {},
        data: {
          meta: {
            title: seriesTitle,
            description: ''
          }
        }
      })

      .state(seriesPermalink + '.host', {
        controller: 'HostCtrl as host',
        url: '/host/:step',
        templateUrl: 'host/host.html',
        resolve: {
          QuestionService: require('./services/group_questions.service'),
          questions: function(QuestionService) {
            return QuestionService.get().$promise;
          }
        },
        data: {
          meta: {
            title: seriesTitle,
            description: ''
          }
        }
      })
    ;

    $urlRouterProvider.otherwise('/' + seriesPermalink + '/welcome');

  }

})();
