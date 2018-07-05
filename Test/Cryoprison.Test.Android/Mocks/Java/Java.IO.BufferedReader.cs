using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Java.IO
{
    public class BufferedReader : IDisposable
    {
        public bool MockDisposed { get; set; }

        public InputStreamReader MockStreamReader { get; set; }

        private int lineNumber;

        public void Dispose()
        {
            this.MockDisposed = true;
        }

        public BufferedReader(InputStreamReader streamReader)
        {
            this.MockStreamReader = streamReader;
        }

        public string ReadLine()
        {
            var lines = this.MockStreamReader?.MockInputStream?.MockInput?.ToList();
            if (lines == null)
            {
                return null;
            }

            if (this.lineNumber >= lines.Count())
            {
                return null;
            }

            return lines[this.lineNumber++];
        }
    }
}
