angular.module('DConfig').controllerProvider.register('Intents.ProductsBrowser', function ($scope, $modalInstance, parameters, BreadCrumpsProvider, UserMessagesProvider, CompetitiveAnalysisDataProvider, EventsProvider, $modal, IntentsProvider, scopeService, $upload) {
    var ct;
    $scope.Selected = { SelectedTemplate: null };
    $scope.LoadingProducts = false;

    $scope.initialize = function () {
        UserMessagesProvider.displayLoading();
        CompetitiveAnalysisDataProvider.getTemplates().then(function (data) {
            $scope.templates = data;
            CompetitiveAnalysisDataProvider.getPropertiesGroups().then(function (data) {
                $scope.propertiesGroups = data;
                UserMessagesProvider.hideLoading();
            }, function (data, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        }, function (data, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });
    }

    $scope.$watch('Selected.SelectedTemplate', function (newValue, oldValue) {
        // Ignore initial setup.
        if (newValue === oldValue) {
            return;
        }
        initializeProductsTree();
    });

    function initializeProductsTree() {
        $scope.Products = [];
        UserMessagesProvider.addLoadingToWindow('productsTree_productSelector');
        $scope.LoadingProducts = true;
        CompetitiveAnalysisDataProvider.getTemplateProductsTree($scope.Selected.SelectedTemplate.Id).then(function (data) {
            for (i = 0; i < data.length; i++) {
                $scope.Products.push(data[i].obj);
            }
            UserMessagesProvider.removeLoadingFromWindow('productsTree_productSelector');
            $scope.LoadingProducts = false;
        }, function (data, status, headers, config) {
            UserMessagesProvider.removeLoadingFromWindow('productsTree_productSelector');
            UserMessagesProvider.errorHandler(status);
            $scope.LoadingProducts = false;
        });
    }

    $scope.ok = function (Id) {
        $modalInstance.close('CompetitiveAnalysis;RenderProductDetails;' + Id);
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
});
