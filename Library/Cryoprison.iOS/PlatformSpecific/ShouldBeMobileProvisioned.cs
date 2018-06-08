using System;
using System.Collections.Generic;
using Foundation;

namespace Cryoprison.Inspectors
{
    public class ShouldBeMobileProvisioned : IInspector
    {
        public static bool Disabled { get; set; } = false;

        public IInspector Init(string id, string path)
        {
            return this;
        }

        public string Id
        {
            get
            {
                return "EMBEDDED_MOBILEPROVISION_SHOULD_BE_PRESENT";
            }
        }

        public bool Ok
        {
            get
            {
                if (Disabled)
                {
                    // not required, e.g. of debug builds
                    return true;
                }
                return IsMobileProvisioned();
            }
        }

        public static bool IsMobileProvisioned()
        {
            try
            {
                var bundle = NSBundle.MainBundle;
                if (bundle != null)
                {
                    var mobileProvisionPath = bundle.PathForResource("embedded", "mobileprovision");
                    return !string.IsNullOrEmpty(mobileProvisionPath);
                }
                return false;
            }
            catch (Exception ex)
            {
                Reporter.ReportException($"IsMobileProvisioned bombed", ex);
                return false;
            }
        }
    }
}
