angular.module('virtoCommerce.modulesPublishing')
.factory('virtoCommerce.modulesPublishing.resource', ['$resource', function ($resource) {
    return $resource('api/modulespublishing/', {}, {
        getModules: { method: 'GET', url: 'api/modulespublishing/sources/', isArray: true },
        runPublish: { method: 'POST', url: 'api/modulespublishing/publish' },
    });
}]);