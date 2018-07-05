using System;
using System.Collections.Generic;
using System.Linq;

namespace Java.Lang
{
    public class Process : IDisposable
    {
        public string[] MockExecArgs { get; set; }

        public bool MockDisposed { get; set; }

        public bool MockDestroyed { get; set; }

        private List<string> mockStdOutput = new List<string>();
        public IEnumerable<string> MockStdOutput
        {
            get
            {
                return mockStdOutput;
            }

            set
            {
                mockStdOutput = value?.ToList();
                this.InputStream.MockInput = mockStdOutput;
            }
        }

        public void Dispose()
        {
            this.MockDisposed = true;
        }

        public void Destroy()
        {
            this.MockDestroyed = true;
        }

        public Java.IO.InputStream InputStream { get; set; } = new Java.IO.InputStream();
    }
}
