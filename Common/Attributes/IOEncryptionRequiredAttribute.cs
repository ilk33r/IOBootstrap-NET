using System;

namespace IOBootstrap.NET.Common.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class IOEncryptionRequiredAttribute : Attribute
{
    public IOEncryptionRequiredAttribute()
    {
    }
}
