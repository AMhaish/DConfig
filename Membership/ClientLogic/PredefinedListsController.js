﻿appRoot.controllerProvider.register('Membership.PredefinedListsController', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, MembershipDataProvider, EventsProvider, $modal, scopeService) {
    $scope.DetailsPanelStates = {
        None: 0,
        ListDetails: 1,
        ListFields: 2
    };
    $scope.ExecutingContexts = {
        None: 0,
        Deleting: 1,
        Creating: 2,
        Renaming: 3,
        Updating: 4
    };
    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
    $scope.state = $scope.DetailsPanelStates.None;
    var listsTreeContainer = $('#listsTreeContainer');
    var listsSearchBox = $('#listsSearchBox');

    function initialize() {
        BreadCrumpsProvider.breadCrumps.path = ['Membership Manager', 'Predefined Lists'];
        UserMessagesProvider.displayLoading();
        MembershipDataProvider.getUserFieldEnumsTree().success(function (data) {
            $scope.listsTree = data;
            listsTreeContainer
            .on("create_node.jstree", function (e, data) { })
            .on("rename_node.jstree", function (e, data) { scopeService.safeApply($scope, function () { $scope.renameNode(e, data); }); })
            .on("move_node.jstree", function () { })
            .on("copy_node.jstree", function () { })
            .on("cut.jstree", function () { })
            .on("copy.jstree", function () { })
            .on("paste.jstree", function () { })
            .on("changed.jstree", function (e, data) { scopeService.safeApply($scope, function () { $scope.displayDetails(e, data); }); })
            .jstree({
                "plugins": ["contextmenu", "search", "sort", "state", "types", "unique", "wholerow"],
                "core": {
                    "data": data,
                    "check_callback": true,
                    "multiple": false,
                    'themes': {
                        'responsive': true
                    },
                    'check_callback': function (operation, node, node_parent, node_position, more) {
                        // operation can be 'create_node', 'rename_node', 'delete_node', 'move_node' or 'copy_node'
                        // in case of 'rename_node' node_position is filled with the new node name
                        if (operation === 'delete_node' && $scope.CurrentExecutingContext == $scope.ExecutingContexts.None) {
                            UserMessagesProvider.confirmHandler("Are you sure you want to delete this node?", function () {
                                UserMessagesProvider.displayLoading();
                                $scope.CurrentExecutingContext = $scope.ExecutingContexts.Deleting;
                                MembershipDataProvider.deleteUserFieldEnum($scope.currentObj).success(function (returnValue) {
                                    UserMessagesProvider.hideLoading();
                                    if (returnValue.result == 'true') {
                                        listsTreeContainer.jstree(true).delete_node($scope.currentNodeObject);
                                        $scope.closeDetailsForm();
                                    }
                                    else {
                                        UserMessagesProvider.errorHandler(999, returnValue.message);
                                    }
                                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                                })
                                .error(function (errorData, status, headers, config) {
                                    UserMessagesProvider.hideLoading();
                                    UserMessagesProvider.errorHandler(status);
                                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                                });
                            }, null);
                            return false;
                        }
                        return true;
                    }
                },
                "types": {
                    "default": {
                        "icon": "jstree-file"
                    }
                },
                "checkbox": {
                    "whole_node": false,
                    "keep_selected_style": false,
                    "three_state": false
                },
                'contextmenu': {
                    'select_node': true,
                    "items": function (node) {
                        var items = $.jstree.defaults.contextmenu.items();
                        delete items.ccp;
                        delete items.create;
                        return items;
                    }
                },
            });
            $scope.closeDetailsForm();
            //Binding Searchbox
            var to = false;
            listsSearchBox.keyup(function () {
                if (to) { clearTimeout(to); }
                to = setTimeout(function () {
                    var v = listsSearchBox.val();
                    listsTreeContainer.jstree(true).search(v);
                }, 250);
            });
            UserMessagesProvider.hideLoading();
        })
        .error(function (data, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });
    }

    $scope.closeDetailsForm = function () {
        listsTreeContainer.jstree(true).deselect_all(false);
        $scope.currentObj = null;
        $scope.state = $scope.DetailsPanelStates.None;
    }

    $scope.displayDetails = function (e, data) {
        if (data.node != undefined) {
            if (data.node.original != undefined) {
                $scope.currentNodeObject = data.node;
                $scope.currentObj = jQuery.extend(true, {}, $scope.currentNodeObject.original.obj);
                $scope.state = $scope.DetailsPanelStates.ListDetails;
            }
        }
    }

    $scope.createNode = function () {
        $scope.closeDetailsForm();
        $scope.CurrentExecutingContext = $scope.ExecutingContexts.Creating;
        $scope.currentObj = {};
        var modalInstance = $modal.open({
            templateUrl: 'createForm.html',
            controller: createPredefinedListController,
            size: 'lg',
            resolve: {
                UMP: function () { return UserMessagesProvider; }
            }
        });
        modalInstance.result.then(function (newNode) {
            UserMessagesProvider.displayLoading();
            MembershipDataProvider.createUserFieldEnum(newNode).success(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.result == 'true') {
                    listsTreeContainer.jstree(true).create_node('#', { text: newNode.Name, obj: returnValue.obj }, 'last', null, false);
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.message);
                }
                $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
            })
            .error(function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
                $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
            });

        }, function () {
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
        });

    }

    $scope.renameNode = function (e, data) {
        if ($scope.CurrentExecutingContext == $scope.ExecutingContexts.None) {
            if ($scope.currentObj.Name != data.node.text) {
                $scope.currentObj.Name = data.node.text;
                $scope.CurrentExecutingContext = $scope.ExecutingContexts.Renaming;
                $scope.saveChanges();
            }
        }
    }

    $scope.saveChanges = function () {
        if ($scope.detialsForm.$valid) {
            if ($scope.currentObj != null && ($scope.CurrentExecutingContext == $scope.ExecutingContexts.None || $scope.CurrentExecutingContext == $scope.ExecutingContexts.Renaming)) {
                UserMessagesProvider.displayLoading();
                var distNode = listsTreeContainer.jstree(true).get_node($scope.currentNodeObject);
                MembershipDataProvider.updateUserFieldEnum($scope.currentObj).success(function (returnValue) {
                    UserMessagesProvider.hideLoading();
                    if (returnValue.result == 'true') {
                        distNode.original.obj = returnValue.obj;
                        listsTreeContainer.jstree(true).rename_node($scope.currentNodeObject, $scope.currentObj.Name);
                        $scope.closeDetailsForm();
                    }
                    else {
                        UserMessagesProvider.errorHandler(999, returnValue.message);
                        listsTreeContainer.jstree(true).rename_node($scope.currentNodeObject, distNode.original.obj.Name);
                        if ($scope.CurrentExecutingContext == $scope.ExecutingContexts.Renaming) {
                            $scope.currentObj.Name = distNode.original.obj.Name;
                        }
                    }
                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                })
                .error(function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                    listsTreeContainer.jstree(true).rename_node($scope.currentNodeObject, distNode.original.obj.Name);
                    if ($scope.CurrentExecutingContext == $scope.ExecutingContexts.Renaming) {
                        $scope.currentObj.Name = distNode.original.obj.Name;
                    }
                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                });
            }
        } else {
            UserMessagesProvider.invalidHandler();
        }
    }

    $scope.displayGeneralDetails = function () {
        $scope.state = $scope.DetailsPanelStates.ListDetails;
    }

    $scope.displayFields = function () {
        $scope.state = $scope.DetailsPanelStates.ListFields;
    }

    $scope.saveFields = function () {
        UserMessagesProvider.displayLoading();
        var distNode = listsTreeContainer.jstree(true).get_node($scope.currentNodeObject);
        MembershipDataProvider.updateUserFieldEnumValues($scope.currentObj.Id, { Values: $scope.currentObj.Values }).success(function (returnValue) {
            UserMessagesProvider.hideLoading();
            if (returnValue.result == 'true') {
                for (i = 0; i < $scope.currentObj.Values.length; i++) {
                    $scope.currentObj.Values[i].open = false;
                }
                distNode.original.obj = returnValue.obj;
                $scope.closeDetailsForm();
            }
            else {
                UserMessagesProvider.errorHandler(999, returnValue.message);
            }
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
        })
        .error(function (errorData, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
        });
    }

    $scope.addNewField = function () {
        if ($scope.currentObj.Values == undefined) {
            $scope.currentObj.Values = [];
        }
        $scope.currentObj.Values.push({ Name: '', Type: '', open: true });
    }

    $scope.removeField = function (index) {
        if ($scope.currentObj.Values[index].Id == undefined) {
            $scope.currentObj.Values.splice(index, 1);
            return;
        }
        UserMessagesProvider.confirmHandler("Are you sure you want to delete this item?", function () {
            var distNode = listsTreeContainer.jstree(true).get_node($scope.currentNodeObject);
            UserMessagesProvider.displayLoading();
            MembershipDataProvider.deleteUserFieldEnumValue($scope.currentObj.Values[index].Id).success(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.result == 'true') {
                    $scope.currentObj.Values.splice(index, 1);
                    distNode.original.obj.Values.splice(index, 1);
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.message);
                }
                $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
            })
            .error(function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        }, null);
    }

    initialize();
});
var createPredefinedListController = function ($scope, $modalInstance, UMP) {
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