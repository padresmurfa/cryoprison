using A = Android;

namespace Cryoprison.Test.Android
{
    public class GlobalLock : Cryoprison.Test.Shared.GlobalLock
    {
        public GlobalLock(int timeout = 30000)
            : base(timeout)
        {
            Java.Lang.Runtime.MockRuntime = new Java.Lang.Runtime();
            A.App.Application.Context = new A.Content.Context();
            A.OS.Build.Tags = null;
        }
    }
}
