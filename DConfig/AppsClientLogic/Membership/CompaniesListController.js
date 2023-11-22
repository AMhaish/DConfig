angular.module('DConfig').controllerProvider.register('Membership.CompaniesListController', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, MembershipDataProvider, EventsProvider, $modal, filterFilter) {
    $scope.DetailsPanelStates = {
        None: 0,
        Edit: 1,
        Create: 2,
        Info: 3,
        Users: 4
    };
    function initialize() {
        BreadCrumpsProvider.breadCrumps.path = ['Membership Manager', 'Companies'];
        $scope.closeDetailsForm();
        UserMessagesProvider.displayLoading();
        MembershipDataProvider.getCompanies().then(function (data) {
            $scope.companies = data.data;
            UserMessagesProvider.hideLoading();
        }, function (data, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });
    }

    $scope.closeDetailsForm = function () {
        $scope.state = $scope.DetailsPanelStates.None;
    }

    $scope.displayDetails = function (index) {
        if ($scope.state == $scope.DetailsPanelStates.Info && $scope.selectedIndex != undefined && index == $scope.selectedIndex) {
            $scope.selectedIndex = undefined;
            $scope.closeDetailsForm();
        } else {
            $scope.selectedIndex = index;
            $scope.currentCompany = jQuery.extend(true, {}, filterFilter($scope.companies, $scope.searchText)[index]);
            $scope.state = $scope.DetailsPanelStates.Info;
        }
    }

    $scope.displayEditForm = function (index) {
        if ($scope.state == $scope.DetailsPanelStates.Edit && $scope.selectedIndex != undefined && index == $scope.selectedIndex) {
            $scope.closeDetailsForm();
            $scope.selectedIndex = undefined;
        } else {
            $scope.selectedIndex = index;
            $scope.currentCompany = jQuery.extend(true, {}, filterFilter($scope.companies, $scope.searchText)[index]);
            $scope.state = $scope.DetailsPanelStates.Edit;
        }
    }

    $scope.displayCreateForm = function () {
        $scope.closeDetailsForm();
        if ($scope.state != $scope.DetailsPanelStates.Create) {
            $scope.currentCompany = {};
            $scope.state = $scope.DetailsPanelStates.Create;
        }
    }

    $scope.displayCompanyUsers = function (index) {
        if ($scope.state == $scope.DetailsPanelStates.Users && $scope.selectedIndex != undefined && index == $scope.selectedIndex) {
            $scope.closeDetailsForm();
            $scope.selectedIndex = undefined;
        } else {
            $scope.selectedIndex = index;
            $scope.currentCompany = jQuery.extend(true, {}, filterFilter($scope.companies, $scope.searchText)[index]);
            $scope.state = $scope.DetailsPanelStates.Users;
        }
    }

    $scope.addUsersToCompany = function () {
        var modalInstance = $modal.open({
            templateUrl: 'UsersSelector.html',
            controller: usersSearchController,
            size: 'lg',
            resolve: {
                DataProvider: function () {
                    return MembershipDataProvider;
                },
                UMP: function () {
                    return UserMessagesProvider;
                }
            }
        });
        modalInstance.result.then(function (result) {
            var ids = [];
            for (i = 0; i < result.objs.length; i++) {
                ids.push(result.objs[i].Id);
            }
            UserMessagesProvider.displayLoading();
            MembershipDataProvider.addUserToCompany($scope.currentCompany.Id, ids).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                var exists = false;
                if (returnValue.data.result == 'true') {
                    for (i = 0; i < result.objs.length; i++) {
                        exists = false;
                        if ($scope.currentCompany.Users == null) {
                            $scope.currentCompany.Users = [];
                        }
                        for (j = 0; j < $scope.currentCompany.Users.length; j++) {
                            if (result.objs[i].Id == $scope.currentCompany.Users[j].Id) {
                                exists = true;
                                break;
                            }
                        }
                        if (!exists) {
                            $scope.currentCompany.Users.push(result.objs[i]);
                            $scope.companies[$scope.selectedIndex].Users.push(result.objs[i]);
                        }
                    }
                    $scope.companies.splice(index, 1);
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.data.message);
                }
                UserMessagesProvider.hideLoading();
            }, function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(999);
            });
        }, function () {
        });
    }

    $scope.removeUsersFromCompany = function (index) {
        UserMessagesProvider.confirmHandler("Are you sure you want to remove this user from comapny?", function () {
            UserMessagesProvider.displayLoading();
            MembershipDataProvider.removeUsersFromCompany($scope.currentCompany.Id, $scope.currentCompany.Users[index].Id).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    $scope.currentCompany.Users.splice(index, 1);
                    $scope.companies[$scope.selectedIndex].Users.splice(index, 1);
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.data.message);
                }
            }, function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(999);
            });
        }, null);
    }

    $scope.deleteCompany = function (index) {
        $scope.selectedIndex = index;
        $scope.currentCompany = filterFilter($scope.companies, $scope.searchText)[index];
        UserMessagesProvider.confirmHandler("Are you sure you want to delete this company?", function () {
            UserMessagesProvider.displayLoading();
            MembershipDataProvider.deleteCompany($scope.currentCompany).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    $scope.closeDetailsForm();
                    $scope.companies.splice(index, 1);
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.data.message);
                }
            }, function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(999);
            });
        }, null);
    }

    $scope.createCompany = function () {
        if ($scope.createForm.$valid) {
            UserMessagesProvider.displayLoading();
            MembershipDataProvider.createCompany($scope.currentCompany).then(function (data) {
                if (data.data.result == 'true') {
                    $scope.currentCompany.Id = data.data.obj.Id;
                    $scope.companies.push($scope.currentCompany);
                    $scope.closeDetailsForm();
                }
                else {
                    UserMessagesProvider.errorHandler(999, data.data.message);
                }
                UserMessagesProvider.hideLoading();
            }, function (data, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        } else {
            UserMessagesProvider.invalidHandler();
        }
    }

    $scope.editCompany = function () {
        if ($scope.editForm.$valid) {
            UserMessagesProvider.displayLoading();
            MembershipDataProvider.updateCompany($scope.currentCompany).then(function (data) {
                if (data.data.result == 'true') {
                    $scope.companies[$scope.selectedIndex] = $scope.currentCompany;
                    $scope.closeDetailsForm();
                } else {
                    UserMessagesProvider.errorHandler(999, data.data.message);
                }
                UserMessagesProvider.hideLoading();
            }, function (data, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        } else {
            UserMessagesProvider.invalidHandler();
        }
    }

    initialize();
});
var usersSearchController = function ($scope, $modal, $modalInstance, DataProvider, UMP) {
    $scope.SelectedUsers = { arr: [] };
    $scope.userName = { value: '' };

    $scope.ok = function () {
        if ($scope.SelectedUsers.arr.length > 0) {
            $modalInstance.close({ objs: $scope.SelectedUsers.arr });
        } else {
            UMP.errorHandler(999, "You haven't chosen any user to add");
        }
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.loadUsers = function () {
        $scope.Users = [];
        var pattern = $scope.userName.value;
        if (pattern.length >= 3 || pattern == "*") {
            UMP.addLoadingToWindow('usersList');
            DataProvider.getUsersByUserName(pattern).then(function (data) {
                $scope.Users = data.data;
                UMP.removeLoadingFromWindow('usersList');
            }, function (data, status, headers, config) {
                UMP.removeLoadingFromWindow('usersList');
                UMP.errorHandler(status);
            });
        }
    }
};