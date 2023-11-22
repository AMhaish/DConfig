angular.module('DConfigSharedLib')
    .factory('IntentsProvider', ['$http', '$modal', 'UserMessagesProvider', 'AdditionalViewsProvider', function ($http, $modal, UserMessagesProvider, AdditionalViewsProvider) {
        var intentsCollections = {};
        var intentsProvider = {};

        function processIntentWindow(extention, parametersObj, okHandler, cancelHandler) {
            var modalInstance = $modal.open({
                template: extention.view,
                controller: 'Intents.' + extention.Name,
                size: 'lg',
                resolve: {
                    parameters: function () { return parametersObj; }
                }
            });
            modalInstance.result.then(function (obj) {
                if (okHandler)
                    okHandler(obj);
            }, function () {
                if (cancelHandler)
                    cancelHandler(obj);
            });
        }

        function loadIntentViews(extention, successHandler, failedHandler) {
            $http.get("/DConfig/AppsAPI/appextentionview/" + extention.Id).then(function (view) {
                extention.viewLoaded = true;
                extention.view = view.data;
                //var c=$("<div/>", {
                //    name: extention.Name + '_views'
                //}).appendTo("body").html(view);
                if (successHandler)
                    successHandler();
            }, function (data, status, headers, config) {
                UserMessagesProvider.errorHandler(807);
                if (failedHandler)
                    failedHandler();
            });
        }

        function loadIntentScripts(extention, successHandler, failedHandler) {
            $http.get("/DConfig/AppsAPI/AppExtentionScripts/" + extention.Id).then(function (pathObj) {
                $.getScript(pathObj.data.data, function () {
                    extention.scriptsLoaded = true;
                    if (successHandler)
                        successHandler();
                }).fail(function () {
                    if (arguments[0].readyState == 0) {
                        //script failed to load
                        UserMessagesProvider.errorHandler(804);
                    } else {
                        //script loaded but failed to parse
                        UserMessagesProvider.errorHandler(805);
                    }
                });
            }, function (data, status, headers, config) {
                UserMessagesProvider.errorHandler(803);
                if (failedHandler)
                    failedHandler();
            });
        }

        intentsProvider.startIntent = function (intentName, parametersObj, okHandler, cancelHandler) {
            var extention;
            if (!intentsCollections.hasOwnProperty(intentName) && intentsCollections[intentName] == undefined) {
                UserMessagesProvider.displayLoading();
                $http.get("/DConfig/AppsAPI/intentExtentions/" + intentName).then(function (data) {
                    if (data.data.length == 1) {
                        intentsCollections[intentName] = data.data[0];
                        extention = intentsCollections[intentName];
                        if (extention.scriptsLoaded == undefined || extention.viewLoaded == undefined) {
                            loadIntentScripts(extention, function () {
                                loadIntentViews(extention, function () {
                                    UserMessagesProvider.hideLoading();
                                    processIntentWindow(extention, parametersObj, okHandler, cancelHandler);
                                }, null);
                            }, null);
                        } else {
                            UserMessagesProvider.hideLoading();
                            processIntentWindow(extention, parametersObj, okHandler, cancelHandler);
                        }
                    } else if (data.data.length > 1) {
                        intentsCollections[intentName] = data.data;
                        var modalInstance = $modal.open({
                            templateUrl: 'IntentExecutorSelector.html',
                            controller: IntentExecutorSelectorController,
                            size: 'sm',
                            resolve: {
                                listOfItems: function () {
                                    return intentsCollections[intentName];
                                }
                            }
                        });
                        modalInstance.result.then(function (id) {
                            for (i = 0; i < intentsCollections[intentName].length; i++) {
                                if (intentsCollections[intentName][i].id == id) {
                                    extention = intentsCollections[intentName][i];
                                }
                            }
                            if (extention != undefined && (extention.scriptsLoaded == undefined || extention.viewLoaded == undefined)) {
                                loadIntentScripts(extention, function () {
                                    loadIntentViews(extention, function () {
                                        UserMessagesProvider.hideLoading();
                                        processIntentWindow(extention, parametersObj, okHandler, cancelHandler);
                                    }, null);
                                }, null);
                            } else {
                                UserMessagesProvider.hideLoading();
                                processIntentWindow(extention, parametersObj, okHandler, cancelHandler);
                            }
                        }, function () {
                        });
                    } else {
                        UserMessagesProvider.hideLoading();
                        UserMessagesProvider.notificationHandler('Sorry, system could not found a way to execute this order');
                    }
                }, function (data, status, headers, config) {
                    UserMessagesProvider.hideLoading();
                    UserMessagesProvider.errorHandler(999, data.message);
                });
            } else {
                var dist = intentsCollections[intentName];
                if (dist instanceof Array) {
                    var modalInstance = $modal.open({
                        templateUrl: 'IntentExecutorSelector.html',
                        controller: IntentExecutorSelectorController,
                        size: 'sm',
                        resolve: {
                            listOfItems: function () {
                                return intentsCollections[intentName];
                            }
                        }
                    });
                    modalInstance.result.then(function (id) {
                        for (i = 0; i < intentsCollections[intentName].length; i++) {
                            if (intentsCollections[intentName][i].id == id) {
                                extention = intentsCollections[intentName][i];
                            }
                        }
                        if (extention != undefined && (extention.scriptsLoaded == undefined || extention.viewLoaded == undefined)) {
                            loadIntentScripts(extention, function () {
                                loadIntentViews(extention, function () {
                                    UserMessagesProvider.hideLoading();
                                    processIntentWindow(extention, parametersObj, okHandler, cancelHandler);
                                }, null);
                            }, null);
                        } else {
                            UserMessagesProvider.hideLoading();
                            processIntentWindow(extention, parametersObj, okHandler, cancelHandler);
                        }
                    }, function () {
                    });
                } else {
                    extention = intentsCollections[intentName];
                    if (extention != undefined && (extention.scriptsLoaded == undefined || extention.viewLoaded == undefined)) {
                        loadIntentScripts(extention, function () {
                            loadIntentViews(extention, function () {
                                UserMessagesProvider.hideLoading();
                                processIntentWindow(extention, parametersObj, okHandler, cancelHandler);
                            }, null);
                        }, null);
                    } else {
                        UserMessagesProvider.hideLoading();
                        processIntentWindow(extention, parametersObj, okHandler, cancelHandler);
                    }
                }
            }
        }

        intentsProvider.clearCache = function () {
            intentsCollections = {};
        }

        return intentsProvider;
    }]);


var IntentExecutorSelectorController = function ($scope, $modalInstance, listOfItems) {
    $scope.items = listOfItems;
    $scope.container = {};
    $scope.ok = function () {
        $modalInstance.close($scope.container.extentionId);
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};