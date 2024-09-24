using System;
using System.Globalization;
using Vogen;
[assembly: VogenDefaults(disableStackTraceRecordingInDebug: true)]

namespace Whatever.Validation;

[ValueObject(typeof(string))]
public readonly partial record struct CustomerId;

[ValueObject<byte>]
[Instance("None", "0")]
public readonly partial record struct TenantId
{
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, out TenantId result)
    {
        bool parsed = TryParse(utf8Text, CultureInfo.InvariantCulture,  out result);
        result = parsed ? result : None;

        return parsed;
    }
}