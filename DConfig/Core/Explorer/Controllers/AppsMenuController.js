angular.module('DConfig')
    .controller('AppsMenuController', ['$scope', '$location', 'BreadCrumpsProvider', 'UserMessagesProvider', 'ExplorerDataProvider', 'EventsProvider', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, ExplorerDataProvider, EventsProvider) {
        BreadCrumpsProvider.breadCrumps.path = ['Apps Menu'];
        UserMessagesProvider.displayLoading();
        ExplorerDataProvider.getApps().then(function (data) {
            if (data.data.result == "false") {
                UserMessagesProvider.errorHandler(999, data.message);
            } else {
                $scope.apps = data.data;
            }
            UserMessagesProvider.hideLoading();
        }, function (data, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });
    }]);