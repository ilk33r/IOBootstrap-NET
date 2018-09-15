using System;
using System.Reflection;
using IOBootstrap.NET.Common.Enumerations;

namespace IOBootstrap.NET.Common.Attributes
{

    [AttributeUsage(AttributeTargets.Method)]
    public class IOUserRoleAttribute : Attribute
    {
        public UserRoles requiredRole;

        public IOUserRoleAttribute(UserRoles requiredRole)
        {
            this.requiredRole = requiredRole;
        }
    }
}
