using System.Collections.Frozen;

public interface IConfigurationSource
{
    FrozenDictionary<string, string> LoadConfiguration();
}

[Singleton]
public sealed class ConfigurationManager
{
    private FrozenDictionary<string, string> dictionary;

    public ConfigurationManager(IConfigurationSource configurationSource)
    {
        dictionary = configurationSource.LoadConfiguration();
    }

    public string GetValue(string key) => dictionary[key];
}

namespace Prod
{
    class ProductionClass
    {
        void M(IConfigurationSource configurationSource)
        {
            _ = new ConfigurationManager(configurationSource);
        }
    }
}

namespace Tests
{
    class TestClass
    {
        void M(IConfigurationSource configurationSource)
        {
            _ = new ConfigurationManager(configurationSource);
        }
    }
}