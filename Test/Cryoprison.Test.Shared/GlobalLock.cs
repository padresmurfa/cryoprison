using System;
using System.IO;
using System.Threading;

namespace Cryoprison.Test.Shared
{
    public class GlobalLock : IDisposable
    {
        private static object lockObject = new { };
        private bool lockTaken;

        public Ex.Env Env { get; set; } = new Ex.Env();

        public GlobalLock(int timeout = 30000)
        {
            bool lockTaken = false;
            Monitor.TryEnter(GlobalLock.lockObject, timeout, ref lockTaken);
            this.lockTaken = lockTaken;

            if (!this.lockTaken)
            {
                throw new Exception($"Failed to acquire global engine lock - timeout after {timeout} ms");
            }

            this.Env.System.IO.File.Exists = (path) => {
                return false;
            };

            this.Env.System.IO.File.Open = (a, b, c, d) => {
                throw new FileNotFoundException();
            };
        }

        public void Dispose()
        {
            if (this.lockTaken)
            {
                Monitor.Exit(GlobalLock.lockObject);
            }
        }
    }
}
