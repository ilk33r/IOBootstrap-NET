using System;

namespace IOBootstrap.NET.Common.Enumerations
{
    public enum UserRoles
    {
        SuperAdmin = 0,
        Admin = 1,
        User = 2
    }

    public static class UserRoleUtility 
    {
        public static bool CheckRole(UserRoles requiredRole, UserRoles userRole) 
        {
            int userRoleValue = (int)userRole;
            int requiredRoleValue = (int)requiredRole;

            return userRole <= requiredRole;
        }
    }
}
