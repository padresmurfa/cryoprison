using System;

namespace Foundation
{
    public class NSBundle
    {
        public static NSBundle MainBundle { get; set; } = new NSBundle();

        public Func<string, string, string> MockPathForResource { get; set; }

        public string PathForResource(string name, string ofType)
        {
            if (this.MockPathForResource != null)
            {
                return this.MockPathForResource(name, ofType);
            }

            return null;
        }
    }
}
