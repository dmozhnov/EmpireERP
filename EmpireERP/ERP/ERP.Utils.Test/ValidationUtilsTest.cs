using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.Utils.Test
{
    [TestClass]
    public class ValidationUtilsTest
    {
        #region IsNull

        [TestMethod]
        public void ValidationUtils_IsNull_WithNullValues_DoesntThrowException()
        {
            ValidationUtils.IsNull((bool?)null, "Значение не пусто.");
            ValidationUtils.IsNull((byte?)null, "Значение не пусто.");
            ValidationUtils.IsNull((int?)null, "Значение не пусто.");
            ValidationUtils.IsNull((short?)null, "Значение не пусто.");
            ValidationUtils.IsNull((decimal?)null, "Значение не пусто.");
            ValidationUtils.IsNull((double?)null, "Значение не пусто.");

            ValidationUtils.IsNull((string)null, "Значение не пусто.");

            ValidationUtils.IsNull((Guid?)null, "Непустой гуид.");
            ValidationUtils.IsNull((DateTime?)null, "Непустая дата.");

            ValidationUtils.IsNull((ValidationUtilsTest)null, "Значение не пусто.");
        }

        [TestMethod]
        public void ValidationUtils_IsNull_WithNotNullInt_ThrowsException()
        {
            try
            {
                ValidationUtils.IsNull((int?)5, "Значение не пусто.");

                Assert.Fail("Исключение не было выброшено.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Значение не пусто.", ex.Message);
            }
        }

        #endregion

        #region NotNull

        [TestMethod]
        public void ValidationUtils_NotNull_WithNotNullValues_DoesntThrowException()
        {
            ValidationUtils.NotNull((bool?)true, "Значение пусто.");
            ValidationUtils.NotNull((byte?)1, "Значение пусто.");
            ValidationUtils.NotNull((int?)123423133, "Значение пусто.");
            ValidationUtils.NotNull((short?)20000, "Значение пусто.");
            ValidationUtils.NotNull((decimal?)2489.2M, "Значение пусто.");
            ValidationUtils.NotNull((double?)3545.44, "Значение пусто.");

            ValidationUtils.NotNull((string)"", "Значение пусто.");
            ValidationUtils.NotNull((string)"Экскурсии", "Значение пусто.");

            ValidationUtils.NotNull((Guid?)Guid.Empty, "Пустой гуид.");
            ValidationUtils.NotNull((DateTime?)DateTime.Now, "Пустая дата.");

            ValidationUtils.NotNull((ValidationUtilsTest)this, "Значение пусто.");
        }

        [TestMethod]
        public void ValidationUtils_NotNull_WithNullInt_ThrowsException()
        {
            try
            {
                ValidationUtils.NotNull((int?)null, "Значение пусто.");

                Assert.Fail("Исключение не было выброшено.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Значение пусто.", ex.Message);
            }
        }

        #endregion

        #region Разное

        [TestMethod]
        public void ValidationUtils_NotNullOrDefault_WithZeroInt_ThrowsException()
        {
            try
            {
                ValidationUtils.NotNullOrDefault(0, "Равно нулю.");

                Assert.Fail("Исключение не было выброшено.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Равно нулю.", ex.Message);
            }
        }

        [TestMethod]
        public void ValidationUtils_NotNullOrDefault_WithZeroShort_ThrowsException()
        {
            try
            {
                ValidationUtils.NotNullOrDefault((short)0, "Равно нулю.");

                Assert.Fail("Исключение не было выброшено.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Равно нулю.", ex.Message);
            }
        }

        [TestMethod]
        public void ValidationUtils_NotNullOrDefault_WithZeroDecimal_ThrowsException()
        {
            try
            {
                ValidationUtils.NotNullOrDefault((decimal)0, "Равно нулю.");

                Assert.Fail("Исключение не было выброшено.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Равно нулю.", ex.Message);
            }
        }

        [TestMethod]
        public void ValidationUtils_NotNullOrDefault_WithZeroDouble_ThrowsException()
        {
            try
            {
                ValidationUtils.NotNullOrDefault((double)0, "Равно нулю.");

                Assert.Fail("Исключение не было выброшено.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Равно нулю.", ex.Message);
            }
        }

        [TestMethod]
        public void ValidationUtils_NotNullOrDefault_WithNullInt_ThrowsException()
        {
            try
            {
                ValidationUtils.NotNullOrDefault((int?)null, "Равно null.");

                Assert.Fail("Исключение не было выброшено.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Равно null.", ex.Message);
            }
        }


        [TestMethod]
        public void ValidationUtils_NotNullOrDefault_WithEmptyGuid_ThrowsException()
        {
            try
            {
                ValidationUtils.NotNullOrDefault(Guid.Empty, "Пустой гуид.");

                Assert.Fail("Исключение не было выброшено.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Пустой гуид.", ex.Message);
            }
        }

        [TestMethod]
        public void ValidationUtils_NotNullOrDefault_WithNonNullValues_DoesntThrowException()
        {
            ValidationUtils.NotNullOrDefault((int)5, "Равно нулю.");
            ValidationUtils.NotNullOrDefault((short)5, "Равно нулю.");
            ValidationUtils.NotNullOrDefault((decimal)5, "Равно нулю.");
            ValidationUtils.NotNullOrDefault((double)5, "Равно нулю.");

            ValidationUtils.NotNullOrDefault(Guid.NewGuid(), "Пустой гуид.");
        }

        [TestMethod]
        public void ValidationUtils_WithNull_MustThrowException()
        {
            try
            {
                ValidationUtils.TryGetGuid(null);
                Assert.Fail("Исключение не было выброшено.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Неверное значение уникального идентификатора.", ex.Message);
            }           
        }

        [TestMethod]
        public void ValidationUtils_WithEmptyString_MustThrowException()
        {
            try
            {
                ValidationUtils.TryGetGuid("");
                Assert.Fail("Исключение не было выброшено.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Неверное значение уникального идентификатора.", ex.Message);
            }
        }

        #endregion

        #region TryGetDecimal

        [TestMethod]
        public void ValidationUtils_TryGetDecimal_Object_DoesntThrowException()
        {
            Assert.AreEqual(5M, ValidationUtils.TryGetDecimal(5, "Ошибка."));
            Assert.AreEqual(5M, ValidationUtils.TryGetDecimal(5M, "Ошибка."));
            Assert.AreEqual(-1.3M, ValidationUtils.TryGetDecimal(-1.3M, "Ошибка."));

            object obj; decimal dec = -1.3M; string str = "-1.3";
            Assert.AreEqual(-1.3M, ValidationUtils.TryGetDecimal(dec, "Ошибка."));

            obj = dec;
            Assert.AreEqual(-1.3M, ValidationUtils.TryGetDecimal(obj, "Ошибка."));

            obj = str;
            Assert.AreEqual(-1.3M, ValidationUtils.TryGetDecimal(obj, "Ошибка."));
        }

        [TestMethod]
        public void ValidationUtils_TryGetDecimal_String_DoesntThrowException()
        {
            Assert.AreEqual(5M, ValidationUtils.TryGetDecimal("5", "Ошибка."));
            Assert.AreEqual(5M, ValidationUtils.TryGetDecimal("5.0", "Ошибка."));
            Assert.AreEqual(-1.3M, ValidationUtils.TryGetDecimal("-1.3", "Ошибка."));
            Assert.AreEqual(-1.32100101M, ValidationUtils.TryGetDecimal("-1.32100101", "Ошибка."));
            Assert.AreEqual(12321001M, ValidationUtils.TryGetDecimal("12321001", "Ошибка."));
        }

        [TestMethod]
        public void ValidationUtils_TryGetDecimal_Scale_DoesntThrowException()
        {
            Assert.AreEqual(5M, ValidationUtils.TryGetDecimal("5.00", 2, "Ошибка парсинга.", "Ошибка дробной части."));
            Assert.AreEqual(5M, ValidationUtils.TryGetDecimal("5.0", 2, "Ошибка парсинга.", "Ошибка дробной части."));
            Assert.AreEqual(5M, ValidationUtils.TryGetDecimal("5", 2, "Ошибка парсинга.", "Ошибка дробной части."));
            Assert.AreEqual(-1.3M, ValidationUtils.TryGetDecimal("-1.3", 1, "Ошибка парсинга.", "Ошибка дробной части."));
            Assert.AreEqual(-1.321001M, ValidationUtils.TryGetDecimal("-1.321001", 6, "Ошибка парсинга.", "Ошибка дробной части."));
            Assert.AreEqual(12321001M, ValidationUtils.TryGetDecimal("12321001", 0, "Ошибка парсинга.", "Ошибка дробной части."));
        }

        [TestMethod]
        public void ValidationUtils_TryGetDecimal_Scale_ThrowsException()
        {
            try
            {
                Assert.AreEqual(5M, ValidationUtils.TryGetDecimal("5.00", 1, "Ошибка парсинга.", "Ошибка дробной части."));
                Assert.Fail("Исключение не было выброшено.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Ошибка дробной части.", ex.Message);
            }
        }

        [TestMethod]
        public void ValidationUtils_TryGetDecimal_Precision_And_Scale_DoesntThrowException()
        {
            Assert.AreEqual(5M, ValidationUtils.TryGetDecimal("5", 1, 0, "Ошибка.", "Ошибка целой части.", "Ошибка дробной части."));
            Assert.AreEqual(5M, ValidationUtils.TryGetDecimal("5.0", 1, 1, "Ошибка.", "Ошибка целой части.", "Ошибка дробной части."));
            Assert.AreEqual(-1.3M, ValidationUtils.TryGetDecimal("-1.3", 1, 1, "Ошибка.", "Ошибка целой части.", "Ошибка дробной части."));
            Assert.AreEqual(-1.32100101M, ValidationUtils.TryGetDecimal("-1.32100101", 1, 8, "Ошибка.", "Ошибка целой части.", "Ошибка дробной части."));
            Assert.AreEqual(12321001M, ValidationUtils.TryGetDecimal("12321001", 8, 0, "Ошибка.", "Ошибка целой части.", "Ошибка дробной части."));
        }

        [TestMethod]
        public void ValidationUtils_TryGetDecimal_Precision_And_Scale_ThrowsException()
        {
            try
            {
                Assert.AreEqual(5M, ValidationUtils.TryGetDecimal("5.0", 1, 0, "Ошибка.", "Ошибка целой части.", "Ошибка дробной части."));
                Assert.Fail("Исключение не было выброшено.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Ошибка дробной части.", ex.Message);
            }

            try
            {
                Assert.AreEqual(5M, ValidationUtils.TryGetDecimal("15.0", 1, 1, "Ошибка.", "Ошибка целой части.", "Ошибка дробной части."));
                Assert.Fail("Исключение не было выброшено.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Ошибка целой части.", ex.Message);
            }
        }

        #endregion
    }
}
