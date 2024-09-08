#pragma warning disable CS8321 // Local function is declared but never used

#if NULLABLE_DISABLED_BUILD
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#endif

// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable RedundantCast
// ReSharper disable ClassNeverInstantiated.Global



// ReSharper disable RedundantNullableDirective

using System;
using System.Globalization;
using System.Threading.Tasks;
using JetBrains.Annotations;
using static Vogen.ComparisonGeneration;
using static Vogen.ParsableForPrimitives;
using static Vogen.ParsableForStrings;

namespace Vogen.Examples.TypicalScenarios.Nullable;

[UsedImplicitly]
internal class BasicExamples : IScenario
{
    public Task Run()
    {
        // nothing to show, just code examples for different nullability scenarios.
        return Task.CompletedTask;

        void NullableDisabledClassValueObjectWrappingAnInt()
        {
            // Error CS1503 : Argument 1: cannot convert from '<null>' to 'int'
            // Even though nullability was disabled, because it wraps an int, we can pass null.
            // _ = NullableDisabledClassWrappingInt.From(null);
            _ = NullableDisabledClassWrappingInt.TryFrom(null);
            
            var nd1 = NullableDisabledClassWrappingInt.From(1);
            var nd2 = NullableDisabledClassWrappingInt.From(2);

            // This is fine, there are no annotations for the parameters for parse.
            NullableDisabledClassWrappingInt.Parse(null);

            // Error CS8600 : Converting null literal or possible null value to non-nullable type.
            // Even though we turned off nullability for this value object, the signatures for some methods
            // are 'hoisted' from the primitive.
            // _ = NullableDisabledClassWrappingInt.TryParse((string)null, out _);
            
            _ = nd1.CompareTo(null);

            _ = nd1.Equals(null as object);
            _ = nd1.Equals(null as NullableDisabledClassWrappingInt);

            _ = nd1 == nd2;
            _ = nd1 == 1;
            _ = nd1 == null;
        }

        void NullableEnabledClassValueObjectWrappingAnInt()
        {
            // Error CS1503 : Argument 1: cannot convert from '<null>' to 'int'
            // Even though nullability was disabled, because it wraps an int, we can pass null.
            // NullableEnabledClassWrappingInt.From(null);
            // NullableEnabledClassWrappingInt.TryFrom(null);
            
            var nd1 = NullableEnabledClassWrappingInt.From(1);
            var nd2 = NullableEnabledClassWrappingInt.From(2);

            // Error CS8625 : Cannot convert null literal to non-nullable reference type
            // This is no longer fine, there is an annotation for the parameter.
            // NullableEnabledClassWrappingInt.Parse(null);

            // Error CS8600 : Converting null literal or possible null value to non-nullable type.
            // Even though we turned off nullability for this value object, the signatures for some methods
            // are 'hoisted' from the primitive.
            // _ = NullableEnabledClassWrappingInt.TryParse((string)null, out _);
            
            _ = nd1.CompareTo(null);

            _ = nd1.Equals(null);

            _ = nd1 == nd2;
            _ = nd1 == 1;
            _ = nd1 == null;
        }

        void NullableDisabledStructValueObjectWrappingAnInt()
        {
            // Error CS1503 : Argument 1: cannot convert from '<null>' to 'int'
            // Even though nullability was disabled, because it wraps an int, we can pass null.
            // _ = NullableDisabledStructWrappingInt.From(null);
            // NullableDisabledStructWrappingInt.TryFrom(null);
            
            var nd1 = NullableDisabledStructWrappingInt.From(1);
            var nd2 = NullableDisabledStructWrappingInt.From(2);

            // This is fine, there are no annotations for the parameters for parse.
            NullableDisabledStructWrappingInt.Parse(null);

            // Error CS8600 : Converting null literal or possible null value to non-nullable type.
            // Even though we turned off nullability for this value object, the signatures for some methods
            // are 'hoisted' from the primitive.
            // _ = NullableDisabledStructWrappingInt.TryParse((string)null, out _);
            
            _ = nd1.CompareTo(null);

            _ = nd1.Equals(null);

            _ = nd1 == nd2;
            _ = nd1 == 1;
            
            // makes no sense to compare to null as it's a struct, resulting in Error CS8073 : The result of the expression is always 'false'
            // _ = nd1 == null;
        }

        void NullableEnabledStructValueObjectWrappingAnInt()
        {
            // Error CS1503 : Argument 1: cannot convert from '<null>' to 'int'
            // Even though nullability was enabled, because the value object itself
            // is a class, we still get CS1503
            // NullableEnabledStructWrappingInt.From(null);
            // NullableEnabledStructWrappingInt.TryFrom(null);
            
            var nd1 = NullableEnabledStructWrappingInt.From(1);
            var nd2 = NullableEnabledStructWrappingInt.From(2);

            // Error CS8625 : Cannot convert null literal to non-nullable reference type
            // This is no longer fine, there is an annotation for the parameter.
            // NullableEnabledStructWrappingInt.Parse(null);

            // Error CS8600 : Converting null literal or possible null value to non-nullable type.
            // Even though we turned off nullability for this value object, the signatures for some methods
            // are 'hoisted' from the primitive.
            // _ = NullableEnabledStructWrappingInt.TryParse((string)null, out _);
            
            _ = nd1.CompareTo(null);

            _ = nd1.Equals(null);

            _ = nd1 == nd2;
            _ = nd1 == 1;
            // makes no sense to compare to null as it's a struct, resulting in Error CS8073 : The result of the expression is always 'false'
            // _ = nd1 == null;
        }

        void NullableDisabledClassValueObjectWrappingAString()
        {
            // No issues - we've disabled null when creating this value object, so no warnings
            NullableDisabledClassWrappingString c1 = NullableDisabledClassWrappingString.From(null);
            
            // CS8600 - even though nullability is disabled, the `MaybeNullWhen` creates the error. 
            // _ = NullableDisabledClassWrappingString.TryFrom(null, out NullableDisabledClassWrappingString c2);
            
            Console.WriteLine(c1.Value); // NullReferenceException
            
            var nd1 = NullableDisabledClassWrappingString.From("1");
            var nd2 = NullableDisabledClassWrappingString.From("2");

            // This is not fine - nullability is enabled right here, and, even though it's disabled
            // for the value object, the signature for that parameter in the method
            // signifies non-nullable.
            // NullableDisabledClassWrappingString.Parse(null, CultureInfo.InvariantCulture);

            // Error CS8600 : Converting null literal or possible null value to non-nullable type.
            // Even though we turned off nullability for this value object, the signatures for some methods
            // are 'hoisted' from the primitive.
            // _ = NullableDisabledClassWrappingString.TryParse((string)null, out _);
            
            _ = nd1.CompareTo(null);

            // not marked as nullable
            // _ = nd1.Equals((string)null);

            // CS8600 - as not marked as nullable
            // _ = nd1.Equals((NullableDisabledClassWrappingString)null);

            // CS8600 - as not marked as nullable
            // _ = nd1 == (string)null;

            _ = nd1 == nd2;
            _ = nd1 == "1";
        }

        void NullableEnabledClassValueObjectWrappingAString()
        {
            // CS8625 - cannot provide null as nullability is enabled in the generated value object
            // NullableEnabledClassWrappingString c1 = NullableEnabledClassWrappingString.From(null);
            
            // CS8600 - needs nullable 
            // _ = NullableEnabledClassWrappingString.TryFrom(null, out NullableEnabledClassWrappingString c2);
            
            // OK
            _ = NullableEnabledClassWrappingString.TryFrom(null, out NullableEnabledClassWrappingString? _);
            
            var vo1 = NullableEnabledClassWrappingString.From("1");
            var vo2 = NullableEnabledClassWrappingString.From("2");

            // CS8625 - cannot supply null
            // NullableEnabledClassWrappingString.Parse(null, CultureInfo.InvariantCulture);

            // OK
            _ = NullableEnabledClassWrappingString.TryParse(null, CultureInfo.InvariantCulture, out _);
            
            // CS8602 dereference of possibly null reference
            // Console.WriteLine(r.Value);
            
            _ = vo1.CompareTo(null);

            // not marked as nullable
            _ = vo1.Equals(null as string);

            // CS8600 - as not marked as nullable
            
            //OK
            _ = vo1.Equals(null as string);
            _ = vo1.Equals(null as NullableEnabledClassWrappingString);

            // OK
            _ = vo1 == null as string;
            _ = vo1 == null as NullableEnabledClassWrappingString;

            _ = vo1 == vo2;
            _ = vo1 == "1";
        }

        void NullableEnabledStructValueObjectWrappingString()
        {
            // CS8625 - cannot provide null as nullability is enabled in the generated value object
            // NullableEnabledStructWrappingString c1 = NullableEnabledStructWrappingString.From(null);
            
            // CS8600 - needs nullable 
            // _ = NullableEnabledStructWrappingString.TryFrom(null, out NullableEnabledStructWrappingString c2);
            
            // OK - nullable not needed because the value object is a value type
            _ = NullableEnabledStructWrappingString.TryFrom(null, out NullableEnabledStructWrappingString _);
            
            var vo1 = NullableEnabledStructWrappingString.From("1");
            var vo2 = NullableEnabledStructWrappingString.From("2");

            // CS8625 - cannot supply null
            // NullableEnabledStructWrappingString.Parse(null, CultureInfo.InvariantCulture);

            // OK
            _ = NullableEnabledStructWrappingString.TryParse(null, CultureInfo.InvariantCulture, out _);
            
            _ = vo1.CompareTo(null);

            // marked as nullable
            _ = vo1.Equals(null);

            //OK
            _ = vo1.Equals(null as string);

            // OK
            _ = vo1 == null as string;

            _ = vo1 == vo2;
            _ = vo1 == "1";
        }

        void NullableDisabledStructValueObjectWrappingString()
        {
            // OK - even though nullability is disabled, the 
            _ = NullableDisabledStructWrappingString.From(null);
            
            // OK
            _ = NullableDisabledStructWrappingString.TryFrom(null, out NullableDisabledStructWrappingString _);
            
            var vo1 = NullableDisabledStructWrappingString.From("1");
            var vo2 = NullableDisabledStructWrappingString.From("2");

            // CS8625 - cannot supply null
            // NullableDisabledStructWrappingString.Parse(null, CultureInfo.InvariantCulture);

            // OK
            _ = NullableDisabledStructWrappingString.TryParse(null, CultureInfo.InvariantCulture, out _);
            
            // CS8602 dereference of possibly null reference
            // Console.WriteLine(r.Value);
            
            _ = vo1.CompareTo(null);

            // not marked as nullable
            _ = vo1.Equals(null);

            // CS8600 - as not marked as nullable
            
            //OK
            _ = vo1.Equals(null);
            _ = vo1.Equals(null as object);

            // OK
            _ = vo1 == null as string;

            _ = vo1 == vo2;
            _ = vo1 == "1";
        }
    }
}

#nullable disable
[ValueObject(comparison:UseUnderlying, parsableForPrimitives: HoistMethodsAndInterfaces)]
public partial class NullableDisabledClassWrappingInt;

[ValueObject(comparison:UseUnderlying, parsableForPrimitives: HoistMethodsAndInterfaces)]
public partial struct NullableDisabledStructWrappingInt;

[ValueObject<string>(comparison:UseUnderlying, parsableForPrimitives: HoistMethodsAndInterfaces, parsableForStrings: GenerateMethodsAndInterface)]
public partial class NullableDisabledClassWrappingString;

[ValueObject<string>(comparison:UseUnderlying, parsableForPrimitives: HoistMethodsAndInterfaces, parsableForStrings: GenerateMethodsAndInterface)]
public partial struct NullableDisabledStructWrappingString;

#nullable restore

#nullable enable

[ValueObject(comparison:UseUnderlying, parsableForPrimitives: HoistMethodsAndInterfaces)]
public partial class NullableEnabledClassWrappingInt;

[ValueObject(comparison:UseUnderlying, parsableForPrimitives: HoistMethodsAndInterfaces)]
public partial struct NullableEnabledStructWrappingInt;

[ValueObject<string>(comparison:UseUnderlying, parsableForPrimitives: HoistMethodsAndInterfaces, parsableForStrings: GenerateMethodsAndInterface)]
public partial class NullableEnabledClassWrappingString;

[ValueObject<string>(comparison:UseUnderlying, parsableForPrimitives: HoistMethodsAndInterfaces, parsableForStrings: GenerateMethodsAndInterface)]
public partial struct NullableEnabledStructWrappingString;

#nullable restore