# Contributing

We'll happily accept contributions from the community!  There's an open [list of issues here](https://github.com/SteveDunn/Vogen/issues).

Vogen itself is split up into a few projects:
#### Vogen
The 'main' project that contains the source generator and analyzers

#### Vogen.CodeFixers
The project that contains the code fixers. Code fixers are the automated alt+enter generators for things such as 'Add validation method' etc.

#### Pack
The project to package Vogen into a NuGet package

#### Vogen.SharedTypes
This is a small library containing the attributes and enums that clients use. It is shipped as part of the same NuGet package as the source generator.

#### AnalyzerTests
Tests for each analyzer.

#### Shared
Shared types used amongst the test projects

#### SnapshotTests
These tests contain value object declarations and compare the generated source code to known and verified snapshots.
If one of these tests fail in an unexpected location, it shows that something unexpected has been changed.
They are also a good way to flex the variations of using value objects; there's some horrendous contortions of value objects, for instance, escaped types with reserved keywords in namespaces with reserved keywords, wrapping types with strange names!

These tests snapshot thousands of files and can take up to about 30 minutes to run.

As well as the various contortions and edge cases, they're also run on different frameworks and in different locales.

#### Vogen.Tests
Unit tests of the various types used in Vogen itself

<br/>

As well as the main solution (`Vogen.sln`), there's a solution for consumer code (`Consumers.sln`). This contains various consumer projects, such as:
* an ASPNET Core project (with a consumer using various serializers and Open API schams)
* an 'Onion architecture' project
* an Orleans project
* a project that demonstrates using Vogen in an AOT (Ahead-Of-Time) scenario.

## FAQ

### I've made a change that means the 'Snapshot' tests are expectedly failing in the build - what do I do?

Vogen uses a combination of unit tests, in-memory compilation tests, and snapshot tests. The snapshot tests are used
to compare the output of the source generators to the expected output stored on disk.

If your feature/fix changes the output of the source generators, then running the snapshot tests will bring up your
configured code diff tool, for instance, Beyond Compare, to show the differences. You can accept the differences in that
tool, or, if there's a lot of differences (and they're all expected!), you have various options depending on your
platform and tooling. Those are [described here](https://github.com/VerifyTests/Verify/blob/main/docs/clipboard.md).

**NOTE: If the change to the source generators expectedly changes the majority of the snapshot tests, then you can tell the
snapshot runner to overwrite the expected files with the actual files that are generated.**

To do this, run `.\RunSnapshots.ps1 -v "Minimal" -reset $true`. This deletes all `snaphsot` folders under the `tests` folder
and treats everything that's generated as the new baseline for future comparisons.

This will mean that there are potentially **thousands** of changed files that will end up in the commit, but it's expected and unavoidable.

### How do I debug the source generator?

The easiest way is to debug the SnapshotTests. Put a breakpoint in the code, and then just debug a test somewhere.

To debug an analyzer, select or write a test in the AnalyzerTests. There are tests that exercise the various analyzers and code-fixers.

### How do I run the tests that actually use the source generator?

It is difficult to run tests that _use_ the source generator in the same project **as** the source generator, so there
is a separate solution for this. It's called `Consumers.sln`. What happens is that `build.ps1` builds the generator, runs
the tests, and creates the NuGet package _in a private local folder_. The package is version `999.9.xxx` and the consumer
references the latest version. The consumer can then really use the source generator, just like anything else.

> Note: if you want to run the lengthy snapshot tests, run `.\RunSnapshots.ps1 -v "minimal"`

### How do I run the benchmarks?

`dotnet run -c Release -- --job short --framework net6.0 --filter *`

### Why do I get a build error when running `.\Build.ps1`?

You might see this:
```
.\Build.ps1 : File C:\Build.ps1 cannot be loaded. The file C:\Build.ps1 is not digitally signed. You cannot run this script on the current system. 
```

To get around this, run `Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
