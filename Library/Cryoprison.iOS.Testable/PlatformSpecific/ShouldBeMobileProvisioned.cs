using System;
using System.Collections.Generic;
using Foundation;

namespace Cryoprison.Inspectors
{
    /// <summary>
    /// iOS specific check to determine if the app is has been provisioned for
    /// mobile.
    /// </summary>
    public class ShouldBeMobileProvisioned : IInspector
    {
        private Ex.Env Env;

        /// <summary>
        /// Initialize the inspector.  This inspector does not have any need
        /// for params really, so it ignores them.
        /// </summary>
        public IInspector Init(Ex.Env env, string id, string path)
        {
            this.Env = env;

            return this;
        }

        /// <inheritdoc/>
        public string Id
        {
            get
            {
                return "EMBEDDED_MOBILEPROVISION_SHOULD_BE_PRESENT";
            }
        }

        /// <inheritdoc/>
        public bool Ok
        {
            get
            {
                return IsMobileProvisioned();
            }
        }

        /// <summary>
        /// Determines if the app is provisioned for mobile.
        /// </summary>
        public bool IsMobileProvisioned()
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
                this.Env.Reporter.ReportException($"IsMobileProvisioned bombed", ex);
                return false;
            }
        }
    }
}
