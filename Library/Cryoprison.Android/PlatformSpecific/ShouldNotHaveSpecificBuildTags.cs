using System;
using System.Collections.Generic;
using AndroidBuild = Android.OS.Build;

namespace Cryoprison.Android.PlatformSpecific
{
    /// <summary>
    /// Determines if the kernel was built with specific tags
    /// </summary>
    public class ShouldNotHaveSpecificBuildTags : IInspector
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
                return !HasBuildTag(this.path);
            }
        }

        /// <summary>
        /// Determines if the app has been rooted, by checking if it has the
        /// specified build tag.
        /// </summary>
        public static bool HasBuildTag(string target)
        {
            try
            {
                var buildTags = AndroidBuild.Tags?.ToLowerInvariant();

                return buildTags?.Contains(target.ToLowerInvariant()) ?? false;
            }
            catch (Exception ex)
            {
                Reporter.ReportException($"HasBuildTag bombed", ex);
                return false;
            }
        }
    }
}
