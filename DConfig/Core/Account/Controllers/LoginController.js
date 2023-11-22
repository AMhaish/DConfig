angular.module('DConfig')
    .controller('LoginController', ['$scope', '$location', 'BreadCrumpsProvider', 'UserMessagesProvider', 'AccountDataProvider', 'EventsProvider', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, AccountDataProvider, EventsProvider) {
        //BreadCrumpsProvider.breadCrumps.path = ['الوظائف', 'الموارد', 'المخافر'];
        $('.login').animate({ 'left': '0', 'opacity': '1' }, 500);

        $scope.doLogin = function () {
            UserMessagesProvider.displayLoading();
            AccountDataProvider.login($scope.user).then(function (data) {
                if (data.data.result == 'true') {
                    if (data.returnUrl != null) {
                        window.location == data.returnUrl;
                    } else {
                        window.location = '/DConfig/Explorer#/';
                    }
                } else {
                    UserMessagesProvider.errorHandler(999, data.message);
                }
            }, function (data, status, headers, config) {
                UserMessagesProvider.errorHandler(status);
            });
        };

        $scope.goToRegisterPage = function () {
            $location.path('/Register');
        };

        $scope.doExternalLogin = function (provider) {
            $('#provider').val(provider);
            document.externalLoginForm.submit();
            //document.getElementById('formID').submit();
        };
    }]);