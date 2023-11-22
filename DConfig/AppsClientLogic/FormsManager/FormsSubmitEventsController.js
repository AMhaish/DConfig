angular.module('DConfig').controllerProvider.register('FormsManager.FormsSubmitEventsController', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, FormsManagerDataProvider, EventsProvider, $modal, scopeService) {
    $scope.DetailsPanelStates = {
        None: 0,
        EventDetails: 1
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
    var formsTreeContainer = $('#formsTreeContainer');
    var formsSearchBox = $('formsSearchBox');

    function initialize() {
        BreadCrumpsProvider.breadCrumps.path = ['Forms Manager', 'Submit Events'];
        UserMessagesProvider.displayProgress(3);
        FormsManagerDataProvider.getTemplates().then(function (data) {
            $scope.Templates = data.data;
            UserMessagesProvider.increaseProgress();
        }, function (data, status, headers, config) {
            UserMessagesProvider.increaseProgress();
            UserMessagesProvider.errorHandler(status);
        });
        FormsManagerDataProvider.getFormSubmitEventsTypes().then(function (data) {
            $scope.EventsTypes = data.data;
            UserMessagesProvider.increaseProgress();
        }, function (data, status, headers, config) {
            UserMessagesProvider.increaseProgress();
            UserMessagesProvider.errorHandler(status);
        });
        FormsManagerDataProvider.getForgetFormSubmitEventsTree().then(function (data) {
            $scope.formsTree = data.data;
            formsTreeContainer
                .on("create_node.jstree", function (e, data) { scopeService.safeApply($scope, function () { $scope.createNode(e, data); }); })
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
                                    FormsManagerDataProvider.deleteFormSubmitEmailEvent($scope.currentObj).then(function (returnValue) {
                                        UserMessagesProvider.hideLoading();
                                        if (returnValue.data.result == 'true') {
                                            formsTreeContainer.jstree(true).delete_node($scope.currentNodeObject);
                                            $scope.closeDetailsForm();
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
                                }, null);
                                return false;
                            } else if (operation === 'create_node') {
                                if ($scope.CurrentExecutingContext == $scope.ExecutingContexts.None)
                                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.Creating;
                                else
                                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                            }
                            return true;
                        }
                    },
                    "types": {
                        "default": {
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
                    },
                    'contextmenu': {
                        'select_node': true,
                        "items": function (node) {
                            var items = $.jstree.defaults.contextmenu.items();
                            if (node.type == "Container") {
                                delete items.ccp;
                                delete items.remove;
                                delete items.rename;
                            } else {
                                delete items.ccp;
                                delete items.create;
                            }
                            return items;
                        }
                    },
                });
            $scope.closeDetailsForm();
            //Binding Searchbox
            var to = false;
            formsSearchBox.keyup(function () {
                if (to) { clearTimeout(to); }
                to = setTimeout(function () {
                    var v = formsSearchBox.val();
                    formsTreeContainer.jstree(true).search(v);
                }, 250);
            });
            UserMessagesProvider.increaseProgress();
        }, function (data, status, headers, config) {
            UserMessagesProvider.increaseProgress();
            UserMessagesProvider.errorHandler(status);
        });
    }

    $scope.closeDetailsForm = function () {
        formsTreeContainer.jstree(true).deselect_all(false);
        $scope.currentObj = null;
        $scope.state = $scope.DetailsPanelStates.None;
    }

    $scope.displayDetails = function (e, data) {
        if (data.node != undefined) {
            if (data.node.original != undefined) {
                $scope.currentNodeObject = data.node;
                $scope.currentObj = $scope.currentNodeObject.original.obj;
                if ($scope.currentNodeObject.original.type == "Container") {
                    $scope.state = $scope.DetailsPanelStates.None;
                } else {
                    $scope.state = $scope.DetailsPanelStates.EventDetails;
                    var parentNodeObject = formsTreeContainer.jstree(true).get_node(formsTreeContainer.jstree(true).get_parent(data.node));
                    $scope.parentObject = parentNodeObject.original.obj;
                }
            }
        }
    }

    $scope.createNode = function (e, data) {
        if ($scope.CurrentExecutingContext == $scope.ExecutingContexts.Creating) {
            var parentNodeObject;
            parentNodeObject = formsTreeContainer.jstree(true).get_node(formsTreeContainer.jstree(true).get_parent(data.node));
            $scope.parentObject = parentNodeObject.original.obj;
            $scope.currentObj = {};
            var modalInstance = $modal.open({
                templateUrl: 'createForm.html',
                controller: createEventController,
                size: 'lg',
                resolve: {
                    parent: function () {
                        return $scope.parentObject;
                    },
                    UMP: function () { return UserMessagesProvider; },
                    EventsTypes: function () {
                        return $scope.EventsTypes;
                    },
                    Templates: function () {
                        return $scope.Templates;
                    }
                }
            });
            modalInstance.result.then(function (newNode) {
                UserMessagesProvider.displayLoading();
                FormsManagerDataProvider.createFormSubmitEmailEvent(newNode).then(function (returnValue) {
                    UserMessagesProvider.hideLoading();
                    if (returnValue.data.result == 'true') {
                        var distNode = formsTreeContainer.jstree(true).get_node(data.node);
                        formsTreeContainer.jstree(true).rename_node(data.node, newNode.Name);
                        distNode.original.obj = returnValue.data.obj;
                        formsTreeContainer.jstree(true).deselect_node(parentNodeObject, false);
                        formsTreeContainer.jstree(true).select_node(data.node, false);
                    }
                    else {
                        UserMessagesProvider.errorHandler(999, returnValue.data.message);
                        formsTreeContainer.jstree(true).delete_node(data.node);
                    }
                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                }, function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                    formsTreeContainer.jstree(true).delete_node(data.node);
                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                });

            }, function () {
                formsTreeContainer.jstree(true).delete_node(data.node);
                $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
            });
        }
        else {
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
        }
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
                var distNode = formsTreeContainer.jstree(true).get_node($scope.currentNodeObject);
                FormsManagerDataProvider.updateFormSubmitEmailEvent($scope.currentObj).then(function (returnValue) {
                    UserMessagesProvider.hideLoading();
                    if (returnValue.data.result == 'true') {
                        distNode.original.obj = returnValue.data.obj;
                        formsTreeContainer.jstree(true).rename_node($scope.currentNodeObject, $scope.currentObj.Name);
                        $scope.closeDetailsForm();
                    }
                    else {
                        UserMessagesProvider.errorHandler(999, returnValue.data.message);
                        formsTreeContainer.jstree(true).rename_node($scope.currentNodeObject, distNode.original.obj.Name);
                        if ($scope.CurrentExecutingContext == $scope.ExecutingContexts.Renaming) {
                            $scope.currentObj.Name = distNode.original.obj.Name;
                        }
                    }
                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                }, function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                    formsTreeContainer.jstree(true).rename_node($scope.currentNodeObject, distNode.original.obj.Name);
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


    initialize();
});
var createEventController = function ($scope, $modalInstance, UMP, EventsTypes, parent, Templates) {
    $scope.newNode = { FormId: parent.Id };
    $scope.parentObject = parent;
    $scope.createForm = {};
    $scope.EventsTypes = EventsTypes;
    $scope.Templates = Templates;
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