angular.module('DConfig')
    .controller('RegisterController', ['$scope', '$location', 'BreadCrumpsProvider', 'UserMessagesProvider', 'AccountDataProvider', 'EventsProvider', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, AccountDataProvider, EventsProvider) {
        //BreadCrumpsProvider.breadCrumps.path = ['الوظائف', 'الموارد', 'المخافر'];
        $('.register').animate({ 'left': '0', 'opacity': '1' }, 500);

        $scope.doRegister = function () {
            UserMessagesProvider.displayLoading();
            AccountDataProvider.register($scope.user).then(function (data) {
                if (data.data.result == 'true') {
                    UserMessagesProvider.successHandler();
                } else {
                    UserMessagesProvider.errorHandler(999, data.message);
                }
            }, function (data, status, headers, config) {
                UserMessagesProvider.errorHandler(status);
            });
        };

        $scope.goToLoginPage = function () {
            $location.path('/Login');
        };
    }]);