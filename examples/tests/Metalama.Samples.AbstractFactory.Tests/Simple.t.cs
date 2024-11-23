[ConcreteFactory(typeof(IControlFactory))]
public class WpfControlFactory
{
  public IButton CreateButton()
  {
    return new WpfButton();
  }
  public ITextBox CreateTextBox()
  {
    return new WpfTextBox();
  }
}