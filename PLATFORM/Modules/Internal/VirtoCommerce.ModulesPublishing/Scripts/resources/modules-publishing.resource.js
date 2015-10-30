angular.module('virtoCommerce.modulesPublishing')
.factory('virtoCommerce.modulesPublishing.resource', ['$resource', function ($resource) {
    return $resource('api/modulespublishing/', {}, {
        runPublish: { method: 'POST', url: 'api/modulespublishing' },
    });
}]);