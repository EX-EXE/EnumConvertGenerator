[![NuGet version](https://badge.fury.io/nu/EnumConvertGenerator.svg)](https://badge.fury.io/nu/EnumConvertGenerator)
# EnumConvertGenerator
Provides extension methods around enum conversion using source generation.

## How To Use
### Install by nuget
PM> Install-Package [EnumConvertGenerator](https://www.nuget.org/packages/EnumConvertGenerator/)

## Attribute
| Attribute | Description | Generate Extension Methods |
|---|---|---|
|[EnumConvertGenerator]| Attribute can be added to Enum.<br/>  | TargetEnum ToTargetEnum(this int type) <br/> TargetEnum ToTargetEnum(this string name) <br/> string ToName(this TargetEnum type) <br/> int ToValue(this TargetEnum type) |
|[EnumTo]| Attribute can be added to Enum member. <br/> Convert to another class. | ConvertType ToConvertType(this TargetEnum type) |
|[EnumFrom]| Attribute can be added to Enum member.<br/> Convert from another class.  | TargetEnum ToTargetEnum(this ConvertType1 type1, ConvertType2 type2)  <br/>  TargetEnum ToTargetEnum(this ConvertType2 type2, ConvertType1 type1)  | 
|[EnumName]| Attribute can be added to Enum member. <br/> Replace the name.| | 
|[EnumAlias]| Attribute can be added to Enum member.<br/> Convert from a string. | |
|[EnumIgnore]| Attribute can be added to Enum member. <br/> Disable source generation.  | |

## Sample1
### Source Code
```csharp
using EnumConvertGenerator;
[EnumConvertGenerator]
public enum NumberEnumType
{
    [EnumName("EnumOne")]
    [EnumAlias("Alias1")]
    One = 1,
    [EnumName("EnumTwo")]
    [EnumAlias("Alias2")]
    Two,
    [EnumName("EnumThree")]
    [EnumAlias("Alias3")]
    Three,
    [EnumName("EnumFour")]
    [EnumAlias("Alias4")]
    Four,
}
```
### Generate Code
```csharp
public static partial class NumberEnumTypeExtensions
{
   public static readonly NumberEnumType[] GetNumberEnumTypeArray
        = new NumberEnumType[]{ EnumConvertSample.NumberEnumType.One, EnumConvertSample.NumberEnumType.Two, EnumConvertSample.NumberEnumType.Three, EnumConvertSample.NumberEnumType.Four };

    public static NumberEnumType ToNumberEnumType(this int type)
    {
        return type switch
        {
			1 => EnumConvertSample.NumberEnumType.One,
			2 => EnumConvertSample.NumberEnumType.Two,
			3 => EnumConvertSample.NumberEnumType.Three,
			4 => EnumConvertSample.NumberEnumType.Four,
            _ => throw new ArgumentException($"Invalid parameter. : {type}(NumberEnumType)"),
        };
    }

    public static NumberEnumType ToNumberEnumType(this string name)
    {
        return name switch
        {
            // Name
			"EnumOne" => EnumConvertSample.NumberEnumType.One,
			"EnumTwo" => EnumConvertSample.NumberEnumType.Two,
			"EnumThree" => EnumConvertSample.NumberEnumType.Three,
			"EnumFour" => EnumConvertSample.NumberEnumType.Four,
            // Alias
			"Alias1" => EnumConvertSample.NumberEnumType.One,
			"Alias2" => EnumConvertSample.NumberEnumType.Two,
			"Alias3" => EnumConvertSample.NumberEnumType.Three,
			"Alias4" => EnumConvertSample.NumberEnumType.Four,
            _ => throw new ArgumentException($"Invalid parameter. : {name}(NumberEnumType)"),
        };
    }

    public static string ToName(this NumberEnumType type)
    {
        return type switch
        {
			EnumConvertSample.NumberEnumType.One => "EnumOne",
			EnumConvertSample.NumberEnumType.Two => "EnumTwo",
			EnumConvertSample.NumberEnumType.Three => "EnumThree",
			EnumConvertSample.NumberEnumType.Four => "EnumFour",
            _ => throw new ArgumentException($"Invalid parameter. : {type}(NumberEnumType)"),
        };
    }

    public static string ToAlias(this NumberEnumType type)
    {
        return type switch
        {
			EnumConvertSample.NumberEnumType.One => "Alias1",
			EnumConvertSample.NumberEnumType.Two => "Alias2",
			EnumConvertSample.NumberEnumType.Three => "Alias3",
			EnumConvertSample.NumberEnumType.Four => "Alias4",
            _ => throw new ArgumentException($"Invalid parameter. : {type}(NumberEnumType)"),
        };
    }

	private static readonly string[] ___One_Aliases = { "Alias1" };
	private static readonly string[] ___Two_Aliases = { "Alias2" };
	private static readonly string[] ___Three_Aliases = { "Alias3" };
	private static readonly string[] ___Four_Aliases = { "Alias4" };
    public static string[] ToAliases(this NumberEnumType type)
    {
        return type switch
        {
			EnumConvertSample.NumberEnumType.One => ___One_Aliases,
			EnumConvertSample.NumberEnumType.Two => ___Two_Aliases,
			EnumConvertSample.NumberEnumType.Three => ___Three_Aliases,
			EnumConvertSample.NumberEnumType.Four => ___Four_Aliases,
            _ => throw new ArgumentException($"Invalid parameter. : {type}(NumberEnumType)"),
        };
    }

    public static int ToValue(this NumberEnumType type)
    {
        return type switch
        {
			EnumConvertSample.NumberEnumType.One => 1,
			EnumConvertSample.NumberEnumType.Two => 2,
			EnumConvertSample.NumberEnumType.Three => 3,
			EnumConvertSample.NumberEnumType.Four => 4,
            _ => throw new ArgumentException($"Invalid parameter. : {type}(NumberEnumType)"),
        };
    }
}
```

## Sample2
### Source Code
```csharp
using System;
using EnumConvertGenerator;

namespace EnumConvertSample
{
    [EnumConvertGenerator]
    public enum GroupEnumType
    {
        GroupA, GroupB
    }

    [EnumConvertGenerator]
    public enum AlphabetEnumType
    {
        A, B, C, D,
    }

    [EnumConvertGenerator]
    public enum NumberEnumType
    {
        One, Two, Three, Four,
    }

    [EnumConvertGenerator]
    public enum SampleEnum
    {
        [EnumName("One And A")]
        [EnumTo<NumberEnumType>(NumberEnumType.One)]
        [EnumTo<AlphabetEnumType>(AlphabetEnumType.A)]
        [EnumFrom<NumberEnumType, AlphabetEnumType>(NumberEnumType.One, AlphabetEnumType.A)]
        [EnumFrom<GroupEnumType, AlphabetEnumType>(GroupEnumType.GroupA, AlphabetEnumType.A)]
        One_A = 100,

        [EnumName("Two&B")]
        [EnumAlias("2", "二", "Ⅱ", "弐")]
        [EnumTo<NumberEnumType>(NumberEnumType.Two)]
        [EnumTo<AlphabetEnumType>(AlphabetEnumType.B)]
        [EnumFrom<NumberEnumType, AlphabetEnumType>(NumberEnumType.Two, AlphabetEnumType.B)]
        [EnumFrom<GroupEnumType, AlphabetEnumType>(GroupEnumType.GroupA, AlphabetEnumType.B)]
        Two_B,

        [EnumName("Three C")]
        [EnumTo<NumberEnumType>(NumberEnumType.Three)]
        [EnumTo<AlphabetEnumType>(AlphabetEnumType.C)]
        [EnumFrom<NumberEnumType, AlphabetEnumType>(NumberEnumType.Three, AlphabetEnumType.C)]
        [EnumFrom<GroupEnumType, AlphabetEnumType>(GroupEnumType.GroupB, AlphabetEnumType.C)]
        Three_C,

        [EnumFrom<GroupEnumType, AlphabetEnumType>(GroupEnumType.GroupB, AlphabetEnumType.D)]
        Four_D,

        [EnumIgnore]
        Ignore,
    }
}
```

### Generate Code
```csharp
public static partial class SampleEnumExtensions
{
   public static readonly SampleEnum[] GetSampleEnumArray
        = new SampleEnum[]{ EnumConvertSample.SampleEnum.One_A, EnumConvertSample.SampleEnum.Two_B, EnumConvertSample.SampleEnum.Three_C, EnumConvertSample.SampleEnum.Four_D };

    public static SampleEnum ToSampleEnum(this int type)
    {
        return type switch
        {
			100 => EnumConvertSample.SampleEnum.One_A,
			101 => EnumConvertSample.SampleEnum.Two_B,
			102 => EnumConvertSample.SampleEnum.Three_C,
			103 => EnumConvertSample.SampleEnum.Four_D,
            _ => throw new ArgumentException($"Invalid parameter. : {type}(SampleEnum)"),
        };
    }

    public static SampleEnum ToSampleEnum(this string name)
    {
        return name switch
        {
            // Name
			"One And A" => EnumConvertSample.SampleEnum.One_A,
			"Two&B" => EnumConvertSample.SampleEnum.Two_B,
			"Three C" => EnumConvertSample.SampleEnum.Three_C,
			"Four_D" => EnumConvertSample.SampleEnum.Four_D,
            // Alias
			"2" => EnumConvertSample.SampleEnum.Two_B,
			"二" => EnumConvertSample.SampleEnum.Two_B,
			"Ⅱ" => EnumConvertSample.SampleEnum.Two_B,
			"弐" => EnumConvertSample.SampleEnum.Two_B,
            _ => throw new ArgumentException($"Invalid parameter. : {name}(SampleEnum)"),
        };
    }

    public static string ToName(this SampleEnum type)
    {
        return type switch
        {
			EnumConvertSample.SampleEnum.One_A => "One And A",
			EnumConvertSample.SampleEnum.Two_B => "Two&B",
			EnumConvertSample.SampleEnum.Three_C => "Three C",
			EnumConvertSample.SampleEnum.Four_D => "Four_D",
            _ => throw new ArgumentException($"Invalid parameter. : {type}(SampleEnum)"),
        };
    }

    public static string ToAlias(this SampleEnum type)
    {
        return type switch
        {
			EnumConvertSample.SampleEnum.Two_B => "2",
            _ => throw new ArgumentException($"Invalid parameter. : {type}(SampleEnum)"),
        };
    }

	private static readonly string[] ___One_A_Aliases = {  };
	private static readonly string[] ___Two_B_Aliases = { "2", "二", "Ⅱ", "弐" };
	private static readonly string[] ___Three_C_Aliases = {  };
	private static readonly string[] ___Four_D_Aliases = {  };
    public static string[] ToAliases(this SampleEnum type)
    {
        return type switch
        {
			EnumConvertSample.SampleEnum.One_A => ___One_A_Aliases,
			EnumConvertSample.SampleEnum.Two_B => ___Two_B_Aliases,
			EnumConvertSample.SampleEnum.Three_C => ___Three_C_Aliases,
			EnumConvertSample.SampleEnum.Four_D => ___Four_D_Aliases,
            _ => throw new ArgumentException($"Invalid parameter. : {type}(SampleEnum)"),
        };
    }

    public static int ToValue(this SampleEnum type)
    {
        return type switch
        {
			EnumConvertSample.SampleEnum.One_A => 100,
			EnumConvertSample.SampleEnum.Two_B => 101,
			EnumConvertSample.SampleEnum.Three_C => 102,
			EnumConvertSample.SampleEnum.Four_D => 103,
            _ => throw new ArgumentException($"Invalid parameter. : {type}(SampleEnum)"),
        };
    }

    public static SampleEnum ToSampleEnum(this EnumConvertSample.NumberEnumType numberEnumType, EnumConvertSample.AlphabetEnumType alphabetEnumType)
    {
        if(numberEnumType == NumberEnumType.One && alphabetEnumType == AlphabetEnumType.A)
        {
            return EnumConvertSample.SampleEnum.One_A;
        }
        if(numberEnumType == NumberEnumType.Two && alphabetEnumType == AlphabetEnumType.B)
        {
            return EnumConvertSample.SampleEnum.Two_B;
        }
        if(numberEnumType == NumberEnumType.Three && alphabetEnumType == AlphabetEnumType.C)
        {
            return EnumConvertSample.SampleEnum.Three_C;
        }

        throw new ArgumentException($"Invalid parameter.");
    }

    public static SampleEnum ToSampleEnum(this EnumConvertSample.AlphabetEnumType alphabetEnumType, EnumConvertSample.NumberEnumType numberEnumType)
        => ToSampleEnum(numberEnumType,alphabetEnumType);

    public static SampleEnum ToSampleEnum(this EnumConvertSample.GroupEnumType groupEnumType, EnumConvertSample.AlphabetEnumType alphabetEnumType)
    {
        if(groupEnumType == GroupEnumType.GroupA && alphabetEnumType == AlphabetEnumType.A)
        {
            return EnumConvertSample.SampleEnum.One_A;
        }
        if(groupEnumType == GroupEnumType.GroupA && alphabetEnumType == AlphabetEnumType.B)
        {
            return EnumConvertSample.SampleEnum.Two_B;
        }
        if(groupEnumType == GroupEnumType.GroupB && alphabetEnumType == AlphabetEnumType.C)
        {
            return EnumConvertSample.SampleEnum.Three_C;
        }
        if(groupEnumType == GroupEnumType.GroupB && alphabetEnumType == AlphabetEnumType.D)
        {
            return EnumConvertSample.SampleEnum.Four_D;
        }

        throw new ArgumentException($"Invalid parameter.");
    }

    public static SampleEnum ToSampleEnum(this EnumConvertSample.AlphabetEnumType alphabetEnumType, EnumConvertSample.GroupEnumType groupEnumType)
        => ToSampleEnum(groupEnumType,alphabetEnumType);

    public static EnumConvertSample.NumberEnumType ToNumberEnumType(this SampleEnum type)
    {
        return type switch
        {
			EnumConvertSample.SampleEnum.One_A => NumberEnumType.One,
			EnumConvertSample.SampleEnum.Two_B => NumberEnumType.Two,
			EnumConvertSample.SampleEnum.Three_C => NumberEnumType.Three,

            _ => throw new ArgumentException($"Invalid parameter. : {type}(NumberEnumType)"),
        };
    }

    public static EnumConvertSample.AlphabetEnumType ToAlphabetEnumType(this SampleEnum type)
    {
        return type switch
        {
			EnumConvertSample.SampleEnum.One_A => AlphabetEnumType.A,
			EnumConvertSample.SampleEnum.Two_B => AlphabetEnumType.B,
			EnumConvertSample.SampleEnum.Three_C => AlphabetEnumType.C,

            _ => throw new ArgumentException($"Invalid parameter. : {type}(AlphabetEnumType)"),
        };
    }
}
```
