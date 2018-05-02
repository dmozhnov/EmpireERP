using ERP.Wholesale.Domain.Entities;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface IAccountingPriceCalcRuleService
    {
        AccountingPriceCalcRule GetReadyAccountingPriceCalcRule(AccountingPriceCalcRule rule, int articleId, User user);
        AccountingPriceCalcRule GetReadyAccountingPriceCalcRule(AccountingPriceCalcRule rule, IEnumerable<int> articleIdList, User user);

        LastDigitCalcRule GetReadyLastDigitCalcRule(LastDigitCalcRule rule, int articleId, User user);
        LastDigitCalcRule GetReadyLastDigitCalcRule(LastDigitCalcRule rule, IEnumerable<int> articleIdList, User user);

        /// <summary>
        /// Подготовка дефолтных правил к работе. Метод должен быть вызван перед подсчетом цен, иначе, в случае если по указанному правилу цену подсчитать не получится, будет выброшен эксепшн.
        /// </summary>        
        void InitializeDefaultRules(AccountingPriceCalcRule accountingPriceCalcRule, LastDigitCalcRule lastDigitCalcRule, IEnumerable<int> articleIdList, User user);
    }
}
