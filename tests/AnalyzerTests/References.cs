using System;
using System.Collections.Immutable;
using System.IO;
using Microsoft.CodeAnalysis.Testing;
using Vogen;

namespace AnalyzerTests
{
    public static class References
    {
        static readonly string _loc = typeof(ValueObjectAttribute).Assembly.Location;

        public static Lazy<ReferenceAssemblies> Net80AndOurs = new(() =>
            new ReferenceAssemblies(
                    "net8.0",
                    new PackageIdentity(
                        "Microsoft.NETCore.App.Ref",
                        "8.0.0"),
                    Path.Combine("ref", "net8.0"))
                .AddAssemblies(
                    ImmutableArray.Create("Vogen", "Vogen.SharedTypes", _loc.Replace(".dll", string.Empty))));

        public static Lazy<ReferenceAssemblies> Net80WithEfCoreAndOurs = new(
            () =>
                new ReferenceAssemblies("net8.0", new PackageIdentity("Microsoft.NETCore.App.Ref", "8.0.0"), Path.Combine("ref", "net8.0"))
                    .AddAssemblies(ImmutableArray.Create("Vogen", "Vogen.SharedTypes", _loc.Replace(".dll", string.Empty)))
                    .AddPackages(ImmutableArray.Create(new PackageIdentity("Microsoft.EntityFrameworkCore", "8.0.10"))));
    }
}