using System;

namespace IOBootstrap.NET.Common.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class IONonceRequiredAttribute : Attribute
{
    public IONonceRequiredAttribute()
    {
    }
}
