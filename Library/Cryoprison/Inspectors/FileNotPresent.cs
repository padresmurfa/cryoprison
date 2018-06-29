using System;
using System.Collections.Generic;

namespace Cryoprison.Inspectors
{
    /// <summary>
    /// Determines if a file is present, reporting a jailbreak if it is.
    /// </summary>
    public class FileNotPresent : InspectorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cryoprison.Inspectors.FileNotDestructivelyWritable"/> class.
        /// </summary>
        public FileNotPresent()
            : base("FILE_{0}_SHOULD_NOT_BE_PRESENT")
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
                return !IsFilePresent(this.val);
            }
        }

        /// <summary>
        /// Determines if the file is in deed present
        /// </summary>
        /// <param name="path">The full path of the file.</param>
        public static bool IsFilePresent(string path)
        {
            try
            {
                return System.IO.File.Exists(path);
            }
            catch (Exception ex)
            {
                Reporter.ReportException($"IsFilePresent bombed for {path}", ex);
                return false;
            }
        }
    }
}
