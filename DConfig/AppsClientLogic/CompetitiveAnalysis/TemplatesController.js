angular.module('DConfig').controllerProvider.register('CompetitiveAnalysis.TemplatesController', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, CompetitiveAnalysisDataProvider, EventsProvider, $modal, scopeService) {
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
    $scope.BrandFactoryTypes = [
        { Name: 'Own', Value: 'Own' },
        { Name: 'Supplier', Value: 'Supplier' },
        { Name: 'Competitor', Value: 'Competitor' },
    ];
    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
    $scope.state = $scope.DetailsPanelStates.None;
    var templatesTreeContainer = $('#templatesTreeContainer');
    var formsSearchBox = $('formsSearchBox');

    function initialize() {
        BreadCrumpsProvider.breadCrumps.path = ['Competitive Analysis', 'Product Types'];
        UserMessagesProvider.displayProgress(2);
        CompetitiveAnalysisDataProvider.getPropertiesGroups().then(function (data) {
            $scope.PropertiesGroups = data.data;
            UserMessagesProvider.increaseProgress();
        }, function (data, status, headers, config) {
            UserMessagesProvider.increaseProgress();
            UserMessagesProvider.errorHandler(status);
        });
        CompetitiveAnalysisDataProvider.getTemplatesTree().then(function (data) {
            $scope.templatesTree = data.data;
            templatesTreeContainer
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
                                    CompetitiveAnalysisDataProvider.deleteProductsTemplate($scope.currentObj).then(function (returnValue) {
                                        UserMessagesProvider.hideLoading();
                                        if (returnValue.data.result == 'true') {
                                            templatesTreeContainer.jstree(true).delete_node($scope.currentNodeObject);
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
            formsSearchBox.keyup(function () {
                if (to) { clearTimeout(to); }
                to = setTimeout(function () {
                    var v = formsSearchBox.val();
                    templatesTreeContainer.jstree(true).search(v);
                }, 250);
            });
            UserMessagesProvider.increaseProgress();
        }, function (data, status, headers, config) {
            UserMessagesProvider.increaseProgress();
            UserMessagesProvider.errorHandler(status);
        });
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
                $scope.state = $scope.DetailsPanelStates.GroupDetails;
                $scope.currentObj.PropertiesGroups = [];
                if ($scope.PropertiesGroups != undefined) {
                    for (i = 0; i < $scope.PropertiesGroups.length; i++) {
                        var groupExists = false;
                        for (j = 0; j < $scope.currentObj.Properties.length; j++) {
                            if ($scope.currentObj.Properties[j].GroupId == $scope.PropertiesGroups[i].Id) {
                                if (!groupExists) {
                                    groupExists = true;
                                    $scope.currentObj.PropertiesGroups.push({ Id: $scope.PropertiesGroups[i].Id, Name: $scope.PropertiesGroups[i].Name, Properties: [] });
                                }
                            }
                        }
                    }
                    for (i = 0; i < $scope.currentObj.PropertiesGroups.length; i++) {
                        for (j = 0; j < $scope.currentObj.Properties.length; j++) {
                            if ($scope.currentObj.Properties[j].GroupId == $scope.currentObj.PropertiesGroups[i].Id) {
                                var isHighlight = false;
                                var invisibileToFactoryTypes = '';
                                for (k = 0; k < $scope.currentObj.PropertiesRelations.length; k++) {
                                    if ($scope.currentObj.PropertiesRelations[k].PropertyId == $scope.currentObj.Properties[j].Id) {
                                        isHighlight = $scope.currentObj.PropertiesRelations[k].IsHighlight;
                                        invisibileToFactoryTypes = $scope.currentObj.PropertiesRelations[k].InvisibileToFactoryTypes;
                                        break;
                                    }
                                }
                                $scope.currentObj.Properties[j].IsHighlight = isHighlight;
                                $scope.currentObj.Properties[j].InvisibileToFactoryTypes = invisibileToFactoryTypes;
                                $scope.currentObj.PropertiesGroups[i].Properties.push(jQuery.extend(true, {}, $scope.currentObj.Properties[j]));
                            }
                        }
                    }

                }
            }
        }
    }

    $scope.createRootNode = function () {
        $scope.CurrentExecutingContext = $scope.ExecutingContexts.Creating;
        $scope.currentObj = {};
        var modalInstance = $modal.open({
            templateUrl: 'createForm.html',
            controller: createTemplateController,
            size: 'lg',
            resolve: {
                UMP: function () { return UserMessagesProvider; }
            }
        });
        modalInstance.result.then(function (newNode) {
            UserMessagesProvider.displayLoading();
            CompetitiveAnalysisDataProvider.createProductsTemplate(newNode).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    templatesTreeContainer.jstree(true).create_node('#', { text: newNode.Name, obj: returnValue.data.obj }, 'last', null, false);
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
                var distNode = templatesTreeContainer.jstree(true).get_node($scope.currentNodeObject);
                CompetitiveAnalysisDataProvider.updateProductsTemplate($scope.currentObj).then(function (returnValue) {
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

    $scope.displayGeneralDetails = function () {
        $scope.state = $scope.DetailsPanelStates.GroupDetails;
    }

    $scope.displayProperties = function () {
        $scope.state = $scope.DetailsPanelStates.Properties;
    }

    $scope.saveProperties = function () {
        UserMessagesProvider.displayLoading();
        $scope.currentObj.Properties = [];
        var distNode = templatesTreeContainer.jstree(true).get_node($scope.currentNodeObject);
        if ($scope.PropertiesGroups != undefined) {
            for (i = 0; i < $scope.currentObj.PropertiesGroups.length; i++) {
                for (ii = 0; ii < $scope.currentObj.PropertiesGroups[i].Properties.length; ii++) {
                    var itemExists = false;
                    for (j = 0; j < $scope.currentObj.Properties.length; j++) {
                        if ($scope.currentObj.Properties[j].Id == $scope.currentObj.PropertiesGroups[i].Properties[ii].Id) {
                            itemExists = true;
                            break;
                        }
                    }
                    if (!itemExists) {
                        $scope.currentObj.Properties.push(jQuery.extend(true, {}, $scope.currentObj.PropertiesGroups[i].Properties[ii]));
                    }
                }
            }
        }
        CompetitiveAnalysisDataProvider.updateTemplateProperties($scope.currentObj.Id, $scope.currentObj.Properties).then(function (returnValue) {
            UserMessagesProvider.hideLoading();
            if (returnValue.data.result == 'true') {
                distNode.original.obj = returnValue.data.obj;
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
    }

    function addNewPropertySelectorWindow() {
        var modalInstance = $modal.open({
            templateUrl: 'ItemsSelector.html',
            controller: itemsSelectorController,
            size: 'sm',
            resolve: {
                listOfItems: function () {
                    return $scope.PropertiesGroups;
                }
            }
        });
        modalInstance.result.then(function (obj) {
            if ($scope.currentObj.PropertiesGroups == undefined) {
                $scope.currentObj.PropertiesGroups = [];
            }
            if ($scope.PropertiesGroups != undefined) {
                var existsGroupIndex;
                for (i = 0; i < $scope.currentObj.PropertiesGroups.length; i++) {
                    if (obj.Id == $scope.currentObj.PropertiesGroups[i].Id) {
                        existsGroupIndex = i;
                        break;
                    }
                }
                if (existsGroupIndex != undefined)
                    $scope.currentObj.PropertiesGroups.splice(existsGroupIndex, 1);
            }
            $scope.currentObj.PropertiesGroups.push(jQuery.extend(true, {}, obj));
        }, function () {
        });
    }

    $scope.addNewProperty = function () {
        addNewPropertySelectorWindow();
    }

    $scope.removeProperty = function (groupId, propertyId) {
        var index1;
        var index2;
        for (i = 0; i < $scope.currentObj.PropertiesGroups.length; i++) {
            if ($scope.currentObj.PropertiesGroups[i].Id == groupId) {
                index1 = i;
                for (j = 0; j < $scope.currentObj.PropertiesGroups[i].Properties.length; j++) {
                    if ($scope.currentObj.PropertiesGroups[i].Properties[j].Id == propertyId) {
                        index2 = j;
                        break;
                    }
                }
                break;
            }
        }
        if (index1 != undefined && index2 != undefined)
            $scope.currentObj.PropertiesGroups[index1].Properties.splice(index2, 1);
    }

    $scope.removePropertiesGroup = function (index) {
        $scope.currentObj.PropertiesGroups.splice(index, 1);
    }

    $scope.changeHeightLightState = function (property) {
        if (property.IsHighlight == undefined || !property.IsHighlight) {
            property.IsHighlight = true;
        } else {
            property.IsHighlight = false;
        }
    }

    initialize();
});
var createTemplateController = function ($scope, $modalInstance, UMP) {
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