angular.module('DConfig').controllerProvider.register('ApplicationsManager.InstallAppController', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, ApplicationsManagerDataProvider, EventsProvider, $modal) {
    $scope.DetailsPanelStates = {
        None: 0,
        Info: 3
    };
    function initialize() {
        $scope.closeDetailsForm();
        UserMessagesProvider.displayLoading();
        ApplicationsManagerDataProvider.getApps().then(function (data) {
            $scope.apps = data.data;
            UserMessagesProvider.hideLoading();
        }, function (data, status, headers, config) {
            UserMessagesProvider.hideLoading();
        });
    }

    $scope.closeDetailsForm = function () {
        $scope.state = $scope.DetailsPanelStates.None;
    }

    $scope.displayDetails = function (index) {
        if ($scope.state == $scope.DetailsPanelStates.Info && $scope.selectedIndex != undefined && index == $scope.selectedIndex) {
            $scope.selectedIndex = undefined;
            $scope.closeDetailsForm();
        } else {
            $scope.selectedIndex = index;
            $scope.currentUser = jQuery.extend(true, {}, $scope.users[index]);
            $scope.state = $scope.DetailsPanelStates.Info;
            fetchExtendedInfo(index);
        }
    }

    $scope.uninstallApp = function (index) {

    }

    $scope.installNewApp = function () {

    }

    initialize();
});

var itemsSelectorController = function ($scope, $modalInstance, listOfItems) {
    $scope.items = listOfItems;
    $scope.container = {};
    $scope.ok = function () {
        $modalInstance.close($scope.container.obj);
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};