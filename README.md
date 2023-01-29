# EnumConvertGenerator
Provides extension methods around enum conversion using source generation.

## How To Use
ToDo

## Sample

### Code
```
public enum AlphabetEnumType
{
    A, B, C, D,
}
public enum NemberEnumType
{
    One, Two, Three, Four,
}
public enum GroupEnumType
{
    GroupA, GroupB
}

[EnumConvertGenerator]
public enum SampleEnum
{
    [EnumName("One And A")]
    [EnumTo<NemberEnumType>(NemberEnumType.One)]
    [EnumTo<AlphabetEnumType>(AlphabetEnumType.A)]
    [EnumFrom<NemberEnumType, AlphabetEnumType>(NemberEnumType.One, AlphabetEnumType.A)]
    [EnumFrom<GroupEnumType, AlphabetEnumType>(GroupEnumType.GroupA, AlphabetEnumType.A)]
    One_A = 100,

    [EnumName("Two&B")]
    [EnumAlias("2", "二", "Ⅱ", "弐")]
    [EnumTo<NemberEnumType>(NemberEnumType.Two)]
    [EnumTo<AlphabetEnumType>(AlphabetEnumType.B)]
    [EnumFrom<NemberEnumType, AlphabetEnumType>(NemberEnumType.Two, AlphabetEnumType.B)]
    [EnumFrom<GroupEnumType, AlphabetEnumType>(GroupEnumType.GroupA, AlphabetEnumType.B)]
    Two_B,

    [EnumName("Three C")]
    [EnumTo<NemberEnumType>(NemberEnumType.Three)]
    [EnumTo<AlphabetEnumType>(AlphabetEnumType.C)]
    [EnumFrom<NemberEnumType, AlphabetEnumType>(NemberEnumType.Three, AlphabetEnumType.C)]
    [EnumFrom<GroupEnumType, AlphabetEnumType>(GroupEnumType.GroupB, AlphabetEnumType.C)]
    Three_C,

    [EnumFrom<GroupEnumType, AlphabetEnumType>(GroupEnumType.GroupB, AlphabetEnumType.D)]
    Four_D,

    [EnumIgnore]
    Ignore,
}
```

### Generate
```
public static partial class SampleEnumExtensions
{
    public static SampleEnum ToSampleEnum(this int type)
    {
        return type switch
        {
			100 => EnumConvertSample.SampleEnum.One_A,
			101 => EnumConvertSample.SampleEnum.Two_B,
			102 => EnumConvertSample.SampleEnum.Three_C,
			103 => EnumConvertSample.SampleEnum.Four_D,
            _ => throw new ArgumentException(type.ToString()),
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
            _ => throw new ArgumentException(name.ToString()),
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
            _ => throw new ArgumentException(type.ToString()),
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
            _ => throw new ArgumentException(type.ToString()),
        };
    }

    public static SampleEnum ToSampleEnum(this EnumConvertSample.NemberEnumType nemberEnumType, EnumConvertSample.AlphabetEnumType alphabetEnumType)
    {
        if(nemberEnumType == NemberEnumType.One && alphabetEnumType == AlphabetEnumType.A)
        {
            return EnumConvertSample.SampleEnum.One_A;
        }
        if(nemberEnumType == NemberEnumType.Two && alphabetEnumType == AlphabetEnumType.B)
        {
            return EnumConvertSample.SampleEnum.Two_B;
        }
        if(nemberEnumType == NemberEnumType.Three && alphabetEnumType == AlphabetEnumType.C)
        {
            return EnumConvertSample.SampleEnum.Three_C;
        }

        throw new ArgumentException();
    }

    public static SampleEnum ToSampleEnum(this EnumConvertSample.AlphabetEnumType alphabetEnumType, EnumConvertSample.NemberEnumType nemberEnumType)
        => ToSampleEnum(nemberEnumType,alphabetEnumType);

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

        throw new ArgumentException();
    }

    public static SampleEnum ToSampleEnum(this EnumConvertSample.AlphabetEnumType alphabetEnumType, EnumConvertSample.GroupEnumType groupEnumType)
        => ToSampleEnum(groupEnumType,alphabetEnumType);

    public static EnumConvertSample.NemberEnumType ToNemberEnumType(this SampleEnum type)
    {
        return type switch
        {
			EnumConvertSample.SampleEnum.One_A => NemberEnumType.One,
			EnumConvertSample.SampleEnum.Two_B => NemberEnumType.Two,
			EnumConvertSample.SampleEnum.Three_C => NemberEnumType.Three,

            _ => throw new ArgumentException(type.ToString()),
        };
    }

    public static EnumConvertSample.AlphabetEnumType ToAlphabetEnumType(this SampleEnum type)
    {
        return type switch
        {
			EnumConvertSample.SampleEnum.One_A => AlphabetEnumType.A,
			EnumConvertSample.SampleEnum.Two_B => AlphabetEnumType.B,
			EnumConvertSample.SampleEnum.Three_C => AlphabetEnumType.C,

            _ => throw new ArgumentException(type.ToString()),
        };
    }
}
```
