using System.Collections.Generic;
using System.Linq;
using System;
using System.IO.Compression;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Practices.ServiceLocation;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Core.Asset;
using VirtoCommerce.ModulesPublishing.Model;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.ModulesPublishing.Converters;
using System.Net;
using System.IO;

namespace VirtoCommerce.ModulesPublishing.Controllers.Api
{
    [RoutePrefix("api/modulespublishing")]
    public class ModulesPublishingController : ApiController
    {
        private readonly IBlobStorageProvider _blobStorageProvider;
        private readonly IItemService _itemService;
        private readonly ICatalogSearchService _catalogSearchService;

        public ModulesPublishingController(IBlobStorageProvider blobStorageProvider, IItemService itemService, ICatalogSearchService catalogSearchService)
        {
            _blobStorageProvider = blobStorageProvider;
            _itemService = itemService;
            _catalogSearchService = catalogSearchService;
        }

        [HttpGet]
        [ResponseType(typeof(SourceModule[]))]
        [Route("sources")]
        public IHttpActionResult Get()
        {
            var fileNames = GetLoadedModules();
            return Ok(fileNames);
        }

        [HttpPost]
        [ResponseType(typeof(void))]
        [Route("publish")]
        public IHttpActionResult Publish(string[] moduleIds )
        {
            PublishModules(moduleIds);
            return StatusCode(HttpStatusCode.NoContent);
        }


        private SourceModule[] GetLoadedModules()
        {
            var sources = GetModulesForPublishing();
            var result = sources.Select(x => ManifestConverter.ConvertToSourceModule(x.Manifest)).ToArray();
            return result.ToArray();
        }

        private void PublishModules(string[] moduleIds)
        {
            var sources = GetModulesForPublishing(moduleIds);
            foreach (var source in sources)
            {
                if (source.Product == null)
                {
                    //add product
                    var product = ManifestConverter.ConvertToProduct(source);
                    source.Product = _itemService.Create(product);
                }


                //add variation + asset
                var variation = ManifestConverter.ConvertToVariation(source);
                var assetUrl = UploadAsset(source);
                variation.Assets = new List<Asset>();
                variation.Assets.Add(new Asset { Url = assetUrl, Name = source.VariationCode });

                _itemService.Create(variation);

            }
        }

        private SourceModuleManifest[] GetModulesForPublishing(string[] moduleIds = null)
        {
            var categories = GetCategories();

            var sources = GetSourceModuleManifests();
            if (moduleIds != null)
            {
                sources = sources.Where(x => moduleIds.Any(m => m == x.Manifest.Id)).ToArray();
            }

            foreach (var source in sources)
            {
                var category = GetCategory(source, categories); //todo search category for source
                source.Category = category;
                source.Product = GetProductByCode(source.Manifest.Id, category.Id);

                if (source.Product != null)
                {
                    source.Variation = GetProductByCode(source.VariationCode, category.Id); 
                }
            }
            return sources.Where(x => x.Variation == null).ToArray();
        }

        private SourceModuleManifest[] GetSourceModuleManifests()
        {
            var settingsManager = ServiceLocator.Current.GetInstance<ISettingsManager>();
            var sourcePath = settingsManager.GetValue("VirtoCommerce.ModulesPublishing.General.SourcePath", String.Empty);
            var manifestPartUri = "module.manifest";
            var result = new List<SourceModuleManifest>();

            if (!string.IsNullOrEmpty(sourcePath))
            {
                string path = HttpContext.Current.Server.MapPath(sourcePath);
                if (System.IO.Directory.Exists(path))
                {
                    var zipModulePaths = System.IO.Directory.GetFiles(path);
                    foreach (var zipModulePath in zipModulePaths)
                    {
                        using (ZipArchive archive = ZipFile.OpenRead(zipModulePath))
                        {
                            var manifestEntry = archive.Entries.FirstOrDefault(x => x.Name == manifestPartUri);
                            if (manifestEntry != null)
                            {
                                using (var manifestStream = manifestEntry.Open())
                                {
                                    var manifest = ManifestReader.Read(manifestStream);
                                    result.Add(new SourceModuleManifest { Manifest = manifest, ZipPath = zipModulePath });
                                }
                            }
                        }
                    }
                }
            }
            return result.ToArray();
        }

        private CatalogProduct GetProductByCode(string code, string categoryId)
        {
            const ResponseGroup responseGroup = ResponseGroup.WithProducts| ResponseGroup.WithVariations;
            var searchResponse = _catalogSearchService.Search(new SearchCriteria { Count = 1, Start = 0, ResponseGroup = responseGroup, CategoryId = categoryId, Code = code });
            return searchResponse.Products.FirstOrDefault();
        }

        private Category[] GetCategories()
        {
            const ResponseGroup responseGroup = ResponseGroup.WithCatalogs | ResponseGroup.WithCategories;
            var searchResponse = _catalogSearchService.Search(new SearchCriteria { Count = int.MaxValue, GetAllCategories = true, Start = 0, ResponseGroup = responseGroup });
            return searchResponse.Categories.ToArray();
        }

        private Category GetCategory(SourceModuleManifest sourceModuleManifest, Category[] categories)
        {
            var result = categories.FirstOrDefault(x => x.Code == "coremodules");
            return result;
        }

        private string UploadAsset(SourceModuleManifest source)
        {
            using (var zipStream = new FileStream(source.ZipPath, FileMode.Open))
            {
                var asset = new UploadStreamInfo
                {
                    FileName = Path.GetFileName(source.ZipPath),
                    FolderName = "Install",
                    FileByteStream = zipStream
                };
                var result = _blobStorageProvider.Upload(asset);
                return result;
            }
        }

    }
}