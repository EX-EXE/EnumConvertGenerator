using System;
using EnumConvertGenerator;

namespace EnumConvertSample
{
    public class Test
    {
        public Test()
        {

        }
    }

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

}
