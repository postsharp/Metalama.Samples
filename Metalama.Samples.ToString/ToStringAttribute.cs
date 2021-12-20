// Copyright (c) SharpCrafters s.r.o. All rights reserved.
// This project is not open source. Please see the LICENSE.md file in the repository root for details.

using Caravela.Framework.Aspects;
using Caravela.Framework.Code.SyntaxBuilders;
using System.Linq;

namespace Caravela.Samples.ToString
{
    internal class ToStringAttribute : TypeAspect
    {
        [Introduce( WhenExists = OverrideStrategy.Override, Name = "ToString" )]
        public string IntroducedToString()
        {
            var stringBuilder = new InterpolatedStringBuilder();
            stringBuilder.AddText( "{ " );
            stringBuilder.AddText( meta.Target.Type.Name );
            stringBuilder.AddText( " " );

            var fields = meta.Target.Type.FieldsAndProperties.Where( f => !f.IsStatic ).ToList();

            var i = meta.CompileTime( 0 );

            foreach ( var field in fields )
            {
                if ( i > 0 )
                {
                    stringBuilder.AddText( ", " );
                }

                stringBuilder.AddText( field.Name );
                stringBuilder.AddText( "=" );
                stringBuilder.AddExpression( field.Invokers.Final.GetValue( meta.This ) );

                i++;
            }

            stringBuilder.AddText( " }" );

            return stringBuilder.ToValue();
        }
    }
}