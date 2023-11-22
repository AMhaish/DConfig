angular.module('DConfig').controllerProvider.register('WebsiteManager.ViewTypesTreeController', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, WebsiteManagerDataProvider, EventsProvider, $modal, scopeService) {
    $scope.DetailsPanelStates = {
        None: 0,
        ViewTypeDetails: 1,
        ViewTypeFields: 2,
        ViewTypeChildren: 3
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
    $scope.isOpen = false;

    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
    $scope.state = $scope.DetailsPanelStates.None;
    var viewTypesTreeContainer = $('#viewTypesTreeContainer');
    var viewTypesSearchBox = $('#viewTypesSearchBox');

    function initialize() {
        BreadCrumpsProvider.breadCrumps.path = ['Website Manager', 'View Types'];
        UserMessagesProvider.displayProgress(4);
        WebsiteManagerDataProvider.getViewFieldTypes().then(function (data) {
            $scope.FieldsTypes = data.data;
            UserMessagesProvider.increaseProgress();
        }, function (data, status, headers, config) {
            UserMessagesProvider.errorHandler(status);
            UserMessagesProvider.increaseProgress();
        });
        WebsiteManagerDataProvider.getViewFieldsEnums().then(function (data) {
            $scope.Enums = data.data;
            UserMessagesProvider.increaseProgress();
        }, function (data, status, headers, config) {
            UserMessagesProvider.increaseProgress();
            UserMessagesProvider.errorHandler(status);
        });
        WebsiteManagerDataProvider.getViewTypes().then(function (data) {
            $scope.viewTypes = data.data;
            UserMessagesProvider.increaseProgress();
        }, function (data, status, headers, config) {
            UserMessagesProvider.errorHandler(status);
            UserMessagesProvider.increaseProgress();
        });
        WebsiteManagerDataProvider.getViewTypesTree().then(function (data) {
            $scope.viewTypesTree = data.data;
            console.log($scope.viewTypesTree);
            viewTypesTreeContainer
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
                        "data": data.data,
                        "multiple": false,
                        'themes': {
                            'responsive': true
                        },
                        'check_callback': function (operation, node, node_parent, node_position, more) {
                            // operation can be 'create_node', 'rename_node', 'delete_node', 'move_node' or 'copy_node'
                            // in case of 'rename_node' node_position is filled with the new node name
                            if (operation === 'delete_node' && $scope.CurrentExecutingContext === $scope.ExecutingContexts.None) {
                                UserMessagesProvider.confirmHandler("Are you sure you want to delete this node?", function () {
                                    UserMessagesProvider.displayLoading();
                                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.Deleting;
                                    WebsiteManagerDataProvider.deleteViewType($scope.currentObj).then(function (returnValue) {
                                        UserMessagesProvider.hideLoading();
                                        if (returnValue.data.result === 'true') {
                                            for (i = 0; i < $scope.viewTypes.length; i++) {
                                                if ($scope.viewTypes[i].Id == $scope.currentObj.Id) {
                                                    $scope.viewTypes.splice(i, 1);
                                                    break;
                                                }
                                            }
                                            viewTypesTreeContainer.jstree(true).delete_node($scope.currentNodeObject);
                                            $scope.closeDetailsForm();
                                        }
                                        else {
                                            UserMessagesProvider.errorHandler(999, returnValue.message);
                                        }
                                        $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                                    }, function (errorData, status, headers, config) {
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
            viewTypesSearchBox.keyup(function () {
                if (to) { clearTimeout(to); }
                to = setTimeout(function () {
                    var v = viewTypesSearchBox.val();
                    viewTypesTreeContainer.jstree(true).search(v);
                }, 250);
            });
            UserMessagesProvider.increaseProgress();
        }, function (data, status, headers, config) {
            UserMessagesProvider.increaseProgress();
            UserMessagesProvider.errorHandler(status);
        });
    }

    $scope.closeDetailsForm = function () {
        viewTypesTreeContainer.jstree(true).deselect_all(false);
        $scope.currentObj = null;
        $scope.state = $scope.DetailsPanelStates.None;
    }

    $scope.displayDetails = function (e, data) {
        if (data.node != undefined) {
            if (data.node.original != undefined) {
                $scope.currentNodeObject = data.node;
                $scope.currentObj = jQuery.extend(true, {}, $scope.currentNodeObject.original.obj);
                $scope.state = $scope.DetailsPanelStates.ViewTypeDetails;
            }
        }
    }

    $scope.createNode = function () {
        $scope.CurrentExecutingContext = $scope.ExecutingContexts.Creating;
        $scope.currentObj = {};
        var modalInstance = $modal.open({
            templateUrl: 'createForm.html',
            controller: createViewTypeController,
            size: 'lg',
            resolve: {
                UMP: function () { return UserMessagesProvider; }
            }
        });
        modalInstance.result.then(function (newNode) {
            UserMessagesProvider.displayLoading();
            WebsiteManagerDataProvider.createViewType(newNode).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    viewTypesTreeContainer.jstree(true).create_node('#', { text: newNode.Name, obj: returnValue.data.obj }, 'last', null, false);
                    UserMessagesProvider.successHandler();
                    WebsiteManagerDataProvider.getViewTypes().then(function (data) {
                        $scope.viewTypes = data.data;
                    }, function (data, status, headers, config) {
                        UserMessagesProvider.errorHandler(status);
                    });
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.message);
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
        console.log($scope.currentObj);
        if ($scope.detialsForm.$valid) {
            if ($scope.currentObj != null && ($scope.CurrentExecutingContext == $scope.ExecutingContexts.None || $scope.CurrentExecutingContext == $scope.ExecutingContexts.Renaming)) {
                UserMessagesProvider.displayLoading();
                var distNode = viewTypesTreeContainer.jstree(true).get_node($scope.currentNodeObject);
                WebsiteManagerDataProvider.updateViewType($scope.currentObj).then(function (returnValue) {
                    UserMessagesProvider.hideLoading();
                    if (returnValue.data.result == 'true') {
                        distNode.original.obj = returnValue.data.obj;
                        viewTypesTreeContainer.jstree(true).rename_node($scope.currentNodeObject, $scope.currentObj.Name);
                        for (i = 0; i < $scope.viewTypes.length; i++) {
                            if ($scope.viewTypes[i].Id == distNode.original.obj.Id) {
                                $scope.viewTypes[i].Name = distNode.original.obj.Name;
                            }
                        }
                        UserMessagesProvider.successHandler();
                    }
                    else {
                        UserMessagesProvider.errorHandler(999, returnValue.data.message);
                        viewTypesTreeContainer.jstree(true).rename_node($scope.currentNodeObject, distNode.original.obj.Name);
                        if ($scope.CurrentExecutingContext == $scope.ExecutingContexts.Renaming) {
                            $scope.currentObj.Name = distNode.original.obj.Name;
                        }
                    }
                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                }, function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                    viewTypesTreeContainer.jstree(true).rename_node($scope.currentNodeObject, distNode.original.obj.Name);
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
        $scope.state = $scope.DetailsPanelStates.ViewTypeDetails;
    }

    $scope.displayFields = function () {
        $scope.state = $scope.DetailsPanelStates.ViewTypeFields;
    }

    $scope.displayChildrenTypes = function () {
        if ($scope.currentObj) {
            UserMessagesProvider.displayLoading();
            WebsiteManagerDataProvider.getViewTypeChildren($scope.currentObj).then(function (data) {
                $scope.state = $scope.DetailsPanelStates.ViewTypeChildren;
                $scope.currentObj.Children = jQuery.extend(true, [], $scope.viewTypes);
                if (data.data.length > 0) {
                    for (j = 0; j < $scope.currentObj.Children.length; j++) {
                        for (i = 0; i < data.data.length; i++) {
                            if ($scope.currentObj.Children[j].Id == data.data[i].Id) {
                                $scope.currentObj.Children[j].Selected = true;
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

    $scope.saveFields = function () {
        UserMessagesProvider.displayLoading();
        var distNode = viewTypesTreeContainer.jstree(true).get_node($scope.currentNodeObject);
        for (i = 0; i < $scope.currentObj.ViewFields.length; i++) {
            $scope.currentObj.ViewFields[i].Priority = i;
        }
        console.log($scope.currentObj.ViewFields);
        WebsiteManagerDataProvider.updateViewTypeFields($scope.currentObj).then(function (returnValue) {
            UserMessagesProvider.hideLoading();
            if (returnValue.data.result == 'true') {
                for (i = 0; i < $scope.currentObj.ViewFields.length; i++) {
                    $scope.currentObj.ViewFields[i].open = false;
                }
                distNode.original.obj = returnValue.data.obj;
                $scope.currentObj = jQuery.extend(true, {}, $scope.currentNodeObject.original.obj);
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

    $scope.saveChildren = function () {
        var ids = [];
        for (i = 0; i < $scope.currentObj.Children.length; i++) {
            if ($scope.currentObj.Children[i].Selected) {
                ids.push($scope.currentObj.Children[i].Id);
            }
        }
        UserMessagesProvider.displayLoading();
        WebsiteManagerDataProvider.updateViewTypeChildren($scope.currentObj, { ChildrenIds: ids }).then(function (returnValue) {
            UserMessagesProvider.hideLoading();
            if (returnValue.data.result == 'true') {
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

    $scope.addNewField = function () {
        if ($scope.currentObj.ViewFields == undefined) {
            $scope.currentObj.ViewFields = [];
        }
        $scope.currentObj.ViewFields.push({ Name: '', Type: '', open: true });
    }

    $scope.removeField = function (index) {
        if ($scope.currentObj.ViewFields[index].Id == undefined) {
            $scope.currentObj.ViewFields.splice(index, 1);
            return;
        }
        UserMessagesProvider.confirmHandler("Deleting view type field will causing all the contents data linked with this field to be lost, are you sure you want to delete it?", function () {
            var distNode = viewTypesTreeContainer.jstree(true).get_node($scope.currentNodeObject);
            UserMessagesProvider.displayLoading();
            WebsiteManagerDataProvider.deleteViewTypeField($scope.currentObj.ViewFields[index].Id).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    $scope.currentObj.ViewFields.splice(index, 1);
                    distNode.original.obj.ViewFields.splice(index, 1);
                    UserMessagesProvider.successHandler();
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.message);
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
var createViewTypeController = function ($scope, $modalInstance, UMP) {
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