using System;

namespace IOBootstrap.NET.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class IORequireHTTPSAttribute : Attribute
    {
        public IORequireHTTPSAttribute()
        {
        }
    }
}
