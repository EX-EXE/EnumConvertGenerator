using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace EnumConvertGenerator;

[Generator(LanguageNames.CSharp)]
public partial class EnumConvertGenerator : IIncrementalGenerator
{
    private static readonly string GeneratorNamespace = "EnumConvertGenerator";
    private static readonly string EnumHookAttribute = $"{GeneratorNamespace}.EnumConvertGeneratorAttribute";
    private static readonly string EnumIgnoreAttribute = $"{GeneratorNamespace}.EnumIgnoreAttribute";
    private static readonly string EnumNameAttribute = $"{GeneratorNamespace}.EnumNameAttribute";
    private static readonly string EnumAliasAttribute = $"{GeneratorNamespace}.EnumAliasAttribute";
    private static readonly string EnumToAttribute = $"{GeneratorNamespace}.EnumToAttribute";
    private static readonly string EnumFromAttribute = $"{GeneratorNamespace}.EnumFromAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // SourceGenerator用のAttribute生成
        GenerateAttribute(context);

        // Hook Attribute
        var source = context.SyntaxProvider.ForAttributeWithMetadataName(
            EnumHookAttribute,
            static (node, token) => true,
            static (context, token) => context);

        // 出力コード部分はちょっとごちゃつくので別メソッドに隔離
        context.RegisterSourceOutput(source, GenerateSource);
    }

    public static void GenerateAttribute(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static context =>
        {
            // Enumフック
            context.AddSource($"{EnumHookAttribute}.cs", """
namespace EnumConvertGenerator;
using System;
[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
internal sealed class EnumConvertGeneratorAttribute : Attribute
{
}
""");
            // 無視
            context.AddSource($"{EnumIgnoreAttribute}.cs", """
namespace EnumConvertGenerator;
using System;
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
internal sealed class EnumIgnoreAttribute : Attribute
{
}
""");
            // Name 置き換え
            context.AddSource($"{EnumNameAttribute}.cs", """
namespace EnumConvertGenerator;
using System;
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
internal sealed class EnumNameAttribute : Attribute
{
    public string Name { get; private set; } = string.Empty;
    public EnumNameAttribute(string name)
    {
        Name = name;
    }
}
""");
            // 別名
            context.AddSource($"{EnumAliasAttribute}.cs", """
namespace EnumConvertGenerator;
using System;
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
internal sealed class EnumAliasAttribute : Attribute
{
    public string[] Aliases { get; private set; } = Array.Empty<string>();
    public EnumAliasAttribute(params string[] aliases)
    {
        Aliases = aliases;
    }
}
""");
            // 変換先
            context.AddSource($"{EnumToAttribute}.cs", """
namespace EnumConvertGenerator;
using System;
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
internal sealed class EnumToAttribute<T>: Attribute
{
    public T Value { get; private set; } = default;
    public EnumToAttribute(T value)
    {
        Value = value;
    }
}
""");
            // 変換元
            var sourceEnumString = new StringBuilder($$"""
namespace EnumConvertGenerator;
using System;

""");
            foreach (var num in Enumerable.Range(1, 10))
            {
                var types = $"<{string.Join(",", Enumerable.Range(0, num).Select(x => $"T{x}"))}>";
                var typeValues = string.Join(",", Enumerable.Range(0, num).Select(x => $"T{x} value{x}"));
                var properties = string.Join("\n\t", Enumerable.Range(0, num).Select(x => $"public T{x} Value{x} {{ get; private set; }} = default;"));
                var init = string.Join("\n\t\t", Enumerable.Range(0, num).Select(x => $"Value{x} = value{x};"));
                sourceEnumString.AppendLine($$"""
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
internal sealed class EnumFromAttribute{{types}}: Attribute
{
    {{properties}}
    public EnumFromAttribute({{typeValues}})
    {
        {{init}}
    }
}
""");
            }
            context.AddSource($"{EnumFromAttribute}.cs", sourceEnumString.ToString());
        });
    }

    public static void GenerateSource(SourceProductionContext context, GeneratorAttributeSyntaxContext source)
    {
        var semanticModel = source.SemanticModel;
        var typeSymbol = (INamedTypeSymbol)source.TargetSymbol;
        var enumNode = (EnumDeclarationSyntax)source.TargetNode;
        var enumNamespace = typeSymbol.ToFullType();

        var enumMemberList = new List<EnumMemberInfo>();
        // Enum値ごとに処理
        foreach (var enumMember in enumNode.Members)
        {
            var a = semanticModel.GetSymbolInfo(enumMember);
            var attributes = semanticModel.GetAttributeSymbolInfos(enumMember);
            // Ignoreがついている場合は、スキップ
            if (attributes.Where(x => x.symbol.ContainingType.ToFullType() == EnumIgnoreAttribute).Any())
            {
                continue;
            }

            // Attribute情報を取得
            var membernInfo = new EnumMemberInfo();
            var memberName = enumMember.Identifier.ValueText;
            membernInfo.NameSpace = $"{enumNamespace}.{memberName}";
            membernInfo.Name = $"\"{memberName}\"";
            foreach (var (attributeSyntex, attributeSymbolInfo) in attributes)
            {
                // GeneratorAttributeを取得
                var type = attributeSymbolInfo.ContainingType.ToFullTypeNoGeneric();
                // 引数情報を取得
                var parameters = AnalyzeUtility.GetParameters(attributeSyntex, attributeSymbolInfo).ToArray();
                if (type == EnumNameAttribute)
                {
                    membernInfo.Name = parameters.First().Value;
                }
                else if (type == EnumAliasAttribute)
                {
                    membernInfo.Aliases = parameters.Select(x => x.Value).ToList();
                }
                else if (type == EnumToAttribute)
                {
                    var paramFirst = parameters.FirstOrDefault();
                    if (paramFirst.Type.Equals("string"))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.NotAllowedStringType, attributeSyntex.GetLocation(), typeSymbol.Name));
                    }
                    if (membernInfo.ToList.Where(x => x.EqualsType(paramFirst)).Any())
                    {
                        context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.NotAllowedDuplicateType, attributeSyntex.GetLocation(), typeSymbol.Name));
                    }
                    else
                    {
                        membernInfo.ToList.Add(paramFirst);
                    }
                }
                else if (type == EnumFromAttribute)
                {
                    var paramArrayInfo = new AnalyzeParameterArrayInfo(parameters);
                    if (paramArrayInfo.IsDuplicateTypes())
                    {
                        context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.NotAllowedDuplicateType, attributeSyntex.GetLocation(), typeSymbol.Name));
                    }
                    else if (membernInfo.FromList.Where(x => x.EqualsType(paramArrayInfo)).Any())
                    {
                        context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.NotAllowedSameTypes, attributeSyntex.GetLocation(), typeSymbol.Name));
                    }
                    else if (paramArrayInfo.Parameters.Length == 1 && paramArrayInfo.Parameters[0].Type.Equals("string"))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.NotAllowedStringOnly, attributeSyntex.GetLocation(), typeSymbol.Name));
                    }
                    else
                    {
                        membernInfo.FromList.Add(paramArrayInfo);
                    }
                }
            }
            enumMemberList.Add(membernInfo);
        }
        // ConstantValue
        foreach (var enumMemberSymbol in typeSymbol.GetMembers().OfType<IFieldSymbol>())
        {
            var symbolName = enumMemberSymbol.Name;
            var symbolNamespace = enumMemberSymbol.Type.ToFullType();
            var searchStr = $"{symbolNamespace}.{symbolName}";
            var find = enumMemberList.Where(x => x.NameSpace.Equals(searchStr)).FirstOrDefault();
            if (find != default)
            {
                find.Value = enumMemberSymbol.ConstantValue as int?;
            }
        }

        // ソース生成
        var sourceEmumFrom = new StringBuilder();
        foreach (var uniqueTypeFromParameterArrayInfo in EnumMemberInfo.UniqueFromTypeList(enumMemberList))
        {
            // 各パラメーターの拡張関数を生成する
            for (var parameterIndex = 0; parameterIndex < uniqueTypeFromParameterArrayInfo.Parameters.Length; ++parameterIndex)
            {
                // 引数を生成(this {parameterIndexのパラメーター} , それ以外のパラメーター)
                var funcParams = $"{uniqueTypeFromParameterArrayInfo.Parameters[parameterIndex].Type} {uniqueTypeFromParameterArrayInfo.Parameters[parameterIndex].Type.ToName(false)}";
                foreach (var (currentIndex, paramInfo) in uniqueTypeFromParameterArrayInfo.Parameters.Select((x, i) => (i, x)))
                {
                    if (parameterIndex != currentIndex)
                    {
                        funcParams += $", {paramInfo.Type} {paramInfo.Type.ToName(false)}";
                    }
                }

                // 処理はindex0に集約して、それ以外はindex0の関数を呼び出す
                if (parameterIndex == 0)
                {
                    // 全てのEnumメンバーから同じ型リストを抽出し、条件式を生成する
                    var sourceCondition = new StringBuilder();
                    foreach (var enumMember in enumMemberList)
                    {
                        foreach (var enumFromList in enumMember.FromList.Where(x => x.EqualsType(uniqueTypeFromParameterArrayInfo)))
                        {
                            sourceCondition.AppendLine($$"""
        if({{string.Join(" && ", enumFromList.Parameters.Select(x => $"{x.Type.ToName(false)} == {x.Value}"))}})
        {
            return {{enumMember.NameSpace}};
        }
""");
                        }
                    }
                    // 関数生成
                    sourceEmumFrom.AppendLine($$"""
    public static {{typeSymbol.Name}} To{{typeSymbol.Name}}(this {{funcParams}})
    {
{{sourceCondition}}
        throw new ArgumentException();
    }

""");
                }
                else
                {
                    // index0の関数を呼び出す
                    sourceEmumFrom.AppendLine($$"""
    public static {{typeSymbol.Name}} To{{typeSymbol.Name}}(this {{funcParams}})
        => To{{typeSymbol.Name}}({{string.Join(",", uniqueTypeFromParameterArrayInfo.Parameters.Select(x => $"{x.Type.ToName(false)}"))}});

""");
                }
            }
        }


        var sourceEnumTo = new StringBuilder();
        foreach (var uniqueTypeToParameterInfo in EnumMemberInfo.UniqueToTypeList(enumMemberList))
        {
            // ネームスペースを含まない型名を取得する
            var typeName = uniqueTypeToParameterInfo.Type.ToName(true);

            var sourceCase = new StringBuilder();
            // 全てのEnumメンバーから同じ型リストを抽出し、条件式を生成する
            foreach (var enumMember in enumMemberList)
            {
                foreach (var enumToList in enumMember.ToList.Where(x => x.EqualsType(uniqueTypeToParameterInfo)))
                {
                    sourceCase.AppendLine($"\t\t\t{enumMember.NameSpace} => {enumToList.Value},");
                }
            }
            // 生成
            sourceEmumFrom.AppendLine($$"""
    public static {{uniqueTypeToParameterInfo.Type}} To{{typeName}}(this {{typeSymbol.Name}} type)
    {
        return type switch
        {
{{sourceCase}}
            _ => throw new ArgumentException(type.ToString()),
        };
    }

""");
        }

        var sourceNamespace = typeSymbol.ContainingNamespace.IsGlobalNamespace
            ? ""
            : $"namespace {typeSymbol.ContainingNamespace};";
        var sourceCode = $$"""
// <auto-generated/>
#nullable enable
#pragma warning disable CS8600
#pragma warning disable CS8601
#pragma warning disable CS8602
#pragma warning disable CS8603
#pragma warning disable CS8604

using System;

{{sourceNamespace}}

public static partial class {{typeSymbol.Name}}Extensions
{
    public static {{typeSymbol.Name}} To{{typeSymbol.Name}}(this int type)
    {
        return type switch
        {
{{string.Join("\n", enumMemberList.Select(x => $"\t\t\t{x.Value} => {x.NameSpace},"))}}
            _ => throw new ArgumentException(type.ToString()),
        };
    }

    public static {{typeSymbol.Name}} To{{typeSymbol.Name}}(this string name)
    {
        return name switch
        {
            // Name
{{string.Join("\n", enumMemberList.Select(x => $"\t\t\t{x.Name} => {x.NameSpace},"))}}
            // Alias
{{string.Join("\n", enumMemberList.SelectMany(x => x.Aliases.DefaultIfEmpty(), (item, value) => (item.NameSpace, value)).Where(x => !string.IsNullOrEmpty(x.value)).Select(x => $"\t\t\t{x.value} => {x.NameSpace},"))}}
            _ => throw new ArgumentException(name.ToString()),
        };
    }

    public static string ToName(this {{typeSymbol.Name}} type)
    {
        return type switch
        {
{{string.Join("\n", enumMemberList.Select(x => $"\t\t\t{x.NameSpace} => {x.Name},"))}}
            _ => throw new ArgumentException(type.ToString()),
        };
    }

    public static int ToValue(this {{typeSymbol.Name}} type)
    {
        return type switch
        {
{{string.Join("\n", enumMemberList.Select(x => $"\t\t\t{x.NameSpace} => {x.Value},"))}}
            _ => throw new ArgumentException(type.ToString()),
        };
    }

{{sourceEmumFrom}}

{{sourceEnumTo}}
}
""";
        // AddSourceで出力
        context.AddSource($"{enumNamespace}.{GeneratorNamespace}.g.cs", sourceCode);
    }

    public static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor NotAllowedStringType = new(
            id: "000",
            title: "String type is not allowed.",
            messageFormat: "String type is not allowed.",
            category: GeneratorNamespace,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor NotAllowedDuplicateType = new(
            id: "001",
            title: "Duplicate type is not allowed.",
            messageFormat: "Duplicate type is not allowed.",
            category: GeneratorNamespace,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor NotAllowedSameTypes = new(
            id: "002",
            title: "Same types is not allowed.",
            messageFormat: "Same types is not allowed.",
            category: GeneratorNamespace,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor NotAllowedStringOnly = new(
            id: "003",
            title: "String only is not allowed.",
            messageFormat: "String only is not allowed.",
            category: GeneratorNamespace,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);
    }

    public class EnumMemberInfo
    {
        public string Name { get; set; } = string.Empty;
        public string NameSpace { get; set; } = string.Empty;
        public int? Value { get; set; } = null;
        public List<string> Aliases { get; set; } = new List<string>();

        public List<AnalyzeParameterInfo> ToList { get; set; } = new List<AnalyzeParameterInfo>();
        public List<AnalyzeParameterArrayInfo> FromList { get; set; } = new List<AnalyzeParameterArrayInfo>();

        public static AnalyzeParameterInfo[] UniqueToTypeList(IEnumerable<EnumMemberInfo> infos)
        {
            var result = new List<AnalyzeParameterInfo>();
            foreach (var info in infos)
            {
                foreach (var infoToParameter in info.ToList)
                {
                    if (!infoToParameter.ContainsType(result))
                    {
                        result.Add(infoToParameter);
                    }
                }
            }
            return result.ToArray();
        }

        public static AnalyzeParameterArrayInfo[] UniqueFromTypeList(IEnumerable<EnumMemberInfo> infos)
        {
            var result = new List<AnalyzeParameterArrayInfo>();
            foreach (var info in infos)
            {
                foreach (var infoFromParameter in info.FromList)
                {
                    if (!infoFromParameter.ContainsType(result))
                    {
                        result.Add(infoFromParameter);
                    }
                }
            }
            return result.ToArray();
        }
    }
}
