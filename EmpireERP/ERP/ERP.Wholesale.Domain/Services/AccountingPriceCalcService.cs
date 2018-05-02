using System;
using System.Collections.Generic;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Services
{
    public class AccountingPriceCalcService : IAccountingPriceCalcService
    {
        public AccountingPriceCalcService()
        {                    
        }

        #region Публичные методы

        /// <summary>
        /// Рассчитать учетную цену 
        /// </summary>
        /// <param name="rule">Правило расчета учетной цены </param>
        /// <param name="digitRule">Правило расчета последней цифры</param>
        /// <param name="article">Товар, для которого рассчитывать</param>
        /// <returns>Рассчитанная учетная цена</returns>
        public decimal CalculateAccountingPrice(AccountingPriceCalcRule rule, LastDigitCalcRule digitRule, Article article)
        {
            bool accPriceCalcError = false, lastDigitError = false;

            return CalculateAccountingPrice(rule, digitRule, article, out accPriceCalcError, out lastDigitError);
        }
      
        /// <summary>
        /// Рассчитать учетную цену 
        /// </summary>
        /// <param name="rule">Правило расчета учетной цены </param>
        /// <param name="digitRule">Правило расчета последней цифры</param>
        /// <param name="article">Товар, для которого рассчитывать</param>  
        /// <param name="error">Признак того, что не удалось применить одно из правил</param>  
        /// <returns>Рассчитанная учетная цена</returns>
        public decimal CalculateAccountingPrice(AccountingPriceCalcRule rule, LastDigitCalcRule digitRule, Article article, out bool error)
        {
            bool accPriceCalcError = false, lastDigitError = false;
            var accPrice = CalculateAccountingPrice(rule, digitRule, article, out accPriceCalcError, out lastDigitError);

            error = accPriceCalcError & lastDigitError;

            return accPrice;
        }
        
        /// <summary>
        /// Рассчитать учетную цену
        /// </summary>
        /// <param name="rule">Правило расчета учетной цены</param>
        /// <param name="digitRule">Правило расчета последней цифры</param>
        /// <param name="article">Товар, для которого рассчитывать</param>
        /// <param name="accPriceCalcError">true, если не удалось использовать заданное правило расчета учетной цены </param>
        /// <param name="lastDigitError">true, если не удалось использовать заданное правило расчета последней цифры</param>
        public decimal CalculateAccountingPrice(AccountingPriceCalcRule rule, LastDigitCalcRule digitRule, Article article, out bool accPriceCalcError, out bool lastDigitError)
        {
            accPriceCalcError = false;
            lastDigitError = false;
            decimal? accountingPrice = rule.CalculateAccountingPriceValue(article);

            if (accountingPrice == null)
            {
                accPriceCalcError = true;
                return 0;
            }
           
            if (accountingPrice <= 0)
            {
                return 0;
            }

            var finalAccountingPrice = digitRule.CalculateLastDigit(article, accountingPrice.Value);

            if (finalAccountingPrice == null)
            {
                lastDigitError = true;
                return 0;
            }

            return finalAccountingPrice.Value;
        }

        /// <summary>
        /// Рассчитать учетную цену 
        /// </summary>
        /// <param name="rule">Правило расчета учетной цены </param>
        /// <param name="digitRule">Правило расчета последней цифры</param>
        /// <param name="articleList">Список товаров, для которых нужно рассчитывать</param>
        /// <param name="accPriceCalcError">Словарь по товарам: true, если не удалось использовать заданное правило расчета учетной цены</param>
        /// <param name="lastDigitError">Словарь по товарам: true, если не удалось использовать заданное правило расчета для последней цифры</param>
        /// <returns>Рассчитанная учетная цена</returns>
        public Dictionary<int, decimal> CalculateAccountingPrice(AccountingPriceCalcRule rule, LastDigitCalcRule digitRule,
            IEnumerable<Article> articleList, out Dictionary<int, bool> accPriceCalcError, out Dictionary<int, bool> lastDigitError)
        {
            var result = new Dictionary<int, decimal>();
            bool accPriceCalcErrorForArticle = false, lastDigitErrorForArticle = false;

            accPriceCalcError = new Dictionary<int, bool>();
            lastDigitError = new Dictionary<int, bool>();

            foreach (var article in articleList)
            {
                result.Add(article.Id, CalculateAccountingPrice(rule, digitRule, article, out accPriceCalcErrorForArticle,
                    out lastDigitErrorForArticle));

                // сохраняем информацию о том, что не удалось использовать заданное правило
                accPriceCalcError.Add(article.Id, accPriceCalcErrorForArticle);
                lastDigitError.Add(article.Id, lastDigitErrorForArticle);
            }

            return result;
        }

        #endregion
    }    
}