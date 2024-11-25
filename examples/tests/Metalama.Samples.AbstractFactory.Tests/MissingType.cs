namespace Metalama.Samples.AbstractFactory.Tests.MissingType;

public interface IControlFactory
{
    ITextBox CreateTextBox( string text );
    IButton CreateButton();
}


public interface ITextBox;

public interface IButton;

[FactoryComponent(typeof(WpfControlFactory))]
public class WpfTextBox : ITextBox
{
    public WpfTextBox(string text)
    {
    }
}

// <target>
[ConcreteFactory(typeof(IControlFactory))]
public class WpfControlFactory
#if TEST_RUNNER
    : IControlFactory
#endif
{
    
}

