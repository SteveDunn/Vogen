using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Vogen.Examples.SerializationAndConversion.EFCore;

internal class SomeIdValueGenerator : ValueGenerator<Id>
{
    public override Id Next(EntityEntry entry)
    {
        var entities = ((DbContext)entry.Context).Entities;

        var next = Math.Max(MaxFrom(entities.Local), MaxFrom(entities)) + 1;

        return Id.From(next);

        static int MaxFrom(IEnumerable<PersonEntity> es)
        {
            return es.Any() ? es.Max(e => e.Id.Value) : 0;
        }
    }

    public override bool GeneratesTemporaryValues => false;
}