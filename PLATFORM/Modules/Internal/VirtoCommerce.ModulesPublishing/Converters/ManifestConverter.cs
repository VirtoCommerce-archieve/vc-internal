using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.ModulesPublishing.Model;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.ModulesPublishing.Converters
{
    public class ManifestConverter
    {
        public static CatalogProduct ConvertToProduct(SourceModuleManifest source)
        {
            var product = new CatalogProduct
            {
                Name = source.Manifest.Title,
                Code = source.Manifest.Id,
                CategoryId = source.Category.Id,
                CatalogId = source.Category.CatalogId,
            };
            return product;
        }

        public static CatalogProduct ConvertToVariation(SourceModuleManifest source)
        {
            var variation = new CatalogProduct
            {
                Name = source.VariationName,
                Code = source.VariationCode,
                MainProductId = source.Product.Id,
                CategoryId = source.Category.Id,
                CatalogId = source.Category.CatalogId,
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