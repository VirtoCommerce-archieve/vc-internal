angular.module('virtoCommerce.modulesPublishing')
.controller('virtoCommerce.modulesPublishing.modulesListController', ['$injector', '$rootScope', '$scope',
function ($injector, $rootScope, $scope) {
    $scope.data = "some content";
    $scope.blade.title = "some title";
    $scope.blade.isLoading = false;
}]);