using System;

namespace Foundation
{
    public class NSFileManager
    {
        public static NSFileManager DefaultManager { get; set; } = new NSFileManager();

        public delegate NSFileAttributes MockGetAttributesDelegate(string path, out NSError error);

        public MockGetAttributesDelegate MockGetAttributes { get; set; }

        public NSFileAttributes GetAttributes(string path, out NSError error)
        {
            error = null;

            if (this.MockGetAttributes != null)
            {
                return this.MockGetAttributes(path, out error);
            }

            return null;
        }
    }
}
