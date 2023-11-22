angular.module('DConfig')
    .controller('ResetPasswordController', ['$scope', '$location', 'BreadCrumpsProvider', 'UserMessagesProvider', 'AccountDataProvider', 'EventsProvider', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, AccountDataProvider, EventsProvider) {

        $scope.doResetPassword = function () {
            UserMessagesProvider.displayLoading();
            $scope.user.UserId = getQueryStringParameterByName('UserId');
            $scope.user.Token = getQueryStringParameterByName('Token');
            AccountDataProvider.resetpassword($scope.user).then(function (data) {
                if (data.data.result === 'true') {
                    UserMessagesProvider.successHandler("Your new password has been set successfully.");
                    $location.path('/Login');
                } else {
                    UserMessagesProvider.errorHandler(999, data.data.message);
                }
            }, function (data, status, headers, config) {
                UserMessagesProvider.errorHandler(status);
            });
        };

        $scope.backToLogin = function () {
            $location.path('/Login');
        }

    }]);