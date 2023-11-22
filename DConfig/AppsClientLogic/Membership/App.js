angular.module('DConfig').routeProvider.when('/Membership', { controller: 'Membership.UsersListController', templateUrl: '/Membership/UsersList' });
angular.module('DConfig').routeProvider.when('/Membership/Roles', { controller: 'Membership.RolesListController', templateUrl: '/Membership/RolesList' });
angular.module('DConfig').routeProvider.when('/Membership/Companies', { controller: 'Membership.CompaniesListController', templateUrl: '/Membership/CompaniesList' });
angular.module('DConfig').routeProvider.when('/Membership/AppsPriviliges', { controller: 'Membership.AppsPriviligesController', templateUrl: '/Membership/AppsPriviliges' });
angular.module('DConfig').routeProvider.when('/Membership/Priviliges', { controller: 'Membership.PriviligesController', templateUrl: '/Membership/Priviliges' });
angular.module('DConfig').routeProvider.when('/Membership/PagesPriviliges', { controller: 'Membership.PagesPriviligesController', templateUrl: '/Membership/PagesPriviliges' });
angular.module('DConfig').routeProvider.when('/Membership/Settings', { controller: 'Membership.SettingsController', templateUrl: '/Membership/Settings' });
angular.module('DConfig').routeProvider.when('/Membership/PredefinedLists', { controller: 'Membership.PredefinedListsController', templateUrl: '/Membership/PredefinedLists' });





