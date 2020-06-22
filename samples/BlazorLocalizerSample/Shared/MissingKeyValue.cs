using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorLocalizerSample.Shared
{
    public class MissingKeyValue : KeyValue
    {
        public string Culture { get; set; }
        public string Category { get; set; }
    }
}

