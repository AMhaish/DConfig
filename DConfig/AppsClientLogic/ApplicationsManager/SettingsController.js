angular.module('DConfig').controllerProvider.register('Membership.SettingsController', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, MembershipDataProvider, EventsProvider, $modal) {
    $scope.DetailsPanelStates = {
        None: 0,
        DefinedUsersProporties: 1
    };
    $scope.FieldsTypes = [
        { Id: "String", Text: "String" }
    ];
    $scope.currentObj = {};

    function initialize() {
        $scope.closeDetailsForm();
    }

    $scope.closeDetailsForm = function () {
        $scope.state = $scope.DetailsPanelStates.None;
    }

    $scope.displayDefinedUsersProporties = function () {
        UserMessagesProvider.displayLoading();
        MembershipDataProvider.getUsersFields().then(function (data) {
            $scope.currentObj.DefinedUserProporties = data.data;
            $scope.state = $scope.DetailsPanelStates.DefinedUsersProporties;
            UserMessagesProvider.hideLoading();
        }, function (data, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });
    }

    $scope.addDefinedUserProportiesNewField = function () {
        $scope.currentObj.DefinedUserProporties.push({ Name: '', Type: '', open: true });
    }

    $scope.removeDefinedUserProportiesNewField = function (index) {
        var modalInstance = $modal.open({
            templateUrl: 'FieldDeleteConfirmation.html',
            controller: confirmMessageController,
            size: 'sm'
        });
        modalInstance.result.then(function () {
            UserMessagesProvider.displayLoading();
            MembershipDataProvider.deleteUserField($scope.currentObj.DefinedUserProporties[index].Id).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    $scope.currentObj.DefinedUserProporties.splice(index, 1);
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.message);
                }
                $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
            }, function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        }, function () {
        });
    }

    $scope.saveDefinedUserProporties = function () {
        UserMessagesProvider.displayLoading();
        MembershipDataProvider.updateUsersFields($scope.currentObj).then(function (returnValue) {
            UserMessagesProvider.hideLoading();
            if (returnValue.data.result == 'true') {
                for (i = 0; i < $scope.currentObj.DefinedUserProporties.length; i++) {
                    $scope.currentObj.DefinedUserProporties[i].open = false;
                }
                $scope.closeDetailsForm();
            }
            else {
                UserMessagesProvider.errorHandler(999, returnValue.message);
            }
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
        }, function (errorData, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
        });
    }
    initialize();
});