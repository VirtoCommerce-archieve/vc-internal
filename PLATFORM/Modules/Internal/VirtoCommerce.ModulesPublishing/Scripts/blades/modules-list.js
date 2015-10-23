angular.module('virtoCommerce.modulesPublishing')
.controller('virtoCommerce.modulesPublishing.modulesListController', ['$injector', '$rootScope', '$scope', 'virtoCommerce.modulesPublishing.resource',
function ($injector, $rootScope, $scope, api) {
    var blade = $scope.blade;
    $scope.data = "some content";
    $scope.blade.title = "some title";
    $scope.moduleList = {};

    $scope.refresh = function ()
    {
        blade.isLoading = true;
        $scope.moduleList = api.getModules();
        blade.isLoading = false;
    }

    $scope.canStartProcess = function () {
        var selectedModuleIds = getSelectedModuleIds();
        return selectedModuleIds.length > 0;
    }

    $scope.startProcess = function () {
        blade.isLoading = true;

        var selectedModuleIds = getSelectedModuleIds();

        api.runPublish(selectedModuleIds,
        function (data) { $scope.refresh(); },
        function (error) { blade.isLoading = false; });
    }

    function getSelectedModuleIds()
    {
        if ($scope.moduleList)
        {
            var selectedModules = _.where($scope.moduleList, { isChecked: true });
            var selectedModuleIds = _.pluck(selectedModules, 'id');
            return selectedModuleIds;
        }
        return new {};
    }

    $scope.refresh();

}]);