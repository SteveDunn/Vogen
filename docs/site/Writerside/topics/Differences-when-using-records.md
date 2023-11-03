# Differences with records

TL;DR: there are differences, and it's best to stick to a `class` or `struct` rather than records as the benefits of records don't really apply to Vogen where a single primitive value is being wrapped and protected.

For classes and structs, Vogen generates a lot of boilerplate code. But for records, some of this boilerplate code is 
already generated. This page lists the differences between records (classes and structs) and non-record classes and structs.

* the generated code for records have an `init` accessibility on the `Value` property in order to support `with`, 
e.g. `var vo2 = vo1 with { Value = 42 }` - but initializing via this doesn't set the object as being initialized as this
would promote the use of public constructor (even though the analyzer will still cause a compilation error)
* the generated code for records still overrides `ToString` as the default enumerates fields, which we don't want

Something to consider in the forthcoming C# 12, is primary constructors for classes, and how they will fit in with Vogen.
