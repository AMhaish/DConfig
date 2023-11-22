angular.module('DConfig').provide.factory('GalleryDataProvider', ['$http', function config($http) {
    var dataFactory = {};
    dataFactory.getFoldersTree = function (rootPath) {
        return $http.get('/DConfig/IOServicesAPI/getFoldersTree?RootPath=' + (rootPath!=null?rootPath:''));
    };
    dataFactory.createFolder = function (path) {
        return $http.post('/DConfig/IOServicesAPI/createFolder', { Path: path });
    };
    dataFactory.updateFolder = function (path,newPath) {
        return $http.put('/DConfig/IOServicesAPI/updateFolder', { Path: path, NewPath: newPath });
    };
    dataFactory.deleteFolder = function (path) {
        return $http.delete('/DConfig/IOServicesAPI/deleteFolder?Path=' + path);
    };
    dataFactory.getFiles = function (path) {
        return $http.get('/DConfig/IOServicesAPI/getFiles?Path=' + path);
    };
    dataFactory.createFile = function (path) {
        return $http.post('/DConfig/IOServicesAPI/createFile', { Path: path });
    };
    dataFactory.updateFile = function (path, newPath) {
        return $http.put('/DConfig/IOServicesAPI/updateFile', { Path: path, NewPath: newPath });
    };
    dataFactory.deleteFile = function (path) {
        return $http.delete('/DConfig/IOServicesAPI/deleteFile?Path=' + path);
    };
    return dataFactory;
}]);



//@ sourceURL=FormsManagerScripts.js