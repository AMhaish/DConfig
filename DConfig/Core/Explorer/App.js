angular.module('DConfig', ['DConfigSharedLib']);

angular.module('DConfig')
    .config(['$routeProvider', '$controllerProvider', '$compileProvider', '$filterProvider', '$provide', '$httpProvider', function ($routeProvider, $controllerProvider, $compileProvider, $filterProvider, $provide, $httpProvider) {
        function resolveRoute(params, path, search) {
            if (path.contains('/runApp'))
                return '/notFound';
            else
                return '/runApp' + path;
        }
        //Setup routes to load partial templates from server. TemplateUrl is the location for the server view (Razor .cshtml view)
        $routeProvider
            .when('/', { controller: 'AppsMenuController', templateUrl: '/DConfig/Explorer/AppsMenu' })
            .when('/notFound', { controller: 'NotFoundController', templateUrl: '/DConfig/Explorer/NotFound' })
            .when('/runApp/:appPath*', { controller: 'RunAppController', templateUrl: '/DConfig/Explorer/RunApp' })
            .when('/Desktop', { controller: 'DesktopController', templateUrl: '/DConfig/Explorer/Desktop' })
            .when('/AppsMenu', { controller: 'AppsMenuController', templateUrl: '/DConfig/Explorer/AppsMenu' })
            .when('/Settings', { controller: 'SettingsController', templateUrl: '/DConfig/Explorer/Settings' })
            .otherwise({ redirectTo: resolveRoute });
        angular.module('DConfig').controllerProvider = $controllerProvider;
        angular.module('DConfig').compileProvider = $compileProvider;
        angular.module('DConfig').routeProvider = $routeProvider;
        angular.module('DConfig').filterProvider = $filterProvider;
        angular.module('DConfig').provide = $provide;
        angular.module('DConfig').httpProvider = $httpProvider;
        if (!$httpProvider.defaults.headers.get) {
            $httpProvider.defaults.headers.get = {};
        }
        $httpProvider.defaults.headers.common["Ajax"] = 'true';
        $httpProvider.interceptors.push('AuthHttpInterceptor');
        //disable IE ajax request caching
        $httpProvider.defaults.headers.get['If-Modified-Since'] = 'Mon, 26 Jul 1997 05:00:00 GMT';
        // extra
        $httpProvider.defaults.headers.get['Cache-Control'] = 'no-cache';
        $httpProvider.defaults.headers.get['Pragma'] = 'no-cache';

    }])
    .controller('RootController', ['$scope', '$route', '$routeParams', '$location', 'BreadCrumpsProvider', 'EventsProvider', 'UserMessagesProvider', 'GlobalVariablesProvider', '$timeout', 'ExplorerDataProvider', '$cookies', function ($scope, $route, $routeParams, $location, BreadCrumpsProvider, EventsProvider, UserMessagesProvider, GlobalVariablesProvider, $timeout, ExplorerDataProvider, $cookies) {

        function initialize() {
            UserMessagesProvider.displayLoading();
            angular.module('DConfig').httpProvider.defaults.headers.common["ContextId"] = $cookies.get("ContextId");
            $.jstree.defaults.search.show_only_matches = true;
            UserMessagesProvider.setProgressBarUpdateFunction(function (value) {
                $scope._progressValue = value;
            });
            $('#breadcrumb').show();
            $scope.$on('$routeChangeSuccess', function (e, current, previous) {
                $scope._CurrentPath = $location.path();
                UserMessagesProvider.hideLoading();
                $scope.breadCrumps = BreadCrumpsProvider.breadCrumps;
                EventsProvider.ExecuteOnRouteChangeSuccessHandlers();
            });
            $scope.$on('$viewContentLoaded', function () {
                $timeout(function () {
                    var appMenuContainer = $('#appMenuContainer');
                    var appMenu = $('#appMenu');
                    if (appMenuContainer.length > 0) {
                        appMenu.html(appMenuContainer.html());
                    } else {
                        appMenu.html('');
                    }
                    window.materialadmin.Demo.initialize();
                    window.materialadmin.Demo.initializePanelsHeights();
                }, 2);
            });
            $scope.$on('$routeChangeStart', function (e, next, current) {
                UserMessagesProvider.displayLoading();
                BreadCrumpsProvider.breadCrumps.path = [];
            });
            $scope.$on('$locationChangeStart', function (scope, next, current) {
                if (!EventsProvider.ExecuteOnRouteChangeStartHandlers()) {
                    scope.preventDefault();
                }
            });
            ExplorerDataProvider.getUserCompanies().then(function (data) {
                $scope._companies = data.data;
                UserMessagesProvider.hideLoading();
            }, function (data, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        }

        $scope.setCurrentContext = function (id) {
            UserMessagesProvider.displayLoading();
            ExplorerDataProvider.setCurrentContext(id).then(function (data) {
                if (data.data.result === 'true') {
                    $cookies.put("ContextId", id);
                    angular.module('DConfig').httpProvider.defaults.headers.common["ContextId"] = id;
                    UserMessagesProvider.successHandler('Account switched successfully');
                    for (i = 0; i < $scope._companies.length; i++) {
                        if ($scope._companies[i].Id === id) {
                            $scope._companies[i].Current = true;
                        } else {
                            $scope._companies[i].Current = false;
                        }
                    }
                    $location.path('/');
                } else {
                    UserMessagesProvider.errorHandler(data.data.message);
                }
                UserMessagesProvider.hideLoading();
            }, function (data, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        }

        $scope.startApp = function (app_name, apppath) {
            var appName = app_name;
            var startPath = apppath;
            UserMessagesProvider.displayLoading();
            if (startPath.startWith('#')) {
                startPath = startPath.substring(1, apppath.length);
                if (!GlobalVariablesProvider.cached("/DConfig/AppsAPI/appstyles/" + appName)) {
                    GlobalVariablesProvider.getFromUrl("/DConfig/AppsAPI/appstyles/" + appName,
                        function (pathObj) {
                            $("<link/>", {
                                rel: "stylesheet",
                                type: "text/css",
                                href: pathObj.data
                            }).appendTo("head");
                        },
                        function () {
                            UserMessagesProvider.errorHandler(800);
                        }
                    );
                }
                if (!GlobalVariablesProvider.cached("/DConfig/AppsAPI/appscripts/" + appName)) {
                    GlobalVariablesProvider.getFromUrl("/DConfig/AppsAPI/appscripts/" + appName,
                        function (pathObj) {
                            $.getScript(pathObj.data, function () {
                                $scope.$apply(function () {
                                    $location.path(startPath);
                                });
                            }).fail(function () {
                                if (arguments[0].readyState === 0) {
                                    //script failed to load
                                    UserMessagesProvider.errorHandler(801);
                                    return false;
                                } else {
                                    //script loaded but failed to parse
                                    UserMessagesProvider.errorHandler(802);
                                    return false;
                                }
                            });
                        },
                        function () {
                            UserMessagesProvider.errorHandler(800);
                            return false;
                        }
                    );
                } else {
                    $location.path(startPath);
                }
            } else {
                window.location.assign(startPath);
            }
            GlobalVariablesProvider.set(appName, "Loaded");
            return true;
        }

        initialize();
    }]);
