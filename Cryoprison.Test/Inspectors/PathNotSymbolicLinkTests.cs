using System;
using Cryoprison;
using Xunit;
using System.IO;
using Cryoprison.Test.Mocks;

namespace Cryoprison.Test.Inspectors
{
    public class PathNotSymbolicLinkTests
    {
        [Fact]
        public void HasCorrectId()
        {
            var env = new Ex.Env();

            var i = new PathNotSymbolicLink().Init(env, "TEST", "file");

            Assert.Equal("PATH_TEST_SHOULD_NOT_REFER_TO_SYMBOLIC_LINK", i.Id);
        }

        [Fact]
        public void DetectsSymbolicLink()
        {
            var env = new Ex.Env();

            var i = new PathNotSymbolicLink {
                SymbolicLink = true
            }.Init(env, "TEST", "file");

            Assert.False(i.Ok);
        }

        [Fact]
        public void DetectsNotSymbolicLink()
        {
            var env = new Ex.Env();

            var i = new PathNotSymbolicLink
            {
                SymbolicLink = false
            }.Init(env, "TEST", "file");

            Assert.True(i.Ok);
        }

        [Fact]
        public void IsRobust()
        {
            var env = new Ex.Env();

            string message = null;
            Exception exception = null;
            env.Reporter.OnExceptionReported = (msg, ex) =>
            {
                message = msg;
                exception = ex;
            };

            var i = new PathNotSymbolicLink
            {
                ThrowException = new Exception("fubar")
            }.Init(env, "TEST", "file");

            Assert.True(i.Ok);
            Assert.Equal("DoesPathPointToSymbolicLink bombed for file", message);
            Assert.Equal("BOOM!", exception.Message);
            Assert.Equal("fubar", exception.InnerException.Message);
        }
    }
}
