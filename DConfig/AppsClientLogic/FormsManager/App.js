angular.module('DConfig').routeProvider.when('/FormsManager', { controller: 'FormsManager.PostedFormsController', templateUrl: '/DConfig/FormsManager/PostedForms' });
angular.module('DConfig').routeProvider.when('/FormsManager/FormsDefinitions', { controller: 'FormsManager.FormsTreeController', templateUrl: '/DConfig/FormsManager/FormsTree' });
angular.module('DConfig').routeProvider.when('/FormsManager/FormsSubmitEvents', { controller: 'FormsManager.FormsSubmitEventsController', templateUrl: '/DConfig/FormsManager/FormsSubmitEvents' });
angular.module('DConfig').routeProvider.when('/FormsManager/PredefinedLists', { controller: 'FormsManager.PredefinedListsController', templateUrl: '/DConfig/FormsManager/PredefinedLists' });






