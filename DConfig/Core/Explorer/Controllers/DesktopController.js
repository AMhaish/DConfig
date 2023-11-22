angular.module('DConfig')
    .controller('DesktopController', ['$scope', '$location', 'BreadCrumpsProvider', 'UserMessagesProvider', 'ExplorerDataProvider', 'EventsProvider', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, ExplorerDataProvider, EventsProvider) {
        BreadCrumpsProvider.breadCrumps.path = ['Dashboard'];
        UserMessagesProvider.displayLoading();
        ExplorerDataProvider.getDesktopWidgets().then(function (data) {
            $scope.desktopWidgets = data;
            UserMessagesProvider.hideLoading();
        }, function (data, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });
        //Bootstraping Widgets
        //angular.bootstrap();
    }]);