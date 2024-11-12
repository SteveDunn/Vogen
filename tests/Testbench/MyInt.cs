using Vogen;

namespace N1;

[ValueObject<int>(conversions: Conversions.MessagePack)]
public partial class MyInt;