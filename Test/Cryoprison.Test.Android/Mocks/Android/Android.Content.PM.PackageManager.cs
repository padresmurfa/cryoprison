using System;
using System.Collections;
using System.Collections.Generic;

namespace Android.Content.PM
{
    public class PackageManager
    {
        public class NameNotFoundException : Exception
        {
        }

        public Dictionary<string, Android.Content.PM.PackageInfo> MockPackages { get; set; } = new Dictionary<string, Android.Content.PM.PackageInfo>();

        public Func<string, int, PackageInfo> MockGetPackageInfo { get; set; }

        public Android.Content.PM.PackageInfo GetPackageInfo(string packageName, int flags)
        {
            if (MockGetPackageInfo != null)
            {
                return MockGetPackageInfo(packageName, flags);
            }
            if (MockPackages?.ContainsKey(packageName) ?? false)
            {
                return MockPackages[packageName];
            }

            throw new NameNotFoundException();
        }
    }
}
