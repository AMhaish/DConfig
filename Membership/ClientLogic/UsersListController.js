appRoot.controllerProvider.register('Membership.UsersListController', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, MembershipDataProvider, EventsProvider, $modal, filterFilter) {
    $scope.DetailsPanelStates = {
        None: 0,
        Edit: 1,
        Create: 2,
        Info: 3,
        Roles: 4,
        ResetPassword: 5
    };
    function initialize() {
        BreadCrumpsProvider.breadCrumps.path = ['Membership Manager', 'Users'];
        $scope.closeDetailsForm();
        UserMessagesProvider.displayLoading();
        MembershipDataProvider.getRoles().success(function (data) {
            $scope.roles = data;
        })
        .error(function (data, status, headers, config) {
            UserMessagesProvider.hideLoading();
        });
        MembershipDataProvider.getUsers().success(function (data) {
            $scope.users = data;
            UserMessagesProvider.hideLoading();
        })
        .error(function (data, status, headers, config) {
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
            $scope.currentUser = jQuery.extend(true, {}, filterFilter($scope.users, $scope.searchText)[index]);
            $scope.state = $scope.DetailsPanelStates.Info;
            fetchExtendedInfo(index);
        }
    }

    $scope.displayEditForm = function (index) {
        if ($scope.state == $scope.DetailsPanelStates.Edit && $scope.selectedIndex != undefined && index == $scope.selectedIndex) {
            $scope.closeDetailsForm();
            $scope.selectedIndex = undefined;
        } else {
            $scope.selectedIndex = index;
            $scope.currentUser = jQuery.extend(true, {}, filterFilter($scope.users, $scope.searchText)[index]);
            $scope.state = $scope.DetailsPanelStates.Edit;
            fetchExtendedInfo(index);
        }
    }

    $scope.displayResetPasswordForm = function (index) {
        if ($scope.state == $scope.DetailsPanelStates.ResetPassword && $scope.selectedIndex != undefined && index == $scope.selectedIndex) {
            $scope.closeDetailsForm();
            $scope.selectedIndex = undefined;
        } else {
            $scope.selectedIndex = index;
            $scope.currentUser = jQuery.extend(true, {}, $scope.users[index]);
            $scope.state = $scope.DetailsPanelStates.ResetPassword;
        }
    }

    $scope.displayCreateForm = function () {
        $scope.closeDetailsForm();
        if ($scope.state != $scope.DetailsPanelStates.Create) {
            $scope.currentUser = {};
            $scope.state = $scope.DetailsPanelStates.Create;
        }
    }

    $scope.displayRolesForm = function (index) {
        if ($scope.state == $scope.DetailsPanelStates.Roles && $scope.selectedIndex != undefined && index == $scope.selectedIndex) {
            $scope.closeDetailsForm();
            $scope.selectedIndex = undefined;
        } else {
            $scope.selectedIndex = index;
            $scope.currentUser = jQuery.extend(true, {}, filterFilter($scope.users, $scope.searchText)[index]);
            $scope.currentUser.RolesNames = [];
            if ($scope.currentUser.Roles != undefined) {
                for (i = 0; i < $scope.currentUser.Roles.length; i++) {
                    for (j = 0; j < $scope.roles.length; j++) {
                        if ($scope.currentUser.Roles[i].RoleId == $scope.roles[j].Id) {
                            $scope.currentUser.RolesNames.push($scope.roles[j].Name);
                            break;
                        }
                    }
                }
            }
            $scope.state = $scope.DetailsPanelStates.Roles;
        }
    }

    $scope.addUserToRole = function () {
        var modalInstance = $modal.open({
            templateUrl: 'ItemsSelector.html',
            controller: itemsSelectorController,
            size: 'sm',
            resolve: {
                listOfItems: function () {
                    return $scope.roles;
                }
            }
        });
        modalInstance.result.then(function (obj) {
            if ($scope.currentUser.Roles == undefined) {
                $scope.currentUser.Roles = [];
            }
            if ($scope.currentUser.RolesNames == undefined) {
                $scope.currentUser.RolesNames = [];
            }
            if ($scope.users[$scope.selectedIndex].Roles ==undefined) {
                $scope.users[$scope.selectedIndex].Roles = [];
            }
            var exists = false;
            for (j = 0; j < $scope.currentUser.RolesNames.length; j++) {
                if (obj.Name == $scope.currentUser.RolesNames[j]) {
                    exists = true;
                    break;
                }
            }
            if (!exists) {
                MembershipDataProvider.addUserToRole($scope.currentUser, obj).success(function (returnValue) {
                    UserMessagesProvider.hideLoading();
                    if (returnValue.result == 'true') {
                        $scope.currentUser.Roles.push({ UserId: $scope.currentUser.Id, RoleId: obj.Id });
                        $scope.users[$scope.selectedIndex].Roles.push({ UserId: $scope.currentUser.Id, RoleId: obj.Id });
                        $scope.currentUser.RolesNames.push(obj.Name);
                    }
                    else {
                        UserMessagesProvider.errorHandler(999, returnValue.message);
                    }
                })
                .error(function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(999);
                });
            } else {
                UserMessagesProvider.errorHandler(999, "This role has been already added");
            }
        }, function () {
        });
    }

    $scope.removeUserFromRole = function (index) {
        UserMessagesProvider.confirmHandler("Are you sure you want to remove the user from this role?", function () {
            UserMessagesProvider.displayLoading();
            MembershipDataProvider.removeUserFromRole($scope.currentUser, $scope.currentUser.RolesNames[index]).success(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.result == 'true') {
                    $scope.currentUser.RolesNames.splice(index, 1);
                    $scope.users[$scope.selectedIndex].Roles.splice(index, 1);
                    $scope.currentUser.Roles.splice(index, 1);
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.message);
                }
            })
            .error(function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(999);
            });
        }, null);
    }

    $scope.deleteUser = function (index) {
        $scope.selectedIndex = index;
        $scope.currentUser = filterFilter($scope.users, $scope.searchText)[index];
        UserMessagesProvider.confirmHandler("Are you sure you want to delete this user?", function () {
            UserMessagesProvider.displayLoading();
            MembershipDataProvider.deleteUser($scope.currentUser).success(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.result == 'true') {
                    $scope.closeDetailsForm();
                    $scope.users.splice(index, 1);
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.message);
                }
            })
            .error(function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(999);
            });
        }, null);
    }

    $scope.createUser = function () {
        if ($scope.createForm.$valid) {
            UserMessagesProvider.displayLoading();
            MembershipDataProvider.createUser($scope.currentUser).success(function (returnValue) {
                if (returnValue.result == 'true') {
                    $scope.users.push($scope.currentUser);
                    UserMessagesProvider.hideLoading();
                    $scope.closeDetailsForm();
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.message);
                }
            })
            .error(function (data, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        } else {
            UserMessagesProvider.invalidHandler();
        }
    }

    $scope.editUser = function () {
        if ($scope.editForm.$valid) {
            UserMessagesProvider.displayLoading();
            MembershipDataProvider.updateUser($scope.currentUser).success(function (returnValue) {
                if (returnValue.result == 'true') {
                    $scope.users[$scope.selectedIndex] = $scope.currentUser;
                    MembershipDataProvider.updateUsersFieldsValues({ Id: $scope.currentUser.Id, DefinedUserProporties: $scope.definedUserProporties }).success(function (returnValue) {
                        if (returnValue.result == 'true') {
                            $scope.users[$scope.selectedIndex].definedUserProporties = $scope.definedUserProporties;
                            UserMessagesProvider.hideLoading();
                            $scope.closeDetailsForm();
                        } else {
                            UserMessagesProvider.errorHandler(999, returnValue.message);
                        }
                    }).error(function (data, status, headers, config) {
                        UserMessagesProvider.hideLoading();
                        UserMessagesProvider.errorHandler(status);
                    });
                } else {
                    UserMessagesProvider.errorHandler(999, returnValue.message);
                }
            })
            .error(function (data, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        } else {
            UserMessagesProvider.invalidHandler();
        }
    }

    $scope.resetPassword = function () {
        if ($scope.resetPasswordForm.$valid) {
            UserMessagesProvider.displayLoading();
            MembershipDataProvider.resetPassword($scope.currentUser).success(function (returnValue) {
                if (returnValue.result == 'true') {
                    UserMessagesProvider.hideLoading();
                    $scope.closeDetailsForm();
                } else {
                    UserMessagesProvider.errorHandler(999, returnValue.message);
                }
            })
            .error(function (data, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        } else {
            UserMessagesProvider.invalidHandler();
        }
    }

    function fetchExtendedInfo(index) {
        if ($scope.users[index].definedUserProporties == undefined) {
            UserMessagesProvider.displayLoading();
            MembershipDataProvider.getUsersFieldsValues($scope.currentUser.Id).success(function (data) {
                $scope.users[index].definedUserProporties = data;
                $scope.definedUserProporties = jQuery.extend(true, [], $scope.users[index].definedUserProporties);
                UserMessagesProvider.hideLoading();
            })
            .error(function (data, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        } else {
            $scope.definedUserProporties = jQuery.extend(true, [], $scope.users[index].definedUserProporties);
        }
    }

    initialize();
});

var itemsSelectorController = function ($scope, $modalInstance, listOfItems) {
    $scope.items = listOfItems;
    $scope.container = {};
    $scope.ok = function () {
        $modalInstance.close($scope.container.obj);
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};