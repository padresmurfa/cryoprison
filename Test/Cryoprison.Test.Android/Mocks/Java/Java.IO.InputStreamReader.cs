using System;

namespace Java.IO
{
    public class InputStreamReader : IDisposable
    {
        public InputStream MockInputStream { get; set; }

        public bool MockDisposed { get; set; }

        public void Dispose()
        {
            this.MockDisposed = true;
        }

        public InputStreamReader(InputStream inputStream)
        {
            this.MockInputStream = inputStream;
        }
    }
}
