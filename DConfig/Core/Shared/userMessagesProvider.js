angular.module('DConfigSharedLib')
    .factory('UserMessagesProvider', ['$modal', function ($modal, $scope) {
        var mProvider = {};
        mProvider.progressStep = 0;
        mProvider.progressCurrentValue = 0;

        toastr.options.closeButton = true;
        toastr.options.progressBar = false;
        toastr.options.debug = false;
        toastr.options.positionClass = 'toast-top-full-width';
        toastr.options.showDuration = 330;
        toastr.options.hideDuration = 330;
        toastr.options.timeOut = 5000;
        toastr.options.extendedTimeOut = 1000;
        toastr.options.showEasing = 'swing';
        toastr.options.hideEasing = 'swing';
        toastr.options.showMethod = 'slideDown';
        toastr.options.hideMethod = 'slideUp';

        //999 for Custom Errors
        //900 series for Account errors
        //800 series for Explorer errors
        mProvider.errorHandler = function (errorCode, error) {
            mProvider.hideLoading();
            var result;
            switch (errorCode) {
                case 999:
                    if (error != null)
                        result = error;
                    else
                        result = 'Invalid inputs, please recheck.';
                    break;
                case 990:
                    result = 'Invalid inputs, please recheck.';
                    break;
                case 800:
                    result = 'error loading the application scripts path.';
                    break;
                case 801:
                    result = 'error loading the application scripts.';
                    break;
                case 802:
                    result = 'error parsing the application scripts.';
                    break;
                case 803:
                    result = 'error loading the application extention scripts path.';
                    break;
                case 804:
                    result = 'error fetching the application extention scripts.';
                    break;
                case 805:
                    result = 'error parsing the application extention scripts.';
                    break;
                default:
                    result = 'Sorry, maybe your internet connection went off or there is a problem in your connection.';
                    //return alert(text);
                    //$.pnotify_onlyOne({
                    //    title: 'يرجى المعذرة، لم يتم استقبال طلبك، لايوجد اتصال بالانترنت أو قد يكون هنالك مشكلة في الاتصال بالانترنت',
                    //    text: '',
                    //    type: 'error',
                    //    history: false,
                    //    cornerclass: 'ui-pnotify-sharp',
                    //    sticker: false
                    //});
                    break;
            }
            toastr.error(result, '');
            //var modalInstance = $modal.open({
            //    templateUrl: 'Message.html',
            //    controller: messageController,
            //    size: 'sm',
            //    resolve: {
            //        message: function () { return result; },
            //        header: function () { return "Error" }
            //    }
            //});
        }
        mProvider.notificationHandler = function (message) {
            //var modalInstance = $modal.open({
            //    templateUrl: 'Message.html',
            //    controller: messageController,
            //    size: 'sm',
            //    resolve: {
            //        message: function () { return message; },
            //        header: function () { return "Notification" }
            //    }
            //});
            toastr.info(message, '');
        }
        mProvider.invalidHandler = function (text) {
            mProvider.hideLoading();
            var result;
            if (text != null) {
                result = text;

            } else {
                result = 'Sorry, one or more inputs are invalid';
                //$.pnotify_onlyOne({
                //    title: 'يوجد حقل أو أكثر غير محقق لشروطه',
                //    text: '',
                //    type: 'error',
                //    history: false,
                //    cornerclass: 'ui-pnotify-sharp',
                //    sticker: false
                //});
            }
            toastr.info(result, '');
            //var modalInstance = $modal.open({
            //    templateUrl: 'Message.html',
            //    controller: messageController,
            //    size: 'sm',
            //    resolve: {
            //        message: function () { return result; },
            //        header: function () { return "Invalid inputs" }
            //    }
            //});
        }
        mProvider.confirmHandler = function (text, okHandler, cancelHandler) {
            var modalInstance = $modal.open({
                templateUrl: 'DeleteConfirmation.html',
                controller: confirmMessageController,
                size: 'sm',
                resolve: {
                    message: function () { return text; }
                }
            });
            modalInstance.result.then(function () {
                okHandler();
            }, function () {
                cancelHandler();
            });
        }
        mProvider.successHandler = function (text) {
            mProvider.hideLoading();
            if (text != null) {
                toastr.then(text, '');
            } else {
                toastr.then('Saved successfully.', '');
            }
        }
        mProvider.displayLoading = function () {
            $('#loading').show();
        }
        mProvider.hideLoading = function () {
            $('#loading').hide();
        }
        mProvider.addLoadingToWindow = function (windowId) {
            var loadingPnl = $('#' + windowId);
            var loading = loadingPnl.find('.smallLoading');
            if (loading == undefined) {
                loadingPnl.append('<div class="smallLoading"><span></span></div>');
            }
        }
        mProvider.removeLoadingFromWindow = function (windowId) {
            $('#' + windowId).find('.smallLoading').remove();
        }
        mProvider.displayProgress = function (numberOfSteps) {
            $('#progress').show();
            mProvider.progressStep = Math.ceil(100 / numberOfSteps);
            $scope._progressValue = 0;
        }
        mProvider.increaseProgress = function () {
            if ($scope._progressValue + mProvider.progressStep < 100) {
                $scope._progressValue += mProvider.progressStep;
            } else {
                $scope._progressValue = 100;
                $('#progress').hide();
            }
        }
        mProvider.imageDisplayer = function (value) {
            var modalInstance = $modal.open({
                templateUrl: 'ImageDisplayer.html',
                controller: imageDisplayerController,
                size: 'lg',
                resolve: {
                    value: function () { return value; }
                }
            });
            modalInstance.result.then(function () {
            }, function () {
            });
        }

        var confirmMessageController = function ($scope, $modalInstance, message) {
            $scope.Message = message;
            $scope.ok = function () {
                $modalInstance.close();
            };
            $scope.cancel = function () {
                $modalInstance.dismiss('cancel');
            };
        };
        var imageDisplayerController = function ($scope, $modalInstance, value) {
            $scope.Value = value;
            $scope.close = function () {
                $modalInstance.dismiss('cancel');
            };
        };
        var messageController = function ($scope, $modalInstance, header, message) {
            $scope.Message = message;
            $scope.Header = header;
            $scope.ok = function () {
                $modalInstance.close();
            };
        };
        return mProvider;
    }]);