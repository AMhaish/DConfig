angular.module('DConfigSharedLib')
        .factory('EventsProvider', function () {

            var eventsProvider = {};

            var onRoutesChangeStartHandlers = [];
            eventsProvider.AddHandlerToOnRouteChangeStart = function (handler) {
                onRoutesChangeStartHandlers.push(handler);
            };
            eventsProvider.ExecuteOnRouteChangeStartHandlers = function () {
                var result = true;
                for (i = 0; i < onRoutesChangeStartHandlers.length; i++) {
                    var handler = onRoutesChangeStartHandlers.pop();
                    if (handler) {
                        if (!handler()) {
                            result = false;
                        }
                    }
                }
                return result;
            }

            var onRoutesChangeSuccessHandlers = [];
            eventsProvider.AddHandlerToOnRouteChangeSuccess = function (handler) {
                onRoutesChangeSuccessHandlers.push(handler);
            };
            eventsProvider.ExecuteOnRouteChangeSuccessHandlers = function () {
                for (i = 0; i < onRoutesChangeSuccessHandlers.length; i++) {
                    var handler = onRoutesChangeSuccessHandlers.pop();
                    if (handler) {
                        handler();
                    }
                }
            }

            return eventsProvider;
        });