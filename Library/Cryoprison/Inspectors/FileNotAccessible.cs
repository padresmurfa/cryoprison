using System;
using System.Collections.Generic;
using System.IO;
using Cryoprison.Ex;

namespace Cryoprison.Inspectors
{
    /// <summary>
    /// Detects whether or not a file is accessible, that is can be opened for
    /// reading
    /// </summary>
    public class FileNotAccessible : InspectorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cryoprison.Inspectors.FileNotAccessible"/> class.
        /// </summary>
        public FileNotAccessible()
            : base("FILE_{0}_SHOULD_NOT_BE_ACCESSIBLE")
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
                return !IsFileAccessible(this.val);
            }
        }

        /// <summary>
        /// Determines whether the specified file is accessible, that is can be read
        /// <param name="path">The path to the file.</param>
        public bool IsFileAccessible(string path)
        {
            try
            {
                using (var file = Env.System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite | System.IO.FileShare.Delete))
                {
                    return true;
                }
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            catch (Exception ex)
            {
                ReportException($"IsFileAccessible bombed for {path}", ex);
                return false;
            }
        }
    }
}
