using System;
using IOBootstrap.NET.Common.Enumerations;

namespace IOBootstrap.NET.Common.Utilities
{
    public static class IOUserRoleUtility
    {
        public static bool CheckRole(UserRoles requiredRole, UserRoles userRole)
        {
            return userRole <= requiredRole;
        }

        public static bool CheckRawRole(int requiredRole, int userRole)
        {
            return userRole <= requiredRole;
        }
    }
}