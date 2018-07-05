using System;
using Xunit;
using Cryoprison.Android.PlatformSpecific;
using System.Linq;
using Cryoprison.Test.Android.Mocks;

namespace Cryoprison.Test.Android.PlatformSpecific
{
    public class ShouldNotBeAbleToLocateFileTests
    {
        [Fact]
        public void HasCorrectId()
        {
            using (var gel = new GlobalLock())
            {
                var env = new Ex.Env();

                var i = new ShouldNotBeAbleToLocateFile().Init(env, "TEST", "file");

                Assert.Equal("SHOULD_NOT_LOCATE_TEST", i.Id);
            }
        }

        [Fact]
        public void DetectsFileLocatable()
        {
            using (var gel = new GlobalLock())
            {
                var env = new Ex.Env();

                var runtime = Java.Lang.Runtime.GetRuntime();

                Java.Lang.Process process = null;
                runtime.MockExec = (p) =>
                {
                    if (p.Count() == 2 && p[0] == "which" && p[1] == "file")
                    {
                        process = new Java.Lang.Process();
                        process.MockExecArgs = p;
                        process.MockStdOutput = new[] { "/found/it/file" };
                        return process;
                    }
                    throw new Exception($"Could not execute: {p}");
                };

                var i = new ShouldNotBeAbleToLocateFile().Init(env, "TEST", "file");

                Assert.False(i.Ok);

                Assert.NotNull(process);
                Assert.True(process.MockDestroyed);
                Assert.True(process.MockDisposed);

                Assert.Equal(2, process.MockExecArgs.Count());
                Assert.Equal("which", process.MockExecArgs.First());
                Assert.Equal("file", process.MockExecArgs.Last());
            }
        }

        [Fact]
        public void DetectsFileNotLocatable()
        {
            using (var gel = new GlobalLock())
            {
                var env = new Ex.Env();

                var runtime = Java.Lang.Runtime.GetRuntime();

                Java.Lang.Process process = null;
                runtime.MockExec = (p) =>
                {
                    if (p.Count() == 2 && p[0] == "which" && p[1] == "file")
                    {
                        process = new Java.Lang.Process();
                        process.MockExecArgs = p;
                        process.MockStdOutput = new[] { "" };
                        return process;
                    }
                    throw new Exception($"Could not execute: {p}");
                };

                var i = new ShouldNotBeAbleToLocateFile().Init(env, "TEST", "file");

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

                var runtime = Java.Lang.Runtime.GetRuntime();

                runtime.MockExec = (p) =>
                {
                    throw new Exception("BOOM!");
                };

                var i = new ShouldNotBeAbleToLocateFile().Init(env, "TEST", "file");

                Assert.True(i.Ok);
                Assert.Equal("CanLocateFile bombed for file", message);
                Assert.Equal("BOOM!", exception.Message);
            }
        }
    }
}
