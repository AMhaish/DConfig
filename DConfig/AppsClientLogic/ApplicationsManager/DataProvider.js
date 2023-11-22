angular.module('DConfig').provide.factory('ApplicationsManagerDataProvider', ['$http', function config($http) {
            var dataFactory = {};
            dataFactory.getApps = function () {
                return $http.get('/DConfig/AppsAPI/GetApps');
            };
            dataFactory.uninstallApp = function (name) {
                return $http.get('/DConfig/AppsAPI/UninstallAppAction?Name=' + name);
            };
            return dataFactory;
}]);

//@ sourceURL=MembershipScripts.js