using System;

namespace IOBootstrap.NET.Common.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class IOIgnorePasswordExpireAttribute : Attribute
{
    public IOIgnorePasswordExpireAttribute()
    {
    }
}
