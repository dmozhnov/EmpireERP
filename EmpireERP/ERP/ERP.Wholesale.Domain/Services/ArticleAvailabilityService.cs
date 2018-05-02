using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    public class ArticleAvailabilityService : IArticleAvailabilityService
    {
        #region Поля
        
        private readonly IReceiptWaybillRepository receiptWaybillRepository;
        private readonly IMovementWaybillRepository movementWaybillRepository;
        private readonly IChangeOwnerWaybillRepository changeOwnerWaybillRepository;
        private readonly IWriteoffWaybillRepository writeoffWaybillRepository;
        private readonly IExpenditureWaybillRepository expenditureWaybillRepository;
        private readonly IReturnFromClientWaybillRepository returnFromClientWaybillRepository;

        private readonly IArticleRepository articleRepository;
        private readonly IStorageRepository storageRepository;

        private readonly IIncomingAcceptedArticleAvailabilityIndicatorService incomingAcceptedArticleAvailabilityIndicatorService;
        private readonly IOutgoingAcceptedFromExactArticleAvailabilityIndicatorService outgoingAcceptedFromExactArticleAvailabilityIndicatorService;
        private readonly IOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService;
        private readonly IExactArticleAvailabilityIndicatorService exactArticleAvailabilityIndicatorService;
        private readonly IIncomingWaybillRowService incomingWaybillRowService;
        
        #endregion

        #region Конструкторы
        
        public ArticleAvailabilityService(IReceiptWaybillRepository receiptWaybillRepository, 
            IMovementWaybillRepository movementWaybillRepository,
            IChangeOwnerWaybillRepository changeOwnerWaybillRepository,
            IWriteoffWaybillRepository writeoffWaybillRepository,
            IExpenditureWaybillRepository expenditureWaybillRepository,
            IReturnFromClientWaybillRepository returnFromClientWaybillRepository,
            IArticleRepository articleRepository, 
            IStorageRepository storageRepository,
            IIncomingAcceptedArticleAvailabilityIndicatorService incomingAcceptedArticleAvailabilityIndicatorService,
            IOutgoingAcceptedFromExactArticleAvailabilityIndicatorService outgoingAcceptedFromExactArticleAvailabilityIndicatorService,
            IOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService,
            IExactArticleAvailabilityIndicatorService exactArticleAvailabilityIndicatorService, 
            IIncomingWaybillRowService incomingWaybillRowService)
        {
            this.receiptWaybillRepository = receiptWaybillRepository;
            this.movementWaybillRepository = movementWaybillRepository;
            this.changeOwnerWaybillRepository = changeOwnerWaybillRepository;
            this.writeoffWaybillRepository = writeoffWaybillRepository;
            this.expenditureWaybillRepository = expenditureWaybillRepository;
            this.returnFromClientWaybillRepository = returnFromClientWaybillRepository;

            this.articleRepository = articleRepository;
            this.storageRepository = storageRepository;

            this.incomingAcceptedArticleAvailabilityIndicatorService = incomingAcceptedArticleAvailabilityIndicatorService;
            this.outgoingAcceptedFromExactArticleAvailabilityIndicatorService = outgoingAcceptedFromExactArticleAvailabilityIndicatorService;
            this.outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService = outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService;
            this.exactArticleAvailabilityIndicatorService = exactArticleAvailabilityIndicatorService;
            this.incomingWaybillRowService = incomingWaybillRowService; 
        } 
        #endregion

        #region Методы

        #region Точное наличие

        /// <summary>
        /// Получение списка показателей точного наличия по параметрам.
        /// </summary>
        /// <param name="storageIds">Список идентификаторов мест хранения.</param>
        /// <param name="articleIds">Список идентификаторов товаров.</param>
        /// <param name="date">Дата</param>        
        public IEnumerable<ExactArticleAvailabilityIndicator> GetExactArticleAvailability(IEnumerable<short> storageIds, IEnumerable<int> articleIds, DateTime date)
        {
            var storageSubQuery = storageRepository.GetListSubQuery(storageIds);
            var articleSubquery = articleRepository.GetListSubQuery(articleIds);

            return exactArticleAvailabilityIndicatorService.GetList(storageSubQuery, articleSubquery, date);
        }

        /// <summary>
        /// Получение списка показателей точного наличия по параметрам.
        /// </summary>
        /// <param name="storageSubQuery">Подзапрос на МХ</param>
        /// <param name="articleIds">Список идентификаторов товаров.</param>
        /// <param name="date">Дата</param>        
        public IEnumerable<ExactArticleAvailabilityIndicator> GetExactArticleAvailability(ISubQuery storageSubQuery, IEnumerable<int> articleIds, DateTime date)
        {
            var articleSubquery = articleRepository.GetListSubQuery(articleIds);

            return exactArticleAvailabilityIndicatorService.GetList(storageSubQuery, articleSubquery, date);
        }

        /// <summary>
        /// Получение списка показателей точного наличия по параметрам.
        /// </summary>
        /// <param name="storageIds">Список идентификаторов мест хранения.</param>
        /// <param name="articleGroupIds">Список идентификаторов групп товаров.</param>
        /// <param name="date">Дата.</param>        
        public IEnumerable<ExactArticleAvailabilityIndicator> GetExactArticleAvailability(IEnumerable<short> storageIds, IEnumerable<short> articleGroupIds, DateTime date)
        {
            var storageSubQuery = storageRepository.GetListSubQuery(storageIds);
            var articleSubquery = articleRepository.GetArticleSubQueryByArticleGroupList(articleGroupIds);

            return exactArticleAvailabilityIndicatorService.GetList(storageSubQuery, articleSubquery, date);
        }

        /// <summary>
        /// Получение списка показателей точного наличия для всех товаров на указанных складах.
        /// </summary>
        /// <param name="storageIds">Список идентификаторов мест хранения.</param>        
        /// <param name="date">Дата.</param>        
        public IEnumerable<ExactArticleAvailabilityIndicator> GetExactArticleAvailability(IEnumerable<short> storageIds, DateTime date)
        {
            var storageSubQuery = storageRepository.GetListSubQuery(storageIds);

            return exactArticleAvailabilityIndicatorService.GetList(storageSubQuery, date);
        }

        /// <summary>
        /// Получение подзапроса для получения индикаторов точного наличия по параметрам.
        /// </summary>
        /// <param name="date">Дата.</param>
        /// <param name="storageIds">Список идентификаторов мест хранения.</param>        
        public ISubQuery GetArticleSubqueryByExactArticleAvailability(DateTime date, IEnumerable<short> storageIds)
        {
            return receiptWaybillRepository.SubQuery<ExactArticleAvailabilityIndicator>()
                .OneOf(x => x.StorageId, storageIds)
                .Where(x => x.StartDate < date && (x.EndDate > date || x.EndDate == null))
                .Where(x => x.Count != 0).Select(x => x.ArticleId);
        }

        /// <summary>
        /// Получение списка показателей точного наличия по параметрам для всех товаров на указанном складе.
        /// </summary>
        /// <param name="storageIds">Список идентификаторов мест хранения.</param> 
        /// <param name="articleId">Идентификаторо товара.</param>
        /// <param name="date">Дата.</param>   
        public IEnumerable<ExactArticleAvailabilityIndicator> GetExactArticleAvailability(IEnumerable<short> storageIds, int articleId, DateTime date)
        {
            //получение подзапроса для мест хранения
            var storageSubQuery = storageRepository.GetListSubQuery(storageIds);
            var articleSubquery = articleRepository.GetArticleSubQuery(articleId);

            return exactArticleAvailabilityIndicatorService.GetList(storageSubQuery, articleSubquery, date);
        }

        #endregion

        #region Расширенное наличие

        /// <summary>
        /// Получение списка краткой информации о расширенном наличии по параметрам. 
        /// </summary>
        /// <param name="articleId">Товар.</param>
        /// <param name="storageIds">Список идентификаторов мест хранения.</param>
        /// <param name="date">Дата.</param>
        /// <returns>Списка краткой информации по расширенному наличию. </returns>
        public IEnumerable<ArticleBatchAvailabilityShortInfo> GetExtendedArticleAvailability(int articleId, IEnumerable<short> storageIds, DateTime date)
        {
            return GetExtendedArticleAvailability(new List<int>() { articleId }, storageIds, date);
        }

        /// <summary>
        /// Получение списка краткой информации о расширенном наличии по параметрам.
        /// </summary>
        /// <param name="accountingPriceList">Реестр цен, по товарам и местам хранения которого будет получено наличие.</param>
        /// <param name="date">Дата.</param>
        /// <returns>Список краткой информации по расширенному наличию по параметрам.</returns>
        public IEnumerable<ArticleBatchAvailabilityShortInfo> GetExtendedArticleAvailability(AccountingPriceList accountingPriceList, DateTime date)
        {
            //подзапрос для складов по РЦ
            var storageSubQuery = receiptWaybillRepository.SubQuery<AccountingPriceList>()
                .Where(x => x.Id == accountingPriceList.Id);
            storageSubQuery.Restriction<Storage>(x => x.Storages)
                .Select(x => x.Id);

            //подзапрос для товаров по РЦ
            var articleSubquery = receiptWaybillRepository.SubQuery<ArticleAccountingPrice>()
                .Where(x => x.AccountingPriceList.Id == accountingPriceList.Id)
                .Select(x => x.Article.Id);

            return GetExtendedArticleAvailability(articleSubquery, storageSubQuery, date);
        }

        /// <summary>
        /// Получение списка краткой информации о расширенном наличии по параметрам.
        /// </summary>
        /// <param name="articleIds">Список идентификаторов товаров.</param>
        /// <param name="storageIds">Список идентификаторов мест хранения.</param>
        /// <param name="date">Дата</param>        
        public IEnumerable<ArticleBatchAvailabilityShortInfo> GetExtendedArticleAvailability(IEnumerable<int> articleIds, IEnumerable<short> storageIds, DateTime date)
        {
            var articleSubquery = articleRepository.GetListSubQuery(articleIds);
            var storageSubQuery = storageRepository.GetListSubQuery(storageIds);
            
            return GetExtendedArticleAvailability(articleSubquery, storageSubQuery, date);
        }

        /// <summary>
        /// Есть ли расширенное наличие (большее нуля) для указанных параметров.
        /// </summary>
        /// <param name="storageId">Идентификатор места хранения.</param>
        /// <param name="accountOrganizationId">Идентификатор собственной организации.</param>
        /// <param name="date">Дата.</param>        
        public bool IsExtendedArticleAvailability(short storageId, int accountOrganizationId, DateTime date)
        {
                   // проверяем точное наличие
            return exactArticleAvailabilityIndicatorService.IsArticleAvailability(storageId, accountOrganizationId, date) ||
                   // проверяем входящеее проведенное наличие
                   incomingAcceptedArticleAvailabilityIndicatorService.IsArticleAvailability(storageId, accountOrganizationId, date) ||
                   // проверяем исходящее проведенное из точного наличие
                   outgoingAcceptedFromExactArticleAvailabilityIndicatorService.IsArticleAvailability(storageId, accountOrganizationId, date) ||
                   // проверяем исходящее проведенное из входящего проведенного наличие
                   outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.IsArticleAvailability(storageId, accountOrganizationId, date);
        }

        /// <summary>
        /// Получение краткой информации о расширенном наличии по параметрам.
        /// </summary>
        /// <param name="storageId">Идентификатор места хранения.</param>
        /// <param name="accountOrganizationId">Идентификатор собственной организации.</param>
        /// <param name="date">Дата.</param>        
        public IEnumerable<ArticleBatchAvailabilityShortInfo> GetExtendedArticleAvailability(short storageId, int accountOrganizationId, DateTime date)
        {
            // получаем индикаторы точного наличия
            var exactAvailabilityList = exactArticleAvailabilityIndicatorService.GetList(storageId, accountOrganizationId, date);
            // получаем индикаторы входящего проведенного наличия
            var incomingAcceptedAvailabilityList = incomingAcceptedArticleAvailabilityIndicatorService.GetList(storageId, accountOrganizationId, date);
            // получаем индикаторы исходящего проведенного из точного наличия
            var outgoingAcceptedFromExactAvailabilityList = outgoingAcceptedFromExactArticleAvailabilityIndicatorService.GetList(storageId, accountOrganizationId, date);
            // получаем индикаторы исходящего проведенного из входящего проведенного наличия
            var outgoingAcceptedFromIncomingAcceptedAvailabilityList = outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.GetList(storageId, accountOrganizationId, date);

            return GetAvailabilityIndicatorsSumShortInfo(exactAvailabilityList, incomingAcceptedAvailabilityList, outgoingAcceptedFromExactAvailabilityList, outgoingAcceptedFromIncomingAcceptedAvailabilityList);
        }
        
        /// <summary>
        /// Получение полной информации о расширенном наличии по параметрам.
        /// </summary>
        /// <param name="articleBatch">Партия товара, для товаров которой будет получено наличие.</param>
        /// <param name="storage">Склад.</param>
        /// <param name="accountOrganization">Собственная организация.</param>
        /// <param name="date">Дата.</param>        
        public ArticleBatchAvailabilityExtendedInfo GetExtendedArticleBatchAvailability(ReceiptWaybillRow articleBatch, Storage storage,
            AccountOrganization accountOrganization, DateTime date)
        {
            // получаем индикаторы точного наличия
            var exactAvailabilityIndicator = exactArticleAvailabilityIndicatorService.GetList(articleBatch.Id, storage.Id, accountOrganization.Id, date);
            // получаем индикаторы входящего проведенного наличия
            var incomingAcceptedAvailabilityIndicator = incomingAcceptedArticleAvailabilityIndicatorService.GetList(articleBatch.Id, storage.Id, accountOrganization.Id, date);
            // получаем индикаторы исходящего проведенного из точного наличия
            var outgoingAcceptedFromExactAvailabilityIndicator = outgoingAcceptedFromExactArticleAvailabilityIndicatorService.GetList(articleBatch.Id, storage.Id, accountOrganization.Id, date);
            // получаем индикаторы исходящего проведенного из входящего проведенного наличия
            var outgoingAcceptedFromIncomingAcceptedAvailabilityIndicator = outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.GetList(articleBatch.Id, storage.Id, accountOrganization.Id, date);

            return new ArticleBatchAvailabilityExtendedInfo(articleBatch, storage.Id, accountOrganization.Id,
                exactAvailabilityIndicator == null ? 0 : exactAvailabilityIndicator.Count,
                incomingAcceptedAvailabilityIndicator == null ? 0 : incomingAcceptedAvailabilityIndicator.Count,
                // резерв
                (outgoingAcceptedFromExactAvailabilityIndicator == null ? 0 : outgoingAcceptedFromExactAvailabilityIndicator.Count),
                (outgoingAcceptedFromIncomingAcceptedAvailabilityIndicator == null ? 0 : outgoingAcceptedFromIncomingAcceptedAvailabilityIndicator.Count)
            );
        }

        /// <summary>
        /// Получение списка полной информации о расширенном наличии по параметрам.
        /// </summary>
        /// <param name="article">Товар.</param>
        /// <param name="storage">Место хранения.</param>
        /// <param name="accountOrganization">Собственная организация.</param>
        /// <param name="date">Дата.</param>        
        public IEnumerable<ArticleBatchAvailabilityExtendedInfo> GetExtendedArticleBatchAvailability(Article article, Storage storage, 
            AccountOrganization accountOrganization, DateTime date)
        {
            // получаем индикаторы точного наличия
            var exactAvailabilityList = 
                exactArticleAvailabilityIndicatorService.GetList(article.Id, storage.Id, accountOrganization.Id, date);
            // получаем индикаторы входящего проведенного наличия
            var incomingAcceptedAvailabilityList = 
                incomingAcceptedArticleAvailabilityIndicatorService.GetList(article.Id, storage.Id, accountOrganization.Id, date);
            // получаем индикаторы исходящего проведенного из точного наличия
            var outgoingAcceptedFromExcatAvailabilityList = 
                outgoingAcceptedFromExactArticleAvailabilityIndicatorService.GetList(article.Id, storage.Id, accountOrganization.Id, date);
            // получаем индикаторы исходящего проведенного из входящего проведенного наличия
            var outgoingAcceptedFromIncomingAcceptedAvailabilityList = 
                outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.GetList(article.Id, storage.Id, accountOrganization.Id, date);

            return GetAvailabilityIndicatorsSumExtendedInfo(exactAvailabilityList, incomingAcceptedAvailabilityList, 
                outgoingAcceptedFromExcatAvailabilityList, outgoingAcceptedFromIncomingAcceptedAvailabilityList);
        }

        /// <summary>
        /// Получение списка полной информации о расширенном наличии по параметрам.
        /// </summary>
        /// <param name="articleIds">Список идентификаторов товаров.</param>
        /// <param name="storage">Место хранения.</param>
        /// <param name="accountOrganization">Собственная организация.</param>
        /// <param name="date">Дата.</param>        
        public IEnumerable<ArticleBatchAvailabilityExtendedInfo> GetExtendedArticleBatchAvailability(IEnumerable<int> articleIds, Storage storage,
            AccountOrganization accountOrganization, DateTime date)
        {
            // получаем индикаторы точного наличия
            var exactAvailabilityList = 
                exactArticleAvailabilityIndicatorService.GetList(articleIds, storage.Id, accountOrganization.Id, date);
            // получаем индикаторы входящего проведенного наличия
            var incomingAcceptedAvailabilityList = 
                incomingAcceptedArticleAvailabilityIndicatorService.GetList(articleIds, storage.Id, accountOrganization.Id, date);
            // получаем индикаторы исходящего проведенного из точного наличия
            var outgoingAcceptedFromExcatAvailabilityList = 
                outgoingAcceptedFromExactArticleAvailabilityIndicatorService.GetList(articleIds, storage.Id, accountOrganization.Id, date);
            // получаем индикаторы исходящего проведенного из входящего проведенного наличия
            var outgoingAcceptedFromIncomingAcceptedAvailabilityList = 
                outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.GetList(articleIds, storage.Id, accountOrganization.Id, date);

            return GetAvailabilityIndicatorsSumExtendedInfo(exactAvailabilityList, incomingAcceptedAvailabilityList,
                outgoingAcceptedFromExcatAvailabilityList, outgoingAcceptedFromIncomingAcceptedAvailabilityList);
        }

        /// <summary>
        /// Получение списка полной информации о расширенном наличии по параметрам.
        /// </summary>
        /// <param name="articleGroupIds">Список идентификаторов групп товаров. Наличие будет получено для всех товаров, относящихся к этим группам.</param>
        /// <param name="storageIds">Список идентификаторов мест хранения.</param>
        /// <param name="date">Дата.</param>
        public IEnumerable<ArticleBatchAvailabilityExtendedInfo> GetExtendedArticleBatchAvailability(IEnumerable<short> articleGroupIds, IEnumerable<short> storageIds, DateTime date)
        {
            return GetExtendedArticleBatchAvailabilityLocal(articleRepository.GetArticleSubQueryByArticleGroupList(articleGroupIds), storageIds, date);
        }

        /// <summary>
        /// Получение списка полной информации о расширенном наличии по параметрам.
        /// </summary>
        /// <param name="articleGroupIds">Список идентификаторов товаров.</param>
        /// <param name="storageIds">Список идентификаторов мест хранения.</param>
        /// <param name="date">Дата.</param>
        public IEnumerable<ArticleBatchAvailabilityExtendedInfo> GetExtendedArticleBatchAvailability(IEnumerable<int> articleIds, IEnumerable<short> storageIds, DateTime date)
        {
            return GetExtendedArticleBatchAvailabilityLocal(articleRepository.GetListSubQuery(articleIds), storageIds, date);
        }

        /// <summary>
        /// Получение списка полной информации о расширенном наличии
        /// </summary>
        /// <param name="articleSubquery">Подзапрос для товаров.</param>
        /// <param name="storageIds">Список идентификаторов мест хранения.</param>
        /// <param name="date">Дата.</param>
        private IEnumerable<ArticleBatchAvailabilityExtendedInfo> GetExtendedArticleBatchAvailabilityLocal(ISubQuery articleSubquery, IEnumerable<short> storageIds, DateTime date)
        {
            var storageSubquery = storageRepository.GetListSubQuery(storageIds);

            // получаем индикаторы точного наличия
            var exactAvailabilityList =
                exactArticleAvailabilityIndicatorService.GetList(storageSubquery, articleSubquery, date);
            // получаем индикаторы входящего проведенного наличия
            var incomingAcceptedAvailabilityList =
                incomingAcceptedArticleAvailabilityIndicatorService.GetList(storageSubquery, articleSubquery, date);
            // получаем индикаторы исходящего проведенного из точного наличия
            var outgoingAcceptedFromExcatAvailabilityList =
                outgoingAcceptedFromExactArticleAvailabilityIndicatorService.GetList(storageSubquery, articleSubquery, date);
            // получаем индикаторы исходящего проведенного из входящего проведенного наличия
            var outgoingAcceptedFromIncomingAcceptedAvailabilityList =
                outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.GetList(storageSubquery, articleSubquery, date);

            return GetAvailabilityIndicatorsSumExtendedInfo(exactAvailabilityList, incomingAcceptedAvailabilityList,
                outgoingAcceptedFromExcatAvailabilityList, outgoingAcceptedFromIncomingAcceptedAvailabilityList);
        }
        

        /// <summary>
        /// Получение списка краткой информации о расширенном наличии по параметрам.
        /// </summary>
        /// <param name="articleSubquery">Список идентификаторов товаров в виде подзапроса.</param>
        /// <param name="storageSubQuery">Список идентификаторов мест хранения в виде подзапроса.</param>
        /// <param name="date">Дата.</param>        
        private IEnumerable<ArticleBatchAvailabilityShortInfo> GetExtendedArticleAvailability(ISubQuery articleSubquery, ISubQuery storageSubQuery, DateTime date)
        {
            // получаем индикаторы точного наличия
            var exactAvailabilityList = 
                exactArticleAvailabilityIndicatorService.GetList(storageSubQuery, articleSubquery, date);
            // получаем индикаторы входящего проведенного наличия
            var incomingAcceptedAvailabilityList = 
                incomingAcceptedArticleAvailabilityIndicatorService.GetList(storageSubQuery, articleSubquery, date);
            // получаем индикаторы исходящего проведенного из точного наличия
            var outgoingAcceptedFromExactAvailabilityList = 
                outgoingAcceptedFromExactArticleAvailabilityIndicatorService.GetList(storageSubQuery, articleSubquery, date);
            // получаем индикаторы исходящего проведенного из входящего проведенного наличия
            var outgoingAcceptedFromIncomingAcceptedAvailabilityList = 
                outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.GetList(storageSubQuery, articleSubquery, date);

            return GetAvailabilityIndicatorsSumShortInfo(exactAvailabilityList, incomingAcceptedAvailabilityList, 
                outgoingAcceptedFromExactAvailabilityList, outgoingAcceptedFromIncomingAcceptedAvailabilityList);
        }
        
        /// <summary>
        /// Суммирование списков показателей по формуле: точное наличие + проведенное входящее - проведенное исходящее.
        /// </summary>
        /// <param name="exactAvailabilityList">Список показателей точного наличия.</param>
        /// <param name="incomingAcceptedAvailabilityList">Список показателей входящего проведенного наличия.</param>
        /// <param name="outgoingAcceptedFromExactAvailabilityList">Список показателей исходящего проведенного из точного наличия.</param>
        /// <param name="outgoingAcceptedFromIncomingAcceptedAvailabilityList">Список показателей исходящего проведенного из входящего проведенного наличия.</param>        
        private List<ArticleBatchAvailabilityShortInfo> GetAvailabilityIndicatorsSumShortInfo(IEnumerable<ExactArticleAvailabilityIndicator> exactAvailabilityList, 
            IEnumerable<IncomingAcceptedArticleAvailabilityIndicator> incomingAcceptedAvailabilityList, 
            IEnumerable<OutgoingAcceptedFromExactArticleAvailabilityIndicator> outgoingAcceptedFromExactAvailabilityList,
            IEnumerable<OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator> outgoingAcceptedFromIncomingAcceptedAvailabilityList)
        {
            var result = new List<ArticleBatchAvailabilityShortInfo>();

            // добавляем в результат точное наличие
            foreach (var item in exactAvailabilityList)
            {
                result.Add(new ArticleBatchAvailabilityShortInfo(item.BatchId, item.ArticleId, item.StorageId, item.AccountOrganizationId, item.Count));
            }

            // добавляем в результат входящее проведенное наличие
            foreach (var item in incomingAcceptedAvailabilityList)
            {
                result = AddArticleBatchAvailabilityShortInfo(result, item.BatchId, item.ArticleId, item.StorageId, item.AccountOrganizationId, item.Count);
            }

            // вычитаем из результата исходящее проведенное из точного наличие
            foreach (var item in outgoingAcceptedFromExactAvailabilityList)
            {
                result = AddArticleBatchAvailabilityShortInfo(result, item.BatchId, item.ArticleId, item.StorageId, item.AccountOrganizationId, -item.Count);
            }

            // вычитаем из результата исходящее проведенное из входящего проведенного наличие
            foreach (var item in outgoingAcceptedFromIncomingAcceptedAvailabilityList)
            {
                result = AddArticleBatchAvailabilityShortInfo(result, item.BatchId, item.ArticleId, item.StorageId, item.AccountOrganizationId, -item.Count);
            }

            return result;
        }

        /// <summary>
        /// Суммирование списков показателей по формуле: точное наличие + проведенное входящее - проведенное исходящее.
        /// </summary>
        /// <param name="exactAvailabilityList">Список показателей точного наличия.</param>
        /// <param name="incomingAcceptedAvailabilityList">Список показателей входящего проведенного наличия.</param>
        /// <param name="outgoingAcceptedFromExactAvailabilityList">Список показателей исходящего проведенного из точного наличия.</param>
        /// <param name="outgoingAcceptedFromIncomingAcceptedAvailabilityList">Список показателей исходящего проведенного из входящего проведенного наличия.</param>
        private List<ArticleBatchAvailabilityExtendedInfo> GetAvailabilityIndicatorsSumExtendedInfo(IEnumerable<ExactArticleAvailabilityIndicator> exactAvailabilityList,
            IEnumerable<IncomingAcceptedArticleAvailabilityIndicator> incomingAcceptedAvailabilityList,
            IEnumerable<OutgoingAcceptedFromExactArticleAvailabilityIndicator> outgoingAcceptedFromExactAvailabilityList,
            IEnumerable<OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator> outgoingAcceptedFromIncomingAcceptedAvailabilityList)
        {
            var result = new List<ArticleBatchAvailabilityExtendedInfo>();

            // получаем информацию обо всех партиях товара по Id
            var batchIdList = exactAvailabilityList.Select(x => x.BatchId)
                .Concat(incomingAcceptedAvailabilityList.Select(y => y.BatchId))
                .Concat(outgoingAcceptedFromExactAvailabilityList.Select(y => y.BatchId))
                .Concat(outgoingAcceptedFromIncomingAcceptedAvailabilityList.Select(y => y.BatchId))
                .Distinct();

            var batchList = receiptWaybillRepository.GetRows(batchIdList);
            ValidationUtils.Assert(batchIdList.Count() == batchList.Count(), "Одна из партий не найдена. Возможно, она была удалена.");

            // показатели точного наличия
            foreach (var item in exactAvailabilityList)
            {
                result.Add(new ArticleBatchAvailabilityExtendedInfo(batchList[item.BatchId], 
                    item.StorageId, item.AccountOrganizationId, item.Count, 0, 0, 0));
            }

            // показатели входящего проведенного наличия
            foreach (var item in incomingAcceptedAvailabilityList)
            {
                result = AddArticleBatchAvailabilityExtendedInfo(result, batchList[item.BatchId],
                    item.StorageId, item.AccountOrganizationId, 0, item.Count, 0, 0);
            }

            // показатели исходящего проведенного из точного наличия
            foreach (var item in outgoingAcceptedFromExactAvailabilityList)
            {
                result = AddArticleBatchAvailabilityExtendedInfo(result, batchList[item.BatchId], 
                    item.StorageId, item.AccountOrganizationId, 0, 0, item.Count, 0);
            }

            // показатели исходящего проведенного из входящего проведенного наличия
            foreach (var item in outgoingAcceptedFromIncomingAcceptedAvailabilityList)
            {
                result = AddArticleBatchAvailabilityExtendedInfo(result, batchList[item.BatchId],
                    item.StorageId, item.AccountOrganizationId, 0, 0, 0, item.Count);
            }

            return result;
        }

        /// <summary>
        /// Добавление нового значения в коллекцию краткой информации о наличии по партии или обновление существующего.
        /// </summary>
        /// <param name="source">Коллекция краткой информации, куда надо добавить новое значение.</param>
        /// <param name="batchId">Идентификатор партии.</param>
        /// <param name="articleId">Идентификатор товара.</param>
        /// <param name="storageId">Идентификатор места хранения.</param>
        /// <param name="accountOrganizationId">Идентификатор собственной организации.</param>
        /// <param name="count">Изменение количества.</param>
        private List<ArticleBatchAvailabilityShortInfo> AddArticleBatchAvailabilityShortInfo(IEnumerable<ArticleBatchAvailabilityShortInfo> source,
            Guid batchId, int articleId, short storageId, int accountOrganizationId, decimal count)
        {
            var oldElement = source.FirstOrDefault(x => x.BatchId == batchId && x.StorageId == storageId && 
                x.AccountOrganizationId == accountOrganizationId);

            var result = source.ToList();

            // если элемента с данными параметрами нет - добавляем его
            if (oldElement == null)
            {
                result.Add(new ArticleBatchAvailabilityShortInfo(batchId, articleId, storageId, accountOrganizationId, count));
            }
            // иначе меняем параметры существующего
            else
            {
                oldElement.Count += count;
            }

            return result;
        }

        /// <summary>
        /// Добавление нового значения в коллекцию подробной информации о наличии по партии или обновление существующего.
        /// </summary>
        /// <param name="source">Коллекция полной информации о наличии, куда будет добавлен новый элемент.</param>
        /// <param name="batch">Партия товара.</param>
        /// <param name="storageId">Идентификатор места хранения.</param>
        /// <param name="accountOrganizationId">Идентификатор собственной организации.</param>
        /// <param name="availableInStorageCount">Количество товара, доступное на складе.</param>
        /// <param name="pendingCount">Ожидаемое количество товара.</param>
        /// <param name="reservedFromExactAvailabilityCount">Количество товара, зарезервированное из точного наличия.</param>
        /// <param name="reservedFromIncomingAcceptedAvailabilityCount">Количество товара, зарезервированное из проведенного входящего наличия.</param>        
        private static List<ArticleBatchAvailabilityExtendedInfo> AddArticleBatchAvailabilityExtendedInfo(IEnumerable<ArticleBatchAvailabilityExtendedInfo> source,
            ReceiptWaybillRow batch, short storageId, int accountOrganizationId, decimal availableInStorageCount, decimal pendingCount, 
            decimal reservedFromExactAvailabilityCount, decimal reservedFromIncomingAcceptedAvailabilityCount)
        {
            var oldElement = source.FirstOrDefault(x => x.ArticleBatchId == batch.Id && x.StorageId == storageId && x.AccountOrganizationId == accountOrganizationId);

            var result = source.ToList();

            // если элемента с данными параметрами нет - добавляем его
            if (oldElement == null)
            {
                result.Add(new ArticleBatchAvailabilityExtendedInfo(batch, storageId, accountOrganizationId, availableInStorageCount, pendingCount, 
                    reservedFromExactAvailabilityCount, reservedFromIncomingAcceptedAvailabilityCount));
            }
            // иначе меняем параметры существующего
            else
            {
                oldElement.AvailableInStorageCount += availableInStorageCount;
                oldElement.PendingCount += pendingCount;
                oldElement.ReservedFromExactAvailabilityCount += reservedFromExactAvailabilityCount;
                oldElement.ReservedFromIncomingAcceptedAvailabilityCount += reservedFromIncomingAcceptedAvailabilityCount; 
            }

            return result;
        }

        #endregion

        #region Доступное для товародвижения наличие

        /// <summary>
        /// Получение списка входящих позиций по товару, МХ и организации, из которых доступно резервирование товаров.
        /// </summary>
        /// <param name="article">Товар.</param>
        /// <param name="storage">Место хранения.</param>
        /// <param name="organization">Собственная организация.</param> 
        /// <param name="batch">Опциональный параметр для фильтра по партии.</param>
        /// <param name="waybillType">Опциональный параметр для фильтра по типу накладной.</param>
        /// <param name="startDate">Опциональный параметр для фильтра по дате (начало интервала).</param>
        /// <param name="endDate">Опциональный параметр для фильтра по дате (конец интервала).</param>
        /// <param name="number">Опциональный параметр для фильтра по номеру накладной.</param>
        /// <returns>Отфильтрованный список позиций накладных с товаром article на складе storage от собственной организации organization.</returns>      
        public IEnumerable<IncomingWaybillRow> GetAvailableToReserveWaybillRows(Article article, AccountOrganization organization, Storage storage,
            ReceiptWaybillRow batch = null, Guid? waybillRowId = null, WaybillType? waybillType = null, DateTime? startDate = null, DateTime? endDate = null, string number = null)
        {
            // получаем коллекцию позиций входящих накладных
            var incomingWaybillRows = incomingWaybillRowService.GetAvailableToReserveList(article, storage, organization, batch, waybillRowId, waybillType, startDate, endDate, number);

            return incomingWaybillRows;
        }
        
        /// <summary>
        /// Получение по списку партий, МХ, организации на указанную дату кол-ва, доступного для резервирования из точного наличия.
        /// </summary>
        /// <param name="articleBatchIdSubQuery">Список идентификаторов партий товаров в виде подзапроса.</param>
        /// <param name="storageId">Идентификатор места хранения.</param>
        /// <param name="accountOrganizationId">Идентификатор собственной организации.</param>
        /// <param name="date">Дата.</param>        
        public IEnumerable<ArticleBatchAvailabilityShortInfo> GetAvailableToReserveFromExactArticleAvailability(ISubQuery articleBatchIdSubQuery, short storageId, int accountOrganizationId, DateTime date)
        {
            // получаем индикаторы точного наличия
            var exactAvailabilityList = exactArticleAvailabilityIndicatorService.GetList(articleBatchIdSubQuery, storageId, accountOrganizationId, date);
            // получаем индикаторы исходящего проведенного из точного наличия
            var outgoingAcceptedFromExactAvailabilityList = outgoingAcceptedFromExactArticleAvailabilityIndicatorService.GetList(storageId, accountOrganizationId, date);

            var result = new List<ArticleBatchAvailabilityShortInfo>();

            // добавляем в результат точное наличие
            foreach (var item in exactAvailabilityList)
            {
                result.Add(new ArticleBatchAvailabilityShortInfo(item.BatchId, item.ArticleId, item.StorageId, item.AccountOrganizationId, item.Count));
            }

            // вычитаем из результата исходящее проведенное из точного наличие
            foreach (var item in outgoingAcceptedFromExactAvailabilityList)
            {
                result = AddArticleBatchAvailabilityShortInfo(result, item.BatchId, item.ArticleId, item.StorageId, item.AccountOrganizationId, -item.Count);
            }

            return result;
        } 
        
        #endregion

        #region Обновление показателей наличия

        #region Приходная накладная
        
        #region Проводка / отмена проводки

        /// <summary>
        /// Приходная накладная проведена
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        public void ReceiptWaybillAccepted(ReceiptWaybill waybill)
        {
            // увеличение показателя входящего проведенного наличия
            var recipientIncomingAcceptedArticleAvailabilityIndicators = new List<IncomingAcceptedArticleAvailabilityIndicator>();

            foreach (var row in waybill.Rows)
            {
                var recipientIndicator = new IncomingAcceptedArticleAvailabilityIndicator(waybill.AcceptanceDate.Value, waybill.ReceiptStorage.Id,
                    waybill.AccountOrganization.Id, row.Article.Id, row.Id, row.PurchaseCost, row.PendingCount);

                recipientIncomingAcceptedArticleAvailabilityIndicators.Add(recipientIndicator);
            }
            incomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id, receiptWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientIncomingAcceptedArticleAvailabilityIndicators);
        }

        /// <summary>
        /// Проводка приходной накладной отменена
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        public void ReceiptWaybillAcceptanceCanceled(ReceiptWaybill waybill)
        {
            // уменьшение показателя входящего проведенного наличия
            var recipientIncomingAcceptedArticleAvailabilityIndicators = new List<IncomingAcceptedArticleAvailabilityIndicator>();
            var acceptanceDate = waybill.AcceptanceDate.Value;

            foreach (var row in waybill.Rows)
            {
                var recipientIndicator = new IncomingAcceptedArticleAvailabilityIndicator(acceptanceDate, waybill.ReceiptStorage.Id,
                    waybill.AccountOrganization.Id, row.Article.Id, row.Id, row.PurchaseCost, -row.PendingCount);

                recipientIncomingAcceptedArticleAvailabilityIndicators.Add(recipientIndicator);
            }
            incomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id, receiptWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientIncomingAcceptedArticleAvailabilityIndicators);
        }

        #endregion

        #region Приемка / отмена приемки

        /// <summary>
        /// Приемка приходной накладной
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        /// <param name="outgoingWaybillRowDict">словарь исходящих позиций</param>
        public void ReceiptWaybillReceipted(ReceiptWaybill waybill, DynamicDictionary<Guid, Dictionary<OutgoingWaybillRow, decimal>> outgoingWaybillRowDict)
        {
            // уменьшаем входящее проведенное наличие для всех позиций, кроме добавленных при приемке
            var recipientIncomingAcceptedArticleAvailabilityIndicators = new List<IncomingAcceptedArticleAvailabilityIndicator>();

            foreach (var row in waybill.Rows.Where(x => !x.IsAddedOnReceipt))
            {
                var recipientIndicator = new IncomingAcceptedArticleAvailabilityIndicator(waybill.ReceiptDate.Value, waybill.ReceiptStorage.Id,
                    waybill.AccountOrganization.Id, row.Article.Id, row.Id, row.PurchaseCost, -row.PendingCount);

                recipientIncomingAcceptedArticleAvailabilityIndicators.Add(recipientIndicator);
            }

            incomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id, receiptWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientIncomingAcceptedArticleAvailabilityIndicators);

            // увеличиваем точное наличие для позиций без расхождений
            var exactArticleAvailabilityIndicators = new List<ExactArticleAvailabilityIndicator>();

            var recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();
            var recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators = new List<OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator>();

            foreach (var row in waybill.Rows.Where(x => x.AreDivergencesAfterReceipt == false && x.PendingCount > 0M))
            {
                var indicator = new ExactArticleAvailabilityIndicator(waybill.ReceiptDate.Value, waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id, 
                    row.Article.Id, row.Id, row.PurchaseCost, row.CurrentCount);

                exactArticleAvailabilityIndicators.Add(indicator);

                if (row.AcceptedCount > 0)
                {
                    var outgoingWaybillRows = outgoingWaybillRowDict[row.Id].Where(x => x.Key.AcceptanceDate != null);    // исходящие позиции
                    // Цикл просмотра всех исходящих позиций
                    foreach (var outgoingWaybillRow in outgoingWaybillRows)
                    {
                        var indDate = outgoingWaybillRow.Key.AcceptanceDate > waybill.ReceiptDate.Value ? outgoingWaybillRow.Key.AcceptanceDate.Value : waybill.ReceiptDate.Value;

                        // уменьшаем исходящее проведенное (из входящего проведенного) наличие получателя
                        var recipientOutgoingAcceptedFromIncomingAcceptedIndicator = new OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator(indDate,
                            waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id, row.Article.Id, row.Id, row.PurchaseCost, -outgoingWaybillRow.Value);

                        recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators.Add(recipientOutgoingAcceptedFromIncomingAcceptedIndicator);

                        // увеличиваем исходящее проведенное (из точного) наличие получателя
                        var recipientOutgoingAcceptedFromExactIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(indDate, waybill.ReceiptStorage.Id,
                            waybill.AccountOrganization.Id, row.Article.Id, row.Id, row.PurchaseCost, outgoingWaybillRow.Value);

                        recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(recipientOutgoingAcceptedFromExactIndicator);
                    }
                }
            }
            exactArticleAvailabilityIndicatorService.Update(waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id, 
                receiptWaybillRepository.GetArticleBatchesSubquery(waybill.Id), exactArticleAvailabilityIndicators);

            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id, 
                receiptWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators);

            outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id, 
                receiptWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators);
        }

        /// <summary>
        /// Приемка приходной накладной отменена
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        /// <param name="outgoingWaybillRowDict">словарь исходящих позиций</param>
        public void ReceiptWaybillReceiptCanceled(ReceiptWaybill waybill, DynamicDictionary<Guid, Dictionary<OutgoingWaybillRow, decimal>> outgoingWaybillRowDict)
        {
            // увеличиваем входящее проведенное наличие для всех позиций
            var recipientIncomingAcceptedArticleAvailabilityIndicators = new List<IncomingAcceptedArticleAvailabilityIndicator>();

            foreach (var row in waybill.Rows)
            {
                var recipientIndicator = new IncomingAcceptedArticleAvailabilityIndicator(waybill.ReceiptDate.Value, waybill.ReceiptStorage.Id,
                    waybill.AccountOrganization.Id, row.Article.Id, row.Id, row.PurchaseCost, row.PendingCount);

                recipientIncomingAcceptedArticleAvailabilityIndicators.Add(recipientIndicator);
            }

            incomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id, receiptWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientIncomingAcceptedArticleAvailabilityIndicators);

            // уменьшаем показатели точного наличия для позиций без расхождений
            var exactArticleAvailabilityIndicators = new List<ExactArticleAvailabilityIndicator>();

            var recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();
            var recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators = new List<OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator>();

            foreach (var row in waybill.Rows.Where(x => x.AreDivergencesAfterReceipt == false && x.PendingCount > 0M))
            {
                var indicator = new ExactArticleAvailabilityIndicator(waybill.ReceiptDate.Value, waybill.ReceiptStorage.Id,
                    waybill.AccountOrganization.Id, row.Article.Id, row.Id, row.PurchaseCost, -row.CurrentCount);

                exactArticleAvailabilityIndicators.Add(indicator);

                if (row.AcceptedCount > 0)
                {
                    // Цикл по исходящим позициям из позиции прихода
                    foreach (var outgoingRow in outgoingWaybillRowDict[row.Id].Where(x => x.Key.AcceptanceDate != null))
                    {
                        DateTime acceptanceDate = outgoingRow.Key.AcceptanceDate.Value;
                        // Получае наибольшую дату
                        var indDate = waybill.ReceiptDate.Value > acceptanceDate ? waybill.ReceiptDate.Value : acceptanceDate;

                        // увеличиваем исходящее проведенное (из входящего проведенного) наличие получателя
                        var recipientOutgoingAcceptedFromIncomingAcceptedIndicator = new OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator(indDate, waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id,
                            row.Article.Id, row.Id, row.PurchaseCost, outgoingRow.Value);

                        recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators.Add(recipientOutgoingAcceptedFromIncomingAcceptedIndicator);

                        // уменьшаем исходящее проведенное (из точного) наличие получателя
                        var recipientOutgoingAcceptedFromExactIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(indDate, waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id,
                                        row.Article.Id, row.Id, row.PurchaseCost, -outgoingRow.Value);

                        recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(recipientOutgoingAcceptedFromExactIndicator);
                    }
                }
            }

            exactArticleAvailabilityIndicatorService.Update(waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id, receiptWaybillRepository.GetArticleBatchesSubquery(waybill.Id), exactArticleAvailabilityIndicators);

            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id, receiptWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators);

            outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id, receiptWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators);
        }

        #endregion

        #region Согласование / отмена согласования

        /// <summary>
        /// Приходная накладная согласована
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        /// <param name="outgoingWaybillRowDict">словарь исходящих позиций</param>
        public void ReceiptWaybillApproved(ReceiptWaybill waybill, DynamicDictionary<Guid, Dictionary<OutgoingWaybillRow, decimal>> outgoingWaybillRowDict)
        {
            // увеличиваем показатели точного наличия для позиций с расхождениями
            var exactArticleAvailabilityIndicators = new List<ExactArticleAvailabilityIndicator>();

            var recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();
            var recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators = new List<OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator>();

            foreach (var row in waybill.Rows.Where(x => x.AreDivergencesAfterReceipt == true || x.PendingCount == 0))
            {
                var indicator = new ExactArticleAvailabilityIndicator(waybill.ApprovementDate.Value, waybill.ReceiptStorage.Id,
                    waybill.AccountOrganization.Id, row.Article.Id, row.Id, row.PurchaseCost, row.CurrentCount);

                exactArticleAvailabilityIndicators.Add(indicator);

                if (row.AcceptedCount > 0)
                {
                    var outgoingWaybillRows = outgoingWaybillRowDict[row.Id].Where(x => x.Key.AcceptanceDate != null);    // исходящие позиции
                    // Цикл просмотра всех исходящих позиций
                     foreach (var outgoingWaybillRow in outgoingWaybillRows)
                     {
                         var indDate = outgoingWaybillRow.Key.AcceptanceDate > waybill.ApprovementDate.Value ? outgoingWaybillRow.Key.AcceptanceDate.Value : waybill.ApprovementDate.Value;

                         // уменьшаем исходящее проведенное (из входящего проведенного) наличие получателя
                         var recipientOutgoingAcceptedFromIncomingAcceptedIndicator = new OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator(indDate, waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id,
                             row.Article.Id, row.Id, row.PurchaseCost, -outgoingWaybillRow.Value);

                         recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators.Add(recipientOutgoingAcceptedFromIncomingAcceptedIndicator);

                         // увеличиваем исходящее проведенное (из точного) наличие получателя
                         var recipientOutgoingAcceptedFromExactIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(indDate, waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id,
                             row.Article.Id, row.Id, row.PurchaseCost, outgoingWaybillRow.Value);

                         recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(recipientOutgoingAcceptedFromExactIndicator);
                     }
                }
            }

            exactArticleAvailabilityIndicatorService.Update(waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id, 
                receiptWaybillRepository.GetArticleBatchesSubquery(waybill.Id), exactArticleAvailabilityIndicators);

            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id, 
                receiptWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators);

            outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id, 
                receiptWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators);
        }

        /// <summary>
        /// Согласование приходной накладной отменено
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        /// <param name="outgoingWaybillRowDict">словарь исходящих позиций</param>
        public void ReceiptWaybillApprovementCanceled(ReceiptWaybill waybill, DynamicDictionary<Guid, Dictionary<OutgoingWaybillRow, decimal>> outgoingWaybillRowDict)
        {
            // если в накладной есть позиции с расхождениями при приемке
            if (waybill.AreDivergencesAfterReceipt)
            {
                // Пересчет показателей точного наличия

                // уменьшаем показатели точного наличия для позиций c расхождениями
                var exactArticleAvailabilityIndicators = new List<ExactArticleAvailabilityIndicator>();

                var recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();
                var recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators = new List<OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator>();

                foreach (var row in waybill.Rows.Where(x => x.AreDivergencesAfterReceipt == true || x.PendingCount == 0))
                {
                    var indicator = new ExactArticleAvailabilityIndicator(waybill.ApprovementDate.Value, waybill.ReceiptStorage.Id,
                        waybill.AccountOrganization.Id, row.Article.Id, row.Id, row.PurchaseCost, -row.CurrentCount);

                    exactArticleAvailabilityIndicators.Add(indicator);

                    if (row.AcceptedCount > 0)
                    {
                        // Цикл по исходящим позициям из позиции прихода
                        foreach (var outgoingRow in outgoingWaybillRowDict[row.Id].Where(x => x.Key.AcceptanceDate != null))
                        {
                            decimal articleCount = outgoingRow.Value;
                            DateTime indDate = outgoingRow.Key.AcceptanceDate > waybill.ApprovementDate ? outgoingRow.Key.AcceptanceDate.Value : waybill.ApprovementDate.Value;

                            // увеличиваем исходящее проведенное (из входящего проведенного) наличие получателя
                            var recipientOutgoingAcceptedFromIncomingAcceptedIndicator = new OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator(indDate, 
                                waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id, row.Article.Id, row.Id, row.PurchaseCost, row.AcceptedCount);

                            recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators.Add(recipientOutgoingAcceptedFromIncomingAcceptedIndicator);

                            // уменьшаем исходящее проведенное (из точного) наличие получателя
                            var recipientOutgoingAcceptedFromExactIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(indDate, waybill.ReceiptStorage.Id, 
                                waybill.AccountOrganization.Id, row.Article.Id, row.Id, row.PurchaseCost, -articleCount);

                            recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(recipientOutgoingAcceptedFromExactIndicator);
                        }
                    }
                }
                exactArticleAvailabilityIndicatorService.Update(waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id, 
                    receiptWaybillRepository.GetArticleBatchesSubquery(waybill.Id), exactArticleAvailabilityIndicators);

                outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id, 
                    receiptWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators);

                outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id, 
                    receiptWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators);
            }
            // иначе накладная сразу будет переведена в статус «Проведено - ожидает поставки»
            else
            {
                // Пересчет показателей точного и проведенного наличия
                var recipientIncomingAcceptedArticleAvailabilityIndicators = new List<IncomingAcceptedArticleAvailabilityIndicator>();
                var exactArticleAvailabilityIndicators = new List<ExactArticleAvailabilityIndicator>();
                var recipientOutgoingAcceptedFromIncomingAcceptedIndicators = new List<OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator>();
                var recipientOutgoingAcceptedFromExactIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();

                foreach (var row in waybill.Rows)
                {
                    // увеличиваем входящее проведенное наличие для всех позиций
                    var recipientIndicator = new IncomingAcceptedArticleAvailabilityIndicator(waybill.ReceiptDate.Value, waybill.ReceiptStorage.Id,
                        waybill.AccountOrganization.Id, row.Article.Id, row.Id, row.PurchaseCost, row.CurrentCount);

                    recipientIncomingAcceptedArticleAvailabilityIndicators.Add(recipientIndicator);

                    // уменьшаем показатели точного наличия для всех позиций
                    var recipientExactIndicator = new ExactArticleAvailabilityIndicator(waybill.ReceiptDate.Value, waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id,
                        row.Article.Id, row.Id, row.PurchaseCost, -row.CurrentCount);

                    exactArticleAvailabilityIndicators.Add(recipientExactIndicator);

                    // Цикл по исходящим позициям из позиции прихода
                    foreach (var outgoingRow in outgoingWaybillRowDict[row.Id].Where(x => x.Key.AcceptanceDate != null))
                    {
                        decimal articleCount = outgoingRow.Value;
                        DateTime acceptanceDate = outgoingRow.Key.AcceptanceDate.Value;

                        // Определяем дату, от которой нужно изменить показатель (максимум из даты проводки исходящей позиции и датой приемки прихода)
                        var indDate = acceptanceDate > waybill.ReceiptDate.Value ? acceptanceDate : waybill.ReceiptDate.Value;

                        // увеличиваем исходящее проведенное (из входящего проведенного) наличие получателя
                        var recipientOutgoingAcceptedFromIncomingAcceptedIndicator = new OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator(indDate,
                            waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id, row.Article.Id, row.Id, row.PurchaseCost, articleCount);

                        recipientOutgoingAcceptedFromIncomingAcceptedIndicators.Add(recipientOutgoingAcceptedFromIncomingAcceptedIndicator);
                        

                        // уменьшаем исходящее проведенное (из точного) наличие получателя
                        var recipientOutgoingAcceptedFromExactIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(indDate, waybill.ReceiptStorage.Id, 
                            waybill.AccountOrganization.Id, row.Article.Id, row.Id, row.PurchaseCost, -articleCount);

                        recipientOutgoingAcceptedFromExactIndicators.Add(recipientOutgoingAcceptedFromExactIndicator);
                        
                    }
                }
                outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id,
                    receiptWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientOutgoingAcceptedFromIncomingAcceptedIndicators);

                outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id,
                    receiptWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientOutgoingAcceptedFromExactIndicators);

                incomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id, 
                    receiptWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientIncomingAcceptedArticleAvailabilityIndicators);

                exactArticleAvailabilityIndicatorService.Update(waybill.ReceiptStorage.Id, waybill.AccountOrganization.Id, 
                    receiptWaybillRepository.GetArticleBatchesSubquery(waybill.Id), exactArticleAvailabilityIndicators);
            }
        }

        #endregion

        #endregion

        #region Накладная перемещения

        #region Проводка / отмена проводки

        /// <summary>
        /// Накладная перемещения проведена
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        public void MovementWaybillAccepted(MovementWaybill waybill, IEnumerable<OutgoingWaybillRowSourceReservationInfo> reservationInfoList)
        {
            // увеличиваем исходящее проведенное наличие отправителя и входящее проведенное наличие получателя
            var senderOutgoingAcceptedFromExactArticleAvailabilityIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();
            var senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators = new List<OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator>();

            var recipientIncomingAcceptedArticleAvailabilityIndicators = new List<IncomingAcceptedArticleAvailabilityIndicator>();

            foreach (var row in waybill.Rows)
            {
                var reservationInfo = reservationInfoList.FirstOrDefault(x => x.RowId == row.Id);

                // увеличиваем исходящее проведенное из точного наличие отправителя
                if (reservationInfo.ReservedFromExactArticleAvailabilityCount > 0)
                {
                    var senderIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(waybill.AcceptanceDate.Value, waybill.SenderStorage.Id, waybill.Sender.Id,
                        row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost,
                        reservationInfo.ReservedFromExactArticleAvailabilityCount);

                    senderOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(senderIndicator);
                }

                // увеличиваем исходящее проведенное из входящего проведенного наличие отправителя
                if (reservationInfo.ReservedFromIncomingAcceptedArticleAvailabilityCount > 0)
                {
                    var senderIndicator = new OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator(waybill.AcceptanceDate.Value, waybill.SenderStorage.Id, waybill.Sender.Id,
                        row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost,
                        reservationInfo.ReservedFromIncomingAcceptedArticleAvailabilityCount);

                    senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators.Add(senderIndicator);
                }

                var recipientIndicator = new IncomingAcceptedArticleAvailabilityIndicator(waybill.AcceptanceDate.Value, waybill.RecipientStorage.Id, waybill.Recipient.Id,
                    row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, row.MovingCount);

                recipientIncomingAcceptedArticleAvailabilityIndicators.Add(recipientIndicator);
            }


            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.SenderStorage.Id, waybill.Sender.Id, 
                movementWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderOutgoingAcceptedFromExactArticleAvailabilityIndicators);

            outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.SenderStorage.Id, waybill.Sender.Id,
                movementWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators);

            incomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.RecipientStorage.Id, waybill.Recipient.Id,
                movementWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientIncomingAcceptedArticleAvailabilityIndicators);
        }

        /// <summary>
        /// Проводка накладной перемещения отменена
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        /// <param name="reservationInfoList">Информация о резервировании товара</param>
        /// <param name="sourceWaybillRowDict">Словарь источников</param>
        public void MovementWaybillAcceptanceCanceled(MovementWaybill waybill, IEnumerable<OutgoingWaybillRowSourceReservationInfo> reservationInfoList,
            DynamicDictionary<Guid, Dictionary<IncomingWaybillRow, decimal>> sourceWaybillRowDict)
        {
            // Пересчет показателей входящего и исходящего проведенного наличия
            var acceptanceDate = waybill.AcceptanceDate.Value;

            // уменьшаем исходящее проведенное наличие отправителя и входящее проведенное наличие получателя
            var senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators = new List<OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator>();
            var senderOutgoingAcceptedFromExactArticleAvailabilityIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();
            var recipientIncomingAcceptedArticleAvailabilityIndicators = new List<IncomingAcceptedArticleAvailabilityIndicator>();

            foreach (var row in waybill.Rows)
            {
                var reservationInfo = reservationInfoList.FirstOrDefault(x => x.RowId == row.Id);

                if (reservationInfo.ReservedFromExactArticleAvailabilityCount > 0)
                {
                    var sourceWaybillRows = sourceWaybillRowDict[row.Id].Where(x=>x.Key.FinalizationDate != null);   // Получаем источники, которые в точном наличии
                    // Цикл по всем источникам
                    foreach (var sourceWaybillRow in sourceWaybillRows)
                    {
                        // дата поступления источника в точное наличие
                        var sourceReceiptDate = sourceWaybillRow.Key.FinalizationDate;

                        // Берем наибольшую дату между поступлением в точное наличие источника и проводкой исходящей позиции
                        var indDate = sourceReceiptDate > acceptanceDate ? sourceReceiptDate.Value : acceptanceDate;

                        // фигурные скобки добавлены, чтобы избежать перекрытия имен переменных
                        {
                            // уменьшаем исходящее проведенное из точного наличие отправителя
                            var senderIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(indDate, waybill.SenderStorage.Id, waybill.Sender.Id,
                                row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost,
                                -sourceWaybillRow.Value);

                            senderOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(senderIndicator);
                        }
                        // Если источник поступил в точное наличие после проводки исходящей накладной ...
                        if (sourceReceiptDate > acceptanceDate)
                        {
                            // ... то необходимо обновить проведенное исходящее из проведенного входящего.
                            var senderIndicator = new OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator(acceptanceDate, waybill.SenderStorage.Id, waybill.Sender.Id,
                                row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost,
                                -sourceWaybillRow.Value) { EndDate = sourceReceiptDate.Value };

                            senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators.Add(senderIndicator);
                        }
                        // Иначе ничего больше обновлять не нужно.
                    }
                }

                if (reservationInfo.ReservedFromIncomingAcceptedArticleAvailabilityCount > 0)
                {
                    // уменьшаем исходящее проведенное из входящего проведенного наличие отправителя
                    var senderIndicator = new OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator(acceptanceDate, waybill.SenderStorage.Id, waybill.Sender.Id,
                        row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost,
                        -reservationInfo.ReservedFromIncomingAcceptedArticleAvailabilityCount);

                    senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators.Add(senderIndicator);
                }

                var recipientIndicator = new IncomingAcceptedArticleAvailabilityIndicator(acceptanceDate, waybill.RecipientStorage.Id, waybill.Recipient.Id,
                    row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, -row.MovingCount);

                recipientIncomingAcceptedArticleAvailabilityIndicators.Add(recipientIndicator);
            }

            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.SenderStorage.Id, waybill.Sender.Id,
                movementWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderOutgoingAcceptedFromExactArticleAvailabilityIndicators);

            outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.SenderStorage.Id, waybill.Sender.Id, 
                movementWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators);

            incomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.RecipientStorage.Id, waybill.Recipient.Id, 
                movementWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientIncomingAcceptedArticleAvailabilityIndicators);
        }

        #endregion

        #region Приемка / отмена приемки

        /// <summary>
        /// Накладная перемещения принята
        /// </summary>
        /// <param name="waybill">накладная перемещения</param>
        public void MovementWaybillReceipted(MovementWaybill waybill)
        {
            // Пересчет показателей точного и проведенного наличия
            var senderOutgoingAcceptedFromExactArticleAvailabilityIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();
            var recipientIncomingAcceptedArticleAvailabilityIndicators = new List<IncomingAcceptedArticleAvailabilityIndicator>();

            var recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();
            var recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators = new List<OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator>();

            var senderExactArticleAvailabilityIndicators = new List<ExactArticleAvailabilityIndicator>();
            var recipientExactArticleAvailabilityIndicators = new List<ExactArticleAvailabilityIndicator>();

            foreach (var row in waybill.Rows)
            {
                // показатели проведенного наличия
                // уменьшаем исходящее проведенное наличие отправителя
                var senderIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(waybill.ReceiptDate.Value, waybill.SenderStorage.Id, waybill.Sender.Id,
                    row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, -row.MovingCount);

                senderOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(senderIndicator);

                // уменьшаем входящее проведенное наличие получателя
                var recipientIndicator = new IncomingAcceptedArticleAvailabilityIndicator(waybill.ReceiptDate.Value, waybill.RecipientStorage.Id, waybill.Recipient.Id,
                    row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, -row.MovingCount);

                recipientIncomingAcceptedArticleAvailabilityIndicators.Add(recipientIndicator);

                if (row.AcceptedCount > 0)
                {
                    // уменьшаем исходящее проведенное (из входящего проведенного) наличие получателя
                    var recipientOutgoingAcceptedFromIncomingAcceptedIndicator = new OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator(waybill.ReceiptDate.Value, 
                        waybill.RecipientStorage.Id, waybill.Recipient.Id, row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, -row.AcceptedCount);

                    recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators.Add(recipientOutgoingAcceptedFromIncomingAcceptedIndicator);

                    // увеличиваем исходящее проведенное (из точного) наличие получателя
                    var recipientOutgoingAcceptedFromExactIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(waybill.ReceiptDate.Value, waybill.RecipientStorage.Id, 
                        waybill.Recipient.Id, row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, row.AcceptedCount);

                    recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(recipientOutgoingAcceptedFromExactIndicator);
                }

                // показатели точного наличия
                // уменьшаем точное наличие отправителя
                var senderExactIndicator = new ExactArticleAvailabilityIndicator(waybill.ReceiptDate.Value, waybill.SenderStorage.Id, waybill.Sender.Id,
                    row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, -row.MovingCount);

                senderExactArticleAvailabilityIndicators.Add(senderExactIndicator);

                // увеличиваем точное наличие получателя
                var recipientExactIndicator = new ExactArticleAvailabilityIndicator(waybill.ReceiptDate.Value, waybill.RecipientStorage.Id, waybill.Recipient.Id,
                    row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, row.MovingCount);

                recipientExactArticleAvailabilityIndicators.Add(recipientExactIndicator);
            }

            // показатели проведенного наличия
            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.SenderStorage.Id, waybill.Sender.Id, 
                movementWaybillRepository.GetArticleBatchesSubquery(waybill.Id), 
                senderOutgoingAcceptedFromExactArticleAvailabilityIndicators);

            incomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.RecipientStorage.Id, waybill.Recipient.Id, 
                movementWaybillRepository.GetArticleBatchesSubquery(waybill.Id), 
                recipientIncomingAcceptedArticleAvailabilityIndicators);

            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.RecipientStorage.Id, waybill.Recipient.Id, 
                movementWaybillRepository.GetArticleBatchesSubquery(waybill.Id), 
                recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators);

            outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.RecipientStorage.Id, waybill.Recipient.Id, 
                movementWaybillRepository.GetArticleBatchesSubquery(waybill.Id), 
                recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators);

            // показатели точного наличия
            exactArticleAvailabilityIndicatorService.Update(waybill.SenderStorage.Id, waybill.Sender.Id, 
                movementWaybillRepository.GetArticleBatchesSubquery(waybill.Id), 
                senderExactArticleAvailabilityIndicators);

            exactArticleAvailabilityIndicatorService.Update(waybill.RecipientStorage.Id, waybill.Recipient.Id, 
                movementWaybillRepository.GetArticleBatchesSubquery(waybill.Id), 
                recipientExactArticleAvailabilityIndicators);
        }

        /// <summary>
        /// Отмена приемки накладной перемещения
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        /// <param name="outgoingWaybillRowDict">словарь исходящих позиций</param>
        public void MovementWaybillReceiptCanceled(MovementWaybill waybill, DynamicDictionary<Guid, Dictionary<OutgoingWaybillRow, decimal>> outgoingWaybillRowDict)
        {
            // Пересчет показателей точного и проведенного наличия
            var senderOutgoingAcceptedFromExactArticleAvailabilityIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();
            var recipientIncomingAcceptedArticleAvailabilityIndicators = new List<IncomingAcceptedArticleAvailabilityIndicator>();

            var senderExactArticleAvailabilityIndicators = new List<ExactArticleAvailabilityIndicator>();
            var recipientExactArticleAvailabilityIndicators = new List<ExactArticleAvailabilityIndicator>();

            var recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators = new List<OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator>();
            var recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();            

            foreach (var row in waybill.Rows)
            {
                // показатели проведенного наличия
                // увеличиваем исходящее проведенное наличие отправителя
                var senderIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(waybill.ReceiptDate.Value, waybill.SenderStorage.Id, waybill.Sender.Id,
                    row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, row.MovingCount);
                senderOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(senderIndicator);

                // увеличиваем входящее проведенное наличие получателя
                var recipientIndicator = new IncomingAcceptedArticleAvailabilityIndicator(waybill.ReceiptDate.Value, waybill.RecipientStorage.Id, waybill.Recipient.Id,
                    row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, row.MovingCount);

                recipientIncomingAcceptedArticleAvailabilityIndicators.Add(recipientIndicator);

                if (row.AcceptedCount > 0)
                {
                     // Цикл по исходящим позициям из позиции накладной перемещения
                    foreach (var outgoingRow in outgoingWaybillRowDict[row.Id].Where(x => x.Key.AcceptanceDate != null))
                    {
                        decimal articleCount = outgoingRow.Value;
                        DateTime acceptanceDate = outgoingRow.Key.AcceptanceDate.Value;

                        // Определяем дату, от которой нужно изменить показатель (максимум из даты проводки исходящей позиции и датой приемки накладной перемещения)
                        var indDate = acceptanceDate > waybill.ReceiptDate.Value ? acceptanceDate : waybill.ReceiptDate.Value;

                        // увеличиваем исходящее проведенное (из входящего проведенного) наличие получателя
                        var recipientOutgoingAcceptedFromIncomingAcceptedIndicator = new OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator(indDate, 
                            waybill.RecipientStorage.Id, waybill.Recipient.Id, row.Article.Id, row.ReceiptWaybillRow.Id, 
                            row.ReceiptWaybillRow.PurchaseCost, articleCount);

                        recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators.Add(recipientOutgoingAcceptedFromIncomingAcceptedIndicator);

                        // уменьшаем исходящее проведенное (из точного) наличие получателя
                        var recipientOutgoingAcceptedFromExactIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(indDate, 
                            waybill.RecipientStorage.Id, waybill.Recipient.Id, row.Article.Id, row.ReceiptWaybillRow.Id, 
                            row.ReceiptWaybillRow.PurchaseCost, -articleCount);

                        recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(recipientOutgoingAcceptedFromExactIndicator);
                    }
                }

                // показатели точного наличия
                // увеличиваем точное наличие отправителя
                var senderExactIndicator = new ExactArticleAvailabilityIndicator(waybill.ReceiptDate.Value, waybill.SenderStorage.Id, waybill.Sender.Id,
                    row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, row.MovingCount);

                senderExactArticleAvailabilityIndicators.Add(senderExactIndicator);

                // уменьшаем точное наличие получателя
                var recipientExactIndicator = new ExactArticleAvailabilityIndicator(waybill.ReceiptDate.Value, waybill.RecipientStorage.Id, waybill.Recipient.Id,
                    row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, -row.MovingCount);

                recipientExactArticleAvailabilityIndicators.Add(recipientExactIndicator);
            }

            outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.RecipientStorage.Id,
                waybill.Recipient.Id, movementWaybillRepository.GetArticleBatchesSubquery(waybill.Id),
                recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators);

            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.RecipientStorage.Id, waybill.Recipient.Id,
                movementWaybillRepository.GetArticleBatchesSubquery(waybill.Id),
                recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators);

            // показатели проведенного наличия
            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.SenderStorage.Id, waybill.Sender.Id, 
                movementWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderOutgoingAcceptedFromExactArticleAvailabilityIndicators);

            incomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.RecipientStorage.Id, waybill.Recipient.Id, 
                movementWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientIncomingAcceptedArticleAvailabilityIndicators);

            // показатели точного наличия
            exactArticleAvailabilityIndicatorService.Update(waybill.SenderStorage.Id, waybill.Sender.Id, 
                movementWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderExactArticleAvailabilityIndicators);

            exactArticleAvailabilityIndicatorService.Update(waybill.RecipientStorage.Id, waybill.Recipient.Id, 
                movementWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientExactArticleAvailabilityIndicators);
        }

        #endregion

        #endregion

        #region Накладная смены собственника

        #region Проводка / отмена проводки
        
        /// <summary>
        /// Накладная смены собственника проведена
        /// </summary>
        /// <param name="waybill">Накладная смены собственника</param>
        public void ChangeOwnerWaybillAccepted(ChangeOwnerWaybill waybill, IEnumerable<OutgoingWaybillRowSourceReservationInfo> reservationInfoList)
        {
            // Пересчет показателей входящего и исходящего проведенного наличия

            // увеличиваем исходящее проведенное наличие отправителя и входящее проведенное наличие получателя
            var senderOutgoingAcceptedFromExactArticleAvailabilityIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();
            var senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators = new List<OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator>();

            var recipientIncomingAcceptedArticleAvailabilityIndicators = new List<IncomingAcceptedArticleAvailabilityIndicator>();

            foreach (var row in waybill.Rows)
            {
                var reservationInfo = reservationInfoList.FirstOrDefault(x => x.RowId == row.Id);

                // увеличиваем исходящее проведенное из точного наличие отправителя
                if (reservationInfo.ReservedFromExactArticleAvailabilityCount > 0)
                {
                    var senderIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(waybill.AcceptanceDate.Value, waybill.Storage.Id, waybill.Sender.Id,
                        row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost,
                        reservationInfo.ReservedFromExactArticleAvailabilityCount);

                    senderOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(senderIndicator);
                }

                // увеличиваем исходящее проведенное из входящего проведенного наличие отправителя
                if (reservationInfo.ReservedFromIncomingAcceptedArticleAvailabilityCount > 0)
                {
                    var senderIndicator = new OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator(waybill.AcceptanceDate.Value, 
                        waybill.Storage.Id, waybill.Sender.Id, row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost,
                        reservationInfo.ReservedFromIncomingAcceptedArticleAvailabilityCount);

                    senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators.Add(senderIndicator);
                }

                var recipientIndicator = new IncomingAcceptedArticleAvailabilityIndicator(waybill.AcceptanceDate.Value, waybill.Storage.Id, 
                    waybill.Recipient.Id, row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, row.MovingCount);

                recipientIncomingAcceptedArticleAvailabilityIndicators.Add(recipientIndicator);
            }

            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.Storage.Id, waybill.Sender.Id, 
                changeOwnerWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderOutgoingAcceptedFromExactArticleAvailabilityIndicators);

            outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.Storage.Id, waybill.Sender.Id, 
                changeOwnerWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators);

            incomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.Storage.Id, waybill.Recipient.Id, 
                changeOwnerWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientIncomingAcceptedArticleAvailabilityIndicators);
        }

        /// <summary>
        /// Проводка накладной смены собственника отменена
        /// </summary>
        /// <param name="waybill">Накладная смены собственника</param>
        /// <param name="reservationInfoList">Информация о резервировании товара</param>
        /// <param name="sourceWaybillRowDict">Словарь источников</param>
        public void ChangeOwnerWaybillAcceptanceCanceled(ChangeOwnerWaybill waybill, IEnumerable<OutgoingWaybillRowSourceReservationInfo> reservationInfoList,
            DynamicDictionary<Guid, Dictionary<IncomingWaybillRow, decimal>> sourceWaybillRowDict)
        {
            // Пересчет показателей входящего и исходящего проведенного наличия

            var acceptanceDate = waybill.AcceptanceDate.Value;

            // уменьшаем исходящее проведенное наличие отправителя и входящее проведенное наличие получателя
            var senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators = new List<OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator>();
            var senderOutgoingAcceptedFromExactArticleAvailabilityIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();

            var recipientIncomingAcceptedArticleAvailabilityIndicators = new List<IncomingAcceptedArticleAvailabilityIndicator>();

            foreach (var row in waybill.Rows)
            {
                var reservationInfo = reservationInfoList.FirstOrDefault(x => x.RowId == row.Id);

                // уменьшаем исходящее проведенное из точного наличие отправителя
                if (reservationInfo.ReservedFromExactArticleAvailabilityCount > 0)
                {
                    var sourceWaybillRows = sourceWaybillRowDict[row.Id].Where(x => x.Key.FinalizationDate != null);   // Получаем источники, которые в точном наличии
                    // Цикл по всем источникам
                    foreach (var sourceWaybillRow in sourceWaybillRows)
                    {
                        // дата поступления источника в точное наличие
                        var sourceReceiptDate = sourceWaybillRow.Key.FinalizationDate;

                        // Берем наибольшую даты между поступлением в точное наличие источника и  проводкой исходящей позиции
                        var indDate = sourceReceiptDate > acceptanceDate ? sourceReceiptDate.Value : acceptanceDate;
                        // фигурные скобки добавлены, чтобы избежать перекрытия имен переменных
                        {
                            var senderIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(indDate, 
                                waybill.Storage.Id, waybill.Sender.Id, row.Article.Id, row.ReceiptWaybillRow.Id, 
                                row.ReceiptWaybillRow.PurchaseCost, -sourceWaybillRow.Value);

                            senderOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(senderIndicator);
                        }
                        // Если источник поступил в точное наличие после проводки исходящей накладной ...
                        if (sourceReceiptDate > acceptanceDate)
                        {
                            // ... то необходимо обновить проведенное исходящее из проведенного входящего.
                            var senderIndicator = new OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator(acceptanceDate, 
                                waybill.Storage.Id, waybill.Sender.Id,row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost,
                                -sourceWaybillRow.Value) { EndDate = sourceReceiptDate.Value };

                            senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators.Add(senderIndicator);
                        }
                        // Иначе ничего больше обновлять не нужно.
                    }
                }

                // уменьшаем исходящее проведенное из входящего проведенного наличие отправителя
                if (reservationInfo.ReservedFromIncomingAcceptedArticleAvailabilityCount > 0)
                {
                    var senderIndicator = new OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator(acceptanceDate, 
                        waybill.Storage.Id, waybill.Sender.Id,row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost,
                        -reservationInfo.ReservedFromIncomingAcceptedArticleAvailabilityCount);

                    senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators.Add(senderIndicator);
                }

                var recipientIndicator = new IncomingAcceptedArticleAvailabilityIndicator(acceptanceDate, waybill.Storage.Id, waybill.Recipient.Id,
                    row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, -row.MovingCount);

                recipientIncomingAcceptedArticleAvailabilityIndicators.Add(recipientIndicator);
            }

            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.Storage.Id, waybill.Sender.Id,
                changeOwnerWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderOutgoingAcceptedFromExactArticleAvailabilityIndicators);

            outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.Storage.Id, waybill.Sender.Id, 
                changeOwnerWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators);

            incomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.Storage.Id, waybill.Recipient.Id, 
                changeOwnerWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientIncomingAcceptedArticleAvailabilityIndicators);
        }

        #endregion

        #region Смена владельца / отмена смены владельца

        /// <summary>
        /// Собственник изменен
        /// </summary>
        /// <param name="waybill">Накладная смены собственника</param>
        /// <param name="outgoingWaybillRowDict">Словарь исходящих позиций</param>
        public void ChangeOwnerWaybillOwnerChanged(ChangeOwnerWaybill waybill, DynamicDictionary<Guid, Dictionary<OutgoingWaybillRow, decimal>> outgoingWaybillRowDict)
        {
            // Пересчет показателей точного наличия и проведенного наличия
            var senderOutgoingAcceptedFromExactArticleAvailabilityIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();
            var recipientIncomingAcceptedArticleAvailabilityIndicators = new List<IncomingAcceptedArticleAvailabilityIndicator>();

            var recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();
            var recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators = new List<OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator>();

            var senderExactArticleAvailabilityIndicators = new List<ExactArticleAvailabilityIndicator>();
            var recipientExactArticleAvailabilityIndicators = new List<ExactArticleAvailabilityIndicator>();

            foreach (var row in waybill.Rows)
            {
                // показатели проведенного наличия
                // уменьшаем исходящее проведенное наличие отправителя
                var senderIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(waybill.ChangeOwnerDate.Value, waybill.Storage.Id, waybill.Sender.Id,
                    row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, -row.MovingCount);

                senderOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(senderIndicator);
                
                // уменьшаем входящее проведенное наличие получателя
                var recipientIndicator = new IncomingAcceptedArticleAvailabilityIndicator(waybill.ChangeOwnerDate.Value, waybill.Storage.Id, waybill.Recipient.Id,
                    row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, -row.MovingCount);

                recipientIncomingAcceptedArticleAvailabilityIndicators.Add(recipientIndicator);

                if (row.AcceptedCount > 0)
                {
                    // Цикл прохода исходящих позиций
                    foreach (var outgoingRow in outgoingWaybillRowDict[row.Id].Where(x => x.Key.AcceptanceDate != null))
                    {
                        // Определяем дату обновления индикатора
                        var indDate = outgoingRow.Key.AcceptanceDate > waybill.ChangeOwnerDate ? outgoingRow.Key.AcceptanceDate.Value : waybill.ChangeOwnerDate.Value;

                        // уменьшаем исходящее проведенное (из входящего проведенного) наличие получателя
                        var recipientOutgoingAcceptedFromIncomingAcceptedIndicator = new OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator(indDate, 
                            waybill.Storage.Id, waybill.Recipient.Id, row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, -outgoingRow.Value);

                        recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators.Add(recipientOutgoingAcceptedFromIncomingAcceptedIndicator);

                        // увеличиваем исходящее проведенное (из точного) наличие получателя
                        var recipientOutgoingAcceptedFromExactIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(indDate, 
                            waybill.Storage.Id, waybill.Recipient.Id, row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, outgoingRow.Value);

                        recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(recipientOutgoingAcceptedFromExactIndicator);
                    }
                }

                // показатели точного наличия
                // уменьшаем показатель точного наличия отправителя
                var senderExactIndicator = new ExactArticleAvailabilityIndicator(waybill.ChangeOwnerDate.Value, waybill.Storage.Id, 
                    waybill.Sender.Id, row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, -row.MovingCount);

                senderExactArticleAvailabilityIndicators.Add(senderExactIndicator);

                // увеличиваем показатель точного наличия получателя
                var recipientExactIndicator = new ExactArticleAvailabilityIndicator(waybill.ChangeOwnerDate.Value, waybill.Storage.Id, 
                    waybill.Recipient.Id, row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, row.MovingCount);

                recipientExactArticleAvailabilityIndicators.Add(recipientExactIndicator);
            }

            // показатели проведенного наличия            
            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.Storage.Id, waybill.Sender.Id, 
                changeOwnerWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderOutgoingAcceptedFromExactArticleAvailabilityIndicators);

            incomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.Storage.Id, waybill.Recipient.Id, 
                changeOwnerWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientIncomingAcceptedArticleAvailabilityIndicators);

            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.Storage.Id, waybill.Recipient.Id, 
                changeOwnerWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators);

            outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.Storage.Id, waybill.Recipient.Id, 
                changeOwnerWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators);

            // показатели точного наличия            
            exactArticleAvailabilityIndicatorService.Update(waybill.Storage.Id, waybill.Sender.Id, 
                changeOwnerWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderExactArticleAvailabilityIndicators);
            exactArticleAvailabilityIndicatorService.Update(waybill.Storage.Id, waybill.Recipient.Id, 
                changeOwnerWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientExactArticleAvailabilityIndicators);
        }

        /// <summary>
        /// Смена собственника отменена
        /// </summary>
        /// <param name="waybill">Накладная смены собственника</param>
        /// <param name="outgoingWaybillRowDict">словарь исходящих позиций</param>
        public void ChangeOwnerWaybillOwnerChangeCanceled(ChangeOwnerWaybill waybill, DynamicDictionary<Guid, Dictionary<OutgoingWaybillRow, decimal>> outgoingWaybillRowDict)
        {
            // Пересчет показателей точного и проведенного наличия
            var senderOutgoingAcceptedFromExactArticleAvailabilityIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();
            var recipientIncomingAcceptedArticleAvailabilityIndicators = new List<IncomingAcceptedArticleAvailabilityIndicator>();

            var senderExactArticleAvailabilityIndicators = new List<ExactArticleAvailabilityIndicator>();
            var recipientExactArticleAvailabilityIndicators = new List<ExactArticleAvailabilityIndicator>();

            var recipientOutgoingAcceptedFromIncomingAcceptedIndicators = new List<OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator>();

            foreach (var row in waybill.Rows)
            {
                // показатели проведенного наличия
                // увеличиваем исходящее проведенное наличие отправителя
                var senderIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(waybill.ChangeOwnerDate.Value, 
                    waybill.Storage.Id, waybill.Sender.Id, row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, row.MovingCount);

                senderOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(senderIndicator);

                // увеличиваем входящее проведенное наличие получателя
                var recipientIndicator = new IncomingAcceptedArticleAvailabilityIndicator(waybill.ChangeOwnerDate.Value, 
                    waybill.Storage.Id, waybill.Recipient.Id, row.Article.Id, row.ReceiptWaybillRow.Id, 
                    row.ReceiptWaybillRow.PurchaseCost, row.MovingCount);

                recipientIncomingAcceptedArticleAvailabilityIndicators.Add(recipientIndicator);

                if (row.AcceptedCount > 0)
                {
                    // Цикл по исходящим позициям из позиции накладной смены собственника
                    foreach (var outgoingRow in outgoingWaybillRowDict[row.Id].Where(x => x.Key.AcceptanceDate != null))
                    {
                        decimal articleCount = outgoingRow.Value;
                        DateTime acceptanceDate = outgoingRow.Key.AcceptanceDate.Value;

                        // Определяем дату, от которой нужно изменить показатель (максимум из даты проводки исходящей позиции и датой смены собственника)
                        var indDate = acceptanceDate > waybill.ChangeOwnerDate.Value ? acceptanceDate : waybill.ChangeOwnerDate.Value;

                        // увеличиваем исходящее проведенное (из входящего проведенного) наличие получателя
                        var recipientOutgoingAcceptedFromIncomingAcceptedIndicator = new OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator(indDate, 
                            waybill.Storage.Id, waybill.Recipient.Id, row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, articleCount);

                        recipientOutgoingAcceptedFromIncomingAcceptedIndicators.Add(recipientOutgoingAcceptedFromIncomingAcceptedIndicator);

                        // уменьшаем исходящее проведенное (из точного) наличие получателя
                        var recipientOutgoingAcceptedFromExactIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(indDate, 
                            waybill.Storage.Id, waybill.Recipient.Id, row.Article.Id, row.ReceiptWaybillRow.Id, 
                            row.ReceiptWaybillRow.PurchaseCost, -articleCount);

                        senderOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(recipientOutgoingAcceptedFromExactIndicator);
                    }
                }

                // точное наличие
                // увеличиваем показатель точного наличия отправителя
                var senderExactIndicator = new ExactArticleAvailabilityIndicator(waybill.ChangeOwnerDate.Value, waybill.Storage.Id, 
                    waybill.Sender.Id, row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, row.MovingCount);

                senderExactArticleAvailabilityIndicators.Add(senderExactIndicator);

                // уменьшаем показатель точного наличия получателя
                var recipientExactIndicator = new ExactArticleAvailabilityIndicator(waybill.ChangeOwnerDate.Value, waybill.Storage.Id, 
                    waybill.Recipient.Id, row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, -row.MovingCount);

                recipientExactArticleAvailabilityIndicators.Add(recipientExactIndicator);
            }

            // показатели проведенного наличия
            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.Storage.Id, waybill.Sender.Id, 
                changeOwnerWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderOutgoingAcceptedFromExactArticleAvailabilityIndicators);

            outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.Storage.Id, waybill.Recipient.Id,
                changeOwnerWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientOutgoingAcceptedFromIncomingAcceptedIndicators);

            incomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.Storage.Id, waybill.Recipient.Id, 
                changeOwnerWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientIncomingAcceptedArticleAvailabilityIndicators);

            // показатели точного наличия
            exactArticleAvailabilityIndicatorService.Update(waybill.Storage.Id, waybill.Sender.Id, 
                changeOwnerWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderExactArticleAvailabilityIndicators);
            exactArticleAvailabilityIndicatorService.Update(waybill.Storage.Id, waybill.Recipient.Id, 
                changeOwnerWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientExactArticleAvailabilityIndicators);
        }

        #endregion

        #endregion

        #region Накладная списания

        #region Проводка / Отмена проводки

        /// <summary>
        /// Накладная списания проведена
        /// </summary>
        /// <param name="waybill">Накладная списания</param>
        /// <param name="reservationInfoList">Информация о резервировании товара</param>
        public void WriteoffWaybillAccepted(WriteoffWaybill waybill, IEnumerable<OutgoingWaybillRowSourceReservationInfo> reservationInfoList)
        {
            // Пересчет показателей исходящего проведенного наличия

            // увеличиваем исходящее проведенное наличие отправителя
            var senderOutgoingAcceptedFromExactArticleAvailabilityIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();
            var senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators = new List<OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator>();

            foreach (var row in waybill.Rows)
            {
                var reservationInfo = reservationInfoList.FirstOrDefault(x => x.RowId == row.Id);

                // увеличиваем исходящее проведенное из точного наличие отправителя
                if (reservationInfo.ReservedFromExactArticleAvailabilityCount > 0)
                {
                    var senderIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(waybill.AcceptanceDate.Value, 
                        waybill.SenderStorage.Id, waybill.Sender.Id, row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost,
                        reservationInfo.ReservedFromExactArticleAvailabilityCount);

                    senderOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(senderIndicator);
                }

                // увеличиваем исходящее проведенное из входящего проведенного наличие отправителя
                if (reservationInfo.ReservedFromIncomingAcceptedArticleAvailabilityCount > 0)
                {
                    var senderIndicator = new OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator(waybill.AcceptanceDate.Value, 
                        waybill.SenderStorage.Id, waybill.Sender.Id, row.Article.Id, row.ReceiptWaybillRow.Id,
                        row.ReceiptWaybillRow.PurchaseCost, reservationInfo.ReservedFromIncomingAcceptedArticleAvailabilityCount);

                    senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators.Add(senderIndicator);
                }
            }
            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.SenderStorage.Id, waybill.Sender.Id, 
                writeoffWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderOutgoingAcceptedFromExactArticleAvailabilityIndicators);

            outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.SenderStorage.Id, waybill.Sender.Id, 
                writeoffWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators);
        }

        /// <summary>
        /// Проводка накладной списания отменена
        /// </summary>
        /// <param name="waybill">Накладная списания</param>
        /// <param name="reservationInfoList">Информация о резервировании товара</param>
        /// <param name="sourceWaybillRowDict">Словарь источников</param>
        public void WriteoffWaybillAcceptanceCanceled(WriteoffWaybill waybill, IEnumerable<OutgoingWaybillRowSourceReservationInfo> reservationInfoList,
           DynamicDictionary<Guid, Dictionary<IncomingWaybillRow, decimal>> sourceWaybillRowDict)
        {
            // Пересчет показателей входящего проведенного наличия
            var acceptanceDate = waybill.AcceptanceDate.Value;

            // уменьшаем исходящее проведенное наличие отправителя
            var senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators = new List<OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator>();
            var senderOutgoingAcceptedFromExactArticleAvailabilityIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();

            foreach (var row in waybill.Rows)
            {
                var reservationInfo = reservationInfoList.FirstOrDefault(x => x.RowId == row.Id);

                // уменьшаем исходящее проведенное из точного наличие отправителя
                if (reservationInfo.ReservedFromExactArticleAvailabilityCount > 0)
                {
                    var sourceWaybillRows = sourceWaybillRowDict[row.Id].Where(x => x.Key.FinalizationDate != null);   // Получаем источники, которые в точном наличии
                    // Цикл по всем источникам
                    foreach (var sourceWaybillRow in sourceWaybillRows)
                    {
                        // дата поступления источника в точное наличие
                        var sourceReceiptDate = sourceWaybillRow.Key.FinalizationDate;

                        // Берем наибольшую даты между поступлением в точное наличие источника и  проводкой исходящей позиции
                        var indDate = sourceReceiptDate > acceptanceDate ? sourceReceiptDate.Value : acceptanceDate;
                        // фигурные скобки добавлены, чтобы избежать перекрытия имен переменных
                        {
                            var senderIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(indDate, 
                                waybill.SenderStorage.Id, waybill.Sender.Id, row.Article.Id, row.ReceiptWaybillRow.Id, 
                                row.ReceiptWaybillRow.PurchaseCost, -sourceWaybillRow.Value);

                            senderOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(senderIndicator);
                        }
                        // Если источник поступил в точное наличие после проводки исходящей накладной ...
                        if (sourceReceiptDate > acceptanceDate)
                        {
                            // ... то необходимо обновить проведенное исходящее из проведенного входящего.
                            var senderIndicator = new OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator(acceptanceDate, 
                                waybill.SenderStorage.Id, waybill.Sender.Id, row.Article.Id, row.ReceiptWaybillRow.Id, 
                                row.ReceiptWaybillRow.PurchaseCost, -sourceWaybillRow.Value) { EndDate = sourceReceiptDate.Value };

                            senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators.Add(senderIndicator);
                        }
                        // Иначе ничего больше обновлять не нужно.
                    }
                }

                // уменьшаем исходящее проведенное из входящего проведенного наличие отправителя
                if (reservationInfo.ReservedFromIncomingAcceptedArticleAvailabilityCount > 0)
                {
                    var senderIndicator = new OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator(acceptanceDate, 
                        waybill.SenderStorage.Id, waybill.Sender.Id, row.Article.Id, row.ReceiptWaybillRow.Id, 
                        row.ReceiptWaybillRow.PurchaseCost, -reservationInfo.ReservedFromIncomingAcceptedArticleAvailabilityCount);

                    senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators.Add(senderIndicator);
                }
            }

            outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.SenderStorage.Id, waybill.Sender.Id, 
                writeoffWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators);

            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.SenderStorage.Id, waybill.Sender.Id,
                writeoffWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderOutgoingAcceptedFromExactArticleAvailabilityIndicators);
        }

        #endregion

        #region Списание / Отмена списания

        /// <summary>
        /// Накладная списания списана
        /// </summary>
        /// <param name="waybill">Накладная списания</param>
        public void WriteoffWaybillWrittenOff(WriteoffWaybill waybill)
        {
            // Пересчет показателей проведенного исходящего и точного наличия
            var exactArticleAvailabilityIndicators = new List<ExactArticleAvailabilityIndicator>();
            var senderOutgoingAcceptedFromExactArticleAvailabilityIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();

            foreach (var row in waybill.Rows)
            {
                // уменьшаем исходящее проведенное наличие отправителя
                var senderIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(waybill.WriteoffDate.Value, waybill.SenderStorage.Id,
                    waybill.Sender.Id, row.Article.Id, row.ReceiptWaybillRow.Id,
                    row.ReceiptWaybillRow.PurchaseCost, -row.WritingoffCount);

                senderOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(senderIndicator);

                // уменьшаем точное наличие отправителя
                var senderExactIndicator = new ExactArticleAvailabilityIndicator(waybill.WriteoffDate.Value, waybill.SenderStorage.Id, waybill.Sender.Id,
                    row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, -row.WritingoffCount);

                exactArticleAvailabilityIndicators.Add(senderExactIndicator);
            }

            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.SenderStorage.Id, waybill.Sender.Id,
                writeoffWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderOutgoingAcceptedFromExactArticleAvailabilityIndicators);
            exactArticleAvailabilityIndicatorService.Update(waybill.SenderStorage.Id, waybill.Sender.Id, 
                writeoffWaybillRepository.GetArticleBatchesSubquery(waybill.Id), exactArticleAvailabilityIndicators);
        }

        /// <summary>
        /// Списание отменено
        /// </summary>
        /// <param name="waybill">Наклданая списания</param>
        public void WriteoffWaybillWriteoffCanceled(WriteoffWaybill waybill)
        {
            // Пересчет показателей проведенного исходящего и точного наличия
            var senderOutgoingAcceptedFromExactArticleAvailabilityIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();
            var exactArticleAvailabilityIndicators = new List<ExactArticleAvailabilityIndicator>();

            foreach (var row in waybill.Rows)
            {
                // увеличиваем исходящее проведенное наличие отправителя
                var senderIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(waybill.WriteoffDate.Value, waybill.SenderStorage.Id,
                    waybill.Sender.Id, row.Article.Id, row.ReceiptWaybillRow.Id,
                    row.ReceiptWaybillRow.PurchaseCost, row.WritingoffCount);

                senderOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(senderIndicator);

                // увеличиваем точное наличие отправителя
                var senderExactIndicator = new ExactArticleAvailabilityIndicator(waybill.WriteoffDate.Value, waybill.SenderStorage.Id, waybill.Sender.Id,
                    row.Article.Id, row.ReceiptWaybillRow.Id, row.ReceiptWaybillRow.PurchaseCost, row.WritingoffCount);

                exactArticleAvailabilityIndicators.Add(senderExactIndicator);
            }

            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.SenderStorage.Id, waybill.Sender.Id, 
                writeoffWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderOutgoingAcceptedFromExactArticleAvailabilityIndicators);
            exactArticleAvailabilityIndicatorService.Update(waybill.SenderStorage.Id, waybill.Sender.Id, 
                writeoffWaybillRepository.GetArticleBatchesSubquery(waybill.Id), exactArticleAvailabilityIndicators);
        }

        #endregion

        #endregion

        #region Накладная реализации

        #region Проводка / отмена проводки

        /// <summary>
        /// Накладная реализации проведена
        /// </summary>
        /// <param name="waybill">Накладная реализации</param>
        /// <param name="sourceWaybillRowDict">Словарь источников</param>
        public void ExpenditureWaybillAccepted(ExpenditureWaybill waybill, DynamicDictionary<Guid, Dictionary<IncomingWaybillRow, decimal>> sourceWaybillRowDict)
        {
            // Пересчет показателей исходящего проведенного наличия

            // увеличиваем исходящее проведенное наличие отправителя
            var senderOutgoingAcceptedFromExactArticleAvailabilityIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();
            var senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators = new List<OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator>();

            foreach (var row in waybill.Rows)
            {
                var sourceWaybillRows = sourceWaybillRowDict[row.Id];   // Получаем источники
                // Цикл прохода источников
                foreach (var sourceWaybillRow in sourceWaybillRows)
                {
                    // увеличиваем исходящее проведенное из точного наличие отправителя
                    if (sourceWaybillRow.Key.FinalizationDate != null)
                    {
                        if (sourceWaybillRow.Key.FinalizationDate <= waybill.AcceptanceDate.Value)
                        {
                            var senderIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(waybill.AcceptanceDate.Value, 
                                waybill.SenderStorage.Id, waybill.Sender.Id, row.Article.Id, row.As<ExpenditureWaybillRow>().ReceiptWaybillRow.Id,
                                row.As<ExpenditureWaybillRow>().ReceiptWaybillRow.PurchaseCost,
                                sourceWaybillRow.Value);

                            senderOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(senderIndicator);
                        }
                        else
                        {
                            var senderIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(sourceWaybillRow.Key.FinalizationDate.Value, 
                                waybill.SenderStorage.Id, waybill.Sender.Id, row.Article.Id, row.As<ExpenditureWaybillRow>().ReceiptWaybillRow.Id,
                                row.As<ExpenditureWaybillRow>().ReceiptWaybillRow.PurchaseCost, sourceWaybillRow.Value);

                            senderOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(senderIndicator);
                        }

                    }
                    // увеличиваем исходящее проведенное из входящего проведенного наличие отправителя
                    if (sourceWaybillRow.Key.FinalizationDate == null || sourceWaybillRow.Key.FinalizationDate > waybill.AcceptanceDate.Value)
                    {
                        // Искуссвенное добавление открытого индикатора. Т.к. при проводке реализации задним числом, может быть записан индикатор 
                        // с установленными начальной и конечной датой (при условии, что в базе нет индикаторов за требумый период), что является ошибкой. 
                        // Добавление обновления индикаторов на +0 создаст открытый индикатор и ошибки не будет.
                        OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator senderIndicator = null;

                        if (sourceWaybillRow.Key.FinalizationDate != null)
                        {
                            senderIndicator = new OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator(sourceWaybillRow.Key.FinalizationDate.Value,
                                waybill.SenderStorage.Id, waybill.Sender.Id, row.Article.Id, row.As<ExpenditureWaybillRow>().ReceiptWaybillRow.Id,
                                row.As<ExpenditureWaybillRow>().ReceiptWaybillRow.PurchaseCost, 0);

                            senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators.Add(senderIndicator);
                        }

                        senderIndicator = new OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator(waybill.AcceptanceDate.Value, 
                            waybill.SenderStorage.Id, waybill.Sender.Id, row.Article.Id, row.As<ExpenditureWaybillRow>().ReceiptWaybillRow.Id,
                            row.As<ExpenditureWaybillRow>().ReceiptWaybillRow.PurchaseCost,
                            sourceWaybillRow.Value) { EndDate = sourceWaybillRow.Key.FinalizationDate };

                        senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators.Add(senderIndicator);
                    }
                }
            }
            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.SenderStorage.Id, waybill.Sender.Id, 
                expenditureWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderOutgoingAcceptedFromExactArticleAvailabilityIndicators);
            
            outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.SenderStorage.Id, waybill.Sender.Id, 
                expenditureWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators);
        }

        /// <summary>
        /// Проводка реализации отменена
        /// </summary>
        /// <param name="waybill">Накладная реализации</param>
        /// <param name="reservationInfoList">Информация о резервировании товаров</param>
        /// <param name="sourceWaybillRowDict">Словарь источников</param>
        public void ExpenditureWaybillAcceptanceCanceled(ExpenditureWaybill waybill, IEnumerable<OutgoingWaybillRowSourceReservationInfo> reservationInfoList,
            DynamicDictionary<Guid, Dictionary<IncomingWaybillRow, decimal>>  sourceWaybillRowDict)
        {
            // Пересчет показателей входящего проведенного наличия
            var acceptanceDate = waybill.AcceptanceDate.Value;

            // уменьшаем исходящее проведенное наличие отправителя
            var senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators = new List<OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator>();
            var senderOutgoingAcceptedFromExactArticleAvailabilityIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();

            foreach (var row in waybill.Rows)
            {
                var reservationInfo = reservationInfoList.FirstOrDefault(x => x.RowId == row.Id);

                // уменьшаем исходящее проведенное из точного наличие отправителя
                if (reservationInfo.ReservedFromExactArticleAvailabilityCount > 0)
                {
                    var sourceWaybillRows = sourceWaybillRowDict[row.Id].Where(x => x.Key.FinalizationDate != null);   // Получаем источники, которые в точном наличии
                    // Цикл по всем источникам
                    foreach (var sourceWaybillRow in sourceWaybillRows)
                    {
                        // дата поступления источника в точное наличие
                        var sourceReceiptDate = sourceWaybillRow.Key.FinalizationDate;

                        // Берем наибольшую даты между поступлением в точное наличие источника и  проводкой исходящей позиции
                        var indDate = sourceReceiptDate > acceptanceDate ? sourceReceiptDate.Value : acceptanceDate;
                        // фигурные скобки добавлены, чтобы избежать перекрытия имен переменных
                        {
                            var senderIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(indDate, 
                                waybill.SenderStorage.Id, waybill.Sender.Id, row.Article.Id, row.As<ExpenditureWaybillRow>().ReceiptWaybillRow.Id,
                                row.As<ExpenditureWaybillRow>().ReceiptWaybillRow.PurchaseCost, -sourceWaybillRow.Value);

                            senderOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(senderIndicator);
                        }
                        // Если источник поступил в точное наличие после проводки исходящей накладной ...
                        if (sourceReceiptDate > acceptanceDate)
                        {
                            var senderIndicator = new OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator(acceptanceDate, 
                                waybill.SenderStorage.Id, waybill.Sender.Id, row.Article.Id, row.As<ExpenditureWaybillRow>().ReceiptWaybillRow.Id,
                                row.As<ExpenditureWaybillRow>().ReceiptWaybillRow.PurchaseCost, -sourceWaybillRow.Value) 
                                { EndDate = sourceReceiptDate.Value };

                            senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators.Add(senderIndicator);
                        }
                        // Иначе ничего больше обновлять не нужно.
                    }
                }

                // уменьшаем исходящее проведенное из входящего проведенного наличие отправителя
                if (reservationInfo.ReservedFromIncomingAcceptedArticleAvailabilityCount > 0)
                {
                    var senderIndicator = new OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator(acceptanceDate, 
                        waybill.SenderStorage.Id, waybill.Sender.Id, row.Article.Id, row.As<ExpenditureWaybillRow>().ReceiptWaybillRow.Id,
                        row.As<ExpenditureWaybillRow>().ReceiptWaybillRow.PurchaseCost, 
                        -reservationInfo.ReservedFromIncomingAcceptedArticleAvailabilityCount);

                    senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators.Add(senderIndicator);
                }
            }

            outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.SenderStorage.Id, waybill.Sender.Id, 
                expenditureWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators);

            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.SenderStorage.Id, waybill.Sender.Id,
                expenditureWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderOutgoingAcceptedFromExactArticleAvailabilityIndicators);
        }
        #endregion

        #region Отгрузка / отмена отгрузки

        /// <summary>
        /// Отгрузка накладной реализации
        /// </summary>
        /// <param name="waybill">Накладная реализации</param>
        public void ExpenditureWaybillShipped(ExpenditureWaybill waybill)
        {
            // Пересчет показателей проведенного исходящего и точного наличия
            var senderOutgoingAcceptedFromExactArticleAvailabilityIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();
            var exactArticleAvailabilityIndicators = new List<ExactArticleAvailabilityIndicator>();

            foreach (var row in waybill.Rows)
            {
                // уменьшаем исходящее проведенное наличие отправителя
                var senderIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(waybill.ShippingDate.Value, waybill.SenderStorage.Id,
                    waybill.Sender.Id, row.Article.Id, row.As<ExpenditureWaybillRow>().ReceiptWaybillRow.Id,
                    row.As<ExpenditureWaybillRow>().ReceiptWaybillRow.PurchaseCost, -row.SellingCount);

                senderOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(senderIndicator);

                // уменьшаем точное наличие отправителя
                var senderExactIndicator = new ExactArticleAvailabilityIndicator(waybill.ShippingDate.Value, waybill.SenderStorage.Id,
                    waybill.Sender.Id, row.As<ExpenditureWaybillRow>().Article.Id,
                    row.As<ExpenditureWaybillRow>().ReceiptWaybillRow.Id, row.As<ExpenditureWaybillRow>().ReceiptWaybillRow.PurchaseCost, -row.SellingCount);

                exactArticleAvailabilityIndicators.Add(senderExactIndicator);
            }
            
            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.SenderStorage.Id, waybill.Sender.Id, 
                expenditureWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderOutgoingAcceptedFromExactArticleAvailabilityIndicators);
            exactArticleAvailabilityIndicatorService.Update(waybill.SenderStorage.Id, waybill.Sender.Id,
                expenditureWaybillRepository.GetArticleBatchesSubquery(waybill.Id), exactArticleAvailabilityIndicators);
        }

        /// <summary>
        /// Отгрузка накладной реализации отменена
        /// </summary>
        /// <param name="waybill">Накладная реализации</param>
        public void ExpenditureWaybillShippingCanceled(ExpenditureWaybill waybill)
        {
            // Пересчет показателей проведенного исходящего и точного наличия
            var senderOutgoingAcceptedFromExactArticleAvailabilityIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();
            var exactArticleAvailabilityIndicators = new List<ExactArticleAvailabilityIndicator>();

            foreach (var row in waybill.Rows)
            {
                // увеличиваем исходящее проведенное наличие отправителя
                var senderIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(waybill.ShippingDate.Value, waybill.SenderStorage.Id,
                    waybill.Sender.Id, row.Article.Id, row.As<ExpenditureWaybillRow>().ReceiptWaybillRow.Id,
                    row.As<ExpenditureWaybillRow>().ReceiptWaybillRow.PurchaseCost, row.SellingCount);

                senderOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(senderIndicator);

                // увеличиваем точное наличие отправителя
                var senderExactIndicator = new ExactArticleAvailabilityIndicator(waybill.ShippingDate.Value, waybill.SenderStorage.Id,
                    waybill.Sender.Id, row.As<ExpenditureWaybillRow>().Article.Id,
                    row.As<ExpenditureWaybillRow>().ReceiptWaybillRow.Id, row.As<ExpenditureWaybillRow>().ReceiptWaybillRow.PurchaseCost, row.SellingCount);

                exactArticleAvailabilityIndicators.Add(senderExactIndicator);
            }

            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.SenderStorage.Id, waybill.Sender.Id, 
                expenditureWaybillRepository.GetArticleBatchesSubquery(waybill.Id), senderOutgoingAcceptedFromExactArticleAvailabilityIndicators);

            exactArticleAvailabilityIndicatorService.Update(waybill.SenderStorage.Id, waybill.Sender.Id, expenditureWaybillRepository.GetArticleBatchesSubquery(waybill.Id), 
                exactArticleAvailabilityIndicators);
        }

        #endregion

        #endregion

        #region Возврат товаров от клиента

        #region Проводка / Отмена проводки
        
        /// <summary>
        /// Возврат товара проведен
        /// </summary>
        /// <param name="waybill">Накладная возврата товара</param>
        public void ReturnFromClientWaybillAccepted(ReturnFromClientWaybill waybill)
        {
            // Пересчет показателей входящего проведенного наличия

            // увеличение показателя входящего проведенного наличия
            var recipientIncomingAcceptedArticleAvailabilityIndicators = new List<IncomingAcceptedArticleAvailabilityIndicator>();

            foreach (var row in GroupWaybillRows(waybill.Rows))
            {
                var recipientIndicator = new IncomingAcceptedArticleAvailabilityIndicator(waybill.AcceptanceDate.Value, waybill.RecipientStorage.Id,
                    waybill.Recipient.Id, row.Article.Id, row.ReceiptWaybillRow.Id, row.PurchaseCost, row.ReturnCount);

                recipientIncomingAcceptedArticleAvailabilityIndicators.Add(recipientIndicator);
            }
            
            incomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.RecipientStorage.Id, waybill.Recipient.Id, 
                returnFromClientWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientIncomingAcceptedArticleAvailabilityIndicators);
        }

        /// <summary>
        /// Проводка возврата товара отменена
        /// </summary>
        /// <param name="waybill">Накладная возврата товара</param>
        public void ReturnFromClientWaybillAcceptanceCanceled(ReturnFromClientWaybill waybill)
        {
            // Пересчет показателей входящего проведенного наличия
            var acceptanceDate = waybill.AcceptanceDate.Value;

            // уменьшение показателя входящего проведенного наличия
            var recipientIncomingAcceptedArticleAvailabilityIndicators = new List<IncomingAcceptedArticleAvailabilityIndicator>();

            foreach (var row in GroupWaybillRows(waybill.Rows))
            {
                var recipientIndicator = new IncomingAcceptedArticleAvailabilityIndicator(acceptanceDate, waybill.RecipientStorage.Id,
                    waybill.Recipient.Id, row.Article.Id, row.ReceiptWaybillRow.Id, row.PurchaseCost, -row.ReturnCount);

                recipientIncomingAcceptedArticleAvailabilityIndicators.Add(recipientIndicator);
            }
            
            incomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.RecipientStorage.Id, waybill.Recipient.Id, 
                returnFromClientWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientIncomingAcceptedArticleAvailabilityIndicators);
        }

        #endregion

        #region Приемка / Отмена приемки

        /// <summary>
        /// Накладная возврата товара принята
        /// </summary>
        /// <param name="waybill">Накладная возврата товара</param>
        public void ReturnFromClientWaybillReceipted(ReturnFromClientWaybill waybill)
        {
            // Пересчет показателей входящего проведенного и точного наличия
            var recipientIncomingAcceptedArticleAvailabilityIndicators = new List<IncomingAcceptedArticleAvailabilityIndicator>();
            var exactArticleAvailabilityIndicators = new List<ExactArticleAvailabilityIndicator>();

            var recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();
            var recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators = new List<OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator>();

            foreach (var row in GroupWaybillRows(waybill.Rows.Where(x => x.SaleWaybillRow.Is<ExpenditureWaybillRow>())))
            {
                // уменьшение показателей проведенного входящего наличия
                var recipientIndicator = new IncomingAcceptedArticleAvailabilityIndicator(waybill.ReceiptDate.Value, waybill.RecipientStorage.Id,
                    waybill.Recipient.Id, row.Article.Id, row.ReceiptWaybillRow.Id, row.PurchaseCost, -row.ReturnCount);

                recipientIncomingAcceptedArticleAvailabilityIndicators.Add(recipientIndicator);

                // увеличение показателя точного наличия
                var recipientExactIndicator = new ExactArticleAvailabilityIndicator(waybill.ReceiptDate.Value, waybill.RecipientStorage.Id,
                    waybill.Recipient.Id, row.Article.Id, row.ReceiptWaybillRow.Id, row.PurchaseCost, row.ReturnCount);

                exactArticleAvailabilityIndicators.Add(recipientExactIndicator);

                if (row.AcceptedCount > 0)
                {
                    // уменьшаем исходящее проведенное (из входящего проведенного) наличие получателя
                    var recipientOutgoingAcceptedFromIncomingAcceptedIndicator = new OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator(
                        waybill.ReceiptDate.Value, waybill.RecipientStorage.Id, waybill.Recipient.Id, row.Article.Id, 
                        row.ReceiptWaybillRow.Id, row.PurchaseCost, -row.AcceptedCount);

                    recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators.Add(recipientOutgoingAcceptedFromIncomingAcceptedIndicator);

                    // увеличиваем исходящее проведенное (из точного) наличие получателя
                    var recipientOutgoingAcceptedFromExactIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(
                        waybill.ReceiptDate.Value, waybill.RecipientStorage.Id, waybill.Recipient.Id,
                        row.Article.Id, row.ReceiptWaybillRow.Id, row.PurchaseCost, row.AcceptedCount);

                    recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(recipientOutgoingAcceptedFromExactIndicator);
                }
            }

            foreach (var row in waybill.Rows.Where(x => !x.SaleWaybillRow.Is<ExpenditureWaybillRow>()))
            {
                throw new Exception("Не реализовано для накладных реализации, не являющихся накладными реализации товаров.");
            }

            incomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.RecipientStorage.Id, waybill.Recipient.Id,
                returnFromClientWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientIncomingAcceptedArticleAvailabilityIndicators);

            exactArticleAvailabilityIndicatorService.Update(waybill.RecipientStorage.Id, waybill.Recipient.Id, 
                returnFromClientWaybillRepository.GetArticleBatchesSubquery(waybill.Id), exactArticleAvailabilityIndicators);

            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.RecipientStorage.Id, waybill.Recipient.Id,
                returnFromClientWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators);

            outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.RecipientStorage.Id, waybill.Recipient.Id, 
                returnFromClientWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators);
        }

        /// <summary>
        /// Приемка накладной возврата отменена
        /// </summary>
        /// <param name="waybill">Накладная возврата товара</param>
        /// <param name="outgoingWaybillRowDict">словарь исходящих позиций</param>
        public void ReturnFromClientWaybillReceiptCanceled(ReturnFromClientWaybill waybill,
            DynamicDictionary<Guid, Dictionary<OutgoingWaybillRow, decimal>> outgoingWaybillRowDict)
        {
            // Пересчет показателей входящего проведенного и точного наличия
            var recipientIncomingAcceptedArticleAvailabilityIndicators = new List<IncomingAcceptedArticleAvailabilityIndicator>();
            var exactArticleAvailabilityIndicators = new List<ExactArticleAvailabilityIndicator>();

            var recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators = new List<OutgoingAcceptedFromExactArticleAvailabilityIndicator>();
            var recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators = new List<OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator>();

            foreach (var row in GroupWaybillRows(waybill.Rows.Where(x => x.SaleWaybillRow.Is<ExpenditureWaybillRow>())))
            {
                // увеличение показателей проведенного входящего наличия
                var recipientIndicator = new IncomingAcceptedArticleAvailabilityIndicator(waybill.ReceiptDate.Value, waybill.RecipientStorage.Id,
                    waybill.Recipient.Id, row.Article.Id, row.ReceiptWaybillRow.Id, row.PurchaseCost, row.ReturnCount);

                recipientIncomingAcceptedArticleAvailabilityIndicators.Add(recipientIndicator);

                // уменьшене показателя точного наличия
                var recipientExactIndicator = new ExactArticleAvailabilityIndicator(waybill.ReceiptDate.Value, waybill.RecipientStorage.Id,
                    waybill.Recipient.Id, row.Article.Id, row.ReceiptWaybillRow.Id, row.PurchaseCost, -row.ReturnCount);

                exactArticleAvailabilityIndicators.Add(recipientExactIndicator);
            }
            // деалем цикл по всем позициям возврата, т.к. каждая из позиций может быть источником для исходящей
            foreach(var row  in waybill.Rows.Where(x => x.SaleWaybillRow.Is<ExpenditureWaybillRow>()))
            {
                if (row.AcceptedCount > 0)
                {
                    // Цикл по исходящим позициям из позиции накладной возврата
                    foreach (var outgoingRow in outgoingWaybillRowDict[row.Id].Where(x => x.Key.AcceptanceDate != null))
                    {
                        decimal articleCount = outgoingRow.Value;
                        DateTime acceptanceDate = outgoingRow.Key.AcceptanceDate.Value;

                        // Определяем дату, от которой нужно изменить показатель (максимум из даты проводки исходящей позиции и датой приемки возврата)
                        var indDate = acceptanceDate > waybill.ReceiptDate.Value ? acceptanceDate : waybill.ReceiptDate.Value;

                        // увеличиваем исходящее проведенное (из входящего проведенного) наличие получателя
                        var recipientOutgoingAcceptedFromIncomingAcceptedIndicator = new OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator(
                            indDate, waybill.RecipientStorage.Id, waybill.Recipient.Id,
                            row.Article.Id, row.ReceiptWaybillRow.Id, row.PurchaseCost, articleCount);

                        recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators.Add(recipientOutgoingAcceptedFromIncomingAcceptedIndicator);

                        // уменьшаем исходящее проведенное (из точного) наличие получателя
                        var recipientOutgoingAcceptedFromExactIndicator = new OutgoingAcceptedFromExactArticleAvailabilityIndicator(
                            indDate, waybill.RecipientStorage.Id, waybill.Recipient.Id,
                            row.Article.Id, row.ReceiptWaybillRow.Id, row.PurchaseCost, -articleCount);

                        recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators.Add(recipientOutgoingAcceptedFromExactIndicator);
                    }
                }
            }

            foreach (var row in waybill.Rows.Where(x => !x.SaleWaybillRow.Is<ExpenditureWaybillRow>()))
            {
                throw new Exception("Не реализовано для накладных реализации, не являющихся накладными реализации товаров.");
            }

            incomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.RecipientStorage.Id, waybill.Recipient.Id, 
                returnFromClientWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientIncomingAcceptedArticleAvailabilityIndicators);

            exactArticleAvailabilityIndicatorService.Update(waybill.RecipientStorage.Id, waybill.Recipient.Id, 
                returnFromClientWaybillRepository.GetArticleBatchesSubquery(waybill.Id), exactArticleAvailabilityIndicators);

            outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.Update(waybill.RecipientStorage.Id, waybill.Recipient.Id,
                       returnFromClientWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicators);

            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.Update(waybill.RecipientStorage.Id, waybill.Recipient.Id,
                returnFromClientWaybillRepository.GetArticleBatchesSubquery(waybill.Id), recipientOutgoingAcceptedFromExactArticleAvailabilityIndicators);
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Группировка строк накладной, чтобы для тех позиций, которые сделаны по одной партии, но из разных реализаций, записывалось не несколько индикаторов, а один, 
        /// но с суммой возвращаемых количеств по этим позициям
        /// </summary>
        private IEnumerable<dynamic> GroupWaybillRows(IEnumerable<ReturnFromClientWaybillRow> rows)
        {
            return rows.GroupBy(x => new { x.Article, x.ReceiptWaybillRow, x.PurchaseCost },
                (key, group) => new
                {
                    Article = key.Article,
                    ReceiptWaybillRow = key.ReceiptWaybillRow,
                    PurchaseCost = key.PurchaseCost,
                    ReturnCount = group.Sum(x => x.ReturnCount),
                    AcceptedCount = group.Sum(x => x.AcceptedCount)
                });
        }

        #endregion

        #endregion

        #endregion

        #endregion
    }
}