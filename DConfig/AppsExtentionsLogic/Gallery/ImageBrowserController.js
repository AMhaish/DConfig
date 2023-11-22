angular.module('DConfig').controllerProvider.register('Intents.GalleryImageBrowser', function ($scope, $modalInstance, parameters, BreadCrumpsProvider, UserMessagesProvider, GalleryDataProvider, EventsProvider, $modal, IntentsProvider, scopeService, $upload) {
    $scope.DetailsPanelStates = {
        None: 0,
        FolderDetails: 1,
    };
    $scope.ExecutingContexts = {
        None: 0,
        Deleting: 1,
        Creating: 2,
        Renaming: 3,
        Updating: 4
    };
    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
    var foldersTreeContainer;
    var foldersSearchBox;
    $scope.Selected = { SelectedImages: [] };

    $scope.initialize = function () {
        //Initialize parameters
        $scope.FoldersTreeEnabled = (parameters == null || parameters == undefined || parameters.FoldersTree == undefined || parameters.FoldersTree == null ? true : parameters.FoldersTree);
        //=====================================
        foldersTreeContainer = $('#foldersTree');
        foldersSearchBox = $('#foldersSearchBox');
        UserMessagesProvider.displayLoading();
        GalleryDataProvider.getFoldersTree((parameters != null && parameters != undefined && parameters.RootPath != undefined ? parameters.RootPath : null)).then(function (data) {
            $scope.data = data.data;
            foldersTreeContainer
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
                            'responsive': false
                        },
                        'check_callback': function (operation, node, node_parent, node_position, more) {
                            //operation can be 'create_node', 'rename_node', 'delete_node', 'move_node' or 'copy_node'
                            //in case of 'rename_node' node_position is filled with the new node name
                            if (operation === 'delete_node' && $scope.CurrentExecutingContext == $scope.ExecutingContexts.None) {
                                UserMessagesProvider.confirmHandler("Are you sure you want to delete this folder, all its content will also be deleted?", function () {
                                    UserMessagesProvider.displayLoading();
                                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.Deleting;
                                    GalleryDataProvider.deleteFolder($scope.currentObj.Path).then(function (returnValue) {
                                        UserMessagesProvider.hideLoading();
                                        if (returnValue.data.result == 'true') {
                                            foldersTreeContainer.jstree(true).delete_node($scope.currentNodeObject);
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
                        "Container": {
                            "icon": "jstree-folder"
                        },
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
                            return items;
                        }
                    },
                });
            $scope.closeDetailsForm();
            //Binding Searchbox
            var to = false;
            foldersSearchBox.keyup(function () {
                if (to) { clearTimeout(to); }
                to = setTimeout(function () {
                    var v = foldersSearchBox.val();
                    foldersTreeContainer.jstree(true).search(v);
                }, 250);
            });
            if (parameters != null && parameters != undefined)
                $scope.MultipleImages = parameters.MultipleImages;
            if (parameters != null && parameters != undefined && parameters.RootPath != undefined && $scope.FoldersTreeEnabled == false) {
                $scope.currentObj = $scope.data[0].obj;
                $scope.state = $scope.DetailsPanelStates.FolderDetails;
                UserMessagesProvider.displayLoading();
                GalleryDataProvider.getFiles($scope.currentObj.Path).then(function (data) {
                    $scope.currentObj.Files = data.data;
                    if (parameters.MultipleImages == true) {
                        var pathes = parameters.Value.split(',');
                        for (i = 0; i < pathes.length; i++) {
                            for (j = 0; j < pathes.length; j++) {
                                if ($scope.currentObj.Files[j].Path == pathes[i]) {
                                    $scope.Selected.SelectedImages.push(j);
                                }
                            }
                        }
                    }
                    UserMessagesProvider.hideLoading();
                }, function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                });
            }
            UserMessagesProvider.hideLoading();
        }, function (data, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });
        new ClipboardJS('.copy-link', {
            text: function (trigger) {
                return trigger.getAttribute('aria-label');
            }
        }).on('success', function (e) {
            UserMessagesProvider.notificationHandler('Link copyed successfully');
        });
    }

    $scope.closeDetailsForm = function () {
        foldersTreeContainer.jstree(true).deselect_all(false);
        $scope.currentObj = null;
        $scope.state = $scope.DetailsPanelStates.None;
    }

    $scope.displayDetails = function (e, data) {
        if (data.node != undefined) {
            if (data.node.original != undefined) {
                $scope.currentNodeObject = data.node;
                $scope.currentObj = jQuery.extend(true, {}, $scope.currentNodeObject.original.obj);
                $scope.state = $scope.DetailsPanelStates.FolderDetails;
                UserMessagesProvider.displayLoading();
                GalleryDataProvider.getFiles($scope.currentObj.Path).then(function (data) {
                    $scope.currentObj.Files = data.data;
                    UserMessagesProvider.hideLoading();
                }, function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                });
            }
        }
    }

    $scope.createNode = function (e, data) {
        if ($scope.CurrentExecutingContext == $scope.ExecutingContexts.Creating) {
            var parentNodeObject;
            parentNodeObject = foldersTreeContainer.jstree(true).get_node(foldersTreeContainer.jstree(true).get_parent(data.node));
            $scope.parentObject = parentNodeObject.original.obj;
            $scope.currentObj = {};
            var modalInstance = $modal.open({
                templateUrl: 'galleryext_createForm.html',
                controller: createFolderController,
                size: 'lg',
                resolve: {
                    parentPath: function () {
                        return $scope.parentObject.Path;
                    },
                    UMP: function () { return UserMessagesProvider; }
                }
            });
            modalInstance.result.then(function (newNode) {
                UserMessagesProvider.displayLoading();
                GalleryDataProvider.createFolder(newNode.Path).then(function (returnValue) {
                    UserMessagesProvider.hideLoading();
                    if (returnValue.data.result == 'true') {
                        var distNode = foldersTreeContainer.jstree(true).get_node(data.node);
                        foldersTreeContainer.jstree(true).rename_node(data.node, newNode.Name);
                        distNode.original.obj = newNode;
                        distNode.original.type = 'Container';
                        foldersTreeContainer.jstree(true).set_type(data.node, 'Container');
                        foldersTreeContainer.jstree(true).deselect_node(parentNodeObject, false);
                        foldersTreeContainer.jstree(true).select_node(data.node, false);
                    }
                    else {
                        UserMessagesProvider.errorHandler(999, returnValue.message);
                        foldersTreeContainer.jstree(true).delete_node(data.node);
                    }
                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                }, function (errorData, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(status);
                    foldersTreeContainer.jstree(true).delete_node(data.node);
                    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                });

            }, function () {
                foldersTreeContainer.jstree(true).delete_node(data.node);
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

    $scope.unselectAll = function () {
        $scope.Selected.SelectedImages = [];
    }

    $scope.saveChanges = function () {
        if ($scope.currentObj != null && ($scope.CurrentExecutingContext == $scope.ExecutingContexts.None || $scope.CurrentExecutingContext == $scope.ExecutingContexts.Renaming)) {
            UserMessagesProvider.displayLoading();
            var distNode = foldersTreeContainer.jstree(true).get_node($scope.currentNodeObject);
            var newPath = $scope.currentObj.Path.substring(0, $scope.currentObj.Path.length - distNode.original.obj.Name.length) + $scope.currentObj.Name;
            GalleryDataProvider.updateFolder($scope.currentObj.Path, newPath).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    distNode.original.obj.Name = $scope.currentObj.Name;
                    foldersTreeContainer.jstree(true).rename_node($scope.currentNodeObject, $scope.currentObj.Name);
                    $scope.closeDetailsForm();
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.data.message);
                    foldersTreeContainer.jstree(true).rename_node($scope.currentNodeObject, distNode.original.obj.Name);
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
    }

    $scope.uploadFile = function () {
        var modalInstance = $modal.open({
            templateUrl: 'uploadForm.html',
            controller: uploadFileController,
            size: 'md',
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
                }
            }
        });
        modalInstance.result.then(function (newFile) {
            GalleryDataProvider.getFiles($scope.currentObj.Path).then(function (data) {
                $scope.currentObj.Files = data.data;
                UserMessagesProvider.hideLoading();
            }, function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        }, function () {
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
        });
    }

    $scope.deleteFile = function () {
        if ($scope.Selected.SelectedImages != undefined && $scope.Selected.SelectedImages.length > 0) {
            UserMessagesProvider.confirmHandler("Are you sure you want to delete the selected files?", function () {
                var indexes = $scope.Selected.SelectedImages.slice();
                UserMessagesProvider.displayLoading();
                var deletedFiles = 0;
                for (i = 0; i < indexes.length; i++) {
                    GalleryDataProvider.deleteFile($scope.currentObj.Files[indexes[i]].Path).then(function (returnValue) {
                        deletedFiles++;
                        if (deletedFiles == indexes.length) {
                            if (deletedFiles == 1) {
                                $scope.currentObj.Files.splice(indexes[0], 1);
                                UserMessagesProvider.hideLoading();
                            } else {
                                GalleryDataProvider.getFiles($scope.currentObj.Path).then(function (data) {
                                    $scope.currentObj.Files = data;
                                    UserMessagesProvider.hideLoading();
                                }, function (errorData, status, headers, config) {
                                    UserMessagesProvider.hideLoading();
                                    UserMessagesProvider.errorHandler(status);
                                });
                            }
                            $scope.Selected.SelectedImages = [];
                        }
                        if (returnValue.data.result == 'true') {

                        }
                        else {
                            UserMessagesProvider.errorHandler(999, returnValue.message);
                        }
                    }, function (errorData, status, headers, config) {
                        deletedFiles++;
                        UserMessagesProvider.hideLoading();
                        UserMessagesProvider.errorHandler(status);
                    });
                }
            }, null);
        }
    }

    $scope.ok = function (path, index) {
        if ($scope.MultipleImages == true) {
            $scope.Selected.SelectedImages.push(index);
        } else {
            $modalInstance.close(path);
        }
    };

    $scope.chooseSelected = function () {
        if ($scope.Selected.SelectedImages.length > 0) {
            var result = '';
            for (i = 0; i < $scope.Selected.SelectedImages.length; i++) {
                if (result == '') {
                    result = $scope.currentObj.Files[$scope.Selected.SelectedImages[i]].CDNPath;
                } else {
                    result = result + ',' + $scope.currentObj.Files[$scope.Selected.SelectedImages[i]].CDNPath;
                }
            }
            $modalInstance.close(result);
        }
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
});
var createFolderController = function ($scope, $modalInstance, parentPath, UMP) {
    $scope.newNode = { Name: '', Path: parentPath };
    $scope.createForm = {};
    $scope.ok = function () {
        if ($scope.createForm.form.$valid) {
            $scope.newNode.Path += '/' + $scope.newNode.Name;
            $modalInstance.close($scope.newNode);
        } else {
            UMP.invalidHandler();
        }
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};
var uploadFileController = function ($scope, $modalInstance, path, UMP, uploadProvider, scopeService) {
    var uploadedFiles = 0;
    $scope.newFile = { Name: '', Path: path, FilePath: '', Files: [] };
    $scope.progress = 0;
    $scope.createForm = {};
    $scope.ok = function () {
        if ($scope.createForm.form.$valid) {
            $modalInstance.close($scope.newFile);
        } else {
            UMP.invalidHandler();
        }
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.$watch('newFile.Files', function (oldValue, newValue) {
        if (oldValue == newValue) {
            return;
        }
        for (var i = 0; i < $scope.newFile.Files.length; i++) {
            var file = $scope.newFile.Files[i];
            $scope.upload = uploadProvider.upload({
                url: '/DConfig/IOServicesAPI/createFile', // upload.php script, node.js route, or servlet url
                method: 'POST',
                data: { Path: $scope.newFile.Path },
                file: file, // single file or a list of files. list is only for html5
                //fileName: 'doc.jpg' or ['1.jpg', '2.jpg', ...] // to modify the name of the file(s)
                //fileFormDataName: myFile, // file formData name ('Content-Disposition'), server side request form name
                // could be a list of names for multiple files (html5). Default is 'file'
                //formDataAppender: function(formData, key, val){}  // customize how data is added to the formData. 
                // See #40#issuecomment-28612000 for sample code

            }).progress(function (evt) {
                scopeService.safeApply($scope, function () { $scope.progress = parseInt(100.0 * evt.loaded / evt.total) });
                //console.log('progress: ' + parseInt(100.0 * evt.loaded / evt.total) + '% file :' + evt.config.file.name);
            }).then(function (data, status, headers, config) {
                // file is uploaded successfully
                //console.log('file ' + config.file.name + 'is uploaded successfully. Response: ' + data);
                uploadedFiles += 1;
                if (data.data.result == 'true') {
                    scopeService.safeApply($scope, function () {
                        if (uploadedFiles == $scope.newFile.Files.length) {
                            $modalInstance.close();
                        }
                    });
                } else {
                    UMP.errorHandler(999, data.data.message);
                }
            });
            //.error(...)
            //.then(success, error, progress); // returns a promise that does NOT have progress/abort/xhr functions
            //.xhr(function(xhr){xhr.upload.addEventListener(...)}) // access or attach event listeners to 
            //the underlying XMLHttpRequest
        }

        /* alternative way of uploading, send the file binary with the file's content-type.
           Could be used to upload files to CouchDB, imgur, etc... html5 FileReader is needed. 
           It could also be used to monitor the progress of a normal http post/put request. 
           Note that the whole file will be loaded in browser first so large files could crash the browser.
           You should verify the file size before uploading with $upload.http().
        */
        // $scope.upload = $upload.http({...})  // See 88#issuecomment-31366487 for sample code.

    });
};
