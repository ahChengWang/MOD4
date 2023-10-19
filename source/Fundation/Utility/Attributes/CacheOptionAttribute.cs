using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Attributes
{
    public class CacheOptionAttribute : Attribute
    {
        public int Second { get; set; }
        public bool Refresh { get; set; }
        public string IdentityItem = "Id";
    }
}
