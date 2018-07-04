using System;
using Cryoprison;
using Xunit;
using Cryoprison.Inspectors;
using System.IO;

namespace Cryoprison.Test.Inspectors
{
    public class FileNotDestructivelyWritableTets
    {
        [Fact]
        public void HasCorrectId()
        {
            var env = new Ex.Env();

            var i = new FileNotDestructivelyWritable().Init(env, "TEST", "file");

            Assert.Equal("FILE_TEST_SHOULD_NOT_BE_DESTRUCTIVELY_WRITABLE", i.Id);
        }

        [Fact]
        public void DetectsFileWritable()
        {
            var env = new Ex.Env();

            env.System.IO.File.Open = (path, mode, access, share) => {
                return new MemoryStream(new byte[100]);
            };

            var deleted = false;
            env.System.IO.File.Delete = (path) => {
                deleted = true;
            };

            var i = new FileNotDestructivelyWritable().Init(env, "TEST", "file");

            Assert.False(i.Ok);
            Assert.True(deleted);
        }

        [Fact]
        public void DetectsDirectoryNotFound()
        {
            var env = new Ex.Env();

            env.System.IO.File.Open = (path, mode, access, share) => {
                throw new DirectoryNotFoundException();
            };

            var i = new FileNotDestructivelyWritable().Init(env, "TEST", "file");

            Assert.True(i.Ok);
        }

        [Fact]
        public void DetectsFileNotWritable()
        {
            var env = new Ex.Env();

            env.System.IO.File.Open = (path, mode, access, share) => {
                throw new System.UnauthorizedAccessException("Cannot open in write mode");
            };

            var i = new FileNotDestructivelyWritable().Init(env, "TEST", "file");

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

            env.System.IO.File.Open = (path, mode, access, share) => {
                throw new Exception("fubar");
            };

            var deleted = false;
            env.System.IO.File.Delete = (path) => {
                deleted = true;
            };

            var i = new FileNotDestructivelyWritable().Init(env, "TEST", "file");

            Assert.True(i.Ok);
            Assert.Equal("IsFileDestructivelyWritable bombed for file", message);
            Assert.Equal("fubar", exception.Message);
            Assert.True(deleted);
        }
    }
}
