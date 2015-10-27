using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.PushNotifications;

namespace VirtoCommerce.ModulesPublishing.Import
{
    public class ModulePublishingPushNotification: PushNotification
    {
        public ModulePublishingPushNotification(string creator)
            : base(creator)
        {
            Errors = new List<string>();
        }
        [JsonProperty("finished")]
        public DateTime? Finished { get; set; }
        [JsonProperty("totalCount")]
        public long TotalCount { get; set; }
        [JsonProperty("createdCount")]
        public long CreatedCount { get; set; }
        [JsonProperty("updatedCount")]
        public long UpdatedCount { get; set; }
        [JsonProperty("processedCount")]
        public long ProcessedCount { get; set; }
        [JsonProperty("errorCount")]
        public long ErrorCount
        {
            get
            {
                return Errors != null ? Errors.Count() : 0;
            }
        }
        [JsonProperty("errors")]
        public ICollection<string> Errors { get; set; }
    }

}