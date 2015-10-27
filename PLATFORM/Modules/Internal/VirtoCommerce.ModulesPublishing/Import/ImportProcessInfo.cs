using VirtoCommerce.Platform.Core.ExportImport;

namespace VirtoCommerce.ModulesPublishing.Import
{
    public class ImportProcessInfo : ExportImportProgressInfo
    {
        public long CreatedCount { get; set; }
        public long UpdatedCount { get; set; }
    }
}