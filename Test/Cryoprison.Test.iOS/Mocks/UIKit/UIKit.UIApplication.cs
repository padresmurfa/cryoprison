using System;
using Foundation;

namespace UIKit
{
	public class UIApplication
	{
        public static UIApplication SharedApplication { get; set; } = new UIApplication();

        public Func<NSUrl, bool> MockCanOpenUrl { get; set; }

        public bool CanOpenUrl(NSUrl url)
        {
            if (this.MockCanOpenUrl != null)
            {
                return this.MockCanOpenUrl(url);
            }

            return false;
        }
	}
}
