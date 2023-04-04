---
uid: sample-log
---

# Example: Logging

This series of examples have a single goal: to build an aspect that writes a message before and after any targeted method is executed.

We start from the most trivial implementation and progressively add features:

| Article | Description |
|--------|-------------|
| [Logging a constant message](log-101/README.md) | This is the simplest possible implementation of logging. |
| [Adding the method name](log-102/README.md) | Instead of logging a constant, generic message, we now include the method name in the message. |
| [Adding the parameter values](log-103/README.md) | We now add the parameter values and the return value to the message. |
| [Using to ILogger](log-104/README.md) | Instead of using `Console.WriteLine`, we inject an `ILogger` into the target type using dependency injection. |
| [ILogger without dependency injection](log-105/README.md) | Instead of using dependency injection, we expect the source code to contain an `ILogger` field, and report errors if it does not. |