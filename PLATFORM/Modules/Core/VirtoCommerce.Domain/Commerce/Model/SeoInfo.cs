﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Domain.Commerce.Model
{
	public class SeoInfo : AuditableEntity, ILanguageSupport
	{
		public string SemanticUrl { get; set; }
		public string PageTitle { get; set; }
		public string MetaDescription { get; set; }
		public string ImageAltDescription { get; set; }
		public string MetaKeywords { get; set; }
		public string ObjectId { get; set; }
		public string ObjectType { get; set; }

		#region ISupportLanguage Members

		public string LanguageCode { get; set; }

		#endregion
	}
}
