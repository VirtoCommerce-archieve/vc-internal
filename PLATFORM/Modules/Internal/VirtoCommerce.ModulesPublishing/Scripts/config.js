//Call this to register our module to main application
var moduleTemplateName = "virtoCommerce.modulesPublishing";

if (AppDependencies != undefined) {
    AppDependencies.push(moduleTemplateName);
}

angular.module(moduleTemplateName, [])
.run(
  ['platformWebApp.mainMenuService', '$state', 'virtoCommerce.catalogModule.catalogImportService', 'platformWebApp.pushNotificationTemplateResolver', 'platformWebApp.bladeNavigationService',
      function (mainMenuService, $state, catalogImportService, notificationTemplateResolver, bladeNavigationService) {
          var blade = {
              id: 'modulesPublishing',
              name: 'Publishing modules',
              description: 'Publish modules, using packages',
              icon: 'fa fa-download',
              title: "Publishing modules",
              controller: 'virtoCommerce.modulesPublishing.modulesPublishingController',
              template: 'Modules/$(VirtoCommerce.ModulesPublishing)/Scripts/blades/modules-publishing.tpl.html',
          }
          catalogImportService.register(blade);

          var historyPublishingTemplate =
            {
                priority: 900,
                satisfy: function (notify, place) { return place == 'history' && (notify.notifyType == 'ModulePublishingPushNotification'); },
                template: '$(Platform)/Scripts/app/exportImport/notifications/history.tpl.html',
                action: function (notify) {
                    var blade = {
                        id: 'modulesPublishingNotification',
                        controller: 'virtoCommerce.modulesPublishing.modulesPublishingController',
                        template: 'Modules/$(VirtoCommerce.ModulesPublishing)/Scripts/blades/modules-publishing.tpl.html',
                        icon: 'fa fa-download',
                        title: "Publishing modules",
                        notification: notify
                    };
                    bladeNavigationService.showBlade(blade);
                }
            };
          notificationTemplateResolver.register(historyPublishingTemplate);

          //var menuPublishingTemplate =
          //    {
          //        priority: 900,
          //        satisfy: function (notify, place) { return place == 'menu' && notify.notifyType == 'ModulePublishingPushNotification'; },
          //        template: 'Modules/$(VirtoCommerce.ModulesPublishing)/Scripts/blades/notifications/publishing.menu.tpl.html',
          //        action: function (notify) { $state.go('notificationsHistory', notify) }
          //    };
          //notificationTemplateResolver.register(menuPublishingTemplate);

      }]);


