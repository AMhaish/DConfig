angular.module('DConfig').provide.factory('WebsiteManagerDataProvider', ['$http', function config($http) {
    var dataFactory = {};
    dataFactory.getContentsTree = function (id) {
        if (id == undefined)
            return $http.get('/DConfig/WebsiteContentAPI/GetContentsTree');
        else
            return $http.get('/DConfig/WebsiteContentAPI/GetContentsTree/' + id);
    };
    dataFactory.getQuickContentList = function (id) {
        return $http.get('/DConfig/WebsiteContentAPI/getquickcontentslist');
    };
    dataFactory.getContentsListsTree = function (id) {
        if (id == undefined)
            return $http.get('/DConfig/WebsiteContentAPI/getcontentslisttreeaction');
        else
            return $http.get('/DConfig/WebsiteContentAPI/getcontentslisttreeaction/' + id);
    };
    dataFactory.getContentChildren = function (id, limit, skip, keyword) {
        return $http.get('/DConfig/WebsiteContentAPI/GetContentChildren/' + id + '?limit=' + limit + '&skip=' + skip + '&keyword=' + keyword);
    };
    dataFactory.getContentChildrenCount = function (id, keyword) {
        return $http.get('/DConfig/WebsiteContentAPI/GetContentChildrenCount/' + id + '?keyword=' + keyword);
    };
    dataFactory.getContentInstances = function (id) {
        return $http.get('/DConfig/WebsiteContentAPI/GetContentInstances/' + id);
    };
    dataFactory.getcontentinstancesForPrevVersions = function (id) {
        return $http.get('/DConfig/WebsiteContentAPI/GetContentInstancesForPrevVersions/' + id);
    };
    dataFactory.getcontentinstanceFieldsValues = function (id) {
        return $http.get('/DConfig/WebsiteContentAPI/getcontentinstanceFieldsValues/' + id);
    };
    dataFactory.createContentNode = function (content, isContentVersion) {
        if (isContentVersion)
            return $http.post('/DConfig/WebsiteContentAPI/CreateContentInstance', content);
        else
            return $http.post('/DConfig/WebsiteContentAPI/CreateContent', content);
    };
    dataFactory.cloneContentNode = function (id, isContentVersion,suffix) {
        if (isContentVersion)
            return $http.post('/DConfig/WebsiteContentAPI/cloneContentInstance/' + id, { Suffix: suffix});
        else
            return $http.post('/DConfig/WebsiteContentAPI/cloneContent/' + id, { Suffix: suffix });
    };
    dataFactory.updateContentNode = function (content, typeOfNode) {
        if (typeOfNode == "ContentVersion")
            return $http.put('/DConfig/WebsiteContentAPI/UpdateContentInstance', content);
        else
            return $http.put('/DConfig/WebsiteContentAPI/UpdateContent', content);
    };
    dataFactory.updateContentNodeParent = function (relationInfo, typeOfNode) {
        if (typeOfNode == "ContentVersion")
            return $http.put('/DConfig/WebsiteContentAPI/UpdateContentInstanceParent', relationInfo);
        else
            return $http.put('/DConfig/WebsiteContentAPI/UpdateContentparent', relationInfo);
    };
    dataFactory.deleteContentNode = function (content, typeOfNode) {
        if (typeOfNode == "ContentVersion")
            return $http.delete('/DConfig/WebsiteContentAPI/DeleteContentInstance?Id=' + content.Id + "&Name=" + content.Name);
        else
            return $http.delete('/DConfig/WebsiteContentAPI/DeleteContent?Id=' + content.Id + "&Name=" + content.Name);
    };
    dataFactory.updateContentInstanceFieldValues = function (contentInstance) {
        return $http.put('/DConfig/WebsiteContentAPI/UpdateContentInstanceFieldValues', contentInstance);
    };
    dataFactory.updateContentsOrdering = function (contents) {
        return $http.put('/DConfig/WebsiteContentAPI/updateContentsOrdering', { Contents: contents });
    };
    dataFactory.updateScriptsOrdering = function (bundleId, scripts) {
        return $http.put('/DConfig/WebsiteContentAPI/updateScriptsOrdering', { BundleId: bundleId, Scripts: scripts });
    };
    dataFactory.updateStylesOrdering = function (bundleId, styles) {
        return $http.put('/DConfig/WebsiteContentAPI/updateStylesOrdering', { BundleId: bundleId, Styles: styles });
    };

    dataFactory.addDomainToContent = function (content, domain) {
        return $http.put('/DConfig/WebsiteContentAPI/addDomainToContent/' + content.Id + '?Domain=' + domain, {});
    };
    dataFactory.removeDomainFromContent = function (content, domain) {
        return $http.delete('/DConfig/WebsiteContentAPI/removeDomainFromContent/' + content.Id + '?Domain=' + domain);
    };

    dataFactory.getTemplatesTree = function () {
        return $http.get('/DConfig/WebsiteContentAPI/GetTemplatesTree');
    };
    dataFactory.getTemplates = function () {
        return $http.get('/DConfig/WebsiteContentAPI/GetTemplates');
    };
    dataFactory.createTemplate = function (template) {
        return $http.post('/DConfig/WebsiteContentAPI/CreateTemplate', template);
    };
    dataFactory.createTemplateClone = function (id, suffix) {
        return $http.post('/DConfig/WebsiteContentAPI/createTemplateClone', {Id:id,Suffix:suffix});
    };
    dataFactory.updateTemplate = function (template) {
        return $http.put('/DConfig/WebsiteContentAPI/UpdateTemplate', template);
    };
    dataFactory.updateTemplateLayout = function (relationInfo) {
        return $http.put('/DConfig/WebsiteContentAPI/UpdateTemplateLayout', relationInfo);
    };
    dataFactory.deleteTemplate = function (template) {
        return $http.delete('/DConfig/WebsiteContentAPI/DeleteTemplate?Id=' + template.Id + "&Name=" + template.Name + "&Path=" + template.Path);
    };
    dataFactory.getViewTypes = function (templateContextId) {
        return $http.get('/DConfig/WebsiteContentAPI/getViewTypes?templateContextId=' + (templateContextId ? templateContextId : ''));
    };
    dataFactory.getRootTemplates = function () {
        return $http.get('/DConfig/WebsiteContentAPI/getRootTemplates');
    };
    dataFactory.getRootViewTypes = function (templateContextId) {
        return $http.get('/DConfig/WebsiteContentAPI/getRootViewTypes?templateContextId=' + (templateContextId ? templateContextId:'') );
    };
    dataFactory.getTemplateView = function (template) {
        return $http.get('/DConfig/WebsiteContentAPI/getTemplateView?id=' + template.Id);
    };
    dataFactory.updateTemplateView = function (template, viewObj) {
        return $http.put('/DConfig/WebsiteContentAPI/updateTemplateView?id=' + template.Id, viewObj);
    };
    dataFactory.getTemplateViewTypeFields = function (template) {
        return $http.get('/DConfig/WebsiteContentAPI/getTemplateViewTypeFields?Id=' + template.Id);
    };
    dataFactory.loadShowcaseConfig = function () {
        return $http.get('/DConfig/SettingsAPI/getCustomSettings?Key=ShowcaseConfig');
    };
    dataFactory.getScriptsTree = function () {
        return $http.get('/DConfig/WebsiteContentAPI/getScriptsTree');
    };
    dataFactory.getScripts = function () {
        return $http.get('/DConfig/WebsiteContentAPI/getScripts');
    };
    dataFactory.createScript = function (Script) {
        return $http.post('/DConfig/WebsiteContentAPI/createScript', Script);
    };
    dataFactory.updateScript = function (Script) {
        return $http.put('/DConfig/WebsiteContentAPI/updateScript', Script);
    };
    dataFactory.deleteScript = function (Script) {
        return $http.delete('/DConfig/WebsiteContentAPI/deleteScript?Id=' + Script.Id + "&Name=" + Script.Name + "&Path=" + Script.Path);
    };
    dataFactory.getScriptCode = function (Script) {
        return $http.get('/DConfig/WebsiteContentAPI/getScriptCode?id=' + Script.Id);
    };
    dataFactory.updateScriptCode = function (Script, viewObj) {
        return $http.put('/DConfig/WebsiteContentAPI/updateScriptCode?id=' + Script.Id, viewObj);
    };
    dataFactory.createScriptsBundle = function (ScriptBundle) {
        return $http.post('/DConfig/WebsiteContentAPI/createScriptsBundle', ScriptBundle);
    };
    dataFactory.updateScriptsBundle = function (ScriptBundle) {
        return $http.put('/DConfig/WebsiteContentAPI/updateScriptsBundle', ScriptBundle);
    };
    dataFactory.deleteScriptsBundle = function (ScriptBundle) {
        return $http.delete('/DConfig/WebsiteContentAPI/deleteScriptsBundle?Id=' + ScriptBundle.Id + "&Name=" + ScriptBundle.Name);
    };
    dataFactory.getScriptsBundles = function () {
        return $http.get('/DConfig/WebsiteContentAPI/getScriptsBundles');
    };

    dataFactory.getStylesTree = function () {
        return $http.get('/DConfig/WebsiteContentAPI/getStylesTree');
    };
    dataFactory.getStyles = function () {
        return $http.get('/DConfig/WebsiteContentAPI/getStyles');
    };
    dataFactory.createStyle = function (Style) {
        return $http.post('/DConfig/WebsiteContentAPI/createStyle', Style);
    };
    dataFactory.updateStyle = function (Style) {
        return $http.put('/DConfig/WebsiteContentAPI/updateStyle', Style);
    };
    dataFactory.deleteStyle = function (Style) {
        return $http.delete('/DConfig/WebsiteContentAPI/deleteStyle?Id=' + Style.Id + "&Name=" + Style.Name + "&Path=" + Style.Path);
    };
    dataFactory.getStyleCode = function (Style) {
        return $http.get('/DConfig/WebsiteContentAPI/getStyleCode?id=' + Style.Id);
    };
    dataFactory.updateStyleCode = function (Style, viewObj) {
        return $http.put('/DConfig/WebsiteContentAPI/updateStyleCode?id=' + Style.Id, viewObj);
    };
    dataFactory.createStylesBundle = function (StyleBundle) {
        return $http.post('/DConfig/WebsiteContentAPI/createStylesBundle', StyleBundle);
    };
    dataFactory.updateStylesBundle = function (StyleBundle) {
        return $http.put('/DConfig/WebsiteContentAPI/updateStylesBundle', StyleBundle);
    };
    dataFactory.deleteStylesBundle = function (StyleBundle) {
        return $http.delete('/DConfig/WebsiteContentAPI/deleteStylesBundle?Id=' + StyleBundle.Id + "&Name=" + StyleBundle.Name);
    };
    dataFactory.getStylesBundles = function () {
        return $http.get('/DConfig/WebsiteContentAPI/getStylesBundles');
    };

    dataFactory.getViewTypesTree = function () {
        return $http.get('/DConfig/WebsiteContentAPI/getViewTypesTree');
    };
    dataFactory.createViewType = function (type) {
        return $http.post('/DConfig/WebsiteContentAPI/createViewType', type);
    };
    dataFactory.updateViewType = function (type) {
        return $http.put('/DConfig/WebsiteContentAPI/updateViewType', type);
    };
    dataFactory.deleteViewType = function (type) {
        return $http.delete('/DConfig/WebsiteContentAPI/deleteViewType?Id=' + type.Id + "&Name=" + type.Name);
    };
    dataFactory.updateViewTypeFields = function (type) {
        return $http.put('/DConfig/WebsiteContentAPI/updateViewTypeFields', type);
    };
    dataFactory.deleteViewTypeField = function (fieldId) {
        return $http.delete('/DConfig/WebsiteContentAPI/DeleteViewTypeField?id=' + fieldId);
    };
    dataFactory.getViewFieldTypes = function () {
        return $http.get('/DConfig/WebsiteContentAPI/getViewFieldTypes');
    };
    dataFactory.getViewTypeChildren = function (viewType) {
        return $http.get('/DConfig/WebsiteContentAPI/getViewTypeChildren?Id=' + viewType.Id);
    };
    dataFactory.updateViewTypeChildren = function (viewType, ids) {
        return $http.put('/DConfig/WebsiteContentAPI/updateViewTypeChildren?Id=' + viewType.Id, ids);
    };

    dataFactory.getForms = function () {
        return $http.get('/DConfig/FormsAPI/getForms');
    };

    dataFactory.getViewFieldsEnumsTree = function () {
        return $http.get('/DConfig/WebsiteContentAPI/getViewFieldsEnumsTree');
    };
    dataFactory.getViewFieldsEnums = function () {
        return $http.get('/DConfig/WebsiteContentAPI/getViewFieldsEnums');
    };
    dataFactory.createViewFieldsEnum = function (enumObj) {
        return $http.post('/DConfig/WebsiteContentAPI/createViewFieldsEnum', enumObj);
    };
    dataFactory.updateViewFieldsEnum = function (enumObj) {
        return $http.put('/DConfig/WebsiteContentAPI/updateViewFieldsEnum', enumObj);
    };
    dataFactory.deleteViewFieldsEnum = function (enumObj) {
        return $http.delete('/DConfig/WebsiteContentAPI/deleteViewFieldsEnum?Id=' + enumObj.Id + "&Name=" + enumObj.Name);
    };
    dataFactory.updateViewFieldsEnumValues = function (id, values) {
        return $http.put('/DConfig/WebsiteContentAPI/updateViewFieldsEnumValues?Id=' + id, values);
    };
    dataFactory.deleteViewFieldsEnumValue = function (id) {
        return $http.delete('/DConfig/WebsiteContentAPI/deleteViewFieldsEnumValue?Id=' + id);
    };

    dataFactory.getStages = function () {
        return $http.get('/DConfig/WebsiteContentAPI/getStages');
    };
    dataFactory.getStageRoles = function (stageId) {
        return $http.get('/DConfig/WebsiteContentAPI/getStageRoles?Id=' + stageId);
    };
    dataFactory.getStagesbyRole = function () {
        return $http.get('/DConfig/WebsiteContentAPI/getStagesbyRole');
    };
    dataFactory.createStage = function (stage) {
        return $http.post('/DConfig/WebsiteContentAPI/createStage', stage);
    };
    dataFactory.updateStage = function (stage) {
        return $http.put('/DConfig/WebsiteContentAPI/updateStage', stage);
    };
    dataFactory.deleteStage = function (stage) {
        return $http.delete('/DConfig/WebsiteContentAPI/deleteStage?Id=' + stage.Id + "&Name=" + stage.Name);
    };  
    dataFactory.getNextStages = function () {
        return $http.get('/DConfig/WebsiteContentAPI/getNextStages');
    };
    dataFactory.updateNextStages = function (stage, ids) {
        return $http.put('/DConfig/WebsiteContentAPI/updateNextStages?Id=' + stage.Id, {ids: ids });
    };
    dataFactory.stagingContentInstance = function (contentInstanceId, stageId, comments) {
        return $http.put('/DConfig/WebsiteContentAPI/stagingContentInstance?Id=' + stageId, { ContentId: contentInstanceId, Comments: comments });
    };
    dataFactory.getRoles = function () {
        return $http.get('/Membership/getroles');
    };

    dataFactory.addStageToRole = function (stage, role) {
        return $http.put('/DConfig/WebsiteContentAPI/addStageToRole?Id=' + stage.Id, { RoleId: role.Id });
    };
    dataFactory.removeStageFromRole = function (stage, roleName) {
        return $http.delete('/DConfig/WebsiteContentAPI/removeStageFromRole?Id=' + stage.Id + "&RoleName=" + roleName );
    };

    dataFactory.ImportContentsFromExcel = function (ContentId, ExcelPath, ViewTypeId) {
        return $http.post('/DConfig/WebsiteContentAPI/importContentsFromExcel', { ContentId: ContentId, ExcelPath: ExcelPath, ViewTypeId: ViewTypeId});
    };

    dataFactory.ExportTemplateAsExcel = function (ContentId) {
        return $http.get('/DConfig/WebsiteContentAPI/exportTemplateExcel?ContentId=' + ContentId);
    };

    return dataFactory;
}]);


//@ sourceURL=WebsiteManagerScripts.js