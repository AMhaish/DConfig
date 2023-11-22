angular.module('DConfig').controllerProvider.register('Membership.AppsPriviligesController', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, MembershipDataProvider, EventsProvider, $modal, filterFilter) {
    $scope.DetailsPanelStates = {
        None: 0,
        Priviliges: 1
    };
    function initialize() {
        BreadCrumpsProvider.breadCrumps.path = ['Membership Manager', 'Apps Priviliges'];
        $scope.closeDetailsForm();
        UserMessagesProvider.displayLoading();
        MembershipDataProvider.getAllApps().then(function (data) {
            $scope.apps = data.data;
            UserMessagesProvider.hideLoading();
        }, function (data, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });
        MembershipDataProvider.getRoles().then(function (data) {
            $scope.roles = data.data;
            UserMessagesProvider.hideLoading();
        }, function (data, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });
    }

    $scope.closeDetailsForm = function () {
        $scope.state = $scope.DetailsPanelStates.None;
    }

    $scope.displayDetails = function (index) {
        if ($scope.state == $scope.DetailsPanelStates.Priviliges && $scope.selectedIndex != undefined && index == $scope.selectedIndex) {
            $scope.selectedIndex = undefined;
            $scope.closeDetailsForm();
        } else {
            $scope.selectedIndex = index;
            $scope.currentApp = jQuery.extend(true, {}, filterFilter($scope.apps, $scope.searchText)[index]);
            $scope.state = $scope.DetailsPanelStates.Priviliges;
            $scope.currentApp.roles = jQuery.extend(true, [], $scope.roles);
            if ($scope.currentApp.roles.length > 0) {
                for (j = 0; j < $scope.currentApp.roles.length; j++) {
                    for (i = 0; i < $scope.currentApp.AppRoles.length; i++) {
                        if ($scope.currentApp.roles[j].Id == $scope.currentApp.AppRoles[i].Id) {
                            $scope.currentApp.roles[j].Selected = true;
                        }
                    }
                }
            }
        }
    }

    $scope.savePrivileges = function () {
        var ids = [];
        for (i = 0; i < $scope.currentApp.roles.length; i++) {
            if ($scope.currentApp.roles[i].Selected) {
                ids.push($scope.currentApp.roles[i].Id);
            }
        }
        UserMessagesProvider.displayLoading();
        MembershipDataProvider.updateAppRoles($scope.currentApp.Name, { RolesIds: ids }).then(function (returnValue) {
            UserMessagesProvider.hideLoading();
            if (returnValue.data.result == 'true') {
                UserMessagesProvider.successHandler();
                var app = filterFilter($scope.apps, $scope.searchText)[$scope.selectedIndex];
                for (i = 0; i < $scope.currentApp.roles.length; i++) {
                    if ($scope.currentApp.roles[i].Selected) {
                        app.AppRoles.push($scope.currentApp.roles[i]);
                    }
                }
            }
            else {
                UserMessagesProvider.errorHandler(999, returnValue.data.message);
            }
        }, function (errorData, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });
    }

    initialize();
});
