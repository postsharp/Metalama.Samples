using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metalama.Samples.Builder1;

public interface ISimple
{
    string? Name { get; }

    int Age { get; }
}

public interface ISimpleBuilder : IBuilder<ISimple>
{
    string? Name { get; set; }

    int Age { get; set; }
}

internal class Simple1 : ISimple
{
    public string? Name { get; init; }

    public int Age { get; }

    public Simple1( int age)
    {
        this.Age = age;
    }
}

internal class Simple2 : ISimple
{
    public string? Name { get; }

    public int Age { get; }

    public Simple2( string firstName, string secondName, int age )
    {
        this.Name = $"{firstName} {secondName}";
        this.Age = age;
    }
}

internal class Simple1Builder : ISimpleBuilder
{

}

