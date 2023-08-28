---
uid: sample-notifypropertychanged-sdk
---

# Example: INotifyPropertyChanged with dependencies

This example shows how to use `Metalama.Framework.Sdk` to access the Roslyn code model and perform analysis of the syntax tree for the sake of detecting dependencies between properties.

The example will only discover dependencies within the current type and will not report unsupported situations.