# Testing

<note>
This topic is incomplete and is currently being improved.

Also, this topic relates to the Vogen project and not directly related to users of the Vogen package.
</note>


Testing source generators is tricky.
You can't run the generators directly from code because it's the IDE that loads the generators, not your tests.

So the tests are in two solutions:

* In the **main solution**, there are [snapshot tests](https://github.com/VerifyTests/Verify) which create **in-memory projects** that exercise the generators using different versions of the .NET Framework.
These are slow to run because they use many different permutations of features and dozens of variations of configuration/primitive-type/C#-type/accessibility-type/converters, for example:
    * does it correctly generate a record struct with instances and normalization and a type converter and a Dapper serializer?
    * does it correctly generate a class with no converters, no validation, and no serialization?
    * does it correctly generate a readonly struct with a LinqToDb converter?
    * etc. etc.

    (all tests run for each supported framework)

The snapshot tests in the IDE run in about 5 minutes. In the CI build, we set a `THOROUGH` flag which exercises more variations, and that can take up to **several hours**.

* In the **consumer solution**, the tests involve consuming the 'real' Vogen NuGet package and exercising it via
 _real_ C# code.
 To ensure it tests the latest version of Vogen, `test.ps1` first builds Vogen, then forces a NuGet 
 package to be built locally with a version of `999.9.xxx`.
  These tests are much quicker to run.
  They verify the behavior
 of created Value Objects, such as:
    * [Normalization](NormalizationHowTo.md "How to use normalization")
    * Equality
    * Hashing
    * ToString
    * Validation
    * Instance Fields

