using System;
using Xunit;
using Cryoprison.Android;
using A = Android;
using Java.Lang;
using System.Linq;

namespace Cryoprison.Test.Android
{
    public class JailbreakDetectorAndroidTests
    {
        [Fact]
        public void ShouldNotClaimThatTheDeviceIsJailbrokenByDefault()
        {
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.Android.JailbreakDetector(gel.Env, simulatorFriendly: false);

                Assert.False(jbd.IsJailbroken);
            }
        }

        [Theory]
        [InlineData("su")]
        public void ShouldDetectIfSuHasBeenInstalled(string executable)
        {
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.Android.JailbreakDetector(gel.Env, simulatorFriendly: false);

                Java.Lang.Runtime.MockRuntime.MockExec = (parms) => {
                    if (parms.Count() == 2 && parms[0] == "which" && parms[1] == executable)
                    {
                        var process = new Process();
                        process.MockExecArgs = parms;
                        process.MockStdOutput = new[] {
                            "/asdf/" + executable
                        };
                        return process;
                    }
                    return null;
                };

                Assert.True(jbd.IsJailbroken);
            }
        }

        [Theory]
        [InlineData("test-keys")]
        [InlineData("asdf;test-keys")]
        [InlineData("test-keys;asdfasdf")]
        public void ShouldDetectIfKernelHasBeenCompiledByThirdParty(string key)
        {
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.Android.JailbreakDetector(gel.Env, simulatorFriendly: false);

                A.OS.Build.Tags = key;

                Assert.True(jbd.IsJailbroken);
            }
        }

        [Theory]
        [InlineData("eu.chainfire.supersu")]
        [InlineData("com.koushikdutta.superuser")]
        [InlineData("com.noshufou.android.su.elite")]
        [InlineData("com.thirdparty.superuser")]
        [InlineData("com.topjohnwu.magisk")]
        [InlineData("com.yellowes.su")]
        public void ShouldDetectIfRootAppsAreInstalled(string package)
        {
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.Android.JailbreakDetector(gel.Env, simulatorFriendly: false);

                A.App.Application.Context.PackageManager.MockPackages = new System.Collections.Generic.Dictionary<string, A.Content.PM.PackageInfo>
                {
                    { package, new A.Content.PM.PackageInfo() }
                };

                Assert.True(jbd.IsJailbroken);
            }
        }

        [Theory]
        [InlineData("com.chelpus.lackypatch")]
        [InlineData("com.chelpus.luckypatcher")]
        [InlineData("com.dimonvideo.luckypatcher")]
        [InlineData("com.android.vending.billing.InAppBillingService.COIN")]
        [InlineData("com.koushikdutta.rommanager")]
        [InlineData("com.koushikdutta.rommanager.license")]
        [InlineData("com.ramdroid.appquarantine")]
        [InlineData("com.ramdroid.appquarantinepro")]
        public void ShouldDetectIfDangerousAppsAreInstalled(string package)
        {
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.Android.JailbreakDetector(gel.Env, simulatorFriendly: false);

                A.App.Application.Context.PackageManager.MockPackages = new System.Collections.Generic.Dictionary<string, A.Content.PM.PackageInfo>
                {
                    { package, new A.Content.PM.PackageInfo() }
                };

                Assert.True(jbd.IsJailbroken);
            }
        }

        [Theory]
        [InlineData("com.amphoras.hidemyroot")]
        [InlineData("com.amphoras.hidemyrootadfree")]
        [InlineData("com.devadvance.rootcloak")]
        [InlineData("com.devadvance.rootcloakplus")]
        [InlineData("com.formyhm.hiderootPremium")]
        [InlineData("com.formyhm.hideroot")]
        [InlineData("com.saurik.substrate")]
        [InlineData("de.robv.android.xposed.installer")]
        [InlineData("com.zachspong.temprootremovejb")]
        public void ShouldDetectIfRootCloakingAppsAreInstalled(string package)
        {
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.Android.JailbreakDetector(gel.Env, simulatorFriendly: false);

                A.App.Application.Context.PackageManager.MockPackages = new System.Collections.Generic.Dictionary<string, A.Content.PM.PackageInfo>
                {
                    { package, new A.Content.PM.PackageInfo() }
                };

                Assert.True(jbd.IsJailbroken);
            }
        }

        private static Random randomizer = new Random();

        [Theory]
        [InlineData("su")]
        [InlineData("magisk")]
        public void ShouldDetectIfExecutableCanBeLocated(string executable)
        {
            var locations = Cryoprison.Android.JailbreakDetector.CommonExecutableRootDirectories;

            var index = randomizer.Next(locations.Length);
            var location = locations[index];

            using (var gel = new GlobalLock())
            {
                gel.Env.System.IO.File.Exists = (path) => {
                    return path == location + executable;
                };

                var jbd = new Cryoprison.Android.JailbreakDetector(gel.Env, simulatorFriendly: false);

                Assert.True(jbd.IsJailbroken);
            }
        }

        [Theory]
        [InlineData("/etc/")]
        [InlineData("/sbin/")]
        [InlineData("/system/")]
        [InlineData("/system/bin/")]
        [InlineData("/system/sbin/")]
        [InlineData("/system/xbin/")]
        [InlineData("/vendor/bin/")]
        public void ShouldDetectIfDirectoryIsWritable(string directory)
        {
            var filename = directory + "cryoprison";

            using (var gel = new GlobalLock())
            {
                var env = gel.Env;

                env.System.IO.Directory.Exists = (path) => {
                    return path == directory;
                };

                env.System.IO.File.Exists = (path) => {
                    return path == filename;
                };

                env.System.IO.File.Open = (path, mode, access, share) =>
                {
                    if (path == filename)
                    {
                        return new System.IO.MemoryStream(new byte[100]);
                    }
                    throw new AccessViolationException();
                };

                var jbd = new Cryoprison.Android.JailbreakDetector(env, simulatorFriendly: false);

                Assert.True(jbd.IsJailbroken);
            }
        }

        [Theory]
        [InlineData("[ro.debuggable]: [1]")]
        [InlineData("[ro.secure]=[0]")]
        public void ShouldDetectIfPropertyHasSpecificValue(string stdout)
        {
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.Android.JailbreakDetector(gel.Env, simulatorFriendly: false);

                Java.Lang.Runtime.MockRuntime.MockExec = (parms) => {
                    if (parms.Count() == 1 && parms[0] == "getprop")
                    {
                        var process = new Process();
                        process.MockExecArgs = parms;
                        process.MockStdOutput = new[] {
                            stdout
                        };
                        return process;
                    }
                    return null;
                };

                Assert.True(jbd.IsJailbroken, $"Did not detect a jailbreak via: {stdout}");
            }
        }
    }
}
