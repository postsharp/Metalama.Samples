using System.Runtime.CompilerServices;
using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;

namespace Metalama.Samples.Proxy;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class GenerateProxyAspect : CompilationAspect
{
    private readonly Type _interfaceType;
    private readonly string _ns;
    private readonly string _typeName;

    public GenerateProxyAspect(Type interfaceType, string ns, string typeName)
    {
        this._interfaceType = interfaceType;
        this._ns = ns;
        this._typeName = typeName;
    }

    public override void BuildAspect(IAspectBuilder<ICompilation> builder)
    {
        base.BuildAspect(builder);

        // Add a type.
        var type = builder.WithNamespace(this._ns).IntroduceClass(this._typeName,
            buildType: t => t.Accessibility = Accessibility.Public);


        // Add a field with the intercepted object.
        var interceptedField = type.IntroduceField(
                "_intercepted",
                this._interfaceType,
                IntroductionScope.Instance)
            .Declaration;

        // Add a field for the interceptor.
        var interceptorField = type.IntroduceField("_interceptor", typeof(IInterceptor))
            .Declaration;

        // Implement the interface.
        type.ImplementInterface(this._interfaceType);

        // Implement interface members.
        var namedType = (INamedType)TypeFactory.GetType(this._interfaceType);

        foreach (var method in namedType.Methods)
        {
            var argsType = TupleHelper.CreateTupleType(method);

            type.IntroduceMethod(
                method.ReturnType.SpecialType == SpecialType.Void
                    ? nameof(this.VoidMethodTemplate)
                    : nameof(this.NonVoidMethodTemplate),
                IntroductionScope.Instance,
                buildMethod: methodBuilder =>
                {
                    methodBuilder.Accessibility = Accessibility.Public;
                    methodBuilder.Name = method.Name;
                    methodBuilder.ReturnType = method.ReturnType;

                    foreach (var parameter in method.Parameters)
                    {
                        methodBuilder.AddParameter(
                            parameter.Name,
                            parameter.Type,
                            parameter.RefKind);
                    }
                },
                args:
                new
                {
                    TArgs = argsType, TResult = method.ReturnType, method, interceptedField,
                    interceptorField
                });
        }

        // Add the constructor.
        type.IntroduceConstructor(
            nameof(this.Constructor),
            buildConstructor: constructorBuilder
                => constructorBuilder.AddParameter("intercepted", this._interfaceType),
            args: new { interceptedField, interceptorField });
    }

    [Template]
    private void VoidMethodTemplate<[CompileTime] TArgs>(
        IMethod method, IField interceptedField, IField interceptorField)
        where TArgs : struct, ITuple
    {
        // Prepare the context.
        var args = (TArgs)TupleHelper.CreateTupleExpression(method).Value!;
        var argsExpression = ExpressionFactory.Capture(args);

        // Get writable parameters.
        var writableParameters = method.Parameters.Where(p =>
            p.RefKind is RefKind.Out or RefKind.Ref).ToList();

        // Invoke the interceptor.
        if (writableParameters.Count == 0)
        {
            // We don't need a try...finally if we don't have to write back writable parameters.
            ((IInterceptor)interceptorField.Value!).Invoke(ref args, Invoke);
        }
        else
        {
            try
            {
                ((IInterceptor)interceptorField.Value!).Invoke(ref args, Invoke);
            }
            finally
            {
                // Copy back parameters.
                foreach (var parameter in writableParameters)
                {
                    parameter.Value =
                        TupleHelper.GetTupleItemExpression(argsExpression, parameter.Index);
                }
            }
        }

        ValueTuple Invoke(ref TArgs receivedArgs)
        {
            var receivedArgsExpression = ExpressionFactory.Parse("receivedArgs");

            var arguments = method.Parameters.Select(p =>
                TupleHelper.GetTupleItemExpression(receivedArgsExpression, p.Index));

            method.With(interceptedField).Invoke(arguments);

            return default;
        }
    }

    [Template]
    private TResult NonVoidMethodTemplate<[CompileTime] TArgs, [CompileTime] TResult>(
        IMethod method, IField interceptedField, IField interceptorField)
        where TArgs : struct, ITuple
    {
        // Prepare the context.
        var args = (TArgs)TupleHelper.CreateTupleExpression(method).Value!;
        var argsExpression = ExpressionFactory.Capture(args);

        // Get writable parameters.
        var writableParameters = method.Parameters.Where(p =>
            p.RefKind is RefKind.Out or RefKind.Ref).ToList();

        // Invoke the interceptor.
        if (writableParameters.Count == 0)
        {
            // We don't need a try...finally if we don't have to write back writable parameters.
            return ((IInterceptor)interceptorField.Value!).Invoke(ref args, Invoke);
        }
        else
        {
            try
            {
                return ((IInterceptor)interceptorField.Value!).Invoke(ref args, Invoke);
            }
            finally
            {
                // Copy back parameters.
                foreach (var parameter in writableParameters)
                {
                    parameter.Value =
                        TupleHelper.GetTupleItemExpression(argsExpression, parameter.Index);
                }
            }
        }

        TResult Invoke(ref TArgs receivedArgs)
        {
            var receivedArgsExpression = ExpressionFactory.Parse("receivedArgs");

            var arguments = method.Parameters.Select(p =>
                TupleHelper.GetTupleItemExpression(receivedArgsExpression, p.Index));

            return method.With(interceptedField).Invoke(arguments)!;
        }
    }


    [Template]
    public void Constructor(IInterceptor interceptor, IField interceptedField,
        IField interceptorField)
    {
        interceptorField.Value = interceptor;
        interceptedField.Value = meta.Target.Parameters["intercepted"].Value;
    }
}