using System;
using Xunit;
using Cryoprison.Android.PlatformSpecific;
using System.Linq;

namespace Cryoprison.Test.Android.PlatformSpecific
{
    public class ShouldNotHavePropValuesTests
    {
        [Fact]
        public void HasCorrectId()
        {
            using (var gel = new GlobalLock())
            {
                var env = new Ex.Env();

                var i = new ShouldNotHavePropValues().Init(env, "TEST", "fu=bar");

                Assert.Equal("SHOULD_NOT_HAVE_PROP_VALUES_TEST", i.Id);
            }
        }

        [Fact]
        public void DetectsPropValue()
        {
            using (var gel = new GlobalLock())
            {
                var env = new Ex.Env();

                var runtime = Java.Lang.Runtime.GetRuntime();

                Java.Lang.Process process = null;
                runtime.MockExec = (p) =>
                {
                    if (p.Count() == 1 && p[0] == "getprop")
                    {
                        process = new Java.Lang.Process();
                        process.MockExecArgs = p;
                        process.MockStdOutput = new[] {
                            "[fu.bar]: [fubar]"
                        };
                        return process;
                    }
                    throw new Exception($"Could not execute: {p}");
                };

                var i = new ShouldNotHavePropValues().Init(env, "TEST", "fu.bar=fubar");

                Assert.False(i.Ok);

                Assert.NotNull(process);
                Assert.True(process.MockDestroyed);
                Assert.True(process.MockDisposed);
            }
        }

        [Fact]
        public void DetectsMissingPropValue()
        {
            using (var gel = new GlobalLock())
            {
                var env = new Ex.Env();

                var runtime = Java.Lang.Runtime.GetRuntime();

                Java.Lang.Process process = null;
                runtime.MockExec = (p) =>
                {
                    if (p.Count() == 1 && p[0] == "getprop")
                    {
                        process = new Java.Lang.Process();
                        process.MockExecArgs = p;
                        process.MockStdOutput = new[] {
                            "[fu.bar]: [foobarr]",
                            "[fu.bar]: [fubar]"
                        };
                        return process;
                    }
                    throw new Exception($"Could not execute: {p}");
                };

                var i = new ShouldNotHavePropValues().Init(env, "TEST", "fu.bar=foobar");
                Assert.True(i.Ok);

                i = new ShouldNotHavePropValues().Init(env, "TEST", "foo.bar=fubar");
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

                Java.Lang.Process process = null;
                runtime.MockExec = (p) =>
                {
                    if (p.Count() == 1 && p[0] == "getprop")
                    {
                        process = new Java.Lang.Process();
                        process.MockExecArgs = p;
                        process.MockStdOutput = new[] {
                            "[fu.bar]: [fubar]"
                        };
                        return process;
                    }
                    throw new Exception($"Could not execute: {p}");
                };

                var i = new ShouldNotHavePropValues().Init(env, "TEST", "fubar");

                Assert.True(i.Ok);
                Assert.Equal("ShouldNotHavePropValues init bombed for TEST=fubar", message);
                Assert.Equal(exception.GetType().FullName, typeof(ArgumentException).FullName);
                Assert.Equal("fubar", exception.Message);

                runtime.MockExec = (p) =>
                {
                    throw new Exception("BOOM!");
                };

                i = new ShouldNotHavePropValues().Init(env, "TEST", "fu.bar=fubar");

                Assert.True(i.Ok);
                Assert.Equal("HasPropValue bombed for TEST (fu.bar=fubar)", message);
                Assert.Equal("BOOM!", exception.Message);
            }
        }
    }
}
