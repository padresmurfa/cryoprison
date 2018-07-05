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
            var env = new Ex.Env();

            var d = new Mocks.JailbreakDetector(env);

            Assert.False(d.HasRun);

            Assert.False(d.IsJailbroken);
            Assert.Empty(d.Violations);

            Assert.True(d.HasRun);
        }

        [Fact]
        public void RefreshWorks()
        {
            var env = new Ex.Env();

            var d = new Mocks.JailbreakDetector(env);

            Assert.False(d.HasRun);

            Assert.False(d.IsJailbroken);

            Assert.True(d.HasRun);

            d.Refresh();

            Assert.False(d.HasRun);
        }

        [Fact]
        public void AddInspectorsWorks()
        {
            var env = new Ex.Env();

            var i1 = new Mocks.Inspector();
            var i2 = new Mocks.Inspector();

            var d = new Mocks.JailbreakDetector(env, new[] { i1, i2 });

            var inspectors = d.GetInspectors();

            Assert.Equal(2, inspectors.Count());
            Assert.Same(i1, inspectors.First());
            Assert.Same(i2, inspectors.Last());
        }

        [Fact]
        public void AddInspectorsIsRobust()
        {
            var env = new Ex.Env();

            // 1. empty list
            var d = new Mocks.JailbreakDetector(env, new IInspector[0]);

            var inspectors = d.GetInspectors();

            Assert.Equal(0, inspectors.Count());

            // 2. null list
            d = new Mocks.JailbreakDetector(null);

            inspectors = d.GetInspectors();

            Assert.Equal(0, inspectors.Count());

            // 3. null in otherwise empty list
            d = new Mocks.JailbreakDetector(env, new IInspector[] { null });

            inspectors = d.GetInspectors();

            Assert.Equal(0, inspectors.Count());

            var i1 = new Mocks.Inspector();

            // 4. null in otherwise non-empty list
            d = new Mocks.JailbreakDetector(env, new IInspector[] { null, i1, null });

            inspectors = d.GetInspectors();

            Assert.Equal(1, inspectors.Count());

            Assert.Same(i1, inspectors.First());
        }

        [Fact]
        public void RunsInspectorsWhenNeeded()
        {
            var env = new Ex.Env();

            // 1. IsJailbroken
            var inspector = new Mocks.Inspector();

            var d = new Mocks.JailbreakDetector(env, new[] { inspector });

            Assert.False(d.IsJailbroken);

            Assert.True(inspector.Ran);

            // 2. Violations
            inspector = new Mocks.Inspector();

            d = new Mocks.JailbreakDetector(env, new[] { inspector });

            Assert.Empty(d.Violations);

            Assert.True(inspector.Ran);
        }

        [Fact]
        public void DetectsJailbreaks()
        {
            var env = new Ex.Env();

            var inspector = new Mocks.Inspector();

            inspector.Init(env, "NOTOK", "asdf");

            var d = new Mocks.JailbreakDetector(env, new[] { inspector });

            Assert.True(d.IsJailbroken);
            Assert.Equal(1, d.Violations.Count());
            Assert.Equal("MOCK_NOTOK_ID", d.Violations.First());
        }

        [Fact]
        public void DetectsLackOfJailbreaks()
        {
            var env = new Ex.Env();

            var inspector = new Mocks.Inspector();

            inspector.Init(env, "OK", "asdf");

            var d = new Mocks.JailbreakDetector(env, new[] { inspector });

            Assert.False(d.IsJailbroken);
            Assert.Equal(0, d.Violations.Count());
        }

        [Fact]
        public void IsThreadsafeToDegreeReasonablyTestable()
        {
            var env = new Ex.Env();

            // Violations returns a shallow-copied list.
            var inspector = new Mocks.Inspector();

            inspector.Init(env, "OK", "asdf");

            var d = new Mocks.JailbreakDetector(env, new[] { inspector });

            var v1 = d.Violations;
            d.Refresh();
            var v2 = d.Violations;

            Assert.NotSame(v1, v2);

            // That's about as far as can be reasonably tested at the moment.
        }

        [Fact]
        public void ReportsJailbreaks()
        {
            var env = new Ex.Env();

            var inspector = new Mocks.Inspector();

            inspector.Init(env, "NOTOK", "asdf");

            var d = new Mocks.JailbreakDetector(env, new[] { inspector });

            string reportedJailbreak = null;
            env.Reporter.OnJailbreakReported = (j) =>
            {
                reportedJailbreak = j;
            };
            Assert.True(d.IsJailbroken);

            Assert.Equal("MOCK_NOTOK_ID", reportedJailbreak);
        }

        [Fact]
        public void DoesNotReportLackOfJailbreaks()
        {
            var env = new Ex.Env();

            var inspector = new Mocks.Inspector();

            inspector.Init(env, "OK", "asdf");

            var d = new Mocks.JailbreakDetector(env, new[] { inspector });

            string reportedJailbreak = null;
            env.Reporter.OnJailbreakReported = (j) =>
            {
                reportedJailbreak = j;
            };
            Assert.False(d.IsJailbroken);

            Assert.Null(reportedJailbreak);
        }

        [Fact]
        public void ReportsExceptions()
        {
            var env = new Ex.Env();

            var inspector = new Mocks.InspectorThatBombsDuringRun();

            inspector.Init(env, "OK", "asdf");

            var d = new Mocks.JailbreakDetector(env, new[] { inspector });

            string reportedMessage = null;
            Exception reportedException = null;

            env.Reporter.OnExceptionReported = (msg,ex) =>
            {
                reportedMessage = msg;
                reportedException = ex;
            };

            Assert.False(d.IsJailbroken);

            Assert.Equal("Inspector crashed: MOCK_OK_ID", reportedMessage);
        }
    }
}
