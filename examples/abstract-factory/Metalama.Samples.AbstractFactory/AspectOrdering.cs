using Metalama.Framework.Aspects;
using Metalama.Samples.AbstractFactory;

[assembly: AspectOrder(AspectOrderDirection.CompileTime, typeof(FactoryComponentAttribute), typeof(ConcreteFactoryAttribute))]