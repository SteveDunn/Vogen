using System;
using FluentAssertions;
using Xunit;

namespace Vogen.Tests;

public class TemplatesTests
{
    //todo: theory with other handlers
    [Fact]
    public void CanGetDateOnly() => Templates.TryGetForSpecificType(typeof(DateOnly), "DapperTypeHandler").Should().NotBeNull();

    [Fact]
    public void CanGetTimeOnly() => Templates.TryGetForSpecificType(typeof(TimeOnly), "DapperTypeHandler").Should().NotBeNull();
}