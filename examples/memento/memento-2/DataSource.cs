using NameGenerator.Generators;
using NameGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    public class DataSource : IDataSource
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

        public string GetNewName()
        {
            return this._nameGenerator.Generate();
        }

        public string GetNewSpecies()
        {
            return FishSpecies[this._random.Next(FishSpecies.Length)];
        }
    }
}
