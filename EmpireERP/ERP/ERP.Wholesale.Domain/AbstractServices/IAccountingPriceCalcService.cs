using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface IAccountingPriceCalcService
    {
        decimal CalculateAccountingPrice(AccountingPriceCalcRule rule, LastDigitCalcRule digitRule, Article article);
        decimal CalculateAccountingPrice(AccountingPriceCalcRule rule, LastDigitCalcRule digitRule, Article article, out bool accPriceCalcError, out bool lastDigitError);
        decimal CalculateAccountingPrice(AccountingPriceCalcRule rule, LastDigitCalcRule digitRule, Article article, out bool error);

        /// <summary>
        /// Рассчитать учетную цену 
        /// </summary>
        /// <param name="rule">Правило расчета учетной цены </param>
        /// <param name="digitRule">Правило расчета последней цифры</param>
        /// <param name="articleList">Список товаров, для которых нужно рассчитывать</param>
        /// <param name="accPriceCalcError">Словарь по товарам: true, если не удалось использовать заданное правило расчета учетной цены и взято правило </param>
        /// <param name="lastDigitError">Словарь по товарам: true, если не удалось использовать заданное правило расчета для последней цифры</param>
        /// <returns>Рассчитанная учетная цена</returns>
        Dictionary<int, decimal> CalculateAccountingPrice(AccountingPriceCalcRule rule, LastDigitCalcRule digitRule,
            IEnumerable<Article> articleList, out Dictionary<int, bool> accPriceCalcError, out Dictionary<int, bool> lastDigitError);
    }
}
