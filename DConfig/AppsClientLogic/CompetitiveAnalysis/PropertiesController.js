angular.module('DConfig').controllerProvider.register('CompetitiveAnalysis.PropertiesController', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, CompetitiveAnalysisDataProvider, EventsProvider, $modal, scopeService, filterFilter) {
    $scope.DetailsPanelStates = {
        None: 0,
        GroupDetails: 1,
        Properties: 2
    };
    $scope.ExecutingContexts = {
        None: 0,
        Deleting: 1,
        Creating: 2,
        Renaming: 3,
        Updating: 4
    };
    $scope.SortableOptions = {
        //items: '.panel',
        handle: '.handle'
    };
    $scope.groupsSortingEnabled = false;
    $scope.GroupsSortableOptions = {
        //items: '.panel',
        handle: '.handle',
        stop: function (event, ui) {
            $scope.closeDetailsForm();
            $scope.groupsSortingEnabled = true;
        }
    };

    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
    $scope.state = $scope.DetailsPanelStates.None;
    //var groupsTreeContainer = $('#groupsTreeContainer');
    //var formsSearchBox = $('formsSearchBox');

    function initialize() {
        BreadCrumpsProvider.breadCrumps.path = ['Competitive Analysis', 'Properties'];
        UserMessagesProvider.displayProgress(3);
        CompetitiveAnalysisDataProvider.getPropertiesTypes().then(function (data) {
            $scope.PropertiesTypes = data.data;
            UserMessagesProvider.increaseProgress();
        }, function (data, status, headers, config) {
            UserMessagesProvider.increaseProgress();
            UserMessagesProvider.errorHandler(status);
        });
        CompetitiveAnalysisDataProvider.getPropertiesEnums().then(function (data) {
            $scope.Enums = data.data;
            UserMessagesProvider.increaseProgress();
        }, function (data, status, headers, config) {
            UserMessagesProvider.increaseProgress();
            UserMessagesProvider.errorHandler(status);
        });
        CompetitiveAnalysisDataProvider.getPropertiesGroups().then(function (data) {
            $scope.groupsTree = data.data;
            UserMessagesProvider.increaseProgress();
        }, function (data, status, headers, config) {
            UserMessagesProvider.increaseProgress();
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
            $scope.currentObj = jQuery.extend(true, {}, filterFilter($scope.groupsTree, $scope.searchText)[index]);
            $scope.state = $scope.DetailsPanelStates.Properties;
        }
    }

    $scope.updateGroupsOrder = function () {
        UserMessagesProvider.displayLoading();
        for (i = 0; i < $scope.groupsTree.length; i++) {
            $scope.groupsTree[i].Priority = i;
        }
        CompetitiveAnalysisDataProvider.updatePropertiesGroupsOrder($scope.groupsTree).then(function (returnValue) {
            UserMessagesProvider.hideLoading();
            if (returnValue.data.result == 'true') {
                $scope.closeDetailsForm();
                $scope.groupsSortingEnabled = false;
                UserMessagesProvider.successHandler();
            }
            else {
                UserMessagesProvider.errorHandler(999, returnValue.data.message);
            }
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
        }, function (errorData, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });
    }

    $scope.deleteGroup = function (index) {
        $scope.selectedIndex = index;
        $scope.currentObj = $scope.groupsTree[index];
        UserMessagesProvider.confirmHandler("Are you sure you want to delete this group?", function () {
            UserMessagesProvider.displayLoading();
            CompetitiveAnalysisDataProvider.deletePropertiesGroup($scope.currentObj).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    $scope.closeDetailsForm();
                    $scope.groupsTree.splice(index, 1);
                    UserMessagesProvider.successHandler();
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

    $scope.createRootNode = function () {
        $scope.CurrentExecutingContext = $scope.ExecutingContexts.Creating;
        $scope.currentObj = {};
        var modalInstance = $modal.open({
            templateUrl: 'createForm.html',
            controller: createGroupController,
            size: 'lg',
            resolve: {
                UMP: function () { return UserMessagesProvider; }
            }
        });
        modalInstance.result.then(function (newNode) {
            UserMessagesProvider.displayLoading();
            CompetitiveAnalysisDataProvider.createPropertiesGroup(newNode).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    $scope.groupsTree.push(returnValue.data.obj);
                    UserMessagesProvider.successHandler();
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.data.message);
                }
                $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
            }, function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
                $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
            });

        }, function () {
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
        });

    }

    $scope.saveChanges = function () {
        if ($scope.detialsForm.$valid) {
            if ($scope.currentObj != null && ($scope.CurrentExecutingContext == $scope.ExecutingContexts.None || $scope.CurrentExecutingContext == $scope.ExecutingContexts.Renaming)) {
                UserMessagesProvider.displayLoading();
                CompetitiveAnalysisDataProvider.updatePropertiesGroup($scope.currentObj).then(function (returnValue) {
                    UserMessagesProvider.hideLoading();
                    if (returnValue.data.result == 'true') {
                        $scope.groupsTree[$scope.selectedIndex] = returnValue.data.obj;
                        $scope.currentObj = returnValue.data.obj;
                        UserMessagesProvider.successHandler();
                    }
                    else {
                        UserMessagesProvider.errorHandler(999, returnValue.data.message);
                    }
                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                }, function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                });
            }
        } else {
            UserMessagesProvider.invalidHandler();
        }
    }

    $scope.displayGeneralDetails = function () {
        $scope.state = $scope.DetailsPanelStates.GroupDetails;
    }

    $scope.displayProperties = function () {
        $scope.state = $scope.DetailsPanelStates.Properties;
    }

    $scope.saveProperties = function () {
        UserMessagesProvider.displayLoading();
        for (i = 0; i < $scope.currentObj.Properties.length; i++) {
            $scope.currentObj.Properties[i].Priority = i;
        }
        CompetitiveAnalysisDataProvider.updateGroupProperties($scope.currentObj).then(function (returnValue) {
            UserMessagesProvider.hideLoading();
            if (returnValue.data.result == 'true') {
                for (i = 0; i < $scope.currentObj.Properties.length; i++) {
                    $scope.currentObj.Properties[i].open = false;
                }
                $scope.groupsTree[$scope.selectedIndex] = returnValue.data.obj;
                UserMessagesProvider.successHandler();
            }
            else {
                UserMessagesProvider.errorHandler(999, returnValue.data.message);
            }
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
        }, function (errorData, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
        });
    }

    $scope.addNewProperty = function () {
        if ($scope.currentObj.Properties == undefined) {
            $scope.currentObj.Properties = [];
        }
        $scope.currentObj.Properties.push({ Name: '', Type: '', open: true });
    }

    $scope.removeProperty = function (index) {
        if ($scope.currentObj.Properties[index].Id == undefined) {
            $scope.currentObj.Properties.splice(index, 1);
            return;
        }
        UserMessagesProvider.confirmHandler("Deleting property will causing all the products properties linked with this property to be lost, are you sure you want to delete it?", function () {
            UserMessagesProvider.displayLoading();
            CompetitiveAnalysisDataProvider.deleteGroupProperty($scope.currentObj.Properties[index].Id).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    $scope.currentObj.Properties.splice(index, 1);
                    $scope.groupsTree[index].Properties.splice(index, 1);
                    UserMessagesProvider.successHandler();
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.data.message);
                }
                $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
            }, function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        }, null);
    }

    initialize();
});
var createGroupController = function ($scope, $modalInstance, UMP) {
    $scope.newNode = {};
    $scope.createForm = {};
    $scope.ok = function (createForm) {
        if ($scope.createForm.form.$valid) {
            $modalInstance.close($scope.newNode);
        } else {
            UMP.invalidHandler();
        }
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};