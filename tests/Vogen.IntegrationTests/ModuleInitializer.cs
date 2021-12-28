using System.Runtime.CompilerServices;
using VerifyTests;

namespace Vogen.IntegrationTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init() => VerifySourceGenerators.Enable();
}