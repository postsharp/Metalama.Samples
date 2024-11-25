namespace Metalama.Samples.AbstractFactory.Tests.NoConstructor;

public interface IControlFactory
{
    ITextBox CreateTextBox( string text );
    IButton CreateButton();
}


public interface ITextBox;

public interface IButton;

[FactoryComponent(typeof(WpfControlFactory))]
public class WpfTextBox : ITextBox;

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

