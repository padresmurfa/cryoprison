using System;
using Cryoprison;
using Xunit;
using System.IO;
using Cryoprison.Test.Mocks;

namespace Cryoprison.Test.Inspectors
{
    public class UrlNotOpenableTests
    {
        [Fact]
        public void HasCorrectId()
        {
            var env = new Ex.Env();

            var i = new UrlNotOpenable().Init(env, "TEST", "url");

            Assert.Equal("URL_TEST_SHOULD_NOT_BE_OPENABLE", i.Id);
        }

        [Fact]
        public void DetectsOpenableUrl()
        {
            var env = new Ex.Env();

            var i = new UrlNotOpenable
            {
                CanOpen = true
            }.Init(env, "TEST", "url");

            Assert.False(i.Ok);
        }

        [Fact]
        public void DetectsNotOpenableUrl()
        {
            var env = new Ex.Env();

            var i = new UrlNotOpenable
            {
                CanOpen = false
            }.Init(env, "TEST", "url");

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

            var i = new UrlNotOpenable
            {
                ThrowException = new Exception("fubar")
            }.Init(env, "TEST", "url");

            Assert.True(i.Ok);
            Assert.Equal("IsUrlOpenable bombed for url", message);
            Assert.Equal("BOOM!", exception.Message);
            Assert.Equal("fubar", exception.InnerException.Message);
        }
    }
}
