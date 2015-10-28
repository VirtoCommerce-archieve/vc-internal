using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Platform.Core.Caching;
using System.IO;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Platform.Core.PushNotifications;
using Microsoft.Practices.ServiceLocation;
using VirtoCommerce.Platform.Core.Settings;
using System.IO.Compression;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Asset;
using VirtoCommerce.ModulesPublishing.Import;

namespace VirtoCommerce.CatalogModule.Web.ExportImport
{
    public sealed class ModuleImporter : IModuleImporter
    {
        private enum PublishingResult
        {
            Product,
            Variation,
            None
        }

        private readonly ICatalogService _catalogService;
        private readonly ICategoryService _categoryService;
        private readonly IItemService _productService;
        private readonly IPushNotificationManager _pushNotificationManager;
        private readonly CacheManager _cacheManager;
        private readonly ICatalogSearchService _searchService;
        private readonly IBlobStorageProvider _blobStorageProvider;

        public ModuleImporter(ICatalogService catalogService, ICategoryService categoryService, IItemService productService, IBlobStorageProvider blobStorageProvider,
                                IPushNotificationManager pushNotificationManager, CacheManager cacheManager, ICatalogSearchService searchService)
        {
            _catalogService = catalogService;
            _categoryService = categoryService;
            _productService = productService;
            _pushNotificationManager = pushNotificationManager;
            _cacheManager = cacheManager;
            _searchService = searchService;
            _blobStorageProvider = blobStorageProvider;
        }

        // add not existing products or variations.
        public void DoImport(ImportManifest importManifest, Action<ImportProcessInfo> progressCallback)
        {
            var settingsManager = ServiceLocator.Current.GetInstance<ISettingsManager>();
            var progressInfo = new ImportProcessInfo();

            var newAppCategory = GetCategoriesByCode(importManifest.NewAppCategoryCode, importManifest.CatalodId);

            progressInfo.Description = "Importing ...";
            progressCallback(progressInfo);


            string path = importManifest.ModulesPath;
            var zipModulePaths = Directory.GetFiles(path);
            progressInfo.TotalCount = zipModulePaths.Count();

            foreach (var zipModulePath in zipModulePaths)
            {
                ModuleManifest manifest = null;
                byte[] icon = null;

                using (ZipArchive archive = ZipFile.OpenRead(zipModulePath))
                {

                    var manifestEntry = archive.Entries.FirstOrDefault(x => x.Name == importManifest.ManifestFileName);
                    if (manifestEntry != null)
                    {
                        using (var manifestStream = manifestEntry.Open())
                        {
                            manifest = ManifestReader.Read(manifestStream);
                        }
                        if (manifest != null)
                        {
                            icon = ReadIcon(archive, manifest.IconUrl);
                        }
                    }
                }

                if (manifest != null)
                {
                    var publishingResult = Publish(manifest, importManifest, newAppCategory, zipModulePath, icon);

                    progressInfo.CreatedCount += publishingResult == PublishingResult.Product ? 1 : 0;
                    progressInfo.UpdatedCount += publishingResult == PublishingResult.Variation ? 1 : 0;
                    progressInfo.ProcessedCount++;
                    progressCallback(progressInfo);
                }
            }
        }

        private PublishingResult Publish(ModuleManifest manifest, ImportManifest importManifest, Category defaultCategory, string zipModulePath, byte[] icon)
        {
            var result = PublishingResult.None;
            var variationCode = string.Format("{0}-{1}", manifest.Id, manifest.Version);
            Image image = null;

            var product = GetProductByCode(manifest.Id, importManifest.CatalodId);

            if (product == null)
            {
                //add product
                product = CreateProduct(manifest, defaultCategory);
                image = UploadProductImage(manifest.IconUrl, variationCode, icon);
                AddImage(product, image);

                product = _productService.Create(product);
                result = PublishingResult.Product;
            }

            //add variation + asset
            var variation = GetProductByCode(variationCode, importManifest.CatalodId);
            if (variation == null)
            {
                //add variation
                variation = CreateVariation(manifest, product, variationCode);
                AddImage(variation, image ?? UploadProductImage(manifest.IconUrl, variationCode, icon));

                var assetUrl = UploadAsset(zipModulePath, importManifest.AssetFolder);
                variation.Assets = new List<Asset>();
                variation.Assets.Add(new Asset { Url = assetUrl, Name = variationCode });

                _productService.Create(variation);
                if (result == PublishingResult.None)
                {
                    result = result = PublishingResult.Variation;
                }
            }
            return result;
        }

        private CatalogProduct CreateProduct(ModuleManifest manifest, Category defaultCategory)
        {
            var product = new CatalogProduct
            {
                Name = manifest.Title,
                Code = manifest.Id,
                CategoryId = defaultCategory.Id,
                CatalogId = defaultCategory.CatalogId,
            };
            return product;
        }

        private CatalogProduct CreateVariation(ModuleManifest manifest, CatalogProduct product, string variationCode)
        {
            var variationName = string.Format("{0} ({1})", manifest.Title, manifest.Version);
            var variation = new CatalogProduct
            {
                Name = variationName,
                Code = variationCode,
                MainProductId = product.Id,
                CategoryId = product.CategoryId,
                CatalogId = product.CatalogId,
            };
            return variation;
        }

        private byte[] ReadIcon(ZipArchive archive, string iconPath)
        {
            var iconFileName = Path.GetFileName(iconPath);
            byte[] result = null;

            if (!string.IsNullOrEmpty(iconFileName))
            {
                var iconEntry = archive.Entries.FirstOrDefault(x => x.Name == iconFileName);

                using (var iconStream = iconEntry.Open())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        iconStream.CopyTo(ms);
                        result = ms.ToArray();
                    }
                }
            }
            return result;
        }

        private void AddImage(CatalogProduct product, Image image)
        {
            if (image != null)
            {
                product.Images = new List<Image>();
                product.Images.Add(image);
            }
        }

        private CatalogProduct GetProductByCode(string code, string catalogId)
        {
            var criteria = GetSearchCriteria(ResponseGroup.WithProducts | ResponseGroup.WithVariations, code, catalogId);
            var searchResponse = _searchService.Search(criteria);
            return searchResponse.Products.FirstOrDefault();
        }

        private Category GetCategoriesByCode(string code, string catalogId)
        {
            var criteria = GetSearchCriteria(ResponseGroup.WithCatalogs | ResponseGroup.WithCategories, code, catalogId);
            criteria.GetAllCategories = true;
            var searchResponse = _searchService.Search(criteria);
            return searchResponse.Categories.FirstOrDefault();
        }

        private SearchCriteria GetSearchCriteria(ResponseGroup responseGroup, string code, string catalogId)
        {
            var criteria = new SearchCriteria { Count = 1, Start = 0, ResponseGroup = responseGroup, Code = code };
            if (string.IsNullOrEmpty(catalogId))
            {
                criteria.CatalogId = catalogId;
            }

            return criteria;
        }

        private Image UploadProductImage(string imageFileName, string imageName, byte[] icon)
        {
            imageFileName = Path.GetFileName(imageFileName);
            var extention = Path.GetExtension(imageFileName);
            imageName = Path.ChangeExtension(imageName, extention);
            if (!string.IsNullOrEmpty(imageFileName) && icon != null)
            {

                using (MemoryStream ms = new MemoryStream(icon))
                {
                    var image = new Image();
                    image.Url = _blobStorageProvider.Upload(new UploadStreamInfo { FileByteStream = ms, FileName = imageName, FolderName = "catalog" });
                    return image;
                }
            }
            return null;
        }

        private string UploadAsset(string path, string assetFolderName)
        {
            using (var zipStream = new FileStream(path, FileMode.Open))
            {
                var asset = new UploadStreamInfo
                {
                    FileName = Path.GetFileName(path),
                    FolderName = assetFolderName,
                    FileByteStream = zipStream
                };
                var result = _blobStorageProvider.Upload(asset);
                return result;
            }
        }

    }
}