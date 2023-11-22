angular.module('DConfig').controllerProvider.register('Intents.ReportsBrowser', function ($scope, $modalInstance, parameters, BreadCrumpsProvider, UserMessagesProvider, CompetitiveAnalysisDataProvider, EventsProvider, $modal, IntentsProvider, scopeService, $upload) {

    $scope.initialize = function () {
        CompetitiveAnalysisDataProvider.getComparisons().then(function (data) {
            $scope.reports = data;
            UserMessagesProvider.hideLoading();
        }, function (data, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });
    }

    $scope.ok = function (Id) {
        $modalInstance.close('CompetitiveAnalysis;RenderComparison;' + Id);
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
});
