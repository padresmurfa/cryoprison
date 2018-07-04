using System;
using Cryoprison;
using Xunit;
using Cryoprison.Inspectors;
using System.IO;

namespace Cryoprison.Test.Inspectors
{
    public class FileNotPresentTests
    {
        [Fact]
        public void HasCorrectId()
        {
            var env = new Ex.Env();

            var i = new FileNotPresent().Init(env, "TEST", "file");

            Assert.Equal("FILE_TEST_SHOULD_NOT_BE_PRESENT", i.Id);
        }

        [Fact]
        public void DetectsFilePresent()
        {
            var env = new Ex.Env();

            env.System.IO.File.Exists = (path) => {
                return true;
            };

            var i = new FileNotPresent().Init(env, "TEST", "file");

            Assert.False(i.Ok);
        }

        [Fact]
        public void DetectsFileNotPresent()
        {
            var env = new Ex.Env();

            env.System.IO.File.Exists = (path) => {
                return false;
            };

            var i = new FileNotPresent().Init(env, "TEST", "file");

            Assert.True(i.Ok);
        }

        [Fact]
        public void DetectsDirectoryNotPresent()
        {
            var env = new Ex.Env();

            env.System.IO.File.Exists = (path) => {
                throw new DirectoryNotFoundException();
            };

            var i = new FileNotPresent().Init(env, "TEST", "file");

            Assert.True(i.Ok);
        }

        [Fact]
        public void IsRobust()
        {
            var env = new Ex.Env();

            env.System.IO.File.Exists = (path) => {
                throw new Exception("fubar");
            };

            string message = null;
            Exception exception = null;
            env.Reporter.OnExceptionReported = (msg, ex) =>
            {
                message = msg;
                exception = ex;
            };

            var i = new FileNotPresent().Init(env, "TEST", "file");

            Assert.True(i.Ok);
            Assert.Equal("IsFilePresent bombed for file", message);
            Assert.Equal("fubar", exception.Message);
        }
    }
}
