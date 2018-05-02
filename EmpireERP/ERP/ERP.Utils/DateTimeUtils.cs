using System;
using System.Linq;
using System.Collections.Generic;

namespace ERP.Utils
{
    public static class DateTimeUtils
    {
        /// <summary>
        /// Проверяет, находится ли дата в заданном диапазоне
        /// </summary>
        /// <param name="value">проверяемая дата</param>
        /// <param name="start">начало диапазона</param>
        /// <param name="end">конец диапазона</param>
        /// <returns>true, если дата попадает в диапазон</returns>
        public static bool IsInRange(this DateTime value, DateTime start, DateTime end, bool inclusive = true)
        {
            return value.IsInRange(start, (DateTime?)end, inclusive);
        }

        /// <summary>
        /// Проверяет, находится ли дата в заданном диапазоне
        /// </summary>
        /// <param name="value">проверяемая дата</param>
        /// <param name="start">начало диапазона</param>
        /// <param name="end">конец диапазона, <value>null</value> - диапазон не ограничен в будущем</param>
        /// <returns>true, если дата попадает в диапазон</returns>
        public static bool IsInRange(this DateTime value, DateTime start, DateTime? end, bool inclusive = true)
        {
            if (end.HasValue && start > end.Value) // inclusive не влияет на эту проверку
            {
                throw new Exception("Начальная дата диапазона больше конечной даты.");
            }

            if (end.HasValue)
            {
                return start <= value && (inclusive ? (value <= end) : (value < end));
            }
            else
            {
                return start <= value;
            }
        }

        /// <summary>
        /// Установить для даты произвольные часы, минуты и секунды
        /// </summary>
        /// <param name="value">изменяемая дата</param>
        /// <param name="_hour">новое значение часов</param>
        /// <param name="_minute">новое значение минут</param>
        /// <param name="_second">новое значение секунд</param>
        /// <returns></returns>
        public static DateTime SetHoursMinutesAndSeconds(this DateTime value, int _hour, int _minute, int _second)
        {
            return new DateTime(value.Year, value.Month, value.Day, _hour, _minute, _second);
        }

        /// <summary>
        /// Округлить до секунд
        /// </summary>
        /// <param name="value">Исходная дата. Остается неизменной.</param>
        /// <returns></returns>
        public static DateTime RoundToSeconds(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
        }

        /// <summary>
        /// Привести дату к строке вида "гггг-мм-дд чч:мм:сс"
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ConvertToSqlDate(DateTime? date)
        {
            return date == null ? "null" : String.Format("'{0}-{1}-{2} {3}:{4}:{5}'",
                date.Value.Year,
                (date.Value.Month < 10 ? "0" : "") + date.Value.Month,
                (date.Value.Day < 10 ? "0" : "") + date.Value.Day,
                (date.Value.Hour < 10 ? "0" : "") + date.Value.Hour,
                (date.Value.Minute < 10 ? "0" : "") + date.Value.Minute,
                (date.Value.Second < 10 ? "0" : "") + date.Value.Second);
        }

        /// <summary>
        /// Возвращает представление времени в формате HH:mm:ss
        /// </summary>
        public static string ToFullTimeString(this DateTime value)
        {
            return value.ToString("HH:mm:ss");
        }

        /// <summary>
        /// Возвращает представление даты в формате dd.MM.yyyy HH:mm:ss
        /// </summary>
        public static string ToFullDateTimeString(this DateTime value)
        {
            return value.ToString("dd.MM.yyyy HH:mm:ss");
        }

        /// <summary>
        /// Возвращает представление даты в формате dd.MM.yyyy HH:mm
        /// </summary>
        public static string ToShortDateTimeString(this DateTime value)
        {
            return value.ToString("dd.MM.yyyy HH:mm");
        }

	    /// <summary>
        /// Получение текущих даты и времени без милисекунд
        /// </summary>
        public static DateTime GetCurrentDateTime()
        {
            var now = DateTime.Now;

            return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
        }

        /// <summary>
        /// Прибавить указанное количество рабочих дней к дате.
        /// Если дата - выходной день, делаем так же, как если бы нам передали ближайший следующий рабочий день
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="daysCount">Количество рабочих дней</param>
        /// <param name="mondayIsWorkDay"></param>
        /// <param name="tuesdayIsWorkDay"></param>
        /// <param name="wednesdayIsWorkDay"></param>
        /// <param name="thursdayIsWorkDay"></param>
        /// <param name="fridayIsWorkDay"></param>
        /// <param name="saturdayIsWorkDay"></param>
        /// <param name="sundayIsWorkDay"></param>
        /// <returns></returns>
        public static DateTime AddWorkDays(DateTime startDate, int daysCount,
            bool mondayIsWorkDay, bool tuesdayIsWorkDay, bool wednesdayIsWorkDay, bool thursdayIsWorkDay, bool fridayIsWorkDay, bool saturdayIsWorkDay, bool sundayIsWorkDay)
        {
            ValidationUtils.Assert(daysCount >= 0, "Количество рабочих дней не может быть отрицательным.");
            ValidationUtils.Assert(daysCount <= 100000, "Слишком большое количество рабочих дней.");
            CheckWorkDaysPlan(mondayIsWorkDay, tuesdayIsWorkDay, wednesdayIsWorkDay, thursdayIsWorkDay, fridayIsWorkDay, saturdayIsWorkDay, sundayIsWorkDay);

            var date = startDate;
            while (daysCount > 0)
            {
                date = NextWorkDay(date, mondayIsWorkDay, tuesdayIsWorkDay, wednesdayIsWorkDay, thursdayIsWorkDay, fridayIsWorkDay, saturdayIsWorkDay, sundayIsWorkDay);
                daysCount--;
            }

            return date;
        }

        /// <summary>
        /// Получить следующий рабочий день после текущего рабочего.
        /// Если переданный день - выходной, пропускаем 2 рабочих дня.
        /// Например: если на 5-дневке передан понедельник, возвращаем вторник.
        /// Если на 5-дневке передана суббота, возвращаем вторник
        /// </summary>
        /// <param name="startDate">Текущий день</param>
        /// <param name="mondayIsWorkDay"></param>
        /// <param name="tuesdayIsWorkDay"></param>
        /// <param name="wednesdayIsWorkDay"></param>
        /// <param name="thursdayIsWorkDay"></param>
        /// <param name="fridayIsWorkDay"></param>
        /// <param name="saturdayIsWorkDay"></param>
        /// <param name="sundayIsWorkDay"></param>
        /// <returns>Следующий рабочий день</returns>
        public static DateTime NextWorkDay(DateTime startDate,
            bool mondayIsWorkDay, bool tuesdayIsWorkDay, bool wednesdayIsWorkDay, bool thursdayIsWorkDay, bool fridayIsWorkDay, bool saturdayIsWorkDay, bool sundayIsWorkDay)
        {
            CheckWorkDaysPlan(mondayIsWorkDay, tuesdayIsWorkDay, wednesdayIsWorkDay, thursdayIsWorkDay, fridayIsWorkDay, saturdayIsWorkDay, sundayIsWorkDay);

            var date = startDate;

            // Пропускаем выходные дни, пока не найдем рабочий
            while (!IsWorkDay(date, mondayIsWorkDay, tuesdayIsWorkDay, wednesdayIsWorkDay, thursdayIsWorkDay, fridayIsWorkDay, saturdayIsWorkDay, sundayIsWorkDay))
            {
                date = date.AddDays(1);
            }

            // Теперь ищем следующий рабочий
            do
            {
                date = date.AddDays(1);
            }
            while (!IsWorkDay(date, mondayIsWorkDay, tuesdayIsWorkDay, wednesdayIsWorkDay, thursdayIsWorkDay, fridayIsWorkDay, saturdayIsWorkDay, sundayIsWorkDay));

            return date;
        }

        /// <summary>
        /// Проверка, является ли данный день рабочим днем
        /// </summary>
        /// <param name="date"></param>
        /// <param name="mondayIsWorkDay"></param>
        /// <param name="tuesdayIsWorkDay"></param>
        /// <param name="wednesdayIsWorkDay"></param>
        /// <param name="thursdayIsWorkDay"></param>
        /// <param name="fridayIsWorkDay"></param>
        /// <param name="saturdayIsWorkDay"></param>
        /// <param name="sundayIsWorkDay"></param>
        /// <returns></returns>
        public static bool IsWorkDay(DateTime date,
            bool mondayIsWorkDay, bool tuesdayIsWorkDay, bool wednesdayIsWorkDay, bool thursdayIsWorkDay, bool fridayIsWorkDay, bool saturdayIsWorkDay, bool sundayIsWorkDay)
        {
            CheckWorkDaysPlan(mondayIsWorkDay, tuesdayIsWorkDay, wednesdayIsWorkDay, thursdayIsWorkDay, fridayIsWorkDay, saturdayIsWorkDay, sundayIsWorkDay);

            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return mondayIsWorkDay;
                case DayOfWeek.Tuesday:
                    return tuesdayIsWorkDay;
                case DayOfWeek.Wednesday:
                    return wednesdayIsWorkDay;
                case DayOfWeek.Thursday:
                    return thursdayIsWorkDay;
                case DayOfWeek.Friday:
                    return fridayIsWorkDay;
                case DayOfWeek.Saturday:
                    return saturdayIsWorkDay;
                case DayOfWeek.Sunday:
                    return sundayIsWorkDay;
                default:
                    throw new Exception("Неизвестный день недели.");
            };
        }

        /// <summary>
        /// Проверка того, что план рабочих дней заполнен (если нет ни одного рабочего дня, происходит исключение)
        /// </summary>
        /// <param name="mondayIsWorkDay"></param>
        /// <param name="tuesdayIsWorkDay"></param>
        /// <param name="wednesdayIsWorkDay"></param>
        /// <param name="thursdayIsWorkDay"></param>
        /// <param name="fridayIsWorkDay"></param>
        /// <param name="saturdayIsWorkDay"></param>
        /// <param name="sundayIsWorkDay"></param>
        public static void CheckWorkDaysPlan(bool mondayIsWorkDay, bool tuesdayIsWorkDay, bool wednesdayIsWorkDay, bool thursdayIsWorkDay, bool fridayIsWorkDay, bool saturdayIsWorkDay, bool sundayIsWorkDay)
        {
            ValidationUtils.Assert(mondayIsWorkDay || tuesdayIsWorkDay || wednesdayIsWorkDay || thursdayIsWorkDay || fridayIsWorkDay || saturdayIsWorkDay || sundayIsWorkDay,
                "График рабочих дней должен содержать хотя бы один рабочий день.");
        }

        /// <summary>
        /// Получение названия месяца в родительном падеже
        /// </summary>
        /// <param name="monthNumber">Номер месяца (1 - январь)</param>
        /// <returns></returns>
        public static string GetGenitiveMonthName(int monthNumber)
        {
            string result;
            switch (monthNumber)
            {
                case 1:
                    result = "января";
                    break;
                case 2:
                    result = "февраля";
                    break;
                case 3:
                    result = "марта";
                    break;
                case 4:
                    result = "апреля";
                    break;
                case 5:
                    result = "мая";
                    break;
                case 6:
                    result = "июня";
                    break;
                case 7:
                    result = "июля";
                    break;
                case 8:
                    result = "августа";
                    break;
                case 9:
                    result = "сентября";
                    break;
                case 10:
                    result = "октября";
                    break;
                case 11:
                    result = "ноября";
                    break;
                case 12:
                    result = "декабря";
                    break;
                default:
                    throw new Exception(String.Format("Месяца под номером «{0}» не существует.", monthNumber));
            }

            return result;
        }

        /// <summary>
        /// Получение минимальной даты из коллекции
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DateTime? GetMinDate(IEnumerable<DateTime> list)
        {
            DateTime? value = null;
            foreach (var data in list)
            {
                if ((value == null && data != null) || value > data)
                {
                    value = data;
                }
            }

            return value;
        }

        /// <summary>
        /// Получение максимальной даты из коллекции
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DateTime? GetMaxDate(IEnumerable<DateTime> list)
        {
            DateTime? value = null;
            foreach (var date in list)
            {
                if ((value == null && date != null) || value < date)
                {
                    value = date;
                }
            }

            return value;
        }

        /// <summary>
        /// Получение максимальной из двух дат
        /// </summary>
        public static DateTime GetMaxDate(DateTime firstDate, DateTime secondDate)
        {
            ValidationUtils.NotNull(firstDate, "Не указана первая дата для сравнения.");
            ValidationUtils.NotNull(secondDate, "Не указана вторая дата для сравнения.");
            
            return firstDate > secondDate ? firstDate : secondDate;
        }
    }
}
