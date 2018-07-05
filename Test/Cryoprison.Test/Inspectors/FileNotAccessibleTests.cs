using System;
using Cryoprison;
using Xunit;
using Cryoprison.Inspectors;
using System.IO;

namespace Cryoprison.Test.Inspectors
{
    public class FileNotAccessibleTests
    {
        [Fact]
        public void HasCorrectId()
        {
            var env = new Ex.Env();

            var i = new FileNotAccessible().Init(env, "TEST", "file");

            Assert.Equal("FILE_TEST_SHOULD_NOT_BE_ACCESSIBLE", i.Id);
        }

        [Fact]
        public void DetectsFileAccessible()
        {
            var env = new Ex.Env();

            env.System.IO.File.Open = (path, mode, access, share) => {
                return new MemoryStream();
            };

            var i = new FileNotAccessible().Init(env, "TEST", "file");

            Assert.False(i.Ok);
        }

        [Fact]
        public void DetectsFileNotFound()
        {
            var env = new Ex.Env();

            env.System.IO.File.Open = (path, mode, access, share) => {
                throw new FileNotFoundException();
            };

            var i = new FileNotAccessible().Init(env, "TEST", "file");

            Assert.True(i.Ok);
        }

        [Fact]
        public void DetectsFileNotReadable()
        {
            var env = new Ex.Env();

            env.System.IO.File.Open = (path, mode, access, share) => {
                throw new System.UnauthorizedAccessException("Cannot open in read mode");
            };

            var i = new FileNotAccessible().Init(env, "TEST", "file");

            Assert.True(i.Ok);
        }

        [Fact]
        public void IsRobust()
        {
            var env = new Ex.Env();

            env.System.IO.File.Open = (path, mode, access, share) => {
                throw new Exception("fubar");
            };

            string message = null;
            Exception exception = null;
            env.Reporter.OnExceptionReported = (msg, ex) =>
            {
                message = msg;
                exception = ex;
            };

            var i = new FileNotAccessible().Init(env, "TEST", "file");

            Assert.True(i.Ok);
            Assert.Equal("IsFileAccessible bombed for file", message);
            Assert.Equal("fubar", exception.Message);
        }
    }
}
