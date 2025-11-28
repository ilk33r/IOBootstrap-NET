using System;
using IOBootstrap.NET.Common.Enumerations;

namespace IOBootstrap.NET.Common.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class IOUserRoleAttribute : Attribute
{
    public int RequiredRole;

    public IOUserRoleAttribute(UserRoles requiredRole)
    {
        this.RequiredRole = (int)requiredRole;
    }

    public IOUserRoleAttribute(int requiredRole)
    {
        this.RequiredRole = requiredRole;
    }
}
