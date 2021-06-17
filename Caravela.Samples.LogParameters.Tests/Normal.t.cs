namespace Caravela.Samples.LogParameters.Tests.Normal
{
    class Foo
    {
        [Log]
        void Bar(int a, string b)
        {
            var arguments = new object[] { a, b };
            global::System.Console.WriteLine("Caravela.Samples.LogParameters.Tests.Normal.Foo.Bar(a = {0}, b = {1}) started", arguments);
            try
            {
                global::Caravela.Framework.Aspects.__Void result;
                global::System.Console.WriteLine(string.Format("Caravela.Samples.LogParameters.Tests.Normal.Foo.Bar(a = {0}, b = {1})", arguments) + " returned " + result);
                return;
            }
            catch (global::System.Exception e)
            {
                global::System.Console.WriteLine("Caravela.Samples.LogParameters.Tests.Normal.Foo.Bar(a = {0}, b = {1}) failed: " + e, arguments);
                throw;
            }
        }
    }
}