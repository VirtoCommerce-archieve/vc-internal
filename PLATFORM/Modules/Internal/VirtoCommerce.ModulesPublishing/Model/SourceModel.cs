using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtoCommerce.ModulesPublishing.Model
{
    public class SourceModule
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Version { get; set; }

        public string ZipPath { get; set; }

        public bool IsNew { get; set; }

    }
}