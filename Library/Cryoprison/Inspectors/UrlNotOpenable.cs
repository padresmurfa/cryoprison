using System;
using System.Collections.Generic;

namespace Cryoprison.Inspectors
{
    /// <summary>
    /// Determines if the url is openable, which would only be the case on
    /// a jailbroken device.
    /// </summary>
    public class UrlNotOpenable : InspectorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cryoprison.Inspectors.FileNotDestructivelyWritable"/> class.
        /// </summary>
        public UrlNotOpenable()
            : base("URL_{0}_SHOULD_NOT_BE_OPENABLE")
        {
        }

        /// <summary>
        /// Returns true if the file identified by 'val' during initialization
        /// can not be created, written to and deleted.
        /// </summary>
        public override bool Ok
        {
            get
            {
                return !IsUrlOpenable(this.val);
            }
        }

        /// <summary>
        /// Override this method in OS specific versions to perform the actual
        /// url opening check.
        /// </summary>
        /// <param name="url">The url to check</param>
        protected virtual bool CanOpenUrl(string url)
        {
            return true;
        }

        /// <summary>
        /// Determines if the URL is indeed openable, wrapped in the right
        /// exception handling code.
        /// </summary>
        /// <param name="url">The url to check</param>
        public bool IsUrlOpenable(string url)
        {
            try
            {
                return CanOpenUrl(url);
            }
            catch (Exception ex)
            {
                Reporter.ReportException($"IsUrlOpenable bombed for {url}", ex);
                return false;
            }
        }
    }
}
