angular.module('DConfigSharedLib').factory('GlobalVariablesProvider', ['$http', function ($http) {
    var instance = {};

    function bindVariableFromServer(key, successHandler, failedHandler) {
        switch (key) {
            default:
                $http.get(key).then(function (data) {
                    instance[key] = data.data;
                    if (successHandler)
                        successHandler(instance[key]);
                }, function (data, status, headers, config) {
                    if (failedHandler)
                        failedHandler(status);
                });
        }
    }
    return {
        get: function (key) {
            if (instance.hasOwnProperty(key)) {
                return instance[key];
            } else {
                return null;
            }
        },
        getFromUrl: function (url, successHandler, failedHandler) {
            if (!instance.hasOwnProperty(url)) {
                bindVariableFromServer(url, successHandler, failedHandler);
            } else {
                if (successHandler)
                    successHandler(instance[url]);
            }
        },
        set: function (key, value) {
            instance[key] = value;
        },
        cached: function (key) {
            return instance.hasOwnProperty(key);
        },
        clearCache: function () {
            instance = {};
        }
    };

}]);