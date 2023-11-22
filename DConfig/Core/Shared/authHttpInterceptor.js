angular.module('DConfigSharedLib').factory('AuthHttpInterceptor', ['$q', function ($q) {
    return {
        response: function (response) {
            return response;
        },
        responseError: function (response) {
            if (response.status==401) {
                window.location = "/DConfig";
            }else if(response.status==403){
                //UserMessagesProvider.errorHandler(403);
				$('#loading').hide();
                alert('You do not have enough permissions to request this resource or do this action.');
            }
            $q.reject();
        }
    };
}]);