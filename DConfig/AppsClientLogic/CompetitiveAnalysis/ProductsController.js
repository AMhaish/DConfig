angular.module('DConfig').controllerProvider.register('CompetitiveAnalysis.ProductsController', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, CompetitiveAnalysisDataProvider, EventsProvider, $modal, scopeService, IntentsProvider, $routeParams, $cookies, $filter) {
    $scope.DetailsPanelStates = {
        None: 0,
        ProductDetails: 1,
        ProductProperties: 3,
        ProductPrices: 4,
        Share: 5
    };
    $scope.ExecutingContexts = {
        None: 0,
        Deleting: 1,
        Creating: 2,
        Renaming: 3,
        Updating: 4
    };
    $scope.Currencies = ['USD', 'CAD', 'EUR', 'AED', 'TRY', 'SAR', 'QAR', 'BHD', 'KWD', 'HKD', 'JPY', 'CNY', 'SGD'];
    $scope.BrandFactoryTypes = [
        { Name: 'Own', Value: 'Own' },
        { Name: 'Supplier', Value: 'Supplier' },
        { Name: 'Competitor', Value: 'Competitor' },
    ];
    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
    $scope.state = $scope.DetailsPanelStates.None;
    var contentSearchBox = $('#contentSearchBox');

    function initialize() {
        BreadCrumpsProvider.breadCrumps.path = ['Competitive Analysis', 'Products'];
        UserMessagesProvider.displayProgress(4);
        CompetitiveAnalysisDataProvider.getTemplates().then(function (data) {
            $scope.templates = data.data;
            UserMessagesProvider.increaseProgress();
        }, function (data, status, headers, config) {
            UserMessagesProvider.increaseProgress();
            UserMessagesProvider.errorHandler(status);
        });
        CompetitiveAnalysisDataProvider.getPropertiesGroups().then(function (data) {
            $scope.PropertiesGroups = data.data;
            UserMessagesProvider.increaseProgress();
        }, function (data, status, headers, config) {
            UserMessagesProvider.increaseProgress();
            UserMessagesProvider.errorHandler(status);
        });
        CompetitiveAnalysisDataProvider.getCompanies().then(function (data) {
            $scope.Companies = data.data;
            UserMessagesProvider.increaseProgress();
        }, function (data, status, headers, config) {
            UserMessagesProvider.errorHandler(status);
            UserMessagesProvider.increaseProgress();
        });
        if ($routeParams.ProductId != undefined) {
            CompetitiveAnalysisDataProvider.getProduct($routeParams.ProductId).then(function (data) {
                $scope.Products = [data.data];
                for (i = 0; i < $scope.templates.length; i++) {
                    if (data.data.TemplateId == $scope.templates[i].Id) {
                        $scope.currentTemplate = $scope.templates[i];
                        break;
                    }
                }
                $scope.Products[0].template = $scope.currentTemplate;
                UserMessagesProvider.hideLoading();
                $scope.displayDetails(0);
                UserMessagesProvider.increaseProgress();
            }, function (data, status, headers, config) {
                UserMessagesProvider.errorHandler(status);
                UserMessagesProvider.increaseProgress();
            });
        } else {
            UserMessagesProvider.increaseProgress();
        }
    }

    $scope.loadProducts = function (quickPattern) {
        if (quickPattern != undefined) {
            $scope.productPattern = quickPattern;
        }
        var pattern = $scope.productPattern;
        var template = $scope.currentTemplate;
        if (template) {
            if (pattern.length >= 2 || pattern == "*") {
                UserMessagesProvider.addLoadingToWindow('productsList');
                CompetitiveAnalysisDataProvider.getTemplateProductsByPattern(template.Id, pattern).then(function (data) {
                    $scope.Products = data.data;
                    UserMessagesProvider.removeLoadingFromWindow('productsList');
                }, function (data, status, headers, config) {
                    UserMessagesProvider.removeLoadingFromWindow('productsTree_productSelector');
                    UserMessagesProvider.errorHandler(status);
                });
            } else {
                $scope.Products = [];
            }
        } else {
            UserMessagesProvider.errorHandler(999, 'Please select the product template');
            $scope.productPattern = '';
        }
    }

    $scope.closeDetailsForm = function () {
        $scope.currentObj = null;
        $scope.state = $scope.DetailsPanelStates.None;
    }

    $scope.displayDetails = function (index) {
        UserMessagesProvider.displayLoading();
        $scope.selectedIndex = index;
        $scope.currentObj = $scope.Products[index];
        if ($cookies['LastDetailsPanelState'] == undefined) {
            $scope.state = 3;
        } else {
            $scope.state = parseInt($cookies['LastDetailsPanelState']);
        }
        for (i = 0; i < $scope.templates.length; i++) {
            if ($scope.currentObj.TemplateId == $scope.templates[i].Id) {
                $scope.currentObj.template = $scope.templates[i];
                break;
            }
        }
        $scope.parentObject = $scope.currentObj.template;
        if ($scope.currentObj.PropertiesValues != null && $scope.currentObj.PropertiesValues.length > 0) {
            for (i = 0; i < $scope.parentObject.Properties.length; i++) {
                $scope.parentObject.Properties[i].Value = '';
                for (j = 0; j < $scope.currentObj.PropertiesValues.length; j++) {
                    if ($scope.parentObject.Properties[i].Id == $scope.currentObj.PropertiesValues[j].PropertyId) {
                        $scope.parentObject.Properties[i].Value = $scope.currentObj.PropertiesValues[j].Value;
                    }
                }
            }
        } else {
            for (i = 0; i < $scope.parentObject.Properties.length; i++) {
                $scope.parentObject.Properties[i].Value = '';
            }
        }
        $scope.parentObject.PropertiesGroups = [];
        $scope.parentObject.HighlightsGroups = {};
        if ($scope.PropertiesGroups != undefined) {
            for (i = 0; i < $scope.PropertiesGroups.length; i++) {
                var groupExists = false;
                for (j = 0; j < $scope.parentObject.Properties.length; j++) {
                    if ($scope.parentObject.Properties[j].GroupId == $scope.PropertiesGroups[i].Id) {
                        if (!groupExists) {
                            groupExists = true;
                            $scope.parentObject.PropertiesGroups.push({ Id: $scope.PropertiesGroups[i].Id, Name: $scope.PropertiesGroups[i].Name, DisplayAs: $scope.PropertiesGroups[i].DisplayAs, Properties: [] });
                        }
                    }
                }
            }
            for (i = 0; i < $scope.parentObject.PropertiesGroups.length; i++) {
                for (j = 0; j < $scope.parentObject.Properties.length; j++) {
                    if ($scope.parentObject.Properties[j].GroupId == $scope.parentObject.PropertiesGroups[i].Id) {
                        $scope.parentObject.PropertiesGroups[i].Properties.push(jQuery.extend(true, {}, $scope.parentObject.Properties[j]));
                    }
                }
            }
            for (j = 0; j < $scope.parentObject.PropertiesRelations.length; j++) {
                if ($scope.parentObject.PropertiesRelations[j].IsHighlight || ($scope.parentObject.PropertiesRelations[j].InvisibileToFactoryTypes != null && $scope.parentObject.PropertiesRelations[j].InvisibileToFactoryTypes != '')) {
                    for (k = 0; k < $scope.parentObject.Properties.length; k++) {
                        if ($scope.parentObject.PropertiesRelations[j].PropertyId == $scope.parentObject.Properties[k].Id) {
                            if ($scope.parentObject.PropertiesRelations[j].IsHighlight) {
                                if ($scope.parentObject.HighlightsGroups[$scope.parentObject.Properties[k].GroupId] == undefined) {
                                    $scope.parentObject.HighlightsGroups[$scope.parentObject.Properties[k].GroupId] = [];
                                }
                                $scope.parentObject.HighlightsGroups[$scope.parentObject.Properties[k].GroupId].push($scope.parentObject.Properties[k]);
                                //$scope.parentObject.Highlights.push($scope.parentObject.Properties[k]);
                            }
                            if ($scope.parentObject.PropertiesRelations[j].InvisibileToFactoryTypes != null && $scope.parentObject.PropertiesRelations[j].InvisibileToFactoryTypes != '')
                                $scope.parentObject.Properties[k].InvisibileToFactoryTypes = $scope.parentObject.PropertiesRelations[j].InvisibileToFactoryTypes;
                            break;
                        }
                    }
                }
            }
        }
        console.log($scope.parentObject.Properties);
        $scope.currentObj.SSRPPrices = [];
        $scope.currentObj.QTYPrices = [];
        $scope.currentObj.NORMPrices = [];
        $scope.currentObj.QTYLastPrices = [];
        if ($scope.currentObj.Prices != undefined) {
            for (i = 0; i < $scope.currentObj.Prices.length; i++) {
                switch ($scope.currentObj.Prices[i].PriceType) {
                    case 'SSRP':
                        $scope.currentObj.SSRPPrices.push($scope.currentObj.Prices[i]);
                        break;
                    case 'QTY':
                        $scope.currentObj.QTYPrices.push($scope.currentObj.Prices[i]);
                        if ($scope.currentObj.QTYLastPrices.length == 0) {
                            $scope.currentObj.QTYLastPrices.push({
                                from: $scope.currentObj.Prices[i].QuantityFrom,
                                to: $scope.currentObj.Prices[i].QuantityTo,
                                price: $scope.currentObj.Prices[i]
                            });
                        } else {
                            var exists = false;
                            for (j = 0; j < $scope.currentObj.QTYLastPrices.length; j++) {
                                if ($scope.currentObj.QTYLastPrices[j].from <= $scope.currentObj.Prices[i].QuantityFrom && $scope.currentObj.QTYLastPrices[j].to >= $scope.currentObj.Prices[i].QuantityTo) {
                                    exists = true;
                                    break;
                                }
                            }
                            if (!exists) {
                                $scope.currentObj.QTYLastPrices.push({
                                    from: $scope.currentObj.Prices[i].QuantityFrom,
                                    to: $scope.currentObj.Prices[i].QuantityTo,
                                    price: $scope.currentObj.Prices[i]
                                });
                            }
                        }
                        break;
                    default:
                        $scope.currentObj.NORMPrices.push($scope.currentObj.Prices[i]);
                        break;
                }
            }
        }
        var recentProducts;
        if ($cookies['recentProducts'] != undefined) {
            recentProducts = JSON.parse($cookies['recentProducts']);
        } else {
            recentProducts = [];
        }
        recentProducts.push($scope.currentObj.Id);
        if (recentProducts.length > 10) {
            recentProducts.shift();
        }
        $cookies['recentProducts'] = JSON.stringify(recentProducts);
        UserMessagesProvider.hideLoading();
    }

    $scope.displayGeneralDetails = function () {
        $scope.state = $scope.DetailsPanelStates.ProductDetails;
        $cookies['LastDetailsPanelState'] = $scope.DetailsPanelStates.ProductDetails.toString();
    }

    $scope.displayProporties = function () {
        $scope.state = $scope.DetailsPanelStates.ProductProperties;
        $cookies['LastDetailsPanelState'] = $scope.DetailsPanelStates.ProductProperties.toString();
    }

    $scope.displayPrices = function () {
        $scope.state = $scope.DetailsPanelStates.ProductPrices;
        $cookies['LastDetailsPanelState'] = $scope.DetailsPanelStates.ProductPrices.toString();
    }

    $scope.cloneProduct = function (index) {
        UserMessagesProvider.confirmHandler("Are you sure you want to make a copy of this product?", function () {
            if (index != $scope.selectedIndex) {
                $scope.displayDetails(index);
            }
            UserMessagesProvider.displayLoading();
            CompetitiveAnalysisDataProvider.createProduct($scope.currentObj).then(function (returnValue) {
                if (returnValue.data.result == 'true') {
                    if ($scope.Products == undefined) {
                        $scope.Products = [];
                    }
                    for (j = 0; j < $scope.templates.length; j++) {
                        if ($scope.templates[j].Id == $scope.currentObj.TemplateId) {
                            $scope.currentObj.template = $scope.templates[j];
                            break;
                        }
                    }
                    $scope.currentObj.Id = returnValue.data.obj.Id;
                    $scope.Products.push($scope.currentObj);
                    $scope.selectedIndex = $scope.Products.length - 1;
                    $scope.saveProperties(true, function () {
                        $scope.savePrices(true, function () {
                            $scope.displayDetails($scope.Products.length - 1);
                            UserMessagesProvider.hideLoading();
                            UserMessagesProvider.successHandler();
                        });
                    });
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

    $scope.displayRecentProducts = function () {
        if ($cookies['recentProducts'] != undefined) {
            var recentProducts = JSON.parse($cookies['recentProducts']);
            if (recentProducts != undefined && recentProducts.length > 0) {
                UserMessagesProvider.addLoadingToWindow('productsList');
                CompetitiveAnalysisDataProvider.getProductsByIds(recentProducts).then(function (data) {
                    $scope.Products = data.data;
                    UserMessagesProvider.removeLoadingFromWindow('productsList');
                }, function (data, status, headers, config) {
                    UserMessagesProvider.removeLoadingFromWindow('productsTree_productSelector');
                    UserMessagesProvider.errorHandler(status);
                });
            } else {
                UserMessagesProvider.errorHandler(999, 'There is no any products saved in this browser');
            }
        }
    }

    $scope.deleteProduct = function (index) {
        UserMessagesProvider.confirmHandler("Are you sure you want to delete this product?", function () {
            $scope.selectedIndex = index;
            $scope.currentObj = $scope.Products[index];
            UserMessagesProvider.displayLoading();
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.Deleting;
            CompetitiveAnalysisDataProvider.deleteProduct($scope.currentObj).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    $scope.Products.splice(index, 1);
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

    $scope.createNode = function (e, data) {
        $scope.currentObj = {};
        var modalInstance = $modal.open({
            templateUrl: 'createForm.html',
            controller: createProductController,
            size: 'lg',
            resolve: {
                templates: function () {
                    return $scope.templates;
                },
                UMP: function () { return UserMessagesProvider; },
                BrandFactoryTypes: function () { return $scope.BrandFactoryTypes; },
                Companies: function () { return $scope.Companies; }
            }
        });
        modalInstance.result.then(function (newNode) {
            UserMessagesProvider.displayLoading();
            CompetitiveAnalysisDataProvider.createProduct(newNode).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    if ($scope.Products == undefined) {
                        $scope.Products = [];
                    }
                    for (j = 0; j < $scope.templates.length; j++) {
                        if ($scope.templates[j].Id == newNode.TemplateId) {
                            newNode.template = $scope.templates[j];
                            break;
                        }
                    }
                    newNode.Id = returnValue.data.obj.Id;
                    $scope.Products.push(newNode);
                    $scope.displayDetails($scope.Products.length - 1);
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
            $scope.closeDetailsForm();
        });
    }

    $scope.importProducts = function () {
        var modalInstance = $modal.open({
            templateUrl: 'importForm.html',
            controller: importProductsController,
            size: 'lg',
            resolve: {
                templates: function () {
                    return $scope.templates;
                },
                UMP: function () { return UserMessagesProvider; },
                IntentsProvider: function () { return IntentsProvider; },
                BrandFactoryTypes: function () { return $scope.BrandFactoryTypes; },
                Companies: function () { return $scope.Companies; }
            }
        });
        modalInstance.result.then(function (newNode) {
            UserMessagesProvider.displayLoading();
            CompetitiveAnalysisDataProvider.ImportProductsFromExcel(newNode.TemplateId, newNode.ExcelPath, newNode.CompanyId, newNode.BrandFactoryType).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.successHandler();
                $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                var modalInstance = $modal.open({
                    templateUrl: 'importReportForm.html',
                    controller: importProductsReportController,
                    size: 'lg',
                    resolve: {
                        UMP: function () { return UserMessagesProvider; },
                        Report: function () { return returnValue.data; }
                    }
                });
                modalInstance.result.then(function () {
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

    $scope.importProductsImages = function () {
        var modalInstance = $modal.open({
            templateUrl: 'importImagesForm.html',
            controller: importProductsImagesController,
            size: 'lg',
            resolve: {
                UMP: function () { return UserMessagesProvider; },
                IntentsProvider: function () { return IntentsProvider; }
            }
        });
        modalInstance.result.then(function (newNode) {
            UserMessagesProvider.displayLoading();
            CompetitiveAnalysisDataProvider.ImportProductsImagesFromZip(newNode.ZipPath).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.successHandler();
                $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                var modalInstance = $modal.open({
                    templateUrl: 'importReportForm.html',
                    controller: importProductsReportController,
                    size: 'lg',
                    resolve: {
                        UMP: function () { return UserMessagesProvider; },
                        Report: function () { return returnValue.data; }
                    }
                });
                modalInstance.result.then(function () {
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

    $scope.saveChanges = function () {
        if (($scope.state == $scope.DetailsPanelStates.ProductDetails && $scope.detialsForm.$valid) || ($scope.state == $scope.DetailsPanelStates.ContentInstanceDetails && $scope.instanceDetailsForm.$valid)) {
            if ($scope.currentObj != null && ($scope.CurrentExecutingContext == $scope.ExecutingContexts.None || $scope.CurrentExecutingContext == $scope.ExecutingContexts.Renaming)) {
                UserMessagesProvider.displayLoading();
                CompetitiveAnalysisDataProvider.updateProduct($scope.currentObj).then(function (returnValue) {
                    UserMessagesProvider.hideLoading();
                    if (returnValue.data.result == 'true') {
                        $scope.Products[$scope.selectedIndex] = returnValue.data.obj;
                        $scope.displayDetails($scope.selectedIndex);
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
        } else {
            UserMessagesProvider.invalidHandler();
        }
    }

    $scope.saveTags = function () {
        if ($scope.currentObj.TagsString != undefined) {
            var tags = $scope.currentObj.TagsString.split(',');
            UserMessagesProvider.displayLoading();
            CompetitiveAnalysisDataProvider.updateProductTags($scope.currentObj.Id, tags).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    $scope.Products[$scope.selectedIndex].TagsString = $scope.currentObj.TagsString;
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

    }

    function setEmbedPath() {
        $scope.Embed.Path = '<iframe id="prodSpecsIframe" src="http://' + window.location.host + '/_CAPD/' + $scope.currentObj.Id + '?PropertiesIds=' + $scope.Embed.Properties + '&Style=' + $scope.Embed.Style + '" frameborder="' + $scope.Embed.Border + '" width="' + $scope.Embed.Width + '" height="' + $scope.Embed.Height + '"/>';
        $scope.Embed.PurePath = 'http://' + window.location.host + '/_CAPD/' + $scope.currentObj.Id + '?PropertiesIds=' + $scope.Embed.Properties + '&Style=' + $scope.Embed.Style;
        $cookies['LastEmbedCode' + $scope.currentObj.Id.toString()] = JSON.stringify($scope.Embed);
    }
    $scope.displaySharePanel = function () {
        $scope.state = $scope.DetailsPanelStates.Share;
        $cookies['LastDetailsPanelState'] = $scope.DetailsPanelStates.Share;
        if ($cookies['LastEmbedCode' + $scope.currentObj.Id.toString()] != undefined) {
            $scope.Embed = JSON.parse($cookies['LastEmbedCode' + $scope.currentObj.Id.toString()]);
        } else {
            $scope.Embed = { Path: '', PurePath: '', Properties: '', Style: '', Width: '100%', Height: '', Border: '0' };
        }
        $scope.$watch('Embed.Properties', function (newValue, oldValue) {
            if (newValue === oldValue) {
                return;
            }
            setEmbedPath();
        });
        $scope.$watch('Embed.Style', function (newValue, oldValue) {
            if (newValue === oldValue) {
                return;
            }
            setEmbedPath();
        });
        $scope.$watch('Embed.Width', function (newValue, oldValue) {
            if (newValue === oldValue) {
                return;
            }
            setEmbedPath();
        });
        $scope.$watch('Embed.Height', function (newValue, oldValue) {
            if (newValue === oldValue) {
                return;
            }
            setEmbedPath();
        });
        $scope.$watch('Embed.Border', function (newValue, oldValue) {
            if (newValue === oldValue) {
                return;
            }
            setEmbedPath();
        });
        setEmbedPath();
    }

    $scope.selectEmbedCode = function () {
        document.getElementById("embedPath").select();
    }

    $scope.selectEmbedPureCode = function () {
        document.getElementById("embedPurePath").select();
    }

    $scope.saveProperties = function (cloning, callback) {
        UserMessagesProvider.displayLoading();
        var fieldValueExists = false;
        if ($scope.currentObj.PropertiesValues == undefined) {
            $scope.currentObj.PropertiesValues = [];
        }
        for (k = 0; k < $scope.parentObject.PropertiesGroups.length; k++) {
            for (i = 0; i < $scope.parentObject.PropertiesGroups[k].Properties.length; i++) {
                for (j = 0; j < $scope.currentObj.PropertiesValues.length; j++) {
                    if ($scope.parentObject.PropertiesGroups[k].Properties[i].Id == $scope.currentObj.PropertiesValues[j].PropertyId) {
                        $scope.currentObj.PropertiesValues[j].Value = $scope.parentObject.PropertiesGroups[k].Properties[i].Value;
                        fieldValueExists = true;
                        if ($scope.parentObject.PropertiesGroups[k].Properties[i].Type == 'Predefined List - Filter/Select') {
                            var exists = false;
                            if ($scope.parentObject.PropertiesGroups[k].Properties[i].Enum != undefined && $scope.parentObject.PropertiesGroups[k].Properties[i].Enum.OrderedValues != undefined && $scope.parentObject.PropertiesGroups[k].Properties[i].Enum.OrderedValues.length > 0) {
                                for (l = 0; l < $scope.parentObject.PropertiesGroups[k].Properties[i].Enum.OrderedValues.length; l++) {
                                    if ($scope.parentObject.PropertiesGroups[k].Properties[i].Value == $scope.parentObject.PropertiesGroups[k].Properties[i].Enum.OrderedValues[l].Value) {
                                        exists = true;
                                        break;
                                    }
                                }
                            }
                            if (!exists) {
                                $scope.parentObject.PropertiesGroups[k].Properties[i].Enum.OrderedValues.push({ EnumId: $scope.parentObject.PropertiesGroups[k].Properties[i].Enum.Id, Value: $scope.parentObject.PropertiesGroups[k].Properties[i].Value });
                                CompetitiveAnalysisDataProvider.updatePropertyEnumValues($scope.parentObject.PropertiesGroups[k].Properties[i].Enum.Id, { Values: $scope.parentObject.PropertiesGroups[k].Properties[i].Enum.OrderedValues }).then(function (returnValue) {
                                }, function (errorData, status, headers, config) {
                                    UserMessagesProvider.hideLoading();
                                    UserMessagesProvider.errorHandler(status);
                                });
                            }
                        }
                    }
                }
                if (fieldValueExists) {
                    fieldValueExists = false;
                }
                else {
                    $scope.currentObj.PropertiesValues.push({ PropertyId: $scope.parentObject.PropertiesGroups[k].Properties[i].Id, ProductId: $scope.currentObj.Id, Value: $scope.parentObject.PropertiesGroups[k].Properties[i].Value });
                }
            }
        }
        //var distNode = productsTreeContainer.jstree(true).get_node($scope.currentNodeObject);
        CompetitiveAnalysisDataProvider.updateProductPropertiesValues($scope.currentObj.Id, $scope.currentObj.PropertiesValues).then(function (returnValue) {
            if (cloning != undefined && cloning) {
                $scope.Products[$scope.selectedIndex] = returnValue.data.obj;
                $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                if (callback != undefined)
                    callback();
            } else {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    $scope.Products[$scope.selectedIndex] = returnValue.data.obj;
                    $scope.displayDetails($scope.selectedIndex);
                    UserMessagesProvider.successHandler();
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.data.message);
                }
                $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
            }
        }, function (errorData, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
        });
    }

    $scope.savePrices = function (cloning, callback) {
        if (cloning != undefined && cloning) {
            for (i = 0; i < $scope.currentObj.Prices.length; i++) {
                $scope.currentObj.Prices[i].Id = undefined;
            }
        }
        UserMessagesProvider.displayLoading();
        CompetitiveAnalysisDataProvider.updateProductPrices($scope.currentObj).then(function (returnValue) {
            if (cloning != undefined && cloning) {
                $scope.Products[$scope.selectedIndex] = returnValue.data.obj;
                $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
                if (callback != undefined)
                    callback();
            } else {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    $scope.Products[$scope.selectedIndex] = returnValue.data.obj;
                    $scope.displayDetails($scope.selectedIndex);
                    UserMessagesProvider.successHandler();
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.data.message);
                }
                $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
            }
        }, function (errorData, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
        });
    }

    $scope.addNewPrice = function (priceType) {
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1; //January is 0!

        var yyyy = today.getFullYear();
        if (dd < 10) {
            dd = '0' + dd
        }
        if (mm < 10) {
            mm = '0' + mm
        }
        var today = mm + '/' + dd + '/' + yyyy;
        if ($scope.currentObj.Prices == undefined) {
            $scope.currentObj.Prices = [];
        }
        var price = { PriceValue: '', PriceDate: today, Reference: '', Currency: '', open: true, PriceType: priceType, Quantity: '' };
        $scope.currentObj.Prices.push(price);
        switch (priceType) {
            case 'SSRP':
                if ($scope.currentObj.SSRPPrices == undefined) {
                    $scope.currentObj.SSRPPrices = [];
                }
                if ($scope.currentObj.SSRPPrices.length <= 0) {
                    $scope.currentObj.SSRPPrices.push(price);
                }
                break;
            case 'QTY':
                if ($scope.currentObj.QTYPrices == undefined) {
                    $scope.currentObj.QTYPrices = [];
                }
                $scope.currentObj.QTYPrices.push(price);
                break;
            default:
                if ($scope.currentObj.NORMPrices == undefined) {
                    $scope.currentObj.NORMPrices = [];
                }
                $scope.currentObj.NORMPrices.push(price);
                break;
        }
    }

    $scope.removePrice = function (index, priceType) {
        var deletingPrice;
        switch (priceType) {
            case 'SSRP':
                if ($scope.currentObj.SSRPPrices[index].Id == undefined) {
                    $scope.currentObj.SSRPPrices.splice(index, 1);
                    return;
                }
                deletingPrice = $scope.currentObj.SSRPPrices[index];
                break;
            case 'QTY':
                if ($scope.currentObj.QTYPrices[index].Id == undefined) {
                    $scope.currentObj.QTYPrices.splice(index, 1);
                    return;
                }
                deletingPrice = $scope.currentObj.QTYPrices[index];
                break;
            default:
                if ($scope.currentObj.NORMPrices[index].Id == undefined) {
                    $scope.currentObj.NORMPrices.splice(index, 1);
                    return;
                }
                deletingPrice = $scope.currentObj.NORMPrices[index];
                break;
        }
        UserMessagesProvider.confirmHandler("Are you sure you want to delete this price?", function () {
            UserMessagesProvider.displayLoading();
            CompetitiveAnalysisDataProvider.deleteProductPrice(deletingPrice.Id).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    switch (priceType) {
                        case 'SSRP':
                            $scope.currentObj.SSRPPrices.splice(index, 1);
                            break;
                        case 'QTY':
                            $scope.currentObj.QTYPrices.splice(index, 1);

                            break;
                        default:

                            $scope.currentObj.NORMPrices.splice(index, 1);
                            break;
                    }
                    for (i = 0; i < $scope.currentObj.Prices.length; i++) {
                        if ($scope.currentObj.Prices[i].Id == deletingPrice.Id) {
                            $scope.currentObj.Prices.splice(i, 1);
                            $scope.Products[$scope.selectedIndex].Prices.splice(i, 1);
                            break;
                        }
                    }
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.data.message);
                }
                $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
            }, function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        }, null);
    }

    $scope.runIntentSelector = function (index, parentIndex) {
        var intentName = $scope.parentObject.PropertiesGroups[parentIndex].Properties[index].TypeObj.IntentName;
        IntentsProvider.startIntent(intentName, { RootPath: '/Produtcs/' + $scope.currentObj.Id, FoldersTree: false, MultipleImages: $scope.parentObject.PropertiesGroups[parentIndex].Properties[index].Type == "Multiple Images", Value: $scope.parentObject.PropertiesGroups[parentIndex].Properties[index].Value }, function (value) {
            $scope.parentObject.PropertiesGroups[parentIndex].Properties[index].Value = value;
        }, null);
    }

    $scope.resetField = function (index, parentIndex) {
        $scope.parentObject.PropertiesGroups[parentIndex].Properties[index].Value = '';
    }

    $scope.advancedSearch = function () {
        var modalInstance = $modal.open({
            templateUrl: 'AdvancedSearch.html',
            controller: advancedSearchController,
            size: 'lg',
            resolve: {
                Templates: function () {
                    return $scope.templates;
                },
                PropertiesGroups: function () {
                    return $scope.PropertiesGroups;
                },
                DataProvider: function () {
                    return CompetitiveAnalysisDataProvider;
                },
                UMP: function () {
                    return UserMessagesProvider;
                },
                BrandFactoryTypes: function () { return $scope.BrandFactoryTypes; },
                CurrentTemplate: function () { return $scope.currentTemplate; }
            }
        });
        modalInstance.result.then(function (result) {
            $scope.clearProductsList();
            if ($scope.Products == undefined) {
                $scope.Products = [];
            }
            var obj;
            for (i = 0; i < result.objs.length; i++) {
                obj = result.objs[i];
                var exists = false;
                for (j = 0; j < $scope.Products.length; j++) {
                    if ($scope.Products[j].Id == obj.Id) {
                        exists = true;
                        break;
                    }
                }
                if (!exists) {
                    $scope.Products.push(obj);
                }
            }
        }, function () {
        });

    }

    $scope.clearProductsList = function () {
        $scope.Products = [];
        $scope.productPattern = '';
        $scope.closeDetailsForm();
    }

    $scope.showImage = function (value) {
        UserMessagesProvider.imageDisplayer(value);
    }

    initialize();
});
var importProductsReportController = function ($scope, $modalInstance, UMP, Report) {
    $scope.Report = Report;
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
var createProductController = function ($scope, $modalInstance, UMP, templates, BrandFactoryTypes, Companies) {
    $scope.newNode = {};
    $scope.createForm = {};
    $scope.templates = templates;
    $scope.BrandFactoryTypes = BrandFactoryTypes;
    $scope.Companies = Companies;
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
var importProductsController = function ($scope, $modalInstance, UMP, IntentsProvider, templates, BrandFactoryTypes, Companies) {
    $scope.newNode = {};
    $scope.importForm = {};
    $scope.templates = templates;
    $scope.BrandFactoryTypes = BrandFactoryTypes;
    $scope.Companies = Companies;

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
var importProductsImagesController = function ($scope, $modalInstance, UMP, IntentsProvider) {
    $scope.newNode = {};
    $scope.importForm = {};

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
            $scope.newNode.ZipPath = value;
        }, null);
    }

    $scope.resetField = function () {
        $scope.newNode.ZipPath = '';
    }
}
var advancedSearchController = function ($filter, $scope, $modal, $modalInstance, BrandFactoryTypes, Templates, PropertiesGroups, DataProvider, UMP, CurrentTemplate) {
    var ct;
    $scope.templates = Templates;
    $scope.propertiesGroups = PropertiesGroups;
    $scope.Selected = { SelectedTemplate: null, SelectedTemplateId: null, BrandFactoryTypes: '', Tags: '', CreateDateFrom: '', CreateDateTo: '', UpdateDateFrom: '', UpdateDateTo: '' };
    $scope.Filters = [];
    $scope.OrFilters = [];
    $scope.Products = [];
    $scope.ProductsLoaded = false;
    $scope.SelectedProducts = [];
    $scope.ProductsToCompare = { arr: [] };
    $scope.LoadingProducts = false;
    $scope.Tags = [];
    $scope.BrandFactoryTypes = BrandFactoryTypes;
    $scope.allProductsCheckBox = { checked: false };

    function initialize() {
        var searchedTags = new Bloodhound({
            datumTokenizer: Bloodhound.tokenizers.obj.whitespace('Name'),
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            remote: {
                url: '/DConfig/CompetitiveAnalysis/getProductsTags?Pattern=%QUERY',
                wildcard: '%QUERY'
            }
        });
        searchedTags.initialize();
        $scope.typeheadjs_tags = { name: 'Tags', displayKey: 'Name', displayValue: 'Name', source: searchedTags.ttAdapter() };

        $scope.$watch('Selected.SelectedTemplateId', function (newValue, oldValue) {
            // Ignore initial setup.
            if (newValue === oldValue) {
                return;
            }
            for (i = 0; i < $scope.templates.length; i++) {
                if ($scope.templates[i].Id == $scope.Selected.SelectedTemplateId) {
                    $scope.Selected.SelectedTemplate = $scope.templates[i];
                    break;
                }
            }
            if (oldValue == undefined) {
                initializePropertiesGroups();
            } else {
                UMP.confirmHandler("Changing product type will remove all the filters and loaded products, are you sure you want to do that?", function () {
                    $scope.Filters = [];
                    $scope.OrFilters = [];
                    $scope.Products = [];
                    $scope.ProductsLoaded = false;
                    initializePropertiesGroups();
                }, null);
            }
        });

        $scope.$watch('allProductsCheckBox.checked', function (newValue, oldValue) {
            // Ignore initial setup.
            if (newValue === oldValue) {
                return;
            }
            if (newValue == true) {
                $scope.ProductsToCompare.arr = [];
                for (i = 0; i < $scope.Products.length; i++) {
                    $scope.ProductsToCompare.arr.push($scope.Products[i]);
                }
            } else {
                $scope.ProductsToCompare.arr = [];
            }
        });

        $scope.Selected.SelectedTemplateId = CurrentTemplate.Id;
    }

    function initializePropertiesGroups() {
        var addedGroupsCounter = -1;
        var groupAdded = false;
        $scope.Selected.SelectedTemplate.PropertiesGroups = [];
        if ($scope.propertiesGroups != undefined) {
            for (i = 0; i < $scope.propertiesGroups.length; i++) {
                for (j = 0; j < $scope.Selected.SelectedTemplate.Properties.length; j++) {
                    if ($scope.Selected.SelectedTemplate.Properties[j].GroupId == $scope.propertiesGroups[i].Id) {
                        if (!groupAdded) {
                            addedGroupsCounter++;
                            groupAdded = true;
                            $scope.Selected.SelectedTemplate.PropertiesGroups.push({ Id: $scope.propertiesGroups[i].Id, Name: $scope.propertiesGroups[i].Name, Properties: [] });
                        }
                        $scope.Selected.SelectedTemplate.PropertiesGroups[addedGroupsCounter].Properties.push(jQuery.extend(true, {}, $scope.Selected.SelectedTemplate.Properties[j]));
                    }
                }
                groupAdded = false;
            }
        }
    }

    $scope.addFilter = function () {
        if ($scope.Selected.SelectedTemplate != undefined) {
            var modalInstance = $modal.open({
                templateUrl: 'addFilter.html',
                controller: advancedSearchFiltersController,
                size: 'lg',
                resolve: {
                    SelectedTemplate: function () {
                        return $scope.Selected.SelectedTemplate;
                    },
                    UMP: function () {
                        return UMP;
                    },
                    ProductsLoaded: function () {
                        return $scope.ProductsLoaded;
                    }
                }
            });
            modalInstance.result.then(function (obj) {
                $scope.Filters.push(obj);
            }, function () {
            });
        } else {
            UMP.errorHandler(999, "Please choose the product type first");
        }
    }

    $scope.addOrFilter = function () {
        if ($scope.Selected.SelectedTemplate != undefined) {
            var modalInstance = $modal.open({
                templateUrl: 'addFilter.html',
                controller: advancedSearchFiltersController,
                size: 'lg',
                resolve: {
                    SelectedTemplate: function () {
                        return $scope.Selected.SelectedTemplate;
                    },
                    UMP: function () {
                        return UMP;
                    },
                    ProductsLoaded: function () {
                        return $scope.ProductsLoaded;
                    }
                }
            });
            modalInstance.result.then(function (obj) {
                $scope.OrFilters.push(obj);
            }, function () {
            });
        } else {
            UMP.errorHandler(999, "Please choose the product type first");
        }
    }

    $scope.removeFilter = function (index) {
        if (!$scope.Filters[index].ProductsLoaded) {
            UMP.confirmHandler("Loaded products are based on this filter, removing it will needs reloeading products again, are you sure you want to continue?", function () {
                $scope.Products = [];
                $scope.ProductsLoaded = false;
                $scope.Filters.splice(index, 1);
            }, null);
        } else {
            $scope.Filters.splice(index, 1);
        }
    }

    $scope.removeOrFilter = function (index) {
        if (!$scope.Filters[index].ProductsLoaded) {
            UMP.confirmHandler("Loaded products are based on this filter, removing it will needs reloeading products again, are you sure you want to continue?", function () {
                $scope.Products = [];
                $scope.ProductsLoaded = false;
                $scope.OrFilters.splice(index, 1);
            }, null);
        } else {
            $scope.OrFilters.splice(index, 1);
        }
    }

    function initializeProductsTree() {
        if ($scope.Selected.SelectedTemplate != undefined) {
            $scope.Products = [];
            $scope.ProductsToCompare.arr = [];
            UMP.addLoadingToWindow('productsTree_productSelector');
            $scope.LoadingProducts = true;
            var brandTypes = null;
            var tags = null;
            var createDateRange = null;
            var updateDateRange = null;
            if ($scope.Selected.BrandFactoryTypes != '') {
                brandTypes = $scope.Selected.BrandFactoryTypes.split(',');
            }
            if ($scope.Selected.Tags.split(',') != '') {
                tags = $scope.Selected.Tags.split(',');
            }
            if ($scope.Selected.CreateDateFrom != null && $scope.Selected.CreateDateTo != null) {
                createDateRange = [$scope.Selected.CreateDateFrom, $scope.Selected.CreateDateTo];
            }
            if ($scope.Selected.UpdateDateFrom != null && $scope.Selected.UpdateDateTo != null) {
                updateDateRange = [$scope.Selected.UpdateDateFrom, $scope.Selected.UpdateDateTo];
            }
            DataProvider.getTemplateProductsByFilters($scope.Selected.SelectedTemplate.Id, brandTypes, tags, $scope.Filters, $scope.OrFilters, createDateRange, updateDateRange).then(function (data) {
                for (i = 0; i < data.data.length; i++) {
                    $scope.Products.push(data.data[i]);
                }
                UMP.removeLoadingFromWindow('productsTree_productSelector');
                $scope.LoadingProducts = false;
            }, function (data, status, headers, config) {
                UMP.removeLoadingFromWindow('productsTree_productSelector');
                UMP.errorHandler(status);
                $scope.LoadingProducts = false;
            });
        } else {
            UMP.errorHandler(999, 'Please select the product template first');
        }
    }

    $scope.ok = function (saveReport) {
        if ($scope.ProductsToCompare.arr.length > 0) {
            $modalInstance.close({ objs: $scope.ProductsToCompare.arr, template: $scope.Selected.SelectedTemplate });
        } else {
            UMP.errorHandler(999, "You haven't choose any product to compare");
        }
    };

    $scope.removeProductFromCompare = function (index) {
        $scope.ProductsToCompare.arr.splice(index, 1);
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.loadProducts = function () {
        for (i = 0; i < $scope.Filters.length; i++) {
            $scope.Filters[i].ProductsLoaded = false;
        }
        initializeProductsTree();
        $scope.ProductsLoaded = true;
    }

    initialize();
};
var advancedSearchFiltersController = function ($filter, $scope, $modalInstance, SelectedTemplate, UMP, ProductsLoaded) {
    $scope.Selected = { SelectedTemplate: SelectedTemplate, SelectedGroup: null, SelectedProperty: null };
    $scope.FilterValues = { Values: '' };
    $scope.FilterNumericalRange = { min: '', max: '' };
    $scope.FilterNumericBaseValues = { min: 0, max: 1000000 };
    $scope.FilterNumericalStep = 1;
    $scope.FilterDateRange = { from: '', to: '' };
    $scope.ProductsLoaded = ProductsLoaded;

    $scope.$watch('Selected.SelectedGroup', function (newValue, oldValue) {
        $scope.Selected.SelectedProperty = null;
        $scope.FilterValues.Values = '';
        $scope.FilterNumericalRange = { min: '', max: '' };
        $scope.FilterDateRange = { from: '', to: '' };
    });

    $scope.$watch('Selected.SelectedProperty', function (newValue, oldValue) {
        // Ignore initial setup.
        if (newValue === oldValue) {
            return;
        }
        //var min = 0;
        //var max = 0;
        $scope.FilterValues.Values = '';
        $scope.FilterNumericalRange = { min: '', max: '' };
        $scope.FilterDateRange = { from: '', to: '' };
        //if ($scope.Selected.SelectedProperty.Type == 'Number') {
        //    for (j = 0; j < $scope.Products.length; j++) {
        //        for (k = 0; k < $scope.Products[j].PropertiesValues.length; k++) {
        //            if ($scope.Products[j].PropertiesValues[k].PropertyId == $scope.Selected.SelectedProperty.Id) {
        //                if (j == 0) {
        //                    max = $scope.Products[j].PropertiesValues[k].Value;
        //                    min = max;
        //                } else {
        //                    if (min > $scope.Products[j].PropertiesValues[k].Value) {
        //                        min = $scope.Products[j].PropertiesValues[k].Value;
        //                    }
        //                    if (max < $scope.Products[j].PropertiesValues[k].Value) {
        //                        max = $scope.Products[j].PropertiesValues[k].Value;
        //                    }
        //                }
        //                break;
        //            }
        //        }
        //    }
        //    $scope.FilterNumericBaseValues.min = min;
        //    $scope.FilterNumericBaseValues.min = min;
        //    $scope.FilterNumericBaseValues.max = max;
        //    $scope.FilterNumericBaseValues.max = max;
        //    $scope.FilterNumericalStep = (max - min) / 20;
        //}
    });

    $scope.ok = function () {
        var filter;
        switch ($scope.Selected.SelectedProperty.Type) {
            case 'String':
                filter = { Text: $scope.Selected.SelectedProperty.Name + ' equal one of the following values: ' + $scope.FilterValues.Values, PropertyId: $scope.Selected.SelectedProperty.Id, Type: $scope.Selected.SelectedProperty.Type, Values: $scope.FilterValues.Values, ProductsLoaded: $scope.ProductsLoaded };
                break;
            case 'String - Multiple Lines':
                filter = { Text: $scope.Selected.SelectedProperty.Name + ' equal one of the following values: ' + $scope.FilterValues.Values, PropertyId: $scope.Selected.SelectedProperty.Id, Type: $scope.Selected.SelectedProperty.Type, Values: $scope.FilterValues.Values, ProductsLoaded: $scope.ProductsLoaded };
                break;
            case 'Date':
                filter = { Text: $scope.Selected.SelectedProperty.Name + ' from ' + $scope.FilterDateRange.from + ' to ' + $scope.FilterDateRange.to, PropertyId: $scope.Selected.SelectedProperty.Id, Type: $scope.Selected.SelectedProperty.Type, Values: $scope.FilterDateRange, MinDate: $scope.FilterDateRange.from, MaxDate: $scope.FilterDateRange.to, ProductsLoaded: $scope.ProductsLoaded };
                break;
            case 'Predefined List':
            case 'Predefined List - Checkboxes':
            case 'Predefined List - Radio Buttons':
                filter = { Text: $scope.Selected.SelectedProperty.Name + ' equal one of the following values: ' + $scope.FilterValues.Values, PropertyId: $scope.Selected.SelectedProperty.Id, Type: $scope.Selected.SelectedProperty.Type, Values: $scope.FilterValues.Values, ProductsLoaded: $scope.ProductsLoaded };
                break;
            case 'Boolean':
                filter = { Text: $scope.Selected.SelectedProperty.Name + ' is ' + $scope.FilterValues.Values, PropertyId: $scope.Selected.SelectedProperty.Id, Type: $scope.Selected.SelectedProperty.Type, Values: $scope.FilterValues.Values, BoolValue: $scope.FilterValues.Values, ProductsLoaded: $scope.ProductsLoaded };
                break;
            case 'Number':
                filter = { Text: $scope.Selected.SelectedProperty.Name + ' between ' + $scope.FilterNumericalRange.min + ' and ' + $scope.FilterNumericalRange.max, PropertyId: $scope.Selected.SelectedProperty.Id, Type: $scope.Selected.SelectedProperty.Type, Values: $scope.FilterNumericalRange, MinValue: $scope.FilterNumericalRange.min, MaxValue: $scope.FilterNumericalRange.max, ProductsLoaded: $scope.ProductsLoaded };
                break;
        }
        $modalInstance.close(filter);
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    function resetFilterBoxes() {
        $scope.Selected.SelectedGroup = null;
        $scope.Selected.SelectedProperty = null;
        $scope.FilterValues.Values = '';
        $scope.FilterNumericalRange = { min: '', max: '' };
        $scope.FilterDateRange = { from: '', to: '' };
    }
    resetFilterBoxes();
};
angular.module('DConfig').filterProvider.register('propertiesFilter', [function () {
    return function (products, propertiesFilters, propertiesOrFilter) {
        if (!angular.isUndefined(products) && !angular.isUndefined(propertiesFilters) && propertiesFilters.length > 0) {
            var tempProducts = [];
            var valid = true;
            var orValid = false;
            var propertyExists = false;
            angular.forEach(products, function (product) {
                valid = true;
                orValid = false;
                propertyExists = false;
                angular.forEach(propertiesFilters, function (propertyFilter) {
                    for (i = 0; i < product.PropertiesValues.length; i++) {
                        if (product.PropertiesValues[i].PropertyId == propertyFilter.PropertyId) {
                            propertyExists = true;
                            if (product.PropertiesValues[i].Value != '') {
                                switch (propertyFilter.Type) {
                                    case 'Date':
                                        var productDate = new Date(product.PropertiesValues[i].Value);
                                        var fromDate = new Date(propertyFilter.Values.from);
                                        var toDate = new Date(propertyFilter.Values.to);
                                        if (fromDate > productDate || toDate < productDate) {
                                            valid = false;
                                        }
                                        break;
                                    case 'String':
                                    case 'String - Multiple Lines':
                                    case 'Predefined List':
                                    case 'Predefined List - Radio Buttons':
                                    case 'Predefined List - Filter/Select':
                                        if (!propertyFilter.Values.contains(product.PropertiesValues[i].Value)) {
                                            valid = false;
                                        }
                                        break;
                                    case 'Predefined List - Checkboxes':
                                        var valuesArr = propertyFilter.Values.split(",");
                                        valid = false;
                                        for (i = 0; i < valuesArr.length; i++) {
                                            if (product.PropertiesValues[i].Value.contains(valuesArr[i])) {
                                                valid = true;
                                                break;
                                            }
                                        }
                                        break;
                                    case 'Boolean':
                                        if (propertyFilter.Values != product.PropertiesValues[i].Value) {
                                            valid = false;
                                        }
                                        break;
                                    case 'Number':
                                        var min = parseFloat(propertyFilter.Values.min);
                                        var max = parseFloat(propertyFilter.Values.max);
                                        var value = parseFloat(product.PropertiesValues[i].Value);
                                        if (min > value || max < value) {
                                            valid = false;
                                        }
                                        break;
                                }
                            } else {
                                valid = false;
                            }
                            break;
                        }
                    }
                });
                if (propertiesOrFilter != undefined && propertiesOrFilter.length > 0) {
                    angular.forEach(propertiesOrFilter, function (propertyFilter) {
                        for (i = 0; i < product.PropertiesValues.length; i++) {
                            if (product.PropertiesValues[i].PropertyId == propertyFilter.PropertyId) {
                                propertyExists = true;
                                if (product.PropertiesValues[i].Value != '') {
                                    switch (propertyFilter.Type) {
                                        case 'Date':
                                            var productDate = new Date(product.PropertiesValues[i].Value);
                                            var fromDate = new Date(propertyFilter.Values.from);
                                            var toDate = new Date(propertyFilter.Values.to);
                                            if (fromDate <= productDate && toDate >= productDate) {
                                                orValid = true;
                                            }
                                            break;
                                        case 'String':
                                        case 'String - Multiple Lines':
                                        case 'Predefined List':
                                        case 'Predefined List - Radio Buttons':
                                        case 'Predefined List - Filter/Select':
                                            if (propertyFilter.Values.contains(product.PropertiesValues[i].Value)) {
                                                orValid = true;
                                            }
                                            break;
                                        case 'Predefined List - Checkboxes':
                                            var valuesArr = propertyFilter.Values.split(",");
                                            for (i = 0; i < valuesArr.length; i++) {
                                                if (product.PropertiesValues[i].Value.contains(valuesArr[i])) {
                                                    orValid = true;
                                                }
                                            }
                                            break;
                                        case 'Boolean':
                                            if (propertyFilter.Values == product.PropertiesValues[i].Value) {
                                                orValid = true;
                                            }
                                            break;
                                        case 'Number':
                                            var min = parseFloat(propertyFilter.Values.min);
                                            var max = parseFloat(propertyFilter.Values.max);
                                            var value = parseFloat(product.PropertiesValues[i].Value);
                                            if (min <= value && max >= value) {
                                                orValid = true;
                                            }
                                            break;
                                    }
                                }
                                break;
                            }
                        }
                    });
                } else {
                    orValid = true;
                }
                if (valid && orValid && propertyExists) {
                    tempProducts.push(product);
                }
            });
            return tempProducts;
        } else {
            return products;
        }
    };
}]);
