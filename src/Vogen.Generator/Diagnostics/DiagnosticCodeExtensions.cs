namespace Vogen.Generator.Diagnostics;

public static class DiagnosticCodeExtensions
{
    public static string Format(this DiagnosticCode code) => $"VOG{(int)code:D3}";
}
