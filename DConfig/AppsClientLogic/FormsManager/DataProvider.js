angular.module('DConfig').provide.factory('FormsManagerDataProvider', ['$http', function config($http) {
            var dataFactory = {};
            dataFactory.getFormsTree = function () {
                return $http.get('/DConfig/FormsAPI/getFormsTree');
            };
            dataFactory.getRootForms = function () {
                return $http.get('/DConfig/FormsAPI/getRootForms');
            };
            dataFactory.createForm = function (form) {
                return $http.post('/DConfig/FormsAPI/createForm', form);
            };
            dataFactory.updateForm = function (form) {
                return $http.put('/DConfig/FormsAPI/updateForm', form);
            };
            dataFactory.deleteForm = function (form) {
                return $http.delete('/DConfig/FormsAPI/deleteForm?Id=' + form.Id + "&Name=" + form.Name);
            };
            dataFactory.updateFormFields = function (form) {
                return $http.put('/DConfig/FormsAPI/updateFormFields', form);
            };
            dataFactory.deleteFormField = function (fieldId) {
                return $http.delete('/DConfig/FormsAPI/deleteFormField?id=' + fieldId);
            };
            dataFactory.getFormTypes = function () {
                return $http.get('/DConfig/FormsAPI/getFormTypes');
            };
            dataFactory.getFormFieldTypes = function () {
                return $http.get('/DConfig/FormsAPI/getFormFieldTypes');
            };

            dataFactory.getFormsFieldsEnumsTree = function () {
                return $http.get('/DConfig/FormsAPI/getFormsFieldsEnumsTree');
            };
            dataFactory.getFormsFieldsEnums = function () {
                return $http.get('/DConfig/FormsAPI/getFormsFieldsEnums');
            };
            dataFactory.createFormsFieldsEnum = function (enumObj) {
                return $http.post('/DConfig/FormsAPI/createFormsFieldsEnum', enumObj);
            };
            dataFactory.updateFormsFieldsEnum = function (enumObj) {
                return $http.put('/DConfig/FormsAPI/updateFormsFieldsEnum', enumObj);
            };
            dataFactory.deleteFormsFieldsEnum = function (enumObj) {
                return $http.delete('/DConfig/FormsAPI/deleteFormsFieldsEnum?Id=' + enumObj.Id + "&Name=" + enumObj.Name);
            };
            dataFactory.updateFormsFieldsEnumValues = function (id,values) {
                return $http.put('/DConfig/FormsAPI/updateFormsFieldsEnumValues?Id=' + id, values);
            };
            dataFactory.deleteFormsFieldsEnumValue = function (id) {
                return $http.delete('/DConfig/FormsAPI/deleteFormsFieldsEnumValue?Id=' + id);
            };

            dataFactory.getRootFormsTree = function () {
                return $http.get('/DConfig/FormsAPI/getRootFormsTree/');
            };
            dataFactory.getFormInstances = function (id) {
                return $http.get('/DConfig/FormsAPI/getFormInstances/' + id);
            };
            dataFactory.updateFormInstance = function (formInstance,values) {
                return $http.put('/DConfig/FormsAPI/updateFormInstance?Id=' + formInstance.Id, values);
            };
            dataFactory.deleteFormInstance = function (formInstance) {
                return $http.delete('/DConfig/FormsAPI/deleteFormInstance?Id=' + formInstance.Id);
            };

            dataFactory.getForgetFormSubmitEventsTree = function () {
                return $http.get('/DConfig/FormsAPI/getFormSubmitEventsTree');
            };
            dataFactory.getFormSubmitEventsTypes = function () {
                return $http.get('/DConfig/FormsAPI/getFormSubmitEventsTypes');
            };
            dataFactory.createFormSubmitEmailEvent = function (event) {
                return $http.post('/DConfig/FormsAPI/createFormSubmitEmailEvent', event);
            };
            dataFactory.updateFormSubmitEmailEvent = function (event) {
                return $http.put('/DConfig/FormsAPI/updateFormSubmitEmailEvent', event);
            };
            dataFactory.deleteFormSubmitEmailEvent = function (event) {
                return $http.delete('/DConfig/FormsAPI/deleteFormSubmitEmailEvent?Id=' + event.Id + "&Name=" + event.Name);
            };
            dataFactory.getTemplates = function () {
                return $http.get('/DConfig/WebsiteContentAPI/GetTemplates');
            };

            dataFactory.printForm = function (id) {
                window.open('/DConfig/FormsAPI/PrintForm/' + id, '_blank');
            };
            return dataFactory;
}]);



//@ sourceURL=FormsManagerScripts.js