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
            var notification = new ModulePublishingPushNotification(CurrentPrincipal.GetCurrentUserName())
            {
                Title = "Publishing modules task",
                Description = "Publishing task added and will start soon...."
            };
            _pushNotifier.Upsert(notification);

            var importManifest = new ImportManifest();
            var settingsManager = ServiceLocator.Current.GetInstance<ISettingsManager>();
            var sourcePath = settingsManager.GetValue("VirtoCommerce.ModulesPublishing.General.SourcePath", String.Empty);
            importManifest.ModulesPath = HttpContext.Current.Server.MapPath(sourcePath);
            importManifest.NewAppCategoryCode = settingsManager.GetValue("VirtoCommerce.ModulesPublishing.General.NewAppCategory", String.Empty);
            importManifest.CatalodId = catalogId;

            BackgroundJob.Enqueue(() => ModulesImportBackground(importManifest, notification));

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
                _pushNotifier.Upsert(pushNotification);
            }

        }
    }
}