"use strict";

(function () {

  require("./home/home.html");
  require('./home');
  require('./login/login_page.html');
  require('./register/register_form.html');
  require('./content');
  require('./opportunity');
  require('./community_groups_signup')
  require('./mytrips');
  require('./profile/profile.html');
  require('./profile/personal/profile_personal.html');
  require('./profile/profile_account.html');
  require('./profile/skills/profile_skills.html');
  require('./styleguide');
  require('./give');
  require('./myprofile');
  require('./opportunity/view_opportunities.html');
  require('./content/content.html');
  require('./community_groups_signup/group_signup_form.html');
  require('./my_serve');
  require('./go_trip_giving');
  var getCookie = require('./utilities/cookies');

  angular.module("crossroads").config(["$stateProvider", "$urlRouterProvider", "$httpProvider", "$urlMatcherFactoryProvider", "$locationProvider", function ($stateProvider, $urlRouterProvider, $httpProvider, $urlMatcherFactory, $locationProvider) {

        $httpProvider.defaults.useXDomain = true;
        $httpProvider.defaults.headers.common['Authorization'] = getCookie('sessionId');
        // This is a dummy header that will always be returned in any 'Allow-Header' from any CORS request. This needs to be here because of IE.
        $httpProvider.defaults.headers.common["X-Use-The-Force"] = true;

        // This custom type is needed to allow us to NOT URLEncode slashes when using ui-sref
        // See this post for details: https://github.com/angular-ui/ui-router/issues/1119
        var registerType = function(routeType, urlPattern) {
            return($urlMatcherFactory.type(routeType, {
                        encode: function(val) { return val != null ? val.toString() : val; },
                        decode: function(val) { return val != null ? val.toString() : val; },
                        is: function(val) { return this.pattern.test(val); },
                        pattern: urlPattern
                    }));
        };
        registerType("contentRouteType", /^\/.*/);
        registerType("signupRouteType", /\/sign-up\/.*$/);

        //================================================
        // Check if the user is connected
        //================================================
        var checkLoggedin = function ($q, $timeout, $http, $location, $rootScope) {
            console.log("CONFIG: checkLoggedIn");
            var deferred = $q.defer();
            $httpProvider.defaults.headers.common['Authorization'] = getCookie('sessionId');
            $http({
                method: 'GET',
                url: __API_ENDPOINT__ + "api/authenticated",
                headers: {
                    'Authorization': getCookie('sessionId')
                }
            }).success(function (user) {
                // Authenticated
                if (user.userId !== undefined) {
                    $timeout(deferred.resolve, 0);
                    $rootScope.userid = user.userId;
                    $rootScope.username = user.username;
                } else {
                    Session.clear();
                    $rootScope.message = "You need to log in.";
                    $timeout(function () {
                        deferred.reject();
                    }, 0);
                    $location.url("/");
                }
            }).error(function (e) {
                console.log(e);
                console.log("ERROR: trying to authenticate");
            });
            return deferred.promise;
        };

        $stateProvider
            .state("home", {
                url: "/",
                templateUrl: "home/home.html",
                controller: "HomeCtrl"
            })
            .state("homealso", {
                url: "/home",
                templateUrl: "home/home.html",
                controller: "HomeCtrl"
            })
            .state("login", {
                url: "/login",
                templateUrl: "login/login_page.html",
                controller: "LoginCtrl",
                data: {
                    isProtected: false
                }
            })
            .state("register", {
                url: "/register",
                templateUrl: "register/register_form.html",
                controller: "RegisterCtrl"
            })
            .state("profile", {
                url: "/profile",
                resolve: {
                    loggedin: checkLoggedin
                },
                data: {
                    isProtected: true
                },
                views: {
                    "": {
                        templateUrl: "profile/profile.html",
                        controller: "crdsProfileCtrl as profile",
                        resolve: {
                            loggedin: checkLoggedin
                        },
                    },
                    "personal@profile": {
                        templateUrl: "personal/profile_personal.html",
                        data: {
                            isProtected: true
                        },
                    },
                    "account@profile": {
                        templateUrl: "profile/profile_account.html",
                        data: {
                            isProtected: true
                        }
                    },
                    "skills@profile": {
                        controller: "ProfileSkillsController as profile",
                        templateUrl: "skills/profile_skills.html",
                        data: {
                            isProtected: true
                        }
                    }
                }
            })
            .state("myprofile", {
                url: "/myprofile",
                controller: "MyProfileCtrl as myProfile",
                templateUrl: "myprofile/myprofile.html",
                data: {
                    isProtected: true
                },
                resolve: {
                    loggedin: checkLoggedin
                }
            })
            .state("mytrips", {
              url: "/mytrips",
              templateUrl: "mytrips/mytrips.html"
            })
            .state("opportunities", {
                url: "/opportunities",
                controller: "ViewOpportunitiesController as opportunity",
                templateUrl: "opportunity/view_opportunities.html",
                data: {
                    isProtected: true
                },
                resolve: {
                  loggedin: checkLoggedin
                }
            })
            .state("serve-signup", {
              url: "/serve-signup",
              controller: "MyServeController as serve",
              templateUrl: "my_serve/myserve.html",
              data: { isProtected: true },
              resolve: { 
                ServeOpportunities: 'ServeOpportunities',
                Groups: function(ServeOpportunities){
                  return ServeOpportunities.ServeDays.query().$promise;
                },
                loggedin: checkLoggedin 
              }
            })
            .state("styleguide", {
                url: "/styleguide",
                controller: "StyleguideCtrl as styleguide",
                templateUrl: "styleguide/styleguide.html"
            })
            .state("give", {
                url: "/give",
                controller: "GiveCtrl as give",
                templateUrl: "give/give.html"
            })
           .state("give.amount", {
                 url: "/amount",
                 templateUrl: "give/amount.html"
           })
           .state("give.login", {
                 url: "/login",
                 controller: "LoginCtrl",
                 templateUrl: "give/login.html"
           })
           .state("give.register", {
                 url: "/register",
                 controller: "RegisterCtrl",
                 templateUrl: "give/register.html"
           })
           .state("give.account", {
                 url: "/account",
                 templateUrl: "give/account.html"
           })
           .state("give.thank-you", {
             url: "/thank-you",
             templateUrl: "give/thank_you.html"
           })
           //Not a child route of give because I did not want to use the parent give template
           .state("history", {
             url: "/give/history",
             templateUrl: "give/history.html"
           })
           .state("demo", {
             //abstract: true,
             url: '/demo',
             template: '<p>demo</p>'
           })
            .state("go_trip_giving", {
                url: "/go_trip_giving",
                controller: "GoTripGivingCtrl as gotripsearch",
                templateUrl: "go_trip_giving/go_trip_giving.html"
            })
            .state("go_trip_giving_results", {
                url: "/go_trip_giving_results",
                controller: "GoTripGivingCtrl as gotripresults",
                templateUrl: "go_trip_giving/go_trip_giving_results.html"
            })
            .state("/demo/guest-giver", {
                url: "/demo/guest-giver",
                templateUrl: "guest_giver/give.html"
            })
            .state("/demo/guest-giver/login", {
                url: "/demo/guest-giver/login",
                templateUrl: "guest_giver/give-login.html"
            })
            .state("/demo/guest-giver/login-guest", {
                url: "/demo/guest-giver/login-guest",
                controller: "GiveCtrl as give",
                templateUrl: "guest_giver/give-login-guest.html"
            })
            .state("/demo/guest-giver/give-confirmation", {
                url: "/demo/guest-giver/confirmation",
                templateUrl: "guest_giver/give-confirmation.html"
            })
            .state("/demo/guest-giver/give-register", {
                url: "/demo/guest-giver/register",
                templateUrl: "guest_giver/give-register.html"
            })
            .state("/demo/guest-giver/give-logged-in-bank-info", {
                url: "/demo/guest-giver/logged-in-bank-info",
                controller: "GiveCtrl as give",
                templateUrl: "guest_giver/give-logged-in-bank-info.html"
            })
            .state("/demo/guest-giver/give-confirm-amount", {
                url: "/demo/guest_giver/give-confirm-amount",
                templateUrl: "guest_giver/give-confirm-amount.html"
            })
            .state("/demo/guest-giver/give-change-information", {
                url: "/demo/guest_giver/give-change-information",
                controller: "GiveCtrl as give",
                templateUrl: "guest_giver/give-change-information.html"
            })
            .state("/demo/logged-in-giver/existing-giver", {
                url: "/demo/logged-in-giver/existing-giver",
                templateUrl: "guest_giver/give-logged-in.html"
            })
            .state("/demo/logged-in-giver/change-information", {
                url: "/demo/logged-in-giver/change-information",
                controller: "GiveCtrl as give",
                templateUrl: "guest_giver/give-change-information-logged-in.html"
            })
            .state("/demo/logged-in-giver/new-giver", {
                url: "/demo/logged-in-giver/new-giver",
                templateUrl: "guest_giver/give-logged-in-new-giver.html"
            })
            .state("/demo/go-trip-giving", {
                url: "/demo/go-trip-giving",
                templateUrl: "trip_giving/give.html"
            })
            .state("community-groups-signup", {
                url: "{link:signupRouteType}",
                controller: "GroupSignupController as groupsignup",
                templateUrl: "community_groups_signup/group_signup_form.html",
                data: {
                    isProtected: true
                },
                resolve: {
                    loggedin: checkLoggedin
                }
    })
    .state("tools", {
       abstract: true,
       url: '/mptools',
       templateUrl: 'mp_tools/tools.html',
       data: {
        hideMenu: true,
        isProtected: true
       },
       resolve: {
        loggedin: checkLoggedin
       }
    })
    .state("tools.su2s", {
      url: '/su2s',
      controller: 'SignupToServeController as su2s',
      templateUrl: 'signup_to_serve/su2s.html'
    })
    .state("content", {
    // This url will match a slash followed by anything (including additional slashes).
        url: "{link:contentRouteType}",
        controller: "ContentCtrl",
        templateUrl: "content/content.html"
            });
        //Leave the comment below.  Once we have a true 404 page hosted in the same domain, this is how we
        //will handle the routing.
        //.state("404", {
        //    templateUrl: __CMS_ENDPOINT__ + "/page-not-found/"
        //});

        $urlRouterProvider.otherwise("/");
                    }]);
})()
