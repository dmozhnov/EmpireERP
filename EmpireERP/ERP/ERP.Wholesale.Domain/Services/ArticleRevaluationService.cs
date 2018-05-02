using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    /// <summary>
    /// Служба переоценок
    /// </summary>
    public class ArticleRevaluationService : IArticleRevaluationService
    {
        #region Поля

        private readonly IAcceptedArticleRevaluationIndicatorRepository acceptedArticleRevaluationIndicatorRepository;
        private readonly IExactArticleRevaluationIndicatorRepository exactArticleRevaluationIndicatorRepository;
        
        private readonly IAccountingPriceListRepository accountingPriceListRepository;
        private readonly IStorageRepository storageRepository;
        private readonly IAccountingPriceListWaybillTakingRepository accountingPriceListWaybillTakingRepository;
        private readonly IArticleAccountingPriceIndicatorRepository articleAccountingPriceIndicatorRepository;

        private readonly IReceiptWaybillRepository receiptWaybillRepository;
        private readonly IMovementWaybillRepository movementWaybillRepository;
        private readonly IChangeOwnerWaybillRepository changeOwnerWaybillRepository;
        private readonly IWriteoffWaybillRepository writeoffWaybillRepository;
        private readonly IExpenditureWaybillRepository expenditureWaybillRepository;
        private readonly IReturnFromClientWaybillRepository returnFromClientWaybillRepository;

        private readonly IWaybillRowArticleMovementRepository waybillRowArticleMovementRepository;

        private readonly IAcceptedArticleRevaluationIndicatorService acceptedArticleRevaluationIndicatorService;
        private readonly IExactArticleRevaluationIndicatorService exactArticleRevaluationIndicatorService;

        private readonly IIncomingWaybillRowService incomingWaybillRowService;
        private readonly IOutgoingWaybillRowService outgoingWaybillRowService;

        private readonly IAccountingPriceListWaybillTakingService accountingPriceListWaybillTakingService;

        private readonly IArticlePriceService articlePriceService;

        #endregion

        #region Конструкторы

        public ArticleRevaluationService(IAcceptedArticleRevaluationIndicatorRepository acceptedArticleRevaluationIndicatorRepository,
            IExactArticleRevaluationIndicatorRepository exactArticleRevaluationIndicatorRepository,
            IAccountingPriceListRepository accountingPriceListRepository, IStorageRepository storageRepository, 
            IAccountingPriceListWaybillTakingRepository accountingPriceListWaybillTakingRepository,
            IArticleAccountingPriceIndicatorRepository articleAccountingPriceIndicatorRepository, IReceiptWaybillRepository receiptWaybillRepository,
            IMovementWaybillRepository movementWaybillRepository, IChangeOwnerWaybillRepository changeOwnerWaybillRepository,
            IWriteoffWaybillRepository writeoffWaybillRepository, IExpenditureWaybillRepository expenditureWaybillRepository,
            IReturnFromClientWaybillRepository returnFromClientWaybillRepository, IWaybillRowArticleMovementRepository waybillRowArticleMovementRepository,           
            IAcceptedArticleRevaluationIndicatorService acceptedArticleRevaluationIndicatorService,
            IExactArticleRevaluationIndicatorService exactArticleRevaluationIndicatorService,
            IIncomingWaybillRowService incomingWaybillRowService, IOutgoingWaybillRowService outgoingWaybillRowService,
            IAccountingPriceListWaybillTakingService accountingPriceListWaybillTakingService, IArticlePriceService articlePriceService)
        {
            this.acceptedArticleRevaluationIndicatorRepository = acceptedArticleRevaluationIndicatorRepository;
            this.exactArticleRevaluationIndicatorRepository = exactArticleRevaluationIndicatorRepository;

            this.accountingPriceListRepository = accountingPriceListRepository;
            this.storageRepository = storageRepository;
            this.accountingPriceListWaybillTakingRepository = accountingPriceListWaybillTakingRepository;
            this.articleAccountingPriceIndicatorRepository = articleAccountingPriceIndicatorRepository;

            this.receiptWaybillRepository = receiptWaybillRepository;
            this.movementWaybillRepository = movementWaybillRepository;
            this.changeOwnerWaybillRepository = changeOwnerWaybillRepository;
            this.writeoffWaybillRepository = writeoffWaybillRepository;
            this.expenditureWaybillRepository = expenditureWaybillRepository;
            this.returnFromClientWaybillRepository = returnFromClientWaybillRepository;
            this.waybillRowArticleMovementRepository = waybillRowArticleMovementRepository;

            this.acceptedArticleRevaluationIndicatorService = acceptedArticleRevaluationIndicatorService;
            this.exactArticleRevaluationIndicatorService = exactArticleRevaluationIndicatorService;

            this.incomingWaybillRowService = incomingWaybillRowService;
            this.outgoingWaybillRowService = outgoingWaybillRowService;

            this.accountingPriceListWaybillTakingService = accountingPriceListWaybillTakingService;
            this.articlePriceService = articlePriceService;
        }

        #endregion

        #region Методы

        #region Обработчики событий, изменяющих переоценку

        #region Поиск и расчет переоценки для РЦ, вступивших в силу или завершивших действие
        
        /// <summary>
        /// Поиск и расчет переоценки для РЦ, вступивших в силу или завершивших действие
        /// </summary>
        public void CheckAccountingPriceListWithoutCalculatedRevaluation(DateTime currentDateTime)
        {
            // список РЦ, по которым не расчитана переоценка
            var priceListsWithoutCalculatedRevaluation = accountingPriceListRepository.GetAccountingPriceListsWithoutCalculatedRevaluation(currentDateTime);

            // список вступивших в силу РЦ, по которым не расчитана переоценка на начало периода действия
            var accountingPriceListOnStart = priceListsWithoutCalculatedRevaluation.Where(x => !x.IsRevaluationOnStartCalculated);

            // список завершивших действие РЦ, по которым не расчитана переоценка на конец периода действия
            var accountingPriceListOnEnd = priceListsWithoutCalculatedRevaluation.Where(x => !x.IsRevaluationOnEndCalculated && x.EndDate.HasValue && x.EndDate.Value < currentDateTime);

            // обработка вступивших в силу РЦ
            foreach (var priceList in accountingPriceListOnStart)
            {
                AccountingPriceListCameIntoEffect(priceList);

                accountingPriceListRepository.Flush();
            }

            // обработка завершивших действие РЦ
            foreach (var priceList in accountingPriceListOnEnd)
            {
                AccountingPriceListTerminated(priceList);

                accountingPriceListRepository.Flush();
            }
        }

        #endregion

        #region Проводка РЦ
        
        /// <summary>
        /// Проводка РЦ
        /// </summary>
        /// <param name="accountingPriceList">РЦ</param>
        public void AccountingPriceListAccepted(AccountingPriceList accountingPriceList, DateTime currentDateTime)
        {
            // если дата вступления в силу РЦ равна дате начала его действия - сразу выполняем действия при вступлении РЦ в силу
            if (accountingPriceList.StartDate == accountingPriceList.AcceptanceDate)
            {
                AccountingPriceListCameIntoEffect(accountingPriceList);
            }
        } 
        #endregion

        #region Отмена проводки РЦ

        /// <summary>
        /// Отмена проводки РЦ
        /// </summary>
        /// <param name="accountingPriceList">РЦ</param>
        public void AccountingPriceListAcceptanceCancelled(AccountingPriceList accountingPriceList, DateTime currentDateTime)
        {
            // ПОКА РЕШЕНО НЕ ДЕЛАТЬ !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            
            //// если ни переоценка по началу периода действия РЦ, ни по концу не произошла - выходим
            //if (!accountingPriceList.IsRevaluationOnStartCalculated && !accountingPriceList.IsRevaluationOnEndCalculated) return;

            //// подзапрос для МХ РЦ
            //var storageIdsSubQuery = accountingPriceListRepository.GetStoragesSubquery(accountingPriceList.Id);

            //// подзапрос для товаров РЦ
            //var articleIdsSubQuery = accountingPriceListRepository.GetArticlesSubquery(accountingPriceList.Id);

            //// получение списка РЦ, которые будут использоваться при отмене переоценки
            //var intersectingPriceLists = accountingPriceListRepository.GetIntersectingPriceLists(accountingPriceList, currentDateTime);

            //// показатели точной переоценки, начиная с даты вступления в силу отменяемого РЦ
            //var exactArticleRevaluationIndicators = exactArticleRevaluationIndicatorRepository.GetFrom(accountingPriceList.StartDate,
            //    storageIdsSubQuery);

            //// показатели проведенной переоценки, начиная с даты вступления в силу отменяемого РЦ
            //var acceptedArticleRevaluationIndicators = acceptedArticleRevaluationIndicatorRepository.GetFrom(accountingPriceList.StartDate,
            //    storageIdsSubQuery);



            //// Из полученного списка РЦ выделяем 3 группы РЦ:
            //// 1. РЦ, которые будут являться "основанием" для переоценки по отменяемому РЦ (т.е. по каждому МХ и товару берутся РЦ, 
            //// которые действовали за секунду до начала действия отменяемого РЦ и будут действовать через секунду после окончания его действия)
            //// 2. РЦ, по которым необходимо пересчитать переоценки (РЦ, основанием для переоценки по которым является отменяемый РЦ)
            //// 3. РЦ, переоценки по которым открываются после отмены проводки данного РЦ (т.е. переоценки по которым не были расчитаны из-за 
            //// наличия отменяемого РЦ).



            //foreach (var storage in accountingPriceList.Storages)
            //{
            //    foreach (var article in accountingPriceList.ArticlePrices.Select(x => x.Article))
            //    {
            //        var priceListsByStorageAndArticle = intersectingPriceLists.Where(x => x.Storages.Contains(storage) &&
            //            x.ArticlePrices.Select(y => y.Article).Contains(article));

            //        var maxEndDate = accountingPriceList.StartDate;

            //        foreach (var apl in priceListsByStorageAndArticle)
            //        {
            //            if (apl.StartDate > maxEndDate && apl.IsRevaluationOnStartCalculated)
            //            {

            //            }
            //        }
            //    }
            //}

            //// 1. находим основания для переоценки 

            //// УЦ (РЦ) до начала действия
            //var pricesBeforeOneSecond = articlePriceService.GetAccountingPrice(accountingPriceList, accountingPriceList.StartDate.AddSeconds(-1));

            //var pricesAfterOneSecond = new DynamicDictionary<short, DynamicDictionary<int, decimal?>>();

            //// если дата завершения действия отменяемого РЦ установлены, то получаем УЦ (РЦ) после даты окончания его действия
            //if (accountingPriceList.EndDate.HasValue)
            //{
            //    pricesAfterOneSecond = articlePriceService.GetAccountingPrice(accountingPriceList, accountingPriceList.EndDate.Value.AddSeconds(1));
            //}

            //// если уже случилась переоценка на начало периода действия РЦ
            //if (accountingPriceList.IsRevaluationOnStartCalculated)
            //{
            //    foreach (var item in intersectingPriceLists)
            //    {

            //    }
            //}

            //// если уже случилась переоценка на конец периода действия РЦ
            //if (accountingPriceList.IsRevaluationOnStartCalculated)
            //{

            //}
        } 
        #endregion
        
        #region Вступление РЦ в действие

        /// <summary>
        /// РЦ вступил в силу
        /// </summary>
        /// <param name="accountingPriceList"></param>
        private void AccountingPriceListCameIntoEffect(AccountingPriceList accountingPriceList)
        {
            // подзапрос для МХ РЦ
            var storageIdsSubQuery = accountingPriceListRepository.GetStoragesSubquery(accountingPriceList.Id);            
            
            // подзапрос для товаров РЦ
            var articleIdsSubQuery = accountingPriceListRepository.GetArticlesSubquery(accountingPriceList.Id);

            // получение списка входящих принятых на склад без расхождений позиций накладных на дату начала действия РЦ
            var incomingReceiptedExcludingNewWaybillRows = incomingWaybillRowService
                .GetReceiptedWaybillRows(storageIdsSubQuery, articleIdsSubQuery, accountingPriceList.StartDate);

            // получение списка входящих проведенных, но не принятых на склад позиций накладных на дату начала действия РЦ
            var incomingAcceptedAndNotReceiptedWaybillRows = incomingWaybillRowService
                .GetAcceptedAndNotReceiptedWaybillRows(storageIdsSubQuery, articleIdsSubQuery, accountingPriceList.StartDate);

            // получение списка принятых с расхождениями позиций входящих накладных на дату начала действия РЦ
            var incomingReceiptedWithDivergencesExcludingNewWaybillRows = incomingWaybillRowService
                .GetReceiptedWithDivergencesNotApprovedExcludingNewWaybillRows(storageIdsSubQuery, articleIdsSubQuery, accountingPriceList.StartDate);

            // получение списка исходящих проведенных, но не завершенных (отгруженных или принятых) позиций накладных на дату начала действия РЦ
            var outgoingAcceptedAndNotFinalizedWaybillRows = outgoingWaybillRowService
                .GetAcceptedAndNotFinalizedWaybillRows(storageIdsSubQuery, articleIdsSubQuery, accountingPriceList.StartDate);

            // cоздание связи между позициями РЦ и позициями входящих принятых накладных по точному наличию (Дельта 0)
            accountingPriceListWaybillTakingService.CreateTakingFromExactArticleAvailability(incomingReceiptedExcludingNewWaybillRows, accountingPriceList, true, accountingPriceList.StartDate);
        
            // cоздание связей между позициями РЦ и позициями входящих проведенных, но не принятых накладных (Дельта 1+)
            accountingPriceListWaybillTakingService.CreateTakingFromIncomingAcceptedArticleAvailability(incomingAcceptedAndNotReceiptedWaybillRows, accountingPriceList, true, accountingPriceList.StartDate);

            // создание связей между позициями РЦ и принятыми с расхождениями позициями входящих накладных
            accountingPriceListWaybillTakingService.CreateTakingFromIncomingDivergenceArticleAvailability(incomingReceiptedWithDivergencesExcludingNewWaybillRows, accountingPriceList, true, accountingPriceList.StartDate);

            // создание связей между позициями РЦ и позициями исходящих проведенных, но не отгруженных/не принятых получателем накладных (Дельта 1-)
            accountingPriceListWaybillTakingService.CreateTakingFromOutgoingAcceptedArticleAvailability(outgoingAcceptedAndNotFinalizedWaybillRows, accountingPriceList, true, accountingPriceList.StartDate);

            // получение действующих учетных цен (за секунду до начала действия данного РЦ)
            var accountingPrices = articlePriceService.GetAccountingPrice(accountingPriceList, accountingPriceList.StartDate.AddSeconds(-1));


            // пересчет показателей точной и проведенной переоценок
            var exactArticleRevaluationIndicators = new List<ExactArticleRevaluationIndicator>();
            var acceptedArticleRevaluationIndicators = new List<AcceptedArticleRevaluationIndicator>();

            // список собственных организаций, по которым будет производиться переоценка
            var accountOrganizationList = incomingReceiptedExcludingNewWaybillRows.Select(x => x.Recipient).Distinct()
                .Concat(incomingAcceptedAndNotReceiptedWaybillRows.Select(x => x.Recipient).Distinct())
                .Concat(outgoingAcceptedAndNotFinalizedWaybillRows.Select(x => x.Sender).Distinct()).Distinct();

            // цикл по МХ
            foreach (var storage in accountingPriceList.Storages)
	        {
		        // получаем список позиций накладных по конкретному МХ
                var incomingReceiptedRowsForStorage = incomingReceiptedExcludingNewWaybillRows.Where(x => x.RecipientStorage == storage);
                var incomingAcceptedAndNotReceiptedRowsForStorage = incomingAcceptedAndNotReceiptedWaybillRows.Where(x => x.RecipientStorage == storage);
                var outgoingAcceptedAndNotFinalizedRowsForStorage = outgoingAcceptedAndNotFinalizedWaybillRows.Where(x => x.SenderStorage == storage);
                
                // цикл по собственным организациям
                foreach (var accountOrganization in accountOrganizationList)
	            {
		            // получаем список позиций накладных по конкретным МХ и собственной организации
                    var incomingReceiptedRowsForStorageAndOrganization = incomingReceiptedRowsForStorage.Where(x => x.Recipient == accountOrganization);
                    var incomingAcceptedAndNotReceiptedRowsForStorageAndOrganization = incomingAcceptedAndNotReceiptedRowsForStorage.Where(x => x.Recipient == accountOrganization);
                    var outgoingAcceptedAndNotFinalizedRowsForStorageAndOrganization = outgoingAcceptedAndNotFinalizedRowsForStorage.Where(x => x.Sender == accountOrganization);
                    
                    // суммы точной и проведенной переоценок по указанным МХ и организации
                    decimal exactRevaluationSum = 0M, acceptedRevaluationSum = 0M;

                    // цикл по товарам
                    foreach (var article in accountingPriceList.ArticlePrices.Select(x => x.Article).Distinct())
	                {
                        // старая (действующая) УЦ на товар
                        var oldAccountingPrice = accountingPrices[storage.Id][article.Id] ?? 0M;

                        // новая УЦ из текущего РЦ
                        var newAccountingPrice = accountingPriceList.ArticlePrices.Where(x => x.Article == article).FirstOrDefault().AccountingPrice;

                        // кол-во товара в точном наличии
                        var exactAvailableCount = incomingReceiptedRowsForStorageAndOrganization.Where(x => x.Batch.Article == article)
                            .Sum(x => x.AvailableInStorageCount);

                        // кол-во товара в ожидании
                        var incomingAcceptedAvailableCount = incomingAcceptedAndNotReceiptedRowsForStorageAndOrganization.Where(x => x.Batch.Article == article)
                            .Sum(x => x.PendingCount);

                        // кол-во товара в резерве
                        var outgoingAcceptedAvailableCount = outgoingAcceptedAndNotFinalizedRowsForStorageAndOrganization.Where(x => x.Batch.Article == article)
                            .Sum(x => x.Count);

                        // увеличиваем точную переоценку
                        exactRevaluationSum += Math.Round((newAccountingPrice - oldAccountingPrice) * exactAvailableCount, 2);

                        // увеличиваем проведенную переоценку
                        acceptedRevaluationSum += Math.Round((newAccountingPrice - oldAccountingPrice) * (exactAvailableCount + incomingAcceptedAvailableCount - outgoingAcceptedAvailableCount), 2);
	                }

                    // формируем показатель точной переоценки
                    if (exactRevaluationSum != 0)
                    {
                        var ind = new ExactArticleRevaluationIndicator(accountingPriceList.StartDate, storage.Id, accountOrganization.Id, exactRevaluationSum);

                        exactArticleRevaluationIndicators.Add(ind);
                    }

                    // формируем показатель проведенной переоценки
                    if (acceptedRevaluationSum != 0)
                    {
                        var ind = new AcceptedArticleRevaluationIndicator(accountingPriceList.StartDate, storage.Id, accountOrganization.Id, acceptedRevaluationSum);

                        acceptedArticleRevaluationIndicators.Add(ind);
                    }
	            }
	        }

            // пересчет показателей точной переоценки
            exactArticleRevaluationIndicatorService.Update(accountingPriceList.StartDate, storageIdsSubQuery, exactArticleRevaluationIndicators);

            // пересчет показателей проведенной переоценки
            acceptedArticleRevaluationIndicatorService.Update(accountingPriceList.StartDate, storageIdsSubQuery, acceptedArticleRevaluationIndicators);

            // установка признака выполнения переоценки на начало действия РЦ
            accountingPriceList.IsRevaluationOnStartCalculated = true;
        }

        #endregion

        #region Завершение действия РЦ

        /// <summary>
        /// РЦ завершил действие
        /// </summary>
        /// <param name="accountingPriceList">РЦ</param>
        private void AccountingPriceListTerminated(AccountingPriceList accountingPriceList)
        {
            ValidationUtils.Assert(accountingPriceList.EndDate != null, "Данный реестр цен не имеет даты завершения действия.");
            
            // подзапрос для МХ РЦ
            var storageIdsSubQuery = accountingPriceListRepository.GetStoragesSubquery(accountingPriceList.Id);

            // подзапрос для товаров РЦ
            var articleIdsSubQuery = accountingPriceListRepository.GetArticlesSubquery(accountingPriceList.Id);
            
            // получение списка ВСЕХ входящих принятых на склад позиций накладных на дату окончания действия РЦ
            var incomingReceiptedExcludingNewWaybillRowsAll = incomingWaybillRowService
                .GetReceiptedWaybillRows(storageIdsSubQuery, articleIdsSubQuery, accountingPriceList.EndDate.Value);

            // получение списка ВСЕХ входящих проведенных, но не принятых на склад позиций накладных на дату окончания действия РЦ
            var incomingAcceptedAndNotReceiptedWaybillRowsAll = incomingWaybillRowService
                .GetAcceptedAndNotReceiptedWaybillRows(storageIdsSubQuery, articleIdsSubQuery, accountingPriceList.EndDate.Value);

            // получение списка ВСЕХ принятых с расхождениями позиций входящих накладных на дату окончания действия РЦ
            var incomingReceiptedWithDivergencesExcludingNewWaybillRowsAll = incomingWaybillRowService
                .GetReceiptedWithDivergencesNotApprovedExcludingNewWaybillRows(storageIdsSubQuery, articleIdsSubQuery, accountingPriceList.EndDate.Value);

            // получение списка ВСЕХ исходящих проведенных, но не завершенных (отгруженных или принятых) позиций накладных на дату окончания действия РЦ
            var outgoingAcceptedAndNotFinalizedWaybillRowsAll = outgoingWaybillRowService
                .GetAcceptedAndNotFinalizedWaybillRows(storageIdsSubQuery, articleIdsSubQuery, accountingPriceList.EndDate.Value);

            // поиск РЦ, перекрывающих дату завершения действия текущего РЦ (priceList) 
            var overlappingPriceLists = accountingPriceListRepository.GetOverlappingPriceLists(storageIdsSubQuery, articleIdsSubQuery, accountingPriceList.StartDate,
                accountingPriceList.EndDate.Value);


            // список входящих принятых на склад позиций накладных, по МХ и товару, из которых текущий РЦ не перекрывается другими РЦ на 
            // дату завершения его действия
            var incomingReceiptedExcludingNewWaybillRows = incomingReceiptedExcludingNewWaybillRowsAll.Where(x => !overlappingPriceLists
                .Any(y => y.Storages.Contains(x.RecipientStorage) && y.ArticlePrices.Select(a => a.Article).Contains(x.Batch.Article)));

            // список входящих проведенных, но не принятых на склад позиций накладных, по МХ и товару, из которых текущий РЦ не перекрывается 
            // другими РЦ на дату завершения его действия
            var incomingAcceptedAndNotReceiptedWaybillRows = incomingAcceptedAndNotReceiptedWaybillRowsAll.Where(x => !overlappingPriceLists
                .Any(y => y.Storages.Contains(x.RecipientStorage) && y.ArticlePrices.Select(a => a.Article).Contains(x.Batch.Article)));

            // список входящих принятых с расхождениями позиций накладных, по МХ и товару, из которых текущий РЦ не перекрывается 
            // другими РЦ на дату завершения его действия
            var incomingReceiptedWithDivergencesExcludingNewWaybillRows = incomingReceiptedWithDivergencesExcludingNewWaybillRowsAll.Where(x => !overlappingPriceLists
                .Any(y => y.Storages.Contains(x.RecipientStorage) && y.ArticlePrices.Select(a => a.Article).Contains(x.Batch.Article)));
            
            // список исходящих проведенных, но не завершенных (отгруженных или принятых) позиций накладных, по МХ и товару, из которых 
            // текущий РЦ не перекрывается другими РЦ на дату завершения его действия
            var outgoingAcceptedAndNotFinalizedWaybillRows = outgoingAcceptedAndNotFinalizedWaybillRowsAll.Where(x => !overlappingPriceLists
                .Any(y => y.Storages.Contains(x.SenderStorage) && y.ArticlePrices.Select(a => a.Article).Contains(x.Batch.Article)));

            // установка признака перекрытия данной позиции РЦ позицией другого РЦ при переоценке на конец периода действия данного РЦ 
            foreach (var storage in accountingPriceList.Storages)
            {
                foreach (var articleAccountingPrice in accountingPriceList.ArticlePrices)
                {
                    if (overlappingPriceLists.Any(y => y.Storages.Contains(storage) && y.ArticlePrices.Select(a => a.Article).Contains(articleAccountingPrice.Article)))
                    {
                        articleAccountingPrice.IsOverlappedOnEnd = true;
                    }
                }
            }
            
            // cоздание связи между неперекрытыми позициями РЦ и позициями входящих принятых накладных по точному наличию (Дельта 0)
            accountingPriceListWaybillTakingService.CreateTakingFromExactArticleAvailability(incomingReceiptedExcludingNewWaybillRows, accountingPriceList, false, accountingPriceList.EndDate.Value);

            // cоздание связей между неперекрытыми позициями РЦ и позициями входящих проведенных, но не принятых накладных (Дельта 1+)
            accountingPriceListWaybillTakingService.CreateTakingFromIncomingAcceptedArticleAvailability(incomingAcceptedAndNotReceiptedWaybillRows, accountingPriceList, false, accountingPriceList.EndDate.Value);

            // создание связей между позициями РЦ и принятыми с расхождениями позициями входящих накладных
            accountingPriceListWaybillTakingService.CreateTakingFromIncomingDivergenceArticleAvailability(incomingReceiptedWithDivergencesExcludingNewWaybillRows, accountingPriceList, false, accountingPriceList.EndDate.Value);

            // создание связей между неперекрытыми позициями РЦ и позициями исходящих проведенных, но не отгруженных/не принятых получателем накладных (Дельта 1-)
            accountingPriceListWaybillTakingService.CreateTakingFromOutgoingAcceptedArticleAvailability(outgoingAcceptedAndNotFinalizedWaybillRows, accountingPriceList, false, accountingPriceList.EndDate.Value);


            // получение учетных цен, которые будут действовать после завершения действия данного РЦ
            var accountingPrices = articlePriceService.GetAccountingPrice(accountingPriceList, accountingPriceList.EndDate.Value.AddSeconds(1));
            
            // пересчет показателей точной и проведенной переоценок для неперекрытых позиций данного РЦ
            var exactArticleRevaluationIndicators = new List<ExactArticleRevaluationIndicator>();
            var acceptedArticleRevaluationIndicators = new List<AcceptedArticleRevaluationIndicator>();

            // список собственных организаций, по которым будет производиться переоценка
            var accountOrganizationList = incomingReceiptedExcludingNewWaybillRows.Select(x => x.Recipient).Distinct()
                .Concat(incomingAcceptedAndNotReceiptedWaybillRows.Select(x => x.Recipient).Distinct())
                .Concat(outgoingAcceptedAndNotFinalizedWaybillRows.Select(x => x.Sender).Distinct()).Distinct();

            // список товаров, по которым будет производиться переоценка
            var articleList = incomingReceiptedExcludingNewWaybillRows.Select(x => x.Batch.Article).Distinct()
                .Concat(incomingAcceptedAndNotReceiptedWaybillRows.Select(x => x.Batch.Article).Distinct())
                .Concat(outgoingAcceptedAndNotFinalizedWaybillRows.Select(x => x.Batch.Article).Distinct()).Distinct();

            // цикл по МХ
            foreach (var storage in accountingPriceList.Storages)
            {
                // получаем список позиций накладных по конкретному МХ
                var incomingReceiptedRowsForStorage = incomingReceiptedExcludingNewWaybillRows.Where(x => x.RecipientStorage == storage);
                var incomingAcceptedAndNotReceiptedRowsForStorage = incomingAcceptedAndNotReceiptedWaybillRows.Where(x => x.RecipientStorage == storage);
                var outgoingAcceptedAndNotFinalizedRowsForStorage = outgoingAcceptedAndNotFinalizedWaybillRows.Where(x => x.SenderStorage == storage);

                // цикл по собственным организациям
                foreach (var accountOrganization in accountOrganizationList)
                {
                    // получаем список позиций накладных по конкретным МХ и собственной организации
                    var incomingReceiptedRowsForStorageAndOrganization = incomingReceiptedRowsForStorage.Where(x => x.Recipient == accountOrganization);
                    var incomingAcceptedAndNotReceiptedRowsForStorageAndOrganization = incomingAcceptedAndNotReceiptedRowsForStorage.Where(x => x.Recipient == accountOrganization);
                    var outgoingAcceptedAndNotFinalizedRowsForStorageAndOrganization = outgoingAcceptedAndNotFinalizedRowsForStorage.Where(x => x.Sender == accountOrganization);

                    // суммы точной и проведенной переоценок по указанным МХ и организации
                    decimal exactRevaluationSum = 0M, acceptedRevaluationSum = 0M;

                    // цикл по товарам
                    foreach (var article in articleList)
                    {
                        // текущая УЦ на товар (по данному РЦ)
                        var oldAccountingPrice = accountingPriceList.ArticlePrices.Where(x => x.Article == article).FirstOrDefault().AccountingPrice;

                        // новая УЦ после завершения действия текущего РЦ
                        var newAccountingPrice = accountingPrices[storage.Id][article.Id] ?? 0M;

                        // кол-во товара в точном наличии
                        var exactAvailableCount = incomingReceiptedRowsForStorageAndOrganization.Where(x => x.Batch.Article == article)
                            .Sum(x => x.AvailableInStorageCount);

                        // кол-во товара в ожидании
                        var incomingAcceptedAvailableCount = incomingAcceptedAndNotReceiptedRowsForStorageAndOrganization.Where(x => x.Batch.Article == article)
                            .Sum(x => x.PendingCount);

                        // кол-во товара в резерве
                        var outgoingAcceptedAvailableCount = outgoingAcceptedAndNotFinalizedRowsForStorageAndOrganization.Where(x => x.Batch.Article == article)
                            .Sum(x => x.Count);

                        // увеличиваем точную переоценку
                        exactRevaluationSum += Math.Round((newAccountingPrice - oldAccountingPrice) * exactAvailableCount, 2);

                        // увеличиваем проведенную переоценку
                        acceptedRevaluationSum += Math.Round((newAccountingPrice - oldAccountingPrice) * (exactAvailableCount + incomingAcceptedAvailableCount - outgoingAcceptedAvailableCount), 2);
                    }

                    // формируем показатель точной переоценки
                    if (exactRevaluationSum != 0)
                    {
                        var ind = new ExactArticleRevaluationIndicator(accountingPriceList.EndDate.Value, storage.Id, accountOrganization.Id, exactRevaluationSum);

                        exactArticleRevaluationIndicators.Add(ind);
                    }

                    // формируем показатель проведенной переоценки
                    if (acceptedRevaluationSum != 0)
                    {
                        var ind = new AcceptedArticleRevaluationIndicator(accountingPriceList.EndDate.Value, storage.Id, accountOrganization.Id, acceptedRevaluationSum);

                        acceptedArticleRevaluationIndicators.Add(ind);
                    }
                }
            }

            // пересчет показателей точной переоценки
            exactArticleRevaluationIndicatorService.Update(accountingPriceList.EndDate.Value, storageIdsSubQuery, exactArticleRevaluationIndicators);

            // пересчет показателей проведенной переоценки
            acceptedArticleRevaluationIndicatorService.Update(accountingPriceList.EndDate.Value, storageIdsSubQuery, acceptedArticleRevaluationIndicators);

            // установка признака выполнения переоценки на конец действия РЦ
            accountingPriceList.IsRevaluationOnEndCalculated = true;
        }

        #endregion

        #region Проводка накладной

        #region Проводка входящей накладной

        /// <summary>
        /// Проводка приходной накладной
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        public void ReceiptWaybillAccepted(ReceiptWaybill waybill)
        {
            var rowsSubQuery = receiptWaybillRepository.GetRowsSubQuery(waybill.Id);
            var articleSubQuery = receiptWaybillRepository.GetArticlesSubquery(waybill.Id);

            // информация о позициях накладной
            var waybillRowInfo = waybill.Rows.ToDictionary(x => x.Id, x => new Tuple<int, decimal>(x.Article.Id, x.CurrentCount));

            IncomingWaybillAccepted(rowsSubQuery, articleSubQuery, waybillRowInfo, WaybillType.ReceiptWaybill,
                waybill.ReceiptStorage, waybill.AccountOrganization, waybill.AcceptanceDate.Value);
        }

        /// <summary>
        /// Проводка входящей накладной
        /// </summary>
        /// <param name="waybillRowsSubQuery">Подзапрос для позиций накладной</param>
        /// <param name="articleSubQuery">Подзапрос для товаров из накладной</param>
        /// <param name="waybillRowInfo">Информация о позициях накладной</param>
        /// <param name="waybillType">Тип накладной</param>
        /// <param name="storage">МХ</param>
        /// <param name="accountOrganization">Собственная организация</param>
        /// <param name="acceptanceDate">Дата проводки накладной</param>
        private void IncomingWaybillAccepted(ISubQuery waybillRowsSubQuery, ISubQuery articleSubQuery, IDictionary<Guid, Tuple<int, decimal>> waybillRowInfo,
            WaybillType waybillType, Storage storage, AccountOrganization accountOrganization, DateTime acceptanceDate)
        {
            WaybillAccepted(waybillRowsSubQuery, articleSubQuery, waybillRowInfo, waybillType, true, storage, accountOrganization, acceptanceDate);
        }

        #endregion

        #region Проводка исходящей накладной

        /// <summary>
        /// Проводка накладной реализации товаров
        /// </summary>
        /// <param name="waybill">Накладная реализации товаров</param>
        public void ExpenditureWaybillAccepted(ExpenditureWaybill waybill)
        {
            var rowsSubQuery = expenditureWaybillRepository.GetRowsSubQuery(waybill.Id);
            var articleSubQuery = expenditureWaybillRepository.GetArticlesSubquery(waybill.Id);

            // информация о позициях накладной
            var waybillRowInfo = waybill.Rows.ToDictionary(x => x.Id, x => new Tuple<int, decimal>(x.Article.Id, x.SellingCount));

            OutgoingWaybillAccepted(rowsSubQuery, articleSubQuery, waybillRowInfo, WaybillType.ExpenditureWaybill, 
                waybill.SenderStorage, waybill.Sender, waybill.AcceptanceDate.Value);
        }
        
        /// <summary>
        /// Проводка исходящей накладной
        /// </summary>
        /// <param name="waybillRowsSubQuery">Подзапрос для позиций накладной</param>
        /// <param name="articleSubQuery">Подзапрос для товаров из накладной</param>
        /// <param name="waybillRowInfo">Информация о позициях накладной</param>
        /// <param name="waybillType">Тип накладной</param>
        /// <param name="storage">МХ</param>
        /// <param name="accountOrganization">Собственная организация</param>
        /// <param name="acceptanceDate">Дата проводки накладной</param>
        private void OutgoingWaybillAccepted(ISubQuery waybillRowsSubQuery, ISubQuery articleSubQuery, IDictionary<Guid, Tuple<int, decimal>> waybillRowInfo,
            WaybillType waybillType, Storage storage, AccountOrganization accountOrganization, DateTime acceptanceDate)
        {
            WaybillAccepted(waybillRowsSubQuery, articleSubQuery, waybillRowInfo, waybillType, false, storage, accountOrganization, acceptanceDate);
        }

        #endregion

        /// <summary>
        /// Проводка накладной
        /// </summary>
        /// <param name="waybillRowsSubQuery">Подзапрос для позиций накладной</param>
        /// <param name="articleSubQuery">Подзапрос для товаров из накладной</param>
        /// <param name="waybillRowInfo">Информация о позициях накладной</param>
        /// <param name="waybillType">Тип накладной</param>
        /// <param name="isWaybillRowIncoming">Является ли данная проведенная накладная продящей</param>
        /// <param name="storage">МХ</param>
        /// <param name="accountOrganization">Собственная организация</param>
        /// <param name="acceptanceDate">Дата проводки накладной</param>
        private void WaybillAccepted(ISubQuery waybillRowsSubQuery, ISubQuery articleSubQuery, IDictionary<Guid, Tuple<int, decimal>> waybillRowInfo,
            WaybillType waybillType, bool isWaybillRowIncoming, Storage storage, AccountOrganization accountOrganization, DateTime acceptanceDate)
        {
            // находим позиции всех РЦ по указанному МХ и товарам, которые вступили в действие или завершили действие после даты проводки данной накладной
            var accountingPricesRevaluatedOnStart = accountingPriceListRepository.GetArticleAccountingPricesRevaluatedOnStartAfterDate(storage.Id, articleSubQuery, acceptanceDate)
                .ToDictionary(x => x, x => x.AccountingPriceList.StartDate);
            var accountingPricesRevaluatedOnEnd = accountingPriceListRepository.GetArticleAccountingPricesRevaluatedOnEndAfterDate(storage.Id, articleSubQuery, acceptanceDate)
                .ToDictionary(x => x, x => x.AccountingPriceList.EndDate.Value);

            // если после даты проводки накладной не вступал и не завершал действие ни один РЦ, то выходим
            if (!accountingPricesRevaluatedOnStart.Any() && !accountingPricesRevaluatedOnEnd.Any()) return;

            // минимальная и максимальная даты для выборки УЦ
            var minDate = (accountingPricesRevaluatedOnStart.Values.Concat(accountingPricesRevaluatedOnEnd.Values).Min());
            var maxDate = (accountingPricesRevaluatedOnStart.Values.Concat(accountingPricesRevaluatedOnEnd.Values).Max());

            // список всех УЦ по МХ и товарам, действующих между (minDate - 1 сек) и (maxDate + 1)
            var accountingPriceIndicators = articleAccountingPriceIndicatorRepository.GetList(storage.Id, articleSubQuery, minDate.AddSeconds(-1), maxDate.AddSeconds(1));

            // список созданных связей (Дельта_1- для исходящей накладной, Дельта_1+ для входящей) 
            var newAccountingPriceListWaybillTakings = new List<AccountingPriceListWaybillTaking>();

            // проходим все позиции данной накладной и создаем связи (Дельта_1- для исходящей накладной, Дельта_1+ для входящей) между позициями накладной 
            // и позициями РЦ, которые вступили в действие или завершили действие после проводки данной накладной
            foreach (var rowInfo in waybillRowInfo)
            {
                // позиции РЦ, которые вступили в действие после проводки данной накладной
                foreach (var accountingPrice in accountingPricesRevaluatedOnStart.Where(x => x.Key.Article.Id == rowInfo.Value.Item1))
                {
                    var articleAccountingPrice = accountingPrice.Key;

                    var taking = new AccountingPriceListWaybillTaking(accountingPrice.Value, isWaybillRowIncoming,
                        articleAccountingPrice.Id, waybillType, rowInfo.Key, articleAccountingPrice.Article.Id, storage.Id, accountOrganization.Id,
                        articleAccountingPrice.AccountingPrice, true, rowInfo.Value.Item2);

                    accountingPriceListWaybillTakingRepository.Save(taking);

                    newAccountingPriceListWaybillTakings.Add(taking);
                }

                // позиции РЦ, которые завершили действие после проводки данной накладной
                foreach (var accountingPrice in accountingPricesRevaluatedOnEnd.Where(x => x.Key.Article.Id == rowInfo.Value.Item1))
                {
                    var articleAccountingPrice = accountingPrice.Key;

                    var taking = new AccountingPriceListWaybillTaking(accountingPrice.Value, isWaybillRowIncoming,
                        articleAccountingPrice.Id, waybillType, rowInfo.Key, articleAccountingPrice.Article.Id, storage.Id, accountOrganization.Id,
                        articleAccountingPrice.AccountingPrice, false, rowInfo.Value.Item2);

                    accountingPriceListWaybillTakingRepository.Save(taking);

                    newAccountingPriceListWaybillTakings.Add(taking);
                }
            }

            // словарь "Дата показателя проведенной переоценки - приращение показателя проведенной переоценки по сравнению с прошлым значением"
            var acceptedArticleRevaluationCorrectionDeltas = new DynamicDictionary<DateTime, decimal>();

            // проходим все созданные связи (Дельта_1- для исходящей накладной, Дельта_1+ для входящей) и формируем разницы изменений показателя проведенной переоценки
            foreach (var taking in newAccountingPriceListWaybillTakings)
            {
                // получение разницы УЦ для связи
                var accountingPriceVariation = GetTakingAccountingPriceVariation(accountingPriceIndicators, taking);

                // расчет суммы корректировки проведенной переоценки
                var acceptedArticleRevaluationCorrection = (isWaybillRowIncoming ? 1 : -1) * Math.Round(taking.Count * accountingPriceVariation, 2);

                // если есть, что корректировать
                if (acceptedArticleRevaluationCorrection != 0)
                {
                    acceptedArticleRevaluationCorrectionDeltas[taking.TakingDate] += acceptedArticleRevaluationCorrection;
                }
            }

            // пересчитываем показатели проведенной переоценки на основании сформированного словаря "Дата/значение прироста показателя"
            acceptedArticleRevaluationIndicatorService.Update(acceptedArticleRevaluationCorrectionDeltas, storage.Id, accountOrganization.Id);
        }

        #endregion

        #region Отмена проводки накладной

        #region Отмена проводки входящей накладной

        /// <summary>
        /// Отмена проводки приходной накладной
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        public void ReceiptWaybillAcceptanceCancelled(ReceiptWaybill waybill)
        {
            var rowsSubQuery = receiptWaybillRepository.GetRowsSubQuery(waybill.Id);
            var articleSubQuery = receiptWaybillRepository.GetArticlesSubquery(waybill.Id);

            IncomingWaybillAcceptanceCancelled(rowsSubQuery, articleSubQuery, waybill.ReceiptStorage, waybill.AccountOrganization);
        }

        /// <summary>
        /// Отмена проводки накладной возврата от клиента
        /// </summary>
        /// <param name="waybill">Накладная возврата от клиента</param>
        public void ReturnFromClientWaybillAcceptanceCancelled(ReturnFromClientWaybill waybill)
        {
            var rowsSubQuery = returnFromClientWaybillRepository.GetRowsSubQuery(waybill.Id);
            var articleSubQuery = returnFromClientWaybillRepository.GetArticlesSubquery(waybill.Id);

            IncomingWaybillAcceptanceCancelled(rowsSubQuery, articleSubQuery, waybill.RecipientStorage, waybill.Recipient);
        }

        #endregion

        #region Отмена проводки исходящей накладной

        /// <summary>
        /// Отмена проводки накладной реализации товаров
        /// </summary>
        /// <param name="waybill">Накладная реализации товаров</param>
        public void ExpenditureWaybillAcceptanceCancelled(ExpenditureWaybill waybill)
        {
            var rowsSubQuery = expenditureWaybillRepository.GetRowsSubQuery(waybill.Id);
            var articleSubQuery = expenditureWaybillRepository.GetArticlesSubquery(waybill.Id);
            
            OutgoingWaybillAcceptanceCancelled(rowsSubQuery, articleSubQuery, waybill.SenderStorage, waybill.Sender);
        }

        /// <summary>
        /// Отмена проводки накладной списания
        /// </summary>
        /// <param name="waybill">Накладная списания</param>
        public void WriteoffWaybillAcceptanceCancelled(WriteoffWaybill waybill)
        {
            var rowsSubQuery = writeoffWaybillRepository.GetRowsSubQuery(waybill.Id);
            var articleSubQuery = writeoffWaybillRepository.GetArticlesSubquery(waybill.Id);

            OutgoingWaybillAcceptanceCancelled(rowsSubQuery, articleSubQuery, waybill.SenderStorage, waybill.Sender);
        }

        #endregion

        #region Отмена проводки входяще-исходящей накладной
        
        /// <summary>
        /// Отмена проводки накладной перемещения
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        public void MovementWaybillAcceptanceCancelled(MovementWaybill waybill)
        {
            var rowsSubQuery = movementWaybillRepository.GetRowsSubQuery(waybill.Id);
            var articleSubQuery = movementWaybillRepository.GetArticlesSubquery(waybill.Id);

            IncomingWaybillAcceptanceCancelled(rowsSubQuery, articleSubQuery, waybill.RecipientStorage, waybill.Recipient);
            OutgoingWaybillAcceptanceCancelled(rowsSubQuery, articleSubQuery, waybill.SenderStorage, waybill.Sender);
        }

        /// <summary>
        /// Отмена проводки накладной смены собственника
        /// </summary>
        /// <param name="waybill">Накладная смены собственника</param>
        public void ChangeOwnerWaybillAcceptanceCancelled(ChangeOwnerWaybill waybill)
        {
            var rowsSubQuery = changeOwnerWaybillRepository.GetRowsSubQuery(waybill.Id);
            var articleSubQuery = changeOwnerWaybillRepository.GetArticlesSubquery(waybill.Id);

            IncomingWaybillAcceptanceCancelled(rowsSubQuery, articleSubQuery, waybill.Storage, waybill.Recipient);
            OutgoingWaybillAcceptanceCancelled(rowsSubQuery, articleSubQuery, waybill.Storage, waybill.Sender);
        }
 
        #endregion

        /// <summary>
        /// Отмена проводки входящей накладной
        /// </summary>
        /// <param name="waybillRowsSubQuery">Подзапрос для позиций накладной</param>
        /// <param name="articleSubQuery">Подзапрос для товаров из накладной</param>
        /// <param name="storage">МХ</param>
        /// <param name="accountOrganization">Собственная организация</param>
        private void IncomingWaybillAcceptanceCancelled(ISubQuery waybillRowsSubQuery, ISubQuery articleSubQuery, Storage storage, AccountOrganization accountOrganization)
        {
            WaybillAcceptanceCancelled(waybillRowsSubQuery, articleSubQuery, storage, accountOrganization, true);
        }

        /// <summary>
        /// Отмена проводки исходящей накладной
        /// </summary>
        /// <param name="waybillRowsSubQuery">Подзапрос для позиций накладной</param>
        /// <param name="articleSubQuery">Подзапрос для товаров из накладной</param>
        /// <param name="storage">МХ</param>
        /// <param name="accountOrganization">Собственная организация</param>
        private void OutgoingWaybillAcceptanceCancelled(ISubQuery waybillRowsSubQuery, ISubQuery articleSubQuery, Storage storage, AccountOrganization accountOrganization)
        {
            WaybillAcceptanceCancelled(waybillRowsSubQuery, articleSubQuery, storage, accountOrganization, false);
        }

        /// <summary>
        /// Отмена проводки накладной
        /// </summary>
        /// <param name="waybillRowsSubQuery">Подзапрос для позиций накладной</param>
        /// <param name="articleSubQuery">Подзапрос для товаров из накладной</param>
        /// <param name="storage">МХ</param>
        /// <param name="accountOrganization">Собственная организация</param>
        /// <param name="isIncomingWaybill">Накладная является входящей</param>
        private void WaybillAcceptanceCancelled(ISubQuery waybillRowsSubQuery, ISubQuery articleSubQuery, Storage storage, AccountOrganization accountOrganization, bool isIncomingWaybill)
        {
            // получение связей данной накладной с позициями РЦ
            var takings = accountingPriceListWaybillTakingRepository.GetList(waybillRowsSubQuery, storage.Id, accountOrganization.Id);

            // если данная накладная не учитывалась ни одним РЦ - выходим
            if (!takings.Any()) return;

            // минимальная и максимальная даты создания связей с данной накладной
            var minDate = takings.Min(x => x.TakingDate);
            var maxDate = takings.Max(x => x.TakingDate);

            // список всех УЦ по МХ и товарам, действующих между (minDate - 1 сек) и (maxDate + 1 сек)
            var accountingPriceIndicators = articleAccountingPriceIndicatorRepository.GetList(storage.Id, articleSubQuery, minDate.AddSeconds(-1), maxDate.AddSeconds(1));

            // словарь "Дата показателя проведенной переоценки - приращение показателя проведенной переоценки по сравнению с прошлым значением"
            var acceptedArticleRevaluationCorrectionDeltas = new DynamicDictionary<DateTime, decimal>();

            // проходим все связи (Дельта_1- для исходящей накладной, Дельта_1+ для входящей) и формируем разницы изменений показателя проведенной переоценки
            foreach (var taking in takings)
            {
                // получение разницы УЦ для связи
                var accountingPriceVariation = GetTakingAccountingPriceVariation(accountingPriceIndicators, taking);

                // расчет суммы корректировки проведенной переоценки
                var acceptedArticleRevaluationCorrection = (isIncomingWaybill ? -1 : 1) * Math.Round(taking.Count * accountingPriceVariation, 2);

                // если есть, что корректировать
                if (acceptedArticleRevaluationCorrection != 0)
                {
                    acceptedArticleRevaluationCorrectionDeltas[taking.TakingDate] += acceptedArticleRevaluationCorrection;
                }

                // удаление связи с РЦ
                accountingPriceListWaybillTakingRepository.Delete(taking);
            }

            // пересчитываем показатели проведенной переоценки на основании сформированного словаря "Дата/значение прироста показателя"
            acceptedArticleRevaluationIndicatorService.Update(acceptedArticleRevaluationCorrectionDeltas, storage.Id, accountOrganization.Id);
        }
        
        #endregion

        #region Перевод накладной в финальный статус

        #region Входящие накладные
        
        /// <summary>
        /// Приемка приходной накладной
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        public void ReceiptWaybillReceipted(ReceiptWaybill waybill)
        {
            // подзапросы для позиций накладной и товаров
            var rowsSubQuery = receiptWaybillRepository.GetRowsSubQuery(waybill.Id);
            var articleSubQuery = receiptWaybillRepository.GetArticlesSubquery(waybill.Id);

            // коллекции кодов позиций без расхождений при приемке и с расхождениями
            var rowWithoutDivergencesIds = waybill.Rows.Where(x => !x.AreDivergencesAfterReceipt && x.PendingCount > 0).Select(x => x.Id);
            // позиции, добавленные при приемке, не учитываются
            var rowWithDivergencesIds = waybill.Rows.Where(x => x.AreDivergencesAfterReceipt && x.PendingCount > 0).Select(x => x.Id);

            IncomingWaybillReceipted(rowsSubQuery, articleSubQuery, rowWithoutDivergencesIds, rowWithDivergencesIds,
                waybill.ReceiptStorage, waybill.AccountOrganization, waybill.ReceiptDate.Value);
        }

        /// <summary>
        /// Приемка накладной возврата товаров от клиента
        /// </summary>
        /// <param name="waybill">Накладная возврата от клиента</param>
        public void ReturnFromClientWaybillFinalized(ReturnFromClientWaybill waybill)
        {
            // подзапросы для позиций накладной и товаров
            var rowsSubQuery = returnFromClientWaybillRepository.GetRowsSubQuery(waybill.Id);
            var articleSubQuery = returnFromClientWaybillRepository.GetArticlesSubquery(waybill.Id);

            // коллекции кодов позиций без расхождений при приемке и с расхождениями
            var rowWithoutDivergencesIds = waybill.Rows.Select(x => x.Id);
            // расхождений быть не может
            var rowWithDivergencesIds = new List<Guid>();

            IncomingWaybillReceipted(rowsSubQuery, articleSubQuery, rowWithoutDivergencesIds, rowWithDivergencesIds,
                waybill.RecipientStorage, waybill.Recipient, waybill.ReceiptDate.Value);
        }

        #endregion

        #region Исходящие накладные

        /// <summary>
        /// Перевод накладной реализации товаров в финальный статус (отгрузка накладной)
        /// </summary>
        /// <param name="waybill">Накладная реализации товаров</param>
        public void ExpenditureWaybillFinalized(ExpenditureWaybill waybill)
        {
            // подзапросы для позиций накладной и товаров
            var rowsSubQuery = expenditureWaybillRepository.GetRowsSubQuery(waybill.Id);
            var articleSubQuery = expenditureWaybillRepository.GetArticlesSubquery(waybill.Id);

            OutgoingWaybillFinalized(rowsSubQuery, articleSubQuery, waybill.SenderStorage, waybill.Sender, waybill.ShippingDate.Value);
        }

        /// <summary>
        /// Перевод накладной списания товаров в финальный статус (списано)
        /// </summary>
        /// <param name="waybill">Накладная списания товаров</param>
        public void WriteoffWaybillFinalized(WriteoffWaybill waybill)
        {
            // подзапросы для позиций накладной и товаров
            var rowsSubQuery = writeoffWaybillRepository.GetRowsSubQuery(waybill.Id);
            var articleSubQuery = writeoffWaybillRepository.GetArticlesSubquery(waybill.Id);

            OutgoingWaybillFinalized(rowsSubQuery, articleSubQuery, waybill.SenderStorage, waybill.Sender, waybill.WriteoffDate.Value);
        }

        #endregion

        #region Входяще-исходящие накладные

        /// <summary>
        /// Приемка накладной перемещения
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        public void MovementWaybillFinalized(MovementWaybill waybill)
        {
            // подзапросы для позиций накладной и товаров
            var rowsSubQuery = movementWaybillRepository.GetRowsSubQuery(waybill.Id);
            var articleSubQuery = movementWaybillRepository.GetArticlesSubquery(waybill.Id);

            // коллекции кодов позиций без расхождений при приемке и с расхождениями
            var rowWithoutDivergencesIds = waybill.Rows.Select(x => x.Id);
            // расхождений пока быть не может
            var rowWithDivergencesIds = new List<Guid>();

            OutgoingWaybillFinalized(rowsSubQuery, articleSubQuery, waybill.SenderStorage, waybill.Sender, waybill.ReceiptDate.Value);

            IncomingWaybillReceipted(rowsSubQuery, articleSubQuery, rowWithoutDivergencesIds, rowWithDivergencesIds,
                waybill.RecipientStorage, waybill.Recipient, waybill.ReceiptDate.Value);
        }

        /// <summary>
        /// Приемка накладной смены собственника
        /// </summary>
        /// <param name="waybill">Накладная смены собственника</param>
        public void ChangeOwnerWaybillFinalized(ChangeOwnerWaybill waybill)
        {
            // подзапросы для позиций накладной и товаров
            var rowsSubQuery = changeOwnerWaybillRepository.GetRowsSubQuery(waybill.Id);
            var articleSubQuery = changeOwnerWaybillRepository.GetArticlesSubquery(waybill.Id);

            // коллекции кодов позиций без расхождений при приемке и с расхождениями
            var rowWithoutDivergencesIds = waybill.Rows.Select(x => x.Id);
            // расхождений быть не может
            var rowWithDivergencesIds = new List<Guid>();

            OutgoingWaybillFinalized(rowsSubQuery, articleSubQuery, waybill.Storage, waybill.Sender, waybill.ChangeOwnerDate.Value);

            IncomingWaybillReceipted(rowsSubQuery, articleSubQuery, rowWithoutDivergencesIds, rowWithDivergencesIds,
                waybill.Storage, waybill.Recipient, waybill.ChangeOwnerDate.Value);
        }

        #endregion
        
        /// <summary>
        /// Приемка входящей накладной
        /// </summary>
        /// <param name="waybillRowsSubQuery">Подзапрос для позиций входящей накладной</param>
        /// <param name="articleSubQuery">Подзапрос для товаров из накладной</param>
        /// <param name="rowWithoutDivergencesIds">Список Id позиций накладной без расхождений при приемке</param>
        /// <param name="rowWithDivergencesIds">Список Id позиций накладной с расхождениями при приемке</param>
        /// <param name="storage">МХ</param>
        /// <param name="accountOrganization">Собственная организация</param>
        /// <param name="receiptDate">Дата приемки товара по накладной на склад</param>
        private void IncomingWaybillReceipted(ISubQuery waybillRowsSubQuery, ISubQuery articleSubQuery, IEnumerable<Guid> rowWithoutDivergencesIds, IEnumerable<Guid> rowWithDivergencesIds,
            Storage storage, AccountOrganization accountOrganization, DateTime receiptDate)
        {
            // получение связей данной накладной с позициями РЦ
            var takings = accountingPriceListWaybillTakingRepository.GetList(waybillRowsSubQuery, storage.Id, accountOrganization.Id);

            // если данная накладная не учитывалась ни одним РЦ - выходим
            if (!takings.Any()) return;

            // минимальная и максимальная даты создания связей с данной накладной
            var minDate = takings.Min(x => x.TakingDate);
            var maxDate = takings.Max(x => x.TakingDate);

            // список всех УЦ по МХ и товарам, действующих между (minDate - 1 сек) и (maxDate + 1 сек)
            var accountingPriceIndicators = articleAccountingPriceIndicatorRepository.GetList(storage.Id, articleSubQuery, minDate.AddSeconds(-1), maxDate.AddSeconds(1));

            // Для позиций без расхождений:
            // 1. В связях с РЦ устанавливаем дату переоценки
            // 2. Увеличиваем показатель точной переоценки на сумму Дельта_1+ по связям с этой позицией
            
            // словарь "Дата показателя точной переоценки - приращение показателя точной переоценки по сравнению с прошлым значением"
            var exactArticleRevaluationCorrectionDeltas = new DynamicDictionary<DateTime, decimal>();

            foreach (var rowId in rowWithoutDivergencesIds)
            {
                // коллекция связей с данной позицией накладной
                var rowTakings = takings.Where(x => x.WaybillRowId == rowId);

                foreach (var taking in rowTakings)
                {
                    // определяем дату переоценки
                    var revaluationDate = DateTimeUtils.GetMaxDate(receiptDate, taking.TakingDate);

                    // устанавливаем дату осуществления переоценки (перехода в Дельта_0)
                    taking.RevaluationDate = revaluationDate;

                    // получение разницы УЦ для связи
                    var accountingPriceVariation = GetTakingAccountingPriceVariation(accountingPriceIndicators, taking);

                    var exactArticleRevaluationCorrection = Math.Round(taking.Count * accountingPriceVariation, 2);

                    // если есть, что корректировать
                    if (exactArticleRevaluationCorrection != 0)
                    {
                        exactArticleRevaluationCorrectionDeltas[revaluationDate] += exactArticleRevaluationCorrection;
                    }
                }
            }

            // пересчитываем показатели точной переоценки на основании сформированного словаря "Дата/значение прироста показателя"
            exactArticleRevaluationIndicatorService.Update(exactArticleRevaluationCorrectionDeltas, storage.Id, accountOrganization.Id);
            
            // Для позиций с расхождениями
            // 1. В связях с РЦ выставляем кол-во 0
            // 2. Уменьшаем показатель проведенной переоценки, начиная с даты приемки

            // словарь "Дата показателя проведенной переоценки - приращение показателя проведенной переоценки по сравнению с прошлым значением"
            var acceptedArticleRevaluationCorrectionDeltas = new DynamicDictionary<DateTime, decimal>();

            foreach (var rowId in rowWithDivergencesIds)
            {
                // коллекция связей с данной позицией накладной
                var rowTakings = takings.Where(x => x.WaybillRowId == rowId);

                foreach (var taking in rowTakings)
                {
                    // определяем дату начала измненения показателя переоценки
                    var revaluationDate = DateTimeUtils.GetMaxDate(receiptDate, taking.TakingDate);
                    
                    // получение разницы УЦ для связи
                    var accountingPriceVariation = GetTakingAccountingPriceVariation(accountingPriceIndicators, taking);

                    var acceptedArticleRevaluationCorrection = -Math.Round(taking.Count * accountingPriceVariation, 2);

                    // если есть, что корректировать
                    if (acceptedArticleRevaluationCorrection != 0)
                    {
                        acceptedArticleRevaluationCorrectionDeltas[revaluationDate] += acceptedArticleRevaluationCorrection;
                    }
                    
                    // сбрасываем кол-во в связях в 0
                    taking.Count = 0;
                }
            }

            // пересчитываем показатели проведенной переоценки на основании сформированного словаря "Дата/значение прироста показателя"
            acceptedArticleRevaluationIndicatorService.Update(acceptedArticleRevaluationCorrectionDeltas, storage.Id, accountOrganization.Id);
        }

        /// <summary>
        /// Перевод исходящей накладной в финальный статус
        /// </summary>
        /// <param name="waybillRowsSubQuery">Подзапрос для позиций входящей накладной</param>
        /// <param name="articleSubQuery">Подзапрос для товаров из накладной</param>        
        /// <param name="storage">МХ</param>
        /// <param name="accountOrganization">Собственная организация</param>
        /// <param name="finalizationDate">Дата перевода накладной в финальный статус</param>
        private void OutgoingWaybillFinalized(ISubQuery waybillRowsSubQuery, ISubQuery articleSubQuery, Storage storage, 
            AccountOrganization accountOrganization, DateTime finalizationDate)
        {
            // получение связей данной накладной с позициями РЦ
            var takings = accountingPriceListWaybillTakingRepository.GetList(waybillRowsSubQuery, storage.Id, accountOrganization.Id);

            // если данная накладная не учитывалась ни одним РЦ - выходим
            if (!takings.Any()) return;

            // минимальная и максимальная даты создания связей с данной накладной
            var minDate = takings.Min(x => x.TakingDate);
            var maxDate = takings.Max(x => x.TakingDate);

            // список всех УЦ по МХ и товарам, действующих между (minDate - 1 сек) и (maxDate + 1 сек)
            var accountingPriceIndicators = articleAccountingPriceIndicatorRepository.GetList(storage.Id, articleSubQuery, minDate.AddSeconds(-1), maxDate.AddSeconds(1));

            // Для связей с РЦ, вступившими/завершившими действие ДО перевода исходящей накладной в финальный статус:
            // 1. Cвязи с позициями исходящей накладной из Дельта_1- переходят в Дельта_0 (устанавливается дата переоценки). 
            // 2. Уменьшаем показатель точной переоценки на сумму Дельта_1- по связям с этой позицией, начиная с даты перевода накладной в финальный статус.

            var exactArticleRevaluationCorrection = 0M;

            // словарь "Дата показателя точной переоценки - приращение показателя по сравнению с прошлым значением"
            var exactArticleRevaluationCorrectionDeltas = new DynamicDictionary<DateTime, decimal>();

            foreach (var taking in takings.Where(x => x.TakingDate < finalizationDate))
            {
                // устанавливаем дату осуществления переоценки (перехода в Дельта_0)
                taking.RevaluationDate = finalizationDate;

                // получение разницы УЦ для связи
                var accountingPriceVariation = GetTakingAccountingPriceVariation(accountingPriceIndicators, taking);

                exactArticleRevaluationCorrection -= Math.Round(taking.Count * accountingPriceVariation, 2);
            }
            
            // если есть, что корректировать
            if (exactArticleRevaluationCorrection != 0)
            {
                exactArticleRevaluationCorrectionDeltas[finalizationDate] += exactArticleRevaluationCorrection;
            }

            // Для связей с РЦ, вступившими/завершившими действие ПОСЛЕ перевода исходящей накладной в финальный статус:
            // 1. Из связей позиций-источников для позиций исходящей накладной вычитается кол-во по соответствующим исходящим позициям. 
            //    Cвязи с позициями исходящей накладной удаляются. Если после вычитания в связи с позицией-источником получается кол-во = 0, то эту связь тоже удаляем.
            // 2. Уменьшаем показатель точной переоценки на дату каждой из связей на сумму Дельта_1- по удаленным связям.

            // находим позиции всех РЦ по указанному МХ, товарам, которые вступили в действие или завершили действие после даты отгрузки накладной
            var accountingPricesRevaluatedOnStart = accountingPriceListRepository.GetArticleAccountingPricesRevaluatedOnStartAfterDate(storage.Id, articleSubQuery, finalizationDate)
                .ToDictionary(x => x, x => x.AccountingPriceList.StartDate);
            var accountingPricesRevaluatedOnEnd = accountingPriceListRepository.GetArticleAccountingPricesRevaluatedOnEndAfterDate(storage.Id, articleSubQuery, finalizationDate)
                .ToDictionary(x => x, x => x.AccountingPriceList.EndDate.Value);

            // если есть такие РЦ
            if (accountingPricesRevaluatedOnStart.Any() || accountingPricesRevaluatedOnEnd.Any())
            {
                // формируем суммы изменений показателей точной переоценки
                foreach (var taking in takings.Where(x => x.TakingDate >= finalizationDate))
                {
                    // получение разницы УЦ для связи
                    var accountingPriceVariation = GetTakingAccountingPriceVariation(accountingPriceIndicators, taking);

                    var correctionSum = -Math.Round(taking.Count * accountingPriceVariation, 2);

                    if (correctionSum != 0)
                    {
                        exactArticleRevaluationCorrectionDeltas[taking.TakingDate] += correctionSum;
                    }
                }

                // находим связи позиций данной исходящей накладной с ее источниками
                var waybillRowArticleMovements = waybillRowArticleMovementRepository.GetByDestinationSubQuery(waybillRowsSubQuery);
                // подкритерий для источников позиций данной исходящей накладной
                var sourcesSubQuery = waybillRowArticleMovementRepository.GetSourcesSubQueryByDestinationsSubQuery(waybillRowsSubQuery);

                // находим все имеющиеся связи Дельта_0 по источникам для накладной
                var currentTakingsWithSources = accountingPriceListWaybillTakingRepository.GetList(sourcesSubQuery).Where(x => x.TakingDate >= finalizationDate);

                foreach (var taking in takings.Where(x => x.TakingDate >= finalizationDate))
	            {
		            // находим коды позиций входящих накладных
                    var sourceWaybillRowInfo = waybillRowArticleMovements.Where(y => y.DestinationWaybillRowId == taking.WaybillRowId)
                        .ToDictionary(x => x.SourceWaybillRowId, x => x.MovingCount);

                    var takingsWithSource = currentTakingsWithSources.Where(x => x.ArticleAccountingPriceId == taking.ArticleAccountingPriceId &&
                        x.IsOnAccountingPriceListStart == taking.IsOnAccountingPriceListStart);

                    // уменьшаем кол-во в связях позиций-источников с РЦ
                    foreach (var takingWithSource in takingsWithSource)
                    {
                        var currentSourceWaybillRowInfo = sourceWaybillRowInfo.Where(x => x.Key == takingWithSource.WaybillRowId).FirstOrDefault();

                        if (currentSourceWaybillRowInfo.Key != Guid.Empty)
                        {
                            takingWithSource.Count -= currentSourceWaybillRowInfo.Value;

                            if (takingWithSource.Count == 0)
                            {
                                accountingPriceListWaybillTakingRepository.Delete(takingWithSource);
                            }
                        }
                    }

                    // удаляем все связи Дельта_1- для позиций исходящей накладной, связанных с РЦ, вступившими/завершившими действие ПОСЛЕ перевода исходящей накладной в финальный статус
                    accountingPriceListWaybillTakingRepository.Delete(taking);
	            }
            }
            
            // пересчитываем показатель точной переоценки на основании сформированного словаря "Дата/значение прироста показателя"
            exactArticleRevaluationIndicatorService.Update(exactArticleRevaluationCorrectionDeltas, storage.Id, accountOrganization.Id);
        }

        #region Согласование расхождений после приемки входящей накладной

        /// <summary>
        /// Согласование приходной накладной
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        public void ReceiptWaybillApproved(ReceiptWaybill waybill)
        {
            // подзапрос для товаров из позиций с расхождениями при приемке (включая добавленные при приемке позиции)
            var rowsWithDivergencesArticleSubQuery = receiptWaybillRepository.GetWaybillRowsWithDivergencesAfterReceiptArticleSubQuery(waybill.Id, false);
            
            // подзапрос для позиций с расхождениями при приемке (исключая добавленные при приемке позиции)
            var rowsWithDivergencesExcludingAddedOnReceiptSubQuery = receiptWaybillRepository.GetWaybillRowsWithDivergencesAfterReceiptSubQuery(waybill.Id, true);

            // подзапрос для товаров из позиций, добавленных при приемке накладной            
            var addedOnReceiptArticleSubQuery = receiptWaybillRepository.GetAddedOnReceiptArticleSubQuery(waybill.Id);

            // коллекция кодов позиций накладной с расхождениями при приемке с указанием согласованного кол-ва (исключая добавленные при приемке)
            var rowsWithDivergencesExcludingAddedOnReceiptInfo = waybill.Rows.Where(x => x.AreDivergencesAfterReceipt && x.PendingCount > 0).ToDictionary(x => x.Id, x => x.ApprovedCount.Value);

            // коллекция кодов позиций, добавленных при приемке, с указанием товара и кол-ва товара по позиции
            var addedOnReceiptRowInfo = waybill.Rows.Where(x => x.AreDivergencesAfterReceipt && x.PendingCount == 0)
                .ToDictionary(x => x.Id, x => new Tuple<int, decimal>(x.Article.Id, x.ApprovedCount.Value));
                
            IncomingWaybillApproved(rowsWithDivergencesArticleSubQuery, rowsWithDivergencesExcludingAddedOnReceiptSubQuery, addedOnReceiptArticleSubQuery, 
                rowsWithDivergencesExcludingAddedOnReceiptInfo, addedOnReceiptRowInfo, WaybillType.ReceiptWaybill, 
                waybill.ReceiptStorage, waybill.AccountOrganization, waybill.ApprovementDate.Value);
        }

        /// <summary>
        /// Согласование расхождений по входящей накладной
        /// </summary>
        /// <param name="rowsWithDivergencesArticleSubQuery">Подзапрос для товаров из позиций с расхождениями, включая добавленные при приемке</param>
        /// <param name="rowsWithDivergencesExcludingAddedOnReceiptSubQuery">Подзапрос для позиций с расхождениями, исключая добавленные при приемке</param>        
        /// <param name="addedOnReceiptArticleSubQuery">Подзапрос для товаров из добавленных при приемке позиций</param>
        /// <param name="rowsWithDivergencesExcludingAddedOnReceiptInfo">Коллекция кодов позиций накладной с расхождениями при приемке, исключая добавленные при приемке, с указанием согласованного кол-ва</param>
        /// <param name="addedOnReceiptRowInfo">Коллекция кодов позиций накладной, добавленных при приемке, с указанием согласованного кол-ва</param>         
        /// <param name="waybillType">Тип входящей накладной</param>
        /// <param name="storage">МХ</param>
        /// <param name="accountOrganization">Собственная организация</param>
        /// <param name="approvementDate">Дата согласования накладной</param>
        private void IncomingWaybillApproved(ISubQuery rowsWithDivergencesArticleSubQuery, ISubQuery rowsWithDivergencesExcludingAddedOnReceiptSubQuery,
            ISubQuery addedOnReceiptArticleSubQuery, IDictionary<Guid, decimal> rowsWithDivergencesExcludingAddedOnReceiptInfo, IDictionary<Guid, Tuple<int, decimal>> addedOnReceiptRowInfo,
            WaybillType waybillType, Storage storage, AccountOrganization accountOrganization, DateTime approvementDate)
        {
            // получение имеющихся связей между позициями с расхождениями данной накладной и позициями РЦ
            var takings = accountingPriceListWaybillTakingRepository.GetList(rowsWithDivergencesExcludingAddedOnReceiptSubQuery, storage.Id, accountOrganization.Id);

            // находим позиции всех РЦ по указанному МХ и товарам, которые вступили в действие или завершили действие после даты согласования данной накладной
            var accountingPricesRevaluatedOnStart = accountingPriceListRepository.GetArticleAccountingPricesRevaluatedOnStartAfterDate(storage.Id, addedOnReceiptArticleSubQuery, approvementDate)
                .ToDictionary(x => x, x => x.AccountingPriceList.StartDate);
            var accountingPricesRevaluatedOnEnd = accountingPriceListRepository.GetArticleAccountingPricesRevaluatedOnEndAfterDate(storage.Id, addedOnReceiptArticleSubQuery, approvementDate)
                .ToDictionary(x => x, x => x.AccountingPriceList.EndDate.Value);

            // если данная накладная не учитывалась ни одним РЦ и после даты проводки накладной не вступал и не завершал действие ни один РЦ, то выходим
            if (!takings.Any() && !accountingPricesRevaluatedOnStart.Any() && !accountingPricesRevaluatedOnEnd.Any()) return;
            
            // минимальная и максимальная даты для выборки УЦ
            var minDate = accountingPricesRevaluatedOnStart.Values.Concat(accountingPricesRevaluatedOnEnd.Values)
                .Concat(takings.Select(x => x.TakingDate)).Min();

            var maxDate = accountingPricesRevaluatedOnStart.Values.Concat(accountingPricesRevaluatedOnEnd.Values)
                .Concat(takings.Select(x => x.TakingDate)).Max();

            // список всех УЦ по МХ и товарам, действующих между (minDate - 1 сек) и (maxDate + 1 сек)
            var accountingPriceIndicators = articleAccountingPriceIndicatorRepository.GetList(storage.Id, rowsWithDivergencesArticleSubQuery,
                minDate.AddSeconds(-1), maxDate.AddSeconds(1));

            // Для позиций с расхождениями при приемке (не включая добавленные при приемке)
            // 1. Устанавливаем согласованное кол-во и дату переоценки
            // 2. Показатели точной и проведенной переоценок увеличиваются на сумму Дельта_1+ этой позиции. Сами связи переходят в Дельта_0.

            // словарь "Дата показателя переоценки - приращение показателя переоценки по сравнению с прошлым значением"
            // содержит значения приращения показателей как точной, так и проведенной переоценки
            var articleRevaluationCorrectionDeltas = new DynamicDictionary<DateTime, decimal>();

            foreach (var taking in takings)
            {
                // получение разницы УЦ для связи
                var accountingPriceVariation = GetTakingAccountingPriceVariation(accountingPriceIndicators, taking);

                // установка в связи согласованного кол-ва
                ValidationUtils.Assert(rowsWithDivergencesExcludingAddedOnReceiptInfo.Keys.Contains(taking.WaybillRowId), 
                    "Не найдена информация о согласованном количестве товара.");

                taking.Count = rowsWithDivergencesExcludingAddedOnReceiptInfo[taking.WaybillRowId];

                // расчет суммы корректировки
                var articleRevaluationCorrection = Math.Round(taking.Count * accountingPriceVariation, 2);

                // определяем дату переоценки
                var revaluationDate = DateTimeUtils.GetMaxDate(approvementDate, taking.TakingDate);

                // установка времени перехода в Дельта_0
                taking.RevaluationDate = revaluationDate;

                // если есть, что корректировать
                if (articleRevaluationCorrection != 0)
                {
                    articleRevaluationCorrectionDeltas[revaluationDate] += articleRevaluationCorrection;
                }
            }

            // Для позиций, добавленных при приемке
            // 1. Строим связи Дельта_0 от даты вступления/завершения действия РЦ. Дата переоценки в связи = Max(дата согласования накладной, дата формирования связи)
            // 2. Показатели точной и проведенной переоценок одновременно увеличиваются на сумму Дельта_0 по связям на каждую из дат переоценки.

            // список созданных связей Дельта_0 для добавленных при приемке позиций
            var newAccountingPriceListWaybillTakings = new List<AccountingPriceListWaybillTaking>();

            // проходим все добавленные при приемке позиции данной накладной и создаем связи Дельта_0 между позициями накладной 
            // и позициями РЦ, которые вступили в действие или завершили действие после согласования данной накладной
            foreach (var rowInfo in addedOnReceiptRowInfo)
            {
                // позиции РЦ, которые вступили в действие после согласования данной накладной
                foreach (var accountingPrice in accountingPricesRevaluatedOnStart.Where(x => x.Key.Article.Id == rowInfo.Value.Item1))
                {
                    var articleAccountingPrice = accountingPrice.Key;

                    // определяем дату связи и переоценки
                    var takingAndRevaluationDate = DateTimeUtils.GetMaxDate(approvementDate, accountingPrice.Value);

                    var taking = new AccountingPriceListWaybillTaking(takingAndRevaluationDate, true,
                        articleAccountingPrice.Id, waybillType, rowInfo.Key, articleAccountingPrice.Article.Id, storage.Id, accountOrganization.Id,
                        articleAccountingPrice.AccountingPrice, true, rowInfo.Value.Item2) { RevaluationDate = takingAndRevaluationDate };

                    accountingPriceListWaybillTakingRepository.Save(taking);

                    newAccountingPriceListWaybillTakings.Add(taking);
                }

                // позиции РЦ, которые завершили действие после согласования данной накладной
                foreach (var accountingPrice in accountingPricesRevaluatedOnEnd.Where(x => x.Key.Article.Id == rowInfo.Value.Item1))
                {
                    var articleAccountingPrice = accountingPrice.Key;

                    // определяем дату связи и переоценки
                    var takingAndRevaluationDate = DateTimeUtils.GetMaxDate(approvementDate, accountingPrice.Value);

                    var taking = new AccountingPriceListWaybillTaking(takingAndRevaluationDate, true,
                        articleAccountingPrice.Id, waybillType, rowInfo.Key, articleAccountingPrice.Article.Id, storage.Id, accountOrganization.Id,
                        articleAccountingPrice.AccountingPrice, false, rowInfo.Value.Item2) { RevaluationDate = takingAndRevaluationDate };

                    accountingPriceListWaybillTakingRepository.Save(taking);

                    newAccountingPriceListWaybillTakings.Add(taking);
                }
            }

            // проходим все созданные связи Дельта_0 и формируем разницы изменений показателей проведенной и точной переоценок
            foreach (var taking in newAccountingPriceListWaybillTakings)
            {
                // получение разницы УЦ для связи
                var accountingPriceVariation = GetTakingAccountingPriceVariation(accountingPriceIndicators, taking);

                // расчет суммы корректировки переоценки
                var articleRevaluationCorrection = Math.Round(taking.Count * accountingPriceVariation, 2);

                // если есть, что корректировать
                if (articleRevaluationCorrection != 0)
                {
                    articleRevaluationCorrectionDeltas[taking.RevaluationDate.Value] += articleRevaluationCorrection;
                }
            }

            // пересчитываем показатели точной переоценки на основании сформированного словаря "Дата/значение прироста показателя"
            exactArticleRevaluationIndicatorService.Update(articleRevaluationCorrectionDeltas, storage.Id, accountOrganization.Id);

            // пересчитываем показатели проведенной переоценки на основании сформированного словаря "Дата/значение прироста показателя"
            acceptedArticleRevaluationIndicatorService.Update(articleRevaluationCorrectionDeltas, storage.Id, accountOrganization.Id);
        }

        #endregion

        #endregion

        #region Отмена перевода накладной в финальный статус

        #region Входящие накладные

        /// <summary>
        /// Отмена приемки приходной накладной
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        public void ReceiptWaybillReceiptCancelled(ReceiptWaybill waybill)
        {
            // подзапросы для позиций накладной и товаров
            var rowsSubQuery = receiptWaybillRepository.GetRowsSubQuery(waybill.Id);
            var articleSubQuery = receiptWaybillRepository.GetArticlesSubquery(waybill.Id);

            // коллекции кодов позиций без расхождений при приемке и с расхождениями
            var rowWithoutDivergencesIds = waybill.Rows.Where(x => !x.AreDivergencesAfterReceipt && x.PendingCount > 0).Select(x => x.Id);
            // позиции, добавленные при приемке, не учитываются
            var rowWithDivergencesInfo = waybill.Rows.Where(x => x.AreDivergencesAfterReceipt && x.PendingCount > 0).ToDictionary(x => x.Id, x => x.PendingCount);

            IncomingWaybillReceiptCancelled(rowsSubQuery, articleSubQuery, rowWithoutDivergencesIds, rowWithDivergencesInfo,
                waybill.ReceiptStorage, waybill.AccountOrganization, waybill.ReceiptDate.Value);
        }

        /// <summary>
        /// Отмена приемки накладной возврата от клиента
        /// </summary>
        /// <param name="waybill">Накладная возврата от клиента</param>
        public void ReturnFromClientWaybillReceiptCancelled(ReturnFromClientWaybill waybill)
        {
            // подзапросы для позиций накладной и товаров
            var rowsSubQuery = returnFromClientWaybillRepository.GetRowsSubQuery(waybill.Id);
            var articleSubQuery = returnFromClientWaybillRepository.GetArticlesSubquery(waybill.Id);

            // коллекции кодов позиций без расхождений при приемке и с расхождениями
            var rowWithoutDivergencesIds = waybill.Rows.Select(x => x.Id);
            var rowWithDivergencesInfo = new Dictionary<Guid, decimal>();

            IncomingWaybillReceiptCancelled(rowsSubQuery, articleSubQuery, rowWithoutDivergencesIds, rowWithDivergencesInfo,
                waybill.RecipientStorage, waybill.Recipient, waybill.ReceiptDate.Value);
        }

        #region Отмена согласования входящей накладной

        /// <summary>
        /// Отмена согласования приходной накладной 
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        public void ReceiptWaybillApprovementCancelled(ReceiptWaybill waybill)
        {
            // подзапросы для позиций и товаров из позиций с расхождениями при приемке (включая добавленные при приемке)
            var rowsWithDivergencesSubQuery = receiptWaybillRepository.GetWaybillRowsWithDivergencesAfterReceiptSubQuery(waybill.Id, false);
            var rowsWithDivergencesArticleSubQuery = receiptWaybillRepository.GetWaybillRowsWithDivergencesAfterReceiptArticleSubQuery(waybill.Id, false);

            // коллекция кодов позиций накладной с расхождениями при приемке (исключая добавленные при приемке)
            var rowsWithDivergencesExcludingAddedOnReceiptId = waybill.Rows.Where(x => x.AreDivergencesAfterReceipt && x.PendingCount > 0).Select(x => x.Id);
            // коллекция кодов позиций накладной, добавленных при приемке
            var addedOnReceiptRowsId = waybill.Rows.Where(x => x.AreDivergencesAfterReceipt && x.PendingCount == 0).Select(x => x.Id);

            IncomingWaybillApprovementCancelled(rowsWithDivergencesSubQuery, rowsWithDivergencesArticleSubQuery, rowsWithDivergencesExcludingAddedOnReceiptId,
                addedOnReceiptRowsId, waybill.ReceiptStorage, waybill.AccountOrganization, waybill.ApprovementDate.Value);
        }

        /// <summary>
        /// Отмена согласования расхождений по входящей накладной
        /// </summary>
        /// <param name="rowsWithDivergencesSubQuery">Подзапрос для позиций с расхождениями</param>
        /// <param name="rowsWithDivergencesArticleSubQuery">Подзапрос для товаров из позиций с расхождениями</param>
        /// <param name="storage">МХ</param>
        /// <param name="accountOrganization">Собственная организация</param>
        /// <param name="approvementDate">Дата согласования накладной</param>
        private void IncomingWaybillApprovementCancelled(ISubQuery rowsWithDivergencesSubQuery, ISubQuery rowsWithDivergencesArticleSubQuery,
            IEnumerable<Guid> rowsWithDivergencesExcludingAddedOnReceiptId, IEnumerable<Guid> addedOnReceiptRowsId, Storage storage, AccountOrganization accountOrganization, DateTime approvementDate)
        {
            // получение связей позиций с расхождениями данной накладной с позициями РЦ
            var takings = accountingPriceListWaybillTakingRepository.GetList(rowsWithDivergencesSubQuery, storage.Id, accountOrganization.Id);

            // если данная накладная не учитывалась ни одним РЦ - выходим
            if (!takings.Any()) return;

            // минимальная и максимальная даты создания связей с данной накладной
            var minDate = takings.Min(x => x.TakingDate);
            var maxDate = takings.Max(x => x.TakingDate);

            // список всех УЦ по МХ и товарам, действующих между (minDate - 1 сек) и (maxDate + 1 сек)
            var accountingPriceIndicators = articleAccountingPriceIndicatorRepository.GetList(storage.Id, rowsWithDivergencesArticleSubQuery,
                minDate.AddSeconds(-1), maxDate.AddSeconds(1));

            // показатели проведенной переоценки, начиная с даты согласования накладной
            var acceptedArticleRevaluationIndicators = acceptedArticleRevaluationIndicatorRepository.GetFrom(approvementDate, storage.Id, accountOrganization.Id);
            // показатели точной переоценки, начиная с даты согласования накладной
            var exactArticleRevaluationIndicators = exactArticleRevaluationIndicatorRepository.GetFrom(approvementDate, storage.Id, accountOrganization.Id);

            var acceptedArticleRevaluationCorrection = 0M;
            var exactArticleRevaluationCorrection = 0M;

            // проходим все связи, созданные до даты согласования накладной и считаем первичную сумму корректировки переоценки
            foreach (var taking in takings.Where(x => x.TakingDate < approvementDate))
            {
                // получение разницы УЦ для связи
                var accountingPriceVariation = GetTakingAccountingPriceVariation(accountingPriceIndicators, taking);

                // расчет суммы корректировок
                acceptedArticleRevaluationCorrection -= Math.Round(taking.Count * accountingPriceVariation, 2);
                exactArticleRevaluationCorrection -= Math.Round(taking.Count * accountingPriceVariation, 2);
            }

            // пересчитываем показатели проведенной переоценки
            foreach (var ind in acceptedArticleRevaluationIndicators.OrderBy(x => x.StartDate))
            {
                // ищем все связи на дату показателя
                foreach (var taking in takings.Where(x => x.TakingDate == ind.StartDate))
                {
                    // получение разницы УЦ для связи
                    var accountingPriceVariation = GetTakingAccountingPriceVariation(accountingPriceIndicators, taking);

                    // расчет суммы корректировки
                    acceptedArticleRevaluationCorrection -= Math.Round(taking.Count * accountingPriceVariation, 2);
                }

                ind.RevaluationSum += acceptedArticleRevaluationCorrection;
            }

            // пересчитываем показатели точной переоценки
            foreach (var ind in exactArticleRevaluationIndicators.OrderBy(x => x.StartDate))
            {
                // ищем все связи на дату показателя
                foreach (var taking in takings.Where(x => x.TakingDate == ind.StartDate))
                {
                    // получение разницы УЦ для связи
                    var accountingPriceVariation = GetTakingAccountingPriceVariation(accountingPriceIndicators, taking);

                    // расчет суммы корректировки
                    exactArticleRevaluationCorrection -= Math.Round(taking.Count * accountingPriceVariation, 2);
                }

                ind.RevaluationSum += exactArticleRevaluationCorrection;
            }

            // обрабатываем связи
            foreach (var taking in takings)
            {
                // если это связь по позиции с расхождением
                if (rowsWithDivergencesExcludingAddedOnReceiptId.Contains(taking.WaybillRowId))
                {
                    // обнуление кол-ва товара в связи
                    taking.Count = 0;

                    // сброс даты осуществления переоценки
                    taking.RevaluationDate = null;
                }
                // если это связь по позиции, добавленной при приемке
                else if (addedOnReceiptRowsId.Contains(taking.WaybillRowId))
                {
                    accountingPriceListWaybillTakingRepository.Delete(taking);
                }
            }
        }

        #endregion

        #endregion

        #region Исходящие накладные

        /// <summary>
        /// Отмена перевода накладной реализации товаров в финальный статус
        /// </summary>
        /// <param name="waybill">Накладная реализации товаров</param>
        public void ExpenditureWaybillFinalizationCancelled(ExpenditureWaybill waybill)
        {
            var rowsSubQuery = expenditureWaybillRepository.GetRowsSubQuery(waybill.Id);
            var articleSubQuery = expenditureWaybillRepository.GetArticlesSubquery(waybill.Id);

            // информация о позициях накладной
            var waybillRowInfo = waybill.Rows.ToDictionary(x => x.Id, x => new Tuple<int, decimal>(x.Article.Id, x.SellingCount));

            OutgoingWaybillFinalizationCancelled(rowsSubQuery, articleSubQuery, waybillRowInfo, WaybillType.ExpenditureWaybill,
                waybill.SenderStorage, waybill.Sender, waybill.ShippingDate.Value);
        }

        /// <summary>
        /// Отмена перевода накладной списания товаров в финальный статус
        /// </summary>
        /// <param name="waybill">Накладная списания товаров</param>
        public void WriteoffWaybillFinalizationCancelled(WriteoffWaybill waybill)
        {
            var rowsSubQuery = writeoffWaybillRepository.GetRowsSubQuery(waybill.Id);
            var articleSubQuery = writeoffWaybillRepository.GetArticlesSubquery(waybill.Id);

            // информация о позициях накладной
            var waybillRowInfo = waybill.Rows.ToDictionary(x => x.Id, x => new Tuple<int, decimal>(x.Article.Id, x.WritingoffCount));

            OutgoingWaybillFinalizationCancelled(rowsSubQuery, articleSubQuery, waybillRowInfo, WaybillType.WriteoffWaybill,
                waybill.SenderStorage, waybill.Sender, waybill.WriteoffDate.Value);
        }

        #endregion

        #region Входяще-исходящие накладные

        /// <summary>
        /// Отмена приемки накладной перемещения
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        public void MovementWaybillReceiptCancelled(MovementWaybill waybill)
        {
            // подзапросы для позиций накладной и товаров
            var rowsSubQuery = movementWaybillRepository.GetRowsSubQuery(waybill.Id);
            var articleSubQuery = movementWaybillRepository.GetArticlesSubquery(waybill.Id);

            // коллекции кодов позиций без расхождений при приемке и с расхождениями
            var rowWithoutDivergencesIds = waybill.Rows.Select(x => x.Id);
            // по накладной перемещения расхождений пока быть не может
            var rowWithDivergencesInfo = new Dictionary<Guid, decimal>();

            IncomingWaybillReceiptCancelled(rowsSubQuery, articleSubQuery, rowWithoutDivergencesIds, rowWithDivergencesInfo,
                waybill.RecipientStorage, waybill.Recipient, waybill.ReceiptDate.Value);

            // коллекция кодов позиций накладной
            var waybillRowInfo = waybill.Rows.ToDictionary(x => x.Id, x => new Tuple<int, decimal>(x.Article.Id, x.MovingCount));

            OutgoingWaybillFinalizationCancelled(rowsSubQuery, articleSubQuery, waybillRowInfo, WaybillType.MovementWaybill,
                waybill.SenderStorage, waybill.Sender, waybill.ReceiptDate.Value);
        }

        /// <summary>
        /// Отмена приемки накладной смены собственника
        /// </summary>
        /// <param name="waybill">Накладная смены собственника</param>
        public void ChangeOwnerWaybillReceiptCancelled(ChangeOwnerWaybill waybill)
        {
            // подзапросы для позиций накладной и товаров
            var rowsSubQuery = changeOwnerWaybillRepository.GetRowsSubQuery(waybill.Id);
            var articleSubQuery = changeOwnerWaybillRepository.GetArticlesSubquery(waybill.Id);

            // коллекции кодов позиций без расхождений при приемке и с расхождениями
            var rowWithoutDivergencesIds = waybill.Rows.Select(x => x.Id);
            var rowWithDivergencesInfo = new Dictionary<Guid, decimal>();

            IncomingWaybillReceiptCancelled(rowsSubQuery, articleSubQuery, rowWithoutDivergencesIds, rowWithDivergencesInfo,
                waybill.Storage, waybill.Recipient, waybill.ChangeOwnerDate.Value);

            // коллекция кодов позиций накладной
            var waybillRowInfo = waybill.Rows.ToDictionary(x => x.Id, x => new Tuple<int, decimal>(x.Article.Id, x.MovingCount));

            OutgoingWaybillFinalizationCancelled(rowsSubQuery, articleSubQuery, waybillRowInfo, WaybillType.ChangeOwnerWaybill,
                waybill.Storage, waybill.Sender, waybill.ChangeOwnerDate.Value);
        }

        #endregion

        /// <summary>
        /// Отмена приемки входящей накладной
        /// </summary>
        /// <param name="waybillRowsSubQuery">Подзапрос для позиций входящей накладной</param>
        /// <param name="articleSubQuery">Подзапрос для товаров из накладной</param>
        /// <param name="rowWithoutDivergencesIds">Список Id позиций накладной без расхождений при приемке</param>
        /// <param name="rowWithDivergencesIds">Список Id позиций накладной с расхождениями при приемке</param>
        /// <param name="storage">МХ</param>
        /// <param name="accountOrganization">Собственная организация</param>
        /// <param name="receiptDate">Дата приемки товара по накладной на склад</param>
        private void IncomingWaybillReceiptCancelled(ISubQuery waybillRowsSubQuery, ISubQuery articleSubQuery, IEnumerable<Guid> rowWithoutDivergencesIds, IDictionary<Guid, decimal> rowWithDivergencesInfo,
            Storage storage, AccountOrganization accountOrganization, DateTime receiptDate)
        {
            // получение связей данной накладной с позициями РЦ
            var takings = accountingPriceListWaybillTakingRepository.GetList(waybillRowsSubQuery, storage.Id, accountOrganization.Id);

            // если данная накладная не учитывалась ни одним РЦ - выходим
            if (!takings.Any()) return;

            // минимальная и максимальная даты создания связей с данной накладной
            var minDate = takings.Min(x => x.TakingDate);
            var maxDate = takings.Max(x => x.TakingDate);

            // список всех УЦ по МХ и товарам, действующих между (minDate - 1 сек) и (maxDate + 1 сек)
            var accountingPriceIndicators = articleAccountingPriceIndicatorRepository.GetList(storage.Id, articleSubQuery, minDate.AddSeconds(-1), maxDate.AddSeconds(1));

            // показатели проведенной переоценки, начиная с даты приемки накладной
            var acceptedArticleRevaluationIndicators = acceptedArticleRevaluationIndicatorRepository.GetFrom(receiptDate, storage.Id, accountOrganization.Id);
            
            var acceptedArticleRevaluationCorrection = 0M;

            // словарь "Дата показателя точной переоценки - приращение показателя по сравнению с прошлым значением"
            var exactArticleRevaluationCorrectionDeltas = new DynamicDictionary<DateTime, decimal>();

            // проходим все связи, созданные до даты приемки накладной и считаем первичные суммы корректировок переоценок
            foreach (var taking in takings.Where(x => x.TakingDate < receiptDate))
            {
                // получение разницы УЦ для связи
                var accountingPriceVariation = GetTakingAccountingPriceVariation(accountingPriceIndicators, taking);

                if (rowWithoutDivergencesIds.Contains(taking.WaybillRowId))
                {
                    // сбрасываем дату осуществления переоценки (перехода в Дельта_0)
                    taking.RevaluationDate = null;
                    
                    // расчет суммы корректировки точной переоценки
                    var exactArticleRevaluationCorrection = -Math.Round(taking.Count * accountingPriceVariation, 2);

                    // если есть, что корректировать
                    if (exactArticleRevaluationCorrection != 0)
                    {
                        exactArticleRevaluationCorrectionDeltas[receiptDate] += exactArticleRevaluationCorrection;
                    }
                }
                else if (rowWithDivergencesInfo.Keys.Contains(taking.WaybillRowId))
                {
                    // устанавливаем кол-во из ожидания
                    taking.Count = rowWithDivergencesInfo[taking.WaybillRowId];

                    acceptedArticleRevaluationCorrection += Math.Round(taking.Count * accountingPriceVariation, 2);
                }
            }

            // Для позиций без расхождений:
            // 1. В связях с РЦ дату переоценки выставляем в null
            // 2. Уменьшаем показатель точной переоценки на сумму Дельта_1+ по связям с этой позицией

            // пересчитываем показатели точной переоценки
            foreach (var taking in takings.Where(x => rowWithoutDivergencesIds.Contains(x.WaybillRowId) && x.TakingDate >= receiptDate))
            {
                // получение разницы УЦ для связи
                var accountingPriceVariation = GetTakingAccountingPriceVariation(accountingPriceIndicators, taking);

                // расчет суммы корректировки
                var exactArticleRevaluationCorrection = -Math.Round(taking.Count * accountingPriceVariation, 2);

                // сбрасываем дату осуществления переоценки (перехода в Дельта_0)
                taking.RevaluationDate = null;

                // если есть, что корректировать
                if (exactArticleRevaluationCorrection != 0)
                {
                    exactArticleRevaluationCorrectionDeltas[taking.TakingDate] += exactArticleRevaluationCorrection;
                }
            }

            // Для позиций с расхождениями
            // 1. В связях с РЦ выставляем кол-во из ожидания
            // 2. Увеличиваем значения показателей проведенной переоценки, начиная с receiptDate                       

            // пересчитываем показатели проведенной переоценки
            foreach (var ind in acceptedArticleRevaluationIndicators.OrderBy(x => x.StartDate))
            {
                // ищем все связи на дату показателя
                foreach (var taking in takings.Where(x => rowWithDivergencesInfo.Keys.Contains(x.WaybillRowId) && x.TakingDate == ind.StartDate))
                {
                    // получение разницы УЦ для связи
                    var accountingPriceVariation = GetTakingAccountingPriceVariation(accountingPriceIndicators, taking);

                    // устанавливаем кол-во из ожидания
                    taking.Count = rowWithDivergencesInfo[taking.WaybillRowId];

                    acceptedArticleRevaluationCorrection += Math.Round(taking.Count * accountingPriceVariation, 2);
                }

                ind.RevaluationSum += acceptedArticleRevaluationCorrection;
            }

            // пересчитываем показатель точной переоценки на основании сформированного словаря "Дата/значение прироста показателя"
            exactArticleRevaluationIndicatorService.Update(exactArticleRevaluationCorrectionDeltas, storage.Id, accountOrganization.Id);
        }

        private void OutgoingWaybillFinalizationCancelled(ISubQuery waybillRowsSubQuery, ISubQuery articleSubQuery, IDictionary<Guid, Tuple<int, decimal>> waybillRowInfo,
            WaybillType waybillType, Storage storage, AccountOrganization accountOrganization, DateTime finalizationDate)
        {
            // находим позиции всех РЦ по указанному МХ, товарам, которые вступили в действие или завершили действие после даты отгрузки накладной
            var accountingPricesRevaluatedOnStart = accountingPriceListRepository.GetArticleAccountingPricesRevaluatedOnStartAfterDate(storage.Id, articleSubQuery, finalizationDate)
                .ToDictionary(x => x, x => x.AccountingPriceList.StartDate);
            var accountingPricesRevaluatedOnEnd = accountingPriceListRepository.GetArticleAccountingPricesRevaluatedOnEndAfterDate(storage.Id, articleSubQuery, finalizationDate)
                .ToDictionary(x => x, x => x.AccountingPriceList.EndDate.Value);

            // получение связей данной накладной с позициями РЦ
            var takings = accountingPriceListWaybillTakingRepository.GetList(waybillRowsSubQuery, storage.Id, accountOrganization.Id).ToList();

            // минимальная и максимальная даты создания связей с данной накладной
            var minDate = (takings.Any() ? takings.Min(x => x.TakingDate) : finalizationDate);

            var maxStartDate = (accountingPricesRevaluatedOnStart.Any() ? accountingPricesRevaluatedOnStart.Values.Max() : DateTime.MaxValue.AddSeconds(-1));
            var maxEndDate = (accountingPricesRevaluatedOnEnd.Any() ? accountingPricesRevaluatedOnEnd.Values.Max() : DateTime.MaxValue.AddSeconds(-1));
            var maxDate = (maxStartDate >= maxEndDate ? maxStartDate : maxEndDate);

            // список всех УЦ по МХ и товарам, действующих между (minDate - 1 сек) и максимальной датой начала или заверщения РЦ
            var accountingPriceIndicators = articleAccountingPriceIndicatorRepository.GetList(storage.Id, articleSubQuery, minDate.AddSeconds(-1), maxDate.AddSeconds(1));

            // По переоценкам, для которых дата перевода исходящей накладной в финальный статус > даты переоценки (т.е. существует связь Дельта_1-):
            // 1. Увеличиваем показатель точной переоценки на сумму Дельта_1-, начиная с даты перевода исходящей накладной в финальный статус
            // 2. В связях с РЦ сбрасываем дату переоценки
            var exactArticleRevaluationCorrection = 0M;

            // словарь "Дата показателя точной переоценки - приращение показателя по сравнению с прошлым значением"
            var exactArticleRevaluationCorrectionDeltas = new DynamicDictionary<DateTime, decimal>();

            // проходим все связи типа Дельта_1-, созданные до даты перевода исходящей накладной в финальный статус, 
            // чтобы посчитать начальную корректировку показателя точной переоценки
            foreach (var taking in takings.Where(x => x.TakingDate < finalizationDate))
            {
                // получение разницы УЦ для связи
                var accountingPriceVariation = GetTakingAccountingPriceVariation(accountingPriceIndicators, taking);

                // сбрасываем дату осуществления переоценки (перехода в Дельта_0)
                taking.RevaluationDate = null;

                // расчет суммы корректировки точной переоценки
                exactArticleRevaluationCorrection += Math.Round(taking.Count * accountingPriceVariation, 2);
            }

            // если есть, что корректировать
            if (exactArticleRevaluationCorrection != 0)
            {
                exactArticleRevaluationCorrectionDeltas[finalizationDate] += exactArticleRevaluationCorrection;
            }

            // список созданных связей Дельта_1-
            var newAccountingPriceListWaybillTakings = new List<AccountingPriceListWaybillTaking>();

            // проходим все позиции накладной и создаем связи типа Дельта_1- между позициями накладной и позициями РЦ, которые вступили в действие или 
            // завершили действие после перевода исходящей накладной в финальный статус
            foreach (var rowInfo in waybillRowInfo)
            {
                // позиции РЦ, которые вступили в действие после перевода исходящей накладной в финальный статус
                foreach (var accountingPrice in accountingPricesRevaluatedOnStart.Where(x => x.Key.Article.Id == rowInfo.Value.Item1))
                {
                    var articleAccountingPrice = accountingPrice.Key;

                    var taking = new AccountingPriceListWaybillTaking(accountingPrice.Value, false,
                        articleAccountingPrice.Id, waybillType, rowInfo.Key, articleAccountingPrice.Article.Id, storage.Id, accountOrganization.Id,
                        articleAccountingPrice.AccountingPrice, true, rowInfo.Value.Item2);

                    accountingPriceListWaybillTakingRepository.Save(taking);

                    newAccountingPriceListWaybillTakings.Add(taking);
                }

                // позиции РЦ, которые завершили действие после перевода исходящей накладной в финальный статус
                foreach (var accountingPrice in accountingPricesRevaluatedOnEnd.Where(x => x.Key.Article.Id == rowInfo.Value.Item1))
                {
                    var articleAccountingPrice = accountingPrice.Key;

                    var taking = new AccountingPriceListWaybillTaking(accountingPrice.Value, false,
                        articleAccountingPrice.Id, waybillType, rowInfo.Key, articleAccountingPrice.Article.Id, storage.Id, accountOrganization.Id,
                        articleAccountingPrice.AccountingPrice, false, rowInfo.Value.Item2);

                    accountingPriceListWaybillTakingRepository.Save(taking);

                    newAccountingPriceListWaybillTakings.Add(taking);
                }
            }

            // находим связи позиций данной исходящей накладной с ее источниками
            var waybillRowArticleMovements = waybillRowArticleMovementRepository.GetByDestinationSubQuery(waybillRowsSubQuery);
            // подкритерий для источников
            var sourcesSubQuery = waybillRowArticleMovementRepository.GetSourcesSubQueryByDestinationsSubQuery(waybillRowsSubQuery);

            // находим все имеющиеся связи Дельта_0 по источникам для накладной
            var currentTakingsWithSources = accountingPriceListWaybillTakingRepository.GetList(sourcesSubQuery);

            // проходим все созданные связи Дельта_1-
            foreach (var taking in newAccountingPriceListWaybillTakings)
            {
                // по источнику ищем соответствующую ей связи Дельта_0 с позицией-источником
                // связи с источниками
                var currentWaybillRowArticleMovements = waybillRowArticleMovements.Where(x => x.DestinationWaybillRowId == taking.WaybillRowId);

                // для каждого источника пытаемся найти связь Дельта_0
                foreach (var sourceWaybillRowInfo in currentWaybillRowArticleMovements)
                {
                    var currentTaking = currentTakingsWithSources.Where(x => x.WaybillRowId == sourceWaybillRowInfo.SourceWaybillRowId &&
                        x.ArticleAccountingPriceId == taking.ArticleAccountingPriceId && x.IsOnAccountingPriceListStart == taking.IsOnAccountingPriceListStart).FirstOrDefault();

                    // если связь найдена, то увеличиваем кол-о товара в связи на кол-во товара из позиции исходящей накладной (резерва)
                    if (currentTaking != null)
                    {
                        currentTaking.Count += sourceWaybillRowInfo.MovingCount;

                        // получение разницы УЦ для связи
                        var accountingPriceVariation = GetTakingAccountingPriceVariation(accountingPriceIndicators, currentTaking);

                        var correctionSum = Math.Round(sourceWaybillRowInfo.MovingCount * accountingPriceVariation, 2);                        
                        if(correctionSum != 0)
                        {
                            exactArticleRevaluationCorrectionDeltas[currentTaking.TakingDate] += correctionSum;
                        }
                    }
                    // иначе создаем связь
                    else
                    {
                        var newTaking = new AccountingPriceListWaybillTaking(taking.TakingDate, true,
                            taking.ArticleAccountingPriceId, sourceWaybillRowInfo.SourceWaybillType, sourceWaybillRowInfo.SourceWaybillRowId, taking.ArticleId, storage.Id, accountOrganization.Id,
                            taking.AccountingPrice, taking.IsOnAccountingPriceListStart, sourceWaybillRowInfo.MovingCount);

                        // для созданной связи типа Дельта_0 можно сразу установить дату осуществления переоценки = дате создания связи
                        newTaking.RevaluationDate = taking.TakingDate;

                        accountingPriceListWaybillTakingRepository.Save(newTaking);

                        var accountingPriceVariation = GetTakingAccountingPriceVariation(accountingPriceIndicators, newTaking);

                        var correctionSum = Math.Round(sourceWaybillRowInfo.MovingCount * accountingPriceVariation, 2);
                        if (correctionSum != 0)
                        {
                            exactArticleRevaluationCorrectionDeltas[newTaking.TakingDate] += correctionSum;
                        }
                    }
                }
            }

            // пересчитываем показатель точной переоценки на основании сформированного словаря "Дата/значение прироста показателя"
            exactArticleRevaluationIndicatorService.Update(exactArticleRevaluationCorrectionDeltas, storage.Id, accountOrganization.Id);
        }

        #endregion

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Поиск по коллекции показателей УЦ актуальной УЦ по параметрам на указанную дату
        /// </summary>
        /// <param name="indicators">Коллекция показателей</param>
        /// <param name="storageId">Код МХ</param>
        /// <param name="articleId">Код товара</param>
        /// <param name="date">Дата, на которую ищется показатель</param>
        private decimal GetActualAccountingPrice(IEnumerable<ArticleAccountingPriceIndicator> indicators, short storageId, int articleId, DateTime date)
        {
            var ind = indicators
                .Where(x => x.StorageId == storageId && x.ArticleId == articleId && x.StartDate <= date && (x.EndDate > date || x.EndDate == null))
                .OrderByDescending(x => x.StartDate)
                .FirstOrDefault();

            return ind == null ? 0 : ind.AccountingPrice;
        }

        /// <summary>
        /// Получение для связи разница между новой и старой УЦ
        /// </summary>
        /// <param name="accountingPriceIndicators">Список показателей УЦ</param>
        /// <param name="storage">МХ</param>
        /// <param name="taking">Связь позиции РЦ и накладной</param>
        /// <returns>Разница между новой и старой УЦ</returns>
        private decimal GetTakingAccountingPriceVariation(IEnumerable<ArticleAccountingPriceIndicator> accountingPriceIndicators, AccountingPriceListWaybillTaking taking)
        {
            // если связь создана на начало периода действия РЦ - вычитаем секунду (ищем предыдущую цену)
            // если на конец периода действия РЦ - то прибавляем секунду (ищем следующую цену)
            var accountingPriceDate = taking.TakingDate.AddSeconds(taking.IsOnAccountingPriceListStart ? -1 : 1);

            decimal prevAccountingPrice = 0M, newAccountingPrice = 0M;

            if (taking.IsOnAccountingPriceListStart)
            {
                prevAccountingPrice = GetActualAccountingPrice(accountingPriceIndicators, taking.StorageId, taking.ArticleId, accountingPriceDate);
                newAccountingPrice = taking.AccountingPrice;
            }
            else
            {
                prevAccountingPrice = taking.AccountingPrice;
                newAccountingPrice = GetActualAccountingPrice(accountingPriceIndicators, taking.StorageId, taking.ArticleId, accountingPriceDate);
            }

            return newAccountingPrice - prevAccountingPrice;
        }

        #endregion

        #endregion
    }
}
