using System;
using EnumConvertGenerator;

namespace EnumConvertSample
{
    public class Test
    {
        public Test()
        {
            AlphabetEnumType.A.ToSampleEnum(NemberEnumType.One);
        }
    }

    [EnumConvertGenerator]
    public enum AlphabetEnumType
    {
        A, B, C, D,
    }

    [EnumConvertGenerator]
    public enum NemberEnumType
    {
        One, Two, Three, Four,
    }

    [EnumConvertGenerator]
    public enum SampleEnum
    {
        [EnumName("One And A")]
        [EnumTo<NemberEnumType>(NemberEnumType.One)]
        [EnumTo<AlphabetEnumType>(AlphabetEnumType.A)]
        [EnumFrom<NemberEnumType, AlphabetEnumType>(NemberEnumType.One, AlphabetEnumType.A)]
        One_A = 100,

        [EnumName("Two&B")]
        [EnumAlias("2", "二", "Ⅱ", "弐")]
        [EnumTo<NemberEnumType>(NemberEnumType.Two)]
        [EnumTo<AlphabetEnumType>(AlphabetEnumType.B)]
        [EnumFrom<NemberEnumType, AlphabetEnumType>(NemberEnumType.Two, AlphabetEnumType.B)]
        Two_B,

        [EnumName("Three C")]
        [EnumTo<NemberEnumType>(NemberEnumType.Three)]
        [EnumTo<AlphabetEnumType>(AlphabetEnumType.C)]
        [EnumFrom<NemberEnumType, AlphabetEnumType>(NemberEnumType.Three, AlphabetEnumType.C)]
        Three_C,

        Four_D,

        [EnumIgnore]
        Ignore,
    }

}
