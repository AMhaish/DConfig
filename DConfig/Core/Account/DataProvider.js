angular.module('DConfig')
        .factory('AccountDataProvider', ['$http', function config($http) {
            var dataFactory = {};
            var urlMembershipBase = '/Membership';

            dataFactory.login = function (data) {
                return $http.post(urlMembershipBase + '/login', data);
            };

            dataFactory.register = function (data) {
                return $http.post(urlMembershipBase + '/register', data);
            };

            dataFactory.passwordForgotten = function (data) {
                return $http.put(urlMembershipBase + '/forgetPassword', data);
            };


            dataFactory.resetpassword = function (data) {
                return $http.post(urlMembershipBase + '/resetPasswordThroughEmail', data);
            };
            //dataFactory.getEmployee = function (id) {
            //    return $http.get(urlMembershipBase + '/GetEmployee' + id);
            //};

            //dataFactory.createEmployee = function (data) {
            //    return $http.post(urlMembershipBase + '/CreateEmployee', data);
            //};

            //dataFactory.updateEmployee = function (data) {
            //    return $http.put(urlMembershipBase + '/UpdateEmployee/' + data.id, data);
            //};

            //dataFactory.deleteEmployee = function (id) {
            //    return $http.delete(urlMembershipBase + '/DeleteEmployee/' + id);
            //};
            return dataFactory;
        }]);
