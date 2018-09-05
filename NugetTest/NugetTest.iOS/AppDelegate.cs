using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace NugetTest.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private Cryoprison.IJailbreakDetector jailbreakDetector;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            var env = new Cryoprison.Ex.Env();

            env.Reporter.OnJailbreakReported = (id) => {
                Console.WriteLine($"Jailbreak: {id ?? "<null>"}");
            };

            env.Reporter.OnExceptionReported = (message, exception) => {
                Console.WriteLine($"Jailbreak Error: {message}");
                Console.WriteLine(exception.ToString());
            };

            this.jailbreakDetector = new Cryoprison.iOS.JailbreakDetector(env);
            NugetTest.App.IsJailBroken = () =>
            {
                return this.jailbreakDetector.IsJailbroken;
            };

            NugetTest.App.JailBreaks = () =>
            {
                return this.jailbreakDetector.Violations;
            };

            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
