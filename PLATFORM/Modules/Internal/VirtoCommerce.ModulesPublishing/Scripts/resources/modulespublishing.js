angular.module('virtoCommerce.modulesPublishing')
.factory('virtoCommerce.modulesPublishing.resource', ['$resource', function ($resource) {
    return $resource('api/modulespublishing/', {}, {
        get: { method: 'GET', url: 'api/modulespublishing/get/', isArray: true },
    });
}]);