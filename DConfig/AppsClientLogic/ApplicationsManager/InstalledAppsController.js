angular.module('DConfig').controllerProvider.register('ApplicationsManager.InstalledAppsController', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, ApplicationsManagerDataProvider, EventsProvider, $modal, filterFilter) {
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
            $scope.currentObj = jQuery.extend(true, {}, filterFilter($scope.apps, $scope.searchText)[index]);
            $scope.state = $scope.DetailsPanelStates.Info;
        }
    }

    $scope.uninstallApp = function (index) {
        if (!filterFilter($scope.apps, $scope.searchText)[index].BuiltInApp) {
            UserMessagesProvider.confirmHandler("Are you sure you want to uninstall this app?", function () {
                UserMessagesProvider.displayLoading();
                ApplicationsManagerDataProvider.uninstallApp(filterFilter($scope.apps, $scope.searchText)[index]).then(function (data) {
                    if (result == "true") {
                        $scope.closeDetailsForm();
                        $scope.apps.splice(index, 1);
                    }
                    UserMessagesProvider.hideLoading();
                }, function (data, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                });
            }, null);
        } else {
            UserMessagesProvider.errorHandler(999, "Built in apps can't be uninstalled");
        }
    }


    initialize();
});