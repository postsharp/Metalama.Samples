---
uid: sample-memento-1
level: 400
---

# Memento example, step 2: UI transactions

[!metalama-project-buttons .]

In the previous step, we have implemented the `MementoAttribute` that uses type introduction to implement the Memento pattern, 
specifically the `IOriginator` and `IMemento` interfaces. This achieved undo functionality in the sample application.

However, a problem can be easily observed. While we are editing the `ItemViewModel`, the changed are immediately reflected in
other parts of the UI. Specifically, when the name is edited, in the ItemControl, changes are visible in the list of items.

This is not a desired behavior. We want to have a transactional behavior, where changes are not visible in the rest of the UI
until they are committed. 

One option is to create a copy of the ItemViewModel and bind the ItemControl to that copy while editing. When editing is done, 
copy the changes back to the original instance. To do this, we would need to do a lot of work in the code-behind to properly
switch data contexts and to copy the changes.

We need something more transparent that would scale when the UI becomes more complex and editing a single item spans multiple
views.

## Implementation

Let's now look at the aspect code.

[!metalama-file MementoAttribute.cs]
mento with the current state of the target instance.
- Implement the `IOriginator` interface.

> [!NOTE]
> This example is using <xref:Metalama.Framework.Advising.IAdviser`1> extension methods, which were added in Metalama 2024.2.
> Corresponding API is also available in <xref:Metalama.Framework.Aspects.IAdviceFactory>.

> [!div class="see-also"]
> <xref:dependency-injection>