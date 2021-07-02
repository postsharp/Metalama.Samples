using Caravela.TestFramework;
using Xunit;
using Xunit.Abstractions;

namespace Caravela.Samples.Log.Tests
{
    public class TestList : CaravelaTestList
    {
        public TestList(ITestOutputHelper logger): base(logger)
        {
            this.AddAssemblyOf<LogAttribute>();
        }

        [Theory]
        [TestFiles]
        public void Test(string f) => base.RunTest(f);
    }
}