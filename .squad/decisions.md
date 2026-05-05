# Team Decisions

**Canonical decision ledger.** Append-only. Agents read this before starting work. New decisions written to `.squad/decisions/inbox/` by agents; Scribe merges them here.

## 2026-04-26: Team Initialization

**By:** Squad (Coordinator)

**What:** Initial Squad team cast for Vogen project.
- Keaton (Lead) — Architecture, Roslyn versioning, decisions
- Fenster (Backend) — Generator templates, conversions, code emission
- Dallas (Frontend) — Analyzer diagnostics, CodeFixer logic
- Hockney (Tester) — Snapshot tests, AnalyzerTests, consumer validation
- Scribe (Session Logger) — Memory, decisions, orchestration logs
- Ralph (Work Monitor) — Issue triage, work queue management

**Why:** Vogen is a source-generator project requiring deep Roslyn expertise, multi-framework testing, and careful architectural oversight. This team size and shape matches the project's scope and technical depth.

---

## 2026-05-05: Fix Issue #838 - Incorrect Protobuf Documentation

**By:** Fenster (Backend Dev)

**What:** Fix documentation errors in README.md and FAQ.md regarding protobuf-net surrogate configuration.

**Context:** GitHub issue #838 reported that the documented protobuf pattern `Surrogate = typeof(string)` fails in protobuf-net v3 with error: `'Data of this type has inbuilt behaviour, and cannot be added to a model in this way: System.String'`

**Investigation Findings:**
- Vogen does NOT generate protobuf-specific code; no `Conversions.ProtoBuf` flag exists
- The documented pattern is invalid per protobuf-net rules: you cannot use a primitive type like `string` as a surrogate
- A surrogate type MUST be a `[ProtoContract]` class, not a primitive
- This is a documentation issue, NOT a code generation bug

**Decision:** Fix the documentation by replacing the invalid pattern with a correct surrogate DTO class pattern:
```csharp
[ProtoContract]
public class StringSurrogate
{
    [ProtoMember(1)]
    public string Value { get; set; }
}

[ValueObject(Surrogate = typeof(StringSurrogate))]
public partial struct MyValueObject;
```

**Files Changed:**
- README.md — updated protobuf surrogate example
- docs/site/Writerside/topics/reference/FAQ.md — corrected FAQ entry with working pattern and explanatory note

**Commit:** 6f6e919302

**Why:** Prevents users from encountering serialization errors; aligns documentation with protobuf-net v3 requirements.

**Future:** Consider adding proper protobuf support (Option 3) as a feature enhancement in next quarter.

---

## End of decisions

New decisions will be merged here by Scribe from `.squad/decisions/inbox/`.
