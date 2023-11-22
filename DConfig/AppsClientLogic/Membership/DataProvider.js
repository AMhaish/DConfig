angular.module('DConfig').provide.factory('MembershipDataProvider', ['$http', function config($http) {
            var dataFactory = {};
            dataFactory.getUsers = function () {
                return $http.get('/Membership/GetContextUsers');
            };
            dataFactory.getUsersByUserName = function (userName) {
                return $http.get('/Membership/getusersbyname?UserName=' + userName);
            };
            dataFactory.createUser = function (user) {
                return $http.post('/Membership/CreateContextUser',user);
            };
            dataFactory.updateUser = function (user) {
                return $http.put('/Membership/UpdateContextUser',user);
            };
            dataFactory.resetPassword = function (user) {
                return $http.put('/Membership/resetPassword', user);
            };
            /*dataFactory.deleteUser = function (user) {
                return $http.delete('/Membership/DeleteContextUser?UserName=' + user.UserName);
            };*/
            dataFactory.deleteUser = function (user) {
                return $http.delete('/Membership/deleteuser?UserName=' + user.UserName);
            };
            dataFactory.getUsersFields = function () {
                return $http.get('/Membership/GetUsersFields');
            };
            dataFactory.updateUsersFields = function (filedsObj) {
                return $http.put('/Membership/UpdateUsersFields',filedsObj);
            };
            dataFactory.deleteUserField = function (fieldId) {
                return $http.delete('/Membership/DeleteUserField?id=' + fieldId);
            };
            dataFactory.updateUsersFieldsValues = function (filedsObj) {
                return $http.put('/Membership/UpdateUsersFieldsValues', filedsObj);
            };
            dataFactory.getUsersFieldsValues = function (id) {
                return $http.get('/Membership/GetUsersFieldsValues?Id=' + id);
            };
            dataFactory.getRoles = function () {
                return $http.get('/Membership/getroles');
            };
            dataFactory.createRole = function (role) {
                return $http.post('/Membership/createrole', {RoleName:role.Name});
            };
            dataFactory.updateRole = function (role) {
                return $http.put('/Membership/updaterole', {RoleName:role.Name,NewRoleName:role.NewName});
            };
            dataFactory.deleteRole = function (role) {
                return $http.delete('/Membership/deleterole?RoleName=' + role.Name);
            };
            
            dataFactory.getCompanies = function () {
                return $http.get('/Membership/getCompanies');
            };
            dataFactory.createCompany = function (company) {
                return $http.post('/Membership/createCompany', company);
            };
            dataFactory.updateCompany = function (company) {
                return $http.put('/Membership/updateCompany', company);
            };
            dataFactory.deleteCompany = function (company) {
                return $http.delete('/Membership/deleteCompany?Id=' + company.Id);
            };

            dataFactory.addUserToRole = function (user,role) {
                return $http.put('/Membership/addUserToRole?UserName=' + user.UserName + '&RoleName=' + role.Name , {});
            };
            dataFactory.removeUserFromRole = function (user,roleName) {
                return $http.delete('/Membership/removeUserFromRole?UserName=' + user.UserName + '&RoleName=' + roleName);
            };
            
            dataFactory.addUserToCompany = function (companyId,usersIds) {
                return $http.put('/Membership/updateCompanyUsers?Id=' + companyId, { UsersIds: usersIds });
            };
            dataFactory.removeUsersFromCompany = function (companyId, usersId) {
                return $http.delete('/Membership/deleteCompanyUsers?Id=' + companyId + '&UserId=' + usersId);
            };

            dataFactory.getPrivileges = function () {
                return $http.get('/Membership/getPrivileges');
            };
            dataFactory.createPrivilege = function (privilege,controllerName,actionName,requestType) {
                return $http.post('/Membership/createPrivilege?PrvController=' + controllerName + '&PrvAction=' + actionName + '&RequestType=' + requestType, privilege);
            };
            dataFactory.updatePrivilege = function (privilege) {
                return $http.put('/Membership/updatePrivilege', privilege);
            };
            dataFactory.deletePrivilege = function (privilege) {
                return $http.delete('/Membership/deletePrivilege?Id=' + privilege.Id);
            };

            dataFactory.getSystemControllersTree = function () {
                return $http.get('/Membership/GetSystemControllersTree');
            };
            dataFactory.addPrivilegeToRole = function (privilegeId, roleName) {
                return $http.put('/Membership/addPrivilegeToRole?PrivilegeId=' + privilegeId + '&RoleName=' + roleName, {});
            };
            dataFactory.removePrivilegeFromRole = function (privilegeId, roleName) {
                return $http.delete('/Membership/removePrivilegeFromRole?PrivilegeId=' + privilegeId + '&RoleName=' + roleName);
            };

            dataFactory.getContentPrivileges = function () {
                return $http.get('/Membership/getContentPrivileges');
            };
            dataFactory.createContentPrivilege = function (privilege, contentId) {
                return $http.post('/Membership/createContentPrivilege?ContentId=' + contentId, privilege);
            };
            dataFactory.updateContentPrivilege = function (privilege) {
                return $http.put('/Membership/updateContentPrivilege', privilege);
            };
            dataFactory.deleteContentPrivilege = function (contentId) {
                return $http.delete('/Membership/deleteContentPrivilege?ContentId=' + contentId);
            };
            dataFactory.addContentPrivilegeToRole = function (contentId, roleName) {
                return $http.put('/Membership/addContentPrivilegeToRole?ContentId=' + contentId + '&RoleName=' + roleName, {});
            };
            dataFactory.removeContentPrivilegeFromRole = function (contentId, roleName) {
                return $http.delete('/Membership/removeContentPrivilegeFromRole?ContentId=' + contentId + '&RoleName=' + roleName);
            };
            dataFactory.getContentsTree = function () {
                return $http.get('/Membership/GetContentsTree');
            };

            dataFactory.getUserFieldEnumsTree = function () {
                return $http.get('/Membership/getUserFieldEnumsTree');
            };
            dataFactory.getUserFieldEnums = function () {
                return $http.get('/Membership/getUserFieldEnums');
            };
            dataFactory.createUserFieldEnum = function (enumObj) {
                return $http.post('/Membership/createUserFieldEnum', enumObj);
            };
            dataFactory.updateUserFieldEnum = function (enumObj) {
                return $http.put('/Membership/updateUserFieldEnum', enumObj);
            };
            dataFactory.deleteUserFieldEnum = function (enumObj) {
                return $http.delete('/Membership/deleteUserFieldEnum?Id=' + enumObj.Id + "&Name=" + enumObj.Name);
            };
            dataFactory.updateUserFieldEnumValues = function (id, values) {
                return $http.put('/Membership/updateUserFieldEnumValues?Id=' + id, values);
            };
            dataFactory.deleteUserFieldEnumValue = function (id) {
                return $http.delete('/Membership/deleteUserFieldEnumValue?Id=' + id);
            };
            dataFactory.getUserFieldTypes = function () {
                return $http.get('/Membership/getUserFieldTypes');
            };

            dataFactory.getAllApps = function () {
                return $http.get('/DConfig/AppsAPI/GetAllApps');
            };
            dataFactory.updateAppRoles = function (id,roles) {
                return $http.put('/DConfig/AppsAPI/updateAppRoles?Id=' + id, roles);
            };
            return dataFactory;
}]);



//@ sourceURL=MembershipScripts.js