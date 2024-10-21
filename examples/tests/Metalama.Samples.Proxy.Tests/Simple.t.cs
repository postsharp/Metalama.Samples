using Metalama.Samples.Proxy;
using Metalama.Samples.Proxy.Tests.Simple;
[assembly: GenerateProxyAspect(typeof(ISomeInterface), "Metalama.Samples.Proxy.Tests", "SomeProxy")]
namespace Metalama.Samples.Proxy.Tests.Simple;
public interface ISomeInterface
{
  void VoidMethod(int a, string b);
  int NonVoidMethod(int a, string b);
  void VoidNoParamMethod();
}