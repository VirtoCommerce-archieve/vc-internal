using System;

namespace VirtoCommerce.ModulesPublishing.Import
{
    public interface IModuleImporter
    {
        void DoImport(ImportManifest importManifest, Action<ImportProcessInfo> progressCallback);
    }
}