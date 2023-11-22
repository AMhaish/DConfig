angular.module('DConfig').controllerProvider.register('FormsManager.PostedFormsController', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, FormsManagerDataProvider, EventsProvider, $modal, scopeService, $filter, $sce) {
    $scope.DetailsPanelStates = {
        None: 0,
        FormInstances: 1,
        FormInstanceDetails: 2
    };
    $scope.instances = [];
    $scope.instancesCols = [];
    $scope.mySelections = [];
    $scope.gridOptions = {
        showFilter: true,
        showColumnMenu: true,
        showGroupPanel: true,
        enableColumnResize: true,
        showSelectionCheckbox: true,
        selectWithCheckboxOnly: true,
        data: 'instances',
        columnDefs: 'instancesCols',
        enablePinning: true,
        enableCellSelection: true,
        showFooter: true,
        selectedItems: $scope.mySelections
    };
    $scope.state = $scope.DetailsPanelStates.None;
    var formsTreeContainer = $('#formsTreeContainer');
    var formsSearchBox = $('formsSearchBox');

    function initialize() {
        BreadCrumpsProvider.breadCrumps.path = ['Forms Manager', 'Posted Forms'];
        UserMessagesProvider.displayProgress(1);
        FormsManagerDataProvider.getRootForms().then(function (data) {
            $scope.formsTree = data.data;
            formsTreeContainer
                .on("create_node.jstree", function (e, data) { })
                .on("rename_node.jstree", function (e, data) { })
                .on("move_node.jstree", function () { })
                .on("copy_node.jstree", function () { })
                .on("cut.jstree", function () { })
                .on("copy.jstree", function () { })
                .on("paste.jstree", function () { })
                .on("changed.jstree", function (e, data) { scopeService.safeApply($scope, function () { $scope.displayInstances(e, data); }); })
                .jstree({
                    "plugins": ["search", "sort", "state", "types", "unique", "wholerow"],
                    "core": {
                        "data": data.data,
                        "check_callback": true,
                        "multiple": false,
                        'themes': {
                            'responsive': true
                        },
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
                    }
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

    $scope.displayInstances = function (e, data) {
        if (data.node != undefined) {
            if (data.node.original != undefined) {
                $scope.currentNodeObject = data.node;
                $scope.currentObj = jQuery.extend(true, {}, $scope.currentNodeObject.original.obj);
                $scope.state = $scope.DetailsPanelStates.FormInstances;
                $scope.currentObj.AllFormFields = [];
                $scope.currentObj.AllFormFields.push({ field: 'UserName', displayName: 'User Name' });
                $scope.currentObj.AllFormFields.push({ field: 'CreateDate', displayName: 'Post Date', Type: 'Date' });
                for (k = 0; k < $scope.currentObj.FormFields.length; k++) {
                    $scope.currentObj.AllFormFields.push({ field: 'F' + $scope.currentObj.FormFields[k].Id, displayName: $scope.currentObj.FormFields[k].Name, Type: $scope.currentObj.FormFields[k].Type });
                }
                for (i = 0; i < $scope.currentObj.ChildrenForms.length; i++) {
                    for (j = 0; j < $scope.currentObj.ChildrenForms[i].FormFields.length; j++) {
                        $scope.currentObj.AllFormFields.push({ field: 'F' + $scope.currentObj.ChildrenForms[i].FormFields[j].Id.toString(), displayName: $scope.currentObj.ChildrenForms[i].FormFields[j].Name, Type: $scope.currentObj.FormFields[k].Type });
                    }
                }
                $scope.instancesCols = $scope.currentObj.AllFormFields;
                UserMessagesProvider.displayLoading();
                FormsManagerDataProvider.getFormInstances($scope.currentObj.Id).then(function (returnValue) {
                    var data = returnValue.data;
                    $scope.currentObj.instacnes = [];
                    $scope.instances = [];
                    for (l = 0; l < data.length; l++) {
                        var obj = {};
                        obj['Id'] = data[l].Id;
                        obj['CreateDate'] = data[l].CreateDate;
                        if (data[l].UserId != null && data[l].User != null)
                            obj['UserName'] = data[l].User.UserName;
                        for (ll = 0; ll < data[l].FieldsValues.length; ll++) {
                            obj['F' + data[l].FieldsValues[ll].FieldId.toString()] = data[l].FieldsValues[ll].Value.toString();
                        }
                        if (data[l].ChildrenInstances != undefined) {
                            for (i = 0; i < data[l].ChildrenInstances.length; i++) {
                                for (lll = 0; lll < data[l].ChildrenInstances[i].FieldsValues.length; lll++) {
                                    obj['F' + data[l].ChildrenInstances[i].FieldsValues[lll].FieldId.toString()] = data[l].ChildrenInstances[i].FieldsValues[lll].Value.toString();
                                }
                            }
                        }
                        $scope.currentObj.instacnes.push(obj);
                    }
                    $scope.instances = $scope.currentObj.instacnes;
                    UserMessagesProvider.hideLoading();
                }, function (data, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                });
            }
        }
    }

    $scope.saveChanges = function () {
        if ($scope.detialsForm.$valid) {
            if ($scope.currentObj != null && ($scope.CurrentExecutingContext == $scope.ExecutingContexts.None || $scope.CurrentExecutingContext == $scope.ExecutingContexts.Renaming)) {
                UserMessagesProvider.displayLoading();
                var distNode = formsTreeContainer.jstree(true).get_node($scope.currentNodeObject);
                FormsManagerDataProvider.updateForm($scope.currentObj).then(function (returnValue) {
                    UserMessagesProvider.hideLoading();
                    if (returnValue.data.result == 'true') {
                        distNode.original.obj = returnValue.data.obj;
                        formsTreeContainer.jstree(true).rename_node($scope.currentNodeObject, $scope.currentObj.Name);
                        $scope.closeDetailsForm();
                    }
                    else {
                        UserMessagesProvider.errorHandler(999, returnValue.message);
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

    $scope.displayInstanceDetails = function () {
        $scope.state = $scope.DetailsPanelStates.FormInstanceDetails;
    }

    $scope.displayFormInstances = function () {
        $scope.state = $scope.DetailsPanelStates.FormInstances;
    }

    $scope.removeInstances = function () {
        UserMessagesProvider.confirmHandler("Are you sure you want to delete these posted forms? you won't be able to restore them later.", function () {
            angular.forEach($filter('filter')($scope.instances, { isSelected: 'true' }), function (instance) {
                UserMessagesProvider.displayLoading();
                FormsManagerDataProvider.deleteFormInstance(instance).then(function (returnValue) {
                    UserMessagesProvider.hideLoading();
                    if (returnValue.data.result == 'true') {
                        $scope.instances = $filter('filter')($scope.instances, function (element) { return element.Id != instance.Id; });
                    }
                    else {
                        UserMessagesProvider.errorHandler(999, returnValue.data.message);
                    }
                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                }, function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                });
            });
            $scope.mySelections.splice(0, $scope.mySelections.length);
        }, null);
    };

    $scope.getHeader = function () {
        var columns = [];
        for (i = 0; i < $scope.instancesCols.length; i++) {
            columns.push($scope.instancesCols[i].displayName);
        }
        return columns;
    }

    $scope.showImage = function (value) {
        UserMessagesProvider.imageDisplayer(value);
    }

    $scope.printSelected = function () {
        angular.forEach($filter('filter')($scope.instances, { isSelected: 'true' }), function (instance) {
            console.log(instance);
            FormsManagerDataProvider.printForm(instance.Id);
        });
    }

    $scope.IsSelectAll = true;
    $scope.selectAll = function (displayedData, toggleVal) {
        //$scope.mySelections = $scope.instances;
        displayedData.forEach(function (val) {
            val.isSelected = toggleVal;
        });
        $scope.IsSelectAll = !$scope.IsSelectAll;
    }

    initialize();
});