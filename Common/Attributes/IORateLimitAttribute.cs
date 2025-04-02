using System;

namespace IOBootstrap.NET.Common.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class IORateLimitAttribute : Attribute
{
    public int Seconds;
    public int RequestCount;

    public IORateLimitAttribute(int seconds, int requestCount)
    {
        this.Seconds = seconds;
        this.RequestCount = requestCount;
    }
}
