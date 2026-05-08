using System.Linq;
using System.Text;
using Analyzer.Utilities.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Vogen.Generators;

namespace Vogen;

#if NET10_0_OR_GREATER
/// <summary>
/// Generates the <see cref="System.Numerics.INumber{T}"/> or <see cref="System.Numerics.INumberBase{T}"/>
/// implementation for value objects whose underlying primitive type implements the corresponding interface.
///
/// Static abstract interface members (e.g. <c>double.One</c>) cannot be called on concrete
/// types in ordinary code — they require a constrained type parameter. So the generated code
/// includes private generic helper methods that forward every call through a type parameter,
/// which the C# compiler can then resolve to the concrete type's implementation.
///
/// Requires C# 11 or later.
/// </summary>
#elif NETSTANDARD
/// <summary>
/// Generates the System.Numerics.INumberBase of T or System.Numerics.INumberBase of T
/// implementation for value objects whose underlying primitive type implements the corresponding interface.
///
/// Static abstract interface members (e.g. <c>double.One</c>) cannot be called on concrete
/// types in ordinary code — they require a constrained type parameter. So the generated code
/// includes private generic helper methods that forward every call through a type parameter,
/// which the C# compiler can then resolve to the concrete type's implementation.
///
/// Requires C# 11 or later.
/// </summary>
#endif
public static class GenerateCodeForINumber
{
    private enum NumericMode { None, INumber, INumberBase }

    /// <summary>
    /// Returns the numeric interface declaration text (<c>INumber&lt;WrapperType&gt;</c> or
    /// <c>INumberBase&lt;WrapperType&gt;</c>) if the underlying primitive qualifies and C# 11+ is targeted.
    /// </summary>
    public static string GenerateInterfaceDefinitionsIfNeeded(string precedingText, GenerationParameters parameters)
    {
        var (mode, _) = DetermineMode(parameters);
        if (mode == NumericMode.None)
            return string.Empty;

        var wrapperName = parameters.WorkItem.TypeToAugment.Identifier;
        return mode == NumericMode.INumber
            ? $"{precedingText} global::System.Numerics.INumber<{wrapperName}>"
            : $"{precedingText} global::System.Numerics.INumberBase<{wrapperName}>";
    }

    /// <summary>
    /// Generates all static members needed to implement the numeric interface,
    /// skipping any members the user has already provided in their partial type.
    /// </summary>
    public static string GenerateINumberImplementationIfNeeded(GenerationParameters parameters)
    {
        var (mode, interfaceSymbol) = DetermineMode(parameters);
        if (mode == NumericMode.None)
            return string.Empty;

        var wrapperSymbol = parameters.WorkItem.WrapperType;

        // If the user has already implemented the interface themselves, don't generate.
        if (wrapperSymbol.DerivesFromOrImplementsAnyConstructionOf(interfaceSymbol!))
            return string.Empty;

        var wrapperName = parameters.WorkItem.TypeToAugment.Identifier.ToString();
        var primitiveType = parameters.WorkItem.UnderlyingTypeFullNameWithGlobalAlias;
        var item = parameters.WorkItem;
        bool isINumberBase = mode == NumericMode.INumberBase;

        var sb = new StringBuilder();

        // Private generic helpers let us call static abstract interface members, which
        // can only be called through a constrained type parameter, not directly on a
        // concrete type like `double`.
        GenerateHelperMethods(sb, isINumberBase);

        GenerateStaticProperties(sb, wrapperName, primitiveType, wrapperSymbol, item);
        GenerateBooleanChecks(sb, wrapperName, primitiveType, wrapperSymbol);
        GenerateSingleParamWrapperMethods(sb, wrapperName, primitiveType, wrapperSymbol);
        GenerateTwoParamWrapperMethods(sb, wrapperName, primitiveType, wrapperSymbol);
        GenerateGenericCreateMethods(sb, wrapperName, primitiveType, wrapperSymbol);
        GenerateTryConvertMethods(sb, wrapperName, primitiveType);

        if (!isINumberBase)
        {
            GenerateINumberMethods(sb, wrapperName, primitiveType, wrapperSymbol);
        }

        bool isUnsignedInteger = IsUnsignedIntegerType(parameters.WorkItem.UnderlyingType);
        GenerateArithmeticOperators(sb, wrapperName, primitiveType, wrapperSymbol, isINumberBase, isUnsignedInteger);

        if (!isINumberBase)
        {
            GenerateComparisonOperators(sb, wrapperName, wrapperSymbol);

            // Only generate CompareTo if GenerateCodeForComparables isn't already doing it.
            if (item.Config.Comparison != ComparisonGeneration.UseUnderlying)
            {
                GenerateCompareTo(sb, wrapperName, item);
            }
        }

        return sb.ToString();
    }

    private static (NumericMode mode, INamedTypeSymbol? symbol) DetermineMode(GenerationParameters parameters)
    {
        if (parameters.WorkItem.LanguageVersion < LanguageVersion.CSharp11)
            return (NumericMode.None, null);

        // INumber<T> requires IParsable<T> and ISpanParsable<T>. We can only fulfill
        // those requirements if the underlying type publicly exposes Parse/TryParse
        // methods (so Vogen can hoist them). Types like char implement INumber<char>
        // but their IParsable implementation is explicit-only and not hoistable.
        if (!parameters.WorkItem.ParsingInformation.UnderlyingDerivesFromIParsable)
            return (NumericMode.None, null);

        if (parameters.WorkItem.Config.NumericsGeneration != NumericsGeneration.Generate)
            return (NumericMode.None, null);

        var underlying = parameters.WorkItem.UnderlyingType;

        var inumber = parameters.VogenKnownSymbols.INumberOfT;
        if (inumber is not null && underlying.DerivesFromOrImplementsAnyConstructionOf(inumber))
            return (NumericMode.INumber, inumber);

        var ibase = parameters.VogenKnownSymbols.INumberBaseOfT;
        if (ibase is not null && underlying.DerivesFromOrImplementsAnyConstructionOf(ibase))
            return (NumericMode.INumberBase, ibase);

        return (NumericMode.None, null);
    }

    private static bool IsUnsignedIntegerType(ITypeSymbol type) =>
        type.SpecialType is SpecialType.System_Byte
            or SpecialType.System_UInt16
            or SpecialType.System_UInt32
            or SpecialType.System_UInt64
            or SpecialType.System_UIntPtr;

    private static bool StaticMemberExists(INamedTypeSymbol wrapperSymbol, string memberName) =>
        wrapperSymbol.GetMembers(memberName).Any(m => m.IsStatic);

    private static bool InstanceExists(VoWorkItem item, string memberName) =>
        item.InstanceProperties.Exists(i => i.Name == memberName);

    // -------------------------------------------------------------------------
    // Private generic helper methods
    //
    // Static abstract interface members (like INumberBase<T>.One) cannot be called
    // on a concrete type (e.g. `double.One`) — they can only be accessed through a
    // constrained type parameter. These private helpers bridge that gap by forwarding
    // each call through `__T`, which the JIT resolves to the concrete primitive type.
    // -------------------------------------------------------------------------
    private static void GenerateHelperMethods(StringBuilder sb, bool isINumberBase)
    {
        sb.AppendLine("#nullable disable");
        sb.AppendLine("    // Private helpers: forward static abstract INumberBase<T> / INumber<T> calls through a");
        sb.AppendLine("    // constrained type parameter so that the concrete primitive's implementations are invoked.");
        sb.AppendLine("    private static __T __GetOne<__T>() where __T : global::System.Numerics.INumberBase<__T> => __T.One;");
        sb.AppendLine("    private static global::System.Int32 __GetRadix<__T>() where __T : global::System.Numerics.INumberBase<__T> => __T.Radix;");
        sb.AppendLine("    private static __T __GetZero<__T>() where __T : global::System.Numerics.INumberBase<__T> => __T.Zero;");
        sb.AppendLine("    private static __T __GetAdditiveIdentity<__T>() where __T : global::System.Numerics.INumberBase<__T> => __T.AdditiveIdentity;");
        sb.AppendLine("    private static __T __GetMultiplicativeIdentity<__T>() where __T : global::System.Numerics.INumberBase<__T> => __T.MultiplicativeIdentity;");

        // Boolean checks
        string[] boolMethods =
        [
            "IsCanonical", "IsComplexNumber", "IsEvenInteger", "IsFinite",
            "IsImaginaryNumber", "IsInfinity", "IsInteger", "IsNaN",
            "IsNegative", "IsNegativeInfinity", "IsNormal", "IsOddInteger",
            "IsPositive", "IsPositiveInfinity", "IsRealNumber", "IsSubnormal", "IsZero",
        ];
        foreach (var m in boolMethods)
        {
            sb.AppendLine($"    private static global::System.Boolean __{m}<__T>(__T value) where __T : global::System.Numerics.INumberBase<__T> => __T.{m}(value);");
        }

        // Single-param value methods
        sb.AppendLine("    private static __T __Abs<__T>(__T value) where __T : global::System.Numerics.INumberBase<__T> => __T.Abs(value);");

        // Two-param value methods
        string[] twoParamMethods = ["MaxMagnitude", "MaxMagnitudeNumber", "MinMagnitude", "MinMagnitudeNumber"];
        foreach (var m in twoParamMethods)
        {
            sb.AppendLine($"    private static __T __{m}<__T>(__T x, __T y) where __T : global::System.Numerics.INumberBase<__T> => __T.{m}(x, y);");
        }

        // Generic creation methods
        sb.AppendLine("    private static __T __CreateChecked<__T, __TOther>(__TOther value) where __T : global::System.Numerics.INumberBase<__T> where __TOther : global::System.Numerics.INumberBase<__TOther> => __T.CreateChecked<__TOther>(value);");
        sb.AppendLine("    private static __T __CreateSaturating<__T, __TOther>(__TOther value) where __T : global::System.Numerics.INumberBase<__T> where __TOther : global::System.Numerics.INumberBase<__TOther> => __T.CreateSaturating<__TOther>(value);");
        sb.AppendLine("    private static __T __CreateTruncating<__T, __TOther>(__TOther value) where __T : global::System.Numerics.INumberBase<__T> where __TOther : global::System.Numerics.INumberBase<__TOther> => __T.CreateTruncating<__TOther>(value);");

        // TryConvert methods
        sb.AppendLine("    private static global::System.Boolean __TryConvertFromChecked<__T, __TOther>(__TOther value, out __T result) where __T : global::System.Numerics.INumberBase<__T> where __TOther : global::System.Numerics.INumberBase<__TOther> => __T.TryConvertFromChecked<__TOther>(value, out result);");
        sb.AppendLine("    private static global::System.Boolean __TryConvertFromSaturating<__T, __TOther>(__TOther value, out __T result) where __T : global::System.Numerics.INumberBase<__T> where __TOther : global::System.Numerics.INumberBase<__TOther> => __T.TryConvertFromSaturating<__TOther>(value, out result);");
        sb.AppendLine("    private static global::System.Boolean __TryConvertFromTruncating<__T, __TOther>(__TOther value, out __T result) where __T : global::System.Numerics.INumberBase<__T> where __TOther : global::System.Numerics.INumberBase<__TOther> => __T.TryConvertFromTruncating<__TOther>(value, out result);");
        sb.AppendLine("    private static global::System.Boolean __TryConvertToChecked<__T, __TOther>(__T value, out __TOther result) where __T : global::System.Numerics.INumberBase<__T> where __TOther : global::System.Numerics.INumberBase<__TOther> => __T.TryConvertToChecked<__TOther>(value, out result);");
        sb.AppendLine("    private static global::System.Boolean __TryConvertToSaturating<__T, __TOther>(__T value, out __TOther result) where __T : global::System.Numerics.INumberBase<__T> where __TOther : global::System.Numerics.INumberBase<__TOther> => __T.TryConvertToSaturating<__TOther>(value, out result);");
        sb.AppendLine("    private static global::System.Boolean __TryConvertToTruncating<__T, __TOther>(__T value, out __TOther result) where __T : global::System.Numerics.INumberBase<__T> where __TOther : global::System.Numerics.INumberBase<__TOther> => __T.TryConvertToTruncating<__TOther>(value, out result);");

        if (!isINumberBase)
        {
            // INumber<T> methods
            sb.AppendLine("    private static __T __Clamp<__T>(__T value, __T min, __T max) where __T : global::System.Numerics.INumber<__T> => __T.Clamp(value, min, max);");
            sb.AppendLine("    private static __T __CopySign<__T>(__T value, __T sign) where __T : global::System.Numerics.INumber<__T> => __T.CopySign(value, sign);");
            sb.AppendLine("    private static __T __Max<__T>(__T x, __T y) where __T : global::System.Numerics.INumber<__T> => __T.Max(x, y);");
            sb.AppendLine("    private static __T __MaxNumber<__T>(__T x, __T y) where __T : global::System.Numerics.INumber<__T> => __T.MaxNumber(x, y);");
            sb.AppendLine("    private static __T __Min<__T>(__T x, __T y) where __T : global::System.Numerics.INumber<__T> => __T.Min(x, y);");
            sb.AppendLine("    private static __T __MinNumber<__T>(__T x, __T y) where __T : global::System.Numerics.INumber<__T> => __T.MinNumber(x, y);");
            sb.AppendLine("    private static global::System.Int32 __Sign<__T>(__T value) where __T : global::System.Numerics.INumber<__T> => __T.Sign(value);");
        }

        sb.AppendLine("#nullable restore");
        sb.AppendLine();
    }

    private static void GenerateStaticProperties(StringBuilder sb, string wrapperName, string primitiveType, INamedTypeSymbol wrapperSymbol, VoWorkItem item)
    {
        // For each required INumberBase<T> static property:
        //  - If the user's partial already has it as a static member: skip (the user satisfies the interface).
        //  - If an [Instance] with that name exists: the instance generates a readonly field, which does NOT
        //    satisfy a static abstract property. Generate an explicit interface impl that delegates to the field.
        //  - Otherwise: generate the normal public property.
        GenerateNumberProperty(sb, wrapperName, wrapperSymbol, item,
            name: "One",
            iface: $"global::System.Numerics.INumberBase<{wrapperName}>",
            body: $"From(__GetOne<{primitiveType}>())");

        if (!StaticMemberExists(wrapperSymbol, "Radix") && !InstanceExists(item, "Radix"))
        {
            sb.AppendLine("    /// <inheritdoc />");
            sb.AppendLine($"    public static global::System.Int32 Radix => __GetRadix<{primitiveType}>();");
        }

        GenerateNumberProperty(sb, wrapperName, wrapperSymbol, item,
            name: "Zero",
            iface: $"global::System.Numerics.INumberBase<{wrapperName}>",
            body: $"From(__GetZero<{primitiveType}>())");

        GenerateNumberProperty(sb, wrapperName, wrapperSymbol, item,
            name: "AdditiveIdentity",
            iface: $"global::System.Numerics.IAdditiveIdentity<{wrapperName}, {wrapperName}>",
            body: $"From(__GetAdditiveIdentity<{primitiveType}>())");

        GenerateNumberProperty(sb, wrapperName, wrapperSymbol, item,
            name: "MultiplicativeIdentity",
            iface: $"global::System.Numerics.IMultiplicativeIdentity<{wrapperName}, {wrapperName}>",
            body: $"From(__GetMultiplicativeIdentity<{primitiveType}>())");

        sb.AppendLine();
    }

    private static void GenerateNumberProperty(
        StringBuilder sb,
        string wrapperName,
        INamedTypeSymbol wrapperSymbol,
        VoWorkItem item,
        string name,
        string iface,
        string body)
    {
        if (StaticMemberExists(wrapperSymbol, name))
            return; // user's partial satisfies the interface

        if (InstanceExists(item, name))
        {
            // [Instance] generates a readonly field, which doesn't satisfy static abstract property.
            // Generate an explicit interface implementation that delegates to the instance field.
            sb.AppendLine($"    static {wrapperName} {iface}.{name} => {name};");
        }
        else
        {
            sb.AppendLine("    /// <inheritdoc />");
            sb.AppendLine($"    public static {wrapperName} {name} => {body};");
        }
    }

    private static void GenerateBooleanChecks(StringBuilder sb, string wrapperName, string primitiveType, INamedTypeSymbol wrapperSymbol)
    {
        string[] methods =
        [
            "IsCanonical",
            "IsComplexNumber",
            "IsEvenInteger",
            "IsFinite",
            "IsImaginaryNumber",
            "IsInfinity",
            "IsInteger",
            "IsNaN",
            "IsNegative",
            "IsNegativeInfinity",
            "IsNormal",
            "IsOddInteger",
            "IsPositive",
            "IsPositiveInfinity",
            "IsRealNumber",
            "IsSubnormal",
            "IsZero",
        ];

        foreach (var method in methods)
        {
            if (!StaticMemberExists(wrapperSymbol, method))
            {
                sb.AppendLine("    /// <inheritdoc />");
                sb.AppendLine($"    public static global::System.Boolean {method}({wrapperName} value) => __{method}<{primitiveType}>(value.Value);");
            }
        }

        sb.AppendLine();
    }

    private static void GenerateSingleParamWrapperMethods(StringBuilder sb, string wrapperName, string primitiveType, INamedTypeSymbol wrapperSymbol)
    {
        if (!StaticMemberExists(wrapperSymbol, "Abs"))
        {
            sb.AppendLine("    /// <inheritdoc />");
            sb.AppendLine($"    public static {wrapperName} Abs({wrapperName} value) => From(__Abs<{primitiveType}>(value.Value));");
        }

        sb.AppendLine();
    }

    private static void GenerateTwoParamWrapperMethods(StringBuilder sb, string wrapperName, string primitiveType, INamedTypeSymbol wrapperSymbol)
    {
        string[] methods = ["MaxMagnitude", "MaxMagnitudeNumber", "MinMagnitude", "MinMagnitudeNumber"];

        foreach (var method in methods)
        {
            if (!StaticMemberExists(wrapperSymbol, method))
            {
                sb.AppendLine("    /// <inheritdoc />");
                sb.AppendLine($"    public static {wrapperName} {method}({wrapperName} x, {wrapperName} y) => From(__{method}<{primitiveType}>(x.Value, y.Value));");
            }
        }

        sb.AppendLine();
    }

    private static void GenerateGenericCreateMethods(StringBuilder sb, string wrapperName, string primitiveType, INamedTypeSymbol wrapperSymbol)
    {
        (string name, string helper)[] methods =
        [
            ("CreateChecked", "__CreateChecked"),
            ("CreateSaturating", "__CreateSaturating"),
            ("CreateTruncating", "__CreateTruncating"),
        ];

        foreach (var (name, helper) in methods)
        {
            if (!StaticMemberExists(wrapperSymbol, name))
            {
                sb.AppendLine("    /// <inheritdoc />");
                sb.AppendLine($"    public static {wrapperName} {name}<TOther>(TOther value)");
                sb.AppendLine($"        where TOther : global::System.Numerics.INumberBase<TOther>");
                sb.AppendLine($"        => From({helper}<{primitiveType}, TOther>(value));");
            }
        }

        sb.AppendLine();
    }

    private static void GenerateTryConvertMethods(StringBuilder sb, string wrapperName, string primitiveType)
    {
        sb.AppendLine("#nullable disable");

        (string iface, string helper)[] fromMethods =
        [
            ("TryConvertFromChecked",   "__TryConvertFromChecked"),
            ("TryConvertFromSaturating","__TryConvertFromSaturating"),
            ("TryConvertFromTruncating","__TryConvertFromTruncating"),
        ];

        foreach (var (iface, helper) in fromMethods)
        {
            sb.AppendLine($"    static global::System.Boolean global::System.Numerics.INumberBase<{wrapperName}>.{iface}<TOther>(TOther value, out {wrapperName} result)");
            sb.AppendLine("    {");
            sb.AppendLine("        result = default;");
            sb.AppendLine($"        if (!{helper}<{primitiveType}, TOther>(value, out {primitiveType} primitiveResult))");
            sb.AppendLine("            return false;");
            sb.AppendLine("        result = From(primitiveResult);");
            sb.AppendLine("        return true;");
            sb.AppendLine("    }");
            sb.AppendLine();
        }

        (string iface, string helper)[] toMethods =
        [
            ("TryConvertToChecked",    "__TryConvertToChecked"),
            ("TryConvertToSaturating", "__TryConvertToSaturating"),
            ("TryConvertToTruncating", "__TryConvertToTruncating"),
        ];

        foreach (var (iface, helper) in toMethods)
        {
            sb.AppendLine($"    static global::System.Boolean global::System.Numerics.INumberBase<{wrapperName}>.{iface}<TOther>({wrapperName} value, out TOther result)");
            sb.AppendLine("    {");
            sb.AppendLine("        result = default;");
            sb.AppendLine($"        return {helper}<{primitiveType}, TOther>(value.Value, out result);");
            sb.AppendLine("    }");
            sb.AppendLine();
        }

        sb.AppendLine("#nullable restore");
        sb.AppendLine();
    }

    private static void GenerateINumberMethods(StringBuilder sb, string wrapperName, string primitiveType, INamedTypeSymbol wrapperSymbol)
    {
        if (!StaticMemberExists(wrapperSymbol, "Clamp"))
        {
            sb.AppendLine("    /// <inheritdoc />");
            sb.AppendLine($"    public static {wrapperName} Clamp({wrapperName} value, {wrapperName} min, {wrapperName} max) => From(__Clamp<{primitiveType}>(value.Value, min.Value, max.Value));");
        }

        if (!StaticMemberExists(wrapperSymbol, "CopySign"))
        {
            sb.AppendLine("    /// <inheritdoc />");
            sb.AppendLine($"    public static {wrapperName} CopySign({wrapperName} value, {wrapperName} sign) => From(__CopySign<{primitiveType}>(value.Value, sign.Value));");
        }

        (string name, string helper)[] twoParamMethods =
        [
            ("Max", "__Max"),
            ("MaxNumber", "__MaxNumber"),
            ("Min", "__Min"),
            ("MinNumber", "__MinNumber"),
        ];

        foreach (var (name, helper) in twoParamMethods)
        {
            if (!StaticMemberExists(wrapperSymbol, name))
            {
                sb.AppendLine("    /// <inheritdoc />");
                sb.AppendLine($"    public static {wrapperName} {name}({wrapperName} x, {wrapperName} y) => From({helper}<{primitiveType}>(x.Value, y.Value));");
            }
        }

        if (!StaticMemberExists(wrapperSymbol, "Sign"))
        {
            sb.AppendLine("    /// <inheritdoc />");
            sb.AppendLine($"    public static global::System.Int32 Sign({wrapperName} value) => __Sign<{primitiveType}>(value.Value);");
        }

        sb.AppendLine();
    }

    private static void GenerateArithmeticOperators(StringBuilder sb, string wrapperName, string primitiveType, INamedTypeSymbol wrapperSymbol, bool isINumberBase, bool isUnsignedInteger)
    {
        // Binary arithmetic operators — these use the concrete type's built-in C# operators,
        // which are regular static methods (not static abstract interface members), so they
        // can be called directly on the underlying value type.
        if (!wrapperSymbol.ImplementsOperator("op_Addition"))
        {
            sb.AppendLine("    /// <inheritdoc />");
            sb.AppendLine($"    public static {wrapperName} operator +({wrapperName} left, {wrapperName} right) => From(({primitiveType})(left.Value + right.Value));");
        }

        if (!wrapperSymbol.ImplementsOperator("op_Subtraction"))
        {
            sb.AppendLine("    /// <inheritdoc />");
            sb.AppendLine($"    public static {wrapperName} operator -({wrapperName} left, {wrapperName} right) => From(({primitiveType})(left.Value - right.Value));");
        }

        if (!wrapperSymbol.ImplementsOperator("op_Multiply"))
        {
            sb.AppendLine("    /// <inheritdoc />");
            sb.AppendLine($"    public static {wrapperName} operator *({wrapperName} left, {wrapperName} right) => From(({primitiveType})(left.Value * right.Value));");
        }

        if (!wrapperSymbol.ImplementsOperator("op_Division"))
        {
            sb.AppendLine("    /// <inheritdoc />");
            sb.AppendLine($"    public static {wrapperName} operator /({wrapperName} left, {wrapperName} right) => From(({primitiveType})(left.Value / right.Value));");
        }

        // INumber<T> requires IModulusOperators<T,T,T> (the % operator); INumberBase<T> does not.
        // System.Complex, for example, has no modulo operator.
        if (!isINumberBase && !wrapperSymbol.ImplementsOperator("op_Modulus"))
        {
            sb.AppendLine("    /// <inheritdoc />");
            sb.AppendLine($"    public static {wrapperName} operator %({wrapperName} left, {wrapperName} right) => From(({primitiveType})(left.Value % right.Value));");
        }

        // Unary operators
        if (!wrapperSymbol.ImplementsOperator("op_UnaryNegation"))
        {
            sb.AppendLine("    /// <inheritdoc />");
            // For unsigned integer types, unary `-` is not valid in C# (ulong) or narrows incorrectly (uint).
            // Mirror what the BCL does: IUnaryNegationOperators<T,T> for unsigned types is 0 - value (wrapping).
            // The explicit cast back to primitiveType is required for sub-int types (byte, ushort) because
            // their subtraction promotes to int in C#.
            string unaryNegBody = isUnsignedInteger
                ? $"unchecked(({primitiveType})(default({primitiveType}) - value.Value))"
                : $"({primitiveType})(-value.Value)";
            sb.AppendLine($"    public static {wrapperName} operator -({wrapperName} value) => From({unaryNegBody});");
        }

        if (!wrapperSymbol.ImplementsOperator("op_UnaryPlus"))
        {
            sb.AppendLine("    /// <inheritdoc />");
            sb.AppendLine($"    public static {wrapperName} operator +({wrapperName} value) => From(({primitiveType})(+value.Value));");
        }

        // Increment / decrement — use ++ and -- on the underlying value directly
        // rather than calling the static abstract IIncrementOperators<T>.++ member.
        if (!wrapperSymbol.ImplementsOperator("op_Increment"))
        {
            sb.AppendLine("    /// <inheritdoc />");
            sb.AppendLine($"    public static {wrapperName} operator ++({wrapperName} value) {{ var v = value.Value; return From(++v); }}");
        }

        if (!wrapperSymbol.ImplementsOperator("op_Decrement"))
        {
            sb.AppendLine("    /// <inheritdoc />");
            sb.AppendLine($"    public static {wrapperName} operator --({wrapperName} value) {{ var v = value.Value; return From(--v); }}");
        }

        sb.AppendLine();
    }

    private static void GenerateComparisonOperators(StringBuilder sb, string wrapperName, INamedTypeSymbol wrapperSymbol)
    {
        // These use the concrete type's built-in C# comparison operators directly.
        if (!wrapperSymbol.ImplementsOperator("op_LessThan"))
        {
            sb.AppendLine("    /// <inheritdoc />");
            sb.AppendLine($"    public static global::System.Boolean operator <({wrapperName} left, {wrapperName} right) => left.Value < right.Value;");
        }

        if (!wrapperSymbol.ImplementsOperator("op_LessThanOrEqual"))
        {
            sb.AppendLine("    /// <inheritdoc />");
            sb.AppendLine($"    public static global::System.Boolean operator <=({wrapperName} left, {wrapperName} right) => left.Value <= right.Value;");
        }

        if (!wrapperSymbol.ImplementsOperator("op_GreaterThan"))
        {
            sb.AppendLine("    /// <inheritdoc />");
            sb.AppendLine($"    public static global::System.Boolean operator >({wrapperName} left, {wrapperName} right) => left.Value > right.Value;");
        }

        if (!wrapperSymbol.ImplementsOperator("op_GreaterThanOrEqual"))
        {
            sb.AppendLine("    /// <inheritdoc />");
            sb.AppendLine($"    public static global::System.Boolean operator >=({wrapperName} left, {wrapperName} right) => left.Value >= right.Value;");
        }

        sb.AppendLine();
    }

    private static void GenerateCompareTo(StringBuilder sb, string wrapperName, VoWorkItem item)
    {
        string wrapperQ = item.Nullable.QuestionMarkForWrapper;

        if (item.IsTheWrapperAReferenceType)
        {
            sb.AppendLine($$"""
                    public global::System.Int32 CompareTo({{wrapperName}}{{wrapperQ}} other)
                    {
                        if (other is null) return 1;
                        return Value.CompareTo(other.Value);
                    }
                """);
        }
        else
        {
            sb.AppendLine($"    public global::System.Int32 CompareTo({wrapperName} other) => Value.CompareTo(other.Value);");
        }

        sb.AppendLine($$"""
                public global::System.Int32 CompareTo(object{{item.Nullable.QuestionMarkForOtherReferences}} other)
                {
                    if (other is null) return 1;
                    if (other is {{wrapperName}} x) return CompareTo(x);
                    ThrowHelper.ThrowArgumentException("Cannot compare to object as it is not of type {{wrapperName}}", nameof(other));
                    return 0;
                }
            """);

        sb.AppendLine();
    }
}
