using System;
using Xunit;
using Cryoprison.Android.PlatformSpecific;
using System.Linq;
using Cryoprison.Test.Android.Mocks;
using A = Android;

namespace Cryoprison.Test.Android.PlatformSpecific
{
    public class ShouldNotHaveSpecificBuildTagsTests
    {
        [Fact]
        public void HasCorrectId()
        {
            using (var gel = new GlobalLock())
            {
                var env = new Ex.Env();

                var i = new ShouldNotHaveSpecificBuildTags().Init(env, "TEST", "fu=bar");

                Assert.Equal("SHOULD_NOT_HAVE_BUILD_TAG_TEST", i.Id);
            }
        }

        [Fact]
        public void DetectsBuildTag()
        {
            using (var gel = new GlobalLock())
            {
                var env = new Ex.Env();

                A.OS.Build.Tags = "fubar";

                var i = new ShouldNotHaveSpecificBuildTags().Init(env, "TEST", "fubar");

                Assert.False(i.Ok);
            }
        }

        [Fact]
        public void DetectsMissingPropValue()
        {
            using (var gel = new GlobalLock())
            {
                var env = new Ex.Env();

                A.OS.Build.Tags = "f00bar;asdf";

                var i = new ShouldNotHaveSpecificBuildTags().Init(env, "TEST", "fubar");

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

                var i = new ShouldNotHaveSpecificBuildTags().Init(env, "TEST", null);

                Assert.True(i.Ok);

                i = new ShouldNotHaveSpecificBuildTags().Init(env, "TEST", "");

                Assert.True(i.Ok);

                A.OS.Build.Tags = "f00bar;asdf";

                i = new ShouldNotHaveSpecificBuildTags().Init(env, "TEST", "");

                Assert.True(i.Ok);
            }
        }
    }
}
