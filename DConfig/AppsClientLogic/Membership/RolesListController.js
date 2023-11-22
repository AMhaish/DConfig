angular.module('DConfig').controllerProvider.register('Membership.RolesListController', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, MembershipDataProvider, EventsProvider, $modal, filterFilter) {
    $scope.DetailsPanelStates = {
        None: 0,
        Edit: 1,
        Create: 2,
        Info: 3
    };
    function initialize() {
        BreadCrumpsProvider.breadCrumps.path = ['Membership Manager', 'Roles'];
        $scope.closeDetailsForm();
        UserMessagesProvider.displayLoading();
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
        if ($scope.state == $scope.DetailsPanelStates.Info && $scope.selectedIndex != undefined && index == $scope.selectedIndex) {
            $scope.selectedIndex = undefined;
            $scope.closeDetailsForm();
        } else {
            $scope.selectedIndex = index;
            $scope.currentRole = jQuery.extend(true, {}, filterFilter($scope.roles, $scope.searchText)[index]);
            $scope.state = $scope.DetailsPanelStates.Info;
        }
    }

    $scope.displayEditForm = function (index) {
        if ($scope.state == $scope.DetailsPanelStates.Edit && $scope.selectedIndex != undefined && index == $scope.selectedIndex) {
            $scope.closeDetailsForm();
            $scope.selectedIndex = undefined;
        } else {
            $scope.selectedIndex = index;
            $scope.currentRole = jQuery.extend(true, {}, filterFilter($scope.roles, $scope.searchText)[index]);
            $scope.currentRole.NewName = $scope.currentRole.Name;
            $scope.state = $scope.DetailsPanelStates.Edit;
        }
    }

    $scope.displayCreateForm = function () {
        $scope.closeDetailsForm();
        if ($scope.state != $scope.DetailsPanelStates.Create) {
            $scope.currentRole = {};
            $scope.state = $scope.DetailsPanelStates.Create;
        }
    }

    $scope.deleteRole = function (index) {
        $scope.selectedIndex = index;
        $scope.currentRole = filterFilter($scope.roles, $scope.searchText)[index];
        UserMessagesProvider.confirmHandler("Are you sure you want to delete this role?", function () {
            UserMessagesProvider.displayLoading();
            MembershipDataProvider.deleteRole($scope.currentRole).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    $scope.closeDetailsForm();
                    $scope.roles.splice(index, 1);
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.message);
                }
            }, function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(999);
            });
        }, null);
    }

    $scope.createRole = function () {
        if ($scope.createForm.$valid) {
            UserMessagesProvider.displayLoading();
            MembershipDataProvider.createRole($scope.currentRole).then(function (data) {
                if (data.data.result == 'true') {
                    $scope.roles.push($scope.currentRole);
                    $scope.closeDetailsForm();
                }
                else {
                    UserMessagesProvider.errorHandler(999, data.message);
                }
                UserMessagesProvider.hideLoading();
            }, function (data, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        } else {
            UserMessagesProvider.invalidHandler();
        }
    }

    $scope.editRole = function () {
        if ($scope.editForm.$valid) {
            UserMessagesProvider.displayLoading();
            MembershipDataProvider.updateRole($scope.currentRole).then(function (data) {
                if (data.data.result == 'true') {
                    $scope.currentRole.Name = $scope.currentRole.NewName;
                    $scope.roles[$scope.selectedIndex] = $scope.currentRole;
                    $scope.closeDetailsForm();
                } else {
                    UserMessagesProvider.errorHandler(999, data.message);
                }
                UserMessagesProvider.hideLoading();
            }, function (data, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        } else {
            UserMessagesProvider.invalidHandler();
        }
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