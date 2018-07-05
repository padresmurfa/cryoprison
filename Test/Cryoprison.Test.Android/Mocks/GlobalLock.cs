using System;
using System.Threading;
using A = Android;

namespace Cryoprison.Test.Android.Mocks
{
    public class GlobalLock : IDisposable
    {
        private static object lockObject = new { };
        private bool lockTaken;

        public GlobalLock(int timeout = 30000)
        {
            bool lockTaken = false;
            Monitor.TryEnter(GlobalLock.lockObject, timeout, ref lockTaken);
            this.lockTaken = lockTaken;

            if (!this.lockTaken)
            {
                throw new Exception($"Failed to acquire global engine lock - timeout after {timeout} ms");
            }
            else
            {
                Java.Lang.Runtime.MockRuntime = new Java.Lang.Runtime();
                A.App.Application.Context = new A.Content.Context();
                A.OS.Build.Tags = null;
            }
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
