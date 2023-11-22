angular.module('DConfig')
        .factory('ExplorerDataProvider', ['$http', function config($http) {
            var dataFactory = {};
            dataFactory.getApps = function () {
                return $http.get('/DConfig/AppsAPI/GetApps');
            };
            dataFactory.getDesktopWidgets = function () {
                return $http.get('/DConfig/AppsAPI/GetDesktopWidgets');
            };
            dataFactory.getGeneralSettings = function () {
                return $http.get('/DConfig/SettingsAPI/getGeneralSettings');
            };
            dataFactory.updateGeneralSettings = function (obj) {
                return $http.put('/DConfig/SettingsAPI/updateGeneralSettings',obj);
            };
            dataFactory.getEmailSettings = function () {
                return $http.get('/DConfig/SettingsAPI/getEmailSettings');
            };
            dataFactory.getSMSSettings = function () {
                return $http.get('/DConfig/SettingsAPI/getSMSSettings');
            };
            dataFactory.updateEmailSettings = function (obj) {
                return $http.put('/DConfig/SettingsAPI/updateEmailSettings', obj);
            };
            dataFactory.updateSMSSettings = function (obj) {
                return $http.put('/DConfig/SettingsAPI/updateSMSSettings', obj);
            };
            dataFactory.getIdentitySettings = function () {
                return $http.get('/DConfig/SettingsAPI/getIdentitySettings');
            };
            dataFactory.updateIdentitySettings = function (obj) {
                return $http.put('/DConfig/SettingsAPI/updateIdentitySettings', obj);
            };
            dataFactory.getAppearanceSettings = function () {
                return $http.get('/DConfig/SettingsAPI/getAppearanceSettings');
            };
            dataFactory.updateAppearanceSettings = function (obj) {
                return $http.put('/DConfig/SettingsAPI/updateAppearanceSettings', obj);
            };
            dataFactory.getBackupSettings = function () {
                return $http.get('/DConfig/SettingsAPI/getBackupSettings');
            };
            dataFactory.updateBackupSettings = function (obj) {
                return $http.put('/DConfig/SettingsAPI/updateBackupSettings', obj);
            };
            dataFactory.getDatabaseSettings = function () {
                return $http.get('/DConfig/SettingsAPI/getDatabaseSettings');
            };
            dataFactory.updateDatabaseSettings = function (obj) {
                return $http.put('/DConfig/SettingsAPI/updateDatabaseSettings', obj);
            };
            dataFactory.getDateTimeSettings = function () {
                return $http.get('/DConfig/SettingsAPI/getDateTimeSettings');
            };
            dataFactory.updateDateTimeSettings = function (obj) {
                return $http.put('/DConfig/SettingsAPI/updateDateTimeSettings', obj);
            };
            dataFactory.getLanguagesSettings = function () {
                return $http.get('/DConfig/SettingsAPI/getLanguagesSettings');
            };
            dataFactory.updateLanguagesSettings = function (obj) {
                return $http.put('/DConfig/SettingsAPI/updateLanguagesSettings', obj);
            };
            dataFactory.getSecuritySettings = function () {
                return $http.get('/DConfig/SettingsAPI/getSecuritySettings');
            };
            dataFactory.updateSecuritySettings = function (obj) {
                return $http.put('/DConfig/SettingsAPI/updateSecuritySettings', obj);
            };
            dataFactory.getNotificationsSettings = function () {
                return $http.get('/DConfig/SettingsAPI/getNotificationsSettings');
            };
            dataFactory.updateNotificationsSettings = function (obj) {
                return $http.put('/DConfig/SettingsAPI/updateNotificationsSettings', obj);
            };
            dataFactory.getSystemInfoSettings = function () {
                return $http.get('/DConfig/SettingsAPI/getSystemInfoSettings');
            };
            dataFactory.updateSystemInfoSettings = function (obj) {
                return $http.put('/DConfig/SettingsAPI/updateSystemInfoSettings', obj);
            };
            dataFactory.getSystemUpdateSettings = function () {
                return $http.get('/DConfig/SettingsAPI/getSystemUpdateSettings');
            };
            dataFactory.updateSystemUpdateSettings = function (obj) {
                return $http.put('/DConfig/SettingsAPI/updateSystemUpdateSettings', obj);
            };
            dataFactory.getSupportSettings = function () {
                return $http.get('/DConfig/SettingsAPI/getSupportSettings');
            };
            dataFactory.updateSupportSettings = function (obj) {
                return $http.put('/DConfig/SettingsAPI/updateSupportSettings', obj);
            };
            dataFactory.getShowcaseSettings = function () {
                return $http.get('/DConfig/SettingsAPI/getCustomSettings?Key=ShowcaseConfig');
            };
            dataFactory.updateShowcaseSettings = function (obj) {
                return $http.put('/DConfig/SettingsAPI/updateCustomSettings?Key=ShowcaseConfig', obj);
            };
            dataFactory.getStoreSettings = function () {
                return $http.get('/DConfig/SettingsAPI/getCustomSettings?Key=StoreConfig');
            };
            dataFactory.updateStoreSettings = function (obj) {
                return $http.put('/DConfig/SettingsAPI/updateCustomSettings?Key=StoreConfig', obj);
            };
            dataFactory.getLogsSettings = function () {
                return $http.get('/DConfig/SettingsAPI/getLogsSettings');
            };

            dataFactory.getLogFile = function (logFile) {
                return $http.get('/DConfig/SettingsAPI/getLogFile?logFile=' + logFile);
            };


            dataFactory.getUserCompanies = function () {
                return $http.get('/Membership/getUserCompanies');
            };
            dataFactory.setCurrentContext = function (id) {
                return $http.put('/Membership/setCurrentContext/' + id);
            };
            return dataFactory;
        }]);
