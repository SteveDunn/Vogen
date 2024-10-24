using System;
using Vogen;

namespace N;

[ValueObject<DateOnly>]
public partial struct BirthDate;

class C
{
    C() => BirthDate.From(DateOnly.MinValue).ToString();
}