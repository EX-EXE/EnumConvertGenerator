using EnumConvertSample;

namespace EnumConvertGenerator
{
    public class EnumConvertGenerator
    {
        [Fact]
        public void ToSampleEnum1()
        {
            Assert.True(100.ToSampleEnum() == SampleEnum.One_A);
            Assert.True(101.ToSampleEnum() == SampleEnum.Two_B);
            Assert.True(102.ToSampleEnum() == SampleEnum.Three_C);
            Assert.True(103.ToSampleEnum() == SampleEnum.Four_D);
        }

        [Fact]
        public void ToSampleEnum2()
        {
            Assert.True("One And A".ToSampleEnum() == SampleEnum.One_A);
            Assert.True("Two&B".ToSampleEnum() == SampleEnum.Two_B);
            Assert.True("Three C".ToSampleEnum() == SampleEnum.Three_C);
            Assert.True("Four_D".ToSampleEnum() == SampleEnum.Four_D);

            Assert.True("2".ToSampleEnum() == SampleEnum.Two_B);
            Assert.True("“ñ".ToSampleEnum() == SampleEnum.Two_B);
            Assert.True("‡U".ToSampleEnum() == SampleEnum.Two_B);
            Assert.True("“ó".ToSampleEnum() == SampleEnum.Two_B);
        }

        [Fact]
        public void ToSampleEnum3()
        {
            Assert.True(NemberEnumType.One.ToSampleEnum(AlphabetEnumType.A) == SampleEnum.One_A);
            Assert.True(NemberEnumType.Two.ToSampleEnum(AlphabetEnumType.B) == SampleEnum.Two_B);
            Assert.True(NemberEnumType.Three.ToSampleEnum(AlphabetEnumType.C) == SampleEnum.Three_C);
        }

        [Fact]
        public void ToSampleEnum4()
        {
            Assert.True(AlphabetEnumType.A.ToSampleEnum(NemberEnumType.One) == SampleEnum.One_A);
            Assert.True(AlphabetEnumType.B.ToSampleEnum(NemberEnumType.Two) == SampleEnum.Two_B);
            Assert.True(AlphabetEnumType.C.ToSampleEnum(NemberEnumType.Three) == SampleEnum.Three_C);
        }

        [Fact]
        public void ToSampleEnum5()
        {
            Assert.True(GroupEnumType.GroupA.ToSampleEnum(AlphabetEnumType.A) == SampleEnum.One_A);
            Assert.True(GroupEnumType.GroupA.ToSampleEnum(AlphabetEnumType.B) == SampleEnum.Two_B);
            Assert.True(GroupEnumType.GroupB.ToSampleEnum(AlphabetEnumType.C) == SampleEnum.Three_C);
            Assert.True(GroupEnumType.GroupB.ToSampleEnum(AlphabetEnumType.D) == SampleEnum.Four_D);
        }

        [Fact]
        public void ToSampleEnum6()
        {
            Assert.True(AlphabetEnumType.A.ToSampleEnum(GroupEnumType.GroupA) == SampleEnum.One_A);
            Assert.True(AlphabetEnumType.B.ToSampleEnum(GroupEnumType.GroupA) == SampleEnum.Two_B);
            Assert.True(AlphabetEnumType.C.ToSampleEnum(GroupEnumType.GroupB) == SampleEnum.Three_C);
            Assert.True(AlphabetEnumType.D.ToSampleEnum(GroupEnumType.GroupB) == SampleEnum.Four_D);
        }

        [Fact]
        public void ToName1()
        {
            Assert.True(SampleEnum.One_A.ToName() == "One And A");
            Assert.True(SampleEnum.Two_B.ToName() == "Two&B");
            Assert.True(SampleEnum.Three_C.ToName() == "Three C");
            Assert.True(SampleEnum.Four_D.ToName() == "Four_D");
        }

        [Fact]
        public void ToValue1()
        {
            Assert.True(SampleEnum.One_A.ToValue() == 100);
            Assert.True(SampleEnum.Two_B.ToValue() == 101);
            Assert.True(SampleEnum.Three_C.ToValue() == 102);
            Assert.True(SampleEnum.Four_D.ToValue() == 103);
        }

        [Fact]
        public void ToNemberEnumType1()
        {
            Assert.True(SampleEnum.One_A.ToNemberEnumType() == NemberEnumType.One);
            Assert.True(SampleEnum.Two_B.ToNemberEnumType() == NemberEnumType.Two);
            Assert.True(SampleEnum.Three_C.ToNemberEnumType() == NemberEnumType.Three);
        }

        [Fact]
        public void ToAlphabetEnumType1()
        {
            Assert.True(SampleEnum.One_A.ToAlphabetEnumType() == AlphabetEnumType.A);
            Assert.True(SampleEnum.Two_B.ToAlphabetEnumType() == AlphabetEnumType.B);
            Assert.True(SampleEnum.Three_C.ToAlphabetEnumType() == AlphabetEnumType.C);
        }
    }
}