// This is an open-source Metalama example. See https://github.com/postsharp/Metalama.Samples for more.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using System.Text;

public class EnrichExceptionAttribute : OverrideMethodAspect
{
    public override dynamic? OverrideMethod()
    {
        // Compile-time code: create a formatting string containing the method name and placeholder for formatting parameters.
        var methodSignatureBuilder = new InterpolatedStringBuilder();
        methodSignatureBuilder.AddText( meta.Target.Type.ToString() );
        methodSignatureBuilder.AddText( "." );
        methodSignatureBuilder.AddText( meta.Target.Method.Name );
        methodSignatureBuilder.AddText( "(" );
        var i = meta.CompileTime( 0 );

        foreach ( var p in meta.Target.Parameters )
        {
            if ( p.Index > 0 )
            {
                methodSignatureBuilder.AddText( ", " );
            }

            if ( p.RefKind == RefKind.Out )
            {
                methodSignatureBuilder.AddText( $"{p.Name} = <out> " );
            }
            else
            {
                methodSignatureBuilder.AddExpression( p.Value );
            }

            i++;
        }

        methodSignatureBuilder.AddText( ")" );

        try
        {
            return meta.Proceed();
        }
        catch ( Exception e )
        {
            EnrichExceptionHelper.AppendContextFrame( e, methodSignatureBuilder.ToValue() );

            throw;
        }
    }
}

public static class EnrichExceptionHelper
{
    public static void AppendContextFrame( Exception e, string frame )
    {
        // Get or create a StringBuilder for the exception where we will add additional context data.
        var stringBuilder = (StringBuilder?) e.Data["Context"];

        if ( stringBuilder == null )
        {
            stringBuilder = new StringBuilder();
            e.Data["Context"] = stringBuilder;
        }

        // Add current context information to the string builder.
        stringBuilder.Append( frame );
        stringBuilder.AppendLine();
    }
}