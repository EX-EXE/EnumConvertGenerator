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
