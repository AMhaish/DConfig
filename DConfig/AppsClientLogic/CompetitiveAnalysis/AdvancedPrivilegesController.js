angular.module('DConfig').controllerProvider.register('CompetitiveAnalysis.AdvancedPrivilegesController', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, CompetitiveAnalysisDataProvider, EventsProvider, $modal, scopeService) {
    $scope.DetailsPanelStates = {
        None: 0,
        PrivilegeDetails: 1
    };
    $scope.ExecutingContexts = {
        None: 0,
        Deleting: 1,
        Creating: 2,
        Renaming: 3,
        Updating: 4
    };
    $scope.Sections = [
        { Name: 'Edit Properties', Value: 'Edit Properties' },
        { Name: 'Prices', Value: 'Prices' },
        { Name: 'Share', Value: 'Share' },
    ];
    $scope.BrandFactoryTypes = [
        { Name: 'Own', Value: 'Own' },
        { Name: 'Supplier', Value: 'Supplier' },
        { Name: 'Competitor', Value: 'Competitor' },
    ];

    $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
    $scope.state = $scope.DetailsPanelStates.None;

    function initialize() {
        BreadCrumpsProvider.breadCrumps.path = ['Competitive Analysis', 'Advanced Privileges'];
        UserMessagesProvider.displayProgress(3);
        CompetitiveAnalysisDataProvider.getAdvancedPrivileges().then(function (data) {
            $scope.Privileges = data.data;
            UserMessagesProvider.increaseProgress();
        }, function (data, status, headers, config) {
            UserMessagesProvider.increaseProgress();
            UserMessagesProvider.errorHandler(status);
        });
        CompetitiveAnalysisDataProvider.getAllTemplates().then(function (data) {
            $scope.Templates = data.data;
            UserMessagesProvider.increaseProgress();
        }, function (data, status, headers, config) {
            UserMessagesProvider.increaseProgress();
            UserMessagesProvider.errorHandler(status);
        });
        CompetitiveAnalysisDataProvider.getCompanies().then(function (data) {
            $scope.Companies = data.data;
            UserMessagesProvider.increaseProgress();
        }, function (data, status, headers, config) {
            UserMessagesProvider.increaseProgress();
            UserMessagesProvider.errorHandler(status);
        });
    }

    $scope.closeDetailsForm = function () {
        $scope.state = $scope.DetailsPanelStates.None;
    }

    $scope.displayDetails = function (index) {
        if ($scope.state == $scope.DetailsPanelStates.Info && $scope.selectedIndex != undefined && index == $scope.selectedIndex) {
            $scope.selectedIndex = undefined;
            $scope.closeDetailsForm();
        } else {
            $scope.selectedIndex = index;
            $scope.currentObj = jQuery.extend(true, {}, $scope.Privileges[index]);
            $scope.state = $scope.DetailsPanelStates.PrivilegeDetails;
        }
    }

    $scope.updatePrivilege = function () {
        UserMessagesProvider.displayLoading();
        CompetitiveAnalysisDataProvider.updateAdvancedPrivilege($scope.currentObj).then(function (returnValue) {
            UserMessagesProvider.hideLoading();
            if (returnValue.data.result == 'true') {
                $scope.Privileges[index] = returnValue.obj;
                $scope.currentObj = jQuery.extend(true, {}, $scope.Privileges[index]);
                UserMessagesProvider.successHandler();
            }
            else {
                UserMessagesProvider.errorHandler(999, returnValue.message);
            }
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
        }, function (errorData, status, headers, config) {
            UserMessagesProvider.hideLoading();
            UserMessagesProvider.errorHandler(status);
        });
    }

    $scope.deletePrivilege = function (index) {
        $scope.selectedIndex = index;
        $scope.currentObj = $scope.Privileges[index];
        UserMessagesProvider.confirmHandler("Are you sure you want to delete this privilege?", function () {
            UserMessagesProvider.displayLoading();
            CompetitiveAnalysisDataProvider.deleteAdvancedPrivilege($scope.currentObj).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    $scope.closeDetailsForm();
                    $scope.Privileges.splice(index, 1);
                    UserMessagesProvider.successHandler();
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.message);
                }
            }, function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(999);
            });
        }, null);
    }

    $scope.createRootNode = function () {
        $scope.CurrentExecutingContext = $scope.ExecutingContexts.Creating;
        $scope.currentObj = {};
        var modalInstance = $modal.open({
            templateUrl: 'createForm.html',
            controller: createPrivilegeController,
            size: 'lg',
            resolve: {
                Companies: function () { return $scope.Companies; },
                BrandFactoryTypes: function () { return $scope.BrandFactoryTypes; },
                Sections: function () { return $scope.Sections; },
                Templates: function () { return $scope.Templates; },
                UMP: function () { return UserMessagesProvider; }
            }
        });
        modalInstance.result.then(function (newNode) {
            UserMessagesProvider.displayLoading();
            CompetitiveAnalysisDataProvider.createAdvancedPrivilege(newNode).then(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.data.result == 'true') {
                    $scope.Privileges.push(returnValue.obj);
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

        }, function () {
            $scope.CurrentExecutingContext = $scope.ExecutingContexts.None;
        });

    }

    initialize();
});
var createPrivilegeController = function ($scope, $modalInstance, Companies, BrandFactoryTypes, Sections, Templates, UMP) {
    $scope.newNode = {};
    $scope.createForm = {};
    $scope.companies = Companies;
    $scope.BrandFactoryTypes = BrandFactoryTypes;
    $scope.Templates = Templates;
    $scope.Sections = Sections;

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