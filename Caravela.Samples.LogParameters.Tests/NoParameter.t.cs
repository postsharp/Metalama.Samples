namespace Caravela.Samples.LogParameters.Tests.NoParameter
{
    class Foo
    {
        [Log]
        void Bar()
        {
            var arguments = new object[] { };
            global::System.Console.WriteLine("Caravela.Samples.LogParameters.Tests.NoParameter.Foo.Bar() started", arguments);
            try
            {
                global::Caravela.Framework.Aspects.__Void result;
                global::System.Console.WriteLine(string.Format("Caravela.Samples.LogParameters.Tests.NoParameter.Foo.Bar()", arguments) + " returned " + result);
                return;
            }
            catch (global::System.Exception e)
            {
                global::System.Console.WriteLine("Caravela.Samples.LogParameters.Tests.NoParameter.Foo.Bar() failed: " + e, arguments);
                throw;
            }
        }
    }
}