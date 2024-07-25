using NameGenerator;

public class FishGenerator : IFishGenerator
{
    private static string[] FishSpecies =
    [
        "Clownfish",
        "Damselfish",
        "Dottyback",
        "Fairy Basslet",
        "Goby",
        "Hawkfish",
        "Jawfish",
        "Lionfish",
        "Mandarin Dragonet",
        "Neon Goby",
        "Pseudochromis",
        "Royal Gramma",
        "Tang",
        "Wrasse",
        "Scuba Diver"
    ];

    private readonly GeneratorBase _nameGenerator;
    private readonly Random _random;

    public FishGenerator( GeneratorBase nameGenerator )
    {
        this._nameGenerator = nameGenerator;
        this._random = new Random();
    }

    public string GetNewName() => this._nameGenerator.Generate();

    public string GetNewSpecies() => FishSpecies[this._random.Next( FishSpecies.Length )];
}