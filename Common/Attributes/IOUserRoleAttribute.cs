using System;
using System.Reflection;
using IOBootstrap.NET.Common.Enumerations;

namespace IOBootstrap.NET.Common.Attributes
{

    [AttributeUsage(AttributeTargets.Method)]
    public class IOUserRoleAttribute : Attribute
    {
        public int requiredRole;

        public IOUserRoleAttribute(UserRoles requiredRole)
        {
            this.requiredRole = (int)requiredRole;
        }

        public IOUserRoleAttribute(int requiredRole)
        {
            this.requiredRole = requiredRole;
        }
    }
}
