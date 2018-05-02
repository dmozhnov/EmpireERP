using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.Utils.Test
{
    [TestClass]
    public class EnumUtilsTest
    {
        #region Тестовые перечисления

        private enum TestEnumDefault
        {
            [EnumDisplayName("Один")]
            [EnumDescription("Описание enum")]
            one = 1,

            [EnumDisplayName("Два")]
            two,

            [EnumDisplayName("Три")]
            three
        }

        private enum TestEnumByte : byte
        {
            [EnumDisplayName("One")]
            one_byte = 1,

            [EnumDisplayName("Two")]
            two_byte,

            [EnumDisplayName("Three")]
            three_byte
        }

        private enum TestEnumLong : long
        {
            [EnumDisplayName("One")]
            one = 1,

            [EnumDisplayName("Two")]
            two,

            [EnumDisplayName("10G")]
            long_10G = 10111222333,

            [EnumDisplayName("9*10^18")]
            long_9GG = 9111222333111222333,
        }

        #endregion

        #region GetDisplayName

        [TestMethod]
        public void GetDisplayName_Must_Be_Correct()
        {
            Assert.AreEqual("Один", TestEnumDefault.one.GetDisplayName());
            Assert.AreEqual("Два", TestEnumDefault.two.GetDisplayName());
            Assert.AreEqual("Три", TestEnumDefault.three.GetDisplayName());
            Assert.AreEqual("One", TestEnumByte.one_byte.GetDisplayName());
            Assert.AreEqual("Two", TestEnumByte.two_byte.GetDisplayName());
            Assert.AreEqual("Three", TestEnumByte.three_byte.GetDisplayName());
            Assert.AreEqual("One", TestEnumLong.one.GetDisplayName());
            Assert.AreEqual("Two", TestEnumLong.two.GetDisplayName());
            Assert.AreEqual("10G", TestEnumLong.long_10G.GetDisplayName());
            Assert.AreEqual("9*10^18", TestEnumLong.long_9GG.GetDisplayName());
        }

        #endregion

        #region GetDescription

        [TestMethod]
        public void GetDescription_Must_Be_Correct()
        {
            Assert.AreEqual("Описание enum", TestEnumDefault.one.GetDescription());            
        }

        #endregion

        #region ValueToString

        [TestMethod]
        public void ValueToString_Must_Be_Correct_For_Default()
        {
            Assert.AreEqual("1", TestEnumDefault.one.ValueToString());
            Assert.AreEqual("2", TestEnumDefault.two.ValueToString());
            Assert.AreEqual("3", TestEnumDefault.three.ValueToString());
        }

        [TestMethod]
        public void ValueToString_Must_Be_Correct_For_Byte()
        {
            Assert.AreEqual("1", TestEnumByte.one_byte.ValueToString());
            Assert.AreEqual("2", TestEnumByte.two_byte.ValueToString());
            Assert.AreEqual("3", TestEnumByte.three_byte.ValueToString());
        }

        [TestMethod]
        public void ValueToString_Must_Be_Correct_For_Long()
        {
            Assert.AreEqual("1", TestEnumLong.one.ValueToString());
            Assert.AreEqual("2", TestEnumLong.two.ValueToString());
            Assert.AreEqual("10111222333", TestEnumLong.long_10G.ValueToString());
            Assert.AreEqual("9111222333111222333", TestEnumLong.long_9GG.ValueToString());
        }

        #endregion

        #region ContainsIn

        [TestMethod]
        public void ContainsIn_Must_Be_Ok()
        {
            var test = TestEnumDefault.one;

            Assert.IsTrue(test.ContainsIn(TestEnumDefault.one, TestEnumDefault.two));
            Assert.IsFalse(test.ContainsIn(TestEnumDefault.two));
            Assert.IsFalse(test.ContainsIn(TestEnumDefault.two, TestEnumDefault.three));
        }

        [TestMethod]
        public void ContainsIn_Must_Throw_Exception_If_Enum_List_Is_Empty()
        {
            var test = TestEnumDefault.one;

            try
            {
                test.ContainsIn();
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Список выбора значений перечисления пуст.", ex.Message);
            }
        }

        [TestMethod]
        public void ContainsIn_Must_Throw_Exception_If_Enum_Value_From_List_Does_Not_Belong_To_This_Enum()
        {
            var test = TestEnumDefault.one;

            try
            {
                test.ContainsIn(TestEnumLong.long_10G);
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Значение «long_10G» не принадлежит перечислению «TestEnumDefault».", ex.Message);
            }
        }
        #endregion
    }
}
