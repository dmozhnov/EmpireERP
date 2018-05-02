using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.Utils.Test
{
    [TestClass]
    public class DateTimeUtilsTest
    {
        #region Проверка диапазона

        [TestMethod]
        public void DateTimeUtils_IsInRange_Fit()
        {
            Assert.IsTrue(new DateTime(2000, 6, 6).IsInRange(new DateTime(2000, 3, 3), new DateTime(2000, 9, 9)));
            Assert.IsTrue(new DateTime(2000, 1, 1).IsInRange(new DateTime(1999, 1, 1), new DateTime(2001, 1, 1)));
            Assert.IsTrue(new DateTime(2000, 1, 1).IsInRange(new DateTime(1999, 1, 1), new DateTime(2001, 1, 1), false));
            Assert.IsTrue(new DateTime(2000, 1, 1, 12, 50, 40).IsInRange(new DateTime(2000, 1, 1, 12, 40, 00), new DateTime(2000, 1, 1, 12, 59, 59)));
            Assert.IsTrue(new DateTime(2000, 6, 6).IsInRange(new DateTime(2000, 3, 3), null));
            Assert.IsTrue(new DateTime(2000, 1, 1).IsInRange(new DateTime(1999, 1, 1), null));
            Assert.IsTrue(new DateTime(2000, 1, 1).IsInRange(new DateTime(1999, 1, 1), null, false));
            Assert.IsTrue(new DateTime(2000, 1, 1, 12, 50, 40).IsInRange(new DateTime(2000, 1, 1, 12, 40, 00), null));
        }

        [TestMethod]
        public void DateTimeUtils_IsInRange_NotFit_BecauseOfTooEarly()
        {
            Assert.IsFalse(new DateTime(2000, 2, 2).IsInRange(new DateTime(2000, 3, 3), new DateTime(2000, 9, 9)));
            Assert.IsFalse(new DateTime(1998, 1, 1).IsInRange(new DateTime(1999, 1, 1), new DateTime(2001, 1, 1)));
            Assert.IsFalse(new DateTime(2000, 1, 1, 12, 30, 40).IsInRange(new DateTime(2000, 1, 1, 12, 40, 00), new DateTime(2000, 1, 1, 12, 59, 59)));
            Assert.IsFalse(new DateTime(2000, 2, 2).IsInRange(new DateTime(2000, 3, 3), null));
            Assert.IsFalse(new DateTime(1998, 1, 1).IsInRange(new DateTime(1999, 1, 1), null));
            Assert.IsFalse(new DateTime(2000, 1, 1, 12, 30, 40).IsInRange(new DateTime(2000, 1, 1, 12, 40, 00), null));
        }

        [TestMethod]
        public void DateTimeUtils_IsInRange_NotFit_BecauseOfTooLate()
        {
            Assert.IsFalse(new DateTime(2000, 10, 10).IsInRange(new DateTime(2000, 3, 3), new DateTime(2000, 9, 9)));
            Assert.IsFalse(new DateTime(2002, 1, 1).IsInRange(new DateTime(1999, 1, 1), new DateTime(2001, 1, 1)));
            Assert.IsFalse(new DateTime(2000, 1, 1, 12, 59, 59).IsInRange(new DateTime(2000, 1, 1, 12, 40, 00), new DateTime(2000, 1, 1, 12, 59, 50)));
        }

        [TestMethod]
        public void DateTimeUtils_IsInRange_StartGreaterThanEnd()
        {
            try
            {
                bool result = new DateTime(2000, 10, 10).IsInRange(new DateTime(2000, 3, 3), new DateTime(1999, 9, 9));
                Assert.Fail(String.Format("Исключение не вызвано. Возвращено {0}.", result.ToString()));
            }
            catch(Exception ex)
            {
                Assert.AreEqual("Начальная дата диапазона больше конечной даты.", ex.Message);
            }
        }

        [TestMethod]
        public void DateTimeUtils_InclusiveLogic()
        {
            Assert.IsTrue(new DateTime(2000, 7, 12, 12, 30, 45).IsInRange(new DateTime(2000, 7, 12, 12, 30, 45), new DateTime(2000, 7, 12, 12, 30, 45)));
            Assert.IsTrue(new DateTime(2000, 7, 12, 12, 30, 45).IsInRange(new DateTime(2000, 7, 12, 12, 30, 45), new DateTime(2000, 7, 12, 12, 30, 45), true));

            // Промежуток 30:45 - 30:46, включительно
            Assert.IsFalse(new DateTime(2000, 7, 12, 12, 30, 44).IsInRange(new DateTime(2000, 7, 12, 12, 30, 45), new DateTime(2000, 7, 12, 12, 30, 46), true));
            Assert.IsTrue(new DateTime(2000, 7, 12, 12, 30, 45).IsInRange(new DateTime(2000, 7, 12, 12, 30, 45), new DateTime(2000, 7, 12, 12, 30, 46), true));
            Assert.IsTrue(new DateTime(2000, 7, 12, 12, 30, 46).IsInRange(new DateTime(2000, 7, 12, 12, 30, 45), new DateTime(2000, 7, 12, 12, 30, 46), true));
            Assert.IsFalse(new DateTime(2000, 7, 12, 12, 30, 47).IsInRange(new DateTime(2000, 7, 12, 12, 30, 45), new DateTime(2000, 7, 12, 12, 30, 46), true));

            // Промежуток 30:45 - 30:45, не включая конец (т.е. пустой)
            Assert.IsFalse(new DateTime(2000, 7, 12, 12, 30, 44).IsInRange(new DateTime(2000, 7, 12, 12, 30, 45), new DateTime(2000, 7, 12, 12, 30, 45), false));
            Assert.IsFalse(new DateTime(2000, 7, 12, 12, 30, 45).IsInRange(new DateTime(2000, 7, 12, 12, 30, 45), new DateTime(2000, 7, 12, 12, 30, 45), false));
            Assert.IsFalse(new DateTime(2000, 7, 12, 12, 30, 46).IsInRange(new DateTime(2000, 7, 12, 12, 30, 45), new DateTime(2000, 7, 12, 12, 30, 45), false));

            // Промежуток 30:45 - 30:45, включая конец (т.е. только 30:45)
            Assert.IsFalse(new DateTime(2000, 7, 12, 12, 30, 44).IsInRange(new DateTime(2000, 7, 12, 12, 30, 45), new DateTime(2000, 7, 12, 12, 30, 45), true));
            Assert.IsTrue(new DateTime(2000, 7, 12, 12, 30, 45).IsInRange(new DateTime(2000, 7, 12, 12, 30, 45), new DateTime(2000, 7, 12, 12, 30, 45), true));
            Assert.IsFalse(new DateTime(2000, 7, 12, 12, 30, 46).IsInRange(new DateTime(2000, 7, 12, 12, 30, 45), new DateTime(2000, 7, 12, 12, 30, 45), true));

            // Промежуток 30:45 - 30:47, не включая конец (т.е. включает 30:45, 30:46).
            Assert.IsFalse(new DateTime(2000, 7, 12, 12, 30, 44).IsInRange(new DateTime(2000, 7, 12, 12, 30, 45), new DateTime(2000, 7, 12, 12, 30, 47), false));
            Assert.IsTrue(new DateTime(2000, 7, 12, 12, 30, 45).IsInRange(new DateTime(2000, 7, 12, 12, 30, 45), new DateTime(2000, 7, 12, 12, 30, 47), false));
            Assert.IsTrue(new DateTime(2000, 7, 12, 12, 30, 46).IsInRange(new DateTime(2000, 7, 12, 12, 30, 45), new DateTime(2000, 7, 12, 12, 30, 47), false));
            Assert.IsFalse(new DateTime(2000, 7, 12, 12, 30, 47).IsInRange(new DateTime(2000, 7, 12, 12, 30, 45), new DateTime(2000, 7, 12, 12, 30, 47), false));
            Assert.IsFalse(new DateTime(2000, 7, 12, 12, 30, 48).IsInRange(new DateTime(2000, 7, 12, 12, 30, 45), new DateTime(2000, 7, 12, 12, 30, 47), false));
        }

        #endregion

        #region Установка часов, минут и секунд, не трогая дату

        [TestMethod]
        public void DateTimeUtils_ClearHourMinuteSecond()
        {
            DateTime date = new DateTime(2011, 5, 17, 12, 56, 9);
            date = date.SetHoursMinutesAndSeconds(0, 0, 0);

            Assert.AreEqual(2011, date.Year);
            Assert.AreEqual(5, date.Month);
            Assert.AreEqual(17, date.Day);
            Assert.AreEqual(0, date.Hour);
            Assert.AreEqual(0, date.Minute);
            Assert.AreEqual(0, date.Second);
        }

        [TestMethod]
        public void DateTimeUtils_SetHourMinuteSecond1()
        {
            DateTime date = new DateTime(2011, 5, 17, 12, 56, 9);
            date = date.SetHoursMinutesAndSeconds(23, 59, 59);

            Assert.AreEqual(2011, date.Year);
            Assert.AreEqual(5, date.Month);
            Assert.AreEqual(17, date.Day);
            Assert.AreEqual(23, date.Hour);
            Assert.AreEqual(59, date.Minute);
            Assert.AreEqual(59, date.Second);
        }

        [TestMethod]
        public void DateTimeUtils_SetHourMinuteSecond2()
        {
            DateTime date = new DateTime(2011, 5, 17, 12, 56, 9);
            date = date.SetHoursMinutesAndSeconds(20, 3, 48);

            Assert.AreEqual(2011, date.Year);
            Assert.AreEqual(5, date.Month);
            Assert.AreEqual(17, date.Day);
            Assert.AreEqual(20, date.Hour);
            Assert.AreEqual(3, date.Minute);
            Assert.AreEqual(48, date.Second);
        }

        #endregion

        #region AddWorkDays

        /// <summary>
        /// График рабочих дней: 5-дневка. Начальный день: четверг недели 1. Прибавить 3 рабочих дня.
        /// Должен вернуть вторник недели 2
        /// </summary>
        [TestMethod]
        public void DateTimeUtils_AddWorkDays_AfterThursday_In5DaysWeek_MustBeTuesday()
        {
            DateTime date = new DateTime(2011, 5, 19, 12, 56, 9);
            Assert.AreEqual(DayOfWeek.Thursday, date.DayOfWeek);

            date = DateTimeUtils.AddWorkDays(date, 3, true, true, true, true, true, false, false);

            Assert.AreEqual(DayOfWeek.Tuesday, date.DayOfWeek);
            Assert.AreEqual(2011, date.Year);
            Assert.AreEqual(5, date.Month);
            Assert.AreEqual(24, date.Day);
            Assert.AreEqual(12, date.Hour);
            Assert.AreEqual(56, date.Minute);
            Assert.AreEqual(9, date.Second);
        }

        /// <summary>
        /// График рабочих дней: только среда. Начальный день: понедельник недели 1. Прибавить 2 рабочих дня.
        /// Должен вернуть среду недели 3
        /// </summary>
        [TestMethod]
        public void DateTimeUtils_AddWorkDays_AfterMonday_MustBeWednesday()
        {
            DateTime date = new DateTime(2011, 5, 16, 12, 56, 9);
            Assert.AreEqual(DayOfWeek.Monday, date.DayOfWeek);

            date = DateTimeUtils.AddWorkDays(date, 2, false, false, true, false, false, false, false);

            Assert.AreEqual(DayOfWeek.Wednesday, date.DayOfWeek);
            Assert.AreEqual(2011, date.Year);
            Assert.AreEqual(6, date.Month);
            Assert.AreEqual(1, date.Day);
            Assert.AreEqual(12, date.Hour);
            Assert.AreEqual(56, date.Minute);
            Assert.AreEqual(9, date.Second);
        }
        
        #endregion

        #region NextWorkDay

        /// <summary>
        /// График рабочих дней: только среда. Начальный день: понедельник.
        /// Пропустить один рабочий день: должен вернуть среду следующей недели
        /// </summary>
        [TestMethod]
        public void DateTimeUtils_NextWorkDay_AfterMonday_MustBeWednesday()
        {
            DateTime date = new DateTime(2011, 5, 16, 12, 56, 9);
            Assert.AreEqual(DayOfWeek.Monday, date.DayOfWeek);

            date = DateTimeUtils.NextWorkDay(date, false, false, true, false, false, false, false);

            Assert.AreEqual(DayOfWeek.Wednesday, date.DayOfWeek);
            Assert.AreEqual(2011, date.Year);
            Assert.AreEqual(5, date.Month);
            Assert.AreEqual(25, date.Day);
            Assert.AreEqual(12, date.Hour);
            Assert.AreEqual(56, date.Minute);
            Assert.AreEqual(9, date.Second);
        }

        /// <summary>
        /// График рабочих дней: только среда. Начальный день: среда.
        /// Пропустить один рабочий день: должен вернуть среду следующей недели
        /// </summary>
        [TestMethod]
        public void DateTimeUtils_NextWorkDay_AfterWednesday_MustBeWednesday()
        {
            DateTime date = new DateTime(2011, 5, 18, 12, 56, 9);
            Assert.AreEqual(DayOfWeek.Wednesday, date.DayOfWeek);

            date = DateTimeUtils.NextWorkDay(date, false, false, true, false, false, false, false);

            Assert.AreEqual(DayOfWeek.Wednesday, date.DayOfWeek);
            Assert.AreEqual(2011, date.Year);
            Assert.AreEqual(5, date.Month);
            Assert.AreEqual(25, date.Day);
            Assert.AreEqual(12, date.Hour);
            Assert.AreEqual(56, date.Minute);
            Assert.AreEqual(9, date.Second);
        }

        /// <summary>
        /// График рабочих дней: ПН - ПТ. Начальный день: вторник.
        /// Пропустить один рабочий день: должен вернуть среду этой же недели
        /// </summary>
        [TestMethod]
        public void DateTimeUtils_NextWorkDay_AfterTuesday_MustBeWednesday()
        {
            DateTime date = new DateTime(2011, 5, 17, 12, 56, 9);
            Assert.AreEqual(DayOfWeek.Tuesday, date.DayOfWeek);

            date = DateTimeUtils.NextWorkDay(date, true, true, true, true, true, false, false);

            Assert.AreEqual(DayOfWeek.Wednesday, date.DayOfWeek);
            Assert.AreEqual(2011, date.Year);
            Assert.AreEqual(5, date.Month);
            Assert.AreEqual(18, date.Day);
            Assert.AreEqual(12, date.Hour);
            Assert.AreEqual(56, date.Minute);
            Assert.AreEqual(9, date.Second);
        }

        #endregion
    }
}
