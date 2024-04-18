# Differences with records

TL;DR: there are significant differences, and it is best to stick to a `class` or `struct` rather than records as the benefits of records don't apply to Vogen where a single primitive value is being wrapped and protected.

For classes and structs, Vogen generates a lot of boilerplate code. But for records, some of this boilerplate code is 
already generated. This page lists the differences between records (classes and structs) and non-record classes and structs.

* the generated code for records have an `init` accessibility on the `Value` property to support `with`, 
e.g. `var vo2 = vo1 with { Value = 42 }` - but initializing via this doesn't set the object as being initialized as this
would promote using a public constructor (even though the analyzer will still cause a compilation error)
* the generated code for records still overrides `ToString` as the default enumerates fields, which we don't want

Something not yet implemented is primary constructor analysis for classes in C#12, and how they will fit in with Vogen.
This is covered in [this issue](https://github.com/SteveDunn/Vogen/issues/563).
