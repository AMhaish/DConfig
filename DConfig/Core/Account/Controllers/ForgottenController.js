angular.module('DConfig')
    .controller('ForgottenController', ['$scope', '$location', 'BreadCrumpsProvider', 'UserMessagesProvider', 'AccountDataProvider', 'EventsProvider', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, AccountDataProvider, EventsProvider) {

        $scope.doResetPassword = function () {
            UserMessagesProvider.displayLoading();
            AccountDataProvider.passwordForgotten($scope.user).then(function (data) {
                if (data.data.result === 'true') {
                    UserMessagesProvider.notificationHandler("An email send to you with instruction to reset your password");
                    $location.path('/Login');
                } else {
                    UserMessagesProvider.errorHandler(999, data.message);
                }
            }, function (data, status, headers, config) {
                UserMessagesProvider.errorHandler(status);
            });
        };

        $scope.backToLogin = function () {
            $location.path('/Login');
        }

    }]);