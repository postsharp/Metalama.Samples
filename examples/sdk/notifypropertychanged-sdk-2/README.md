---
uid: sample-notifypropertychanged-sdk-2
summary: "This document describes a second implementation strategy of `INotifyPropertyChanged` using the Roslyn API to analyze the syntax tree, supporting base class property dependencies."
---

# Example: INotifyPropertyChanged with dependencies (2)

This is a second implementation strategy of `INotifyPropertyChanged` that uses the Roslyn API to analyze the syntax
tree.
This design supports dependencies on properties of the base class, and moved the smart logic into `OnPropertyChanged`.

