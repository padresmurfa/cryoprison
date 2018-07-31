using System;

namespace Foundation
{
    public class NSUrl : IDisposable
    {
        public string MockUrl;

        public NSUrl(string url)
        {
            this.MockUrl = url;
        }

        public void Dispose()
        {
        }
    }
}
