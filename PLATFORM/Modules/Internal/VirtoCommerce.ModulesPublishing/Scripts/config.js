//Call this to register our module to main application
var moduleTemplateName = "virtoCommerce.modulesPublishing";

if (AppDependencies != undefined) {
    AppDependencies.push(moduleTemplateName);
}


angular.module(moduleTemplateName, [])
.config(['$stateProvider', function ($stateProvider) {
      $stateProvider
          .state('workspace.modulesPublishing', {
              url: '/modulesPublishing',
              templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
              controller: [
                  '$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
                      var blade = {
                          id: 'modulesPublishing',
                          title: 'Modules publishing',
                          breadcrumbs: [],
                          subtitle: 'Install or update modules',
                          controller: 'virtoCommerce.modulesPublishing.modulesListController',
                          template: 'Modules/$(VirtoCommerce.ModulesPublishing)/Scripts/blades/modules-list.tpl.html',
                          isClosingDisabled: true
                      };
                      bladeNavigationService.showBlade(blade);
                      $scope.moduleName = 'vc-modulesPublishing';
                  }
              ]
          });
    }
  ])
.run(
  ['platformWebApp.mainMenuService', '$state', function (mainMenuService, $state) {
      console.log('registr');
      //Register module in main menu
      var menuItem = {
          path: 'configuration/modulesPublishing',
          icon: 'fa fa-cube',
          title: 'Modules publishing',
          priority: 120,
          action: function () { $state.go('workspace.modulesPublishing'); }
      };
      mainMenuService.addMenuItem(menuItem);
  }]);

console.log('config end');