using System;
using System.Collections.Generic;
using A = Android;
using Android.Content.PM;

namespace Cryoprison.Android.PlatformSpecific
{
    /// <summary>
    /// Determines if we can locate a file using the which command
    /// </summary>
    public class ShouldNotHavePackageInstalled : IInspector
    {
        private string id;
        private string path;

        /// <inheritdoc/>
        public IInspector Init(string id, string path)
        {
            this.id = id;
            this.path = path;
            return this;
        }

        /// <inheritdoc/>
        public string Id
        {
            get
            {
                var idUpper = this.id.ToUpperInvariant();
                return $"SHOULD_NOT_HAVE_BUILD_TAG_{idUpper}";
            }
        }

        /// <inheritdoc/>
        public bool Ok
        {
            get
            {
                return !HasPackageInstalled(this.path);
            }
        }

        /// <summary>
        /// Determines if the app has been rooted, by checking if the specified
        /// package is installed
        /// </summary>
        public static bool HasPackageInstalled(string target)
        {
            try
            {
                var context = A.App.Application.Context;
                if (context != null)
                {
                    var pm = context.PackageManager;

                    try
                    {
                        // if we can locate the package, then its obviously
                        // installed
                        pm.GetPackageInfo(target, 0);
                        return true;
                    }
                    catch (PackageManager.NameNotFoundException)
                    {
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Reporter.ReportException($"HasPackageInstalled bombed", ex);
                return false;
            }
        }
    }
}
