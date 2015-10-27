using System;
using VirtoCommerce.ModulesPublishing.Import;
using VirtoCommerce.Platform.Core.ExportImport;

namespace VirtoCommerce.CatalogModule.Web.ExportImport
{
    public interface IModuleImporter
    {
        void DoImport(ImportManifest importManifest, Action<ImportProcessInfo> progressCallback);
    }
}