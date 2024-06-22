using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class MementoManager
{
    public static IMemento CurrentMemento { get; set; }
}

public interface IMemento
{
    IOriginator Originator { get; }
}

public interface IOriginator
{
    IMemento Save();

    void Restore( IMemento memento );
}