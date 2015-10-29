using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtoCommerce.ModulesPublishing.Import
{
    public class ImportManifest
    {
        public ImportManifest()
        {
            ManifestFileName = "module.manifest";
            AssetFolder = "packages";
        }
        public string ManifestFileName { get; set; }
        public string AssetFolder { get; set; }
        public string PackagesPath { get; set; }
        public string DefaultCategoryCode { get; set; }
        public string CatalodId { get; set; }
    }
}