namespace Metalama.Samples.AbstractFactory.Tests.ImplicitDiscovery;

public interface IControlFactory
{
    ITextBox CreateTextBox( string text );
    IButton CreateButton();
}


public interface ITextBox;

public interface IButton;

public class WpfTextBox : ITextBox
{
    public WpfTextBox(string text)
    {
    }
}

public class WpfButton : IButton;


// <target>
[ConcreteFactory(typeof(IControlFactory), WithImplementationsFromCurrentNamespace = true)]
public class WpfControlFactory
#if TEST_RUNNER
    : IControlFactory
#endif
{
    
}

