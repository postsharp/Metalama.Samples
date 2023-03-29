using System.Text;

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