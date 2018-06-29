using System;
using Foundation;

namespace Cryoprison.iOS.PlatformSpecific
{
    /// <summary>
    /// iOS specific version of performing a jailbreak check by determining
    /// if a path is a symbolic link
    /// </summary>
    public class PathNotSymbolicLink : Cryoprison.Inspectors.PathNotSymbolicLink
    {
        /// <summary>
        /// Determines if the path is a symbolic link, iOS style.
        /// </summary>
        protected override bool IsSymLink(string path)
        {
            NSError error = null;
            try
            {
                var attributes = NSFileManager.DefaultManager?.GetAttributes(path, out error);
                if (attributes == null || error != null)
                {
                    return false;
                }

                return attributes.Type == NSFileType.SymbolicLink;
            }
            finally
            {
                error?.Dispose();
            }
        }
    }
}
