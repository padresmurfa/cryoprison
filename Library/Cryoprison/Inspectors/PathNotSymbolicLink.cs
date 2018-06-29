using System;
using System.Collections.Generic;

namespace Cryoprison.Inspectors
{
    /// <summary>
    /// Determines if the specified path is a symbolic link
    /// </summary>
    public class PathNotSymbolicLink : InspectorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cryoprison.Inspectors.FileNotDestructivelyWritable"/> class.
        /// </summary>
        public PathNotSymbolicLink()
            : base("PATH_{0}_SHOULD_NOT_REFER_TO_SYMBOLIC_LINK")
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
                return !DoesPathPointToSymbolicLink(this.val);
            }
        }

        /// <summary>
        /// Override this in method in OS specific fashions to determine if this
        /// path is a sym link.
        /// <param name="path">The full path.</param>
        protected virtual bool IsSymLink(string path)
        {
            return false;
        }

        /// <summary>
        /// Determines whether or not the path refers to a a symbolic link,
        /// wrapped with all the niceties such as exception handling.
        /// </summary>
        /// <param name="path">The full path to the path to check.</param>
        public bool DoesPathPointToSymbolicLink(string path)
        {
            try
            {
                return IsSymLink(path);
            }
            catch (Exception ex)
            {
                Reporter.ReportException($"DoesPathPointToSymbolicLink bombed for {path}", ex);
                return false;
            }
        }
    }
}
