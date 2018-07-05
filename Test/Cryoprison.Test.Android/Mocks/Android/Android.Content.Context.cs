using System;

namespace Android.Content
{
    public class Context
    {
        public Android.Content.PM.PackageManager PackageManager { get; set; } = new PM.PackageManager();
    }
}
