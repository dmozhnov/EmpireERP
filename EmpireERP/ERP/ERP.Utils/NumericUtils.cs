using System;

namespace ERP.Utils
{
    public static class NumericUtils
    {
        /// <summary>
        /// Конвертирует object в decimal, попутно округляя. Выбрасывает исключение, если тип не распознан.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="exceptionText"></param>
        /// <returns></returns>
        public static decimal ConvertObjectToDecimal(object obj, string exceptionText, int roundDigits = 6)
        {
            decimal? cost = null;

            if (obj is int)
                cost = (decimal)(int)obj;
            if (obj is double)
                cost = Math.Round((decimal)(double)obj, roundDigits);
            if (obj is decimal)
                cost = Math.Round((decimal)obj, roundDigits);

            if (!cost.HasValue)
            {
                throw new Exception(exceptionText);
            }

            return cost.Value;
        }
    }
}
