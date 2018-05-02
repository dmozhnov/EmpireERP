using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP.Utils
{
    public static class VatUtils
    {
        /// <summary>
        /// Расчет суммы, остающейся после взимания НДС (суммы без НДС). Округляет результат до 2 знаков
        /// </summary>
        /// <param name="sumWithVat">Сумма с НДС</param>
        /// <param name="vatPercent">Ставка НДС, в %</param>
        public static decimal CalculateWithoutVatSum(decimal sumWithVat, decimal vatPercent)
        {
            decimal denominator = (1M + vatPercent / 100M);
            ValidationUtils.NotNullOrDefault(denominator, "Ставка НДС не может быть равна -100%.");

            return Math.Round(sumWithVat / denominator, 2);
        }

        /// <summary>
        /// Расчет суммы, остающейся после взимания НДС (суммы без НДС). Округляет результат до 2 знаков
        /// В случае null одного из значений возвращает null
        /// </summary>
        /// <param name="sumWithVat">Сумма с НДС</param>
        /// <param name="vatPercent">Ставка НДС, в %</param>
        public static decimal? CalculateWithoutVatSum(decimal? sumWithVat, decimal? vatPercent)
        {
            return sumWithVat.HasValue && vatPercent.HasValue ? CalculateWithoutVatSum(sumWithVat.Value, vatPercent.Value) : (decimal?)null;
        }

        /// <summary>
        /// Расчет суммы взимаемой НДС. Округляет результат до 2 знаков
        /// </summary>
        /// <param name="sumWithVat">Сумма с НДС</param>
        /// <param name="vatPercent">Ставка НДС, в %</param>
        public static decimal CalculateVatSum(decimal sumWithVat, decimal vatPercent)
        {
            return Math.Round(sumWithVat - CalculateWithoutVatSum(sumWithVat, vatPercent), 2);
        }

        /// <summary>
        /// Расчет суммы взимаемой НДС. Округляет результат до 2 знаков
        /// В случае null одного из значений возвращает null
        /// </summary>
        /// <param name="sumWithVat">Сумма с НДС</param>
        /// <param name="vatPercent">Ставка НДС, в %</param>
        public static decimal? CalculateVatSum(decimal? sumWithVat, decimal? vatPercent)
        {
            return sumWithVat.HasValue && vatPercent.HasValue ? CalculateVatSum(sumWithVat.Value, vatPercent.Value) : (decimal?)null;
        }

        /// <summary>
        /// Получение строки с информацией об НДС по документу, сгруппированной по ставкам (в убывающем порядке)
        /// </summary>
        /// <param name="vatInfoList">Набор пар "Процент НДС" - "Сумма". Может содержать повторяющиеся проценты</param>
        /// <param name="emptyListString">Строка, возвращаемая в случае пустого словаря</param>
        public static string GetValueAddedTaxString(ILookup<decimal, decimal> vatInfoList, decimal waybillVatPercent)
        {
            // Группируем по ставкам (процентам) НДС
            var vatInfoListGroupByVat = vatInfoList.ToDictionary(x => x.Key, x => x.Sum());

            // Если у накладной 0 позиций, выводим ставку ее НДС и сумму 0 р. Это единственный случай, когда выводится НДС самой накладной, а не позиций
            // "Без НДС : 0 р" не выводится, а выводится просто "Без НДС"
            if (vatInfoListGroupByVat.Count == 0)
            {
                return waybillVatPercent != 0M ? String.Format("{0} % : {1} р.", waybillVatPercent.ForDisplay(ValueDisplayType.Percent), 0M.ForDisplay(ValueDisplayType.Money))
                    : "Без НДС";
            }

            // Удаляем из списка сумму с 0 НДС
            if (vatInfoListGroupByVat.ContainsKey(0M))
            {
                vatInfoListGroupByVat.Remove(0M);
            }

            if (vatInfoListGroupByVat.Count == 0)
            {
                return "Без НДС";
            }

            if (vatInfoListGroupByVat.Count == 1)
            {
                return String.Format("{0} % : {1} р.", vatInfoListGroupByVat.First().Key.ForDisplay(ValueDisplayType.Percent),
                    vatInfoListGroupByVat.First().Value.ForDisplay(ValueDisplayType.Money));
            }

            var result = String.Format("{0} р. (", vatInfoListGroupByVat.Sum(x => x.Value).ForDisplay(ValueDisplayType.Money));
            var useSemicolon = false;

            foreach (var valueAddedTax in vatInfoListGroupByVat.OrderByDescending(x => x.Key))
            {
                result += String.Format("{0}{1} % : {2} р.", useSemicolon ? "; " : "", valueAddedTax.Key.ForDisplay(ValueDisplayType.Percent),
                    valueAddedTax.Value.ForDisplay(ValueDisplayType.Money));
                useSemicolon = true;
            }

            result += ")";

            return result;
        }
    }
}
