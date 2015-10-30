angular.module('virtoCommerce.modulesPublishing')
.controller('virtoCommerce.modulesPublishing.modulesPublishingController', ['$injector', '$rootScope', '$scope', 'virtoCommerce.modulesPublishing.resource', 'platformWebApp.bladeNavigationService',
function ($injector, $rootScope, $scope, api, bladeNavigationService) {
    var blade = $scope.blade;
    blade.isLoading = false;

    $scope.$on("new-notification-event", function (event, notification) {
        if (blade.notification && notification.id == blade.notification.id) {
            angular.copy(notification, blade.notification);
            if (notification.errorCount > 0) {
                bladeNavigationService.setError('Publish error', blade);
            }
            else if (blade.id == "modulesPublishing") {
                if (blade.notification.finished) {
                    blade.parentBlade.refresh();
                }
            }

        }
    });

    if (blade.id == "modulesPublishing") {
        var postData = { catalogId: blade.catalog.id };
        api.runPublish(postData,
            function (data) { blade.notification = data; },
            function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
    }
}]);