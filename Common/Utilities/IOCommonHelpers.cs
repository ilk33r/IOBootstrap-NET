using System.Reflection;

namespace IOBootstrap.NET.Common.Utilities
{
    public static class IOCommonHelpers
    {
        #region Application Version

        public static string CurrentVersion
        {
            get
            {
                return Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            }
        }

        #endregion
    }
}
