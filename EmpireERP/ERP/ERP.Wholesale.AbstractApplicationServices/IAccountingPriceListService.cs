using System;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IAccountingPriceListService
    {        
        void GetTipsForArticle(AccountingPriceList accountingPriceList, Article article, out decimal? avgAccPrice, out decimal? minAccPrice, 
            out decimal? maxAccPrice, out decimal? avgPurchaseCost, out decimal? minPurchaseCost, out decimal? maxPurchaseCost, out decimal? lastPurchaseCost, User user);
        decimal? CalculateDefaultAccountingPriceByRule(AccountingPriceList accountingPriceList, Article article, out bool accPriceCalc, out bool lastDigitError, User user);

        /// <summary>
        /// Расчет УЦ на основании правила
        /// </summary>
        /// <param name="accountingPriceList">РЦ</param>
        /// <param name="articleList">Список товаров, для которых рассчитывается УЦ</param>
        /// <param name="accPriceCalc">Словарь по товарам: true, если не удалось использовать заданное правило расчета учетной цены</param>
        /// <param name="lastDigitError">Словарь по товарам: true, если не удалось использовать заданное правило расчета последней цифры</param>
        /// <param name="user">Пользователь</param>
        /// <returns>Словарь [код товара][рассчитанная УЦ]</returns>
        Dictionary<int, decimal> CalculateDefaultAccountingPriceByRule(AccountingPriceList accountingPriceList,
            IEnumerable<Article> articleList, out Dictionary<int, bool> accPriceCalc, out Dictionary<int, bool> lastDigitError, User user);

        decimal? GetAccountingPriceForArticle(AccountingPriceList accountingPriceList, Guid? articleAccountingPriceId = null);
        IList<AccountingPriceList> GetFilteredList(object state, ParameterString parameterString, User user);
        AccountingPriceList CheckAccountingPriceListExistence(Guid id, User user);

        void DeleteStorage(AccountingPriceList accPriceList, Storage storage, IEnumerable<Storage> storageList, User user);
        void AddStorage(AccountingPriceList accPriceList, Storage storage, IEnumerable<Storage> storageList, User user);
        
        string GetNextNumber();
        bool IsNumberUnique(string number);
        void Save(AccountingPriceList accountingPriceList);
        void Delete(AccountingPriceList accountingPriceList, User user);
        void DeleteArticleAccountingPrice(AccountingPriceList accountingPriceList, ArticleAccountingPrice articleAccountingPrice);

        void Accept(AccountingPriceList accountingPriceList, DateTime currentDateTime, User user);
        void CancelAcceptance(AccountingPriceList accountingPriceList, DateTime currentDateTime, User user);

        IEnumerable<int> GetArticlesListWithNoAccountingPrice(Storage storage, IEnumerable<Storage> availableStoragesList);

        bool IsPossibilityToViewDetails(AccountingPriceList priceList, User user);
        bool IsPossibilityToEdit(AccountingPriceList priceList, User user);
        bool IsPossibilityToDelete(AccountingPriceList priceList, User user);
        bool IsPossibilityToEditRow(AccountingPriceList priceList, User user);
        bool IsPossibilityToDeleteRow(AccountingPriceList priceList, User user);
        bool IsPossibilityToAccept(AccountingPriceList priceList, User user, bool checkForConflictingPriceLists = true);
        bool IsPossibilityToCancelAcceptation(AccountingPriceList priceList, User user, bool checkAccountingPriceListDependencies = true);
        bool IsPossibilityToEditPrice(AccountingPriceList priceList, User user);
        bool IsPossibilityToAddRow(AccountingPriceList priceList, User user);
        bool IsPossibilityToAddStorage(AccountingPriceList priceList, User user);
        bool IsPossibilityToRemoveStorage(AccountingPriceList priceList, User user);
        bool IsPossibilityToPrintForms(AccountingPriceList accountingPriceList, User user);

        void CheckPossibilityToViewDetails(AccountingPriceList priceList, User user);
        void CheckPossibilityToEdit(AccountingPriceList priceList, User user);
        void CheckPossibilityToDelete(AccountingPriceList priceList, User user);
        void CheckPossibilityToEditRow(AccountingPriceList priceList, User user);
        void CheckPossibilityToDeleteRow(AccountingPriceList priceList, User user);
        void CheckPossibilityToAccept(AccountingPriceList priceList, User user, bool checkForConflictingPriceLists = true);
        void CheckPossibilityToCancelAcceptance(AccountingPriceList priceList, User user, bool checkAccountingPriceListDependencies = true);
        void CheckPossibilityToEditPrice(AccountingPriceList priceList, User user);
        void CheckPossibilityToAddRow(AccountingPriceList priceList, User user);
        void CheckPossibilityToAddStorage(AccountingPriceList priceList, User user);
        void CheckPossibilityToRemoveStorage(AccountingPriceList priceList, User user);
        void CheckPossibilityToPrintForms(AccountingPriceList accountingPriceList, User user);
    }
}
