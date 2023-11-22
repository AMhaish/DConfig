angular.module('DConfig').controllerProvider.register('WebsiteManager.Quick_ContentController', function ($scope, $filter, $location, $window, BreadCrumpsProvider, UserMessagesProvider, WebsiteManagerDataProvider, EventsProvider, $modal, scopeService, filterFilter, IntentsProvider) {
    $scope.DetailsPanelStates = {
        None: 0,
        SubContents: 1,
        CreateContent: 3,
        UpdateSubContent: 4,
        ViewingContent: 5
    };
    $scope.ExecutingContexts = {
        None: 0,
        Deleting: 1,
        Creating: 2,
        Renaming: 3,
        Updating: 4,
        Appending: 5,
        CreatingRoot: 6,
        CreatingContentAndInstance: 7
    };
    $scope.state = $scope.DetailsPanelStates.None;
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
    $scope.search = { keyword: ''};

    $scope.subContentManager = {
        subContentsLevel: 0,
        subContents: [],
        subContentParentId: [],
        subContentsBreadCrump: []
    }
    $scope.stages = [];
    $scope.nextStages = [];
    $scope.tableState = {};
    $scope.numOfItemsPerPage = 25;
    $scope.__ItemsPerSearch = 25;
    $scope.__ItemsPerTable = 25;
    function initialize() {
        BreadCrumpsProvider.breadCrumps.path = ['Website Manager', 'Quick Contents'];
        UserMessagesProvider.displayProgress(3);
        WebsiteManagerDataProvider.getQuickContentList().then(function (data) {
            $scope.contents = data.data;
            UserMessagesProvider.increaseProgress();
        }, function (data, status, headers, config) {
            UserMessagesProvider.errorHandler(status);
            UserMessagesProvider.increaseProgress();
        });
        WebsiteManagerDataProvider.getViewTypes().then(function (data) {
            $scope.viewTypes = data.data;
            UserMessagesProvider.increaseProgress();
        }, function (data, status, headers, config) {
            UserMessagesProvider.errorHandler(status);
            UserMessagesProvider.increaseProgress();
        });
        WebsiteManagerDataProvider.getStagesbyRole().then(function (data) {
            $scope.stages = data.data;
            UserMessagesProvider.increaseProgress();
        }, function (data, status, headers, config) {
            UserMessagesProvider.errorHandler(status);
            UserMessagesProvider.increaseProgress();
        });
    }

    function returnCurrentTableItem(index) {
        //return filterFilter(filterFilter(filterFilter(filterFilter(filterFilter($scope.subContent, { CreateDate: $scope.date_filter }), { DueDate: $scope.due_date_filter }), { Online: $scope.published }), { Name: $scope.Name }), { Id: $scope.Id })[index];
        return $scope.subContents[index];
    }

    $scope.refreshTable = function () {
        $scope.callServer($scope.tableState);
    };

    $scope.callServer = function callServer(tableState) {
        $scope.isLoading = true;
        $scope.tableState = tableState;
        UserMessagesProvider.displayLoading();

        WebsiteManagerDataProvider.getContentChildrenCount($scope.currentObj.Id, $scope.search.keyword === undefined || $scope.search.keyword === null ? null : $scope.search.keyword).then(function (data) {
            $scope.nums = data.data;
            tableState.pagination.numberOfPages = Math.ceil(data.data / $scope.__ItemsPerTable);          
            $scope.loadData(tableState);
        }, function (data, status, headers, config) {
            UserMessagesProvider.errorHandler(data.status);
        });

    };

    $scope.loadData = function (tableState) {
        $scope.isLoading = true;
        var pagination = tableState.pagination;
        var page = pagination.start / $scope.__ItemsPerTable + 1;
        UserMessagesProvider.displayLoading();

        WebsiteManagerDataProvider.getContentChildren($scope.currentObj.Id, $scope.__ItemsPerSearch, $scope.__ItemsPerSearch * (page - 1), $scope.search.keyword === undefined || $scope.search.keyword === null ? null : $scope.search.keyword).then(function (data) {
            $scope.subContents = data.data;
            UserMessagesProvider.hideLoading();
        }, function (data, status, headers, config) {
            UserMessagesProvider.errorHandler(status);
            UserMessagesProvider.hideLoading();
        });
    };

    $scope.CreateContent = function () {
        if ($scope.currentObj.PossibleChildViewTypes != undefined && $scope.currentObj.PossibleChildViewTypes.length > 0) {
            $scope.state = $scope.DetailsPanelStates.CreateContent;
            $scope.UpdatedContent = {
                Id: 0,
                Name: "",
                ViewTypeId: $scope.currentObj.PossibleChildViewTypes[0].Id,
                Online: true,
                PlentyChildren: false,
                ContentType: 0,
                ContentInstances: [],
                StageId: null,
                //ParentId: $scope.currentObj.Id
                ParentId: $scope.subContentManager.subContentParentId[$scope.subContentManager.subContentsLevel - 1] ? $scope.subContentManager.subContentParentId[$scope.subContentManager.subContentsLevel - 1] : $scope.currentObj.Id
            }
            for (i = 0; i < $scope.viewTypes.length; i++) {
                if ($scope.currentObj.PossibleChildViewTypes[0].Id == $scope.viewTypes[i].Id) {
                    $scope.currentObj.PossibleViewTemplates = $scope.viewTypes[i].TypeTemplates;
                    break;
                }
            }
            //$scope.CreateInstance($scope.Languages[0].Value, $scope.Languages[0].Name);
        } else {
            UserMessagesProvider.errorHandler(999, "No possible children can be defined under this content");
        }
    };

    $scope.CreateInstance = function (value, Name) {
        var NotExistedLanguage = true;
        angular.forEach($scope.UpdatedContent.ContentInstances, function (instance) {
            if (value == instance.Language) {
                NotExistedLanguage = false;
            }
        });
        if (NotExistedLanguage == true) {
            $scope.UpdatedContent.ContentInstances.push({
                Id: 0,
                Name: "",
                Title: "",
                Language: value,
                languageName: Name,
                MetaDescription: "",
                MetaKeywords: "",
                Version: 0,
                DownloadPath: "",
                DownloadName: "",
                Online: true,
                ViewTemplateId: "",
                FieldsValues: [],
                StageId: null,
                DueDate: null
            });

        }
        else {
            UserMessagesProvider.errorHandler(status);
        }
        if ($scope.currentObj.PossibleChildViewTypes != undefined && $scope.currentObj.PossibleChildViewTypes.length > 0) {
            for (i = 0; i < $scope.currentObj.PossibleChildViewTypes[0].ViewFields.length; i++) {
                var viewField = jQuery.extend(true, {}, $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i]);
                viewField.Value = '';
                $scope.UpdatedContent.ContentInstances[$scope.UpdatedContent.ContentInstances.length - 1].FieldsValues.push(viewField);
            }
        }
        else {
            UserMessagesProvider.errorHandler(999, "No child view type defined for this content");
        }
    };

    $scope.closeDetailsForm = function () {
        $scope.currentObj = null;
        $scope.state = $scope.DetailsPanelStates.None;
        $scope.selectedIndex = undefined;
        $scope.subContentManager = {
            subContentsLevel: 0,
            subContents: [],
            subContentParentId: [],
            subContentsBreadCrump: []
        }
    };

    $scope.closeInstanceEditForm = function () {
        $scope.state = $scope.DetailsPanelStates.SubContents;
    };

    $scope.displayDetails = function (index) {
        $scope.search.keyword = '';
        if ($scope.state == $scope.DetailsPanelStates.SubContents && $scope.selectedIndex != undefined && index == $scope.selectedIndex) {
            $scope.subContentManager = {
                subContentsLevel: 0,
                subContents: [],
                subContentParentId: [],
                subContentsBreadCrump: []
            }
            $scope.selectedIndex = undefined;
            $scope.closeDetailsForm();
        } else {
            $scope.selectedIndex = index;
            $scope.currentObj = jQuery.extend(true, {}, filterFilter($scope.contents, $scope.searchText)[index]);
            $scope.state = $scope.DetailsPanelStates.SubContents;   
            UserMessagesProvider.displayLoading();
            WebsiteManagerDataProvider.getContentChildren($scope.currentObj.Id, $scope.__ItemsPerSearch, $scope.__ItemsPerSearch * (1 - 1), $scope.search.keyword === undefined || $scope.search.keyword === null ? null : $scope.search.keyword).then(function (data) {
                $scope.subContents = data.data;
                UserMessagesProvider.hideLoading();
            }, function (data, status, headers, config) {
                UserMessagesProvider.errorHandler(status);
                UserMessagesProvider.hideLoading();
            });         
        }
    };

    $scope.displayDetailsWithoutCloseForm = function (index) {
        $scope.currentObj = jQuery.extend(true, {}, filterFilter($scope.contents, $scope.searchText)[index]);
        $scope.state = $scope.DetailsPanelStates.SubContents;
        UserMessagesProvider.displayLoading();

        WebsiteManagerDataProvider.getContentChildren($scope.currentObj.Id, $scope.__ItemsPerSearch, $scope.__ItemsPerSearch * (1 - 1), $scope.search.keyword === undefined || $scope.search.keyword === null ? null : $scope.search.keyword).then(function (data) {
            $scope.subContents = data.data;
            UserMessagesProvider.hideLoading();
        }, function (data, status, headers, config) {
            UserMessagesProvider.errorHandler(status);
            UserMessagesProvider.hideLoading();
        });
      
    };

    $scope.UpdateButton = function (index) {
        //console.log($scope.viewTypes);
        //console.log($scope.currentObj);

        UserMessagesProvider.displayLoading();
        for (i = 0; i < $scope.viewTypes.length; i++) {
            if ($scope.currentObj.PossibleChildViewTypes[0].Id == $scope.viewTypes[i].Id) {
                //$scope.currentObj.PossibleViewTemplates = $scope.viewTypes[i].TypeTemplates;
                $scope.currentObj.PossibleViewTemplates = $scope.viewTypes[i].ViewFields;
                break;
            }
        }
        $scope.UpdatedContent = jQuery.extend(true, {}, returnCurrentTableItem(index));

        $scope.NextStages = [];
        if ($scope.UpdatedContent.Stage != null && $scope.UpdatedContent.Stage.Id > 0) {
            WebsiteManagerDataProvider.getNextStages($scope.UpdatedContent.Stage.Id).then(function (data) {
                if (data != null)
                    $scope.NextStages = data.data;
            }, function (data, status, headers, config) {
                UserMessagesProvider.errorHandler(status);
            });
        } else {
            if ($scope.stages != null) {
                $scope.NextStages = $scope.stages.slice();
            }
        }

        $scope.state = $scope.DetailsPanelStates.UpdateSubContent;
        $scope.currentUpdatedIndex = index;
        WebsiteManagerDataProvider.getContentInstances($scope.UpdatedContent.Id).then(function (data) {
            $scope.UpdatedContent.ContentInstances = data.data;
            angular.forEach($scope.Languages, function (language) {
                // for getting the language name of each instance  
                for (var i = 0; i < $scope.UpdatedContent.ContentInstances.length; i++) {
                    if (language.Value == $scope.UpdatedContent.ContentInstances[i].Language) {
                        $scope.UpdatedContent.ContentInstances[i].languageName = language.Name;
                    }
                }
            });
            angular.forEach($scope.UpdatedContent.ContentInstances, function (instance) {
                if ($scope.currentObj.PossibleChildViewTypes != undefined && $scope.currentObj.PossibleChildViewTypes.length > 0) {
                    for (i = 0; i < $scope.currentObj.PossibleChildViewTypes[0].ViewFields.length; i++) {
                        var found = false;
                        for (j = 0; j < instance.FieldsValues.length; j++) {
                            if (instance.FieldsValues[j].FieldId == $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].Id) {
                                found = true;
                                instance.FieldsValues[j].Type = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].Type;
                                instance.FieldsValues[j].ViewTypeId = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].ViewTypeId;
                                instance.FieldsValues[j].EnumId = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].EnumId;
                                instance.FieldsValues[j].Enum = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].Enum;
                                instance.FieldsValues[j].Name = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].Name;
                                break;
                            }
                        }
                        if (!found) {
                            var viewField = jQuery.extend(true, {}, $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i]);
                            viewField.Value = '';
                            viewField.FieldId = viewField.Id;
                            instance.FieldsValues.push(viewField);
                        }
                    }
                }
                else {
                    UserMessagesProvider.errorHandler(999, "No child view type defined for this content");
                }
                /*var found = false;
                for (i = 0; i < $scope.currentObj.PossibleChildViewTypes[0].ViewFields.length; i++) {
                    found = false;
                    for (j = 0 ; j < instance.FieldsValues.length; j++) {
                        if (instance.FieldsValues[j].FieldId == $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].Id) {
                            instance.FieldsValues[j].Type = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].Type;
                            instance.FieldsValues[j].ViewTypeId = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].ViewTypeId;
                            instance.FieldsValues[j].EnumId = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].EnumId;
                            instance.FieldsValues[j].Enum = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].Enum;
                            instance.FieldsValues[j].Name = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].Name;
                            break;
                        }
                    }
                }*/
                //WebsiteManagerDataProvider.getcontentinstanceFieldsValues($scope.UpdatedContent.Id).then(function (values) {
                //    instance.ViewFields = values;
                //    UserMessagesProvider.hideLoading();
                //},function (data, status, headers, config) {
                //    UserMessagesProvider.hideLoading();
                //    UserMessagesProvider.errorHandler(status);
                //});
            });
            UserMessagesProvider.hideLoading();
        }, function (data, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });
    };

    $scope.ChildrenButton = function (index) {
        UserMessagesProvider.displayLoading();
        $scope.currentObj = jQuery.extend(true, {}, returnCurrentTableItem(index));
        $scope.state = $scope.DetailsPanelStates.SubContents;
        $scope.currentUpdatedIndex = index;
        $scope.subContentManager.subContents[$scope.subContentManager.subContentsLevel] = $scope.subContents;
        if ($scope.subContentManager.subContentsLevel == 0)
            $scope.subContentManager.subContentsBreadCrump[$scope.subContentManager.subContentsLevel] = "<< " + $scope.currentObj.Name;
        else
            $scope.subContentManager.subContentsBreadCrump[$scope.subContentManager.subContentsLevel] = $scope.currentObj.Name;
        WebsiteManagerDataProvider.getContentChildren($scope.currentObj.Id).then(function (data) {
            $scope.subContents = data.data;
            $scope.subContentManager.subContentParentId[$scope.subContentManager.subContentsLevel] = $scope.currentObj.Id;
            $scope.subContentManager.subContentsLevel++;
            UserMessagesProvider.hideLoading();
        });
    };

    $scope.importContents = function () {
        var modalInstance = $modal.open({
            templateUrl: 'importForm.html',
            controller: importContentsController,
            size: 'lg',
            resolve: {
                UMP: function () { return UserMessagesProvider; },
                IntentsProvider: function () { return IntentsProvider; },
                ContentId: function () {
                    return $scope.currentObj.Id;
                },
                viewTypes: function () {                
                    return $scope.currentObj.PossibleChildViewTypes;
                }
            }
        });
        modalInstance.result.then(function (newNode) {
            UserMessagesProvider.displayLoading();
            WebsiteManagerDataProvider.ImportContentsFromExcel(newNode.ContentId, newNode.ExcelPath, newNode.ViewTypeId ).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.successHandler();
                $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                var modalInstance = $modal.open({
                    templateUrl: 'importReportForm.html',
                    controller: importContentsReportController,
                    size: 'lg',
                    resolve: {
                        UMP: function () { return UserMessagesProvider; },
                        Report: function () { return returnValue.data; }
                    }
                });
                modalInstance.result.then(function () {
                    //$location.reload();
                });
            }, function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
                $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
            });
        }, function () {
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
            $scope.closeDetailsForm();
        });
    }

    $scope.downloadTemplate = function (path) {
        $window.open('/DConfig/WebsiteContentAPI/exportTemplateExcel?ContentId=' + $scope.currentObj.Id + '&ViewTypeId=' + $scope.currentObj.PossibleChildViewTypes[0].Id);
    }

    $scope.ViewingButton = function (index) {
        $scope.PrevViewedContentInstance = [];
        ViewedContent = [];
        previousInstancesVersions = [];

        UserMessagesProvider.displayLoading();
        for (i = 0; i < $scope.viewTypes.length; i++) {
            if ($scope.currentObj.PossibleChildViewTypes[0].Id == $scope.viewTypes[i].Id) {
                $scope.currentObj.PossibleViewTemplates = $scope.viewTypes[i].TypeTemplates;
                break;
            }
        }
        $scope.ViewedContent = jQuery.extend(true, {}, returnCurrentTableItem(index));
        $scope.state = $scope.DetailsPanelStates.ViewingContent;
        $scope.currentUpdatedIndex = index;
        WebsiteManagerDataProvider.getContentInstances($scope.ViewedContent.Id).then(function (data) {
            $scope.ViewedContent.ContentInstances = data.data;
            angular.forEach($scope.Languages, function (language) {
                // for getting the language name of each instance  
                for (var i = 0; i < $scope.ViewedContent.ContentInstances.length; i++) {
                    if (language.Value == $scope.ViewedContent.ContentInstances[i].Language) {
                        $scope.ViewedContent.ContentInstances[i].languageName = language.Name;
                    }
                }
            });
            angular.forEach($scope.ViewedContent.ContentInstances, function (instance) {
                if ($scope.currentObj.PossibleChildViewTypes != undefined && $scope.currentObj.PossibleChildViewTypes.length > 0) {
                    for (i = 0; i < $scope.currentObj.PossibleChildViewTypes[0].ViewFields.length; i++) {
                        var viewField = jQuery.extend(true, {}, $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i]);
                        viewField.Value = '';
                        var found = false;
                        if (instance.FieldsValues !== undefined) {
                            for (j = 0; j < instance.FieldsValues.length; j++) {
                                found = false;
                                if (instance.FieldsValues[j].FieldId == $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].Id) {
                                    instance.FieldsValues[j].Type = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].Type;
                                    instance.FieldsValues[j].ViewTypeId = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].ViewTypeId;
                                    instance.FieldsValues[j].EnumId = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].EnumId;
                                    instance.FieldsValues[j].Enum = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].Enum;
                                    instance.FieldsValues[j].Name = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].Name;
                                    found = true;
                                    break;
                                }
                            }
                            if (!found) {
                                instance.FieldsValues.push(viewField);
                            }
                        }
                    }
                }
                else {
                    UserMessagesProvider.errorHandler(999, "No child view type defined for this content");
                }
                var found = false;
                for (i = 0; i < $scope.currentObj.PossibleChildViewTypes[0].ViewFields.length; i++) {
                    if (instance.FieldsValues !== undefined) {
                        found = false;
                        for (j = 0; j < instance.FieldsValues.length; j++) {
                            if (instance.FieldsValues[j].FieldId == $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].Id) {
                                instance.FieldsValues[j].Type = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].Type;
                                instance.FieldsValues[j].ViewTypeId = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].ViewTypeId;
                                instance.FieldsValues[j].EnumId = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].EnumId;
                                instance.FieldsValues[j].Enum = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].Enum;
                                instance.FieldsValues[j].Name = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].Name;
                                break;
                            }
                        }
                    }
                }
            });
        }, function (data, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });

        WebsiteManagerDataProvider.getcontentinstancesForPrevVersions($scope.ViewedContent.Id).then(function (data) {
            $scope.previousInstancesVersions = data.data;
            if ($scope.previousInstancesVersions != null) {
                angular.forEach($scope.Languages, function (language) {
                    // for getting the language name of each instance 
                    for (var i = 0; i < $scope.previousInstancesVersions.length; i++) {
                        if (language.Value == $scope.previousInstancesVersions[i].Language) {
                            $scope.previousInstancesVersions[i].languageName = language.Name;
                        }
                    }

                });
                angular.forEach($scope.previousInstancesVersions, function (instance) {
                    if ($scope.currentObj.PossibleChildViewTypes != undefined && $scope.currentObj.PossibleChildViewTypes.length > 0) {
                        for (i = 0; i < $scope.currentObj.PossibleChildViewTypes[0].ViewFields.length; i++) {
                            var viewField = jQuery.extend(true, {}, $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i]);
                            viewField.Value = '';
                            if (instance.FieldsValues !== undefined) {
                                var found = false;
                                for (j = 0; j < instance.FieldsValues.length; j++) {
                                    found = false;
                                    if (instance.FieldsValues[j].FieldId == $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].Id) {
                                        instance.FieldsValues[j].Type = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].Type;
                                        instance.FieldsValues[j].ViewTypeId = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].ViewTypeId;
                                        instance.FieldsValues[j].EnumId = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].EnumId;
                                        instance.FieldsValues[j].Enum = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].Enum;
                                        instance.FieldsValues[j].Name = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].Name;
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found) {
                                    instance.FieldsValues.push(viewField);
                                }
                            }
                        }
                    }
                    else {
                        UserMessagesProvider.errorHandler(999, "No child view type defined for this content");
                    }
                    var found = false;
                    for (i = 0; i < $scope.currentObj.PossibleChildViewTypes[0].ViewFields.length; i++) {
                        found = false;
                        if (instance.FieldsValues !== undefined) {
                            for (j = 0; j < instance.FieldsValues.length; j++) {
                                if (instance.FieldsValues[j].FieldId == $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].Id) {
                                    instance.FieldsValues[j].Type = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].Type;
                                    instance.FieldsValues[j].ViewTypeId = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].ViewTypeId;
                                    instance.FieldsValues[j].EnumId = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].EnumId;
                                    instance.FieldsValues[j].Enum = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].Enum;
                                    instance.FieldsValues[j].Name = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[i].Name;
                                    break;
                                }
                            }
                        }
                    }
                    //WebsiteManagerDataProvider.getcontentinstanceFieldsValues($scope.UpdatedContent.Id).then(function (values) {
                    //    instance.ViewFields = values;
                    //    UserMessagesProvider.hideLoading();
                    //},function (data, status, headers, config) {
                    //    UserMessagesProvider.hideLoading();
                    //    UserMessagesProvider.errorHandler(status);
                    //});
                });
            }
            UserMessagesProvider.hideLoading();
        }, function (data, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });
    }

    $scope.downloadFile = function (file) {
        window.open(file, '_blank');
        return false;
    }

    $scope.ParentSubContent = function (index) {
        UserMessagesProvider.displayLoading();
        $scope.state = $scope.DetailsPanelStates.SubContents;

        $scope.subContents = $scope.subContentManager.subContents[index];

        $scope.subContentManager.subContents.splice(index);
        $scope.subContentManager.subContentsBreadCrump.splice(index);
        $scope.subContentManager.subContentsLevel = index;

        UserMessagesProvider.hideLoading();
    }

    $scope.createSubContent = function () {
        if ($scope.UpdatedContent.ContentInstances != null) {
            UserMessagesProvider.displayProgress(1 + $scope.UpdatedContent.ContentInstances.length * 2, function () {
                $scope.closeInstanceEditForm();
                $scope.subContents.push($scope.UpdatedContent);
                $scope.UpdatedContent = undefined;
            });
            $scope.UpdatedContent.UrlName = $scope.UpdatedContent.Name.replace(" ", "-").replace("&", "and").replace("?", "-").replace("\"","-").replace("/","-");
            WebsiteManagerDataProvider.createContentNode($scope.UpdatedContent, false).then(function (returnValue) {
                if (returnValue.data.result == 'true') {
                    $scope.UpdatedContent.Id = returnValue.data.obj.Id;
                    $scope.UpdatedContent.CreateDate = returnValue.data.obj.CreateDate;
                    angular.forEach($scope.UpdatedContent.ContentInstances, function (instance) {
                        instance.ContentId = returnValue.data.obj.Id;
                        WebsiteManagerDataProvider.createContentNode(instance, true).then(function (returnValue) {
                            if (returnValue.data.result == 'true') {
                                instance.Id = returnValue.data.obj.Id;
                                if ($scope.stages.length == 1)
                                    instance.StageId = $scope.stages[0].Id;
                                angular.forEach(instance.FieldsValues, function (field) {
                                    field.FieldId = field.Id;
                                });
                                WebsiteManagerDataProvider.updateContentInstanceFieldValues(instance).then(function (returnValue) {
                                    UserMessagesProvider.increaseProgress();
                                    if (returnValue.data.result == 'true') {
                                    }
                                    else {
                                        UserMessagesProvider.errorHandler(999, returnValue.message);
                                    }
                                }, function (errorData, status, headers, config) {
                                    UserMessagesProvider.hideProgress();
                                    UserMessagesProvider.errorHandler(status);
                                });
                                UserMessagesProvider.increaseProgress();
                            }
                            else {
                                UserMessagesProvider.hideProgress();
                                UserMessagesProvider.errorHandler(999, returnValue.message);
                            }

                        }, function (errorData, status, headers, config) {
                            UserMessagesProvider.hideProgress();
                            UserMessagesProvider.errorHandler(status);
                        });
                    });
                    UserMessagesProvider.increaseProgress();
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.message);
                    UserMessagesProvider.hideProgress();
                }
            }, function (errorData, status, headers, config) {
                UserMessagesProvider.hideProgress();
                UserMessagesProvider.errorHandler(status);
            });
        }
        else {
            UserMessagesProvider.errorHandler(999, "You haven't added any translation for the type of content you are creating");
        }
    }

    $scope.updateSubContent = function () {
        if ($scope.UpdatedContent.ContentInstances != null) {
            UserMessagesProvider.displayProgress(1 + $scope.UpdatedContent.ContentInstances.length * 2, function () {
                $scope.closeInstanceEditForm();
                //returnCurrentTableItem($scope.currentUpdatedIndex) = $scope.UpdatedContent;
                $scope.UpdatedContent = undefined;
            });
            WebsiteManagerDataProvider.updateContentNode($scope.UpdatedContent, "Content").then(function (returnValue) {
                var target = returnCurrentTableItem($scope.currentUpdatedIndex);
                target.Name = $scope.UpdatedContent.Name;
                target.Published = $scope.UpdatedContent.Published;
                target.DueDate = $scope.UpdatedContent.DueDate;
                if (returnValue.data.result == 'true') {
                    UserMessagesProvider.increaseProgress();
                    angular.forEach($scope.UpdatedContent.ContentInstances, function (instance) {
                        if (instance.ContentId == undefined) {
                            instance.ContentId = returnValue.data.obj.Id;
                            WebsiteManagerDataProvider.createContentNode(instance, "ContentVersion").then(function (returnValue) {
                                if (returnValue.data.result == 'true') {
                                    UserMessagesProvider.increaseProgress();
                                    UserMessagesProvider.successHandler();
                                    instance.Id = returnValue.data.obj.Id;
                                    if ($scope.stages.length == 1)
                                        instance.StageId = $scope.stages[0].Id;
                                    angular.forEach(instance.FieldsValues, function (field) {
                                        field.FieldId = field.Id;
                                    });
                                    WebsiteManagerDataProvider.updateContentInstanceFieldValues(instance).then(function (returnValue) {
                                        UserMessagesProvider.increaseProgress();
                                        if (returnValue.data.result == 'true') {
                                        }
                                        else {
                                            UserMessagesProvider.errorHandler(999, returnValue.message);
                                        }
                                    }, function (errorData, status, headers, config) {
                                        UserMessagesProvider.hideProgress();
                                        UserMessagesProvider.errorHandler(status);
                                    });
                                }
                                else {
                                    UserMessagesProvider.hideProgress();
                                    UserMessagesProvider.errorHandler(999, returnValue.message);
                                }

                            }, function (errorData, status, headers, config) {
                                UserMessagesProvider.hideProgress();
                                UserMessagesProvider.errorHandler(status);

                            });
                        } else {
                            WebsiteManagerDataProvider.updateContentNode(instance, "ContentVersion").then(function (returnValue) {
                                if (returnValue.data.result == 'true') {
                                    UserMessagesProvider.increaseProgress();
                                    WebsiteManagerDataProvider.updateContentInstanceFieldValues(instance).then(function (returnValue) {
                                        UserMessagesProvider.increaseProgress();
                                        if (returnValue.data.result == 'true') {
                                        }
                                        else {
                                            UserMessagesProvider.errorHandler(999, returnValue.message);
                                        }
                                    }, function (errorData, status, headers, config) {
                                        UserMessagesProvider.hideProgress();
                                        UserMessagesProvider.errorHandler(status);
                                    });
                                }
                                else {
                                    UserMessagesProvider.hideProgress();
                                    UserMessagesProvider.errorHandler(999, returnValue.message);
                                }

                            }, function (errorData, status, headers, config) {
                                UserMessagesProvider.hideProgress();
                                UserMessagesProvider.errorHandler(status);

                            });
                        }
                    });
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.data.message);
                    UserMessagesProvider.hideProgress();
                }
            }, function (errorData, status, headers, config) {
                UserMessagesProvider.hideProgress();
                UserMessagesProvider.errorHandler(status);
            });
        }
        else {
            UserMessagesProvider.errorHandler(999, "You haven't added any translation for the type of content you are creating");
        }
    }

    $scope.removeLanguage = function (LanguageIndex) {
        UserMessagesProvider.confirmHandler("Are you sure you want to delete this language? you won't be able to restore it later.", function () {
            if ($scope.UpdatedContent.ContentInstances[LanguageIndex].ContentId == undefined) {
                $scope.UpdatedContent.ContentInstances.splice(LanguageIndex, 1);
            } else {
                UserMessagesProvider.displayLoading();
                WebsiteManagerDataProvider.deleteContentNode($scope.UpdatedContent.ContentInstances[LanguageIndex], "ContentVersion").then(function (returnValue) {
                    UserMessagesProvider.hideLoading();
                    if (returnValue.data.result == 'true') {
                        $scope.UpdatedContent.ContentInstances.splice(LanguageIndex, 1);
                        UserMessagesProvider.successHandler();
                    }
                    else {
                        UserMessagesProvider.errorHandler(999, returnValue.data.message);
                    }
                }, function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                });
            }
        }, null);
    }

    $scope.removeInstances = function () {
        UserMessagesProvider.confirmHandler("Are you sure you want to delete these posted forms? you won't be able to restore them later.", function () {
            angular.forEach($filter('filter')($scope.subContents, { isSelected: 'true' }), function (instance) {
                UserMessagesProvider.displayLoading();
                WebsiteManagerDataProvider.deleteContentNode(instance).then(function (returnValue) {
                    UserMessagesProvider.hideLoading();
                    if (returnValue.data.result == 'true') {
                        $scope.subContents = $filter('filter')($scope.subContents, function (element) { return element.Id != instance.Id; });
                        UserMessagesProvider.successHandler();
                    }
                    else {
                        UserMessagesProvider.hideLoading();
                        UserMessagesProvider.errorHandler(999, returnValue.data.message);
                    }

                }, function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                });
            });

        }, null);
    };

    $scope.runContentSelector = function (index, instance) {
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
                        instance.FieldsValues[index].Value = obj.UrlFullCode;
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
                instance.FieldsValues[index].Value = obj.UrlFullCode;
            }, function () {
            });
        }
    }

    $scope.runIntentSelectorForDownloadFile = function (index, instance) {
        ;
        IntentsProvider.startIntent('ImageBrowser', null, function (value) {
            instance.DownloadPath = value;
        }, null);
    }

    $scope.runIntentSelector = function (index, instance) {
        var intentName = $scope.currentObj.PossibleChildViewTypes[0].ViewFields[index].TypeObj.IntentName;
        IntentsProvider.startIntent(intentName, { MultipleImages: $scope.currentObj.PossibleChildViewTypes[0].ViewFields[index].Type == "Multiple Images", Value: $scope.currentObj.PossibleChildViewTypes[0].ViewFields[index].Value }, function (value) {
            instance.FieldsValues[index].Value = value;
        }, null);
    }

    $scope.resetField = function (index, instance) {
        instance.FieldsValues[index].Value = '';
    }

    $scope.IsSelectAll = true;
    $scope.selectAll = function (displayedData, toggleVal) {
        //$scope.mySelections = $scope.instances;
        displayedData.forEach(function (val) {
            val.isSelected = toggleVal;
        });
        $scope.IsSelectAll = !$scope.IsSelectAll;
    }

    $scope.NextStageButton = function (index) {
        UserMessagesProvider.displayLoading();
        $scope.UpdatedContent = jQuery.extend(true, {}, returnCurrentTableItem(index));
        $scope.currentUpdatedIndex = index;
        $scope.NextStages = [];
        //if ($scope.UpdatedContent.Stage != null && $scope.UpdatedContent.Stage.Id > 0) {
        WebsiteManagerDataProvider.getNextStages().then(function (data) {
            if (data.data != null) {
                $scope.NextStages = data.data;
                if ($scope.UpdatedContent.ContentInstances == null || $scope.UpdatedContent.ContentInstances.length < 0) {
                    WebsiteManagerDataProvider.getContentInstances($scope.UpdatedContent.Id).then(function (data) {
                        $scope.UpdatedContent.ContentInstances = data.data;
                        $scope.CreateModalInstanceForNextStage();
                        UserMessagesProvider.hideLoading();
                    }, function (data, status, headers, config) {
                        UserMessagesProvider.hideLoading();
                        UserMessagesProvider.errorHandler(status);
                    });
                } else {
                    UserMessagesProvider.hideLoading();
                    $scope.CreateModalInstanceForNextStage();
                }
            }
            else {
                UserMessagesProvider.errorHandler(999);
                UserMessagesProvider.hideLoading();
            }
        }, function (errorData, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(999);
        });
        /*} else {
            $scope.NextStages = $scope.stages.slice();
            if ($scope.UpdatedContent.ContentInstances != null && $scope.UpdatedContent.ContentInstances) {
                WebsiteManagerDataProvider.getContentInstances($scope.UpdatedContent.Id).then(function (data) {
                    $scope.UpdatedContent.ContentInstances = data;
                    $scope.CreateModalInstanceForNextStage();
                    UserMessagesProvider.hideLoading();
                },function (data, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                });
            } else {
                $scope.CreateModalInstanceForNextStage();
                UserMessagesProvider.hideLoading();
            }
        }*/
    }

    $scope.CreateModalInstanceForNextStage = function () {
        var modalInstance = $modal.open({
            templateUrl: 'NextStages.html',
            controller: stageSelectorController,
            size: 'lg',
            resolve: {
                UMP: function () {
                    return UserMessagesProvider;
                },
                listOfStages: function () {
                    return $scope.NextStages;
                },
                listOfInstances: function () {
                    return $scope.UpdatedContent.ContentInstances;
                }
            }
        });
        modalInstance.result.then(function (obj) {
            UserMessagesProvider.displayLoading();
            WebsiteManagerDataProvider.stagingContentInstance(obj.instanceId, obj.stageId, obj.comments).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    //returnCurrentTableItem($scope.currentUpdatedIndex).CurrentStages = obj.Name;
                    $scope.displayDetailsWithoutCloseForm($scope.selectedIndex)
                    $scope.subContents = $filter('filter')($scope.subContents, function (element) { return element.Id != $scope.UpdatedContent.Id; });
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.data.message);
                    UserMessagesProvider.hideLoading();
                }
            }, function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(999);
            });

        }, function () {
        });
    }

    var _MS_PER_DAY = 1000 * 60 * 60 * 24;
    $scope.DueDateBetween_7_4_Days = function (date) {
        if (date == null || date == undefined)
            return false;
        var a = new Date();
        var b = new Date(date); // Or any other JS date

        var remainingDays = dateDiffInDays(b, a);
        if (remainingDays <= 7 && remainingDays > 3) { // Apply you login on remaining days
            return true;
        } else
            return false;


    }

    $scope.DueDateEqualorLess_4_Days = function (date) {
        if (date == null || date == undefined)
            return false;
        var a = new Date();
        var b = new Date(date); // Or any other JS date

        var remainingDays = dateDiffInDays(b, a);
        if (remainingDays <= 3 && remainingDays >= 0) { // Apply you login on remaining days        
            return true;
        } else
            return false;
    }

    $scope.setContentOfflineForStaging = function (content) {
        if (content.StageId != null) { content.Online = false; }
    }

    // a and b are javascript Date objects
    function dateDiffInDays(first, second) {
        return Math.round((second - first) / _MS_PER_DAY);
    }


    initialize();
});
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
var stageSelectorController = function ($scope, $modalInstance, UMP, listOfStages, listOfInstances) {
    $scope.stages = listOfStages;
    $scope.instances = listOfInstances;
    $scope.container = {};
    $scope.nextStageForm = {};

    $scope.ok = function () {
        //if ($scope.nextStageForm.form.$valid) {
        $modalInstance.close($scope.container);
        //} else {
        //UMP.invalidHandler();
        //}
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}

var importContentsController = function ($scope, $modalInstance, UMP, IntentsProvider, ContentId, viewTypes) {
    $scope.newNode = {};
    $scope.importForm = {};
    $scope.newNode.ContentId = ContentId;
    $scope.viewTypes = viewTypes;
    $scope.ok = function (createForm) {
        if ($scope.importForm.form.$valid) {
            $modalInstance.close($scope.newNode);
        } else {
            UMP.invalidHandler();
        }
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.runIntentSelector = function () {
        IntentsProvider.startIntent("ImageBrowser", null, function (value) {
            $scope.newNode.ExcelPath = value;
        }, null);
    }

    $scope.resetField = function () {
        $scope.newNode.ExcelPath = '';
    }
};

var importContentsReportController = function ($scope, $modalInstance, UMP, Report) {
    $scope.Report = Report;
    console.log(Report);
    $scope.ok = function () {
        $modalInstance.close();
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    function initialize() {
        var messages = "";
        for (var propt in Report.FailedDic) {
            messages += "Line/File (" + propt + ")," + Report.FailedDic[propt] + "\n\n";
        }
        $scope.Report.Messages = messages;
    }

    initialize();
}; 