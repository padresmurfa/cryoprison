using System;
using Xunit;
using Cryoprison;
using System.Collections.Generic;
using System.Linq;
using Mocks = Cryoprison.Test.Mocks;
using System.Threading;

namespace Cryoprison.Test
{
    public class JailbreakDetectorTests
    {
        [Fact]
        public void ByDefaultNothingIsInspected()
        {
            var d = new Mocks.JailbreakDetector();

            Assert.False(d.HasRun);

            Assert.False(d.IsJailbroken);
            Assert.Empty(d.Violations);

            Assert.True(d.HasRun);
        }

        [Fact]
        public void RefreshWorks()
        {
            var d = new Mocks.JailbreakDetector();

            Assert.False(d.HasRun);

            Assert.False(d.IsJailbroken);

            Assert.True(d.HasRun);

            d.Refresh();

            Assert.False(d.HasRun);
        }

        [Fact]
        public void AddInspectorsWorks()
        {
            var i1 = new Mocks.Inspector();
            var i2 = new Mocks.Inspector();

            var d = new Mocks.JailbreakDetector(new[] { i1, i2 });

            var inspectors = d.GetInspectors();

            Assert.Equal(2, inspectors.Count());
            Assert.Same(i1, inspectors.First());
            Assert.Same(i2, inspectors.Last());
        }

        [Fact]
        public void AddInspectorsIsRobust()
        {
            // 1. empty list
            var d = new Mocks.JailbreakDetector(new IInspector[0]);

            var inspectors = d.GetInspectors();

            Assert.Equal(0, inspectors.Count());

            // 2. null list
            d = new Mocks.JailbreakDetector(null);

            inspectors = d.GetInspectors();

            Assert.Equal(0, inspectors.Count());

            // 3. null in otherwise empty list
            d = new Mocks.JailbreakDetector(new IInspector[] { null });

            inspectors = d.GetInspectors();

            Assert.Equal(0, inspectors.Count());

            var i1 = new Mocks.Inspector();

            // 4. null in otherwise non-empty list
            d = new Mocks.JailbreakDetector(new IInspector[] { null, i1, null });

            inspectors = d.GetInspectors();

            Assert.Equal(1, inspectors.Count());

            Assert.Same(i1, inspectors.First());
        }

        [Fact]
        public void RunsInspectorsWhenNeeded()
        {
            // 1. IsJailbroken
            var inspector = new Mocks.Inspector();

            var d = new Mocks.JailbreakDetector(new[] { inspector });

            Assert.False(d.IsJailbroken);

            Assert.True(inspector.Ran);

            // 2. Violations
            inspector = new Mocks.Inspector();

            d = new Mocks.JailbreakDetector(new[] { inspector });

            Assert.Empty(d.Violations);

            Assert.True(inspector.Ran);
        }

        [Fact]
        public void DetectsJailbreaks()
        {
            var inspector = new Mocks.Inspector();

            inspector.Init("NOTOK", "asdf");

            var d = new Mocks.JailbreakDetector(new[] { inspector });

            Assert.True(d.IsJailbroken);
            Assert.Equal(1, d.Violations.Count());
            Assert.Equal("MOCK_NOTOK_ID", d.Violations.First());
        }

        [Fact]
        public void DetectsLackOfJailbreaks()
        {
            var inspector = new Mocks.Inspector();

            inspector.Init("OK", "asdf");

            var d = new Mocks.JailbreakDetector(new[] { inspector });

            Assert.False(d.IsJailbroken);
            Assert.Equal(0, d.Violations.Count());
        }

        [Fact]
        public void IsThreadsafeToDegreeReasonablyTestable()
        {
            // Violations returns a shallow-copied list.
            var inspector = new Mocks.Inspector();

            inspector.Init("OK", "asdf");

            var d = new Mocks.JailbreakDetector(new[] { inspector });

            var v1 = d.Violations;
            d.Refresh();
            var v2 = d.Violations;

            Assert.NotSame(v1, v2);

            // That's about as far as can be reasonably tested at the moment.
        }

        [Fact]
        public void ReportsJailbreaks()
        {
            var inspector = new Mocks.Inspector();

            inspector.Init("NOTOK", "asdf");

            var d = new Mocks.JailbreakDetector(new[] { inspector });

            string reportedJailbreak = null;
            Reporter.OnJailbreakReported = (j) =>
            {
                reportedJailbreak = j;
            };
            Assert.True(d.IsJailbroken);

            Assert.Equal("MOCK_NOTOK_ID", reportedJailbreak);
        }

        [Fact]
        public void DoesNotReportLackOfJailbreaks()
        {
            var inspector = new Mocks.Inspector();

            inspector.Init("OK", "asdf");

            var d = new Mocks.JailbreakDetector(new[] { inspector });

            string reportedJailbreak = null;
            Reporter.OnJailbreakReported = (j) =>
            {
                reportedJailbreak = j;
            };
            Assert.False(d.IsJailbroken);

            Assert.Null(reportedJailbreak);
        }

        [Fact]
        public void ReportsExceptions()
        {
            var inspector = new Mocks.InspectorThatBombsDuringRun();

            inspector.Init("OK", "asdf");

            var d = new Mocks.JailbreakDetector(new[] { inspector });

            string reportedMessage = null;
            Exception reportedException = null;

            Reporter.OnExceptionReported = (msg,ex) =>
            {
                reportedMessage = msg;
                reportedException = ex;
            };

            Assert.False(d.IsJailbroken);

            Assert.Equal("Inspector crashed: MOCK_OK_ID", reportedMessage);
        }
    }
}
