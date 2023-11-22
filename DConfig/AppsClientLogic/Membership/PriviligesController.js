angular.module('DConfig').controllerProvider.register('Membership.PriviligesController', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, MembershipDataProvider, EventsProvider, $modal, scopeService) {
    $scope.DetailsPanelStates = {
        None: 0,
        PrivilegeDetails: 1,
        Roles: 2
    };
    $scope.state = $scope.DetailsPanelStates.None;
    var privilegesTreeContainer = $('#privilegesTreeContainer');
    var privilegesSearchBox = $('#privilegesSearchBox');

    function initialize() {
        BreadCrumpsProvider.breadCrumps.path = ['Membership Manager', 'Privileges'];
        UserMessagesProvider.displayLoading();
        MembershipDataProvider.getRoles().then(function (data) {
            $scope.roles = data.data;
        }, function (data, status, headers, config) {
            UserMessagesProvider.errorHandler(status);
        });
        MembershipDataProvider.getSystemControllersTree().then(function (data) {
            $scope.privilegesTree = data.data;
            privilegesTreeContainer
                .on("create_node.jstree", function (e, data) { })
                .on("rename_node.jstree", function (e, data) { })
                .on("move_node.jstree", function () { })
                .on("copy_node.jstree", function () { })
                .on("cut.jstree", function () { })
                .on("copy.jstree", function () { })
                .on("paste.jstree", function () { })
                .on("changed.jstree", function (e, data) { scopeService.safeApply($scope, function () { $scope.displayDetails(e, data); }); })
                .jstree({
                    "plugins": ["search", "sort", "state", "types", "unique", "wholerow"],
                    "core": {
                        "data": data.data,
                        "check_callback": true,
                        "multiple": false,
                        'themes': {
                            'responsive': true
                        },
                        'check_callback': function (operation, node, node_parent, node_position, more) {
                            // operation can be 'create_node', 'rename_node', 'delete_node', 'move_node' or 'copy_node'
                            // in case of 'rename_node' node_position is filled with the new node name
                            return true;
                        }
                    },
                    "types": {
                        "Item": {
                            "icon": "jstree-file"
                        },
                        "Container": {
                            "icon": "jstree-folder"
                        }
                    },
                    "checkbox": {
                        "whole_node": false,
                        "keep_selected_style": false,
                        "three_state": false
                    }
                });
            $scope.closeDetailsForm();
            //Binding Searchbox
            var to = false;
            privilegesSearchBox.keyup(function () {
                if (to) { clearTimeout(to); }
                to = setTimeout(function () {
                    var v = privilegesSearchBox.val();
                    privilegesTreeContainer.jstree(true).search(v);
                }, 250);
            });
            UserMessagesProvider.hideLoading();
        }, function (data, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });
    }

    $scope.closeDetailsForm = function () {
        privilegesTreeContainer.jstree(true).deselect_all(false);
        $scope.currentObj = null;
        $scope.state = $scope.DetailsPanelStates.None;
    }

    $scope.displayDetails = function (e, data) {
        if (data.node != undefined) {
            if (data.node.original != undefined) {
                $scope.currentNodeObject = data.node;
                $scope.currentObj = jQuery.extend(true, {}, $scope.currentNodeObject.original.obj);
                if ($scope.currentNodeObject.original.type != "Container") {
                    $scope.parentNodeObject = privilegesTreeContainer.jstree(true).get_node(privilegesTreeContainer.jstree(true).get_parent($scope.currentNodeObject));
                    if ($scope.currentNodeObject.original.addObj == null) {
                        if ($scope.parentNodeObject.original.addObj != null) {
                            $scope.currentPrv = jQuery.extend(true, {}, $scope.parentNodeObject.original.addObj);
                            $scope.sameAsParent = true;
                        } else {
                            $scope.sameAsParent = false;
                        }
                    } else {
                        $scope.currentPrv = jQuery.extend(true, {}, $scope.currentNodeObject.original.addObj);
                        $scope.sameAsParent = false;
                    }
                } else {
                    $scope.sameAsParent = false;
                    $scope.currentPrv = jQuery.extend(true, {}, $scope.currentNodeObject.original.addObj);
                }
                if ($scope.currentPrv == null) {
                    $scope.currentPrv = {};
                }
                $scope.state = $scope.DetailsPanelStates.PrivilegeDetails;
            }
        }
    }

    $scope.updatePrivilege = function () {
        UserMessagesProvider.displayLoading();
        var distNode = privilegesTreeContainer.jstree(true).get_node($scope.currentNodeObject);
        if (!$scope.currentPrv.NeedAuthentication) {
            $scope.currentPrv.NeedAuthorization = false;
        }
        if ($scope.currentNodeObject.original.addObj == null || $scope.sameAsParent) {
            var controllerName = '';
            var actionName = '';
            var requestType = '';
            if ($scope.currentNodeObject.original.type == "Container") {
                controllerName = $scope.currentObj.Name;
            } else {
                actionName = $scope.currentObj.Name;
                controllerName = $scope.parentNodeObject.original.obj.Name;
            }
            MembershipDataProvider.createPrivilege($scope.currentPrv, controllerName, actionName, requestType).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    distNode.original.addObj = returnValue.data.obj;
                    $scope.closeDetailsForm();
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.data.message);
                    $scope.currentPrv = {};
                }
            }, function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        } else {
            var becomeSameAsParent = $scope.parentNodeObject != undefined && $scope.parentNodeObject.original.addObj.NeedAuthentication == $scope.currentPrv.NeedAuthentication && $scope.parentNodeObject.original.addObj.NeedAuthorization == $scope.currentPrv.NeedAuthorization && $scope.parentNodeObject.original.addObj.RequireHttps == $scope.currentPrv.RequireHttps;
            var sameRoles = true;
            if (becomeSameAsParent) {
                for (i = 0; i < $scope.parentNodeObject.original.addObj.Roles; i++) {
                    sameRoles = false;
                    for (j = 0; j < $scope.$scope.currentNodeObject.original.addObj.Roles; j++) {
                        if ($scope.parentNodeObject.original.addObj.Roles[i] == $scope.$scope.currentNodeObject.original.addObj.Roles[j]) {
                            sameRoles = true;
                            break;
                        }
                    }
                }
            }
            if (becomeSameAsParent && sameRoles) {
                MembershipDataProvider.deletePrivilege($scope.currentPrv).then(function (returnValue) {
                    UserMessagesProvider.hideLoading();
                    if (returnValue.data.result == 'true') {
                        distNode.original.addObj = null;
                        $scope.closeDetailsForm();
                    }
                    else {
                        UserMessagesProvider.errorHandler(999, returnValue.data.message);
                        $scope.currentPrv = jQuery.extend(true, {}, $scope.currentNodeObject.original.addObj);
                    }
                }, function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                });
            } else {
                MembershipDataProvider.updatePrivilege($scope.currentPrv).then(function (returnValue) {
                    UserMessagesProvider.hideLoading();
                    if (returnValue.data.result == 'true') {
                        distNode.original.addObj = returnValue.data.obj;
                        $scope.closeDetailsForm();
                    }
                    else {
                        UserMessagesProvider.errorHandler(999, returnValue.data.message);
                        $scope.currentPrv = jQuery.extend(true, {}, $scope.currentNodeObject.original.addObj);
                    }
                }, function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                });
            }
        }
    }

    $scope.addPrivilegeToRole = function () {
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
            var distNode = privilegesTreeContainer.jstree(true).get_node($scope.currentNodeObject);
            if ($scope.currentNodeObject.original.type == "Container" || $scope.sameAsParent == false) {
                var exists = false;
                if ($scope.currentPrv.Roles != undefined) {
                    for (j = 0; j < $scope.currentPrv.Roles.length; j++) {
                        if (obj.Name == $scope.currentPrv.Roles[j].Name) {
                            exists = true;
                            break;
                        }
                    }
                } else {
                    $scope.currentPrv.Roles = [];
                }
                if (!exists) {
                    UserMessagesProvider.displayLoading();
                    MembershipDataProvider.addPrivilegeToRole($scope.currentPrv.Id, obj.Name).then(function (returnValue) {
                        UserMessagesProvider.hideLoading();
                        if (returnValue.data.result == 'true') {
                            $scope.currentPrv.Roles.push(obj);
                            distNode.original.addObj.Roles.push(obj);
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
            } else {
                var requestType = '';
                var actionName = $scope.currentObj.Name;
                var controllerName = $scope.parentNodeObject.original.obj.Name;
                UserMessagesProvider.displayLoading();
                MembershipDataProvider.createPrivilege($scope.currentPrv, controllerName, actionName, requestType).then(function (returnValue) {
                    if (returnValue.data.result == 'true') {
                        distNode.original.addObj = returnValue.data.obj;
                        $scope.sameAsParent = false;
                        $scope.currentPrv = jQuery.extend(true, {}, returnValue.obj);
                        MembershipDataProvider.addPrivilegeToRole($scope.currentPrv.Id, obj.Name).then(function (returnValue) {
                            UserMessagesProvider.hideLoading();
                            if (returnValue.data.result == 'true') {
                                if ($scope.currentPrv.Roles == undefined) {
                                    $scope.currentPrv.Roles = [];
                                }
                                $scope.currentPrv.Roles.push(obj);
                                distNode.original.addObj.Roles.push(obj);
                            }
                            else {
                                UserMessagesProvider.errorHandler(999, returnValue.data.message);
                            }
                        }, function (errorData, status, headers, config) {
                            UserMessagesProvider.hideLoading();
                            UserMessagesProvider.errorHandler(999);
                        });
                    }
                    else {
                        UserMessagesProvider.errorHandler(999, returnValue.message);
                        $scope.currentPrv = {};
                    }
                }, function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                });
            }
        }, function () {
        });
    }

    $scope.removePrivilegeFromRole = function (index) {
        UserMessagesProvider.confirmHandler("Are you sure you want to remove the user from this role?", function () {
            var distNode = privilegesTreeContainer.jstree(true).get_node($scope.currentNodeObject);
            if ($scope.currentNodeObject.original.type == "Container" || $scope.sameAsParent == false) {
                UserMessagesProvider.displayLoading();
                MembershipDataProvider.removePrivilegeFromRole($scope.currentPrv.Id, $scope.currentPrv.Roles[index].Name).then(function (returnValue) {
                    UserMessagesProvider.hideLoading();
                    if (returnValue.data.result == 'true') {
                        distNode.original.addObj.Roles.splice(index, 1);
                        $scope.currentPrv.Roles.splice(index, 1);
                    }
                    else {
                        UserMessagesProvider.errorHandler(999, returnValue.message);
                    }
                }, function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(999);
                });
            } else {
                var requestType = '';
                var actionName = $scope.currentObj.Name;
                var controllerName = $scope.parentNodeObject.original.obj.Name;
                UserMessagesProvider.displayLoading();
                MembershipDataProvider.createPrivilege($scope.currentPrv, controllerName, actionName, requestType).then(function (returnValue) {
                    if (returnValue.data.result == 'true') {
                        distNode.original.addObj = returnValue.data.obj;
                        $scope.sameAsParent = false;
                        $scope.currentPrv = jQuery.extend(true, {}, returnValue.obj);
                        MembershipDataProvider.removePrivilegeFromRole($scope.currentPrv.Id, $scope.currentPrv.Roles[index].Name).then(function (returnValue) {
                            UserMessagesProvider.hideLoading();
                            if (returnValue.data.result == 'true') {
                                distNode.original.addObj.Roles.splice(index, 1);
                                $scope.currentPrv.Roles.splice(index, 1);
                            }
                            else {
                                UserMessagesProvider.errorHandler(999, returnValue.message);
                            }
                        }, function (errorData, status, headers, config) {
                            UserMessagesProvider.hideLoading();
                            UserMessagesProvider.errorHandler(999);
                        });
                    }
                    else {
                        UserMessagesProvider.errorHandler(999, returnValue.message);
                        $scope.currentPrv = {};
                    }
                }, function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                });
            }
        }, null);
    }

    $scope.displayRoles = function () {
        $scope.state = $scope.DetailsPanelStates.Roles;
    }

    $scope.displayGeneralDetails = function () {
        $scope.state = $scope.DetailsPanelStates.PrivilegeDetails;
    }

    initialize();
});