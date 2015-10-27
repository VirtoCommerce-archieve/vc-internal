using Microsoft.Practices.Unity;
using VirtoCommerce.CatalogModule.Web.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.ModulesPublishing
{
    public class Module : ModuleBase
    {
        private readonly IUnityContainer _container;

        public Module(IUnityContainer container)
        {
            _container = container;
        }

        public override void Initialize()
        {
            base.Initialize();

            _container.RegisterType<IModuleImporter, ModuleImporter>();
        }
    }
}