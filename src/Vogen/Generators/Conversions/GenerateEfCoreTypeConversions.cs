using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Generators.Conversions;

internal class GenerateEfCoreTypeConversions : IGenerateConversion
{
    public string GenerateAnyAttributes(TypeDeclarationSyntax tds, VoWorkItem item)
    {
        return string.Empty;
    }

    public string GenerateAnyBody(TypeDeclarationSyntax tds, VoWorkItem item)
    {
        if (!IsOurs(item.Config.Conversions))
        {
            return string.Empty;
        }

        return GenerateAnyBody3(true, item.UnderlyingType, item.IsTheWrapperAValueType, item.WrapperType);
    }

    public string GenerateAnyBody_Old(TypeDeclarationSyntax tds, VoWorkItem item)
    {
        if (!IsOurs(item.Config.Conversions))
        {
            return string.Empty;
        }

        string code = Templates.TryGetForSpecificType(item.UnderlyingType, "EfCoreValueConverter") ??
                      Templates.GetForAnyType("EfCoreValueConverter");

        code = code.Replace("__CLASS_PREFIX__", string.Empty);
        code = code.Replace("CLASS_PREFIX", string.Empty);

        if (item.IsTheWrapperAValueType)
        {
            code += """
                    
                            public class EfCoreValueComparer : global::Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<VOTYPE>
                            {
                                public EfCoreValueComparer() : base(
                                    (left, right) => DoCompare(left, right), 
                                    instance => instance._isInitialized ? instance.GetHashCode() : 0) 
                                { 
                                }
                                
                                static bool DoCompare(VOTYPE left, VOTYPE right)
                                {
                                    // if neither are initialized, then they're equal
                                    if(!left._isInitialized && !right._isInitialized) return true;
                                    
                                    return left._isInitialized && right._isInitialized && left._value.Equals(right._value);
                                }
                            }
                    """;
        }
        else
        {
            code += """
                    
                            public class EfCoreValueComparer : global::Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<VOTYPE>
                            {
                                public EfCoreValueComparer() : base(
                                    (left, right) => DoCompare(left, right), 
                                    instance => instance._isInitialized ? instance._value.GetHashCode() : 0) 
                                { 
                                }
                                    
                                static bool DoCompare(VOTYPE left, VOTYPE right)
                                {
                                    // if both null, then they're equal
                                    if (left is null) return right is null;
                                    
                                    // if only right is null, then they're not equal
                                    if (right is null) return false;
                                    
                                    // if they're both the same reference, then they're equal
                                    if (ReferenceEquals(left, right)) return true;
                                    
                                    // if neither are initialized, then they're equal
                                    if(!left._isInitialized && !right._isInitialized) return true;
                                    
                                    return left._isInitialized && right._isInitialized && left._value.Equals(right._value);            
                                }                
                            }
                    """;
        }

        code = code.Replace("VOTYPE", item.VoTypeName);
        code = code.Replace("VOUNDERLYINGTYPE", item.UnderlyingTypeFullName);
        
        return code;
    }

    public string GenerateAnyBody_Old2(TypeDeclarationSyntax tds, VoWorkItem item)
    {
        if (!IsOurs(item.Config.Conversions))
        {
            return string.Empty;
        }

        string code = Templates.TryGetForSpecificType(item.UnderlyingType, "EfCoreValueConverter") ??
                      Templates.GetForAnyType("EfCoreValueConverter");

        code = code.Replace("__CLASS_PREFIX__", string.Empty);
        code = code.Replace("CLASS_PREFIX", string.Empty);

        if (item.IsTheWrapperAValueType)
        {
            code += """
                    
                            public class EfCoreValueComparer : global::Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<VOTYPE>
                            {
                                public EfCoreValueComparer() : base(
                                    (left, right) => DoCompare(left, right), 
                                    instance => instance._isInitialized ? instance.GetHashCode() : 0) 
                                { 
                                }
                                
                                static bool DoCompare(VOTYPE left, VOTYPE right)
                                {
                                    // if neither are initialized, then they're equal
                                    if(!left._isInitialized && !right._isInitialized) return true;
                                    
                                    return left._isInitialized && right._isInitialized && left._value.Equals(right._value);
                                }
                            }
                    """;
        }
        else
        {
            code += """
                    
                            public class EfCoreValueComparer : global::Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<VOTYPE>
                            {
                                public EfCoreValueComparer() : base(
                                    (left, right) => DoCompare(left, right), 
                                    instance => instance._isInitialized ? instance._value.GetHashCode() : 0) 
                                { 
                                }
                                    
                                static bool DoCompare(VOTYPE left, VOTYPE right)
                                {
                                    // if both null, then they're equal
                                    if (left is null) return right is null;
                                    
                                    // if only right is null, then they're not equal
                                    if (right is null) return false;
                                    
                                    // if they're both the same reference, then they're equal
                                    if (ReferenceEquals(left, right)) return true;
                                    
                                    // if neither are initialized, then they're equal
                                    if(!left._isInitialized && !right._isInitialized) return true;
                                    
                                    return left._isInitialized && right._isInitialized && left._value.Equals(right._value);            
                                }                
                            }
                    """;
        }

        code = code.Replace("VOTYPE", item.VoTypeName);
        code = code.Replace("VOUNDERLYINGTYPE", item.UnderlyingTypeFullName);
        
        return code;
    }

    private const string _converterForKnownTypes = 
"""

        public class __CLASS_PREFIX__EfCoreValueConverter : global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<VOTYPE, VOUNDERLYINGTYPE>
        {
          public __CLASS_PREFIX__EfCoreValueConverter() : this(null) { }
          public __CLASS_PREFIX__EfCoreValueConverter(global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints mappingHints = null)
              : base(
                  vo => vo.Value,
__WHEN_INNER__                                                                      value => VOTYPE.__Deserialize(value),
__WHEN_OUTER__                                                                      value => Deserialize(value),
                mappingHints
              ) { }
__WHEN_OUTER__      static VOTYPE Deserialize(VOUNDERLYINGTYPE value) => UnsafeDeserialize(default, value);
__WHEN_OUTER__ 
__WHEN_OUTER__      [global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.StaticMethod, Name = "__Deserialize")]
__WHEN_OUTER__      extern static ref VOTYPE UnsafeDeserialize(VOTYPE @this, VOUNDERLYINGTYPE value);      
        }
""";

    private const string _converterForAnyOtherType =
"""


    public class __CLASS_PREFIX__EfCoreValueConverter : global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<VOTYPE, global::System.String>
    {
        public __CLASS_PREFIX__EfCoreValueConverter() : this(null) { }
        public __CLASS_PREFIX__EfCoreValueConverter(global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints mappingHints = null)
            : base(
                vo => global::System.Text.Json.JsonSerializer.Serialize(vo.Value, default(global::System.Text.Json.JsonSerializerOptions)),
__WHEN_INNER__            text => VOTYPE.__Deserialize(global::System.Text.Json.JsonSerializer.Deserialize<VOUNDERLYINGTYPE>(text, default(global::System.Text.Json.JsonSerializerOptions))),
__WHEN_OUTER__            text => ProxyForCall__Deserialize(global::System.Text.Json.JsonSerializer.Deserialize<VOUNDERLYINGTYPE>(text, default(global::System.Text.Json.JsonSerializerOptions))),
                mappingHints
            ) { }
__WHEN_OUTER__  static VOTYPE ProxyForCall__Deserialize(VOUNDERLYINGTYPE value) => Call__Deserialize(default, value);
__WHEN_OUTER__ 
__WHEN_OUTER__  [global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.StaticMethod, Name = "__Deserialize")]
__WHEN_OUTER__  extern static ref VOTYPE Call__Deserialize(VOTYPE @this, VOUNDERLYINGTYPE value);      
    }
""";

    private const string _comparerForValuesTypes =
"""

        public class __CLASS_PREFIX__EfCoreValueComparer : global::Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<VOTYPE>
        {
            public __CLASS_PREFIX__EfCoreValueComparer() : base(
                (left, right) => DoCompare(left, right), 
                instance => instance.IsInitialized() ? instance.GetHashCode() : 0) 
            { 
            }
            
            static bool DoCompare(VOTYPE left, VOTYPE right)
            {
                // if neither are initialized, then they're equal
                if(!left.IsInitialized() && !right.IsInitialized()) return true;
                
__WHEN_INNER__                return left._isInitialized && right._isInitialized && left._value.Equals(right._value);
__WHEN_OUTER__                return left.IsInitialized() && right.IsInitialized() && UnderlyingValue(left).Equals(UnderlyingValue(right));
            }
__WHEN_OUTER__ private static VOUNDERLYINGTYPE UnderlyingValue(VOTYPE i) => UnsafeValueField(i);
__WHEN_OUTER__
__WHEN_OUTER__  [global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "_value")]
__WHEN_OUTER__  extern static ref VOUNDERLYINGTYPE UnsafeValueField(VOTYPE @this);                
        }
""";

    public const string _comparerForReferenceTypes = $$"""

       public class __CLASS_PREFIX__EfCoreValueComparer : global::Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<VOTYPE>
       {
           public __CLASS_PREFIX__EfCoreValueComparer() : base(
               (left, right) => DoCompare(left, right), 
__WHEN_INNER__                instance => instance._isInitialized ? instance._value.GetHashCode() : 0) 
__WHEN_OUTER__                instance => instance.IsInitialized() ? UnderlyingValue(instance).GetHashCode() : 0) 
           { 
           }
               
           static bool DoCompare(VOTYPE left, VOTYPE right)
           {
               // if both null, then they're equal
               if (left is null) return right is null;
               
               // if only right is null, then they're not equal
               if (right is null) return false;
               
               // if they're both the same reference, then they're equal
               if (ReferenceEquals(left, right)) return true;
               
               // if neither are initialized, then they're equal
__WHEN_INNER__                if(!left._isInitialized && !right._isInitialized) return true;
__WHEN_OUTER__                if(!left.IsInitialized() && !right.IsInitialized()) return true;
               
__WHEN_INNER__                return left._isInitialized && right._isInitialized && left._value.Equals(right._value);            
__WHEN_OUTER__                return left.IsInitialized() && right.IsInitialized() && UnderlyingValue(left).Equals(UnderlyingValue(right));            
           }                
__WHEN_OUTER__ private static VOUNDERLYINGTYPE UnderlyingValue(VOTYPE i) => UnsafeValueField(i);
__WHEN_OUTER__
__WHEN_OUTER__  [global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "_value")]
__WHEN_OUTER__  extern static ref VOUNDERLYINGTYPE UnsafeValueField(VOTYPE @this);                
}
""";        
    
    public static string GenerateAnyBody2(INamedTypeSymbol primitiveType, bool isWrapperAValueType, INamedTypeSymbol voSymbol)
    {
        return GenerateAnyBody3(false, primitiveType, isWrapperAValueType, voSymbol);
        // var primitiveFullName = primitiveType.FullName() ?? primitiveType.Name;
        //
        // string code = Templates.IsKnownPrimitive(primitiveType) ? _converterForKnownTypes : _converterForAnyOtherType;
        //
        //
        // if (isWrapperAValueType)
        // {
        //     code += _comparerForValuesTypes;
        // }
        // else
        // {
        //     code += _comparerForReferenceTypes;
        // }
        //
        // code = CodeSections.CutSection(code, "__WHEN_INNER__");
        // code = CodeSections.KeepSection(code, "__WHEN_OUTER__");
        //
        // code = code.Replace("__CLASS_PREFIX__", voSymbol.Name);
        // code = code.Replace("VOTYPE", voSymbol.FullName() ?? voSymbol.Name);
        // code = code.Replace("VOUNDERLYINGTYPE", primitiveFullName);
        //
        // return code;
    }

    private static string GenerateAnyBody3(bool writeInnerClass, INamedTypeSymbol primitiveType, bool isWrapperAValueType, INamedTypeSymbol voSymbol)
    {
        string sectionToCut = writeInnerClass ? "__WHEN_OUTER__" : "__WHEN_INNER__";
        string sectionToKeep = writeInnerClass ? "__WHEN_INNER__" : "__WHEN_OUTER__";
        string classPrefix = writeInnerClass ? string.Empty : voSymbol.Name;
        string voTypeName = writeInnerClass ? voSymbol.Name : voSymbol.FullName() ?? voSymbol.Name;
        
        var isKnownPrimitive = Templates.IsKnownPrimitive(primitiveType);

        var primitiveFullName =  primitiveType.FullName() ?? primitiveType.Name;
        if (isKnownPrimitive)
        {
          //  primitiveFullName = "global::" + primitiveFullName;
        }


        string code = isKnownPrimitive ? _converterForKnownTypes : _converterForAnyOtherType;
        

        if (isWrapperAValueType)
        {
            code += _comparerForValuesTypes;
        }
        else
        {
            code += _comparerForReferenceTypes;
        }

        code = CodeSections.CutSection(code, sectionToCut);
        code = CodeSections.KeepSection(code, sectionToKeep);

        code = code.Replace("__CLASS_PREFIX__", classPrefix);
        code = code.Replace("VOTYPE", voTypeName);
        code = code.Replace("VOUNDERLYINGTYPE", primitiveFullName);

        return code;
    }

    private static bool IsOurs(Vogen.Conversions conversions) => conversions.HasFlag(Vogen.Conversions.EfCoreValueConverter);
}