angular.module('DConfig').routeProvider.when('/ApplicationsManager', { controller: 'ApplicationsManager.InstalledAppsController', templateUrl: '/DConfig/ApplicationsManager/InstalledApps' });
angular.module('DConfig').routeProvider.when('/ApplicationsManager/InstallApp', { controller: 'ApplicationsManager.InstallAppController', templateUrl: '/DConfig/ApplicationsManager/InstallApp' });
angular.module('DConfig').routeProvider.when('/ApplicationsManager/UploadApp', { controller: 'ApplicationsManager.UploadAppController', templateUrl: '/DConfig/ApplicationsManager/UploadApp' });
