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
            if (Cryoprison.Factory.IsSupported)
            {
                var env = Cryoprison.Factory.CreateEnvironment();

                env.Reporter.OnJailbreakReported = (id) =>
                {
                    Console.WriteLine($"Jailbreak: {id ?? "<null>"}");
                };

                env.Reporter.OnExceptionReported = (message, exception) =>
                {
                    Console.WriteLine($"Jailbreak Error: {message}");
                    Console.WriteLine(exception.ToString());
                };

                // The Nuget Test app is intended to show that the library was imported from nuget and worked,
                // so no need to be simulator friendly here.
                this.jailbreakDetector = Cryoprison.Factory.CreateJailbreakDetector(env, simulatorFriendly: false);

                NugetTest.App.IsJailBroken = () =>
                {
                    return this.jailbreakDetector.IsJailbroken;
                };

                NugetTest.App.JailBreaks = () =>
                {
                    return this.jailbreakDetector.Violations;
                };
            }
            else
            {
                NugetTest.App.IsJailBroken = () =>
                {
                    return false;
                };

                NugetTest.App.JailBreaks = () =>
                {
                    return new string[0];
                };
            }

            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
