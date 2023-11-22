angular.module('DConfig').controller('SettingsController', ['$scope', '$location', '$modal', 'BreadCrumpsProvider', 'UserMessagesProvider', 'ExplorerDataProvider', 'EventsProvider', function ($scope, $location, $modal, BreadCrumpsProvider, UserMessagesProvider, ExplorerDataProvider, EventsProvider) {
        $scope.openSettingsPanel = function (settingsType) {
            var modalInstance = $modal.open({
                templateUrl: settingsType + '.html',
                controller: settingsTypeController,
                size: 'lg',
                resolve: {
                    dataProvider: function () {
                        return ExplorerDataProvider;
                    },
                    UMP: function () { return UserMessagesProvider; },
                    settingsType: function () { return settingsType; }
                }
            });
        }

        BreadCrumpsProvider.breadCrumps.path = ['Settings'];
    }]);

var settingsTypeController = function ($scope, $modalInstance, UMP, dataProvider, settingsType) {
    $scope.settings = {};
    $scope.settings.logFile = "";
    $scope.settings.Logs = [];
    $scope.settingsForm = {};
    $scope.initialize = function () {
        InitializeSettings(settingsType, dataProvider, UMP, function (obj) {
            $scope.settings = obj;
            if ($scope.settings.Logs != null) {
                $scope.settings.logFile = $scope.settings.Logs[0];
                $scope.ViewLogFile($scope.settings.logFile);
            }
        });
    }
    $scope.ok = function () {
        if ($scope.settingsForm.form.$valid) {
            ProcessSettings($scope.settings, settingsType, dataProvider, UMP, function () {
                $modalInstance.close();
            });
        } else {
            UMP.invalidHandler();
        }
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.ViewLogFile = function (logFile) {
        dataProvider.getLogFile(logFile).then(function (returnValue) {
            UMP.hideLoading();
            //result = returnValue.replace("\\r\\n", "<br/>");
            //result = returnValue.replace(/\r\n/g, "<br />").replace(/\n/g, "<br />");
            $scope.settings.ViewedLogFile = returnValue.data;
        }, function (errorData, status, headers, config) {
            UMP.hideLoading();
            UMP.errorHandler(status);
        });
    }
};
function ProcessSettings(settingsObj, settingsType, dataProvider, UMP, successHandler) {
    UMP.displayLoading();
    switch (settingsType) {
        case 'General':
            dataProvider.updateGeneralSettings(settingsObj).then(function (returnValue) {
                UMP.hideLoading();
                if (returnValue.data.result == "true")
                    successHandler(returnValue.data);
                else
                    UMP.errorHandler(999, returnValue.message);
            }, function (errorData, status, headers, config) {
                UMP.hideLoading();
                UMP.errorHandler(status);
            });
            return;
        case 'Identity':
            return;
        case 'Appearance':
            return;
        case 'Email':
            dataProvider.updateEmailSettings(settingsObj).then(function (returnValue) {
                UMP.hideLoading();
                if (returnValue.data.result == "true")
                    successHandler(returnValue.data);
                else
                    UMP.errorHandler(999, returnValue.message);
            }, function (errorData, status, headers, config) {
                UMP.hideLoading();
                UMP.errorHandler(status);
            });
            return;
        case 'SMS':
            dataProvider.updateSMSSettings(settingsObj).then(function (returnValue) {
                UMP.hideLoading();
                if (returnValue.data.result == "true")
                    successHandler(returnValue.data);
                else
                    UMP.errorHandler(999, returnValue.message);
            }, function (errorData, status, headers, config) {
                UMP.hideLoading();
                UMP.errorHandler(status);
            });
            return;
        case 'Backup':
            return;
        case 'Database':
            return;
        case 'DateTime':
            return;
        case 'Languages':
            dataProvider.updateLanguagesSettings(settingsObj).then(function (returnValue) {
                UMP.hideLoading();
                if (returnValue.data.result == "true")
                    successHandler(returnValue.data);
                else
                    UMP.errorHandler(999, returnValue.message);
            }, function (errorData, status, headers, config) {
                UMP.hideLoading();
                UMP.errorHandler(status);
            });
            return;
        case 'Security':
            dataProvider.updateSecuritySettings(settingsObj).then(function (returnValue) {
                UMP.hideLoading();
                if (returnValue.data.result == "true")
                    successHandler(returnValue.data);
                else
                    UMP.errorHandler(999, returnValue.message);
            }, function (errorData, status, headers, config) {
                UMP.hideLoading();
                UMP.errorHandler(status);
            });
            return;
        case 'Logs':
            UMP.hideLoading();
            return;
        case 'Notifications':
            return;
        case 'SystemInfo':
            return;
        case 'SystemUpdate':
            return;
        case 'Support':
            return;
        case 'Showcase':
            dataProvider.updateShowcaseSettings(settingsObj).then(function (returnValue) {
                UMP.hideLoading();
                if (returnValue.data.result == "true")
                    successHandler();
                else
                    UMP.errorHandler(999, returnValue.message);
            }, function (errorData, status, headers, config) {
                UMP.hideLoading();
                UMP.errorHandler(status);
            });
            return;
        case 'Store':
            dataProvider.updateStoreSettings(settingsObj).then(function (returnValue) {
                UMP.hideLoading();
                if (returnValue.data.result == "true")
                    successHandler();
                else
                    UMP.errorHandler(999, returnValue.message);
            }, function (errorData, status, headers, config) {
                UMP.hideLoading();
                UMP.errorHandler(status);
            });
            return;
    }
}
function InitializeSettings(settingsType, dataProvider, UMP, successHandler) {
    UMP.displayLoading();
    switch (settingsType) {
        case 'General':
            dataProvider.getGeneralSettings().then(function (returnValue) {
                UMP.hideLoading();
                successHandler(returnValue.data);
            }, function (errorData, status, headers, config) {
                UMP.hideLoading();
                UMP.errorHandler(status);
            });
            return;
        case 'Identity':
            return;
        case 'Appearance':
            return;
        case 'Email':
            dataProvider.getEmailSettings().then(function (returnValue) {
                UMP.hideLoading();
                successHandler(returnValue.data);
            }, function (errorData, status, headers, config) {
                UMP.hideLoading();
                UMP.errorHandler(status);
            });
            return;
        case 'Email':
            dataProvider.getSMSSettings().then(function (returnValue) {
                UMP.hideLoading();
                successHandler(returnValue.data);
            }, function (errorData, status, headers, config) {
                UMP.hideLoading();
                UMP.errorHandler(status);
            });
            return;
        case 'Backup':
            return;
        case 'Database':
            return;
        case 'DateTime':
            return;
        case 'Languages':
            dataProvider.getLanguagesSettings().then(function (returnValue) {
                UMP.hideLoading();
                returnValue.data.Languages = [
                    { Name: 'English', Value: 'EN' },
                    { Name: 'Arabic', Value: 'AR' },
                    { Name: 'Turkish', Value: 'TR' },
                    { Name: 'French', Value: 'FR' },
                    { Name: 'German', Value: 'DE' },
                    { Name: 'Spanish', Value: 'ES' }
                ];
                console.log(returnValue);
                successHandler(returnValue.data);
            }, function (errorData, status, headers, config) {
                UMP.hideLoading();
                UMP.errorHandler(status);
            });
            return;
        case 'Security':
            dataProvider.getSecuritySettings().then(function (returnValue) {
                UMP.hideLoading();
                successHandler(returnValue.data);
            }, function (errorData, status, headers, config) {
                UMP.hideLoading();
                UMP.errorHandler(status);
            });
            return;
        case 'Logs':
            dataProvider.getLogsSettings().then(function (returnValue) {
                UMP.hideLoading();
                successHandler(returnValue.data);
            }, function (errorData, status, headers, config) {
                UMP.hideLoading();
                UMP.errorHandler(status);
            });
            return;
        case 'Notifications':
            return;
        case 'SystemInfo':
            return;
        case 'SystemUpdate':
            return;
        case 'Support':
            return;
        case 'Showcase':
            dataProvider.getShowcaseSettings().then(function (returnValue) {
                UMP.hideLoading();
                successHandler({ Value: returnValue.data });
            }, function (errorData, status, headers, config) {
                UMP.hideLoading();
                UMP.errorHandler(status);
            });
            return;
        case 'Store':
            dataProvider.getStoreSettings().then(function (returnValue) {
                UMP.hideLoading();
                successHandler({ Value: returnValue.data });
            }, function (errorData, status, headers, config) {
                UMP.hideLoading();
                UMP.errorHandler(status);
            });
            return;
    }
}