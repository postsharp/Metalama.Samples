using System.Diagnostics;
using System.Globalization;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;

namespace Metalama.Samples.Proxy;

[CompileTime]
internal static class TupleHelper
{
    public static IExpression CreateTupleExpression(IMethod method)
    {
        if (method.Parameters.Count == 0)
        {
            return ExpressionFactory.Default(typeof(ValueTuple));
        }

        var expressionBuilder = new ExpressionBuilder();

        if (method.Parameters.Count == 1)
        {
            expressionBuilder.AppendTypeName(typeof(ValueTuple));
            expressionBuilder.AppendVerbatim(".");
            expressionBuilder.AppendVerbatim(nameof(ValueTuple.Create));
            expressionBuilder.AppendVerbatim("(");
            AppendParameterValue(0);
            expressionBuilder.AppendVerbatim(")");
        }
        else
        {
            expressionBuilder.AppendVerbatim("(");
            for (var index = 0; index < method.Parameters.Count; index++)
            {
                if (index > 0)
                {
                    expressionBuilder.AppendVerbatim(", ");
                }

                AppendParameterValue(index);
            }

            expressionBuilder.AppendVerbatim(")");

            return expressionBuilder.ToExpression().WithType(CreateTupleType(method));
        }

        return expressionBuilder.ToExpression();

        void AppendParameterValue(int index)
        {
            var parameter = method.Parameters[index];
            if (parameter.RefKind != RefKind.Out)
            {
                expressionBuilder.AppendExpression(parameter);
            }
            else
            {
                expressionBuilder.AppendExpression(ExpressionFactory.Default(parameter.Type));
            }
        }
    }

    public static IExpression GetTupleItemExpression(IExpression tuple, int index)
    {
        var expressionBuilder = new ExpressionBuilder();

        expressionBuilder.AppendExpression(tuple);

        for (var i = 0; i < index / 7; i++)
        {
            expressionBuilder.AppendVerbatim(".Rest");
        }

        var finalIndex = (index % 7) + 1;
        expressionBuilder.AppendVerbatim(".Item");
        expressionBuilder.AppendVerbatim(finalIndex.ToString(CultureInfo.InvariantCulture));

        return expressionBuilder.ToExpression();
    }

    public static IType CreateTupleType(IMethod method)
    {
        if (method.Parameters.Count == 0)
        {
            return TypeFactory.GetType(typeof(ValueTuple));
        }
        else
        {
            return CreateTupleTypeRecursive(0);
        }

        IType CreateTupleTypeRecursive(int firstParameterIndex)
        {
            Debugger.Break();

            var lastDirectParameterIndex =
                Math.Min(method.Parameters.Count, firstParameterIndex + 7);

            var typeArguments = new List<IType>();

            for (var index = firstParameterIndex; index < lastDirectParameterIndex; index++)
            {
                typeArguments.Add(method.Parameters[index].Type);
            }

            if (method.Parameters.Count > lastDirectParameterIndex)
            {
                var restType = CreateTupleTypeRecursive(firstParameterIndex + 7);
                typeArguments.Add(restType);
            }

            if (typeArguments.Count == 0)
            {
                return (INamedType)TypeFactory.GetType(typeof(ValueTuple));
            }
            else
            {
                var typeDefinition =
                    TypeFactory.GetType(typeof(ValueTuple).FullName + "`" + typeArguments.Count);

                return typeDefinition.WithTypeArguments(typeArguments.ToArray());
            }
        }
    }
}