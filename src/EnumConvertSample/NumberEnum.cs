
using EnumConvertGenerator;

namespace EnumConvertSample;

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
