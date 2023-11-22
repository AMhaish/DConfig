angular.module('DConfig', ['DConfigSharedLib']);

angular.module('DConfig')
    .config(['$routeProvider', '$httpProvider', function ($routeProvider, $httpProvider) {
       //Setup routes to load partial templates from server. TemplateUrl is the location for the server view (Razor .cshtml view)
        $routeProvider
            .when('/', {controller: 'LoginController',templateUrl: '/DConfig/Account/Login'})
            .when('/Login', { controller: 'LoginController', templateUrl: '/DConfig/Account/Login' })
            .when('/Register', { controller: 'RegisterController', templateUrl: '/DConfig/Account/Register' })
            .when('/Forgotten', { controller: 'ForgottenController', templateUrl: '/DConfig/Account/Forgotten' })
            .when('/Resetpassword', { controller: 'ResetPasswordController', templateUrl: '/DConfig/Account/Resetpassword' })
            .otherwise({ redirectTo: '/Login' });
        if (!$httpProvider.defaults.headers.get) {
            $httpProvider.defaults.headers.get = {};
        }
        $httpProvider.defaults.headers.common["Ajax"] = 'true';
        $httpProvider.interceptors.push('AuthHttpInterceptor');
    }])
    .controller('RootController', ['$scope', '$route', '$routeParams', '$location', 'EventsProvider', 'UserMessagesProvider', function ($scope, $route, $routeParams, $location, EventsProvider, UserMessagesProvider) {
        $(document).ajaxError(function (e, xhr) {
            if (xhr.status == 401){
                window.location = "/DConfig/Account/Index";
            }
            else if (xhr.status == 403){
                alert("You have no enough permissions to request this resource.");
            }
        });
        $scope.$on('$routeChangeSuccess', function (e, current, previous) {
            //$scope.activeViewPath = $location.path();
            //$scope.breadCrumps = BreadCrumpsProvider.breadCrumps;
            UserMessagesProvider.hideLoading();
            EventsProvider.ExecuteOnRouteChangeSuccessHandlers();
        });
        $scope.$on('$routeChangeStart', function (e, next, current) {
            UserMessagesProvider.displayLoading();
            EventsProvider.ExecuteOnRouteChangeStartHandlers();
        });
    }]);


