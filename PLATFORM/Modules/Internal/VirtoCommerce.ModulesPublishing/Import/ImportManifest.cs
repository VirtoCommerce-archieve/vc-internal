namespace VirtoCommerce.ModulesPublishing.Import
{
    public class ImportManifest
    {
        public ImportManifest()
        {
            ManifestFileName = "module.manifest";
        }
        public string ManifestFileName { get; set; }
        public string PackagesPath { get; set; }
        public string DefaultCategoryCode { get; set; }
        public string CatalogId { get; set; }
    }
}