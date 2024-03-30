using System;

namespace IOBootstrap.NET.Common.Attributes
{

    [AttributeUsage(AttributeTargets.Method)]
    public class IOUserCustomRoleAttribute : Attribute
    {
        public int requiredRawRole;

        public IOUserCustomRoleAttribute(int requiredRole)
        {
            this.requiredRawRole = requiredRole;
        }
    }
}
