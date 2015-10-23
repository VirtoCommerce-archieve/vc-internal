using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Platform.Core.Modularity;


namespace VirtoCommerce.ModulesPublishing.Model
{
    public class SourceModuleManifest
    {
        public ModuleManifest Manifest { get; set; }

        public string ZipPath { get; set; }

        public CatalogProduct Product { get; set; }

        public CatalogProduct Variation { get; set; }

        public Category Category { get; set; }

        public string VariationCode
        {
            get
            {
                return Manifest != null ? string.Format("{0}-{1}", Manifest.Id, Manifest.Version) : string.Empty;
            }
        }

        public string VariationName
        {
            get
            {
                return Manifest != null ? string.Format("{0} ({1})", Manifest.Title, Manifest.Version) : string.Empty;
            }
        }
    }
}