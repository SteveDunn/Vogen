using Vogen;

namespace N2;

[ValueObject<int>(conversions: Conversions.MessagePack)]
public partial class MyString;