using System;
using System.Collections.Generic;
using A = Android;

namespace Cryoprison.Android.PlatformSpecific
{
    /// <summary>
    /// Determines if the kernel was built with specific tags
    /// </summary>
    public class ShouldNotHaveSpecificBuildTags : IInspector
    {
        private Ex.Env Env;
        private string id;
        private string path;

        /// <inheritdoc/>
        public IInspector Init(Ex.Env env, string id, string path)
        {
            this.Env = env;
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new ArgumentException(id);
                }
                else if (string.IsNullOrWhiteSpace(path))
                {
                    throw new ArgumentException(path);
                }

                this.id = id;
                this.path = path;
            }
            catch (Exception ex)
            {
                this.Env.Reporter.ReportException($"ShouldNotHaveSpecificBuildTags bombed in Init ({id})", ex);
            }

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
        public bool HasBuildTag(string target)
        {
            try
            {
                var buildTags = A.OS.Build.Tags?.ToLowerInvariant();

                return buildTags?.Contains(target.ToLowerInvariant()) ?? false;
            }
            catch (Exception ex)
            {
                this.Env.Reporter.ReportException($"HasBuildTag bombed", ex);
                return false;
            }
        }
    }
}
