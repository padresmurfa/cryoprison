using UIKit;

namespace Cryoprison.Test.iOS
{
    public class GlobalLock : Cryoprison.Test.Shared.GlobalLock
    {
        public GlobalLock(int timeout = 30000)
            : base(timeout)
        {
            Foundation.NSBundle.MainBundle = new Foundation.NSBundle();
            Foundation.NSFileManager.DefaultManager = new Foundation.NSFileManager();
            UIApplication.SharedApplication = new UIApplication();
        }
    }
}
