using System;

namespace Plugin.CryoprisonPlugin
{
    /// <summary>
    /// Cross CryoprisonPlugin
    /// </summary>
    public static class CrossCryoprisonPlugin
    {
        static Lazy<ICryoprisonPlugin> implementation = new Lazy<ICryoprisonPlugin>(() => CreateCryoprisonPlugin(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets if the plugin is supported on the current platform.
        /// </summary>
        public static bool IsSupported => implementation.Value == null ? false : true;

        /// <summary>
        /// Current plugin implementation to use
        /// </summary>
        public static ICryoprisonPlugin Current
        {
            get
            {
                ICryoprisonPlugin ret = implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }
                return ret;
            }
        }

        static ICryoprisonPlugin CreateCryoprisonPlugin()
        {
#if NETSTANDARD1_0 || NETSTANDARD2_0
            return null;
#else
#pragma warning disable IDE0022 // Use expression body for methods
            return new CryoprisonPluginImplementation();
#pragma warning restore IDE0022 // Use expression body for methods
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly() =>
            new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");

    }
}
