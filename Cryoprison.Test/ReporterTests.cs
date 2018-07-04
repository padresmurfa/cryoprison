using System;
using Xunit;
using Cryoprison;

namespace Cryoprison.Test
{
    public class ReporterTests
    {
        // NOTE: the reporter is intended as a global debugging hook, so injecting
        //       it was not really intended.  This of course makes unit testing
        //       flaky, if tests are run in parallel, as different tests may
        //       decide to meddle with the global hooks at the same time.
        // TODO: Reconsider, since we are using a jailbreak detector instance
        //       that is instantiated at the same place we probably set up our
        //       debug hooks.
        [Fact]
        public void IsNotSetByDefault()
        {
            var env = new Ex.Env();

            Assert.Null(env.Reporter.OnExceptionReported);
            Assert.Null(env.Reporter.OnJailbreakReported);
        }

        [Fact]
        public void IsOptional()
        {
            var env = new Ex.Env();

            try
            {
                env.Reporter.ReportException("asdf", new Exception());
                env.Reporter.ReportJailbreak("asdf");
            }
            catch (Exception ex)
            {
                // Should never reach this
                Assert.Null(ex);
            }
        }

        [Fact]
        public void ReportsExceptions()
        {
            var env = new Ex.Env();

            string reportedLocation = null;
            Exception reportedException = null;

            env.Reporter.OnExceptionReported = (l, e) => {
                reportedLocation = l;
                reportedException = e;
            };

            var loc = "location";
            var ex = new Exception("exception");

            env.Reporter.ReportException(loc, ex);

            Assert.Same(loc, reportedLocation);
            Assert.Same(ex, reportedException);
        }

        [Fact]
        public void ReportsJailbreaks()
        {
            var env = new Ex.Env();

            string reportedJailbreak = null;

            env.Reporter.OnJailbreakReported = (j) => {
                reportedJailbreak = j;
            };

            var jailbreakId = "jailbreak";

            env.Reporter.ReportJailbreak(jailbreakId);

            Assert.Same(jailbreakId, reportedJailbreak);
        }

        [Fact]
        public void ReportExceptionsIsRobust()
        {
            var env = new Ex.Env();

            string reportedLocation = null;
            Exception reportedException = null;

            env.Reporter.OnExceptionReported = (l, e) => {
                reportedLocation = l;
                reportedException = e;
            };

            var loc = "location";
            var ex = new Exception("exception");

            env.Reporter.ReportException(loc, null);

            Assert.Same(loc, reportedLocation);
            Assert.Equal("Unexpected exception", reportedException.Message);

            reportedLocation = null;
            reportedException = null;

            env.Reporter.ReportException(null, ex);

            Assert.Same(ex, reportedException);
            Assert.Equal("<unknown location>", reportedLocation);

            reportedLocation = null;
            reportedException = null;

            env.Reporter.ReportException(null, null);

            Assert.Equal("Unexpected exception", reportedException.Message);
            Assert.Equal("<unknown location>", reportedLocation);

            env.Reporter.OnExceptionReported = (l, e) => {
                throw new Exception("fubar");
            };

            try
            {
                env.Reporter.ReportException(null, null);
            }
            catch (Exception x)
            {
                // should be non-reachable code
                Assert.Null(x);
            }
        }

        [Fact]
        public void ReportJailbreakIsRobust()
        {
            string reportedJailbreak = null;

            var env = new Ex.Env();

            env.Reporter.OnJailbreakReported = (j) => { reportedJailbreak = j; };

            env.Reporter.ReportJailbreak(null);

            Assert.Equal("unidentified jailbreak", reportedJailbreak);

            env.Reporter.OnJailbreakReported = (j) => { throw new Exception("fubar"); };

            try
            {
                env.Reporter.ReportJailbreak("jailbreak");
            }
            catch (Exception x)
            {
                // should be non-reachable code
                Assert.Null(x);
            }

        }
    }
}
