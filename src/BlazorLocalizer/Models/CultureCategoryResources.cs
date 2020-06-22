using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorLocalizer.Models
{
    internal class CultureCategoryResources
    {
        public DateTime UpdatedTime { get; set; }
        public string Category { get; set; }
        public string Culture { get; set; }
        public IDictionary<string, string> Resources { get; set; }
    }
}
