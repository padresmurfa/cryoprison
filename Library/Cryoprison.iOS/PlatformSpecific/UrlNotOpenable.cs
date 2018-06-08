using System;
using UIKit;
using Foundation;

namespace Cryoprison.iOS.PlatformSpecific
{
    public class UrlNotOpenable : Cryoprison.Inspectors.UrlNotOpenable
    {
        protected override bool CanOpenUrl(string url)
        {
            using (var nsUrl = new NSUrl(url))
            {
                return UIApplication.SharedApplication.CanOpenUrl(nsUrl);
            }
        }
    }
}
