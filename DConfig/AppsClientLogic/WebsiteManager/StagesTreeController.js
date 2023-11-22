angular.module('DConfig').controllerProvider.register('WebsiteManager.StagesTreeController', function ($scope, $filter, $location, BreadCrumpsProvider, UserMessagesProvider, WebsiteManagerDataProvider, EventsProvider, $modal, scopeService, filterFilter) {
    $scope.DetailsPanelStates = {
        None: 0,
        Edit: 1,
        Create: 2,
        Info: 3,
        StageDetails: 4,
        StageRoles: 5,
        NextStages: 6,
    };

    $scope.state = $scope.DetailsPanelStates.None;

    function initialize() {
        BreadCrumpsProvider.breadCrumps.path = ['Website Manager', 'Stages'];
        UserMessagesProvider.displayLoading();
        WebsiteManagerDataProvider.getStages().then(function (data) {
            $scope.stages = data.data;
            UserMessagesProvider.hideLoading();
        }, function (data, status, headers, config) {
            UserMessagesProvider.errorHandler(status);
        });

        WebsiteManagerDataProvider.getRoles().then(function (data) {
            $scope.roles = data.data;
        }, function (data, status, headers, config) {
            UserMessagesProvider.hideLoading();
        });
    }

    $scope.closeDetailsForm = function () {
        $scope.state = $scope.DetailsPanelStates.None;
    }



    $scope.displayEditForm = function (index) {
        if ($scope.state == $scope.DetailsPanelStates.Edit && $scope.selectedIndex != undefined && index == $scope.selectedIndex) {
            $scope.closeDetailsForm();
            $scope.selectedIndex = undefined;
        } else {
            $scope.selectedIndex = index;
            $scope.currentObj = jQuery.extend(true, {}, filterFilter($scope.stages, $scope.searchText)[index]);
            $scope.state = $scope.DetailsPanelStates.Edit;
        }
    }

    $scope.displayCreateForm = function () {
        $scope.closeDetailsForm();
        if ($scope.state != $scope.DetailsPanelStates.Create) {
            $scope.currentObj = {};
            $scope.state = $scope.DetailsPanelStates.Create;
        }
    }

    $scope.displayDetails = function () {
        if ($scope.state != $scope.DetailsPanelStates.StageDetails)
            $scope.state = $scope.DetailsPanelStates.StageDetails;
    }

    $scope.displayDetailsForm = function (index) {
        if ($scope.state == $scope.DetailsPanelStates.StageDetails && $scope.selectedIndex != undefined && index == $scope.selectedIndex) {
            $scope.selectedIndex = undefined;
            $scope.closeDetailsForm();
        } else {
            $scope.selectedIndex = index;
            $scope.currentObj = jQuery.extend(true, {}, filterFilter($scope.stages, $scope.searchText)[index]);
            $scope.state = $scope.DetailsPanelStates.StageDetails;
        }
    }


    $scope.displayNextStages = function () {
        if ($scope.currentObj) {
            UserMessagesProvider.displayLoading();
            WebsiteManagerDataProvider.getNextStages($scope.currentObj.Id).then(function (data) {
                $scope.state = $scope.DetailsPanelStates.NextStages;
                var potentialStages = $scope.stages.slice();
                potentialStages.splice($scope.selectedIndex, 1)
                $scope.currentObj.NextStages = jQuery.extend(true, [], potentialStages);
                if (data.data.length > 0) {
                    for (j = 0; j < $scope.currentObj.NextStages.length; j++) {
                        for (i = 0; i < data.data.length; i++) {
                            if ($scope.currentObj.NextStages[j].Id == data.data[i].Id) {
                                $scope.currentObj.NextStages[j].Selected = true;
                            }
                        }
                    }
                }
                UserMessagesProvider.hideLoading();
            }, function (data, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        }
    }

    $scope.displayRolesForm = function () {
        UserMessagesProvider.displayLoading();
        $scope.currentObj = jQuery.extend(true, {}, filterFilter($scope.stages, $scope.searchText)[$scope.selectedIndex]);
        $scope.currentObj.RolesNames = [];
        WebsiteManagerDataProvider.getStageRoles($scope.currentObj.Id).then(function (data) {
            $scope.currentObj.Roles = data.data;
            if ($scope.currentObj.Roles != undefined) {
                for (i = 0; i < $scope.currentObj.Roles.length; i++) {
                    for (j = 0; j < $scope.roles.length; j++) {
                        if ($scope.currentObj.Roles[i] == $scope.roles[j].Id) {
                            $scope.currentObj.RolesNames.push($scope.roles[j].Name);
                            break;
                        }
                    }
                }
            }

            $scope.state = $scope.DetailsPanelStates.StageRoles;
            UserMessagesProvider.hideLoading();
        }, function (data, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });
    }

    $scope.addStageToRole = function () {
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
            console.log(obj);
            if ($scope.currentObj.Roles == undefined) {
                $scope.currentObj.Roles = [];
            }
            if ($scope.currentObj.RolesNames == undefined) {
                $scope.currentObj.RolesNames = [];
            }
            if ($scope.stages[$scope.selectedIndex].Roles == undefined) {
                $scope.stages[$scope.selectedIndex].Roles = [];
            }
            var exists = false;
            for (j = 0; j < $scope.currentObj.RolesNames.length; j++) {
                if (obj.Name == $scope.currentObj.RolesNames[j]) {
                    exists = true;
                    break;
                }
            }
            if (!exists) {
                UserMessagesProvider.displayLoading();
                WebsiteManagerDataProvider.addStageToRole($scope.currentObj, obj).then(function (returnValue) {
                    UserMessagesProvider.hideLoading();
                    if (returnValue.data.result == 'true') {
                        $scope.currentObj.Roles.push({ StageId: $scope.currentObj.Id, RoleId: obj.Id });
                        $scope.stages[$scope.selectedIndex].Roles.push({ StageId: $scope.currentObj.Id, RoleId: obj.Id });
                        $scope.currentObj.RolesNames.push(obj.Name);
                    }
                    else {
                        UserMessagesProvider.errorHandler(999, returnValue.data.message);
                    }
                }, function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(999);
                });
            } else {
                UserMessagesProvider.errorHandler(999, "This role has been already added");
            }
        }, function () {
        });
    }

    $scope.removeStageFromRole = function (index) {
        UserMessagesProvider.confirmHandler("Are you sure you want to remove the stage from this role?", function () {
            UserMessagesProvider.displayLoading();
            WebsiteManagerDataProvider.removeStageFromRole($scope.currentObj, $scope.currentObj.RolesNames[index]).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    $scope.currentObj.RolesNames.splice(index, 1);
                    $scope.stages[$scope.selectedIndex].Roles.splice(index, 1);
                    $scope.currentObj.Roles.splice(index, 1);
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

    $scope.createStage = function () {
        if ($scope.createForm.$valid) {
            UserMessagesProvider.displayLoading();
            WebsiteManagerDataProvider.createStage($scope.currentObj).then(function (returnValue1) {
                $scope.stages.push(returnValue.data.obj);
                $scope.currentObj = returnValue.data.obj;
                $scope.state = $scope.DetailsPanelStates.CreatedStage;
                UserMessagesProvider.hideLoading();
            }, function (data, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        } else {
            UserMessagesProvider.invalidHandler();
        }

    }

    $scope.editStage = function () {
        console.log($scope.editForm);
        if ($scope.editForm.$valid) {
            UserMessagesProvider.displayLoading();
            WebsiteManagerDataProvider.updateStage($scope.currentObj).then(function (returnValue) {
                if (returnValue.data.result == 'true') {
                    $scope.stages[$scope.selectedIndex] = $scope.currentObj;
                    UserMessagesProvider.hideLoading();
                } else {
                    UserMessagesProvider.errorHandler(999, returnValue.data.message);
                    UserMessagesProvider.hideLoading();
                }
            }, function (data, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });

        } else {
            UserMessagesProvider.invalidHandler();
        }
    }

    $scope.deleteStage = function (index) {
        if (!filterFilter($scope.stages, $scope.searchText)[index].BuiltInApp) {
            UserMessagesProvider.confirmHandler("Are you sure you want to delete this stage?", function () {
                UserMessagesProvider.displayLoading();
                var stage = (jQuery.extend(true, {}, filterFilter($scope.stages, $scope.searchText)[index]));
                WebsiteManagerDataProvider.deleteStage(stage).then(function (data) {
                    if (data.data.result == "true") {
                        $scope.closeDetailsForm();
                        $scope.stages.splice(index, 1);
                    }
                    UserMessagesProvider.hideLoading();
                }, function (data, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                });
            }, null);
        } else {
            UserMessagesProvider.errorHandler(999, "Cannot delete this stage because it's in use");
        }
    }

    $scope.saveStages = function () {
        var ids = [];
        for (i = 0; i < $scope.currentObj.NextStages.length; i++) {
            if ($scope.currentObj.NextStages[i].Selected) {
                ids.push($scope.currentObj.NextStages[i].Id);
            }
        }
        UserMessagesProvider.displayLoading();
        console.log($scope.currentObj);
        console.log(ids);
        WebsiteManagerDataProvider.updateNextStages($scope.currentObj, ids).then(function (returnValue) {
            UserMessagesProvider.hideLoading();
            if (returnValue.data.result == 'true') {
                UserMessagesProvider.successHandler();
            }
            else {
                UserMessagesProvider.errorHandler(999, returnValue.data.message);
            }
        }, function (errorData, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });
    }

    initialize();
});