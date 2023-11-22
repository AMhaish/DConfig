appRoot.controllerProvider.register('Membership.SettingsController', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, MembershipDataProvider, EventsProvider, $modal) {
    $scope.DetailsPanelStates = {
        None: 0,
        DefinedUsersProporties: 1
    };
    $scope.currentObj = {};

    function initialize() {
        BreadCrumpsProvider.breadCrumps.path = ['Membership Manager', 'Settings'];
        UserMessagesProvider.displayLoading();
        MembershipDataProvider.getUserFieldTypes().success(function (data) {
            $scope.FieldsTypes = data;
        })
        .error(function (data, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });
        MembershipDataProvider.getUserFieldEnums().success(function (data) {
            $scope.Enums = data;
            UserMessagesProvider.hideLoading();
        })
        .error(function (data, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });
        $scope.closeDetailsForm();
    }

    $scope.closeDetailsForm = function () {
        $scope.state = $scope.DetailsPanelStates.None;
    }

    $scope.displayDefinedUsersProporties = function () {
        UserMessagesProvider.displayLoading();
        MembershipDataProvider.getUsersFields().success(function (data) {
            $scope.currentObj.DefinedUserProporties = data;
            $scope.state = $scope.DetailsPanelStates.DefinedUsersProporties;
            UserMessagesProvider.hideLoading();
        })
        .error(function (data, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });
    }

    $scope.addDefinedUserProportiesNewField = function () {
        $scope.currentObj.DefinedUserProporties.push({ Name: '', Type: '', open: true });
    }

    $scope.removeDefinedUserProportiesNewField = function (index) {
        if ($scope.currentObj.DefinedUserProporties[index].Id == undefined) {
            $scope.currentObj.DefinedUserProporties.splice(index, 1);
            return;
        }
        UserMessagesProvider.confirmHandler("Deleting field will causing all the users data linked with this field to be lost, are you sure you want to delete it?", function () {
            UserMessagesProvider.displayLoading();
            MembershipDataProvider.deleteUserField($scope.currentObj.DefinedUserProporties[index].Id).success(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.result == 'true') {
                    $scope.currentObj.DefinedUserProporties.splice(index, 1);
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.message);
                }
            })
            .error(function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        }, null);
    }

    $scope.saveDefinedUserProporties = function () {
        UserMessagesProvider.displayLoading();
        MembershipDataProvider.updateUsersFields($scope.currentObj).success(function (returnValue) {
            UserMessagesProvider.hideLoading();
            if (returnValue.result == 'true') {
                for (i = 0; i < $scope.currentObj.DefinedUserProporties.length; i++) {
                    $scope.currentObj.DefinedUserProporties[i].open = false;
                }
                $scope.closeDetailsForm();
            }
            else {
                UserMessagesProvider.errorHandler(999, returnValue.message);
            }
        })
        .error(function (errorData, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });
    }
    initialize();
});