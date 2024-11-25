[ConcreteFactory(typeof(IControlFactory), WithImplementationsFromCurrentNamespace = true)]
public class WpfControlFactory
{
  public IButton CreateButton()
  {
    return new WpfButton();
  }
  public ITextBox CreateTextBox(string text)
  {
    return new WpfTextBox(text);
  }
}