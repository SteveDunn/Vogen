# GitHub Copilot Instructions for Vogen

This document is aimed at AI agents working on the **Vogen** repository.  The
codebase is a .NET source‑generator + analyzer that wraps primitives in
strongly‑typed value objects.  Understanding the structure, build/test
workflows, and project‑specific conventions will make an agent productive
quickly.

---

## 🚀 High‑Level Architecture

* **Core library** lives in `src/Vogen`.  It contains the Roslyn
  source‑generator and analyzer logic.  Look under `Generators` for the
  template code that emits structs/classes/records, and `Conversions` for the
  plumbing that handles primitive conversions.
* **Code fixers** are in `src/Vogen.CodeFixers`; they plug into the validator
  diagnostics produced by the analyzer.
* **Packaging** happens in `src/Vogen.Pack`; this project bundles the
  analyzer/generator dlls for multiple Roslyn versions (`roslyn4.4`,
  `4.6`, `4.8`, `4.11`, `4.12`) so consumers can reference the correct one.
* **Shared types** used by tests live in `src/Vogen.SharedTypes`; these are
  compiled as metadata references when snapshotting across frameworks.
* **Sample/consumer apps** under `samples/*` and the `Consumers.sln` show
  real‑world usage (WebApplication, OrleansExample, etc.).

The generator is invoked during normal `dotnet build` on a project that
contains `[ValueObject]` attributes.  The analyzer emits `VOG###` diagnostics to
prevent invalid construction.  Codefixers offer automatic fixes.

---

## 🗂 Repository Structure (key folders)

```
src/            ← core projects (Vogen, CodeFixers, Pack, SharedTypes)
tests/          ← unit, snapshot, analyzer and consumer tests
samples/        ← lightweight example applications
RunSnapshots.ps1← PowerShell helper for resetting/running snapshot tests
Build.ps1       ← full build-pack-test script used by CI and maintainers
CONTRIBUTING.md ← developer guidance (tests, thorough mode, etc.)
```

Snapshots live alongside generated code under `tests/SnapshotTests/snapshots`.
AnalyzerTests generate code in‑memory and assert diagnostics/solutions.
ConsumerTests are end‑to‑end projects that reference Vogen via NuGet (see
`Build.ps1` for how the local package is built and consumed).

---

## 🛠 Developer Workflows

1. **Quick build** – `dotnet build Vogen.sln -c Release` builds the generator,
   analyzer, codefixers and tests.  `Directory.Build.props` sets
   `LangVersion=preview`, `TreatWarningsAsErrors`, and common suppression
   flags.
2. **Snapshot tests** – run `.\RunSnapshots.ps1` from repo root.  this
   script cleans snapshot folders, builds with `-p Thorough=true` and runs
   `dotnet test tests/SnapshotTests/SnapshotTests.csproj`.  Pass
   `-reset` to delete existing snapshots first.  Add `-p THOROUGH` to the
   build/test invocation for the full permutation set (used by CI).
3. **Analyzer tests** – `dotnet test tests/AnalyzerTests/AnalyzerTests.csproj`.
   These compile generated snippets and assert diagnostics / codefixes.  They
   depend on `SnapshotTests` for the generated output.
4. **Consumer / sample validation** – run `Build.ps1` which:
   * builds the core projects for each Roslyn version;
   * packs `Vogen.Pack` into a local folder with a unique 999.X version;
   * restores `Consumers.sln` using `nuget.private.config` pointing at the
     local package;
   * builds/tests samples and consumer tests in both Debug and Release;
   * optionally rebuilds with `DefineConstants="VOGEN_NO_VALIDATION"`.
5. **Publishing** – `Build.ps1` ends by packing a release NuGet into
   `./artifacts`.

> **Note:** pull requests should include updated snapshots if the
> generator output changes; run the reset script and commit the diffs.

---

## 🧪 Testing Conventions

* Tests target multiple TFMs; snapshot project is multi‑targeted
  (`net461`, `netstandard2.0`, `net8.0` …) to ensure generated code works
  everywhere.  The code generating the tests lives in `tests/SnapshotTests`.
* The `THOROUGH` MSBuild property expands the permutation matrix
  (struct/class/record/readonly, underlying types `int`, `string`, …,
  conversions, etc.).  It slows down local runs; CI always sets it.
* `AnalyzerTests` use XUnit attributes and in‑memory compilation helpers
  (`CompilationHelper.cs`).  They also check codefixers in
  `Vogen.CodeFixers`.
* `ConsumerTests` are plain projects referencing the NuGet package; they are
  rebuilt by `Build.ps1` to exercise packaging scenarios.
* `tests/Testbench` contains scratch code that developers use when
  experimenting; not part of CI.

---

## 🔧 Project‑Specific Patterns

* **Generator templates:** strings assembled with `$@` inside classes like
  `StructGenerator`, `ClassGenerator`, `RecordStructGenerator`.  Helpers in
  `Util.cs` and the `Conversions` namespace produce small fragments.
* **Attribute‑driven design:** `[ValueObject]` (in the `Vogen` namespace) is
  the only public API for consumers.  Additional configuration comes from
  `[ValueObjectConverter]`, `[ValueObjectTypeAttribute]`, etc.  `GenerationParameters`
  and `VoWorkItem` carry the state through the generator.
* **Roslyn versioning:** the `RoslynVersion` MSBuild property is used in
  `src/Vogen/Vogen.csproj` and `Vogen.CodeFixers.csproj` to produce assemblies
  that target specific engine versions – this drives the packaging logic.
* **Compile-time switches:** `VOGEN_NO_VALIDATION` disables the runtime
  validation code; used in consumer tests.  `THOROUGH` and
  `ResetSnapshots` control test behaviours.
* **`Nullable` handling:** the generator has explicit support for nullable
  underlying types (`string?`, `int?`) using helpers like
  `Nullable.QuestionMarkForUnderlying` and `WrapBlock`.
* **Performance-first:** generated types are designed to be as thin as
  possible; tests under `tests/Vogen.Benchmarks` validate performance.
* **Style:** the codebase is very terse and uses C#8/9/10/11 features; maintain
  consistency with existing files when adding or editing generators.

---

## 📌 Integration & External Dependencies

* The only external package references in core code are Roslyn (`Microsoft.CodeAnalysis`)
  and test frameworks (xUnit, FluentAssertions, etc.).
* Consumer examples depend on ASP.NET, Orleans, Refit, ServiceStack, etc.
* CI pipeline (GitHub actions) invokes `Build.ps1` and `RunSnapshots.ps1`.

---

## 👀 Getting Help / Next Steps

If a section here is unclear or you'd like more detail (e.g. typical test
patterns, how conversions are added, build script flags), let me know and I
can elaborate or add examples.

---

*Last updated March 2026.*
