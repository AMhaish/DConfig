angular.module('DConfig').controllerProvider.register('CompetitiveAnalysis.ComparisonController', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, CompetitiveAnalysisDataProvider, EventsProvider, $modal, scopeService, $filter, $sce, $cookies) {
    $scope.DetailsPanelStates = {
        None: 0,
        Comparing: 1,
        Charts: 2,
        Info: 3,
        Share: 4
    };
    $scope.BrandFactoryTypes = [
        { Name: 'Own', Value: 'Own' },
        { Name: 'Supplier', Value: 'Supplier' },
        { Name: 'Competitor', Value: 'Competitor' },
    ];
    $scope.products = [];
    $scope.productsCols = [];
    $scope.AllRows = [];
    $scope.rows = [];
    $scope.csvRows = [];
    $scope.allCsvRows = [];
    $scope.numericalProperties = [];
    $scope.displayedRows = null;
    $scope.showChecked = true;
    $scope.gridOptions = {
        showFilter: true,
        enableColumnResize: true,
        data: 'rows',
        columnDefs: 'productsCols',
        enablePinning: true,
        showFooter: true,
    };
    $scope.status = {
        isopen: false
    };
    $('.dropdown-menu').on('click', function (e) {
        if ($(this).hasClass('dropdown-menu-form')) {
            e.stopPropagation();
        }
    });
    $scope.toggleDropdown = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.status.isopen = !$scope.status.isopen;
    };
    $scope.state = $scope.DetailsPanelStates.None;

    $scope.getHeader = function () {
        var columns = [];
        columns.push('Group Name');
        columns.push('Property Name');
        for (i = 0; i < $scope.productsCols.length; i++) {
            columns.push($scope.productsCols[i].displayName);
        }
        return columns;
    }

    $scope.showCheckedOnly = function (bool) {
        $scope.showChecked = bool;
        if ($scope.showChecked)
            $scope.displayedRows = $scope.rows;
        else
            $scope.displayedRows = $scope.AllRows;
    }

    $scope.toggleRowVisibility = function (row, bool) {
        row.show = bool;
    }

    var checkingUnsavedReports = function () {
        var unsaved = false;
        for (i = 0; i < $scope.Reports.length; i++) {
            if (!$scope.Reports[i].Saved) {
                unsaved = true;
            }
        }
        if (unsaved) {
            if (confirm("Some reports havn't been saved yet, are you sure you want to continue?")) {
                return true;
            } else {
                EventsProvider.AddHandlerToOnRouteChangeStart(checkingUnsavedReports);
                return false;
            }
            //UserMessagesProvider.confirmHandler("Some reports havn't been saved yet, are you sure you want to continue?", function () {
            //    return true;
            //}, function () {
            //    EventsProvider.AddHandlerToOnRouteChangeStart(checkingUnsavedReports);
            //    return false;
            //});
        }
    }

    function initialize() {
        BreadCrumpsProvider.breadCrumps.path = ['Competitive Analysis', 'Comparison'];
        UserMessagesProvider.displayProgress(3);
        CompetitiveAnalysisDataProvider.getTemplates().then(function (data) {
            $scope.Templates = data.data;
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
        CompetitiveAnalysisDataProvider.getPropertiesEnums().then(function (data) {
            $scope.Enums = data.data;
            UserMessagesProvider.increaseProgress();
        }, function (data, status, headers, config) {
            UserMessagesProvider.increaseProgress();
            UserMessagesProvider.errorHandler(status);
        });
        CompetitiveAnalysisDataProvider.getComparisons().then(function (data) {
            $scope.Reports = data.data;
            for (i = 0; i < $scope.Reports.length; i++) {
                $scope.Reports[i].Saved = true;
            }
            UserMessagesProvider.increaseProgress();
        }, function (data, status, headers, config) {
            UserMessagesProvider.increaseProgress();
            UserMessagesProvider.errorHandler(status);
        });
        EventsProvider.AddHandlerToOnRouteChangeStart(checkingUnsavedReports);
    }

    $scope.closeDetailsForm = function () {
        $scope.state = $scope.DetailsPanelStates.None;
    }

    $scope.displayDetails = function (index, callback) {
        $scope.selectedIndex = index;
        $scope.currentReport = jQuery.extend(true, {}, $scope.Reports[index]);
        $scope.products = $scope.Reports[index].Products;
        $scope.state = $scope.DetailsPanelStates.Comparing;
        if ($scope.currentReport.Template == undefined && $scope.products.length > 0) {
            UserMessagesProvider.displayLoading();
            CompetitiveAnalysisDataProvider.getTemplate($scope.products[0].TemplateId).then(function (data) {
                $scope.currentReport.Template = data.data;
                $scope.Reports[$scope.selectedIndex].Template = data.data;
                InitializeComparisionTable();
                UserMessagesProvider.hideLoading();
                if (callback != undefined) {
                    callback();
                }
            }, function (data, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        } else {
            InitializeComparisionTable();
            if (callback != undefined) {
                callback();
            }
        }
    }

    function InitializeComparisionTable() {
        if ($scope.products.length > 0) {
            $scope.productsCols = [];
            $scope.rows = [];
            //Initialize Columns
            //$scope.productsCols.push({ field: 'PropertyName', displayName: 'Property Name', cellTemplate: '<div class="PropertiesColumn"><div class="ngCellText">{{row.getProperty(col.field)}}</div></div>' });
            for (k = 0; k < $scope.products.length; k++) {
                $scope.productsCols.push({
                    field: 'P' + $scope.products[k].Id.toString(), displayName: $scope.products[k].Name,
                    worst: 'W' + $scope.products[k].Id.toString(),
                    best: 'B' + $scope.products[k].Id.toString()
                    //cellTemplate: '<div><div class="ngCellText"><span ng-bind-html="RenderGridCellView(row.getProperty(col.field),row.getProperty(\'Type\'))"></span></div></div>'
                });
            }
            //Binding Rows
            BindingComparisionTableRows();
            if ($scope.showChecked)
                $scope.displayedRows = $scope.rows;
            else
                $scope.displayedRows = $scope.AllRows;
        } else {
            $scope.productsCols = [];
            $scope.rows = [];
        }
    }

    $scope.CurrentGroup;//For rendering purposes
    $scope.CurrentGroupRow;
    $scope.CurrentGroupName;
    function BindingComparisionTableRows() {
        var rows = [];
        var csvRows = [];
        $scope.currentReport.Template.Properties.push({ Name: 'SSRP', GroudId: 0, Type: 'String', Id: 0, DisplayAs: 'SSRP Price' });
        $scope.currentReport.Template.Properties.push({ Name: 'NORM', GroudId: 0, Type: 'String', Id: 0, DisplayAs: 'Normal Price' });
        $scope.currentReport.Template.Properties.push({ Name: 'QTY', GroudId: 0, Type: 'String', Id: 0, DisplayAs: 'Quantity Price' });
        $scope.currentReport.PGroups = [];//For Sharing section
        for (i = 0; i < $scope.currentReport.Template.Properties.length; i++) {
            var row = {};
            var csvRow = {};
            row.show = true;
            row['PropertyName'] = $scope.currentReport.Template.Properties[i].DisplayAs;
            row['PropertyUnit'] = $scope.currentReport.Template.Properties[i].Unit;
            row['GroupId'] = $scope.currentReport.Template.Properties[i].GroupId;
            if (row['GroupId'] != $scope.CurrentGroup) {
                $scope.CurrentGroup = $scope.currentReport.Template.Properties[i].GroupId;
                row['Counter'] = 1;
                row['BaseCounter'] = 1;
                $scope.CurrentGroupRow = row;
                for (v = 0; v < $scope.PropertiesGroups.length; v++) {
                    if ($scope.PropertiesGroups[v].Id == $scope.currentReport.Template.Properties[i].GroupId) {
                        row['GroupName'] = $scope.PropertiesGroups[v].DisplayAs;
                        $scope.CurrentGroupName = $scope.PropertiesGroups[v].DisplayAs;
                        break;
                    }
                }
                if ($scope.currentReport.Template.Properties[i].Id != 0) {//Not price
                    $scope.currentReport.PGroups.push({ Name: $scope.CurrentGroupName, Properties: [] });//For Sharing section
                }
            } else {
                $scope.CurrentGroupRow['Counter']++;
                $scope.CurrentGroupRow['BaseCounter']++;
                row['GroupName'] = $scope.CurrentGroupName;
            }
            if ($scope.currentReport.Template.Properties[i].Id != 0) {//Not price
                $scope.currentReport.PGroups[$scope.currentReport.PGroups.length - 1].Properties.push($scope.currentReport.Template.Properties[i]);//For Sharing section
            }
            row['Type'] = $scope.currentReport.Template.Properties[i].Type;
            csvRow['GroupName'] = $scope.CurrentGroupName;
            csvRow['PropertyName'] = row['PropertyName'];
            for (j = 0; j < $scope.products.length; j++) {
                if ($scope.currentReport.Template.Properties[i].Id == 0) {
                    var currentPriceFound = false;
                    var quantityPrices = [];
                    for (k = 0; k < $scope.products[j].Prices.length; k++) {
                        if ($scope.currentReport.Template.Properties[i].Name == 'SSRP' && $scope.products[j].Prices[k].PriceType == 'SSRP') {
                            row['P' + $scope.products[j].Id.toString()] = $scope.products[j].Prices[k].PriceValue + ' ' + $scope.products[j].Prices[k].Currency;
                        } else if ($scope.currentReport.Template.Properties[i].Name == 'QTY' && $scope.products[j].Prices[k].PriceType == 'QTY') {
                            if (quantityPrices.length <= 0) {
                                row['P' + $scope.products[j].Id.toString()] = ($scope.products[j].Prices[k].QuantityFrom != null ? $scope.products[j].Prices[k].QuantityFrom : '0') + ' to ' + ($scope.products[j].Prices[k].QuantityTo != null ? $scope.products[j].Prices[k].QuantityTo : '0') + ' : ' + $scope.products[j].Prices[k].PriceValue + ' ' + $scope.products[j].Prices[k].Currency;
                                quantityPrices.push($scope.products[j].Prices[k].QuantityFrom + '_' + $scope.products[j].Prices[k].QuantityTo);
                            }
                            else if (quantityPrices.indexOf($scope.products[j].Prices[k].QuantityFrom + '_' + $scope.products[j].Prices[k].QuantityTo)) {
                                row['P' + $scope.products[j].Id.toString()] += ' / ' + ($scope.products[j].Prices[k].QuantityFrom != null ? $scope.products[j].Prices[k].QuantityFrom : '0') + ' to ' + ($scope.products[j].Prices[k].QuantityTo != null ? $scope.products[j].Prices[k].QuantityTo : '0') + ' : ' + $scope.products[j].Prices[k].PriceValue + ' ' + $scope.products[j].Prices[k].Currency;
                            }
                        } else if ($scope.currentReport.Template.Properties[i].Name == 'NORM' && $scope.products[j].Prices[k].PriceType == 'NORM') {
                            if (!currentPriceFound) {
                                currentPriceFound = true;
                                row['P' + $scope.products[j].Id.toString()] = $scope.products[j].Prices[k].PriceValue + ' ' + $scope.products[j].Prices[k].Currency;
                            }
                        }
                    }
                } else {
                    for (k = 0; k < $scope.products[j].PropertiesValues.length; k++) {
                        if ($scope.currentReport.Template.Properties[i].Id == $scope.products[j].PropertiesValues[k].PropertyId) {
                            row['P' + $scope.products[j].Id.toString()] = $scope.products[j].PropertiesValues[k].Value;
                        }
                    }
                }
                csvRow['P' + $scope.products[j].Id.toString()] = row['P' + $scope.products[j].Id.toString()];
            }
            switch ($scope.currentReport.Template.Properties[i].Type) {
                case 'Predefined List':
                case 'Predefined List - Checkboxes':
                case 'Predefined List - Radio Buttons':
                    var worstOne = [];
                    var bestOne = [];
                    var basicValue = undefined;
                    var basicValueForBest = undefined;
                    for (j = 0; j < $scope.products.length; j++) {
                        if (basicValue == undefined) {
                            basicValue = ResolveEnumItemsWeight($scope.currentReport.Template.Properties[i], row['P' + $scope.products[j].Id.toString()]);
                            worstOne.push($scope.products[j].Id.toString());
                        } else {
                            var newValue = ResolveEnumItemsWeight($scope.currentReport.Template.Properties[i], row['P' + $scope.products[j].Id.toString()]);
                            if (basicValue == newValue) {
                                worstOne.push($scope.products[j].Id.toString());
                            } else if (basicValue > newValue) {
                                basicValue = newValue;
                                worstOne = [];
                                worstOne.push($scope.products[j].Id.toString());
                            }
                        }
                        if (basicValueForBest == undefined) {
                            basicValueForBest = ResolveEnumItemsWeight($scope.currentReport.Template.Properties[i], row['P' + $scope.products[j].Id.toString()]);
                            bestOne.push($scope.products[j].Id.toString());
                        } else {
                            var newValue = ResolveEnumItemsWeight($scope.currentReport.Template.Properties[i], row['P' + $scope.products[j].Id.toString()]);
                            if (basicValueForBest == newValue) {
                                bestOne.push($scope.products[j].Id.toString());
                            } else if (basicValueForBest < newValue) {
                                basicValueForBest = newValue;
                                bestOne = [];
                                bestOne.push($scope.products[j].Id.toString());
                            }
                        }
                    }
                    if (worstOne.length == $scope.products.length) {
                        worstOne = [];
                    }
                    if (bestOne.length == $scope.products.length) {
                        bestOne = [];
                    }
                    for (j = 0; j < $scope.products.length; j++) {
                        row['W' + $scope.products[j].Id.toString()] = false;
                        row['B' + $scope.products[j].Id.toString()] = false;
                        for (k = 0; k < worstOne.length; k++) {
                            if (worstOne[k] == $scope.products[j].Id.toString()) {
                                row['W' + $scope.products[j].Id.toString()] = true;
                            }
                        }
                        for (k = 0; k < bestOne.length; k++) {
                            if (bestOne[k] == $scope.products[j].Id.toString()) {
                                row['B' + $scope.products[j].Id.toString()] = true;
                            }
                        }
                    }
                    worstOne = [];
                    bestOne = [];
                    break;
                case 'Number':
                    var worstOne = [];
                    var bestOne = [];
                    var basicValue = undefined;
                    var basicValueForBest = undefined;
                    if ($scope.currentReport.Template.Properties[i].LargerIsBetter != null) {
                        for (j = 0; j < $scope.products.length; j++) {
                            if (basicValue == undefined) {
                                basicValue = parseFloat(row['P' + $scope.products[j].Id.toString()]);
                                worstOne.push($scope.products[j].Id.toString());
                            } else {
                                var newValue = parseFloat(row['P' + $scope.products[j].Id.toString()]);
                                if ($scope.currentReport.Template.Properties[i].LargerIsBetter) {
                                    if (basicValue == newValue) {
                                        worstOne.push($scope.products[j].Id.toString());
                                    } else if (basicValue > newValue) {
                                        basicValue = newValue;
                                        worstOne = [];
                                        worstOne.push($scope.products[j].Id.toString());
                                    }
                                } else {
                                    if (basicValue == newValue) {
                                        worstOne.push($scope.products[j].Id.toString());
                                    } else if (basicValue < newValue) {
                                        basicValue = newValue;
                                        worstOne = [];
                                        worstOne.push($scope.products[j].Id.toString());
                                    }
                                }
                            }
                            if (basicValueForBest == undefined) {
                                basicValueForBest = parseFloat(row['P' + $scope.products[j].Id.toString()]);
                                bestOne.push($scope.products[j].Id.toString());
                            } else {
                                var newValue = parseFloat(row['P' + $scope.products[j].Id.toString()]);
                                if (!$scope.currentReport.Template.Properties[i].LargerIsBetter) {
                                    if (basicValueForBest == newValue) {
                                        bestOne.push($scope.products[j].Id.toString());
                                    } else if (basicValueForBest > newValue) {
                                        basicValueForBest = newValue;
                                        bestOne = [];
                                        bestOne.push($scope.products[j].Id.toString());
                                    }
                                } else {
                                    if (basicValueForBest == newValue) {
                                        bestOne.push($scope.products[j].Id.toString());
                                    } else if (basicValueForBest < newValue) {
                                        basicValueForBest = newValue;
                                        bestOne = [];
                                        bestOne.push($scope.products[j].Id.toString());
                                    }
                                }
                            }
                        }
                        if (worstOne.length == $scope.products.length) {
                            worstOne = [];
                        }
                        if (bestOne.length == $scope.products.length) {
                            bestOne = [];
                        }
                        for (j = 0; j < $scope.products.length; j++) {
                            row['W' + $scope.products[j].Id.toString()] = false;
                            row['B' + $scope.products[j].Id.toString()] = false;
                            for (k = 0; k < worstOne.length; k++) {
                                if (worstOne[k] == $scope.products[j].Id.toString()) {
                                    row['W' + $scope.products[j].Id.toString()] = true;
                                }
                            }
                            for (k = 0; k < bestOne.length; k++) {
                                if (bestOne[k] == $scope.products[j].Id.toString()) {
                                    row['B' + $scope.products[j].Id.toString()] = true;
                                }
                            }
                        }
                    }
                    worstOne = [];
                    bestOne = [];
                    break;
            }
            $scope.$watch('AllRows[' + i.toString() + '].show', function (newValue, oldValue) {
                if (newValue === oldValue) {
                    return;
                }
                $scope.rows = $filter("propertiesNamesFilter")($scope.AllRows);
                $scope.csvRows = $filter("propertiesNamesFilter")($scope.allCsvRows);
            });
            rows.push(row);
            csvRows.push(csvRow);
        }

        $scope.AllRows = rows;
        $scope.rows = rows;
        $scope.csvRows = csvRows;
        $scope.allCsvRows = csvRows;
        UserMessagesProvider.hideLoading();
    }

    $scope.RenderGridCellView = function (bindedValue, type) {
        switch (type) {
            case 'Image':
                return $sce.trustAsHtml('<img src="' + bindedValue + '" style="height:128px"/>');
            default:
                return $sce.trustAsHtml(bindedValue);
        }
    }

    $scope.RenderGridRowClass = function (type) {
        switch (type) {
            case 'Image':
                return 'imageRow';
            default:
                return '';
        }
    }

    function ResolveEnumItemsWeight(property, value) {
        var enumObj;
        var weight = 0;
        for (p = 0; p < $scope.Enums.length; p++) {
            if ($scope.Enums[p].Id == property.EnumId) {
                enumObj = $scope.Enums[p];
            }
        }
        if (enumObj != undefined && value != undefined) {
            for (pp = 0; pp < enumObj.OrderedValues.length; pp++) {
                if (enumObj.OrderedValues[pp].Weight != null) {
                    if (value.contains(enumObj.OrderedValues[pp].Value + ',') || value.contains(',' + enumObj.OrderedValues[pp].Value) || value == enumObj.OrderedValues[pp].Value) {
                        weight += enumObj.OrderedValues[pp].Weight;
                    }
                }
            }
        }
        return weight;
    }
    //function initializeColumns(id) {
    //    UserMessagesProvider.displayLoading();
    //    CompetitiveAnalysisDataProvider.getTemplate(id).then(function (data) {
    //        $scope.Template = data.data;
    //        $scope.AllFields = [];
    //        $scope.AllFields.push({ field: 'Name', displayName: 'Product' });
    //        for (k = 0; k < $scope.Template.Properties.length; k++) {
    //            $scope.AllFields.push({ field: 'F' + $scope.Template.Properties[k].Id, displayName: $scope.Template.Properties[k].Name });
    //            if ($scope.Template.Properties[k].Type == "Number") {
    //                numericalProperties.push($scope.Template.Properties[k]);
    //            }
    //        }
    //        $scope.productsCols = $scope.AllFields;
    //        UserMessagesProvider.hideLoading();
    //    },function (data, status, headers, config) {
    //        UserMessagesProvider.hideLoading();
    //        UserMessagesProvider.errorHandler(status);
    //    });
    //}

    //function initializeProduct(product) {
    //    for (ll = 0; ll < product.PropertiesValues.length; ll++) {
    //        product['F' + product.PropertiesValues[ll].PropertyId.toString()] = product.PropertiesValues[ll].Value.toString();
    //    }
    //}

    $scope.displayComparision = function () {
        $scope.state = $scope.DetailsPanelStates.Comparing;
    }

    $scope.displayCharts = function () {
        $scope.state = $scope.DetailsPanelStates.Charts;
    }

    $scope.displayInfo = function () {
        $scope.state = $scope.DetailsPanelStates.Info;
    }

    function setEmbedPath() {
        $scope.Embed.Path = '<iframe id="comparisonIframe" src="http://' + window.location.host + '/_CAC/' + $scope.currentReport.Id + '?PropertiesIds=' + $scope.Embed.Properties + '" frameborder="' + $scope.Embed.Border + '" width="' + $scope.Embed.Width + '" height="' + $scope.Embed.Height + '"/>';
        $scope.Embed.PurePath = 'http://' + window.location.host + '/_CAC/' + $scope.currentReport.Id + '?PropertiesIds=' + $scope.Embed.Properties;
        $cookies['LastComEmbedCode' + $scope.currentReport.Id.toString()] = JSON.stringify($scope.Embed);
    }
    $scope.displaySharePanel = function () {
        $scope.state = $scope.DetailsPanelStates.Share;
        if ($cookies['LastComEmbedCode' + $scope.currentReport.Id.toString()] != undefined) {
            $scope.Embed = JSON.parse($cookies['LastComEmbedCode' + $scope.currentReport.Id.toString()]);
        } else {
            $scope.Embed = { Path: '', PurePath: '', Properties: '', Width: '100%', Height: '', Border: '0' };
        }
        $scope.$watch('Embed.Properties', function (newValue, oldValue) {
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

    $scope.deleteProduct = function (index) {
        UserMessagesProvider.confirmHandler("Are you sure you want to delete this product from report?", function () {
            $scope.Reports[$scope.selectedIndex].Saved = false;
            UserMessagesProvider.displayLoading();
            $scope.products.splice(index, 1);
            if ($scope.products.length > 0) {
                InitializeComparisionTable();
            } else {
                $scope.productsCols = [];
                $scope.rows = [];
            }
            UserMessagesProvider.hideLoading();
        }, null);
    }

    $scope.newReport = function () {
        var modalInstance = $modal.open({
            templateUrl: 'SelectProduct.html',
            controller: selectProductController,
            size: 'lg',
            resolve: {
                Templates: function () {
                    return $scope.Templates;
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
                newReport: function () {
                    return true;
                },
                BrandFactoryTypes: function () { return $scope.BrandFactoryTypes; },
                report: function () {
                    return undefined;
                }
            }
        });
        modalInstance.result.then(function (report) {
            $scope.products = [];
            var obj;
            for (i = 0; i < report.objs.length; i++) {
                obj = report.objs[i];
                if (obj && ($scope.products.length <= 0 || $scope.products[0].TemplateId == obj.TemplateId)) {
                    var exists = false;
                    for (i = 0; i < $scope.products.length; i++) {
                        if ($scope.products[i].Id == obj.Id) {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists) {
                        $scope.products.push(obj);
                    }
                } else {
                    if (obj) {
                        UserMessagesProvider.errorHandler(999, 'You just can add products from the same template you have added before for the comparision');
                        break;
                    }
                }
            }
            if ($scope.Reports == undefined) {
                $scope.Reports = [];
            }
            var resultFilters = [];
            if (report.andFilters != undefined && report.andFilters.length > 0) {
                for (i = 0; i < report.andFilters.length; i++) {
                    resultFilters.push({
                        ConditionType: 'And',
                        PropertyId: report.andFilters[i].PropertyId,
                        Keywords: report.andFilters[i].Values,
                        FromDate: report.andFilters[i].MinDate,
                        ToDate: report.andFilters[i].MaxDate,
                        FromValue: report.andFilters[i].MinValue,
                        ToValue: report.andFilters[i].MaxValue,
                        BoolValue: report.andFilters[i].BoolValue
                    });
                }
            }
            if (report.orFilters != undefined && report.orFilters.length > 0) {
                for (i = 0; i < report.orFilters.length; i++) {
                    resultFilters.push({
                        ConditionType: 'And',
                        PropertyId: report.orFilters[i].PropertyId,
                        Keywords: report.orFilters[i].Values,
                        FromDate: report.orFilters[i].MinDate,
                        ToDate: report.orFilters[i].MaxDate,
                        FromValue: report.orFilters[i].MinValue,
                        ToValue: report.orFilters[i].MaxValue,
                        BoolValue: report.orFilters[i].BoolValue
                    });
                }
            }
            $scope.Reports.push({
                Name: report.reportName,
                Products: $scope.products,
                Saved: false,
                Tags: report.tags,
                BrandFactoryTypes: report.brandFactoryTypes,
                Filters: resultFilters,
                CreateDate_From: (report.CreateDateFrom != '' ? report.CreateDateFrom : null),
                CreateDate_To: (report.CreateDateFrom != '' ? report.CreateDateTo : null),
                UpdateDate_From: (report.CreateDateFrom != '' ? report.UpdateDateFrom : null),
                UpdateDate_To: (report.CreateDateFrom != '' ? report.UpdateDateTo : null)
            });

            $scope.displayDetails($scope.Reports.length - 1, function () {
                if (report.saveReport) {
                    $scope.saveReport($scope.Reports.length - 1);
                }
            });
        }, function () {
        });
    }

    $scope.saveReport = function (index) {
        var report = $scope.Reports[index];
        if (report.Id == undefined) {
            UserMessagesProvider.displayLoading();
            CompetitiveAnalysisDataProvider.createComparison(report).then(function (data) {
                if (data.data.result == 'true') {
                    report.Id = data.data.obj.Id;
                    CompetitiveAnalysisDataProvider.updateComparisonProducts(report.Id, report.Products).then(function (data) {
                        UserMessagesProvider.hideLoading();
                        if (data.data.result == 'true') {
                            $scope.Reports[index] = data.data.obj;
                            $scope.Reports[index].Saved = true;
                            $scope.currentReport = jQuery.extend(true, {}, $scope.Reports[index]);
                            $scope.displayDetails(index);
                            UserMessagesProvider.successHandler();
                        } else {
                            UserMessagesProvider.errorHandler(999, returnValue.message);
                        }
                    }, function (data, status, headers, config) {
                        UserMessagesProvider.hideLoading();
                        UserMessagesProvider.errorHandler(status);
                    });
                } else {
                    UserMessagesProvider.errorHandler(999, data.message);
                }
            }, function (data, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        } else {
            UserMessagesProvider.displayLoading();
            CompetitiveAnalysisDataProvider.updateComparison(report).then(function (data) {
                if (data.data.result == 'true') {
                    CompetitiveAnalysisDataProvider.updateComparisonProducts(report.Id, report.Products).then(function (data) {
                        UserMessagesProvider.hideLoading();
                        if (data.data.result == 'true') {
                            $scope.Reports[index] = data.data.obj;
                            $scope.Reports[index].Saved = true;
                            $scope.currentReport = jQuery.extend(true, {}, $scope.Reports[index]);
                            $scope.displayDetails(index);
                            UserMessagesProvider.successHandler('Report has been saved successfully');
                        } else {
                            UserMessagesProvider.errorHandler(999, returnValue.message);
                        }
                    }, function (data, status, headers, config) {
                        UserMessagesProvider.hideLoading();
                        UserMessagesProvider.errorHandler(status);
                    });
                } else {
                    UserMessagesProvider.errorHandler(999, returnValue.message);
                }
            }, function (data, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        }
    }

    $scope.deleteReport = function (index) {
        var report = $scope.Reports[index];
        if (report.Id == undefined) {
            $scope.Reports.splice(index, 1);
            return;
        }
        UserMessagesProvider.confirmHandler("Are you sure you want to remove this report?", function () {
            UserMessagesProvider.displayLoading();
            CompetitiveAnalysisDataProvider.deleteComparison(report).then(function (data) {
                UserMessagesProvider.hideLoading();
                if (data.data.result == 'true') {
                    $scope.Reports.splice(index, 1);
                    $scope.productsCols = [];
                    $scope.rows = [];
                    UserMessagesProvider.successHandler();
                } else {
                    UserMessagesProvider.errorHandler(999, returnValue.message);
                }
            }, function (data, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        }, null);
    }

    $scope.updateReport = function (index) {
        if (index != $scope.selectedIndex) {
            $scope.displayDetails(index, displayUpdateWindows);
        } else {
            displayUpdateWindows();
        }
    }

    function displayUpdateWindows() {
        var modalInstance = $modal.open({
            templateUrl: 'SelectProduct.html',
            controller: selectProductController,
            size: 'lg',
            resolve: {
                Templates: function () {
                    return $scope.Templates;
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
                newReport: function () {
                    return false;
                },
                BrandFactoryTypes: function () { return $scope.BrandFactoryTypes; },
                report: function () {
                    return $scope.currentReport;
                }
            }
        });
        modalInstance.result.then(function (report) {
            $scope.products = [];
            var resultFilters = [];
            if (report.andFilters != undefined && report.andFilters.length > 0) {
                for (i = 0; i < report.andFilters.length; i++) {
                    resultFilters.push({
                        ConditionType: 'And',
                        PropertyId: report.andFilters[i].PropertyId,
                        Keywords: report.andFilters[i].Values,
                        FromDate: report.andFilters[i].MinDate,
                        ToDate: report.andFilters[i].MaxDate,
                        FromValue: report.andFilters[i].MinValue,
                        ToValue: report.andFilters[i].MaxValue,
                        BoolValue: report.andFilters[i].BoolValue
                    });
                }
            }
            if (report.orFilters != undefined && report.orFilters.length > 0) {
                for (i = 0; i < report.orFilters.length; i++) {
                    resultFilters.push({
                        ConditionType: 'And',
                        PropertyId: report.orFilters[i].PropertyId,
                        Keywords: report.orFilters[i].Values,
                        FromDate: report.orFilters[i].MinDate,
                        ToDate: report.orFilters[i].MaxDate,
                        FromValue: report.orFilters[i].MinValue,
                        ToValue: report.orFilters[i].MaxValue,
                        BoolValue: report.orFilters[i].BoolValue
                    });
                }
            }
            var obj;
            for (i = 0; i < report.objs.length; i++) {
                obj = report.objs[i];
                if (obj && ($scope.products.length <= 0 || $scope.products[0].TemplateId == obj.TemplateId)) {
                    var exists = false;
                    for (j = 0; j < $scope.products.length; j++) {
                        if ($scope.products[j].Id == obj.Id) {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists) {
                        $scope.products.push(obj);
                    }
                } else {
                    if (obj) {
                        UserMessagesProvider.errorHandler(999, 'You just can add products from the same template you have added before for the comparision');
                        break;
                    }
                }
            }
            $scope.Reports[$scope.selectedIndex] = {
                Id: report.reportId,
                Name: report.reportName,
                Products: $scope.products,
                Saved: false,
                Tags: report.tags,
                BrandFactoryTypes: report.brandFactoryTypes,
                Filters: resultFilters,
                CreateDate_From: report.CreateDateFrom,
                CreateDate_To: report.CreateDateTo,
                UpdateDate_From: report.UpdateDateFrom,
                UpdateDate_To: report.UpdateDateTo
            };
            $scope.Reports[$scope.selectedIndex].Saved = false;

            $scope.displayDetails($scope.selectedIndex);
            if (report.saveReport) {
                $scope.saveReport($scope.selectedIndex);
            }

        }, function () {
        });
    }

    $scope.printComparision = function () {
        var printContents = document.getElementById('comparisionGrid').innerHTML;
        var headContents = $('head').html();
        var popupWin = window.open('', '_blank', 'width=800,height=400');
        popupWin.document.open()
        popupWin.document.write('<html><head>' + headContents + '</head><body onload="window.print()">' + printContents + '</html>');
        popupWin.document.close();
    }

    $scope.showImage = function (value) {
        UserMessagesProvider.imageDisplayer(value);
    }

    initialize();
});
var selectProductController = function ($filter, $scope, $modalInstance, $modal, BrandFactoryTypes, Templates, PropertiesGroups, DataProvider, UMP, newReport, report) {
    var ct;
    $scope.templates = Templates;
    $scope.propertiesGroups = PropertiesGroups;
    $scope.Selected = { SelectedTemplate: null, SelectedTemplateId: null, BrandFactoryTypes: '', Tags: null, CreateDateFrom: null, CreateDateTo: null, UpdateDateFrom: null, UpdateDateTo: null };
    $scope.Filters = [];
    $scope.OrFilters = [];
    $scope.Products = [];
    $scope.ProductsLoaded = false;
    $scope.Report = { ReportName: '' };
    $scope.SelectedProducts = [];
    $scope.ProductsToCompare = { arr: [] };
    $scope.LoadingProducts = false;
    $scope.Tags = [];
    $scope.BrandFactoryTypes = BrandFactoryTypes;
    $scope.newReport = newReport;
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
                for (i = 0; i < $scope.Products.length; i++) {
                    $scope.ProductsToCompare.arr.push($scope.Products[i]);
                }
            } else {
                $scope.ProductsToCompare.arr = [];
            }
        });
        //Initialize report info
        if (report != undefined) {
            $scope.Report.ReportName = report.Name;
            if (report.Template != null) {
                $scope.Selected.SelectedTemplateId = report.Template.Id;
                for (i = 0; i < $scope.templates.length; i++) {
                    if ($scope.templates[i].Id == $scope.Selected.SelectedTemplateId) {
                        $scope.Selected.SelectedTemplate = $scope.templates[i];
                        break;
                    }
                }
                initializePropertiesGroups();
            }
            if (report.Tags != undefined) {
                $scope.Selected.Tags = report.Tags;
            }
            if (report.BrandFactoryTypes != undefined) {
                $scope.Selected.BrandFactoryTypes = report.BrandFactoryTypes;
            }
            if (report.CreateDate_From != undefined) {
                $scope.Selected.CreateDateFrom = report.CreateDate_From;
            }
            if (report.CreateDate_To != undefined) {
                $scope.Selected.CreateDateTo = report.CreateDate_To;
            }
            if (report.UpdateDate_From != undefined) {
                $scope.Selected.UpdateDateFrom = report.UpdateDate_From;
            }
            if (report.UpdateDate_To != undefined) {
                $scope.Selected.UpdateDateTo = report.UpdateDate_To;
            }
            if (report.Filters != undefined && report.Filters.length > 0) {
                for (i = 0; i < report.Filters.length; i++) {
                    var filter;
                    switch (report.Filters[i].Property.Type) {
                        case 'String':
                            filter = { Text: report.Filters[i].Property.Name + ' equal one of the following values: ' + report.Filters[i].Keywords, PropertyId: report.Filters[i].Property.Id, Type: report.Filters[i].Property.Type, Values: report.Filters[i].Keywords, ProductsLoaded: $scope.ProductsLoaded };
                            break;
                        case 'String - Multiple Lines':
                            filter = { Text: report.Filters[i].Property.Name + ' equal one of the following values: ' + report.Filters[i].Keywords, PropertyId: report.Filters[i].Property.Id, Type: report.Filters[i].Property.Type, Values: report.Filters[i].Keywords, ProductsLoaded: $scope.ProductsLoaded };
                            break;
                        case 'Date':
                            filter = { Text: report.Filters[i].Property.Name + ' from ' + report.Filters[i].FromDate + ' to ' + report.Filters[i].ToDate, PropertyId: report.Filters[i].Property.Id, Type: report.Filters[i].Property.Type, Values: [report.Filters[i].FromDate, report.Filters[i].ToDate], MinDate: report.Filters[i].FromDate, MaxDate: report.Filters[i].ToDate, ProductsLoaded: $scope.ProductsLoaded };
                            break;
                        case 'Predefined List':
                        case 'Predefined List - Checkboxes':
                        case 'Predefined List - Radio Buttons':
                            filter = { Text: report.Filters[i].Property.Name + ' equal one of the following values: ' + report.Filters[i].Keywords, PropertyId: report.Filters[i].Property.Id, Type: report.Filters[i].Property.Type, Values: report.Filters[i].Keywords, ProductsLoaded: $scope.ProductsLoaded };
                            break;
                        case 'Boolean':
                            filter = { Text: report.Filters[i].Property.Name + ' is ' + report.Filters[i].BoolValue, PropertyId: report.Filters[i].Property.Id, Type: report.Filters[i].Property.Type, Values: [report.Filters[i].BoolValue], BoolValue: report.Filters[i].BoolValue, ProductsLoaded: $scope.ProductsLoaded };
                            break;
                        case 'Number':
                            filter = { Text: report.Filters[i].Property.Name + ' between ' + report.Filters[i].FromValue + ' and ' + report.Filters[i].ToValue, PropertyId: report.Filters[i].Property.Id, Type: report.Filters[i].Property.Type, Values: [report.Filters[i].FromValue, report.Filters[i].ToValue], MinValue: report.Filters[i].FromValue, MaxValue: report.Filters[i].ToValue, ProductsLoaded: $scope.ProductsLoaded };
                            break;
                    }
                    if (report.Filters[i].ConditionType == 'And') {
                        $scope.Filters.push(filter);
                    } else if (report.Filters[i].ConditionType == 'Or') {
                        $scope.OrFilters.push(filter);
                    }
                }
            }
            setTimeout(function () {
                $scope.loadProducts();
                if (report.Products != undefined) {
                    for (i = 0; i < report.Products.length; i++) {
                        $scope.ProductsToCompare.arr.push(report.Products[i]);
                    }
                }
            });

        }
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

    function resetFilterBoxes() {
        $scope.Selected.SelectedGroup = null;
        $scope.Selected.SelectedProperty = null;
        $scope.FilterValues.Values = '';
        $scope.FilterNumericalRange = { min: '', max: '' };
        $scope.FilterDateRange = { from: '', to: '' };
    }

    $scope.addFilter = function () {
        if ($scope.Selected.SelectedTemplate != undefined) {
            var modalInstance = $modal.open({
                templateUrl: 'addFilter.html',
                controller: selectProductControllerFiltersController,
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
                controller: selectProductControllerFiltersController,
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
            if ($scope.Selected.BrandFactoryTypes != null && $scope.Selected.BrandFactoryTypes != '') {
                brandTypes = $scope.Selected.BrandFactoryTypes.split(',');
            }
            if ($scope.Selected.Tags != null && $scope.Selected.Tags.split(',') != '') {
                tags = $scope.Selected.Tags.split(',');
            }
            if ($scope.Selected.CreateDateFrom != null && $scope.Selected.CreateDateTo != null && $scope.Selected.CreateDateFrom != '' && $scope.Selected.CreateDateTo != '') {
                createDateRange = [$scope.Selected.CreateDateFrom, $scope.Selected.CreateDateTo];
            }
            if ($scope.Selected.UpdateDateFrom != null && $scope.Selected.UpdateDateTo != null && $scope.Selected.UpdateDateFrom != '' && $scope.Selected.UpdateDateTo != '') {
                updateDateRange = [$scope.Selected.UpdateDateFrom, $scope.Selected.UpdateDateTo];
            }
            DataProvider.getTemplateProductsByFilters($scope.Selected.SelectedTemplate.Id, brandTypes, tags, $scope.Filters, $scope.OrFilters, createDateRange, updateDateRange).then(function (data) {
                for (i = 0; i < data.length; i++) {
                    $scope.Products.push(data[i]);
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
        //var ownProducts = false;
        //for (i = 0; i < $scope.ProductsToCompare.arr.length; i++) {
        //    if ($scope.ProductsToCompare.arr[i].BrandFactoryType == "Own") {
        //        ownProducts = true;
        //        break;
        //    }
        //}
        if ($scope.ProductsToCompare.arr.length > 0) {
            //if (ownProducts || !newReport) {
            if (newReport && ($scope.Report == undefined || $scope.Report.ReportName == '')) {
                UMP.errorHandler(999, "Please choose a name for your report");
            } else {
                $modalInstance.close(
                    {
                        reportId: (report == undefined ? undefined : report.Id),
                        reportName: $scope.Report.ReportName,
                        objs: $scope.ProductsToCompare.arr,
                        saveReport: saveReport,
                        tags: $scope.Selected.Tags,
                        brandFactoryTypes: $scope.Selected.BrandFactoryTypes,
                        andFilters: $scope.Filters,
                        orFilters: $scope.OrFilters,
                        CreateDateFrom: $scope.Selected.CreateDateFrom,
                        CreateDateTo: $scope.Selected.CreateDateTo,
                        UpdateDateFrom: $scope.Selected.UpdateDateFrom,
                        UpdateDateTo: $scope.Selected.UpdateDateTo
                    });
            }
            //}
            //else
            //UMP.errorHandler(999, "You must choose one or more from own products to do comparison");
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
var selectProductControllerFiltersController = function ($filter, $scope, $modalInstance, SelectedTemplate, UMP, ProductsLoaded) {
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
angular.module('DConfig').filterProvider.register('productPropertiesFilter', [function () {
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
                                            if (fromDate <= productDate || toDate >= productDate) {
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
                                            if (min <= value || max >= value) {
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
angular.module('DConfig').filterProvider.register('propertiesNamesFilter', [function () {
    return function (rows) {
        if (!angular.isUndefined(rows)) {
            var tempRows = [];
            var valid = true;
            var lastGroupRow;
            var currentGroup;
            angular.forEach(rows, function (row) {
                if (row.show) {
                    if (row['GroupId'] != currentGroup) {
                        currentGroup = row.GroupId;
                        row['Counter'] = 1;
                        lastGroupRow = row;
                    } else {
                        lastGroupRow['Counter']++;
                        row['Counter'] = undefined;
                    }
                    tempRows.push(row);
                }
                else {
                    row['Counter'] = undefined;
                }
            });
            return tempRows;
        } else {
            return rows;
        }
    };
}]);
