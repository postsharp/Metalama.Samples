namespace Metalama.Samples.AbstractFactory.Tests.ManyTypes;

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

[FactoryComponent(typeof(WpfControlFactory))]
public class WpfTextBox2 : ITextBox
{
    public WpfTextBox2(string text)
    {
    }
}


[FactoryComponent(typeof(WpfControlFactory))]
public class WpfButton : IButton;


// <target>
[ConcreteFactory(typeof(IControlFactory))]
public class WpfControlFactory
#if TEST_RUNNER
    : IControlFactory
#endif
{
    
}

