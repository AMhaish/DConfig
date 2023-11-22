angular.module('DConfig').controllerProvider.register('WebsiteManager.StylesTreeController', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, WebsiteManagerDataProvider, EventsProvider, $modal, scopeService, $upload, $route) {
    $scope.DetailsPanelStates = {
        None: 0,
        StyleDetails: 1,
        StyleCode: 2
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
    var stylesTreeContainer = $('#stylesTreeContainer');
    var stylesSearchBox = $('#stylesSearchBox');
    var codeArea;

    function initialize() {
        BreadCrumpsProvider.breadCrumps.path = ['Website Manager', 'Styles'];
        UserMessagesProvider.displayLoading();
        WebsiteManagerDataProvider.getStylesTree().then(function (data) {
            $scope.stylesTree = data.data;
            stylesTreeContainer
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
                                    var distNode = stylesTreeContainer.jstree(true).get_node($scope.currentNodeObject);
                                    if (distNode.type == 'Container') {
                                        WebsiteManagerDataProvider.deleteStylesBundle($scope.currentObj).then(function (returnValue) {
                                            UserMessagesProvider.hideLoading();
                                            if (returnValue.data.result == 'true') {
                                                stylesTreeContainer.jstree(true).delete_node($scope.currentNodeObject);
                                                $scope.closeDetailsForm();
                                                UserMessagesProvider.successHandler();
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
                                    } else {
                                        WebsiteManagerDataProvider.deleteStyle($scope.currentObj).then(function (returnValue) {
                                            UserMessagesProvider.hideLoading();
                                            if (returnValue.data.result == 'true') {
                                                stylesTreeContainer.jstree(true).delete_node($scope.currentNodeObject);
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
                                    }
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
                    'sort': function (a, b) {
                        var a_obj = this.get_node(a).original;
                        var b_obj = this.get_node(b).original;
                        if (a_obj.obj == undefined || b_obj.obj == undefined) {
                            return -1;
                        }
                        return a_obj.obj.Priority > b_obj.obj.Priority ? 1 : -1;
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
                    },
                    'contextmenu': {
                        'select_node': true,
                        "items": function (node) {
                            var items = $.jstree.defaults.contextmenu.items();
                            if (node.type == "Item") {
                                delete items.ccp;
                                delete items.create;
                            } else {
                                delete items.ccp;
                            }
                            return items;
                        }
                    },
                });
            $scope.closeDetailsForm();
            //Binding Searchbox
            var to = false;
            stylesSearchBox.keyup(function () {
                if (to) { clearTimeout(to); }
                to = setTimeout(function () {
                    var v = stylesSearchBox.val();
                    stylesTreeContainer.jstree(true).search(v);
                }, 250);
            });
            UserMessagesProvider.hideLoading();
        }, function (data, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });
        codeArea = ace.edit("codeTextArea");
        codeArea.setTheme("ace/theme/xcode");
        codeArea.setOption("newLineMode", "windows");
        codeArea.getSession().setMode("ace/mode/css");
        codeArea.setAutoScrollEditorIntoView(true);
        codeArea.setOption("minLines", 30);
    }

    $scope.closeDetailsForm = function () {
        stylesTreeContainer.jstree(true).deselect_all(false);
        $scope.currentObj = null;
        $scope.state = $scope.DetailsPanelStates.None;
    }

    $scope.displayDetails = function (e, data) {
        if (data.node != undefined) {
            if (data.node.original != undefined) {
                $scope.currentNodeObject = data.node;
                $scope.currentObj = jQuery.extend(true, {}, $scope.currentNodeObject.original.obj);
                $scope.state = $scope.DetailsPanelStates.StyleDetails;
            }
        }
    }

    $scope.createRootNode = function () {
        $scope.CurrentExecutingContext = $scope.ExecutingContexts.Creating;
        $scope.currentObj = {};
        var modalInstance = $modal.open({
            templateUrl: 'createForm.html',
            controller: createStyleController,
            size: 'lg',
            resolve: {
                parentId: function () {
                    return null;
                },
                UMP: function () { return UserMessagesProvider; }
            }
        });
        modalInstance.result.then(function (newNode) {
            UserMessagesProvider.displayLoading();
            WebsiteManagerDataProvider.createStylesBundle(newNode).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    stylesTreeContainer.jstree(true).create_node('#', { text: newNode.Name, obj: returnValue.data.obj, type: "Container" }, 'last', null, false);
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

    $scope.createNode = function (e, data) {
        if ($scope.CurrentExecutingContext == $scope.ExecutingContexts.Creating) {
            var parentNodeObject;
            parentNodeObject = stylesTreeContainer.jstree(true).get_node(stylesTreeContainer.jstree(true).get_parent(data.node));
            $scope.parentObject = parentNodeObject.original.obj;
            $scope.currentObj = {};
            var modalInstance = $modal.open({
                templateUrl: 'createForm.html',
                controller: createStyleController,
                size: 'lg',
                resolve: {
                    parentId: function () {
                        return $scope.parentObject.Id;
                    },
                    UMP: function () { return UserMessagesProvider; }
                }
            });
            modalInstance.result.then(function (newNode) {
                UserMessagesProvider.displayLoading();
                WebsiteManagerDataProvider.createStyle(newNode).then(function (returnValue) {
                    UserMessagesProvider.hideLoading();
                    if (returnValue.data.result == 'true') {
                        var distNode = stylesTreeContainer.jstree(true).get_node(data.node);
                        stylesTreeContainer.jstree(true).rename_node(data.node, newNode.Name);
                        distNode.original.obj = returnValue.data.obj;
                        distNode.original.type = "Item";
                        stylesTreeContainer.jstree(true).set_type(data.node, "Item");
                        stylesTreeContainer.jstree(true).deselect_node(parentNodeObject, false);
                        stylesTreeContainer.jstree(true).select_node(data.node, false);
                        UserMessagesProvider.successHandler();
                    }
                    else {
                        UserMessagesProvider.errorHandler(999, returnValue.data.message);
                        stylesTreeContainer.jstree(true).delete_node(data.node);
                    }
                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;

                }, function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                    stylesTreeContainer.jstree(true).delete_node(data.node);
                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                });

            }, function () {
                stylesTreeContainer.jstree(true).delete_node(data.node);
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
                var distNode = stylesTreeContainer.jstree(true).get_node($scope.currentNodeObject);
                if (distNode.type == 'Container') {
                    WebsiteManagerDataProvider.updateStylesBundle($scope.currentObj).then(function (returnValue) {
                        UserMessagesProvider.hideLoading();
                        if (returnValue.data.result == 'true') {
                            distNode.original.obj = returnValue.data.obj;
                            stylesTreeContainer.jstree(true).rename_node($scope.currentNodeObject, $scope.currentObj.Name);
                            UserMessagesProvider.successHandler();
                        }
                        else {
                            UserMessagesProvider.errorHandler(999, returnValue.data.message);
                            stylesTreeContainer.jstree(true).rename_node($scope.currentNodeObject, distNode.original.obj.Name);
                            if ($scope.CurrentExecutingContext == $scope.ExecutingContexts.Renaming) {
                                $scope.currentObj.Name = distNode.original.obj.Name;
                            }
                        }
                        $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                    }, function (errorData, status, headers, config) {
                        UserMessagesProvider.hideLoading();
                        UserMessagesProvider.errorHandler(status);
                        stylesTreeContainer.jstree(true).rename_node($scope.currentNodeObject, distNode.original.obj.Name);
                        if ($scope.CurrentExecutingContext == $scope.ExecutingContexts.Renaming) {
                            $scope.currentObj.Name = distNode.original.obj.Name;
                        }
                        $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                    });
                } else {
                    WebsiteManagerDataProvider.updateStyle($scope.currentObj).then(function (returnValue) {
                        UserMessagesProvider.hideLoading();
                        if (returnValue.data.result == 'true') {
                            distNode.original.obj = returnValue.data.obj;
                            stylesTreeContainer.jstree(true).rename_node($scope.currentNodeObject, $scope.currentObj.Name);
                            UserMessagesProvider.successHandler();
                        }
                        else {
                            UserMessagesProvider.errorHandler(999, returnValue.data.message);
                            stylesTreeContainer.jstree(true).rename_node($scope.currentNodeObject, distNode.original.obj.Name);
                            if ($scope.CurrentExecutingContext == $scope.ExecutingContexts.Renaming) {
                                $scope.currentObj.Name = distNode.original.obj.Name;
                            }
                        }
                        $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                    }, function (errorData, status, headers, config) {
                        UserMessagesProvider.hideLoading();
                        UserMessagesProvider.errorHandler(status);
                        stylesTreeContainer.jstree(true).rename_node($scope.currentNodeObject, distNode.original.obj.Name);
                        if ($scope.CurrentExecutingContext == $scope.ExecutingContexts.Renaming) {
                            $scope.currentObj.Name = distNode.original.obj.Name;
                        }
                        $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                    });
                }
            }
        } else {
            UserMessagesProvider.invalidHandler();
        }
    }

    $scope.saveCode = function () {
        if ($scope.currentObj) {
            var view = codeArea.getValue();
            UserMessagesProvider.displayLoading();
            WebsiteManagerDataProvider.updateStyleCode($scope.currentObj, { PostedCode: view }).then(function (data) {
                if (data.data.result == "true") {
                    UserMessagesProvider.successHandler();
                } else {
                    UserMessagesProvider.errorHandler(999, data.data.message);
                }
                UserMessagesProvider.hideLoading();
            }, function (data, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        }
    }

    $scope.displayGeneralDetails = function () {
        $scope.state = $scope.DetailsPanelStates.StyleDetails;
    }

    $scope.displayCode = function () {
        if ($scope.currentObj) {
            UserMessagesProvider.displayLoading();
            WebsiteManagerDataProvider.getStyleCode($scope.currentObj).then(function (data) {
                if (!data.data.startWith("Error!")) {
                    codeArea.setValue(data.data);
                    $scope.state = $scope.DetailsPanelStates.StyleCode;
                } else {
                    UserMessagesProvider.errorHandler(999, data.data);
                }
                UserMessagesProvider.hideLoading();
            }, function (data, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        }
    }

    $scope.displaySortingPanel = function () {
        var arr = [];
        for (i = 0; i < $scope.currentNodeObject.children.length; i++) {
            var node = stylesTreeContainer.jstree(true).get_node($scope.currentNodeObject.children[i]);
            if (node.type !== 'ContentVersion') {
                var id = node.original.obj.Id;
                var name = node.original.obj.Name;
                var path = node.original.obj.Path;
                arr.push({ Id: id, Name: name, Path: path });
            }
        }
        $scope.sortingContents = arr;
        $scope.state = $scope.DetailsPanelStates.ContentsSorting;
    }

    $scope.saveContentsSorting = function () {
        UserMessagesProvider.displayLoading();
        for (i = 0; i < $scope.sortingContents.length; i++) {
            $scope.sortingContents[i].Priority = i;
        }
        WebsiteManagerDataProvider.updateStylesOrdering($scope.currentObj.Id, $scope.sortingContents).then(function (returnValue) {
            UserMessagesProvider.hideLoading();
            console.log(returnValue);
            if (returnValue.data.result == 'true') {
                //console.log(returnValue.data.obj);
                //$scope.currentNodeObject = returnValue.data.obj;        
                $route.reload();
                UserMessagesProvider.successHandler();
            }
            else {
                UserMessagesProvider.errorHandler(999, returnValue.data.message);
            }
            $scope.state = $scope.DetailsPanelStates.None;
        }, function (errorData, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });
    }

    $scope.uploadFile = function () {
        var modalInstance = $modal.open({
            templateUrl: 'uploadForm.html',
            controller: uploadStyleFileController,
            size: 'lg',
            resolve: {
                path: function () {
                    return $scope.currentObj.Path;
                },
                UMP: function () { return UserMessagesProvider; },
                uploadProvider: function () {
                    return $upload;
                },
                scopeService: function () {
                    return scopeService;
                },
                rootScope: function () {
                    return $scope;
                },
                UserMessagesProvider: function () {
                    return UserMessagesProvider;
                },
            }
        });
        modalInstance.result.then(function (newFile) {
            $route.reload();
        }, function () {
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
        });
    }

    initialize();
});

var createStyleController = function ($scope, $modalInstance, parentId, UMP) {
    $scope.newNode = { BundleId: parentId };
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

var uploadStyleFileController = function ($scope, $modalInstance, path, UMP, uploadProvider, scopeService, WebsiteManagerDataProvider, rootScope, UserMessagesProvider) {
    var uploadedFiles = 0;
    var allFiles = 0;
    $scope.newFile = { Name: '', Path: path, FilePath: '', Files: [] };
    $scope.progress = 0;
    $scope.createForm = {};

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.storeStyleFile = function (style, file) {
        var reader = new FileReader();
        reader.readAsText(file, "UTF-8");
        reader.onload = function (e) {
            var cssText = reader.result;
            WebsiteManagerDataProvider.createStyle(style).then(function (returnValue) {
                if (returnValue.data.result === 'true') {
                    WebsiteManagerDataProvider.updateStyleCode({ Id: returnValue.data.obj.Id }, { PostedCode: cssText }).then(function (data) {
                        if (data.data.result === "true") {
                            uploadedFiles += 1;
                            scopeService.safeApply($scope, function () { $scope.progress = parseInt(100.0 * uploadedFiles / allFiles) });
                            if (uploadedFiles >= allFiles) {
                                $modalInstance.close();
                            }
                        } else {
                            UserMessagesProvider.errorHandler(999, data.data.message);
                        }
                        UserMessagesProvider.hideLoading();
                    }, function (data, status, headers, config) {
                        UserMessagesProvider.errorHandler(status);
                    });

                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.data.message);
                }
            }, function (errorData, status, headers, config) {
                UserMessagesProvider.errorHandler(status);
            });
        }
    }

    $scope.$watch('newFile.Files', function (oldValue, newValue) {
        if (oldValue == newValue) {
            return;
        }
        var currentObj = rootScope.currentNodeObject.original.obj;
        if (currentObj) {
            allFiles = $scope.newFile.Files.length;
            for (var i = 0; i < $scope.newFile.Files.length; i++) {
                var file = $scope.newFile.Files[i];
                var style = { BundleId: currentObj.Id, Name: file.name };

                $scope.storeStyleFile(style, file);
            }
            rootScope.CurrentExecutingContext = $scope.ExecutingContexts.None;
        }
    });
};
