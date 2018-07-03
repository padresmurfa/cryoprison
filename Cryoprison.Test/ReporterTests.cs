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
            Assert.Null(Reporter.OnExceptionReported);
            Assert.Null(Reporter.OnJailbreakReported);
        }

        [Fact]
        public void IsOptional()
        {
            try
            {
                Reporter.ReportException("asdf", new Exception());
                Reporter.ReportJailbreak("asdf");
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
            string reportedLocation = null;
            Exception reportedException = null;

            Reporter.OnExceptionReported = (l, e) => {
                reportedLocation = l;
                reportedException = e;
            };

            var loc = "location";
            var ex = new Exception("exception");

            Reporter.ReportException(loc, ex);

            Assert.Same(loc, reportedLocation);
            Assert.Same(ex, reportedException);
        }

        [Fact]
        public void ReportsJailbreaks()
        {
            string reportedJailbreak = null;

            Reporter.OnJailbreakReported = (j) => {
                reportedJailbreak = j;
            };

            var jailbreakId = "jailbreak";

            Reporter.ReportJailbreak(jailbreakId);

            Assert.Same(jailbreakId, reportedJailbreak);
        }

        [Fact]
        public void ReportExceptionsIsRobust()
        {
            string reportedLocation = null;
            Exception reportedException = null;

            Reporter.OnExceptionReported = (l, e) => {
                reportedLocation = l;
                reportedException = e;
            };

            var loc = "location";
            var ex = new Exception("exception");

            Reporter.ReportException(loc, null);

            Assert.Same(loc, reportedLocation);
            Assert.Equal("Unexpected exception", reportedException.Message);

            reportedLocation = null;
            reportedException = null;

            Reporter.ReportException(null, ex);

            Assert.Same(ex, reportedException);
            Assert.Equal("<unknown location>", reportedLocation);

            reportedLocation = null;
            reportedException = null;

            Reporter.ReportException(null, null);

            Assert.Equal("Unexpected exception", reportedException.Message);
            Assert.Equal("<unknown location>", reportedLocation);

            Reporter.OnExceptionReported = (l, e) => {
                throw new Exception("fubar");
            };

            try
            {
                Reporter.ReportException(null, null);
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

            Reporter.OnJailbreakReported = (j) => { reportedJailbreak = j; };

            Reporter.ReportJailbreak(null);

            Assert.Equal("unidentified jailbreak", reportedJailbreak);

            Reporter.OnJailbreakReported = (j) => { throw new Exception("fubar"); };

            try
            {
                Reporter.ReportJailbreak("jailbreak");
            }
            catch (Exception x)
            {
                // should be non-reachable code
                Assert.Null(x);
            }

        }
    }
}
