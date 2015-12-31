﻿using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Domain.Catalog.Services;
using System.IO;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Platform.Core.PushNotifications;
using System.IO.Compression;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Asset;
using System.Web;
using System.Text.RegularExpressions;

namespace VirtoCommerce.ModulesPublishing.Import
{
    public sealed class ModuleImporter : IModuleImporter
    {
        private readonly ICatalogService _catalogService;
        private readonly ICategoryService _categoryService;
        private readonly IItemService _productService;
        private readonly IPushNotificationManager _pushNotificationManager;
        private readonly ICatalogSearchService _searchService;
        private readonly IBlobStorageProvider _blobStorageProvider;
        private readonly IPropertyService _propertyService;
       private readonly IItemService _itemService;

        private Property[] _catalogProperties;
        private Category _defaultCategory;
        private enum PublishingResult
        {
            Product,
            Variation,
            None
        }

        public ModuleImporter(ICatalogService catalogService, ICategoryService categoryService, IItemService productService, 
            IBlobStorageProvider blobStorageProvider, IPushNotificationManager pushNotificationManager, 
            ICatalogSearchService searchService, IPropertyService propertyService, IItemService itemService)
        {
            _catalogService = catalogService;
            _categoryService = categoryService;
            _productService = productService;
            _pushNotificationManager = pushNotificationManager;
            _searchService = searchService;
            _blobStorageProvider = blobStorageProvider;
            _propertyService = propertyService;
            _itemService = itemService;
        }


        // add not existing products or variations.
        public void DoImport(ImportManifest importManifest, Action<ImportProcessInfo> progressCallback)
        {
            var progressInfo = new ImportProcessInfo();
            progressInfo.Description = "Importing ...";
            progressCallback(progressInfo);

            _defaultCategory = GetCategoriesByCode(importManifest.DefaultCategoryCode, importManifest.CatalogId);
            _catalogProperties = _propertyService.GetAllCatalogProperties(importManifest.CatalogId);
            var zipModulePaths = Directory.GetFiles(importManifest.PackagesPath);
            progressInfo.TotalCount = zipModulePaths.Count();

            foreach (var zipModulePath in zipModulePaths)
            {
                ModuleManifest manifest = null;
                byte[] icon = null;
                using (ZipArchive archive = ZipFile.OpenRead(zipModulePath))
                {
                    var manifestEntry = archive.Entries.FirstOrDefault(x => x.Name == importManifest.ManifestFileName);
                    using (var manifestStream = manifestEntry.Open())
                    {
                        manifest = ManifestReader.Read(manifestStream);
                    }

                    icon = ReadIcon(archive, manifest.IconUrl);
                }

                var publishingResult = Publish(manifest, zipModulePath, icon);

                progressInfo.CreatedCount += publishingResult == PublishingResult.Product ? 1 : 0;
                progressInfo.UpdatedCount += publishingResult == PublishingResult.Variation ? 1 : 0;
                progressInfo.ProcessedCount++;
                progressCallback(progressInfo);
            }
        }

        /// <summary>
        /// Publish given module to AppStore catalog
        /// </summary>
        private PublishingResult Publish(ModuleManifest manifest, string zipModulePath, byte[] icon)
        {
            var publishingResult = PublishingResult.None;
            var productCode = manifest.Id;

            productCode = Regex.Replace(productCode, @"[^A-Za-z0-9_]", "_");
            var product = GetProductByCode(productCode, _defaultCategory.Catalog.Id);

            if (product == null)
            {
                //add product
                product = new CatalogProduct
                {
                    Name = manifest.Title,
                    Code = productCode,
                    CategoryId = _defaultCategory.Id,
                    CatalogId = _defaultCategory.CatalogId,
                    IsActive = false,
                    IsBuyable = false
                };

                if (!string.IsNullOrEmpty(manifest.Description))
                {
                    AddProperty(product, "Description", manifest.Description, PropertyValueType.LongText);
                }

                if (!string.IsNullOrEmpty(manifest.IconUrl))
                {
                    AddIcon(product, Path.GetExtension(manifest.IconUrl), icon);
                }

                product = _productService.Create(product);
                publishingResult = PublishingResult.Product;
            }
            return AddVariation(product, manifest, icon, zipModulePath, publishingResult);
        }

        /// <summary>
        /// Add variation to given product + asset + icon + properties
        /// </summary>
        private PublishingResult AddVariation(CatalogProduct product, ModuleManifest manifest, byte[] icon, string zipModulePath, PublishingResult publishingResult)
        {
            //add variation + asset
            var variationCode = string.Format("{0}_{1}", manifest.Id, manifest.Version);
            variationCode = Regex.Replace(variationCode, @"[^A-Za-z0-9_]", "_");
            var variation = product.Variations.Where(x=>x.Code == variationCode).FirstOrDefault();

            if (variation == null)
            {
                variation = new CatalogProduct
                {
                    Name = string.Format("{0} ({1})", manifest.Title, manifest.Version),
                    Code = variationCode,
                    MainProductId = product.Id,
                    CategoryId = product.CategoryId,
                    CatalogId = product.CatalogId,
                };

                AddProperty(variation, "Description", manifest.Description, PropertyValueType.LongText);
                AddProperty(variation, "ReleaseNote", manifest.ReleaseNotes, PropertyValueType.LongText);
                AddProperty(variation, "ReleaseVersion", manifest.Version, PropertyValueType.ShortText);
                AddProperty(variation, "Compatibility", manifest.PlatformVersion, PropertyValueType.ShortText);
                AddProperty(variation, "ReleaseDate", DateTime.UtcNow, PropertyValueType.DateTime);

                AddAsset(variation, zipModulePath);

                if (publishingResult == PublishingResult.None)
                {
                    AddIcon(variation, Path.GetExtension(manifest.IconUrl), icon);
                    publishingResult = PublishingResult.Variation;
                }

                _productService.Create(variation);
            }
            return publishingResult;
        }

        /// <summary>
        /// Add property value to product
        /// </summary>
        private void AddProperty(CatalogProduct product, string propertyName, object value, PropertyValueType propertyValueType)
        {
            var property = _catalogProperties.FirstOrDefault(x => x.Name == propertyName);
            if (property != null)
            {
                var propertyValue = new PropertyValue
                {
                    PropertyId = property.Id,
                    PropertyName = property.Name,
                    Value = value,
                    ValueType = propertyValueType
                };
                if (product.PropertyValues == null)
                {
                    product.PropertyValues = new List<PropertyValue>();
                }
                product.PropertyValues.Add(propertyValue);
            }
        }

        /// <summary>
        /// Add icon to given product
        /// </summary>
        private void AddIcon(CatalogProduct product, string extention, byte[] icon)
        {
            if (icon != null)
            {
                using (MemoryStream ms = new MemoryStream(icon))
                {
                    var blobRelativeUrl = Path.Combine("catalog", product.Code, HttpUtility.UrlDecode(Path.ChangeExtension("icon", extention)));
                    using (var targetStream = _blobStorageProvider.OpenWrite(blobRelativeUrl))
                    {
                        ms.CopyTo(targetStream);
                    }
                    product.Images = new List<Image> { new Image { Url = blobRelativeUrl } };
                }
            }
        }

        /// <summary>
        /// Add asset to given product
        /// </summary>
        private void AddAsset(CatalogProduct product, string path)
        {
            using (var zipStream = new FileStream(path, FileMode.Open))
            {
                var blobRelativeUrl = Path.Combine("catalog", product.Code, HttpUtility.UrlDecode(Path.ChangeExtension(product.Code, "zip")));
                using (var targetStream = _blobStorageProvider.OpenWrite(blobRelativeUrl))
                {
                    zipStream.CopyTo(targetStream);
                }
                product.Assets = new List<Asset> { new Asset { Url = blobRelativeUrl, Name = Path.GetFileName(blobRelativeUrl) } };
            }
        }


        /// <summary>
        /// Get product by Code from given catalog 
        /// </summary>
        private CatalogProduct GetProductByCode(string code, string catalogId)
        {
            var responseGroup = SearchResponseGroup.WithProducts | SearchResponseGroup.WithVariations;

            var criteria = new SearchCriteria { Take = 1, Skip = 0, ResponseGroup = responseGroup, Code = code, CatalogId = catalogId, SearchInChildren = true, WithHidden = true};
            var searchResponse = _searchService.Search(criteria);
            return searchResponse.Products.FirstOrDefault();
        }

        /// <summary>
        /// Get category by Code from given catalog 
        /// </summary>
        private Category GetCategoriesByCode(string code, string catalogId)
        {
            var responseGroup = SearchResponseGroup.WithCatalogs | SearchResponseGroup.WithCategories;
            var criteria = new SearchCriteria { Take = 1, Skip = 0, ResponseGroup = responseGroup, Code = code, CatalogId = catalogId, WithHidden = true };
            var searchResponse = _searchService.Search(criteria);
            return searchResponse.Categories.FirstOrDefault();
        }

        /// <summary>
        /// Read icon of module from archive
        /// </summary>
        /// <returns></returns>
        private byte[] ReadIcon(ZipArchive archive, string iconPath)
        {
            var iconFileName = Path.GetFileName(iconPath);
            byte[] result = null;

            if (!string.IsNullOrEmpty(iconFileName))
            {
                var iconEntry = archive.Entries.FirstOrDefault(x => x.Name == iconFileName);
                if (iconEntry != null)
                {
                    using (var iconStream = iconEntry.Open())
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            iconStream.CopyTo(ms);
                            result = ms.ToArray();
                        }
                    }
                }
            }
            return result;
        }

    }
}