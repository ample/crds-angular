(function() {
  'use strict';
  module.exports = LoginForm;
  LoginForm.$inject = ['$log', 'AUTH_EVENTS', '$rootScope'];
  function LoginForm($log, AUTH_EVENTS, $rootScope) {
    return {
      restrict: 'EA',
      templateUrl: 'login/login_form.html',
      controller: 'LoginController',
      link: function(scope, elements, attrs) {
        var showForm = function() {
          $log.debug('not logged in');
          scope.visible = true;
        };

        if (attrs.prefix) {
          scope.passwordPrefix = attrs.prefix;
        }

        if (attrs.modal) {
          //Eval to avoid isolated scope, and previous dependencies on shared scope
          scope.modal = scope.$eval(attrs.modal);
          scope.credentials.username = $rootScope.email;
        }

        if (attrs.registerUrl) {
          scope.registerUrl = attrs.registerUrl;
          scope.showRegister = true;
        }

        $log.debug(scope.passwordPrefix);
        scope.visible = false;
      }
    };
  }
})();
