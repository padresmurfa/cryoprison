using System;
using Xunit;
using Cryoprison;
using System.Linq;
using Mocks = Cryoprison.Test.Mocks;

namespace Cryoprison.Test
{
    public class ChecksTests
    {
        [Fact]
        public void InitiallyEmpty()
        {
            var checks = new Checks();

            Assert.Empty(checks.GetInspectors<Mocks.Inspector>());
        }

        [Theory]
        [InlineData("ch1", "v1", "MOCK_CH1_ID")]
        [InlineData("ch2", "v2", "MOCK_CH2_ID")]
        public void CanAdd(string checkId, string val, string id)
        {
            var checks = new Checks();

            var checks2 = checks.Add(checkId, val);

            Assert.Same(checks, checks2);

            var inspectors = checks.GetInspectors<Mocks.Inspector>();

            Assert.NotEmpty(inspectors);

            var check = inspectors.First();

            Assert.Equal(id, check.Id);
            Assert.False(check.Ok);
        }

        [Theory]
        [InlineData("ch1", "v1", "MOCK_CH1_ID", false)]
        [InlineData("Ok1", "v1", "MOCK_OK1_ID", true)]
        public void CanCheckForOk(string checkId, string val, string id, bool ok)
        {
            var checks = new Checks().Add(checkId, val);

            var inspectors = checks.GetInspectors<Mocks.Inspector>();

            var check = inspectors.First();

            Assert.Equal(ok, check.Ok);
        }

        [Theory]
        [InlineData("checkId", "MOCK_CHECKID_ID")]
        [InlineData("CheckId", "MOCK_CHECKID_ID")]
        public void UppercasesIds(string checkId, string id)
        {
            var checks = new Checks().Add(checkId, "asdf");

            var inspectors = checks.GetInspectors<Mocks.Inspector>();

            var check = inspectors.First();

            Assert.Equal(id, check.Id);
        }

        [Theory]
        [InlineData("root1", "root2")]
        public void PrependsAllRoots(params string[] roots)
        {
            var checks = new Checks().AddRoots(roots).Add("check", "/asdf");

            var inspectors = checks.GetInspectors<Mocks.Inspector>().ToList();

            Assert.Equal(roots.Length, inspectors.Count());

            for (var i = 0; i < roots.Length; ++i)
            {
                var inspector = inspectors[i] as Mocks.Inspector;
                Assert.Equal("MOCK_CHECK_ID", inspector.Id);

                var v = roots[i] + "/asdf";
                Assert.Equal(v, inspector.Value);
            }
        }

        [Theory]
        [InlineData("check1","check2")]
        public void PrependsRootToAllChecks(params string[] checkIds)
        {
            var checks = new Checks().AddRoots("root");

            foreach (var checkId in checkIds)
            {
                checks.Add(checkId, "/" + checkId + ".asdf");
            }

            var inspectors = checks.GetInspectors<Mocks.Inspector>().ToList();

            Assert.Equal(checkIds.Length, inspectors.Count());

            for (var i = 0; i < checkIds.Length; ++i)
            {
                var inspector = inspectors[i] as Mocks.Inspector;
                Assert.Equal("MOCK_" + checkIds[i].ToUpperInvariant() + "_ID", inspector.Id);

                var v = "root/" + checkIds[i] + ".asdf";
                Assert.Equal(v, inspector.Value);
            }
        }

        [Theory]
        [InlineData()]
        [InlineData(null)]
        [InlineData("")]
        public void AddRootsIsRobust(params string[] roots)
        {
            var checks = new Checks().AddRoots(roots);

            var inspectors = checks.GetInspectors<Mocks.Inspector>().ToList();

            Assert.Empty(inspectors);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(null,null)]
        [InlineData(null,"")]
        [InlineData("checkId", null)]
        [InlineData("checkId", "")]
        [InlineData("checkId", "root")]
        [InlineData("checkId", "root", null)]
        [InlineData("checkId", "root", "")]
        public void AddIsRobust(string checkId, params string[] roots)
        {
            var expectedCount = (roots ?? new string[0]).Count(x => !string.IsNullOrEmpty(x));

            var checks = new Checks().Add(checkId, roots);
            var inspectors = checks.GetInspectors<Mocks.Inspector>().ToList();
            Assert.Equal(expectedCount, inspectors.Count());

            // a bit redundant to do this for every set of inline data, but
            // double check that we are robust against no roots.
            checks = new Checks().Add(checkId);
            inspectors = checks.GetInspectors<Mocks.Inspector>().ToList();
            Assert.Equal(0, inspectors.Count());
        }

        [Fact]
        public void GetInspectorsIsRobust()
        {
            var checks = new Checks();
            var inspectors = checks.GetInspectors<Mocks.Inspector>().ToList();
            Assert.Equal(0, inspectors.Count());

            checks = new Checks().Add("fubar", "fubar").Add("carry","on");

            // bombs during construction, so all are doomed.
            inspectors = checks.GetInspectors<Mocks.InspectorThatBombsDuringConstruction>().ToList();
            Assert.Equal(0, inspectors.Count());

            // bombds during initialization, so only fubar is doomed.
            inspectors = checks.GetInspectors<Mocks.InspectorThatBombsDuringInit>().ToList();
            Assert.Equal(1, inspectors.Count());
        }
    }
}
