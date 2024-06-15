using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using _000.Common;
using _000.Common._03._Attribute;

namespace _999.UnitTest._000._Common
{
    partial class _000_Common
    {
        [Flags]
        public enum UNIT_TEST_ENUM
        {
            [Enum("A1", "First")]
            A = 1,
            [Enum("B2", "Second")]
            B = A << 1,
            [EnumAttribute("C3", "Third")]
            C = B << 1,
        }

        [TestCategory("[CATEGORY] [000_Common] 03_Attribute")]
        [TestMethod("[EnumAttribute] GetEnumAttribute() Check")]
        [DataRow(UNIT_TEST_ENUM.A | UNIT_TEST_ENUM.B | UNIT_TEST_ENUM.C)]
        [DataRow(UNIT_TEST_ENUM.C | UNIT_TEST_ENUM.B | UNIT_TEST_ENUM.A)]
        public void UnitTest_GetEnumAttribute(Enum value)
        {
            EnumAttribute[] enumAttr = Common.GetEnumAttribute(value);
            Assert.IsTrue(enumAttr.Length == 3);
        }

        [TestCategory("[CATEGORY] [000_Common] 03_Attribute")]
        [TestMethod("[EnumAttribute] GetEnumName() Check")]
        [DataRow(UNIT_TEST_ENUM.A | UNIT_TEST_ENUM.B | UNIT_TEST_ENUM.C)]
        [DataRow(UNIT_TEST_ENUM.C | UNIT_TEST_ENUM.B | UNIT_TEST_ENUM.A)]
        public void GetFlagsEnumName(UNIT_TEST_ENUM value)
        {
            string enumName = Common.GetEnumName(value);
            Assert.AreEqual("A1 | B2 | C3", enumName);
        }

        [TestCategory("[CATEGORY] [000_Common] 03_Attribute")]
        [TestMethod("[EnumAttribute] GetEnumDescription() Check")]
        [DataRow(UNIT_TEST_ENUM.A | UNIT_TEST_ENUM.B | UNIT_TEST_ENUM.C)]
        [DataRow(UNIT_TEST_ENUM.C | UNIT_TEST_ENUM.B | UNIT_TEST_ENUM.A)]
        public void GetFlagsEnumDesc(UNIT_TEST_ENUM value)
        {
            string enumName = Common.GetEnumDescription(value);
            Assert.AreEqual("First | Second | Third", enumName);
        }

        [TestCategory("[CATEGORY] [000_Common] 03_Attribute")]
        [TestMethod("[EnumAttribute] GetEnumInfo() Check")]
        [DataRow(UNIT_TEST_ENUM.A | UNIT_TEST_ENUM.B | UNIT_TEST_ENUM.C)]
        [DataRow(UNIT_TEST_ENUM.C | UNIT_TEST_ENUM.B | UNIT_TEST_ENUM.A)]
        public void GetFlagsEnumInfo(UNIT_TEST_ENUM value)
        {
            string enumInfo = Common.GetEnumInfo(value);
            Assert.AreEqual("[A1] First | [B2] Second | [C3] Third", enumInfo);
            enumInfo = Common.GetEnumInfo(value, " ");
            Assert.AreEqual("[A1] First [B2] Second [C3] Third", enumInfo);
            enumInfo = Common.GetEnumInfo(value, "  ");
            Assert.AreEqual("[A1] First  [B2] Second  [C3] Third", enumInfo);

            enumInfo = Common.GetEnumInfo(value, "\n | ");
            Assert.AreEqual("[A1] First\n | [B2] Second\n | [C3] Third", enumInfo);
            enumInfo = Common.GetEnumInfo(value, "\n ");
            Assert.AreEqual("[A1] First\n [B2] Second\n [C3] Third", enumInfo);
            enumInfo = Common.GetEnumInfo(value, "\n  ");
            Assert.AreEqual("[A1] First\n  [B2] Second\n  [C3] Third", enumInfo);
        }
    }
}
