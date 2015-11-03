namespace VirtoCommerce.ModulesPublishing.Import
{
    public class ImportManifest
    {
        public string ManifestFileName { get; set; } = "module.manifest";
        public string PackagesPath { get; set; }
        public string DefaultCategoryCode { get; set; }
        public string CatalogId { get; set; }
    }
}