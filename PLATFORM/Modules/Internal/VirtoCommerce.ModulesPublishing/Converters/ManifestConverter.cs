using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.ModulesPublishing.Model;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.ModulesPublishing.Converters
{
    public class ManifestConverter
    {
        public static CatalogProduct ConvertToVariation(SourceModuleManifest source)
        {
            var variation = new CatalogProduct
            {
                Name = source.VariationName,
                Code = source.VariationCode,
                MainProductId = source.Product.Id,
                CategoryId = source.Product.CategoryId,
                CatalogId = source.Product.CatalogId,
            };
            return variation;
        }

        public static SourceModule ConvertToSourceModule(ModuleManifest manifest)
        {
            var result = new SourceModule
            {
                Id = manifest.Id,
                Name = manifest.Title,
                Description = manifest.Description,
                Version = manifest.Version,
            };
            return result;
        }

    }
}