angular.module('DConfig').provide.factory('CompetitiveAnalysisDataProvider', ['$http', function config($http) {
    var dataFactory = {};
    dataFactory.getPropertiesGroupsTree = function () {
        return $http.get('/DConfig/CompetitiveAnalysis/getPropertiesGroupsTree');
    };
    dataFactory.getPropertiesGroups = function () {
        return $http.get('/DConfig/CompetitiveAnalysis/getPropertiesGroups');
    };
    dataFactory.createPropertiesGroup = function (group) {
        return $http.post('/DConfig/CompetitiveAnalysis/createPropertiesGroup', group);
    };
    dataFactory.updatePropertiesGroup = function (group) {
        return $http.put('/DConfig/CompetitiveAnalysis/updatePropertiesGroup', group);
    };
    dataFactory.updatePropertiesGroupsOrder = function (groups) {
        return $http.put('/DConfig/CompetitiveAnalysis/updateGroupsOrdering', { Groups: groups });
    };
    dataFactory.deletePropertiesGroup = function (group) {
        return $http.delete('/DConfig/CompetitiveAnalysis/deletePropertiesGroup?Id=' + group.Id + "&Name=" + group.Name);
    };
    dataFactory.updateGroupProperties = function (group) {
        return $http.put('/DConfig/CompetitiveAnalysis/updateGroupProperties', group);
    };
    dataFactory.deleteGroupProperty = function (propertyId) {
        return $http.delete('/DConfig/CompetitiveAnalysis/deleteGroupProperty?id=' + propertyId);
    };
    dataFactory.getPropertiesTypes = function () {
        return $http.get('/DConfig/CompetitiveAnalysis/getPropertiesTypes');
    };

    dataFactory.getPropertiesEnumsTree = function () {
        return $http.get('/DConfig/CompetitiveAnalysis/getPropertyEnumsTree');
    };
    dataFactory.getPropertiesEnums = function () {
        return $http.get('/DConfig/CompetitiveAnalysis/getPropertyEnums');
    };
    dataFactory.createPropertyEnum = function (enumObj) {
        return $http.post('/DConfig/CompetitiveAnalysis/createPropertyEnum', enumObj);
    };
    dataFactory.updatePropertyEnum = function (enumObj) {
        return $http.put('/DConfig/CompetitiveAnalysis/updatePropertyEnum', enumObj);
    };
    dataFactory.deletePropertyEnum = function (enumObj) {
        return $http.delete('/DConfig/CompetitiveAnalysis/deletePropertyEnum?Id=' + enumObj.Id + "&Name=" + enumObj.Name);
    };
    dataFactory.updatePropertyEnumValues = function (id, values) {
        return $http.put('/DConfig/CompetitiveAnalysis/updatePropertyEnumValues?Id=' + id, values);
    };
    dataFactory.deletePropertyEnumValue = function (id) {
        return $http.delete('/DConfig/CompetitiveAnalysis/deletePropertyEnumValue?Id=' + id);
    };

    dataFactory.getTemplatesTree = function () {
        return $http.get('/DConfig/CompetitiveAnalysis/getTemplatesTree');
    };
    dataFactory.getTemplates = function () {
        return $http.get('/DConfig/CompetitiveAnalysis/getTemplates');
    };
    dataFactory.getAllTemplates = function () {
        return $http.get('/DConfig/CompetitiveAnalysis/getTemplates?All=true');
    };
    dataFactory.getProperties = function () {
        return $http.get('/DConfig/CompetitiveAnalysis/getProperties');
    };
    dataFactory.createProductsTemplate = function (template) {
        return $http.post('/DConfig/CompetitiveAnalysis/createProductsTemplate', template);
    };
    dataFactory.updateProductsTemplate = function (template) {
        return $http.put('/DConfig/CompetitiveAnalysis/updateProductsTemplate', template);
    };
    dataFactory.deleteProductsTemplate = function (template) {
        return $http.delete('/DConfig/CompetitiveAnalysis/deleteProductsTemplate?Id=' + template.Id + "&Name=" + template.Name);
    };
    dataFactory.updateTemplateProperties = function (templateId, properties) {
        return $http.put('/DConfig/CompetitiveAnalysis/updateTemplateProperties/' + templateId, { Properties: properties });
    };

    dataFactory.getProductsTemplatesTree = function () {
        return $http.get('/DConfig/CompetitiveAnalysis/getProductsTemplatesTree');
    };
    dataFactory.getTemplateProductsTree = function (id) {
        return $http.get('/DConfig/CompetitiveAnalysis/getTemplateProductsTree/' + id);
    };
    dataFactory.getTemplateProductsByPattern = function (id, pattern) {
        return $http.get('/DConfig/CompetitiveAnalysis/getTemplateProductsByPattern/' + id + '?Pattern=' + pattern);
    };
    dataFactory.getTemplateProductsByFilters = function (id, brandFactoryTypes, tags, andFilters, orFilters, createDateRange, updateDateRange) {
        return $http.post('/DConfig/CompetitiveAnalysis/getTemplateProductsByFilters/' + id, { AndFilters: andFilters, OrFilters: orFilters, BrandTypes: brandFactoryTypes, Tags: tags, CreateDateRange: createDateRange, UpdateDateRange: updateDateRange });
    };
    dataFactory.getProductsByIds = function (ids) {
        return $http.post('/DConfig/CompetitiveAnalysis/getProductsByIds', { Ids: ids });
    };
    dataFactory.getProduct = function (id) {
        return $http.get('/DConfig/CompetitiveAnalysis/getProduct/' + id);
    };
    dataFactory.createProduct = function (product) {
        return $http.post('/DConfig/CompetitiveAnalysis/createProduct', product);
    };
    dataFactory.updateProduct = function (product) {
        return $http.put('/DConfig/CompetitiveAnalysis/updateProduct', product);
    };
    dataFactory.deleteProduct = function (product) {
        return $http.delete('/DConfig/CompetitiveAnalysis/deleteProduct?Id=' + product.Id + "&Name=" + product.Name);
    };
    dataFactory.updateProductPropertiesValues = function (productId, properties) {
        return $http.put('/DConfig/CompetitiveAnalysis/updateProductPropertiesValues/' + productId, { Properties: properties });
    };
    dataFactory.updateProductPrices = function (obj) {
        return $http.put('/DConfig/CompetitiveAnalysis/updateProductPrices', obj);
    };
    dataFactory.deleteProductPrice = function (id) {
        return $http.delete('/DConfig/CompetitiveAnalysis/deleteProductPrice?Id=' + id);
    };
    dataFactory.getTemplate = function (id) {
        return $http.get('/DConfig/CompetitiveAnalysis/getTemplate/' + id);
    };
    dataFactory.updateProductTags = function (id, tags) {
        return $http.put('/DConfig/CompetitiveAnalysis/updateProductTags', { Id: id, Tags: tags });
    };
    dataFactory.getProductsTags = function (pattern) {
        return $http.get('/DConfig/CompetitiveAnalysis/getProductsTags?Pattern=' + pattern);
    };
    dataFactory.ImportProductsFromExcel = function (TemplateId, ExcelPath, CompanyId, BrandFactoryType) {
        return $http.post('/DConfig/CompetitiveAnalysis/importProductsFromExcel', { TemplateId: TemplateId, ExcelPath: ExcelPath, CompanyId: CompanyId, BrandFactoryType: BrandFactoryType });
    };
    dataFactory.ImportProductsImagesFromZip = function (ZipPath) {
        return $http.post('/DConfig/CompetitiveAnalysis/importProductsImagesFromZip', { ZipPath: ZipPath });
    };

    dataFactory.getComparisons = function () {
        return $http.get('/DConfig/CompetitiveAnalysis/getComparisons');
    };
    dataFactory.createComparison = function (comparison) {
        return $http.post('/DConfig/CompetitiveAnalysis/createComparison', comparison);
    };
    dataFactory.updateComparison = function (comparison) {
        return $http.put('/DConfig/CompetitiveAnalysis/updateComparison', comparison);
    };
    dataFactory.deleteComparison = function (comparison) {
        return $http.delete('/DConfig/CompetitiveAnalysis/deleteComparison?Id=' + comparison.Id + "&Name=" + comparison.Name);
    };
    dataFactory.updateComparisonProducts = function (comparisonId, products) {
        return $http.put('/DConfig/CompetitiveAnalysis/updateComparisonProducts/' + comparisonId, { Products: products });
    };

    dataFactory.getAdvancedPrivileges = function () {
        return $http.get('/DConfig/CompetitiveAnalysis/getAdvancedPrivileges');
    };
    dataFactory.createAdvancedPrivilege = function (privilege) {
        return $http.post('/DConfig/CompetitiveAnalysis/createAdvancedPrivilege', privilege);
    };
    dataFactory.updateAdvancedPrivilege = function (privilege) {
        return $http.put('/DConfig/CompetitiveAnalysis/updateAdvancedPrivilege', privilege);
    };
    dataFactory.deleteAdvancedPrivilege = function (privilege) {
        return $http.delete('/DConfig/CompetitiveAnalysis/deleteAdvancedPrivilege?Id=' + privilege.Id );
    };

    dataFactory.getCompanies = function () {
        return $http.get('/Membership/getCompanies');
    };
    return dataFactory;
}]);

//@ sourceURL=ProductssManagerScripts.js