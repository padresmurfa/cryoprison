using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace NugetTest.Droid
{
    [Activity(Label = "NugetTest", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private Cryoprison.IJailbreakDetector jailbreakDetector;

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            var env = new Cryoprison.Ex.Env();

            env.Reporter.OnJailbreakReported = (id) => {
                Console.WriteLine($"Jailbreak: {id ?? "<null>"}");
            };

            env.Reporter.OnExceptionReported = (message, exception) => {
                Console.WriteLine($"Jailbreak Error: {message}");
                Console.WriteLine(exception.ToString());
            };

            this.jailbreakDetector = new Cryoprison.Android.JailbreakDetector(env);

            NugetTest.App.IsJailBroken = () =>
            {
                return this.jailbreakDetector.IsJailbroken;
            };

            NugetTest.App.JailBreaks = () =>
            {
                return this.jailbreakDetector.Violations;
            };

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}