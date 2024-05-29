; Shipped analyzer releases
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

## Release 3.0.3

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
AddValidationMethod | Usage | Info | AddValidationAnalyzer
VOG010 | Usage | Error | DoNotUseNewAnalyzer
VOG009 | Usage | Error | DoNotUseDefaultAnalyzer
VOG025 | Usage | Error | DoNotUseReflection

## Release 4.0.1

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
VOG026 | Usage | Warning  | DoNotDeriveFromVogenAttributes

## Release 4.0.2

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
VOG027 | Usage | Error | DoNotUseNewAnalyzer


## Release 4.0.5

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
AddNormalizeInputMethod | Usage | Info | AddNormalizeInputAnalyzer
AddStaticToExistingNormalizeInputMethod | Usage | Info | AddNormalizeInputAnalyzer
AddStaticToExistingValidationMethod | Usage | Info | AddValidationAnalyzer
FixInputTypeOfValidationMethod | Usage | Info | ValidationMethodAnalyzer
VOG028 | Usage | Info | NormalizeInputMethodAnalyzer

## Release 4.0.6

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
VOG029 | Usage | Error | SpecifyTypeExplicitlyInValueObjectAttributeAnalzyer