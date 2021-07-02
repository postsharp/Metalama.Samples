using Caravela.TestFramework;
using Xunit;
using Xunit.Abstractions;

namespace Caravela.Samples.Log.Tests
{
    public class TestSuite : CaravelaTestSuite
    {
        public TestSuite(ITestOutputHelper logger): base(logger)
        {
            this.AddAssemblyOf<LogAttribute>();
        }

        [Theory]
        [TestFiles]
        public void Test(string f) => base.RunTest(f);
    }
}