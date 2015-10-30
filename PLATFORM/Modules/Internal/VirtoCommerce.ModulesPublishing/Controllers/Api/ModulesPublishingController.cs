using System;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Hangfire;
using Omu.ValueInjecter;
using Microsoft.Practices.ServiceLocation;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.ModulesPublishing.Import;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.PushNotifications;

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
        [Route("")]
        public IHttpActionResult Publish(ImportManifest importManifest)
        {
            var settingsManager = ServiceLocator.Current.GetInstance<ISettingsManager>();
            var packagesPath = settingsManager.GetValue("VirtoCommerce.ModulesPublishing.AppStoreImport.PackagesPath", String.Empty);

            importManifest.DefaultCategoryCode = settingsManager.GetValue("VirtoCommerce.ModulesPublishing.AppStoreImport.DefaultCategoryCode", String.Empty);
            importManifest.PackagesPath = HttpContext.Current.Server.MapPath(packagesPath);

            var notification = new ModulePublishingPushNotification(CurrentPrincipal.GetCurrentUserName())
            {
                Title = "Import applications task",
                Description = "Task added and will start soon...."
            };

            if (string.IsNullOrEmpty(importManifest.DefaultCategoryCode))
            {
                notification.Errors.Add("Set 'Category code' setting, before import.");
            }
            if (string.IsNullOrEmpty(packagesPath))
            {
                notification.Errors.Add("Set 'Packages folder' setting, before import.");
            }
            if (!Directory.Exists(importManifest.PackagesPath))
            {
                notification.Errors.Add(string.Format("Path doesn't exists: {0}", importManifest.PackagesPath));
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
                pushNotification.Errors.Add(ex.ToString());
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