angular.module('DConfigSharedLib')
    .factory('UserPermissionsProvider', ['$http', function ($http) {
        var userSecurityGroupProvider = {};
        var permissionsIdsUserHave = [];
        var permissionsIdsUserDontHave = [];
        var userSecurityGroupBase = '/API/AccountServices/CheckPermissionForUser';

        function checkAuthorizationAvialabilityInClinet(requestedPermission) {
            var authorized = false;
            for (var i = 0; i < permissionsIdsUserHave.length; i++) {
                if (permissionsIdsUserHave[i] == requestedPermission) {
                    authorized = true;
                    break;
                }
            }
            return authorized;
        }

        function checkNoAuthorizationAvialabilityInClinet(requestedPermission) {
            var denided = false;
            for (var i = 0; i < permissionsIdsUserDontHave.length; i++) {
                if (permissionsIdsUserDontHave[i] == requestedPermission) {
                    denided = true;
                    break;
                }
            }
            return denided;
        }

        userSecurityGroupProvider.checkAuthorityAndExecuteCommand = function (requestedPermission, authorizedCommand, notAuthorizedCommand, failedCommand) {
            var authorized = checkAuthorizationAvialabilityInClinet(requestedPermission);
            var denided = checkNoAuthorizationAvialabilityInClinet(requestedPermission);
            if (authorized) {
                if (authorizedCommand) {
                    authorizedCommand();
                }
            } else if (denided) {
                if (notAuthorizedCommand) {
                    notAuthorizedCommand();
                }
            }
            else {
                $http.get(userSecurityGroupBase + '/' + requestedPermission).then(function (data) {
                    if (data) {
                        permissionsIdsUserHave.push(requestedPermission);
                        if (authorizedCommand) {
                            authorizedCommand();
                        }
                    }
                    else {
                        permissionsIdsUserDontHave.push(requestedPermission);
                        if (notAuthorizedCommand) {
                            notAuthorizedCommand();
                        }
                    }
                }, function (data, status, headers, config) {
                    if (failedCommand) {
                        failedCommand();
                    }
                });
            }
        }
        return userSecurityGroupProvider;
    }]);
