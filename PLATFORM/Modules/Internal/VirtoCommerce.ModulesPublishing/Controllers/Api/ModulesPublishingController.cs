using System;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Practices.ServiceLocation;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Core.Asset;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.ModulesPublishing.Import;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.PushNotifications;
using Hangfire;
using Omu.ValueInjecter;
using VirtoCommerce.Platform.Data.Common;
using VirtoCommerce.CatalogModule.Web.ExportImport;

namespace VirtoCommerce.ModulesPublishing.Controllers.Api
{
    [RoutePrefix("api/modulespublishing")]
    public class ModulesPublishingController : ApiController
    {
        private readonly IPushNotificationManager _pushNotifier;
        private readonly IModuleImporter _moduleImporter;

        public ModulesPublishingController(IPushNotificationManager pushNotifier, IModuleImporter moduleImporter)
        {
            _pushNotifier = pushNotifier;
            _moduleImporter = moduleImporter;
        }

        [HttpPost]
        [ResponseType(typeof(void))]
        [Route("publish")]
        public IHttpActionResult Publish([FromBody]string catalogId)
        {
            var settingsManager = ServiceLocator.Current.GetInstance<ISettingsManager>();
            var sourcePath = settingsManager.GetValue("VirtoCommerce.ModulesPublishing.AppStoreImport.SourcePath", String.Empty);

            var importManifest = new ImportManifest
            {
                CatalodId = catalogId,
                NewAppCategoryCode = settingsManager.GetValue("VirtoCommerce.ModulesPublishing.AppStoreImport.NewAppCategoryCode", String.Empty),
                ModulesPath = HttpContext.Current.Server.MapPath(sourcePath)
            };

            var notification = new ModulePublishingPushNotification(CurrentPrincipal.GetCurrentUserName())
            {
                Title = "Import applications task",
                Description = "Task added and will start soon...."
            };

            if (string.IsNullOrEmpty(importManifest.NewAppCategoryCode))
            {
                notification.Errors.Add("Set 'Code category' setting, before import.");
            }
            if (string.IsNullOrEmpty(sourcePath))
            {
                notification.Errors.Add("Set 'Source path' setting, before import.");
            }

            if (notification.Errors.Count == 0)
            {
                BackgroundJob.Enqueue(() => ModulesImportBackground(importManifest, notification));
            }

            _pushNotifier.Upsert(notification);
            return Ok(notification);

        }

        public void ModulesImportBackground(ImportManifest importManifest, ModulePublishingPushNotification pushNotification)
        {
            Action<ImportProcessInfo> progressCallback = (x) =>
            {
                pushNotification.InjectFrom(x);
                pushNotification.Errors = x.Errors;
                _pushNotifier.Upsert(pushNotification);
            };

            try
            {
                _moduleImporter.DoImport(importManifest, progressCallback);
            }
            catch (Exception ex)
            {
                pushNotification.Errors.Add(ex.ExpandExceptionMessage());
            }
            finally
            {
                pushNotification.Finished = DateTime.UtcNow;
                pushNotification.Description = "Import finished";
                _pushNotifier.Upsert(pushNotification);
            }

        }
    }
}