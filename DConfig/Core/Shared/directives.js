angular.module('DConfigSharedLib')
    .directive('checklistCsvModel', ['$parse', '$compile', function ($parse, $compile) {
        // contains
        function arrayContains(arr, item) {
            if (arr != undefined && arr != null && (typeof arr === "string" || arr instanceof String)) {
                if (arr == item) {
                    return true;
                } else if (arr.startWith(item + ',')) {
                    return true;
                } else if (arr.endsWith(',' + item)) {
                    return true;
                } else if (arr.contains(',' + item + ',')) {
                    return true;
                } else {
                    return false;
                }
            }
        }

        // add
        function add(arr, item) {
            if (arr == undefined && arr == null && !(typeof arr === "string" || arr instanceof String)) {
                arr = '';
            }
            if (arr == '') {
                arr = item;
            } else {
                if (arrayContains(arr, item)) {
                    return arr;
                } else {
                    arr += ',' + item;
                }
            }
            return arr;

        }

        // remove
        function remove(arr, item) {
            if (arr != undefined) {
                if (arr.contains(',' + item)) {
                    return arr.replace(',' + item, '');
                } else if (arr.contains(item + ',')) {
                    return arr.replace(item + ',', '');
                } else if (arr == item) {
                    return arr.replace(item, '');
                }
                return arr;
            }
        }

        // http://stackoverflow.com/a/19228302/1458162
        function postLinkFn(scope, elem, attrs) {
            // compile with `ng-model` pointing to `checked`
            $compile(elem)(scope);

            // getter / setter for original model
            var getter = $parse(attrs.checklistCsvModel);
            var setter = getter.assign;

            // value added to list
            var value = $parse(attrs.checklistValue)(scope.$parent);
            // watch UI checked change
            scope.$watch('checked', function (newValue, oldValue) {
                if (newValue === oldValue) {
                    return;
                }
                var current = getter(scope.$parent);
                if (newValue === true) {
                    setter(scope.$parent, add(current, value));
                } else {
                    setter(scope.$parent, remove(current, value));
                }
            });

            // watch original model change
            scope.$parent.$watch(attrs.checklistCsvModel, function (newArr, oldArr) {
                scope.checked = arrayContains(newArr, value);
            }, true);
        }

        return {
            restrict: 'A',
            priority: 1000,
            terminal: true,
            scope: true,
            compile: function (tElement, tAttrs) {
                if (tElement[0].tagName !== 'INPUT' || !tElement.attr('type', 'checkbox')) {
                    throw 'checklist-csv-model should be applied to `input[type="checkbox"]`.';
                }

                if (!tAttrs.checklistValue) {
                    throw 'You should provide `checklist-value`.';
                }

                // exclude recursion
                tElement.removeAttr('checklist-csv-model');

                // local scope var storing individual checkbox model
                tElement.attr('ng-model', 'checked');

                return postLinkFn;
            }
        };
    }])
    .directive('checklistModel', ['$parse', '$compile', function ($parse, $compile) {
        // contains
        function contains(arr, item) {
            if (angular.isArray(arr)) {
                for (var i = 0; i < arr.length; i++) {
                    if (angular.equals(arr[i], item)) {
                        return true;
                    }
                }
            }
            return false;
        }

        // add
        function add(arr, item) {
            arr = angular.isArray(arr) ? arr : [];
            for (var i = 0; i < arr.length; i++) {
                if (angular.equals(arr[i], item)) {
                    return arr;
                }
            }
            arr.push(item);
            return arr;
        }

        // remove
        function remove(arr, item) {
            if (angular.isArray(arr)) {
                for (var i = 0; i < arr.length; i++) {
                    if (angular.equals(arr[i], item)) {
                        arr.splice(i, 1);
                        break;
                    }
                }
            }
            return arr;
        }

        // http://stackoverflow.com/a/19228302/1458162
        function postLinkFn(scope, elem, attrs) {
            // compile with `ng-model` pointing to `checked`
            $compile(elem)(scope);

            // getter / setter for original model
            var getter = $parse(attrs.checklistModel);
            var setter = getter.assign;

            // value added to list
            var value = $parse(attrs.checklistValue)(scope.$parent);

            // watch UI checked change
            scope.$watch('checked', function (newValue, oldValue) {
                if (newValue === oldValue) {
                    return;
                }
                var current = getter(scope.$parent);
                if (newValue === true) {
                    setter(scope.$parent, add(current, value));
                } else {
                    setter(scope.$parent, remove(current, value));
                }
            });

            // watch original model change
            scope.$parent.$watch(attrs.checklistModel, function (newArr, oldArr) {
                scope.checked = contains(newArr, value);
            }, true);
        }

        return {
            restrict: 'A',
            priority: 1000,
            terminal: true,
            scope: true,
            compile: function (tElement, tAttrs) {
                if (tElement[0].tagName !== 'INPUT' || !tElement.attr('type', 'checkbox')) {
                    throw 'checklist-model should be applied to `input[type="checkbox"]`.';
                }

                if (!tAttrs.checklistValue) {
                    throw 'You should provide `checklist-value`.';
                }

                // exclude recursion
                tElement.removeAttr('checklist-model');

                // local scope var storing individual checkbox model
                tElement.attr('ng-model', 'checked');

                return postLinkFn;
            }
        };
    }])
    .directive('datePicker', ['$filter', function ($filter) {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, element, attrs, ngModel) {
                $(element).datepicker({
                    changeMonth: true,
                    changeYear: true,
                    dateFormat: 'mm/dd/yy',
                    altFormat: 'mm/dd/yy',
                    isRTL: true,
                    onSelect: function (date) {
                        if (attrs.ngModel.contains('$parent')) {
                            ngModel.$setViewValue($filter('date')(date, 'MM/dd/yyyy'));
                            scope.$apply();
                            element.val($filter('date')(date, 'MM/dd/yyyy'));
                        }
                        else {
                            ngModel.$setViewValue($filter('date')(date, 'MM/dd/yyyy'));
                            scope.$apply();
                            element.val($filter('date')(date, 'MM/dd/yyyy'));
                        }

                    }
                });
                scope.$watch(attrs.ngModel, function (newValue) {
                    element.val($filter('date')(newValue, 'MM/dd/yyyy'));
                });
                //element.bind('change', function () {
                //    scope.$apply(function () {
                //        ngModel.$setViewValue($filter('date')(element.val(), 'MM/dd/yyyy'));
                //    });
                //});
            }
        };
    }])
    .directive('spinner', ['$filter', function ($filter) {
        return {
            restrict: 'A',
            scope: {
                step: '@',
                min: '@',
                max: '@'
            },
            require: 'ngModel',
            link: function (scope, element, attrs, ngModel) {
                $(element).spinner({ step: scope.step, max: scope.max, min: scope.min });
                scope.$watch(attrs.ngModel, function (newValue) {
                    element.val(newValue);
                });
            }
        };
    }])
    .directive('tagsinput', ['$filter', function ($filter) {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, element, attrs, ngModel) {
                $(element).tagsinput({ trimValue: true, typeaheadjs: scope.$eval(attrs.typeheadjs) });
                scope.$watch(attrs.ngModel, function (value) {
                    $(element).tagsinput('removeAll');
                    if (value != null) {
                        var arr = value.split(',');
                        for (var i = 0; i < arr.length; i++) {
                            $(element).tagsinput('add', arr[i]);
                        }
                    }
                });
            }
        };
    }])
    .directive('passwordMatch', [function () {
        return {
            restrict: 'A',
            scope: true,
            require: 'ngModel',
            link: function (scope, elem, attrs, control) {
                var checker = function () {

                    //get the value of the first password
                    var e1 = scope.$eval(attrs.ngModel);

                    //get the value of the other password  
                    var e2 = scope.$eval(attrs.passwordMatch);
                    return e1 == e2;
                };
                scope.$watch(checker, function (n) {

                    //set the form control to valid if both 
                    //passwords are the same, else invalid
                    control.$setValidity("match", n);
                });
            }
        };
    }])
    .directive('timeMask', [function () {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, elem, attrs, control) {
                $(elem).inputmask('h:s', { placeholder: 'hh:mm' });
            }
        };
    }])
    .directive('uploader', ['UserMessagesProvider', function (UserMessagesProvider) {
        return {
            restrict: 'A',
            scope: {
                action: '@',
                destinationObject: '=',
                propertyName: '@'
            },
            controller: ['$scope', function ($scope) {
                $scope.progress = 0;
                $scope.avatar = '';
                $scope.fileName = '';
                $scope.chooseFile = function () {
                    $(this).parents('.uploader').find('input[type="file"]').click();
                }
                $scope.setFileName = function (el) {
                    if (el == null) {
                        el = document.getElementById('uploader');
                    }
                    $scope.fileName = $(el).val();
                }
                $scope.sendFile = function (el) {
                    if (el == null) {
                        el = document.getElementById('uploader');
                    }
                    var $form = $(el).parents('form');
                    if ($(el).val() == '') {
                        return false;
                    }
                    $form.attr('action', $scope.action);
                    $scope.$apply(function () {
                        $scope.progress = 0;
                    });
                    $form.ajaxSubmit({
                        type: 'POST',
                        uploadProgress: function (event, position, total, percentComplete) {
                            $scope.$apply(function () {
                                // upload the progress bar during the upload
                                alert('percentComplete');
                                $scope.progress = percentComplete;
                            });
                        },
                        error: function (event, statusText, responseText, form) {
                            // remove the action attribute from the form
                            $form.removeAttr('action');
                            UserMessagesProvider.errorHandler(statusText);
                        },
                        success: function (responseText, statusText, xhr, form) {
                            //var ar = $(el).val().split('\\'),
                            //    filename = ar[ar.length - 1];
                            // remove the action attribute from the form
                            $form.removeAttr('action');
                            $scope.$apply(function () {
                                $scope.avatar = responseText;
                                $scope.destinationObject[$scope.propertyName] = responseText;

                            });
                        },
                    });
                }
            }],
            link: function (scope, elem, attrs, ctrl) {
                elem.find('.fake-uploader').click(function () {
                    elem.find('input[type="file"]').click();
                });
                function updateControl() {
                    if (scope.destinationObject != undefined && scope.propertyName != undefined) {
                        scope.avatar = scope.destinationObject[scope.propertyName];
                    }
                    if (scope.avatar == '') {
                        scope.progress = 0;
                    }
                }
                scope.$watch('destinationObject', function (oldVal, newVal) {
                    updateControl();
                });
            },
            replace: false,
            templateUrl: '/DConfig/ControllersViews/Uploader'
        };
    }])
    .directive('dropDownForm', [function () {
        return {
            restrict: 'A',
            scope: {
                open: '@',
                btnClass: '@',
                dropUp: '@'
            },
            replace: false,
            transclude: true,    // transclusion allows for the dialog 
            // contents to use angular {{}}
            template:
            '<div class="btn-group" dropdown="" is-open="isOpen" ng-transclude>' +
            '<button type="button" class="btn dropdown-toggle" dropdown-toggle="" data-ng-click="toggleDropdown($event)">' +
            '<i class="fa fa-sort"></i>' +
            '</button>' +
            '</div>',      // the transcluded content 
            //is placed in the div
            controller: ['$scope', function ($scope) {
                //$scope.toggleDropdown = function ($event) {
                //    $event.preventDefault();
                //    $event.stopPropagation();
                //    $scope.isOpen = !$scope.isOpen;
                //};
            }],
            link: function (scope, element, attrs) {
                var elem = $(element);
                elem.css('display', 'inline');
                if (scope.dropUp == 'true') {
                    elem.find('.btn-group').addClass('dropup');
                }
                elem.find('.dropdown-menu').on('click', function (e) {
                    e.stopPropagation();
                    e.preventDefault();
                });
                attrs.$observe('open', function (val) {
                    if (val == 'true') {
                        scope.isOpen = true;
                    }
                    else if (val == 'false') {
                        scope.isOpen = false;

                    }
                });
                elem.find('.dropdown-toggle').on('click', function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    scope.isOpen = !scope.isOpen;
                });
            }
        };
    }])
    .directive('richTextbox', [function () {
        var generatedIds = 0;
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, elm, attrs, ngModel) {
                var expression, options, tinyInstance;
                // generate an ID if not present
                if (!attrs.id) {
                    attrs.$set('id', 'uiTinymce' + generatedIds++);
                }
                options = {
                    // Update model when calling setContent (such as from the source editor popup)
                    setup: function (ed) {
                        ed.on('init', function (args) {
                            ngModel.$render();
                        });
                        // Update model on button click
                        ed.on('ExecCommand', function (e) {
                            ed.save();
                            ngModel.$setViewValue(elm.val());
                            if (!scope.$$phase) {
                                scope.$apply();
                            }
                        });
                        // Update model on keypress
                        ed.on('KeyUp', function (e) {
                            console.log(ed.isDirty());
                            ed.save();
                            ngModel.$setViewValue(elm.val());
                            if (!scope.$$phase) {
                                scope.$apply();
                            }
                        });
                    },
                    mode: 'exact',
                    elements: attrs.id
                };
                if (attrs.uiTinymce) {
                    expression = scope.$eval(attrs.uiTinymce);
                } else {
                    expression = {};
                }
                angular.extend(options, uiTinymceConfig, expression);
                setTimeout(function () {
                    tinymce.init(options);
                });
                ngModel.$render = function () {
                    if (!tinyInstance) {
                        tinyInstance = tinymce.get(attrs.id);
                    }
                    if (tinyInstance) {
                        tinyInstance.setContent(ngModel.$viewValue || '');
                    }
                };
            }
        };
    }])
    .directive('ckEditor', [function () {
        return {
            require: '?ngModel',
            scope: { ckType: '@' },
            link: function ($scope, elm, attr, ngModel) {
                CKEDITOR.config.pasteFromWordNumberedHeadingToList = true;
                CKEDITOR.config.pasteFromWordPromptCleanup = true;
                CKEDITOR.config.pasteFromWordRemoveFontStyles = false;
                CKEDITOR.config.pasteFromWordRemoveStyles = false;
                CKEDITOR.config.extraAllowedContent = 'td{background}';
                var ck;
                switch ($scope.ckType) {
                    case 'Simple':
                        ck = CKEDITOR.replace(elm[0], { customConfig: '/Scripts/ckeditor/simple_config.js' });
                        break;
                    default:
                        ck = CKEDITOR.replace(elm[0]);
                        break;
                }
                function updateModel() {
                    $scope.$apply(function () {
                        ngModel.$setViewValue(ck.getData());
                    });
                }

                ck.on('change', updateModel);
                ck.on('key', updateModel);
                ck.on('dataReady', updateModel);
                ck.on('pasteState', updateModel);

                ngModel.$render = function (value) {
                    ck.setData(ngModel.$modelValue);
                };
            }
        };
    }])
    //Not reviwed or used yet
    .directive('authShow', ['UserPermissionsProvider', function (UserPermissionsProvider) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                element.css('display', 'none');
                var value = attrs.authShow;
                if (value != '') {
                    var conditionExecuter = function () { element.css('display', 'block'); };
                    UserPermissionsProvider.checkAuthorityAndExecuteCommand(value, conditionExecuter);
                }
            }
        };
    }])
    .directive('authEnable', ['UserPermissionsProvider', function (UserPermissionsProvider) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                attrs.$set('disabled', 'disabled');
                var value = attrs.authEnable;
                if (value != '') {
                    var conditionExecuter = function () { attrs.$set('disabled', ''); };
                    UserPermissionsProvider.checkAuthorityAndExecuteCommand(value, conditionExecuter);
                }
            }
        };
    }])
    .directive('timePicker', ['$filter', function ($filter) {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, element, attrs, ngModel) {
                $(element).ptTimeSelect({
                    containerWidth: 400,
                    setButtonLabel: 'حفظ',
                    hoursLabel: 'الساعات',
                    minutesLabel: 'الدقائق',
                    containerClass: 'TimePickerContainer',
                    zIndex: 100000,
                    onClose: function () {
                        if (attrs.ngModel.contains('$parent')) {
                            ngModel.$setViewValue(element.val());
                            scope.$apply();
                        }
                        else {
                            ngModel.$setViewValue(element.val());
                        }
                    }
                });
                scope.$watch(attrs.ngModel, function (newValue) {
                    element.val(newValue);
                });
                //element.bind('change', function () {
                //    scope.$apply(function () {
                //        ngModel.$setViewValue(element.val());
                //    });
                //});
            }
        };
    }])
    .directive('textEditor', function () {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, element, attrs, ngModel) {
                var type = 'full';
                var args;
                switch (attrs.type) {
                    case 'full':
                    default:
                        args = {};
                    case 'mini':
                        args = { buttonList: ['fontSize', 'bold', 'italic', 'underline', 'strikeThrough'], maxHeight: 100 };
                }
                var editor = new nicEditor(args).panelInstance(attrs.id);
                editor.addEvent('blur', function () {
                    scope.$apply(function () {
                        nicEditors.findEditor(attrs.id).saveContent();
                        ngModel.$setViewValue(element.val());
                    });
                });
                scope.$watch(attrs.ngModel, function (newValue) {
                    if (newValue != undefined)
                        nicEditors.findEditor(attrs.id).setContent(newValue);

                });
            }
        };
    })
    .directive('pagination', function () {
        return {
            restrict: 'A',
            scope: {
                currentPage: '=',
                numberOfPages: '=',
                callback: '='
            },
            templateUrl: '/Utilities/PaginationView',
            replace: false,
            link: function (scope, elem, attrs, ctrl) {
            },
            controller: ['$scope', function ($scope) {
                $scope.pageClick = function (pageNum) {
                    $scope.callback(pageNum);
                };
                $scope.previousPage = function () {
                    $scope.callback($scope.currentPage - 1);
                };
                $scope.nextPage = function () {
                    $scope.callback($scope.currentPage + 1);
                };
            }],
        };
    })
    .directive('tooltip', ['$timeout', function ($timeout) {
        return {
            restrict: 'A',
            replace: false,
            link: function (scope, elem, attrs, ctrl) {
                var tooltipOptions = {
                    placement: 'bottom'
                };
                $timeout(function () {
                    $(elem).tooltip(tooltipOptions);
                }, 0);
            },
        };
    }])
    .directive('dialog', ['$timeout', 'UserMessagesProvider', 'EventsProvider', function ($timeout, UserMessagesProvider, EventsProvider) {
        return {
            scope: {
                okButton: '@',
                okCallback: '=',
                cancelButton: '@',
                cancelCallback: '=',
                open: '@',
                title: '@',
                width: '@',
                height: '@',
                show: '@',
                hide: '@',
                autoOpen: '@',
                resizable: '@',
                closeOnEscape: '@',
                hideCloseButton: '@'
            },
            replace: false,
            transclude: true,    // transclusion allows for the dialog 
            // contents to use angular {{}}
            template: '<form name="dialogForm" novalidate="novalidate" enctype="multipart/form-data"><div ng-transclude></div></form>',      // the transcluded content 
            //is placed in the div
            link: function (scope, element, attrs) {

                // Close button is hidden by default
                var hideCloseButton = attrs.hideCloseButton || true;

                // Specify the options for the dialog
                var dialogOptions = {
                    autoOpen: attrs.autoOpen || false,
                    title: attrs.title,
                    width: attrs.width || 350,
                    height: attrs.height || 200,
                    modal: attrs.modal || true,
                    show: attrs.show || 'fade',
                    hide: attrs.hide || '',
                    draggable: attrs.draggable || true,
                    resizable: attrs.resizable,
                    closeOnEscape: attrs.closeOnEscape || false,

                    close: function () {
                        console.log('closing...');
                        //console.log(scope);
                        /*
                        $timeout(function() {
                        scope.$apply(scope.cancelCallback());
                        },0);    
                        */
                    },
                    open: function (event, ui) {
                        // Hide close button 
                        if (hideCloseButton == true) {
                            $(".ui-dialog-titlebar-close", ui.dialog).hide();
                        }
                    }
                };

                // Add the buttons 
                dialogOptions['buttons'] = [];
                if (attrs.okButton) {
                    var btnOptions = {
                        text: attrs.okButton,
                        click: function () {
                            scope.$apply(scope.okCallback(scope.dialogForm));
                        }
                    };
                    dialogOptions['buttons'].push(btnOptions);
                }

                if (attrs.cancelButton) {
                    var btnOptions = {
                        text: attrs.cancelButton,
                        click: function () { scope.$apply(scope.cancelCallback()); }
                    };
                    dialogOptions['buttons'].push(btnOptions);
                }

                // Initialize the element as a dialog
                // For some reason this timeout is required, otherwise it doesn't work
                // for more than one dialog
                $timeout(function () {
                    $(element).dialog(dialogOptions);
                }, 0);

                // This works when observing an interpolated attribute
                // e.g {{dialogOpen}}.  In this case the val is always a string and so
                // must be compared with the string 'true' and not a boolean
                // using open: '@' and open="{{dialogOpen}}"
                attrs.$observe('open', function (val) {
                    if (val == 'true') {
                        $(element).dialog("open");
                    }
                    else if (val == 'false') {
                        $(element).dialog("close");

                    }
                });

                EventsProvider.AddHandlerToOnRouteChangeStart(function () {
                    $(".ui-dialog-content").dialog("close");
                });
                // This allows title to be bound
                //attrs.$observe('title', function (val) {
                //    console.log('observing title: val=' + val);
                //    $(element).dialog("option", "title", val);
                //});
            }
        }
    }])
    /*
    .directive('folder-upload', ['$timeout', function ($timeout) {
        return {
            restrict: 'A',
            replace: false,
            link: function (scope, elem, attrs, ctrl) {
                var uploadPaths = [];

                scope.dragenter = function(event) {
                    // indicates valid drop data
                    // false allows drop
                    return Array.prototype.every.call(
                        event.dataTransfer.items,
                        item => item.kind !== 'file'
                    );
                }

                scope.dragover = function(event) {
                    // indicates valid drop data
                    // false allows drop
                    return Array.prototype.every.call(
                        event.dataTransfer.items,
                        item => item.kind !== 'file'
                    );
                }

                scope.drop = function(event) {
                    const entries = Array.from(event.dataTransfer.items)
                        .filter(item => item.kind === 'file')
                        .map(item => item.webkitGetAsEntry());
                    this.buildTree(entries, '').then(tree => {
                        this.uploadPaths = [];
                        this.upload(tree, '');
                        this.cdr.markForCheck();
                    });
                    // indicates valid drop data
                    // false allows drop
                    return false;
                }

                scope.filesPicked = function(files) {
                    console.log(files);
                    this.uploadPaths = [];
                    Array.prototype.forEach.call(files, file => {
                        this.uploadPaths.push(file.webkitRelativePath);
                    });
                }

                scope.parseFileEntry = function (fileEntry) {
                    return new Promise((resolve, reject) => {
                        fileEntry.file(
                            file => {
                                resolve(file);
                            },
                            err => {
                                reject(err);
                            }
                        );
                    });
                }

                scope.parseDirectoryEntry = function (directoryEntry) {
                    const directoryReader = directoryEntry.createReader();
                    return new Promise((resolve, reject) => {
                        directoryReader.readEntries(
                            entries => {
                                resolve(this.buildTree(entries, directoryEntry.name));
                            },
                            err => {
                                reject(err);
                            }
                        );
                    });
                }

                scope.buildTree = function (entries, name) {
                    const tree = { name, files: [], directories: [] };
                    const promises = [];
                    entries.forEach(entry => {
                        if (entry.isFile) {
                            const promise = this.parseFileEntry(entry).then(file => {
                                tree.files.push(file);
                            });
                            promises.push(promise);
                        } else if (entry.isDirectory) {
                            const promise = this.parseDirectoryEntry(entry).then(directory => {
                                tree.directories.push(directory);
                            });
                            promises.push(promise);
                        });
                    return Promise.all(promises).then(() => tree);
                }

                scope.upload = function (tree, path) {
                    tree.files.forEach(file => {
                        this.uploadPaths.push(path + file.name);
                    });
                    tree.directories.forEach(directory => {
                        const newPath = path + directory.name + '/';
                        this.uploadPaths.push(newPath);
                        this.upload(directory, newPath);
                    });
                }

            },
        };
    }])
*/
    ;
