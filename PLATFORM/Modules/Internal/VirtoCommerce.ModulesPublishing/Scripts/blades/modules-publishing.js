angular.module('virtoCommerce.modulesPublishing')
.controller('virtoCommerce.modulesPublishing.modulesPublishingController', ['$injector', '$rootScope', '$scope', 'virtoCommerce.modulesPublishing.resource', 'platformWebApp.bladeNavigationService',
function ($injector, $rootScope, $scope, api, bladeNavigationService) {
    var blade = $scope.blade;
    blade.isLoading = false;

    $scope.$on("new-notification-event", function (event, notification) {
        console.log("notification", notification);
        if (blade.notification && notification.id == blade.notification.id) {
            angular.copy(notification, blade.notification);
            if (notification.errorCount > 0) {
                bladeNavigationService.setError('Publish error', blade);
            }
        }
    });

    if (blade.id == "modulesPublishing")
    {
        var parameters = { catalogId: blade.catalog.id };
        api.runPublish({}, parameters,
            function (data) { blade.notification = data; },
            function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
    }
}]);