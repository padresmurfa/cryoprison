using System;
using System.Collections.Generic;
using System.IO;
using Cryoprison.Ex;

namespace Cryoprison.Inspectors
{
    /// <summary>
    /// Detects whether or not a file is writable, that is can be created,
    /// written to and delete.
    /// </summary>
    public class FileNotDestructivelyWritable : InspectorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cryoprison.Inspectors.FileNotDestructivelyWritable"/> class.
        /// </summary>
        public FileNotDestructivelyWritable()
            : base("FILE_{0}_SHOULD_NOT_BE_DESTRUCTIVELY_WRITABLE")
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
                return !IsFileDestructivelyWritable(this.val);
            }
        }

        /// <summary>
        /// Determines if the file can be destructively written, that is
        /// create, written to and deleted.
        /// </summary>
        /// <param name="path">The full path.</param>
        public bool IsFileDestructivelyWritable(string path)
        {
            try
            {
                using (var file = Env.System.IO.File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                {
                    var bytes = new byte[] { 0, 1, 2, 3 };
                    file.Write(bytes, 0, 0);
                }
                return true;
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                return false;
            }
            catch (System.UnauthorizedAccessException)
            {
                return false;
            }
            catch (Exception ex)
            {
                ReportException($"IsFileDestructivelyWritable bombed for {path}", ex);
                return false;
            }
            finally
            {
                try
                {
                    Env.System.IO.File.Delete(path);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
