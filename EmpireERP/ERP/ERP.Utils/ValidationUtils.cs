using System;
using System.Text.RegularExpressions;

namespace ERP.Utils
{
    public static class ValidationUtils
    {
        /// <summary>
        /// Проверка условия. Если оно ложно, происходит исключение с выдачей сообщения
        /// </summary>
        /// <param name="condition">Проверяемое условие</param>
        /// <param name="msg">Сообщение</param>
        public static void Assert(bool condition, string msg = "")
        {
            if (!condition)
            {
                if (string.IsNullOrEmpty(msg))
                {
                    msg = "Условие не выполнено.";
                }

                throw new Exception(msg);
            }
        }

        /// <summary>
        /// Проверка условия. Если оно ложно, происходит исключение с выдачей сообщения
        /// </summary>
        /// <param name="condition">Проверяемое условие</param>
        /// <param name="msg">Предикат, возвращающий сообщение</param>
        public static void Assert(bool condition, Func<string> msgPredicate)
        {
            if (!condition)
            {
                var str = msgPredicate();
                Assert(condition, str);
            }
        }

        /// <summary>
        /// Проверка значения на null. В случае "не null" бросает Exception
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="msg"></param>
        public static void IsNull(object obj, string msg = "")
        {
            Assert(obj == null, !String.IsNullOrEmpty(msg) ? msg : "Значение объекта не является пустым.");
        }

        /// <summary>
        /// Проверка значения на null. В случае null бросает Exception
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="msg"></param>
        public static void NotNull(object obj, string msg = "")
        {
            Assert(obj != null, !String.IsNullOrEmpty(msg) ? msg : "Значение объекта не определено.");
        }

        /// <summary>
        /// Проверка значения на null или 0. В случае null или 0 бросает Exception с текстом msg
        /// </summary>
        /// <param name="value"></param>
        /// <param name="msg"></param>
        public static void NotNullOrDefault(this int? value, string msg = "")
        {
            NotNull(value, msg);

            Assert(value != default(int), !String.IsNullOrEmpty(msg) ? msg : "Значение переменной не может быть равно 0.");
        }

        /// <summary>
        /// Проверка значения на null или 0. В случае null или 0 бросает Exception с текстом msg
        /// </summary>
        /// <param name="value"></param>
        /// <param name="msg"></param>
        public static void NotNullOrDefault(this short? value, string msg = "")
        {
            NotNull(value, msg);

            Assert(value != (short)0, !String.IsNullOrEmpty(msg) ? msg : "Значение переменной не может быть равно 0.");
        }

        /// <summary>
        /// Проверка значения на null или 0. В случае null или 0 бросает Exception с текстом msg
        /// </summary>
        /// <param name="value"></param>
        /// <param name="msg"></param>
        public static void NotNullOrDefault(this decimal? value, string msg = "")
        {
            NotNull(value, msg);

            Assert(value != 0M, !String.IsNullOrEmpty(msg) ? msg : "Значение переменной не может быть равно 0.");
        }

        /// <summary>
        /// Проверка значения на null или 0. В случае null или 0 бросает Exception с текстом msg
        /// </summary>
        /// <param name="value"></param>
        /// <param name="msg"></param>
        public static void NotNullOrDefault(this double? value, string msg = "")
        {
            NotNull(value, msg);

            Assert(value != 0.0, !String.IsNullOrEmpty(msg) ? msg : "Значение переменной не может быть равно 0.");
        }

        /// <summary>
        /// Проверка значения на null или Guid.Empty. В случае null или Guid.Empty бросает Exception с текстом msg
        /// </summary>
        /// <param name="value"></param>
        /// <param name="msg"></param>
        public static void NotNullOrDefault(this Guid? value, string msg = "")
        {
            NotNull(value, msg);

            Assert(value != Guid.Empty, !String.IsNullOrEmpty(msg) ? msg : "Значение переменной не может быть пустым.");
        }

        /// <summary>
        /// Проверка значения на null или String.Empty. В случае null или String.Empty бросает Exception с текстом msg
        /// </summary>
        /// <param name="value"></param>
        /// <param name="msg"></param>
        public static void NotNullOrDefault(this string value, string msg = "")
        {
            Assert(!String.IsNullOrEmpty(value), !String.IsNullOrEmpty(msg) ? msg : "Значение переменной не может быть пустым.");
        }

        /// <summary>
        /// Получение значения Guid из строки
        /// </summary>
        /// <param name="value">Строка</param>
        /// <returns></returns>
        public static Guid TryGetGuid(string value, string msg = "")
        {
            Guid guid;
            if (!Guid.TryParse(value, out guid))
            {
                throw new Exception(!String.IsNullOrEmpty(msg) ? msg : "Неверное значение уникального идентификатора.");
            }

            return guid;
        }

        /// <summary>
        /// Получение значения Guid из строки. Значение должно быть не равно Guid.Empty
        /// </summary>
        /// <param name="value">Строка</param>
        /// <returns></returns>
        public static Guid TryGetNotEmptyGuid(string value, string msg = "")
        {
            Guid guid;
            if (!Guid.TryParse(value, out guid) || guid == Guid.Empty)
            {
                throw new Exception(!String.IsNullOrEmpty(msg) ? msg : "Неверное значение уникального идентификатора.");
            }

            return guid;
        }

        /// <summary>
        /// Получение целого значения из строки
        /// </summary>
        /// <param name="value">Строка</param>
        /// <param name="msg">Сообщение при ошибке</param>
        /// <param name="treatEmptyAsZero">Признак нужно ли раценивать пустую строку как нулевое значение</param>
        /// <returns>Целое число</returns>
        public static int TryGetInt(string value, string msg = "", bool treatEmptyAsZero = false)
        {
            int result = 0;
            if (!(treatEmptyAsZero && String.IsNullOrEmpty(value)))
            {
                if (!int.TryParse(value, out result))
                {
                    throw new Exception(!String.IsNullOrEmpty(msg) ? msg : "Значение не является целым числом.");
                }
            }

            return result;
        }

        public static short TryGetShort(string value, string msg = "")
        {
            short result;
            if (!short.TryParse(value, out result))
            {
                throw new Exception(!String.IsNullOrEmpty(msg) ? msg : "Значение не является целым числом.");
            }

            return result;
        }

        public static byte TryGetByte(string value, string msg = "")
        {
            byte result;
            if (!byte.TryParse(value, out result))
            {
                throw new Exception(!String.IsNullOrEmpty(msg) ? msg : "Значение не является целым числом.");
            }

            return result;
        }

        /// <summary>
        /// Метод возвращает значение перечисления. 
        /// </summary>
        /// <typeparam name="T">Тип перечисления</typeparam>
        /// <param name="value">Строка со значением перечисления в виде "MovementWaybill" или "4"</param>
        /// <returns>Значение перечисления</returns>
        public static T TryGetEnum<T>(string value, string msg = "") where T : struct
        {
            T result = default(T);
            if (Enum.TryParse<T>(value, true, out result))
            {
                return result;
            }
            throw new Exception(!String.IsNullOrEmpty(msg) ? msg : "Значение не является членом перечисления.");
        }

        public static decimal TryGetDecimal(object value, string msg = "", bool treatEmptyAsZero = false)
        {
            if (value is string)
            {
                return TryGetDecimal(value as string, msg, treatEmptyAsZero);
            }
            else if (value is decimal)
            {
                return (decimal)value;
            }
            else if (value is int)
            {
                return (decimal)(int)value;
            }
            else
            {
                throw new Exception("Значение имеет недопустимый тип.");
            }
        }

        public static decimal TryGetDecimal(string value, string msg = "", bool treatEmptyAsZero = false)
        {
            if (String.IsNullOrEmpty(value))
            {
                if (treatEmptyAsZero)
                {
                    return 0;
                }
                else
                {
                    throw new Exception(!String.IsNullOrEmpty(msg) ? msg : "Значение не является числом с плавающей точкой.");
                }
            }

            decimal result;
            if (!decimal.TryParse(value.Replace('.', ','), out result))
            {
                throw new Exception(!String.IsNullOrEmpty(msg) ? msg : "Значение не является числом с плавающей точкой.");
            }

            return result;
        }

        /// <summary>
        /// Попытка преобразовать строку в число, ограничив число знаков до и после запятой (точки)
        /// </summary>
        /// <param name="value">Строка</param>
        /// <param name="precision">Максимально допустимое количество знаков до запятой (точки)</param>
        /// <param name="scale">Максимально допустимое количество знаков после запятой (точки)</param>
        /// <param name="parseErrorMsg">Сообщение при ошибке преобразования</param>
        /// <param name="scaleErrorMsg">Сообщение при превышении количества знаков после запятой</param>
        /// <returns>Строка, преобразованная в число</returns>
        public static decimal TryGetDecimal(string value, int precision, int scale, string parseErrorMsg = "", string precisionErrorMsg = "",
            string scaleErrorMsg = "")
        {
            Assert(precision >= 1, "Неверно задано количество знаков до запятой.");
            Assert(scale >= 0, "Неверно задано количество знаков после запятой.");

            Regex parseRegex = new Regex(@"^(-)?[0-9]+([,.][0-9]+)?$", RegexOptions.Compiled);
            ValidationUtils.Assert(parseRegex.IsMatch(value), !String.IsNullOrEmpty(parseErrorMsg) ? parseErrorMsg : "Введите число.");

            Regex scaleRegex = scale > 0 ? new Regex(@"^(-)?[0-9]+([,.][0-9]{1," + scale.ToString() + "})?$", RegexOptions.Compiled) :
                new Regex(@"^(-)?[0-9]+$", RegexOptions.Compiled);
            ValidationUtils.Assert(scaleRegex.IsMatch(value), !String.IsNullOrEmpty(scaleErrorMsg) ? scaleErrorMsg :
                scale > 0 ? String.Format("Введите не более {0} знаков после запятой.", scale) : "Введите целое число.");

            Regex precisionRegex = new Regex(@"^(-)?[0-9]{1," + precision.ToString() + "}([,.][0-9]+)?$", RegexOptions.Compiled);
            ValidationUtils.Assert(precisionRegex.IsMatch(value), !String.IsNullOrEmpty(precisionErrorMsg) ? precisionErrorMsg :
                String.Format("Введите не более {0} знаков до запятой.", precision));

            return TryGetDecimal(value, parseErrorMsg);
        }

        /// <summary>
        /// Попытка преобразовать строку в число, ограничив число знаков после запятой (точки)
        /// </summary>
        /// <param name="value">Строка</param>
        /// <param name="scale">Максимально допустимое количество знаков после запятой (точки)</param>
        /// <param name="parseErrorMsg">Сообщение при ошибке преобразования</param>
        /// <param name="scaleErrorMsg">Сообщение при превышении количества знаков после запятой</param>
        /// <returns>Строка, преобразованная в число</returns>
        public static decimal TryGetDecimal(string value, int scale, string parseErrorMsg = "", string scaleErrorMsg = "")
        {
            Assert(scale >= 0, "Неверно задано количество знаков после запятой.");

            Regex parseRegex = new Regex(@"^(-)?[0-9]+([,.][0-9]+)?$", RegexOptions.Compiled);
            ValidationUtils.Assert(parseRegex.IsMatch(value), !String.IsNullOrEmpty(parseErrorMsg) ? parseErrorMsg : "Введите число.");

            Regex scaleRegex = scale > 0 ? new Regex(@"^(-)?[0-9]+([,.][0-9]{1," + scale.ToString() + "})?$", RegexOptions.Compiled) :
                new Regex(@"^(-)?[0-9]+$", RegexOptions.Compiled);
            ValidationUtils.Assert(scaleRegex.IsMatch(value), !String.IsNullOrEmpty(scaleErrorMsg) ? scaleErrorMsg :
                scale > 0 ? String.Format("Введите не более {0} знаков после запятой.", scale) : "Введите целое число.");

            return TryGetDecimal(value, parseErrorMsg);
        }

        /// <summary>
        /// Проверка числа на количество знаков после запятой (точки)
        /// </summary>
        /// <param name="value">Число</param>
        /// <param name="scale">Максимально допустимое количество знаков после запятой (точки)</param>
        /// <param name="scaleErrorMsg">Сообщение при превышении количества знаков после запятой</param>
        public static void CheckDecimalScale(decimal? value, int scale, string scaleErrorMsg = "")
        {
            if (value.HasValue)
                CheckDecimalScale(value.Value, scale, scaleErrorMsg);
        }

        /// <summary>
        /// Проверка числа на количество знаков после запятой (точки)
        /// </summary>
        /// <param name="value">Число</param>
        /// <param name="precision">Максимально допустимое количество знаков до запятой (точки)</param>
        /// <param name="scale">Максимально допустимое количество знаков после запятой (точки)</param>
        /// <param name="precisionErrorMsg">Сообщение при превышении количества знаков до запятой</param>
        /// <param name="scaleErrorMsg">Сообщение при превышении количества знаков после запятой</param>
        public static void CheckDecimalScale(decimal? value, int precision, int scale, string precisionErrorMsg = "", string scaleErrorMsg = "")
        {
            if (value.HasValue)
                CheckDecimalScale(value.Value, precision, scale, precisionErrorMsg, scaleErrorMsg);
        }

        /// <summary>
        /// Проверка числа на количество знаков после запятой (точки)
        /// </summary>
        /// <param name="value">Число</param>
        /// <param name="scale">Максимально допустимое количество знаков после запятой (точки)</param>
        /// <param name="scaleErrorMsg">Сообщение при превышении количества знаков после запятой</param>
        public static void CheckDecimalScale(decimal value, int scale, string scaleErrorMsg = "")
        {
            Assert(scale >= 0, "Неверно задано количество знаков после запятой.");

            var stringValue = value.ForEdit();

            var pointPosition = stringValue.IndexOfAny(new char[] { '.', ',' });
            if (pointPosition > -1)
            {
                var fractionSize = stringValue.Length - pointPosition - 1;
                ValidationUtils.Assert(fractionSize <= scale, !String.IsNullOrEmpty(scaleErrorMsg) ? scaleErrorMsg :
                    scale > 0 ? String.Format("Введите не более {0} знаков после запятой.", scale) : "Введите целое число.");
            }
        }

        /// <summary>
        /// Проверка числа на количество знаков после запятой (точки)
        /// </summary>
        /// <param name="value">Число</param>
        /// <param name="precision">Максимально допустимое количество знаков до запятой (точки)</param>
        /// <param name="scale">Максимально допустимое количество знаков после запятой (точки)</param>
        /// <param name="precisionErrorMsg">Сообщение при превышении количества знаков до запятой</param>
        /// <param name="scaleErrorMsg">Сообщение при превышении количества знаков после запятой</param>
        public static void CheckDecimalScale(decimal value, int precision, int scale, string precisionErrorMsg = "", string scaleErrorMsg = "")
        {
            Assert(precision >= 1, "Неверно задано количество знаков до запятой.");
            Assert(scale >= 0, "Неверно задано количество знаков после запятой.");

            var stringValue = value.ForEdit();
            int integerSize;

            var pointPosition = stringValue.IndexOfAny(new char[] { '.', ',' });
            if (pointPosition > -1)
            {
                var fractionSize = stringValue.Length - pointPosition - 1;
                integerSize = pointPosition;
                ValidationUtils.Assert(fractionSize <= scale, !String.IsNullOrEmpty(scaleErrorMsg) ? scaleErrorMsg :
                    scale > 0 ? String.Format("Введите не более {0} знаков после запятой.", scale) : "Введите целое число.");
            }
            else
            {
                integerSize = stringValue.Length;
            }

            ValidationUtils.Assert(integerSize <= precision, !String.IsNullOrEmpty(precisionErrorMsg) ? precisionErrorMsg :
                scale > 0 ? String.Format("Введите не более {0} знаков до запятой.", precision) :
                String.Format("Введите целое число, не более {0} знаков.", precision));
        }

        /// <remarks>При изменение функции посмотреть не нужны ли изменения в isTrue и isFalse</remarks>
        public static bool TryGetBool(string value, string msg = "")
        {
            switch (value)
            {
                case "0":
                case "false":
                case "False":
                    return false;
                
                case "1":
                case "true":
                case "True":
                    return true;
                
                default:
                    throw new Exception(!String.IsNullOrEmpty(msg) ? msg : "Значение не является булевским типом.");
            };
        }

        /// <remarks>При изменение функции посмотреть не нужны ли изменения в TryGetBool</remarks>
        public static bool IsTrue(string value)
        {
            switch (value)
            {

                case "1":
                case "true":
                case "True":
                    return true;

                default:
                    return false;
            }
        }

        /// <remarks>При изменение функции посмотреть не нужны ли изменения в TryGetBool</remarks>
        public static bool IsFalse(string value)
        {
            switch (value)
            {

                case "0":
                case "false":
                case "False":
                    return false;

                default:
                    return false;
            }
        }

        public static DateTime TryGetDate(string value, string msg = "")
        {
            DateTime result;

            ValidationUtils.Assert(DateTime.TryParse(value, out result), !String.IsNullOrEmpty(msg) ? msg : "Значение не является датой.");

            return result;
        }
    }
}
