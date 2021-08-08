using System;

namespace IOBootstrap.NET.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class IOValidateRequestModelAttribute : Attribute
    {
        public IOValidateRequestModelAttribute()
        {
        }
    }
}
