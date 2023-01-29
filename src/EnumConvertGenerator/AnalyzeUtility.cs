using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static EnumConvertGenerator.EnumConvertGenerator;

namespace EnumConvertGenerator;

public class AnalyzeParameterArrayInfo
{
    public AnalyzeParameterInfo[] Parameters { get; private set; } = Array.Empty<AnalyzeParameterInfo>();

    public AnalyzeParameterArrayInfo(IEnumerable<AnalyzeParameterInfo> parameters)
    {
        Parameters = parameters.ToArray();
    }


    public bool IsDuplicateTypes()
    {
        for (int i = 0; i < Parameters.Length; ++i)
        {
            for (int o = 0; o < Parameters.Length; ++o)
            {
                if (i == o)
                {
                    continue;
                }
                if (Parameters[i].Type.Equals(Parameters[o].Type))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool ContainsType(IEnumerable<AnalyzeParameterArrayInfo> list)
    {
        return list.Where(x => x.EqualsType(this)).Any();
    }

    public bool EqualsType(AnalyzeParameterArrayInfo info)
    {
        return EqualsType(this, info);
    }

    public static bool EqualsType(AnalyzeParameterArrayInfo x, AnalyzeParameterArrayInfo y)
    {
        if (x.Parameters.Length != y.Parameters.Length)
        {
            return false;
        }

        foreach (var xParam in x.Parameters)
        {
            var find = false;
            foreach (var yParam in y.Parameters)
            {
                if (xParam.Type.Equals(yParam.Type))
                {
                    find = true;
                    break;
                }
            }
            if (!find)
            {
                return false;
            }
        }
        return true;
    }
}

public class AnalyzeParameterInfo
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// リスト内に同じ型が存在するか調べる
    /// </summary>
    /// <param name="infos"></param>
    /// <returns></returns>
    public static bool IsDuplicateTypes(IEnumerable<AnalyzeParameterInfo> infos)
    {
        var infoArray = infos.ToArray();
        for (int i = 0; i < infoArray.Length; ++i)
        {
            for (int o = 0; o < infoArray.Length; ++o)
            {
                if (i == o)
                {
                    continue;
                }
                if (infoArray[i].Type.Equals(infoArray[o].Type))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool ContainsType(IEnumerable<AnalyzeParameterInfo> list)
    {
        return list.Where(x => x.EqualsType(this)).Any();
    }

    public bool EqualsType(AnalyzeParameterInfo info)
    {
        return EqualsType(this, info);
    }

    public bool EqualsType(AnalyzeParameterInfo x, AnalyzeParameterInfo y)
    {
        return x.Type.Equals(y.Type);
    }
}

public static class AnalyzeUtility
{
    public static string ToFullType<T>(this T symbol) where T : ISymbol
    {
        return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Replace("global::", "");
    }
    public static string ToName(this string type,bool topUpper)
    {
        var escapeTypeName = type.Replace("<", "_").Replace(">", "");
        var typeStartIndex = escapeTypeName.LastIndexOf(".");
        var typeName = type.Substring(typeStartIndex + 1);
        if (0 < typeName.Length)
        {
            if (topUpper)
            {
                // 先頭を大文字にする
                typeName = Char.ToUpper(typeName[0]) + typeName.Substring(1);
            }
            else
            {
                typeName = Char.ToLower(typeName[0]) + typeName.Substring(1);
            }
        }
        return typeName;
    }

    public static string ToFullTypeNoGeneric<T>(this T symbol) where T : ISymbol
    {
        var fullType = symbol.ToFullType();
        var index = fullType.IndexOf("<");
        return 0 <= index ? fullType.Substring(0, index) : fullType;
    }

    public static int? ToValue(this EqualsValueClauseSyntax? syntax)
    {
        if (syntax == null)
        {
            return null;
        }
        var literalSyntax = syntax.Value as LiteralExpressionSyntax;
        if (literalSyntax == null)
        {
            return null;
        }
        var token = literalSyntax.Token.ValueText;
        return int.Parse(token);
    }

    public static IEnumerable<(AttributeSyntax syntax, IMethodSymbol symbol)> GetAttributeSymbolInfos<T>(this SemanticModel model, T syntax) where T : MemberDeclarationSyntax
    {
        foreach (var attributeList in syntax.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                var attributeSymbol = model.GetSymbolInfo(attribute).Symbol as IMethodSymbol;
                if (attributeSymbol == null)
                {
                    continue;
                }
                yield return (attribute, attributeSymbol);
            }
        }
    }

    public static IEnumerable<AnalyzeParameterInfo> GetParameters(AttributeSyntax syntax, IMethodSymbol symbol)
    {
        if (syntax.ArgumentList != null)
        {
            foreach (var (index, parameter) in symbol.Parameters.Select((x, i) => (i, x)))
            {
                if (parameter.IsParams)
                {
                    // params (params string[])
                    var arrayTypeSymbol = parameter.Type as IArrayTypeSymbol;
                    if (arrayTypeSymbol == null)
                    {
                        continue;
                    }
                    var arrayTypeName = arrayTypeSymbol.ElementType.ToFullType();
                    foreach (var (argIndex, argValue) in syntax.ArgumentList.Arguments.Skip(index).Select((x, i) => (i, x)))
                    {
                        var value = argValue.ToFullString();
                        // string name_0 "value1"
                        // string name_1 "value2"
                        yield return new AnalyzeParameterInfo() { Type = arrayTypeName, Name = $"{parameter.Name}_{argIndex}", Value = value };
                    }
                }
                else
                {
                    // string[] name "new []{"value1","value2"}"
                    var typeName = parameter.Type.ToFullType();
                    var value = syntax.ArgumentList.Arguments[index].ToFullString();
                    yield return new AnalyzeParameterInfo() { Type = typeName, Name = parameter.Name, Value = value };
                }
            }
        }


        //foreach (var parameter in symbol.Parameters)
        //{
        //    var typeName = parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        //    var namedType = parameter.Type;
        //    var namedTypeName = namedType.ToDisplayString();
        //    yield return (typeName, parameter.Name, parameter.Name);
        //}
    }
}
