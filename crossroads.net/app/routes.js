(function() {
  'use strict';

  module.exports = AppConfig;

  AppConfig.$inject = ['$stateProvider',
    '$urlRouterProvider',
    '$httpProvider',
    '$urlMatcherFactoryProvider',
    '$locationProvider'
  ];

  function AppConfig($stateProvider,
    $urlRouterProvider,
    $httpProvider,
    $urlMatcherFactory,
    $locationProvider
    ) {

    crds_utilities.preventRouteTypeUrlEncoding($urlMatcherFactory, 'contentRouteType', /^\/.*/);
    crds_utilities.preventRouteTypeUrlEncoding($urlMatcherFactory, 'signupRouteType', /\/sign-up\/.*$/);
    crds_utilities.preventRouteTypeUrlEncoding($urlMatcherFactory, 'volunteerRouteType', /\/volunteer-sign-up\/.*$/);
    crds_utilities.preventRouteTypeUrlEncoding($urlMatcherFactory, 'corkboardRouteType', /\/corkboard\/?.*$/);

    $stateProvider
      .state('root', {
        abstract: true,
        template: '<ui-view/>',
        resolve: {
          Meta: function(SystemPage, $state) {
            return SystemPage.get({
              state: $state.next.name
            }).$promise.then(
              function(systemPage) {
                if(systemPage.systemPages[0]){
                  if(!$state.next.data){
                    $state.next.data = {};
                  }
                  $state.next.data.meta = systemPage.systemPages[0];
                }
              });
          }
        }
      })
      .state('noSideBar', {
        parent: 'root',
        abstract:true,
        templateUrl: 'templates/noSideBar.html'
      })
      .state('leftSidebar', {
        parent: 'root',
        abstract:true,
        templateUrl: 'templates/leftSidebar.html'
      })
      .state('rightSidebar', {
        parent: 'root',
        abstract:true,
        templateUrl: 'templates/rightSidebar.html'
      })
      .state('screenWidth', {
        parent: 'root',
        abstract:true,
        templateUrl: 'templates/screenWidth.html'
      })
      .state('noHeaderOrFooter', {
        parent: 'root',
        abstract:true,
        templateUrl: 'templates/noHeaderOrFooter.html'
      })
      .state('giving_history', {
        parent: 'noSideBar',
        url: '/givinghistory',
        templateUrl: 'giving_history/history.html',
        controller: 'GivingHistoryController as giving_history_controller',
        data: {
          isProtected: true,
          meta: {
            title: 'Personal Giving History',
            description: ''
          }
        },
        resolve: {
          loggedin: crds_utilities.checkLoggedin
        }
      })
      .state('login', {
        parent: 'noSideBar',
        url: '/login',
        templateUrl: 'login/login_page.html',
        controller: 'LoginCtrl',
        data: {
          isProtected: false,
          meta: {
            title: 'Login',
            description: ''
          }
        }
      })
      .state('logout', {
        url: '/logout',
        controller: 'LogoutController',
        data: {
          isProtected: false,
          meta: {
            title: 'Logout',
            description: ''
          }
        }
      })
      .state('register', {
        parent: 'noSideBar',
        url: '/register',
        templateUrl: 'register/register_page.html',
        controller: 'RegisterCtrl',
        data: {
          meta: {
            title: 'Register',
            description: ''
          }
        }
      })
      .state('forgotPassword', {
        parent: 'noSideBar',
        url: '/forgot-password',
        templateUrl: 'login/forgot_password.html',
        controller: 'LoginCtrl',
        data: {
          isProtected: false
        }
      })
      .state('resetPassword', {
        parent: 'noSideBar',
        url: '/reset-password',
        templateUrl: 'login/reset_password.html',
        controller: 'LoginCtrl',
        data: {
          isProtected: false
        }
      })
        .state('profilePersonal', {
          parent: 'noSideBar',
          url: '/profile',
          templateUrl: 'common/profile/profilePersonal.template.html',
          controller: 'ProfilePersonalController as profilePersonalController',
          data: {
            isProtected: true,
            meta: {
              title: 'Profile',
              description: ''
            }
          },
          resolve: {
            loggedin: crds_utilities.checkLoggedin,
            $cookies: '$cookies',
            contactId: function($cookies) {
              return $cookies.get('userId');
            }

            //debugger;
            // leaving this in place as a reminder -- not sure we need to have a similar function in profile
            //pageId: function() {
            //  return 0;
            //}
          }
        })
    //    .state('profile') {
    //      parent: 'noSideBar',
    //          url: '/profile',
    //          templateUrl: 'common/profile/profilePersonal.template.html',
    //          controller: 'TripsSignupController as tripsSignup',
    //          resolve:
    //  {
    //    loggedin: crds_utilities.checkLoggedin
    //  },
    //  data: {
    //    isProtected: true,
    //        meta: {
    //          title: 'Profile',
    //              description: ''
    //    },
    //    views: {
    //
    //    }
    //  }
    //}
      //.state('profile', {
      //  parent: 'noSideBar',
      //  url: '/profile',
      //  resolve: {
      //    loggedin: crds_utilities.checkLoggedin
      //  },
      //  data: {
      //    isProtected: true,
      //    meta: {
      //      title: 'Profile',
      //      description: ''
      //    }
      //  },
      //  views: {
      //    '': {
      //      templateUrl: 'profile/profile.html',
      //      controller: 'crdsProfileCtrl as profile',
      //      resolve: {
      //        loggedin: crds_utilities.checkLoggedin
      //      },
      //    },
      //    'account@profile': {
      //      templateUrl: 'profile/profile_account.html',
      //      data: {
      //        isProtected: true
      //      }
      //    },
      //    'skills@profile': {
      //      controller: 'ProfileSkillsController as profile',
      //      templateUrl: 'skills/profile_skills.html',
      //      data: {
      //        isProtected: true
      //      }
      //    },
      //    'giving@profile': {
      //      controller: 'ProfileGivingController as giving_profile_controller',
      //      templateUrl: 'giving/profile_giving.html',
      //      data: {
      //        isProtected: true
      //      }
      //    }
      //  }
      //})
      .state('myprofile', {
        parent: 'noSideBar',
        url: '/myprofile',
        controller: 'MyProfileCtrl as myProfile',
        templateUrl: 'myprofile/myprofile.html',
        data: {
          meta: {
            title: 'Profile',
            description: ''
          }
        }
      })
      .state('explore', {
        parent: 'noHeaderOrFooter',
        url: '/explore',
        templateUrl: 'explore/explore.html',
        data: {
          meta: {
            title: 'Explore',
            description: ''
          }
        }
      })
      .state('adbox', {
        parent: 'noSideBar',
        url: '/adbox',
        controller: 'AdboxCtrl as adbox',
        templateUrl: 'adbox/adbox-index.html'
      })
      .state('serve-signup', {
        parent: 'noSideBar',
        url: '/serve-signup',
        controller: 'MyServeController as serve',
        templateUrl: 'my_serve/myserve.html',
        data: {
          isProtected: true,
          meta: {
            title: 'Signup to Serve',
            description: ''
          }
        },
        resolve: {
          loggedin: crds_utilities.checkLoggedin,
          ServeOpportunities: 'ServeOpportunities',
          $cookies: '$cookies',
          Groups: function(ServeOpportunities, $cookies) {
            return ServeOpportunities.ServeDays.query({
              id: $cookies.get('userId')
            }).$promise;
          }
        }
      })
      .state('styleguide', {
        parent: 'noHeaderOrFooter',
        url: '/styleguide',
        controller: 'StyleguideCtrl as styleguide',
        templateUrl: 'styleguide/styleguide.html'
      })
      .state('thedaily', {
        parent: 'noSideBar',
        url: '/thedaily',
        templateUrl: 'thedaily/thedaily.html'
      })
      .state('go_trip_giving_results', {
        parent: 'noSideBar',
        url: '/go_trip_giving_results',
        controller: 'TripGivingCtrl as gotripresults',
        templateUrl: 'tripgiving/tripgivingresults.html'
      })
      .state('/demo/go-trip-giving', {
        parent: 'noSideBar',
        url: '/demo/go-trip-giving',
        templateUrl: 'trip_giving/give.html'
      })
      .state('community-groups-signup', {
        parent: 'noSideBar',
        url: '{link:signupRouteType}',
        controller: 'GroupSignupController as groupsignup',
        templateUrl: 'community_groups_signup/group_signup_form.html',
        data: {
          isProtected: true,
          meta: {
            title: 'Community Group Signup',
            description: ''
          }
        },
        resolve: {
          loggedin: crds_utilities.checkLoggedin
        }
      })
      .state('volunteer-request', {
        parent: 'noSideBar',
        url: '{link:volunteerRouteType}',
        controller: 'VolunteerController as volunteer',
        templateUrl: 'volunteer_signup/volunteer_signup_form.html',
        data: {
          isProtected: true,
          meta: {
            title: 'Volunteer Signup',
            description: ''
          }
        },
        resolve: {
          loggedin: crds_utilities.checkLoggedin,
          Page: 'Page',
          CmsInfo: function(Page, $stateParams) {
            return Page.get({
              url: $stateParams.link
            }).$promise;
          }
        }
      })
      .state('volunteer-application', {
        parent: 'noSideBar',
        url: '/volunteer-application/:appType/:id',
        controller: 'VolunteerApplicationController as volunteer',
        templateUrl: 'volunteer_application/volunteerApplicationForm.html',
        data: {
          isProtected: true,
          meta: {
            title: 'Volunteer Signup',
            description: ''
          }
        },
        resolve: {
          loggedin: crds_utilities.checkLoggedin,
          Page: 'Page',
          PageInfo: function($q, Profile, Page, $stateParams) {
            var deferred = $q.defer();
            var contactId = $stateParams.id;

            Profile.Person.get({
              contactId: contactId
            }).$promise.then(
              function(contact) {
                var age = contact.age;
                var cmsPath = '/kids-club-applicant-form/adult-applicant-form/';
                if ((age >= 10) && (age <= 15)) {
                  cmsPath = '/kids-club-applicant-form/student-applicant-form/';
                }

                Page.get({
                    url: cmsPath
                  }).$promise.then(function(cmsInfo) {
                      deferred.resolve({
                        contact, cmsInfo
                      });
                    }

                  );
              });

            return deferred.promise;
          },

          Volunteer: 'VolunteerService',
          Family: function(Volunteer) {
            return Volunteer.Family.query({
              contactId: crds_utilities.getCookie('userId')
            }).$promise;
          }
        }
      })
      .state('corkboard', {
        url: '{link:corkboardRouteType}',
        resolve: {
          RedirectToSubSite: function($window, $location) {
            // Force browser to do a full reload to load corkboard's index.html
            $window.location.href = $location.path();
          }
        },
        data: {
          preventRouteAuthentication: true,
          meta: {
            title: 'Corkboard',
            description: ''
          }
        }
      })
      .state('tools', {
        parent: 'noSideBar',
        abstract: true,
        url: '/mptools',
        templateUrl: 'mp_tools/tools.html',
        data: {
          hideMenu: true,
          isProtected: true,
          meta: {
            title: 'Tools',
            description: ''
          }
        },
        resolve: {
          loggedin: crds_utilities.checkLoggedin
        }
      })
      .state('tools.su2s', {
        url: '/su2s',
        controller: 'SignupToServeController as su2s',
        templateUrl: 'signup_to_serve/su2s.html'
      })
      .state('tools.kcApplicant', {
        url: '/kcapplicant',
        controller: 'KCApplicantController as applicant',
        templateUrl: 'kc_applicant/applicant.html',
        data: {
          isProtected: true,
          meta: {
            title: 'Kids Club Application',
            description: ''
          }
        },
        resolve: {
          loggedin: crds_utilities.checkLoggedin,
          Profile: 'Profile',
          MPTools: 'MPTools',
          Contact: function(Profile, MPTools) {
            var params = MPTools.getParams();
            return Profile.Person.get({
              contactId: params.recordId
            }).$promise;
          },

          Page: 'Page',
          CmsInfo: function(Page, $stateParams) {
            return Page.get({
              url: '/volunteer-application/kids-club/'
            }).$promise;
          }
        }
      })
      .state('tools.tripParticipants', {
        url: '/tripParticipants',
        controller: 'TripParticipantController as trip',
        templateUrl: 'trip_participants/trip.html',
        resolve: {
          MPTools: 'MPTools',
          Trip: 'Trip',
          PageInfo: function(MPTools, Trip) {
            var params = MPTools.getParams();
            return Trip.TripFormResponses.get({
              selectionId: params.selectedRecord,
              selectionCount: params.selectedCount,
              recordId: params.recordId
            }).$promise.then(function(data) {
                  // promise fulfilled
                  return data;
                }, function(error) {
                  // promise rejected, could log the error with: console.log('error', error);
                  var data = {};
                  data.errors = error;
                  return error;
                });
          }
        }
      })
      .state('tools.tripPrivateInvite', {
        url: '/tripPrivateInvite',
        controller: 'TripPrivateInviteController as invite',
        templateUrl: 'trip_private_invite/invite.html',
        resolve: {
          MPTools: 'MPTools',
          Trip: 'Trip'
        }
      })
      .state('tools.checkBatchProcessor', {
        url: '/checkBatchProcessor',
        controller: 'CheckBatchProcessor as checkBatchProcessor',
        templateUrl: 'check_batch_processor/checkBatchProcessor.html',
        data: {
          isProtected: true,
          meta: {
            title: 'Check Batch Processor',
            description: ''
          }
        }
      })
      .state('tools.adminGivingHistoryTool', {
        // This is a "launch" page for the tool, it will check access, etc, then forward
        // on to the actual page with the history.
        url: '/adminGivingHistoryTool',
        controller: 'AdminGivingHistoryController as AdminGivingHistory',
        templateUrl: 'admin_giving_history/adminGivingHistoryTool.html'
      })
      .state('tools.adminGivingHistory', {
        url: '/adminGivingHistory',
        controller: 'GivingHistoryController as admin_giving_history_controller',
        templateUrl: 'admin_giving_history/adminGivingHistory.html',
        data: {
          isProtected: true,
          meta: {
            title: 'Giving History - Admin View',
            description: ''
          }
        }
      })
      .state('tools.gpExport', {
        url: '/gpExport',
        controller: 'GPExportController as gpExport',
        templateUrl: 'gp_export/gpExport.html'
      })
      .state('content', {
        // This url will match a slash followed by anything (including additional slashes).
        url: '{link:contentRouteType}',
        views: {
          '': {
            controller: 'ContentCtrl',
            templateProvider: function($rootScope,
              $templateFactory,
              $stateParams,
              Page,
              ContentPageService) {
              var promise;

              var link = addTrailingSlashIfNecessary($stateParams.link);
              promise = Page.get({ url: link }).$promise;

              var childPromise = promise.then(function(originalPromise) {
                if (originalPromise.pages.length > 0) {
                  ContentPageService.page = originalPromise.pages[0];
                  return originalPromise;
                }

                var notFoundPromise = Page.get({url: '/page-not-found/'}).$promise;

                notFoundPromise.then(function(promise) {
                  if (promise.pages.length > 0) {
                    ContentPageService.page = promise.pages[0];
                  } else {
                    ContentPageService.page = {
                      content: '404 Content not found',
                      pageType: '',
                      title: 'Page not found'
                    };
                  }
                });

                return notFoundPromise;
              });

              return childPromise.then(function() {
                var metaDescription = ContentPageService.page.metaDescription;
                if (!metaDescription){
                  //If a meta description is not provided we'll use the Content
                  //The description gets html stripped and shortened to 155 characters
                  metaDescription = ContentPageService.page.content;
                }
                $rootScope.meta = {
                  title: ContentPageService.page.title,
                  description: metaDescription,
                  card: ContentPageService.page.card,
                  type: ContentPageService.page.type,
                  image: ContentPageService.page.image,
                  statusCode: ContentPageService.page.errorCode
                };

                switch (ContentPageService.page.pageType){
                  case 'NoHeaderOrFooter':
                    return $templateFactory.fromUrl('templates/noHeaderOrFooter.html');
                  case 'LeftSidebar':
                    return $templateFactory.fromUrl('templates/leftSideBar.html');
                  case 'RightSidebar':
                    return $templateFactory.fromUrl('templates/rightSideBar.html');
                  case 'ScreenWidth':
                    return $templateFactory.fromUrl('templates/screenWidth.html');
                  default:
                    return $templateFactory.fromUrl('templates/noSideBar.html');
                }
              });
            }
          },
          '@content': {
            templateUrl: 'content/content.html',
          },
          'sidebar@content': {
            templateUrl: 'content/sidebarContent.html'
          }
        }
      });

    //Leave the comment below.  Once we have a true 404 page hosted in the same domain, this is how we
    //will handle the routing.
    //.state('404', {
    //    templateUrl: __CMS_ENDPOINT__ + '/page-not-found/'
    //});

    $urlRouterProvider.otherwise('/');
  }

  function addTrailingSlashIfNecessary(link) {
    if (_.endsWith(link, '/') === false) {
      return link + '/';
    }

    return link;
  }

})();
