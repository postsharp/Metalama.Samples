---
uid: sample-dirty-limitations
---

# Change Tracking example: limitations

A significant limitation of the aspects implemented in the articles above is that they only support fields of _immutable_ type. This is most visible with collection fields. Objects often contain collection fields. The field containing the collection is `readonly`, but the collection itself is mutable.

There are several strategies to mitigate this limitation:

* Manually call `OnChange` from collection operations.
* Use immutable collections.
* Create change-tracking collection classes.

Unless you create change-tracking collection classes, you need to design your class so that it exposes the collections as read-only interfaces to prevent the caller code from _skipping_ the call to `OnChange`. You could add code to the `BuildAspect` method to verify that all exposed fields are either immutable or implement the change-tracking mechanism.

If you have an object model with parent-child relationships, you may need to call the `OnChange` method of the parent object when any child object is being modified.
