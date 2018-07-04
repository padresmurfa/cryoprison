using System;
using System.Collections.Generic;
using Cryoprison.Ex;

namespace Cryoprison.Inspectors
{
    /// <summary>
    /// Detects whether a directory is present, and reports a jailbreak if it
    /// is not.
    /// </summary>
    public class DirectoryNotPresent : InspectorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cryoprison.Inspectors.DirectoryNotPresent"/> class.
        /// </summary>
        public DirectoryNotPresent()
            : base("DIRECTORY_{0}_SHOULD_NOT_BE_PRESENT")
        {
        }

        /// <summary>
        /// Returns true if the directory identified by 'val' during initialization
        /// is not present, otherwise false.  This will cause the check to run.
        /// </summary>
        public override bool Ok
        {
            get
            {
                return !IsDirectoryPresent(this.val);
            }
        }

        /// <summary>
        /// Determines if the directory is actually present
        /// </summary>
        /// <param name="path">The full path.</param>
        public bool IsDirectoryPresent(string path)
        {
            try
            {
                return Env.System.IO.Directory.Exists(path);
            }
            catch (Exception ex)
            {
                ReportException($"IsDirectoryPresent bombed for {path}", ex);
                return false;
            }
        }
    }
}
