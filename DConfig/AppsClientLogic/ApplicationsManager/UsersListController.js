angular.module('DConfig').controllerProvider.register('Membership.UsersListController', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, MembershipDataProvider, EventsProvider, $modal) {
    $scope.DetailsPanelStates = {
        None: 0,
        Edit: 1,
        Create: 2,
        Info: 3
    };
    function initialize() {
        $scope.closeDetailsForm();
        UserMessagesProvider.displayLoading();
        MembershipDataProvider.getUsers().then(function (data) {
            $scope.users = data.data;
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
            $scope.currentUser = jQuery.extend(true, {}, $scope.users[index]);
            $scope.state = $scope.DetailsPanelStates.Info;
            fetchExtendedInfo(index);
        }
    }

    $scope.displayEditForm = function (index) {
        if ($scope.state == $scope.DetailsPanelStates.Edit && $scope.selectedIndex != undefined && index == $scope.selectedIndex) {
            $scope.closeDetailsForm();
            $scope.selectedIndex = undefined;
        } else {
            $scope.selectedIndex = index;
            $scope.currentUser = jQuery.extend(true, {}, $scope.users[index]);
            $scope.state = $scope.DetailsPanelStates.Edit;
            fetchExtendedInfo(index);
        }
    }

    $scope.displayCreateForm = function () {
        $scope.closeDetailsForm();
        if ($scope.state != $scope.DetailsPanelStates.Create) {
            $scope.currentUser = {};
            $scope.state = $scope.DetailsPanelStates.Create;
        }
    }

    $scope.displayOptionsForm = function () {
        $scope.closeDetailsForm();
        if ($scope.state != $scope.DetailsPanelStates.Options) {
            $scope.state = $scope.DetailsPanelStates.Options;
        }
    }

    $scope.deleteUser = function (index) {
        $scope.selectedIndex = index;
        $scope.currentUser = $scope.users[index];
        var modalInstance = $modal.open({
            templateUrl: 'DeleteConfirmation.html',
            controller: confirmMessageController,
            size: 'sm'
        });
        modalInstance.result.then(function () {
            UserMessagesProvider.displayLoading();
            MembershipDataProvider.deleteUser($scope.currentUser).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    $scope.closeDetailsForm();
                    $scope.users.splice(index, 1);
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.message);
                }
            }, function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(999);
            });
        }, function () {
        });
    }

    $scope.createUser = function () {
        if ($scope.createForm.$valid) {
            UserMessagesProvider.displayLoading();
            MembershipDataProvider.createUser($scope.currentUser).then(function (data) {
                $scope.users.push($scope.currentUser);
                UserMessagesProvider.hideLoading();
                $scope.closeDetailsForm();
            }, function (data, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        } else {
            UserMessagesProvider.invalidHandler();
        }
    }

    $scope.editUser = function () {
        if ($scope.editForm.$valid) {
            UserMessagesProvider.displayLoading();
            MembershipDataProvider.updateUser($scope.currentUser).then(function (data) {
                $scope.users[$scope.selectedIndex] = $scope.currentUser;
                MembershipDataProvider.updateUsersFieldsValues({ Id: $scope.currentUser.Id, DefinedUserProporties: $scope.definedUserProporties }).then(function (data) {
                    $scope.users[$scope.selectedIndex].definedUserProporties = $scope.definedUserProporties;
                    UserMessagesProvider.hideLoading();
                    $scope.closeDetailsForm();
                }, function (data, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                });
            }, function (data, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        } else {
            UserMessagesProvider.invalidHandler();
        }
    }

    function fetchExtendedInfo(index) {
        if ($scope.users[index].definedUserProporties == undefined) {
            UserMessagesProvider.displayLoading();
            MembershipDataProvider.getUsersFieldsValues($scope.currentUser.Id).then(function (data) {
                $scope.users[index].definedUserProporties = data.data;
                $scope.definedUserProporties = jQuery.extend(true, {}, $scope.users[index].definedUserProporties);
                UserMessagesProvider.hideLoading();
            }, function (data, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        } else {
            $scope.definedUserProporties = jQuery.extend(true, {}, $scope.users[index].definedUserProporties);
        }
    }

    initialize();
});

var confirmMessageController = function ($scope, $modalInstance) {
    $scope.ok = function () {
        $modalInstance.close();
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};