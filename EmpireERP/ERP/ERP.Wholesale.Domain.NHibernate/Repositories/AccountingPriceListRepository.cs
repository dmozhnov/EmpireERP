using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;
using NHibernate.Linq;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class AccountingPriceListRepository : BaseNHRepository, IAccountingPriceListRepository
    {
        public AccountingPriceListRepository() : base()
        {
        }

        public AccountingPriceList GetById(Guid id)
        {
            return Query<AccountingPriceList>().Where(x => x.Id == id).FirstOrDefault<AccountingPriceList>();
        }

        public void Save(AccountingPriceList entity)
        {
            CurrentSession.SaveOrUpdate(entity);
            CurrentSession.Flush();
        }

        public void Delete(AccountingPriceList entity)
        {
            entity.DeletionDate = DateTime.Now;
            CurrentSession.SaveOrUpdate(entity);
        }

        /// <summary>
        /// Запись изменений в БД
        /// </summary>
        public void Flush()
        {
            CurrentSession.Flush();
        }

        /// <summary>
        /// Получение списка записей с учетом фильтра
        /// </summary>
        public IList<AccountingPriceList> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<AccountingPriceList>(state, ignoreDeletedRows);
        }

        /// <summary>
        /// Получение списка записей с учетом фильтра
        /// </summary>
        public IList<AccountingPriceList> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<AccountingPriceList>(state, parameterString, ignoreDeletedRows);
        }

        #region Фильтр с лямбда выражением в качестве параметра
        
        /// <summary>
        /// Получение списка записей с учетом фильтра
        /// </summary>
        public IList<AccountingPriceList> GetFilteredList(object state, bool ignoreDeletedRows = true, Func<ISubCriteria<AccountingPriceList>, ISubCriteria<AccountingPriceList>> cond = null)
        {
            return GetBaseFilteredList<AccountingPriceList>(state, ignoreDeletedRows, cond);
        }

        /// <summary>
        /// Получение списка записей с учетом фильтра
        /// </summary>
        public IList<AccountingPriceList> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true, Func<ISubCriteria<AccountingPriceList>, ISubCriteria<AccountingPriceList>> cond = null)
        {
            return GetBaseFilteredList<AccountingPriceList>(state, parameterString, ignoreDeletedRows, cond);
        }

        #endregion

        public bool IsNumberUnique(string number)
        {
            var list = Query<AccountingPriceList>(false)
                .Where(x => x.Number == number)
                .ToList<AccountingPriceList>();
            var q = list.Where(x => (DateTime.Now.Year - x.StartDate.Year) == 0);

            return q.Count() == 0;
        }

        public string GetNextNumber()
        {
            var list = Query<AccountingPriceList>(false)
                .Select(x => x.Number, x => x.StartDate)
                .ToList(x => new { Number = (string)x[0], StartDate = (DateTime)x[1] })
                .Where(x => (DateTime.Now.Year - x.StartDate.Year) == 0);
            if (list == null || !list.Any())
            {
                return "1";
            }
            else
            {
                decimal parseVal;

                return (Convert.ToDecimal(list
                    .Max(x => Decimal.TryParse(x.Number, out parseVal) ? parseVal : 0) + 1)).ToString();
                //используется ToDecimal, т.к. поле Number может иметь до 25 знаков, в int такое число не поместится
            }
        }

        /// <summary>
        /// Получение списка товаров данного реестра цен
        /// </summary>
        /// <param name="accountingPriceListId">Код реестра цен</param>
        public IEnumerable<ArticleAccountingPrice> GetArticleAccountingPrices(Guid accountingPriceListId)
        {
            return Query<ArticleAccountingPrice>()
                .Where(x => x.AccountingPriceList.Id == accountingPriceListId)
                .ToList<ArticleAccountingPrice>();
        }

        /// <summary>
        /// Получение списка позиций РЦ по указанному МХ и товарам, которые вступили в действие после указанной даты 
        /// (т.е. по которым произошла переоценка)
        /// </summary>
        /// <param name="storageId">Код МХ</param>
        /// <param name="articleSubquery">Подзапрос для товаров</param>
        /// <param name="date">Дата для поиска РЦ</param>
        public IEnumerable<ArticleAccountingPrice> GetArticleAccountingPricesRevaluatedOnStartAfterDate(short storageId, ISubQuery articleSubquery, DateTime date)
        {
            // подзапрос для РЦ, вступивших в действие
            var accountingPriceListSubQuery = SubQuery<AccountingPriceList>()
                .Where(x => x.AcceptanceDate != null && x.StartDate > date && x.IsRevaluationOnStartCalculated == true)
                .Select(x => x.Id);

            accountingPriceListSubQuery.Restriction<Storage>(x => x.Storages)
                .Where(x => x.Id == storageId);             
            
            return Query<ArticleAccountingPrice>()
                .PropertyIn(x => x.Article, articleSubquery)
                .PropertyIn(x => x.AccountingPriceList, accountingPriceListSubQuery)
                .ToList<ArticleAccountingPrice>();
        }

        /// <summary>
        /// Получение списка позиций РЦ по указанному МХ и товарам, которые завершили действие после указанной даты 
        /// (т.е. по которым произошла переоценка)
        /// </summary>
        /// <param name="storageId">Код МХ</param>
        /// <param name="articleSubquery">Подзапрос для товаров</param>
        /// <param name="date">Дата для поиска РЦ</param>
        public IEnumerable<ArticleAccountingPrice> GetArticleAccountingPricesRevaluatedOnEndAfterDate(short storageId, ISubQuery articleSubquery, DateTime date)
        {
            // подзапрос для РЦ, вступивших в действие
            var accountingPriceListSubQuery = SubQuery<AccountingPriceList>()
                .Where(x => x.AcceptanceDate != null && x.EndDate > date && x.IsRevaluationOnEndCalculated == true)
                .Select(x => x.Id);

            accountingPriceListSubQuery.Restriction<Storage>(x => x.Storages)
                .Where(x => x.Id == storageId);

            return Query<ArticleAccountingPrice>()
                .PropertyIn(x => x.Article, articleSubquery)
                .PropertyIn(x => x.AccountingPriceList, accountingPriceListSubQuery)
                .Where(x => x.IsOverlappedOnEnd == false)
                .ToList<ArticleAccountingPrice>();
        }

        /// <summary>
        /// Получение подзапроса для списка товаров
        /// </summary>
        /// <param name="accountingPriceListId">Код РЦ</param>
        public ISubQuery GetArticlesSubquery(Guid accountingPriceListId)
        {
            return SubQuery<ArticleAccountingPrice>()
               .Where(x => x.AccountingPriceList.Id == accountingPriceListId)
               .Select(x => x.Article.Id);
        }

        /// <summary>
        /// Получение подзапроса для списка позиций РЦ
        /// </summary>
        /// <param name="accountingPriceListId">Код РЦ</param>
        public ISubQuery GetArticleAccountingPricesSubquery(Guid accountingPriceListId)
        {
            return SubQuery<ArticleAccountingPrice>()
               .Where(x => x.AccountingPriceList.Id == accountingPriceListId)
               .Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса для списка МХ
        /// </summary>
        /// <param name="accountingPriceListId">Код РЦ</param>
        public ISubQuery GetStoragesSubquery(Guid accountingPriceListId)
        {
            var storageIds = SubQuery<AccountingPriceList>()
               .Where(x => x.Id == accountingPriceListId);
            
            storageIds.Restriction<Storage>(x => x.Storages)
                .Select(x => x.Id);

            return storageIds;
        }

        /// <summary>
        /// Получение списка позиций позиций РЦ по Id
        /// </summary>
        public Dictionary<Guid, ArticleAccountingPrice> GetRows(IEnumerable<Guid> idList)
        {
            return base.GetList<Guid, ArticleAccountingPrice>(idList);
        }

        /// <summary>
        /// Получение списка вступивших в силу или завершивших действие РЦ, по которым не посчитана переоценка
        /// </summary>
        /// <param name="date">Дата выборки</param>
        public IEnumerable<AccountingPriceList> GetAccountingPriceListsWithoutCalculatedRevaluation(DateTime date)
        {
            return Query<AccountingPriceList>()
                .Where(x => x.AcceptanceDate < date && 
                    ((x.StartDate < date && x.IsRevaluationOnStartCalculated == false) ||   // не посчитана переоценка на вступление в действие
                    (x.EndDate < date && x.IsRevaluationOnEndCalculated == false))) // не посчитана переоценка на завершение действия
                .ToList<AccountingPriceList>();
        }

        /// <summary>
        /// Получение списка проведенных РЦ по списку МХ и товаров и действующих на определенный момент
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="minStartDate">Минимальная дата начала действия РЦ</param>
        /// <param name="date">Дата действия РЦ</param>
        public IEnumerable<AccountingPriceList> GetOverlappingPriceLists(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime minStartDate, DateTime date)
        {
            var query = Query<AccountingPriceList>()
                .Where(x => x.StartDate < date && x.StartDate > minStartDate && (x.EndDate > date || x.EndDate == null) && x.AcceptanceDate != null);

            query.Restriction<Storage>(x => x.Storages)
                .PropertyIn(x => x.Id, storageIdsSubQuery);

            query.Restriction<ArticleAccountingPrice>(x => x.ArticlePrices)
                .PropertyIn(x => x.Article, articleIdsSubQuery);
            
            return query.ToList<AccountingPriceList>().Distinct();
        }

        /// <summary>
        /// Получение списка необходимых РЦ при отмене проводки РЦ
        /// </summary>
        /// <param name="priceList">Отменяемый РЦ</param>
        public IEnumerable<AccountingPriceList> GetIntersectingPriceLists(AccountingPriceList priceList, DateTime currentDateTime)
        {
            // TODO: не самый оптимальный вариант реализации
            // По идее нужно для каждой пары МХ-товар найти РЦ, у которого StartDate < StartDate данного РЦ, а EndDate > EndDate данного РЦ, взять StartDate этого РЦ.
            // Затем для каждой пары МХ-товар нужно выбрать все РЦ, у которых StartDate больше определенного ранее StartDate.

            var endDate = (priceList.EndDate == null ? currentDateTime : priceList.EndDate.Value);
            
            var articleSubQuery = GetArticlesSubquery(priceList.Id);
            var storageSubQuery = GetStoragesSubquery(priceList.Id);

            var query = Query<AccountingPriceList>()
                .Where(x => x.AcceptanceDate != null && x.StartDate < endDate && (x.EndDate > priceList.StartDate || x.EndDate == null))
                .PropertyIn(x => x.Storages, storageSubQuery);

            return query.Restriction<ArticleAccountingPrice>(x => x.ArticlePrices)
                .PropertyIn(x => x.Article, articleSubQuery)
                .ToList<AccountingPriceList>();
        }
    }
}
