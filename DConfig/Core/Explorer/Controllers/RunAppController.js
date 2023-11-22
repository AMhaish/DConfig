angular.module('DConfig')
    .controller('RunAppController', ['$scope', '$location', 'BreadCrumpsProvider', 'UserMessagesProvider', 'ExplorerDataProvider', 'EventsProvider', 'GlobalVariablesProvider', '$routeParams', '$location', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, ExplorerDataProvider, EventsProvider, GlobalVariablesProvider, $routeParams, $location) {
        BreadCrumpsProvider.breadCrumps.path = ['Running App'];
        UserMessagesProvider.displayLoading();
        var appName;
        if ($routeParams.appPath.contains('/'))
            appName = $routeParams.appPath.substring(0, $routeParams.appPath.indexOf('/'));
        else
            appName = $routeParams.appPath;
        if (GlobalVariablesProvider.get(appName) == "Loaded") {
            $location.path('#/notFound');
        } else {
            if (!$scope.startApp(appName, '#/' + $routeParams.appPath)) {
                $location.path('#/notFound');
            }
        }
    }]);