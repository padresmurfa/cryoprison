using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace SampleApp.Droid
{
    [Activity(Label = "SampleApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private Cryoprison.IJailbreakDetector jailbreakDetector;

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            this.jailbreakDetector = new Cryoprison.Android.JailbreakDetector();

            Cryoprison.Android.JailbreakDetector.OnJailbreakReported = (id) => {
                Console.WriteLine($"Jailbreak: {id ?? "<null>"}");
            };

            Cryoprison.Android.JailbreakDetector.OnExceptionReported = (message, exception) => {
                Console.WriteLine($"Jailbreak Error: {message}");
                Console.WriteLine(exception.ToString());
            };

            SampleApp.App.IsJailBroken = () =>
            {
                return this.jailbreakDetector.IsJailbroken;
            };

            SampleApp.App.JailBreaks = () =>
            {
                return this.jailbreakDetector.Violations;
            };

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}

