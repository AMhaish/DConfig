appRoot.controllerProvider.register('Membership.CompaniesListController', function ($scope, $location, BreadCrumpsProvider, UserMessagesProvider, MembershipDataProvider, EventsProvider, $modal, filterFilter) {
    $scope.DetailsPanelStates = {
        None: 0,
        Edit: 1,
        Create: 2,
        Info: 3
    };
    function initialize() {
        BreadCrumpsProvider.breadCrumps.path = ['Membership Manager', 'Companies'];
        $scope.closeDetailsForm();
        UserMessagesProvider.displayLoading();
        MembershipDataProvider.getCompanies().success(function (data) {
            $scope.companies = data;
            UserMessagesProvider.hideLoading();
        })
        .error(function (data, status, headers, config) {
            UserMessagesProvider.hideLoading();
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
            $scope.currentCompany = jQuery.extend(true, {}, filterFilter($scope.companies, $scope.searchText)[index]);
            $scope.state = $scope.DetailsPanelStates.Info;
        }
    }

    $scope.displayEditForm = function (index) {
        if ($scope.state == $scope.DetailsPanelStates.Edit && $scope.selectedIndex != undefined && index == $scope.selectedIndex) {
            $scope.closeDetailsForm();
            $scope.selectedIndex = undefined;
        } else {
            $scope.selectedIndex = index;
            $scope.currentCompany = jQuery.extend(true, {}, filterFilter($scope.companies, $scope.searchText)[index]);
            $scope.state = $scope.DetailsPanelStates.Edit;
        }
    }

    $scope.displayCreateForm = function () {
        $scope.closeDetailsForm();
        if ($scope.state != $scope.DetailsPanelStates.Create) {
            $scope.currentCompany = {};
            $scope.state = $scope.DetailsPanelStates.Create;
        }
    }

    $scope.deleteCompany = function (index) {
        $scope.selectedIndex = index;
        $scope.currentCompany = filterFilter($scope.companies, $scope.searchText)[index];
        UserMessagesProvider.confirmHandler("Are you sure you want to delete this company?", function () {
            UserMessagesProvider.displayLoading();
            MembershipDataProvider.deleteCompany($scope.currentCompany).success(function (returnValue) {
                UserMessagesProvider.hideLoading();
                if (returnValue.result == 'true') {
                    $scope.closeDetailsForm();
                    $scope.companies.splice(index, 1);
                }
                else {
                    UserMessagesProvider.errorHandler(999, returnValue.message);
                }
            })
            .error(function (errorData, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(999);
            });
        }, null);
    }

    $scope.createCompany = function () {
        if ($scope.createForm.$valid) {
            UserMessagesProvider.displayLoading();
            MembershipDataProvider.createCompany($scope.currentCompany).success(function (data) {
                if (data.result == 'true') {
                    $scope.currentCompany.Id = data.obj.Id;
                    $scope.companies.push($scope.currentCompany);
                    $scope.closeDetailsForm();
                }
                else {
                    UserMessagesProvider.errorHandler(999, data.message);
                }
                UserMessagesProvider.hideLoading();
            })
            .error(function (data, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        } else {
            UserMessagesProvider.invalidHandler();
        }
    }

    $scope.editCompany = function () {
        if ($scope.editForm.$valid) {
            UserMessagesProvider.displayLoading();
            MembershipDataProvider.updateCompany($scope.currentCompany).success(function (data) {
                if (data.result == 'true') {
                    $scope.companies[$scope.selectedIndex] = $scope.currentCompany;
                    $scope.closeDetailsForm();
                } else {
                    UserMessagesProvider.errorHandler(999, data.message);
                }
                UserMessagesProvider.hideLoading();
            })
            .error(function (data, status, headers, config) {
                UserMessagesProvider.hideLoading();
                UserMessagesProvider.errorHandler(status);
            });
        } else {
            UserMessagesProvider.invalidHandler();
        }
    }

    initialize();
});
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