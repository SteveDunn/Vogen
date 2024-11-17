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

        public static Lazy<ReferenceAssemblies> Net90AndOurs = new(() =>
            new ReferenceAssemblies(
                    "net9.0",
                    new PackageIdentity(
                        "Microsoft.NETCore.App.Ref",
                        "9.0.0"),
                    Path.Combine("ref", "net9.0"))
                .AddAssemblies(
                    ImmutableArray.Create("Vogen", "Vogen.SharedTypes", _loc.Replace(".dll", string.Empty))));

        public static Lazy<ReferenceAssemblies> Net90WithEfCoreAndOurs = new(
            () =>
                new ReferenceAssemblies("net9.0", new PackageIdentity("Microsoft.NETCore.App.Ref", "9.0.0"), Path.Combine("ref", "net9.0"))
                    .AddAssemblies(ImmutableArray.Create("Vogen", "Vogen.SharedTypes", _loc.Replace(".dll", string.Empty)))
                    .AddPackages(ImmutableArray.Create(new PackageIdentity("Microsoft.EntityFrameworkCore", "9.0.0"))));
    }
}