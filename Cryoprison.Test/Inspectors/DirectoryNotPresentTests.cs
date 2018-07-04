using System;
using Cryoprison;
using Xunit;
using Cryoprison.Inspectors;

namespace Cryoprison.Test.Inspectors
{
    public class DirectoryNotPresentTests
    {
        [Fact]
        public void HasCorrectId()
        {
            var env = new Ex.Env();

            var i = new DirectoryNotPresent().Init(env, "TEST", "directory");

            Assert.Equal("DIRECTORY_TEST_SHOULD_NOT_BE_PRESENT", i.Id);
        }

        [Fact]
        public void DetectsDirectoryPresent()
        {
            var env = new Ex.Env();

            env.System.IO.Directory.Exists = (path) => {
                return path == "directory";
            };

            var i = new DirectoryNotPresent().Init(env, "TEST", "directory");

            Assert.False(i.Ok);
        }

        [Fact]
        public void DetectsDirectoryNotPresent()
        {
            var env = new Ex.Env();

            env.System.IO.Directory.Exists = (path) => {
                return false;
            };

            var i = new DirectoryNotPresent().Init(env, "TEST", "directory");

            Assert.True(i.Ok);
        }

        [Fact]
        public void IsRobust()
        {
            var env = new Ex.Env();

            env.System.IO.Directory.Exists = (path) => {
                throw new Exception("fubar");
            };

            string message = null;
            Exception exception = null;
            env.Reporter.OnExceptionReported = (msg, ex) =>
            {
                message = msg;
                exception = ex;
            };

            var i = new DirectoryNotPresent().Init(env, "TEST", "directory");

            Assert.True(i.Ok);
            Assert.Equal("IsDirectoryPresent bombed for directory", message);
            Assert.Equal("fubar", exception.Message);
        }
    }
}
