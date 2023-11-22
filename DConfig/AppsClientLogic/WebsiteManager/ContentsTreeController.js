var nodeTypes = [
    { Name: 'Page', Value: 'Page' },
    { Name: 'Partial Page', Value: 'Partial' },
    { Name: 'Redirect', Value: 'Redirect' },
    { Name: 'Download', Value: 'Download' },
    { Name: 'Resource', Value: 'Resource' }
    //{ Name: 'Version of Content', Value: 'ContentVersion' }
];

angular.module('DConfig').controllerProvider.register('WebsiteManager.ContentsTreeController', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, WebsiteManagerDataProvider, EventsProvider, $modal, IntentsProvider, scopeService, $filter) {

    $scope.DetailsPanelStates = {
        None: 0,
        ContentDetails: 1,
        ContentInstanceDetails: 2,
        ContentInstanceProporties: 3,
        ContentsSorting: 4,
        Domains: 5,
        ChildrenNodes: 6
    };
    $scope.ExecutingContexts = {
        None: 0,
        Deleting: 1,
        Creating: 2,
        Renaming: 3,
        Updating: 4,
        Appending: 5,
        CreatingRoot: 6,
        CreatingContentAndInstance: 7,
        Cloning: 8
    };
    $scope.NodeTypes = nodeTypes;
    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
    $scope.Languages = [
        { Name: 'English', Value: 'EN' },
        { Name: 'Arabic', Value: 'AR' },
        { Name: 'Turkish', Value: 'TR' },
        { Name: 'French', Value: 'FR' },
        { Name: 'German', Value: 'DE' },
        { Name: 'Spanish', Value: 'ES' },
        { Name: 'Russian', Value: 'RU' },
        { Name: 'Chinese', Value: 'CH' },
        { Name: 'Swedish', Value: 'SV' }
    ];
    $scope.SortableOptions = {
        //items: '.panel',
        handle: '.handle'
    };
    $scope.state = $scope.DetailsPanelStates.None;
    var contentTreeContainer = $('#contentTree');
    var contentSearchBox = $('#contentSearchBox');

    $scope.contentChildren = [];
    $scope.contentChildrenCols = [
        { field: 'Id', displayName: '', cellTemplate: '<a href="" data-ng-click= "editGridNode(row.entity)"><div class=\"ngCellText\"><i class="fa fa-pencil extMenuChoice"></i></div></a>', width: 36, groupable: false, resizable: false, sortable: false, pinnable: false },
        { field: 'Name', displayName: 'Name', width: 100 },
        { field: 'ViewTypeName', displayName: 'View Type', width: 100 },
        { field: 'Online', displayName: 'Online', width: 100 },
        { field: 'UrlName', displayName: 'Url Name', width: 100 },
        { field: 'Priority', displayName: 'Priority', width: 100 },
        { field: 'CreateDate', displayName: 'Create Date', width: 100 },
        { field: 'ContentType', displayName: 'Content Type', width: 100 },
    ];
    $scope.contentChildrenSelections = [];
    $scope.gridOptions = {
        showFilter: true,
        showColumnMenu: true,
        showGroupPanel: true,
        enableColumnResize: true,
        showSelectionCheckbox: true,
        selectWithCheckboxOnly: true,
        data: 'contentChildren',
        columnDefs: 'contentChildrenCols',
        enablePinning: true,
        enableCellSelection: true,
        enableCellEdit: true,
        showFooter: true,
        selectedItems: $scope.contentChildrenSelections
    };

    function initialize() {
        BreadCrumpsProvider.breadCrumps.path = ['Website Manager', 'Contents'];
        UserMessagesProvider.displayLoading();
        WebsiteManagerDataProvider.loadShowcaseConfig().then(function (data) {
            configuration = { templateContextId:'' };
            try {
                var configuration = JSON.parse(data.data);
            } catch (ex) {
                console.log('No showcase configurations');
            }
            WebsiteManagerDataProvider.getRootViewTypes(configuration.templateContextId).then(function (data) {
                $scope.rootViewTypes = data.data;
                WebsiteManagerDataProvider.getViewTypes(configuration.templateContextId).then(function (data) {
                    $scope.viewTypes = data.data;
                    WebsiteManagerDataProvider.getContentsTree().then(function (data) {
                        contentTreeContainer.jstree('destroy');
                        $scope.contentTree = data.data;
                        contentTreeContainer
                            .on("create_node.jstree", function (e, data) { scopeService.safeApply($scope, function () { $scope.createTreeNode(e, data); }); })
                            .on("rename_node.jstree", function (e, data) { scopeService.safeApply($scope, function () { $scope.renameNode(e, data); }); })
                            .on("move_node.jstree", function () { })
                            .on("cut.jstree", function () { })
                            .on("copy.jstree", function (e, data) { scopeService.safeApply($scope, function () { $scope.cloneNode(e, data); }); })
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
                                        if (operation === 'delete_node' && $scope.CurrentExecutingContext == $scope.ExecutingContexts.None) {
                                            $scope.deleteNode();
                                            return false;
                                        } else if (operation === 'create_node') {
                                            if ($scope.CurrentExecutingContext != $scope.ExecutingContexts.Appending && $scope.CurrentExecutingContext != $scope.ExecutingContexts.CreatingRoot && $scope.CurrentExecutingContext != $scope.ExecutingContexts.CreatingContentAndInstance && $scope.CurrentExecutingContext != $scope.ExecutingContexts.Cloning) {
                                                //if ($scope.CurrentExecutingContext == $scope.ExecutingContexts.None)
                                                $scope.CurrentExecutingContext = $scope.ExecutingContexts.Creating;
                                                //else
                                                //$scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                                            }
                                        }
                                        return true;
                                    }
                                },
                                'sort': function (a, b) {
                                    var a_obj = this.get_node(a).original;
                                    var b_obj = this.get_node(b).original;
                                    if (a_obj.type == 'ContentVersion') {
                                        return 1;
                                    }
                                    if (b_obj.type == 'ContentVersion') {
                                        return -1;
                                    }
                                    if (a_obj.obj == undefined || b_obj.obj == undefined) {
                                        return -1;
                                    }
                                    return a_obj.obj.Priority > b_obj.obj.Priority ? 1 : -1;
                                },
                                "types": {
                                    "Page": {
                                        "icon": "jstree-folder"
                                    },
                                    "Partial": {
                                        "icon": "jstree-folder"
                                    },
                                    "Redirect": {
                                        "icon": "jstree-folder"
                                    },
                                    "Download": {
                                        "icon": "jstree-folder"
                                    },
                                    "Resource": {
                                        "icon": "jstree-folder"
                                    },
                                    "ContentVersion": {
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
                                        if (node.type == "ContentVersion") {
                                            items.ccp.action = items.ccp.submenu.copy.action;
                                            items.ccp.label = "Clone";
                                            delete items.ccp.submenu;
                                            delete items.create;

                                        } else {
                                            items.ccp.action = items.ccp.submenu.copy.action;
                                            items.ccp.label = "Clone";
                                            delete items.ccp.submenu.cut;
                                            delete items.ccp.submenu;
                                        }
                                        return items;
                                    }
                                },
                            });
                        $scope.closeDetailsForm();
                        //Binding Searchbox
                        var to = false;
                        contentSearchBox.keyup(function () {
                            if (to) { clearTimeout(to); }
                            to = setTimeout(function () {
                                var v = contentSearchBox.val();
                                contentTreeContainer.jstree(true).search(v);
                            }, 250);
                        });
                        UserMessagesProvider.hideLoading();
                    }, function (data, status, headers, config) {
                        UserMessagesProvider.hideLoading();
                        UserMessagesProvider.errorHandler(status);
                    });
                    if (configuration.pages) {
                        for (i = 0; i < configuration.pages.length; i++) {
                            WebsiteManagerDataProvider.getViewTypes(configuration.pages[i].templateContextId).then(function (data) {
                                if (data.data && data.data.length > 0) {
                                    for (j = 0; j < data.data.length; j++) {
                                        $scope.viewTypes.push(data.data[j]);
                                    }
                                }
                            }, function (data, status, headers, config) {
                                UserMessagesProvider.errorHandler(status);
                            });
                        }
                    }
                }, function (data, status, headers, config) {
                    UserMessagesProvider.errorHandler(status);
                });
            }, function (data, status, headers, config) {
                UserMessagesProvider.errorHandler(status);
            });
        }, function (data, status, headers, config) {
            UserMessagesProvider.errorHandler(status);
        });
    }

    $scope.closeDetailsForm = function () {
        contentTreeContainer.jstree(true).deselect_all(false);
        $scope.currentObj = null;
        $scope.state = $scope.DetailsPanelStates.None;
    }

    $scope.displayDetails = function (e, data) {
        if (data.node != undefined) {
            if (data.node.original != undefined) {
                $scope.currentNodeObject = data.node;
                $scope.currentObj = jQuery.extend(true, {}, $scope.currentNodeObject.original.obj);
                if ($scope.currentNodeObject.original.type == 'ContentVersion') {
                    parentNodeObject = contentTreeContainer.jstree(true).get_node(contentTreeContainer.jstree(true).get_parent(data.node));
                    $scope.parentObject = parentNodeObject.original.obj;
                    $scope.state = $scope.DetailsPanelStates.ContentInstanceDetails;
                    for (i = 0; i < $scope.viewTypes.length; i++) {
                        if ($scope.parentObject.ViewTypeId == $scope.viewTypes[i].Id) {
                            $scope.currentObj.PossibleViewTemplates = $scope.viewTypes[i].TypeTemplates;
                            break;
                        }
                    }

                } else {
                    $scope.state = $scope.DetailsPanelStates.ContentDetails;
                }
            }
        }
    }

    $scope.deleteNode = function () {
        UserMessagesProvider.confirmHandler("The node will be deleted with all its content, are you sure you want to do that?", function () {
            deletedObj = $scope.currentObj;
            deletedType = $scope.currentNodeObject.original.type;
            UserMessagesProvider.displayLoading();
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.Deleting;
            WebsiteManagerDataProvider.deleteContentNode(deletedObj, deletedType).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    if ($scope.currentNodeObject != undefined)
                        contentTreeContainer.jstree(true).delete_node($scope.currentNodeObject);
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
        }, null);
    }

    $scope.deleteGridContents = function () {
        UserMessagesProvider.confirmHandler("Are you sure you want to delete these contents?", function () {
            angular.forEach($scope.contentChildrenSelections, function (instance) {
                WebsiteManagerDataProvider.deleteContentNode(instance, 'Page').then(function (returnValue) {
                    UserMessagesProvider.hideLoading();
                    if (returnValue.data.result == 'true') {
                        $scope.contentChildren = $filter('filter')($scope.contentChildren, function (element) { return element.Id != instance.Id; });
                        UserMessagesProvider.successHandler();
                    }
                    else {
                        UserMessagesProvider.errorHandler(999, returnValue.data.message);
                    }
                }, function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                });
            });
            $scope.contentChildrenSelections.splice(0, $scope.contentChildrenSelections.length);
        }, null);
    }

    $scope.editGridNode = function (obj) {
        UserMessagesProvider.displayLoading();
        WebsiteManagerDataProvider.getContentsTree(obj.Id).then(function (data) {
            var distNode = contentTreeContainer.jstree(true).get_node($scope.currentNodeObject);
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.Appending;
            var id = contentTreeContainer.jstree(true).create_node(distNode, data.data[0], 'last', null, false);
            contentTreeContainer.jstree(true).deselect_node($scope.currentNodeObject, false);
            contentTreeContainer.jstree(true).select_node(contentTreeContainer.jstree(true).get_node(id), false);
            UserMessagesProvider.hideLoading();
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
        }, function (data, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });
    }

    $scope.createGridNode = function () {
        $scope.parentObject = $scope.currentObj;
        $scope.createNode('G', null, null);
    }

    $scope.createRootNode = function () {
        $scope.CurrentExecutingContext = $scope.ExecutingContexts.CreatingRoot;
        $scope.parentObject = null;
        $scope.createNode('R', null, null);
    }

    $scope.createTreeNode = function (e, data) {
        if ($scope.CurrentExecutingContext == $scope.ExecutingContexts.Creating) {
            var parentNodeObject = contentTreeContainer.jstree(true).get_node(contentTreeContainer.jstree(true).get_parent(data.node));
            $scope.parentObject = parentNodeObject.original.obj;
            $scope.createNode('T', data, parentNodeObject);
        }
        else {
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
        }
    }

    $scope.createNode = function (nodeType, data, parentNodeObject) {
        var modalInstance = $modal.open({
            templateUrl: 'createForm.html',
            controller: createContentNodeController,
            size: 'lg',
            resolve: {
                parentId: function () {
                    if ($scope.parentObject == null)
                        return null;
                    else
                        return $scope.parentObject.Id;
                },
                viewTypes: function () {
                    if ($scope.parentObject == null)
                        return $scope.rootViewTypes;
                    else
                        return $scope.parentObject.PossibleChildViewTypes;
                },
                viewTemplates: function () {
                    if ($scope.parentObject == null)
                        return null;
                    else {
                        //return $scope.parentObject.PossibleChildViewTemplates;
                        for (i = 0; i < $scope.viewTypes.length; i++) {
                            if ($scope.parentObject.ViewTypeId == $scope.viewTypes[i].Id) {

                                return $scope.viewTypes[i].TypeTemplates;
                            }
                        }
                    }
                },
                languages: function () {
                    return $scope.Languages;
                },
                UMP: function () { return UserMessagesProvider; }
            }
        });
        modalInstance.result.then(function (newNode) {
            UserMessagesProvider.displayLoading();
            if (newNode.CreatingMode != '2') {
                WebsiteManagerDataProvider.createContentNode(newNode, (newNode.CreatingMode == '0' ? false : true)).then(function (returnValue) {
                    UserMessagesProvider.hideLoading();
                    if (returnValue.data.result == 'true') {
                        if (nodeType == 'R') {
                            contentTreeContainer.jstree(true).create_node('#', { text: newNode.Name, type: (newNode.CreatingMode == '1' ? 'ContentVersion' : newNode.ContentType), obj: returnValue.data.obj }, 'last', null, false);
                        }
                        else if (nodeType == 'T') {
                            $scope.currentObj = {};
                            var distNode = contentTreeContainer.jstree(true).get_node(data.node);
                            contentTreeContainer.jstree(true).rename_node(data.node, newNode.Name);
                            distNode.original.obj = returnValue.data.obj;
                            distNode.original.type = (newNode.CreatingMode == '1' ? 'ContentVersion' : newNode.ContentType);
                            contentTreeContainer.jstree(true).set_type(data.node, (newNode.CreatingMode == '1' ? 'ContentVersion' : newNode.ContentType));
                            contentTreeContainer.jstree(true).deselect_node(parentNodeObject, false);
                            contentTreeContainer.jstree(true).select_node(data.node, false);
                        } else if (nodeType == 'G') {
                            $scope.currentObj.contentsChildren.push(returnValue.data.obj);
                        }
                        UserMessagesProvider.successHandler();
                    }
                    else {
                        UserMessagesProvider.errorHandler(999, returnValue.data.message);
                        if (data != null)
                            contentTreeContainer.jstree(true).delete_node(data.node);
                    }
                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                }, function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                    if (data != null)
                        contentTreeContainer.jstree(true).delete_node(data.node);
                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                });
            } else {
                $scope.CurrentExecutingContext = $scope.ExecutingContexts.CreatingContentAndInstance;
                var distNode;
                var distObj;
                WebsiteManagerDataProvider.createContentNode(newNode, false).then(function (returnValue) {
                    if (returnValue.data.result == 'true') {
                        distObj = returnValue.data.obj;
                        if (nodeType == 'R') {
                            contentTreeContainer.jstree(true).create_node('#', { text: newNode.Name, type: newNode.ContentType, obj: returnValue.data.obj }, 'last', null, false);
                        }
                        else if (nodeType == 'T') {
                            $scope.currentObj = {};
                            distNode = contentTreeContainer.jstree(true).get_node(data.node);
                            contentTreeContainer.jstree(true).rename_node(data.node, newNode.Name);
                            distNode.original.obj = returnValue.data.obj;
                            distNode.original.type = newNode.ContentType;
                            contentTreeContainer.jstree(true).set_type(data.node, newNode.ContentType);
                        } else if (nodeType == 'G') {
                            $scope.currentObj.contentsChildren.push(returnValue.data.obj);
                        }
                        UserMessagesProvider.successHandler();
                        newNode.ContentId = distObj.Id;
                        WebsiteManagerDataProvider.createContentNode(newNode, true).then(function (returnValue) {
                            UserMessagesProvider.hideLoading();
                            if (returnValue.data.result == 'true') {
                                if (nodeType == 'R' || nodeType == 'T') {
                                    contentTreeContainer.jstree(true).create_node(distNode, { text: newNode.Name, type: 'ContentVersion', obj: returnValue.data.obj }, 'last', null, false);
                                }
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
                    else {
                        UserMessagesProvider.errorHandler(999, returnValue.message);
                        if (data != null)
                            contentTreeContainer.jstree(true).delete_node(data.node);
                    }
                }, function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                    if (data != null)
                        contentTreeContainer.jstree(true).delete_node(data.node);
                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                });
            }
        }, function () {
            if (data != null)
                contentTreeContainer.jstree(true).delete_node(data.node);
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
        });

    }

    $scope.cloneNode = function (e, data) {
        $scope.CurrentExecutingContext = $scope.ExecutingContexts.Cloning;
        var modalInstance = $modal.open({
            templateUrl: 'cloneForm.html',
            controller: cloneTemplateController,
            size: 'lg',
            resolve: {
                id: function () {
                    if ($scope.parentObject == null)
                        return null;
                    else
                        return $scope.parentObject.Id;
                },
                UMP: function () { return UserMessagesProvider; }
            }
        });
        modalInstance.result.then(function (newNode) {
            var parentNodeObject = contentTreeContainer.jstree(true).get_node(contentTreeContainer.jstree(true).get_parent(data.node));
            var parentObject;
            if (parentNodeObject !== undefined && parentNodeObject.original !== undefined && parentNodeObject.original.obj !== undefined)
                parentObject = parentNodeObject.original.obj;
            else {
                parentObject = '#';
            }
            UserMessagesProvider.displayLoading();
            if ($scope.currentNodeObject.original.type == 'ContentVersion') {
                WebsiteManagerDataProvider.cloneContentNode($scope.currentObj.Id, true, newNode.Suffix).then(function (returnValue) {
                    UserMessagesProvider.hideLoading();
                    if (returnValue.data.result == 'true') {
                        contentTreeContainer.jstree(true).create_node(parentNodeObject, { text: returnValue.data.obj.Name, type: 'ContentVersion', obj: returnValue.data.obj }, 'last', null, false);
                        parentObject.parentObject.contentsChildren.push(returnValue.data.obj);
                        UserMessagesProvider.successHandler();
                    }
                    else {
                        UserMessagesProvider.errorHandler(999, returnValue.data.message);
                    }
                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                    UserMessagesProvider.hideLoading();
                }, function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                });
            } else {
                WebsiteManagerDataProvider.cloneContentNode($scope.currentObj.Id, false, newNode.Suffix).then(function (returnValue) {
                    if (returnValue.data.result == 'true') {
                        $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                        initialize();
                        // Not working fine (Adding cloned sub contents) so I am reinitializing the tree
                        //var result = contentTreeContainer.jstree(true).create_node(parentNodeObject, { text: returnValue.data.obj.Name, type: returnValue.data.obj.ContentType, obj: returnValue.data.obj }, 'last', null, false);
                        /*WebsiteManagerDataProvider.getContentInstances(returnValue.data.obj.Id, false).then(function (data) {
                            for (i = 0; i < data.data.length; i++) {
                                var contentInstance = data.data[i];
                                $scope.CurrentExecutingContext = $scope.ExecutingContexts.Cloning;
                                contentTreeContainer.jstree(true).create_node(result, { text: contentInstance.Name, type: 'ContentVersion', obj: contentInstance }, 'last', null, false);
                            }
                            UserMessagesProvider.successHandler();
                            UserMessagesProvider.hideLoading();
                            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                        }, function (errorData, status, headers, config) {
                            UserMessagesProvider.hideLoading();
                            UserMessagesProvider.errorHandler(status);
                            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                        });
                        WebsiteManagerDataProvider.getContentsTree(returnValue.obj.Id).then(function (data) {
                            populateLoadedTreeItems(result, data.data[0]);
                        }, function (errorData, status, headers, config) {
                            UserMessagesProvider.hideLoading();
                            UserMessagesProvider.errorHandler(status);
                            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                        });*/
                    }
                    else {
                        UserMessagesProvider.errorHandler(999, returnValue.message);
                        UserMessagesProvider.hideLoading();
                        $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                    }
                }, function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                });
            }
        }, function () {
            if (data != null)
                templatesTreeContainer.jstree(true).delete_node(data.node);
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
        });
    }

    function populateLoadedTreeItems(node, data) {
        if (data != null && data.children != null && data.children.length > 0) {
            for (var i = 0; i < data.children.length; i++) {
                $scope.CurrentExecutingContext = $scope.ExecutingContexts.Cloning;
                var result = contentTreeContainer.jstree(true).create_node(node, { text: data.children[i].text, type: data.children[i].type, obj: data.children[i].obj }, 'last', null, false);
                $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                populateLoadedTreeItems(result, data.children[i]);
            }
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
        if (($scope.state == $scope.DetailsPanelStates.ContentDetails && $scope.detialsForm.$valid) || ($scope.state == $scope.DetailsPanelStates.ContentInstanceDetails && $scope.instanceDetailsForm.$valid)) {
            if ($scope.currentObj != null && ($scope.CurrentExecutingContext == $scope.ExecutingContexts.None || $scope.CurrentExecutingContext == $scope.ExecutingContexts.Renaming)) {
                UserMessagesProvider.displayLoading();
                var distNode = contentTreeContainer.jstree(true).get_node($scope.currentNodeObject);
                WebsiteManagerDataProvider.updateContentNode($scope.currentObj, $scope.currentNodeObject.original.type).then(function (returnValue) {
                    UserMessagesProvider.hideLoading();
                    if (returnValue.data.result == 'true') {
                        var treeNeedReinitialize = distNode.original.obj.PlentyChildren != $scope.currentObj.PlentyChildren;
                        distNode.original.obj = returnValue.data.obj;
                        $scope.currentObj.UrlFullCode = returnValue.data.obj.UrlFullCode;
                        contentTreeContainer.jstree(true).rename_node($scope.currentNodeObject, $scope.currentObj.Name);
                        UserMessagesProvider.successHandler();
                        if (treeNeedReinitialize) {
                            initialize();
                        }
                    }
                    else {
                        UserMessagesProvider.errorHandler(999, returnValue.data.message);
                        contentTreeContainer.jstree(true).rename_node($scope.currentNodeObject, distNode.original.obj.Name);
                        if ($scope.CurrentExecutingContext == $scope.ExecutingContexts.Renaming) {
                            $scope.currentObj.Name = distNode.original.obj.Name;
                        }
                    }
                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                }, function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                    contentTreeContainer.jstree(true).rename_node($scope.currentNodeObject, distNode.original.obj.Name);
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

    $scope.displayInstanceGeneralDetails = function () {
        $scope.state = $scope.DetailsPanelStates.ContentInstanceDetails;
    }

    $scope.displayContentGeneralDetails = function () {
        $scope.state = $scope.DetailsPanelStates.ContentDetails;
    }

    $scope.linkingDomains = function () {
        $scope.state = $scope.DetailsPanelStates.Domains;
    }

    $scope.displaySortingPanel = function () {
        var arr = [];
        for (i = 0; i < $scope.currentNodeObject.children.length; i++) {
            var node = contentTreeContainer.jstree(true).get_node($scope.currentNodeObject.children[i]);
            if (node.type != 'ContentVersion') {
                var id = node.original.obj.Id;
                var name = node.original.obj.Name;
                arr.push({ Id: id, Name: name });
            }
        }
        $scope.sortingContents = arr;
        $scope.state = $scope.DetailsPanelStates.ContentsSorting;
    }

    $scope.displayProporties = function () {
        $scope.state = $scope.DetailsPanelStates.ContentInstanceProporties;
        if ($scope.currentObj.FieldsValues == undefined) {
            UserMessagesProvider.displayLoading();
            var distNode = contentTreeContainer.jstree(true).get_node($scope.currentNodeObject);
            WebsiteManagerDataProvider.getcontentinstanceFieldsValues($scope.currentObj.Id).then(function (data) {
                $scope.currentObj.FieldsValues = data.data;
                distNode.original.obj.FieldsValues = data.data;
                bindContentInstanceFieldValues();
                $scope.state = $scope.DetailsPanelStates.ContentInstanceProporties;
                UserMessagesProvider.hideLoading();
            }, function (data, status, headers, config) {
                UserMessagesProvider.errorHandler(status);
                UserMessagesProvider.hideLoading();
            });
        } else {
            bindContentInstanceFieldValues();
            $scope.state = $scope.DetailsPanelStates.ContentInstanceProporties;
        }
    }

    function bindContentInstanceFieldValues() {
        if ($scope.parentObject.ViewType == undefined) {
            for (k = 0; k < $scope.viewTypes.length; k++) {
                if ($scope.parentObject.ViewTypeId == $scope.viewTypes[k].Id) {
                    $scope.parentObject.ViewType = $scope.viewTypes[k];
                    break;
                }
            }
        }
        if ($scope.parentObject.ViewType != undefined) {
            if ($scope.currentObj.FieldsValues != null && $scope.currentObj.FieldsValues.length > 0) {
                for (i = 0; i < $scope.parentObject.ViewType.ViewFields.length; i++) {
                    $scope.parentObject.ViewType.ViewFields[i].Value = '';
                    for (j = 0; j < $scope.currentObj.FieldsValues.length; j++) {
                        if ($scope.parentObject.ViewType.ViewFields[i].Id == $scope.currentObj.FieldsValues[j].FieldId) {
                            $scope.parentObject.ViewType.ViewFields[i].Value = $scope.currentObj.FieldsValues[j].Value;
                        }
                    }
                }
            } else {
                for (i = 0; i < $scope.parentObject.ViewType.ViewFields.length; i++) {
                    $scope.parentObject.ViewType.ViewFields[i].Value = '';
                }
            }
        }
    }

    $scope.displayChildrenNodes = function () {
        if ($scope.currentObj.contentsChildren == undefined) {
            UserMessagesProvider.displayLoading();
            var distNode = contentTreeContainer.jstree(true).get_node($scope.currentNodeObject);
            WebsiteManagerDataProvider.getContentChildren($scope.currentObj.Id).then(function (data) {
                $scope.currentObj.contentsChildren = data.data;
                distNode.original.obj.contentsChildren = data.data;
                $scope.contentChildren = $scope.currentObj.contentsChildren;
                $scope.state = $scope.DetailsPanelStates.ChildrenNodes;
                UserMessagesProvider.hideLoading();
            }, function (data, status, headers, config) {
                UserMessagesProvider.errorHandler(status);
                UserMessagesProvider.hideLoading();
            });
        } else {
            $scope.contentChildren = $scope.currentObj.contentsChildren;
            $scope.state = $scope.DetailsPanelStates.ChildrenNodes;
        }
    }

    $scope.saveFieldsValues = function () {
        UserMessagesProvider.displayLoading();
        var fieldValueExists = false;
        if ($scope.currentObj.FieldsValues == undefined) {
            $scope.currentObj.FieldsValues = [];
        }
        for (i = 0; i < $scope.parentObject.ViewType.ViewFields.length; i++) {
            for (j = 0; j < $scope.currentObj.FieldsValues.length; j++) {
                if ($scope.parentObject.ViewType.ViewFields[i].Id == $scope.currentObj.FieldsValues[j].FieldId) {
                    $scope.currentObj.FieldsValues[j].Value = $scope.parentObject.ViewType.ViewFields[i].Value;
                    fieldValueExists = true;
                }
            }
            if (fieldValueExists) {
                fieldValueExists = false;
            }
            else {
                $scope.currentObj.FieldsValues.push({ FieldId: $scope.parentObject.ViewType.ViewFields[i].Id, ContentId: $scope.currentObj.Id, Value: $scope.parentObject.ViewType.ViewFields[i].Value });
            }
        }
        var distNode = contentTreeContainer.jstree(true).get_node($scope.currentNodeObject);
        WebsiteManagerDataProvider.updateContentInstanceFieldValues($scope.currentObj).then(function (returnValue) {
            UserMessagesProvider.hideLoading();
            if (returnValue.data.result == 'true') {
                distNode.original.obj = returnValue.data.obj;
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

    $scope.saveContentsSorting = function () {
        UserMessagesProvider.displayLoading();
        for (i = 0; i < $scope.sortingContents.length; i++) {
            $scope.sortingContents[i].Priority = i;
        }
        WebsiteManagerDataProvider.updateContentsOrdering($scope.sortingContents).then(function (returnValue) {
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
        });
    }

    $scope.runContentSelector = function (index) {
        if ($scope.contentsList == undefined) {
            UserMessagesProvider.displayLoading();
            WebsiteManagerDataProvider.getContentsListsTree().then(function (data) {
                $scope.contentsList = data.data;
                UserMessagesProvider.hideLoading();
                var modalInstance = $modal.open({
                    templateUrl: 'SelectContent.html',
                    controller: selectContentController,
                    size: 'lg',
                    resolve: {
                        data: function () {
                            return $scope.contentsList;
                        }
                    }
                });
                modalInstance.result.then(function (obj) {
                    if (obj.UrlFullCode)
                        $scope.parentObject.ViewType.ViewFields[index].Value = obj.UrlFullCode;
                }, function () {
                });
            }, function (data, status, headers, config) {
                UserMessagesProvider.errorHandler(status);
                UserMessagesProvider.hideLoading();
            });
        } else {
            var modalInstance = $modal.open({
                templateUrl: 'SelectContent.html',
                controller: selectContentController,
                size: 'lg',
                resolve: {
                    data: function () {
                        return $scope.contentsList;
                    }
                }
            });
            modalInstance.result.then(function (obj) {
                $scope.parentObject.ViewType.ViewFields[index].Value = obj.UrlFullCode;
            }, function () {
            });
        }
    }

    $scope.runIntentSelectorForDownloadFile = function (index) {
        IntentsProvider.startIntent('ImageBrowser', null, function (value) {
            $scope.currentObj.DownloadPath = value;
        }, null);
    }

    $scope.runIntentSelector = function (index) {
        var intentName = $scope.parentObject.ViewType.ViewFields[index].TypeObj.IntentName;
        IntentsProvider.startIntent(intentName, { MultipleImages: $scope.parentObject.ViewType.ViewFields[index].Type == "Multiple Images", Value: $scope.parentObject.ViewType.ViewFields[index].Value }, function (value) {
            $scope.parentObject.ViewType.ViewFields[index].Value = value;
        }, null);
    }

    $scope.resetField = function (index) {
        $scope.parentObject.ViewType.ViewFields[index].Value = '';
    }

    $scope.removeDomainFromContent = function (index) {
        UserMessagesProvider.confirmHandler("Are you sure you want to remove the domain from this content?", function () {
            UserMessagesProvider.displayLoading();
            var distNode = contentTreeContainer.jstree(true).get_node($scope.currentNodeObject);
            WebsiteManagerDataProvider.removeDomainFromContent($scope.currentObj, $scope.currentObj.DomainAliases[index]).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    distNode.original.obj = returnValue.data.obj;
                    $scope.currentObj.DomainAliases.splice(index, 1);
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

    $scope.addDomainToContent = function () {
        var modalInstance = $modal.open({
            templateUrl: 'addDomain.html',
            controller: addDomainController,
            size: 'sm'
        });
        modalInstance.result.then(function (obj) {
            var distNode = contentTreeContainer.jstree(true).get_node($scope.currentNodeObject);
            UserMessagesProvider.displayLoading();
            WebsiteManagerDataProvider.addDomainToContent($scope.currentObj, obj.Name).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    distNode.original.obj = returnValue.data.obj;
                    if ($scope.currentObj.DomainAliases == undefined) {
                        $scope.currentObj.DomainAliases = [];
                    }
                    $scope.currentObj.DomainAliases.push(obj.Name);
                    UserMessagesProvider.successHandler();
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.data.message);
                }
            }, function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(999);
            });
        }, function () {
        });
    }

    $scope.CheckingChildsViewType = function () {
        if ($scope.currentObj.PossibleChildViewTypes.length > 1) {
            UserMessagesProvider.errorHandler(999, "Can't add to quick content list, this content has more than 1 child view types");
            $scope.currentObj.PlentyChildren = false;
        }
    }

    initialize();
});
var createContentNodeController = function ($scope, $modalInstance, parentId, viewTypes, viewTemplates, languages, UMP) {
    if (parentId == null) {
        $scope.NodeTypes = [nodeTypes[0], nodeTypes[2]];
    } else {
        $scope.NodeTypes = nodeTypes;
    }
    $scope.newNode = { ParentId: parentId, ContentId: parentId };
    $scope.viewTypes = viewTypes;
    $scope.viewTemplates = viewTemplates;
    $scope.languages = languages;
    $scope.createForm = {};

    $scope.$watch('newNode.ViewTypeId', function (newValue, oldValue) {
        // Ignore initial setup.
        if (newValue === oldValue) {
            return;
        }
        for (i = 0; i < $scope.viewTypes.length; i++) {
            if ($scope.viewTypes[i].Id == $scope.newNode.ViewTypeId) {
                $scope.currentViewType = $scope.viewTypes[i];

            }
        }
    });

    $scope.ok = function () {
        if ($scope.newNode.CreatingMode != undefined) {
            if ($scope.createForm.form.$valid) {
                $scope.newNode.Online = true;
                $modalInstance.close($scope.newNode);
            } else {
                UMP.invalidHandler();
            }
        } else {
            UMP.errorHandler(999, "You haven't choose what to create!");
        }
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};
var selectContentController = function ($scope, $modalInstance, data) {
    var ct;
    $scope.initialize = function () {
        ct = $('#contentTree_contentSelector');
        ct.jstree({
            "plugins": ["checkbox", "sort", "state", "types", "unique", "wholerow"],
            "core": {
                "data": data,
                "multiple": false,
                'themes': {
                    'responsive': false
                },
            },
            'sort': function (a, b) {
                var a_obj = this.get_node(a).original;
                var b_obj = this.get_node(b).original;
                if (a_obj.type == 'ContentVersion') {
                    return 1;
                }
                if (b_obj.type == 'ContentVersion') {
                    return -1;
                }
                return a_obj.obj.Priority > b_obj.obj.Priority ? 1 : -1;
            },
            "types": {
                "Page": {
                    "icon": "jstree-folder"
                },
                "Partial": {
                    "icon": "jstree-folder"
                },
                "Redirect": {
                    "icon": "jstree-folder"
                },
                "ContentVersion": {
                    "icon": "jstree-file"
                }
            },
            "checkbox": {
                "whole_node": false,
                "keep_selected_style": false,
                "three_state": false
            }
        });
    }

    $scope.ok = function () {
        var selectedNodes = ct.jstree(true).get_selected('full');
        $modalInstance.close(selectedNodes[0].original.obj);
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};
var addDomainController = function ($scope, $modalInstance) {
    $scope.newDomain = {};
    $scope.createForm = {};
    $scope.ok = function () {
        if ($scope.createForm.form.$valid) {
            $modalInstance.close($scope.newDomain);
        } else {
            UMP.invalidHandler();
        }
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};
var cloneTemplateController = function ($scope, id, $modalInstance, UMP) {
    $scope.newNode = { Id: id, Suffix: '' };
    $scope.createForm = {};
    $scope.ok = function () {
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