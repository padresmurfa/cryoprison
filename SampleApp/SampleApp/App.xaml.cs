using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using System.Linq;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace SampleApp
{
    public partial class App : Application
    {
        public static Func<bool> IsJailBroken = null;
        public static Func<IEnumerable<string>> JailBreaks = null;

        public static string GetJailBreakAlertMessage()
        {
            if (IsJailBroken == null || JailBreaks == null)
            {
                return null;
            }

            if (!IsJailBroken())
            {
                return null;
            }

            return string.Join(", ", JailBreaks());
        }

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
