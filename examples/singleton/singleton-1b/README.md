---
uid: sample-singleton-1b
---

# Singleton bonus: Checking private constructor references

[!metalama-project-buttons .]

So far, we have ensured that the constructor is private, but that does not guarantee it is only ever called once. To do this, we can use Metalama infrastructure for architecture validation. Though since we're not using it for its originally intended purpose, we have to customize it a bit (see <xref:validation-extending>). We do this by creating a custom <xref:Metalama.Framework.Validation.ReferenceValidator> that checks that a member is only used from the `Instance` property and then applying it to the constructur of the singleton by calling <xref:Metalama.Framework.Validation.IValidatorReceiver.ValidateReferences(Metalama.Framework.Validation.ReferenceValidator)>:

[!metalama-test ../../tests/Metalama.Samples.Singleton1b.Tests/Singleton.cs]

Creating a custom <xref:Metalama.Framework.Validation.ReferenceValidator> is necessary here, because the default one used by methods like <xref:Metalama.Extensions.Architecture.Fabrics.VerifierExtensions.CanOnlyBeUsedFrom*> does not validate the type containing the validated member.