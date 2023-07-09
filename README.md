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

## Sample

### Code
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

### Generate
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

    public static string[] ToAliases(this SampleEnum type)
    {
        return type switch
        {
			EnumConvertSample.SampleEnum.One_A => new string[]{  },
			EnumConvertSample.SampleEnum.Two_B => new string[]{ "2", "二", "Ⅱ", "弐" },
			EnumConvertSample.SampleEnum.Three_C => new string[]{  },
			EnumConvertSample.SampleEnum.Four_D => new string[]{  },
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
