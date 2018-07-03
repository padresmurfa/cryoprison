using System;
using Xunit;
using Cryoprison;

namespace Cryoprison.Test
{
    public class CheckTests
    {
        [Fact]
        public void ConsistentInitialValues()
        {
            var check = new Check();

            Assert.Null(check.CheckId);
            Assert.Null(check.Value);
        }

        [Theory]
        [InlineData("asdf")]
        [InlineData("fdassdf")]
        public void CanSetCheckId(string checkId)
        {
            var check = new Check();
            check.CheckId = checkId;

            Assert.Equal(checkId, check.CheckId);
            Assert.Null(check.Value);
        }

        [Theory]
        [InlineData("asdf")]
        [InlineData("fdassdf")]
        public void CanSetValue(string val)
        {
            var check = new Check();
            check.Value = val;

            Assert.Equal(val, check.Value);
            Assert.Null(check.CheckId);
        }

    }
}
