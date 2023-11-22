angular.module('DConfig').controllerProvider.register('WebsiteManager.TemplatesTreeController', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, WebsiteManagerDataProvider, EventsProvider, $modal, scopeService, $route) {
    $scope.DetailsPanelStates = {
        None: 0,
        TemplateDetails: 1,
        TemplateCode: 2
    };
    $scope.ExecutingContexts = {
        None: 0,
        Deleting: 1,
        Creating: 2,
        Renaming: 3,
        Updating: 4,
        Cloning: 5
    };
    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
    $scope.state = $scope.DetailsPanelStates.None;
    var templatesTreeContainer = $('#templatesTreeContainer');
    var templatesSearchBox = $('#templatesSearchBox');
    var codeArea;

    function initialize() {
        BreadCrumpsProvider.breadCrumps.path = ['Website Manager', 'Templates'];
        UserMessagesProvider.displayLoading();
        WebsiteManagerDataProvider.getViewTypes().then(function (data) {
            $scope.viewTypes = data.data;
        }, function (data, status, headers, config) {
            UserMessagesProvider.errorHandler(status);
        });
        WebsiteManagerDataProvider.getTemplatesTree().then(function (data) {
            $scope.templatesTree = data.data;
            templatesTreeContainer
                .on("create_node.jstree", function (e, data) { scopeService.safeApply($scope, function () { $scope.createNode(e, data); }); })
                .on("rename_node.jstree", function (e, data) { scopeService.safeApply($scope, function () { $scope.renameNode(e, data); }); })
                .on("move_node.jstree", function () { })
                .on("copy_node.jstree", function () { })
                .on("cut.jstree", function () { })
                .on("copy.jstree", function (e, data) { scopeService.safeApply($scope, function () { $scope.cloneNode(e, data); }); })
                .on("paste.jstree", function () { })
                .on("changed.jstree", function (e, data) { scopeService.safeApply($scope, function () { $scope.displayDetails(e, data); }); })
                .jstree({
                    "plugins": ["contextmenu", "dnd", "search", "sort", "state", "types", "unique", "wholerow"],
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
                                    WebsiteManagerDataProvider.deleteTemplate($scope.currentObj).then(function (returnValue) {
                                        UserMessagesProvider.hideLoading();
                                        if (returnValue.data.result == 'true') {
                                            templatesTreeContainer.jstree(true).delete_node($scope.currentNodeObject);
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
                                }, null);
                                return false;
                            }
                            else if (operation === 'create_node') {
                                if ($scope.CurrentExecutingContext != $scope.ExecutingContexts.Appending && $scope.CurrentExecutingContext != $scope.ExecutingContexts.CreatingRoot && $scope.CurrentExecutingContext != $scope.ExecutingContexts.CreatingContentAndInstance && $scope.CurrentExecutingContext != $scope.ExecutingContexts.Cloning) {
                                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.Creating;
                                }
                                //if ($scope.CurrentExecutingContext == $scope.ExecutingContexts.None)
                                //    $scope.CurrentExecutingContext = $scope.ExecutingContexts.Creating;
                                //else
                                //    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                            }
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
                    },
                    'contextmenu': {
                        'select_node': true,
                        "items": function (node) {
                            var items = $.jstree.defaults.contextmenu.items();
                            items.ccp.action = items.ccp.submenu.copy.action;
                            items.ccp.label = "Clone";
                            delete items.ccp.submenu;
                            return items;
                        }
                    }
                });
            $scope.closeDetailsForm();
            //Binding Searchbox
            var to = false;
            templatesSearchBox.keyup(function () {
                if (to) { clearTimeout(to); }
                to = setTimeout(function () {
                    var v = templatesSearchBox.val();
                    templatesTreeContainer.jstree(true).search(v);
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
        codeArea.getSession().setMode("ace/mode/html");
        codeArea.setAutoScrollEditorIntoView(true);
        codeArea.setOption("minLines", 30);
    }

    $scope.closeDetailsForm = function () {
        templatesTreeContainer.jstree(true).deselect_all(false);
        $scope.currentObj = null;
        $scope.state = $scope.DetailsPanelStates.None;
    }

    $scope.displayDetails = function (e, data) {
        if (data.node != undefined) {
            if (data.node.original != undefined) {
                $scope.currentNodeObject = data.node;
                $scope.currentObj = jQuery.extend(true, {}, $scope.currentNodeObject.original.obj);
                $scope.state = $scope.DetailsPanelStates.TemplateDetails;
            }
        }
    }

    $scope.createRootNode = function () {
        $scope.CurrentExecutingContext = $scope.ExecutingContexts.Creating;
        $scope.createNode(null, null);
    }

    $scope.createNode = function (e, data) {
        if ($scope.CurrentExecutingContext == $scope.ExecutingContexts.Creating) {
            var parentNodeObject;
            if (data != null) {
                parentNodeObject = templatesTreeContainer.jstree(true).get_node(templatesTreeContainer.jstree(true).get_parent(data.node));
                $scope.parentObject = parentNodeObject.original.obj;
            } else {
                $scope.parentObject = null;
            }
            $scope.currentObj = {};
            var modalInstance = $modal.open({
                templateUrl: 'createForm.html',
                controller: createTemplateController,
                size: 'lg',
                resolve: {
                    parentId: function () {
                        if ($scope.parentObject == null)
                            return null;
                        else
                            return $scope.parentObject.Id;
                    },
                    viewTypes: function () {
                        return $scope.viewTypes;
                    },
                    UMP: function () { return UserMessagesProvider; }
                }
            });
            modalInstance.result.then(function (newNode) {
                UserMessagesProvider.displayLoading();
                WebsiteManagerDataProvider.createTemplate(newNode).then(function (returnValue) {
                    UserMessagesProvider.hideLoading();
                    if (returnValue.data.result == 'true') {
                        if (data == null) {
                            templatesTreeContainer.jstree(true).create_node('#', { text: newNode.Name, type: (newNode.IsContainer ? "Container" : "Item"), obj: returnValue.data.obj }, 'last', null, false);
                        }
                        else {
                            var distNode = templatesTreeContainer.jstree(true).get_node(data.node);
                            templatesTreeContainer.jstree(true).rename_node(data.node, newNode.Name);
                            distNode.original.obj = returnValue.data.obj;
                            distNode.original.type = (newNode.IsContainer ? "Container" : "Item");
                            templatesTreeContainer.jstree(true).set_type(data.node, (newNode.IsContainer ? "Container" : "Item"));
                            templatesTreeContainer.jstree(true).deselect_node(parentNodeObject, false);
                            templatesTreeContainer.jstree(true).select_node(data.node, false);
                        }
                        UserMessagesProvider.successHandler();
                    }
                    else {
                        UserMessagesProvider.errorHandler(999, returnValue.data.message);
                        if (data != null)
                            templatesTreeContainer.jstree(true).delete_node(data.node);
                    }
                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                }, function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                    if (data != null)
                        templatesTreeContainer.jstree(true).delete_node(data.node);
                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                });

            }, function () {
                if (data != null)
                    templatesTreeContainer.jstree(true).delete_node(data.node);
                $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
            });
        }
        else {
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
        }
    }

    $scope.cloneNode = function (e, data) {
        $scope.CurrentExecutingContext = $scope.ExecutingContexts.Cloning;
        var parentNodeObject = templatesTreeContainer.jstree(true).get_node(templatesTreeContainer.jstree(true).get_parent(data.node));
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
            UserMessagesProvider.displayLoading();
            WebsiteManagerDataProvider.createTemplateClone($scope.currentObj.Id, newNode.Suffix).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    //var node=templatesTreeContainer.jstree(true).create_node(parentNodeObject, { text: returnValue.data.obj.Name, type: (returnValue.data.obj.IsContainer ? "Container" : "Item"), obj: returnValue.data.obj }, 'last', null, false);
                    //loadChildrenTemplatesToTree(returnValue.data.obj.ChildrenTemplates, node);
                    $route.reload();
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
        }, function () {
            if (data != null)
                templatesTreeContainer.jstree(true).delete_node(data.node);
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
        });
    }

    //just can be used in case the templates are fetched with the object
    function loadChildrenTemplatesToTree(childTemplates,parent) {
        if (childTemplates != undefined && childTemplates > 0) {
            for (i = 0; i < childTemplates.length; i++) {
                var template = childTemplates[i];
                $scope.CurrentExecutingContext = $scope.ExecutingContexts.Cloning;
                var node = contentTreeContainer.jstree(true).create_node(parent, { text: template.Name, type: (template.IsContainer ? "Container" : "Item"), obj: template }, 'last', null, false);
                loadChildrenTemplatesToTree(template.ChildrenTemplates, node);
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
        if ($scope.detailsForm.$valid) {
            if ($scope.currentObj != null && ($scope.CurrentExecutingContext == $scope.ExecutingContexts.None || $scope.CurrentExecutingContext == $scope.ExecutingContexts.Renaming)) {
                UserMessagesProvider.displayLoading();
                var distNode = templatesTreeContainer.jstree(true).get_node($scope.currentNodeObject);
                WebsiteManagerDataProvider.updateTemplate($scope.currentObj).then(function (returnValue) {
                    UserMessagesProvider.hideLoading();
                    if (returnValue.data.result == 'true') {
                        distNode.original.obj = returnValue.data.obj;
                        templatesTreeContainer.jstree(true).rename_node($scope.currentNodeObject, $scope.currentObj.Name);
                        UserMessagesProvider.successHandler();
                    }
                    else {
                        UserMessagesProvider.errorHandler(999, returnValue.data.message);
                        templatesTreeContainer.jstree(true).rename_node($scope.currentNodeObject, distNode.original.obj.Name);
                        if ($scope.CurrentExecutingContext == $scope.ExecutingContexts.Renaming) {
                            $scope.currentObj.Name = distNode.original.obj.Name;
                        }
                    }
                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                }, function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                    templatesTreeContainer.jstree(true).rename_node($scope.currentNodeObject, distNode.original.obj.Name);
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

    $scope.saveCode = function () {
        if ($scope.currentObj) {
            var view = codeArea.getValue();
            UserMessagesProvider.displayLoading();
            WebsiteManagerDataProvider.updateTemplateView($scope.currentObj, { PostedView: view }).then(function (data) {
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
        $scope.state = $scope.DetailsPanelStates.TemplateDetails;
    }

    $scope.displayCode = function () {
        if ($scope.currentObj) {
            UserMessagesProvider.displayLoading();
            WebsiteManagerDataProvider.getTemplateView($scope.currentObj).then(function (data) {
                if (!data.data.startWith("Error!")) {
                    codeArea.setValue(data.data);
                    $scope.state = $scope.DetailsPanelStates.TemplateCode;
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

    $scope.addAdminScripts = function () {
        codeArea.insert('@if(ViewBag.EditingMode==true)***\n');
        codeArea.find('@if(ViewBag.EditingMode==true){@Styles.Render("~/Content/bootstrap")@Scripts.Render("~/bundles/jquery")@Scripts.Render("~/bundles/angularJS")@Scripts.Render("~/bundles/dconfig_shared")@Scripts.Render("~/bundles/dconfig_EditingMode")}');
        codeArea.replace('\n');
        codeArea.find('@if(ViewBag.EditingMode==true)***\n');
        codeArea.replace('@if(ViewBag.EditingMode==true){@Styles.Render("~/Content/bootstrap")@Scripts.Render("~/bundles/jquery")@Scripts.Render("~/bundles/angularJS")@Scripts.Render("~/bundles/dconfig_shared")@Scripts.Render("~/bundles/dconfig_EditingMode")}\n');
    }

    $scope.addChildRenderPlace = function () {
        codeArea.insert('@RenderBody***\n');
        codeArea.find('@RenderBody()');
        codeArea.replace('\n');
        codeArea.find('@RenderBody***\n');
        codeArea.replace('@RenderBody()\n');
    }

    $scope.addMetaTagsRenderPlace = function () {
        codeArea.insert('@Html.RenderContentMetaTags***\n');
        codeArea.find('@Html.RenderContentMetaTags(Model)');
        codeArea.replace('\n');
        codeArea.find('@Html.RenderContentMetaTags***\n');
        codeArea.replace('@Html.RenderContentMetaTags(Model)\n');
    }

    $scope.addReCapatchaScript = function () {
        codeArea.insert('@Html.RenderCapatchaScript***\n');
        codeArea.find('@Html.RenderCapatchaScript()');
        codeArea.replace('\n');
        codeArea.find('@Html.RenderCapatchaScript***\n');
        codeArea.replace('@Html.RenderCapatchaScript()\n');
    }

    $scope.addReCapatchaInput = function () {
        codeArea.insert('@Html.RenderCapatchaCheckerInput()');
    }

    function addReCapatchaScriptCheckerSelectorWindow() {
        var modalInstance = $modal.open({
            templateUrl: 'ItemsSelector.html',
            controller: itemsSelectorController,
            size: 'sm',
            resolve: {
                listOfItems: function () {
                    return $scope.forms;
                }
            }
        });
        modalInstance.result.then(function (obj) {
            codeArea.insert('@Html.RenderCapatchaCheckerScript("' + obj.Name + '")');
        }, function () {
        });
    }
    $scope.addReCapatchaScriptChecker = function () {
        if ($scope.forms == undefined) {
            UserMessagesProvider.displayLoading();
            WebsiteManagerDataProvider.getForms().then(function (data) {
                UserMessagesProvider.hideLoading();
                $scope.forms = data.data;
                addReCapatchaScriptCheckerSelectorWindow();
            }, function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
                return;
            });
        } else {
            addReCapatchaScriptCheckerSelectorWindow();
        }
    }

    function renderViewTypeFieldSelectorWindow() {
        var modalInstance = $modal.open({
            templateUrl: 'ItemsSelector.html',
            controller: itemsSelectorController,
            size: 'sm',
            resolve: {
                listOfItems: function () {
                    return $scope.fields;
                }
            }
        });
        modalInstance.result.then(function (obj) {
            switch (obj.Type) {
                case 'String':
                case 'Date':
                case 'Boolean':
                case 'Number':
                case 'String - Multiple Lines':
                case 'Predefined List - Radio Buttons':
                case 'Predefined List - Filter/Select':
                case 'Predefined List - Checkboxes':
                case 'Predefined List':
                case 'Rich Text Box':
                case 'Password':
                    codeArea.insert('@Html.RenderStringData(Model,' + obj.Id + ')\n');//@Model[' + obj.Id + ']\n
                    break;
                case 'Content Url':
                    codeArea.insert('@Html.Action("RenderContent","WebsiteContentAPI" ,new { Path = @Model[' + obj.Id + '] })\n');
                    break;
                case 'Image':
                    codeArea.insert('<img src="@Model[' + obj.Id + '] />\n');
                    break;
                default:
                    codeArea.insert('@Html.RenderCustomData(Model,' + obj.Id + ')\n');
            }
        }, function () {
        });
    }
    $scope.renderViewTypeField = function () {
        UserMessagesProvider.displayLoading();
        WebsiteManagerDataProvider.getTemplateViewTypeFields($scope.currentObj).then(function (data) {
            UserMessagesProvider.hideLoading();
            $scope.fields = data.data;
            renderViewTypeFieldSelectorWindow();
        }, function (errorData, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
            return;
        });
    }

    function addScriptSelectorWindow() {
        var modalInstance = $modal.open({
            templateUrl: 'ItemsSelector.html',
            controller: itemsSelectorController,
            size: 'sm',
            resolve: {
                listOfItems: function () {
                    return $scope.scripts;
                }
            }
        });
        modalInstance.result.then(function (obj) {
            codeArea.insert('<script type="text/javascript" src="' + obj.Path + '"></script>\n');
        }, function () {
        });
    }
    $scope.addScript = function () {
        if ($scope.scripts == undefined) {
            UserMessagesProvider.displayLoading();
            WebsiteManagerDataProvider.getScripts().then(function (data) {
                UserMessagesProvider.hideLoading();
                $scope.scripts = data.data;
                addScriptSelectorWindow();
            }, function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
                return;
            });
        } else {
            addScriptSelectorWindow();
        }
    }

    function addStyleSheetSelectorWindow() {
        var modalInstance = $modal.open({
            templateUrl: 'ItemsSelector.html',
            controller: itemsSelectorController,
            size: 'sm',
            resolve: {
                listOfItems: function () {
                    return $scope.styles;
                }
            }
        });
        modalInstance.result.then(function (obj) {
            codeArea.insert('<link href="' + obj.Path + '" rel="stylesheet" />\n');
        }, function () {
        });
    }
    $scope.addStyleSheet = function () {
        if ($scope.styles == undefined) {
            UserMessagesProvider.displayLoading();
            WebsiteManagerDataProvider.getStyles().then(function (data) {
                UserMessagesProvider.hideLoading();
                $scope.styles = data.data;
                addStyleSheetSelectorWindow();
            }, function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
                return;
            });
        } else {
            addStyleSheetSelectorWindow();
        }
    }

    function addScriptsBundleSelectorWindow() {
        var modalInstance = $modal.open({
            templateUrl: 'ItemsSelector.html',
            controller: itemsSelectorController,
            size: 'sm',
            resolve: {
                listOfItems: function () {
                    return $scope.scriptsBundles;
                }
            }
        });
        modalInstance.result.then(function (obj) {
            codeArea.insert('@Scripts.Render("~/bundles/' + obj.Name + '")');
        }, function () {
        });
    }
    $scope.addScriptBundle = function () {
        if ($scope.scriptsBundles == undefined) {
            UserMessagesProvider.displayLoading();
            WebsiteManagerDataProvider.getScriptsBundles().then(function (data) {
                UserMessagesProvider.hideLoading();
                $scope.scriptsBundles = data.data;
                addScriptsBundleSelectorWindow();
            }, function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
                return;
            });
        } else {
            addScriptsBundleSelectorWindow();
        }
    }

    function addStylesBundleSelectorWindow() {
        var modalInstance = $modal.open({
            templateUrl: 'ItemsSelector.html',
            controller: itemsSelectorController,
            size: 'sm',
            resolve: {
                listOfItems: function () {
                    return $scope.stylesBundles;
                }
            }
        });
        modalInstance.result.then(function (obj) {
            codeArea.insert('@Styles.Render("~/Content/' + obj.Name + '")');
        }, function () {
        });
    }
    $scope.addStyleBundle = function () {
        if ($scope.stylesBundles == undefined) {
            UserMessagesProvider.displayLoading();
            WebsiteManagerDataProvider.getStylesBundles().then(function (data) {
                UserMessagesProvider.hideLoading();
                $scope.stylesBundles = data.data;
                addStylesBundleSelectorWindow();
            }, function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
                return;
            });
        } else {
            addStylesBundleSelectorWindow();
        }
    }

    function addPartialViewSelectorWindow() {
        var modalInstance = $modal.open({
            templateUrl: 'ItemsSelector.html',
            controller: itemsSelectorController,
            size: 'sm',
            resolve: {
                listOfItems: function () {
                    return $scope.partialViews;
                }
            }
        });
        modalInstance.result.then(function (obj) {
            codeArea.insert('@Html.Partial("' + obj.Path + '", Model)');
        }, function () {
        });
    }
    $scope.addPartialView = function () {
        if ($scope.partialViews == undefined) {
            UserMessagesProvider.displayLoading();
            WebsiteManagerDataProvider.getTemplates().then(function (data) {
                UserMessagesProvider.hideLoading();
                $scope.partialViews = data.data;
                addPartialViewSelectorWindow();
            }, function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
                return;
            });
        } else {
            addPartialViewSelectorWindow();
        }
    }

    function addFormSelectorWindow() {
        var modalInstance = $modal.open({
            templateUrl: 'ItemsSelector.html',
            controller: itemsSelectorController,
            size: 'sm',
            resolve: {
                listOfItems: function () {
                    return $scope.forms;
                }
            }
        });
        modalInstance.result.then(function (obj) {
            codeArea.insert('@Html.Action("RenderForm", "FormsAPI", new { Id = ' + obj.Id + ', PageUrl = Model.ActiveContent.UrlFullCode,  })');
        }, function () {
        });
    }
    $scope.addForm = function () {
        if ($scope.forms == undefined) {
            UserMessagesProvider.displayLoading();
            WebsiteManagerDataProvider.getForms().then(function (data) {
                UserMessagesProvider.hideLoading();
                $scope.forms = data.data;
                addFormSelectorWindow();
            }, function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
                return;
            });
        } else {
            addFormSelectorWindow();
        }
    }

    initialize();
});
var createTemplateController = function ($scope, viewTypes, $modalInstance, parentId, UMP) {
    $scope.newNode = { LayoutTemplateId: parentId };
    $scope.viewTypes = viewTypes;
    $scope.NodeTypes = [
        { Name: 'Template', Value: false },
        { Name: 'Templates Container', Value: true }
    ];
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