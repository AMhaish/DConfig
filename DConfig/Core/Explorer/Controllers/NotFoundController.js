angular.module('DConfig')
    .controller('NotFoundController', ['$scope', '$location', 'BreadCrumpsProvider', 'UserMessagesProvider', 'ExplorerDataProvider', 'EventsProvider', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, ExplorerDataProvider, EventsProvider) {
        BreadCrumpsProvider.breadCrumps.path = ['Not Found'];
    }]);