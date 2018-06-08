using System;
using Foundation;

namespace Cryoprison.iOS.PlatformSpecific
{
    public class PathNotSymbolicLink : Cryoprison.Inspectors.PathNotSymbolicLink
    {
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
