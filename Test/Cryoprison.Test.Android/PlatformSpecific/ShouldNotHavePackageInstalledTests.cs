using System;
using Xunit;
using Cryoprison.Android.PlatformSpecific;
using System.Linq;
using A=Android;
using System.Collections.Generic;

namespace Cryoprison.Test.Android.PlatformSpecific
{
    public class ShouldNotHavePackageInstalledTests
    {
        [Fact]
        public void HasCorrectId()
        {
            using (var gel = new GlobalLock())
            {
                var env = new Ex.Env();

                var i = new ShouldNotHavePackageInstalled().Init(env, "TEST", "file");

                Assert.Equal("SHOULD_NOT_HAVE_PACKAGE_INSTALLED_TEST", i.Id);
            }
        }

        [Fact]
        public void DetectsPackageInstalled()
        {
            using (var gel = new GlobalLock())
            {
                var env = new Ex.Env();

                var context = A.App.Application.Context;

                context.PackageManager.MockPackages = new Dictionary<string, A.Content.PM.PackageInfo> {
                    { "package", new A.Content.PM.PackageInfo() }
                };

                var i = new ShouldNotHavePackageInstalled().Init(env, "TEST", "package");

                Assert.False(i.Ok);
            }
        }

        [Fact]
        public void DetectsPackagetNotInstalled()
        {
            using (var gel = new GlobalLock())
            {
                var env = new Ex.Env();

                var i = new ShouldNotHavePackageInstalled().Init(env, "TEST", "package");

                Assert.True(i.Ok);
            }
        }

        [Fact]
        public void IsRobust()
        {
            using (var gel = new GlobalLock())
            {
                var env = new Ex.Env();

                string message = null;
                Exception exception = null;
                env.Reporter.OnExceptionReported = (msg, ex) =>
                {
                    message = msg;
                    exception = ex;
                };

                var context = A.App.Application.Context;

                context.PackageManager.MockGetPackageInfo = (A, B) =>
                {
                    throw new Exception("BOOM!");
                };

                var i = new ShouldNotHavePackageInstalled().Init(env, "TEST", "package");

                Assert.True(i.Ok);
                Assert.Equal("HasPackageInstalled bombed for package", message);
                Assert.Equal("BOOM!", exception.Message);
            }
        }
    }
}
