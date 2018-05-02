using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;
using System.Linq;

namespace ERP.Wholesale.Domain.Services
{
    public class ReturnFromClientService : IReturnFromClientService
    {
        #region Поля

        private readonly IReturnFromClientWaybillRepository returnFromClientWaybillRepository;
        private readonly IReceiptWaybillRepository receiptWaybillRepository;
        private readonly IArticleRepository articleRepository;
        
        private readonly IAcceptedSaleIndicatorRepository acceptedSaleIndicatorRepository;
        private readonly IShippedSaleIndicatorRepository shippedSaleIndicatorRepository;
        
        private readonly IAcceptedReturnFromClientIndicatorRepository acceptedReturnFromClientIndicatorRepository;
        private readonly IReceiptedReturnFromClientIndicatorRepository receiptedReturnFromClientIndicatorRepository;
        private readonly IReturnFromClientBySaleAcceptanceDateIndicatorRepository returnFromClientBySaleAcceptanceDateIndicatorRepository;
        private readonly IReturnFromClientBySaleShippingDateIndicatorRepository returnFromClientBySaleShippingDateIndicatorRepository;

        private readonly IAcceptedReturnFromClientIndicatorService acceptedReturnFromClientIndicatorService;
        private readonly IReceiptedReturnFromClientIndicatorService receiptedReturnFromClientIndicatorService;
        private readonly IReturnFromClientBySaleAcceptanceDateIndicatorService returnFromClientBySaleAcceptanceDateIndicatorService;
        private readonly IReturnFromClientBySaleShippingDateIndicatorService returnFromClientBySaleShippingDateIndicatorService;

        #endregion

        #region Конструкторы

        public ReturnFromClientService(IReturnFromClientWaybillRepository returnFromClientWaybillRepository, IReceiptWaybillRepository receiptWaybillRepository,
            IArticleRepository articleRepository, IAcceptedSaleIndicatorRepository acceptedSaleIndicatorRepository,
            IShippedSaleIndicatorRepository shippedSaleIndicatorRepository, 
            IAcceptedReturnFromClientIndicatorRepository acceptedReturnFromClientIndicatorRepository,
            IReturnFromClientBySaleAcceptanceDateIndicatorRepository returnFromClientBySaleAcceptanceDateIndicatorRepository,
            IReturnFromClientBySaleShippingDateIndicatorRepository returnFromClientBySaleShippingDateIndicatorRepository,
            IReceiptedReturnFromClientIndicatorRepository receiptedReturnFromClientIndicatorRepository,
            IAcceptedReturnFromClientIndicatorService acceptedReturnFromClientIndicatorService,
            IReceiptedReturnFromClientIndicatorService receiptedReturnFromClientIndicatorService,
            IReturnFromClientBySaleAcceptanceDateIndicatorService returnFromClientBySaleAcceptanceDateIndicatorService,
            IReturnFromClientBySaleShippingDateIndicatorService returnFromClientBySaleShippingDateIndicatorService
            )
        {
            this.returnFromClientWaybillRepository = returnFromClientWaybillRepository;
            this.receiptWaybillRepository = receiptWaybillRepository;
            this.articleRepository = articleRepository;
            this.acceptedSaleIndicatorRepository = acceptedSaleIndicatorRepository;
            this.shippedSaleIndicatorRepository = shippedSaleIndicatorRepository;
            
            this.acceptedReturnFromClientIndicatorRepository = acceptedReturnFromClientIndicatorRepository;
            this.receiptedReturnFromClientIndicatorRepository = receiptedReturnFromClientIndicatorRepository;
            this.returnFromClientBySaleAcceptanceDateIndicatorRepository = returnFromClientBySaleAcceptanceDateIndicatorRepository;
            this.returnFromClientBySaleShippingDateIndicatorRepository = returnFromClientBySaleShippingDateIndicatorRepository;

            this.acceptedReturnFromClientIndicatorService = acceptedReturnFromClientIndicatorService;
            this.receiptedReturnFromClientIndicatorService = receiptedReturnFromClientIndicatorService;
            this.returnFromClientBySaleAcceptanceDateIndicatorService = returnFromClientBySaleAcceptanceDateIndicatorService;
            this.returnFromClientBySaleShippingDateIndicatorService = returnFromClientBySaleShippingDateIndicatorService;
        }

        #endregion

        #region Методы

        #region Пересчет показателей возвратов от клиентов

        #region Проводка накладной возврата от клиента и отмена проводки

        // TODO: провести рефакторинг расчета показателей (убрать дублирование)

        /// <summary>
        /// Пересчет показателей возвратов от клиентов при проводке накладной возврата от клиента
        /// </summary>
        public void ReturnFromClientWaybillAccepted(ReturnFromClientWaybill waybill)
        {
            // проведенные возвраты
            var acceptedIndicators = new List<AcceptedReturnFromClientIndicator>();
            // возвраты на дату проводки накладной реализации
            var bySaleAcceptanceDateIndicators = new List<ReturnFromClientBySaleAcceptanceDateIndicator>();
            // возвраты на дату отгрузки накладной реализации
            var bySaleShippingDateIndicators = new List<ReturnFromClientBySaleShippingDateIndicator>();

            foreach (var row in waybill.Rows)
            {
                ValidationUtils.Assert(row.SaleWaybillRow.SaleWaybill.Is<ExpenditureWaybill>(),
                    "Расчет показателя для данного типа накладной реализации еще не реализован.");
                
                var saleWaybillRow = row.SaleWaybillRow.As<ExpenditureWaybillRow>();

                ValidationUtils.Assert(saleWaybillRow.AvailableToReturnCount >= row.ReturnCount,
                    String.Format("Недостаточно реализованного товара «{0}» для возврата.", row.Article.FullName));

                saleWaybillRow.SetReturnCounts(saleWaybillRow.AcceptedReturnCount + row.ReturnCount, saleWaybillRow.ReceiptedReturnCount);

                // увеличиваем проведенное возвращенное от клиента кол-во
                var acceptedIndicator = CreateAcceptedReturnFromClientIndicator(waybill.AcceptanceDate.Value, row, 1);                
                acceptedIndicators.Add(acceptedIndicator);

                // увеличиваем возвраты на дату проводки накладной реализации
                // TODO: не самый оптимальный вариант расчета. В итоге тянет из БД данные для каждой позиции накладной возврата
                var bySaleAcceptanceDateIndicator = CreateReturnFromClientBySaleAcceptanceDateIndicator(row, 1);

                returnFromClientBySaleAcceptanceDateIndicatorService.Update(row.SaleWaybillRow.SaleWaybill.AcceptanceDate.Value,
                    waybill.Deal.Id, waybill.Curator.Id, receiptWaybillRepository.GetRowSubQuery(row.ReceiptWaybillRow.Id),
                    new List<ReturnFromClientBySaleAcceptanceDateIndicator>() { bySaleAcceptanceDateIndicator });

                // увеличиваем возвраты на дату отгрузки накладной реализации
                // TODO: не самый оптимальный вариант расчета. В итоге тянет из БД данные для каждой позиции накладной возврата
                var bySaleShippingDateIndicator = CreateReturnFromClientBySaleShippingDateIndicator(row, 1);

                returnFromClientBySaleShippingDateIndicatorService.Update(row.SaleWaybillRow.SaleWaybill.As<ExpenditureWaybill>().ShippingDate.Value,
                    waybill.Deal.Id, waybill.Curator.Id, receiptWaybillRepository.GetRowSubQuery(row.ReceiptWaybillRow.Id),
                    new List<ReturnFromClientBySaleShippingDateIndicator>() { bySaleShippingDateIndicator });
            }

            acceptedReturnFromClientIndicatorService.Update(waybill.AcceptanceDate.Value, waybill.Deal.Id, waybill.Curator.Id,
                returnFromClientWaybillRepository.GetArticleBatchesSubquery(waybill.Id), acceptedIndicators);
        }

        /// <summary>
        /// Пересчет показателей возвратов от клиентов при отмене проводки накладной возврата от клиента
        /// </summary>
        public void ReturnFromClientWaybillAcceptanceCancelled(ReturnFromClientWaybill waybill)
        {
            // проведенные возвраты
            var acceptedIndicators = new List<AcceptedReturnFromClientIndicator>();
            // возвраты на дату проводки накладной реализации
            var bySaleAcceptanceDateIndicators = new List<ReturnFromClientBySaleAcceptanceDateIndicator>();
            // возвраты на дату отгрузки накладной реализации
            var bySaleShippingDateIndicators = new List<ReturnFromClientBySaleShippingDateIndicator>();

            foreach (var row in waybill.Rows)
            {
                ValidationUtils.Assert(row.SaleWaybillRow.SaleWaybill.Is<ExpenditureWaybill>(),
                    "Расчет показателя для данного типа накладной реализации еще не реализован.");
                
                var saleWaybillRow = row.SaleWaybillRow.As<ExpenditureWaybillRow>();

                ValidationUtils.Assert(saleWaybillRow.AcceptedReturnCount >= row.ReturnCount,
                    String.Format("Недостаточно проведенных возвратов по товару {0} для отмены проводки.", row.Article.FullName));

                saleWaybillRow.SetReturnCounts(saleWaybillRow.AcceptedReturnCount - row.ReturnCount, saleWaybillRow.ReceiptedReturnCount);

                // уменьшаем проведенное возвращенное от клиента кол-во
                var acceptedIndicator = CreateAcceptedReturnFromClientIndicator(waybill.AcceptanceDate.Value, row, -1);                
                acceptedIndicators.Add(acceptedIndicator);

                // уменьшаем возвраты на дату проводки накладной реализации
                // TODO: не самый оптимальный вариант расчета. В итоге тянет из БД данные для каждой позиции накладной возврата
                var bySaleAcceptanceDateIndicator = CreateReturnFromClientBySaleAcceptanceDateIndicator(row, -1);

                returnFromClientBySaleAcceptanceDateIndicatorService.Update(row.SaleWaybillRow.SaleWaybill.AcceptanceDate.Value,
                    waybill.Deal.Id, waybill.Curator.Id, receiptWaybillRepository.GetRowSubQuery(row.ReceiptWaybillRow.Id),
                    new List<ReturnFromClientBySaleAcceptanceDateIndicator>() { bySaleAcceptanceDateIndicator });

                // уменьшаем возвраты на дату отгрузки накладной реализации
                // TODO: не самый оптимальный вариант расчета. В итоге тянет из БД данные для каждой позиции накладной возврата
                var bySaleShippingDateIndicator = CreateReturnFromClientBySaleShippingDateIndicator(row, -1);

                returnFromClientBySaleShippingDateIndicatorService.Update(row.SaleWaybillRow.SaleWaybill.As<ExpenditureWaybill>().ShippingDate.Value,
                    waybill.Deal.Id, waybill.Curator.Id, receiptWaybillRepository.GetRowSubQuery(row.ReceiptWaybillRow.Id),
                    new List<ReturnFromClientBySaleShippingDateIndicator>() { bySaleShippingDateIndicator });
            }

            acceptedReturnFromClientIndicatorService.Update(waybill.AcceptanceDate.Value, waybill.Deal.Id, waybill.Curator.Id,
                returnFromClientWaybillRepository.GetArticleBatchesSubquery(waybill.Id), acceptedIndicators);
        }
         
        #endregion

        #region Приемка накладной возврата от клиента и отмена приемки
        
        /// <summary>
        /// Пересчет показателей возвратов от клиентов при приемке накладной возврата от клиента
        /// </summary>
        public void ReturnFromClientWaybillFinalized(ReturnFromClientWaybill waybill)
        {
            var receiptedIndicators = new List<ReceiptedReturnFromClientIndicator>();

            foreach (var row in waybill.Rows)
            {
                ValidationUtils.Assert(row.SaleWaybillRow.SaleWaybill.Is<ExpenditureWaybill>(),
                    "Расчет показателя для данного типа накладной реализации еще не реализован.");
                
                var saleWaybillRow = row.SaleWaybillRow.As<ExpenditureWaybillRow>();

                ValidationUtils.Assert(saleWaybillRow.AcceptedReturnCount >= row.ReturnCount,
                    String.Format("Недостаточно проведенного возвращенного товара «{0}» для окончательной приемки возврата.", row.Article.FullName));

                saleWaybillRow.SetReturnCounts(saleWaybillRow.AcceptedReturnCount - row.ReturnCount, 
                    saleWaybillRow.ReceiptedReturnCount + row.ReturnCount);
                
                // увеличиваем точное возвращенное от клиента кол-во
                var receiptedIndicator = CreateReceiptedReturnFromClientIndicator(waybill.ReceiptDate.Value, row, 1);
                receiptedIndicators.Add(receiptedIndicator);
            }

            receiptedReturnFromClientIndicatorService.Update(waybill.ReceiptDate.Value, waybill.Deal.Id, waybill.Curator.Id,
                returnFromClientWaybillRepository.GetArticleBatchesSubquery(waybill.Id), receiptedIndicators);
        }

        /// <summary>
        /// Пересчет показателей возвратов от клиентов при отмене приемки накладной возврата от клиента
        /// </summary>
        public void ReturnFromClientWaybillFinalizationCancelled(ReturnFromClientWaybill waybill)
        {
            var receiptedIndicators = new List<ReceiptedReturnFromClientIndicator>();

            foreach (var row in waybill.Rows)
            {
                ValidationUtils.Assert(row.SaleWaybillRow.SaleWaybill.Is<ExpenditureWaybill>(),
                    "Расчет показателя для данного типа накладной реализации еще не реализован.");
                
                var saleWaybillRow = row.SaleWaybillRow.As<ExpenditureWaybillRow>();
                    
                ValidationUtils.Assert(saleWaybillRow.ReceiptedReturnCount >= row.ReturnCount,
                    String.Format("Недостаточно возвращенного товара «{0}» для отмены приемки возврата.", row.Article.FullName));

                saleWaybillRow.SetReturnCounts(saleWaybillRow.AcceptedReturnCount + row.ReturnCount,
                    saleWaybillRow.ReceiptedReturnCount - row.ReturnCount);

                // уменьшаем точное возвращенное от клиента кол-во
                var receiptedIndicator = CreateReceiptedReturnFromClientIndicator(waybill.ReceiptDate.Value, row, -1);
                receiptedIndicators.Add(receiptedIndicator);
            }

            receiptedReturnFromClientIndicatorService.Update(waybill.ReceiptDate.Value, waybill.Deal.Id, waybill.Curator.Id,
                returnFromClientWaybillRepository.GetArticleBatchesSubquery(waybill.Id), receiptedIndicators);
            
        } 
        #endregion

        #region Вспомогательные методы
        
        /// <summary>
        /// Создание показателя проведенного возврата по параметрам
        /// </summary>
        private AcceptedReturnFromClientIndicator CreateAcceptedReturnFromClientIndicator(DateTime startDate, ReturnFromClientWaybillRow row, short sign)
        {
            ValidationUtils.Assert(sign == 1 || sign == -1, "Значение множителя может быть только 1 или -1.");

            var saleWaybillRow = row.SaleWaybillRow.As<ExpenditureWaybillRow>();
            var saleWaybill = saleWaybillRow.SaleWaybill;
            var teamId = row.SaleWaybillRow.SaleWaybill.Team.Id;
            var returnFromClientWaybillCuratorId = row.ReturnFromClientWaybill.Curator.Id;

            return new AcceptedReturnFromClientIndicator(startDate, row.Article.Id, saleWaybill.Curator.Id, returnFromClientWaybillCuratorId,
                row.ReceiptWaybillRow.ReceiptWaybill.Contractor.Id, saleWaybillRow.ExpenditureWaybill.SenderStorage.Id, /* именно МХ, с которого сделана реализация */ saleWaybill.Sender.Id,
                saleWaybill.Deal.Client.Id, saleWaybill.Deal.Id, saleWaybill.Deal.Contract.ContractorOrganization.Id,
				teamId, saleWaybill.Id, row.ReceiptWaybillRow.Id,
                sign * Math.Round(row.PurchaseCost * row.ReturnCount, 6),
                sign * Math.Round(row.ArticleAccountingPrice.AccountingPrice * row.ReturnCount, 2),
                sign * Math.Round((row.SalePrice ?? 0M) * row.ReturnCount, 2),
                sign * row.ReturnCount);
        }

        /// <summary>
        /// Создание показателя окончательно принятого возврата по параметрам
        /// </summary>
        private ReceiptedReturnFromClientIndicator CreateReceiptedReturnFromClientIndicator(DateTime startDate, ReturnFromClientWaybillRow row, short sign)
        {
            ValidationUtils.Assert(sign == 1 || sign == -1, "Значение множителя может быть только 1 или -1.");

            var saleWaybillRow = row.SaleWaybillRow.As<ExpenditureWaybillRow>();
            var saleWaybill = saleWaybillRow.SaleWaybill;
            var teamId = row.SaleWaybillRow.SaleWaybill.Team.Id;
            var returnFromClientWaybillCuratorId = row.ReturnFromClientWaybill.Curator.Id;

            return new ReceiptedReturnFromClientIndicator(startDate, row.Article.Id, saleWaybill.Curator.Id, returnFromClientWaybillCuratorId,
                row.ReceiptWaybillRow.ReceiptWaybill.Contractor.Id, saleWaybillRow.ExpenditureWaybill.SenderStorage.Id, /* именно МХ, с которого сделана реализация */ saleWaybill.Sender.Id, 
                saleWaybill.Deal.Client.Id, saleWaybill.Deal.Id, saleWaybill.Deal.Contract.ContractorOrganization.Id, 
				teamId, saleWaybill.Id, row.ReceiptWaybillRow.Id, 
                sign * Math.Round(row.PurchaseCost * row.ReturnCount, 6), 
                sign * Math.Round(row.ArticleAccountingPrice.AccountingPrice * row.ReturnCount, 2), 
                sign * Math.Round((row.SalePrice ?? 0M) * row.ReturnCount, 2), 
                sign * row.ReturnCount);
        }

        /// <summary>
        /// Создание показателя по возвратам на дату проводки накладной реализации
        /// </summary>
        private ReturnFromClientBySaleAcceptanceDateIndicator CreateReturnFromClientBySaleAcceptanceDateIndicator(ReturnFromClientWaybillRow row, short sign)
        {
            ValidationUtils.Assert(sign == 1 || sign == -1, "Значение множителя может быть только 1 или -1.");

            var saleWaybillRow = row.SaleWaybillRow.As<ExpenditureWaybillRow>();
            var saleWaybill = saleWaybillRow.SaleWaybill;
            var teamId = row.SaleWaybillRow.SaleWaybill.Team.Id;
            var returnFromClientWaybillCuratorId = row.ReturnFromClientWaybill.Curator.Id;

            return new ReturnFromClientBySaleAcceptanceDateIndicator(saleWaybill.AcceptanceDate.Value, row.Article.Id, saleWaybill.Curator.Id,
                returnFromClientWaybillCuratorId, row.ReceiptWaybillRow.ReceiptWaybill.Contractor.Id, saleWaybillRow.ExpenditureWaybill.SenderStorage.Id, /* именно МХ, с которого сделана реализация */
                saleWaybill.Sender.Id, saleWaybill.Deal.Client.Id, saleWaybill.Deal.Id, saleWaybill.Deal.Contract.ContractorOrganization.Id,
                teamId, saleWaybill.Id, row.ReceiptWaybillRow.Id,
                sign * Math.Round(row.PurchaseCost * row.ReturnCount, 6),
                sign * Math.Round(row.ArticleAccountingPrice.AccountingPrice * row.ReturnCount, 2),
                sign * Math.Round((row.SalePrice ?? 0M) * row.ReturnCount, 2),
                sign * row.ReturnCount);
        }

        /// <summary>
        /// Создание показателя по возвратам на дату отгрузки накладной реализации
        /// </summary>
        private ReturnFromClientBySaleShippingDateIndicator CreateReturnFromClientBySaleShippingDateIndicator(ReturnFromClientWaybillRow row, short sign)
        {
            ValidationUtils.Assert(sign == 1 || sign == -1, "Значение множителя может быть только 1 или -1.");

            var saleWaybillRow = row.SaleWaybillRow.As<ExpenditureWaybillRow>();
            var saleWaybill = saleWaybillRow.SaleWaybill;
            var teamId = row.SaleWaybillRow.SaleWaybill.Team.Id;
            var returnFromClientWaybillCuratorId = row.ReturnFromClientWaybill.Curator.Id;

            return new ReturnFromClientBySaleShippingDateIndicator(saleWaybillRow.ExpenditureWaybill.ShippingDate.Value, row.Article.Id, saleWaybill.Curator.Id,
                returnFromClientWaybillCuratorId, row.ReceiptWaybillRow.ReceiptWaybill.Contractor.Id, saleWaybillRow.ExpenditureWaybill.SenderStorage.Id, /* именно МХ, с которого сделана реализация */
                saleWaybill.Sender.Id, saleWaybill.Deal.Client.Id, saleWaybill.Deal.Id, saleWaybill.Deal.Contract.ContractorOrganization.Id,
                teamId, saleWaybill.Id, row.ReceiptWaybillRow.Id,
                sign * Math.Round(row.PurchaseCost * row.ReturnCount, 6),
                sign * Math.Round(row.ArticleAccountingPrice.AccountingPrice * row.ReturnCount, 2),
                sign * Math.Round((row.SalePrice ?? 0M) * row.ReturnCount, 2),
                sign * row.ReturnCount);
        }

        #endregion

        #endregion

        #region Получение списка доступных для возврата накладных реализации

        /// <summary>
        /// Получить фильтрованный список товаров, доступных к возврату
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <param name="deal">Сделка</param>
        /// <param name="team">Команда</param>
        /// <returns>Список товаров</returns>
        public IEnumerable<Article> GetAvailableToReturnArticleFilteredList(object state, Deal deal, Team team)
        {
            var articleList = GetAvailablyToReturnArticleList(deal, team);

            return articleRepository.GetFilteredListByCollection(state, articleList);
        }

        /// <summary>
        /// Возвращает список доступных для возврата товаров
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="team">Команда</param>
        /// <returns>Список товаров</returns>
        private IEnumerable<Article> GetAvailablyToReturnArticleList(Deal deal, Team team)
        {
            var articlesSoldCounts = shippedSaleIndicatorRepository.GetSoldCountByArticle(deal.Id, team.Id, DateTime.Now);

            // получаем кол-во проведенных (в том числе принятых) возвратов от клиента по каждому товару
            var acceptedReturnFromClients = acceptedReturnFromClientIndicatorRepository.GetReturnedCountByArticle(deal.Id, team.Id, DateTime.Now);

            var articleIds = new List<int>();

            // вычитание из реализованного кол-ва возвращенного кол-ва по каждому товару
            foreach (var articlesSoldCount in articlesSoldCounts)
            {
                var availableToReturnCount = articlesSoldCounts[articlesSoldCount.Key] - acceptedReturnFromClients[articlesSoldCount.Key];

                // если возвращен еще не весь реализованный товар - добавляем этот в список доступных к возврату товаров
                if (availableToReturnCount > 0)
                {
                    articleIds.Add(articlesSoldCount.Key);
                }
            }

            return articleRepository.GetList(articleIds);
        } 
        #endregion

        /// <summary>
        /// Расчет суммы накладной реализации с учетом всех сделанных по ней возвратов
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        public decimal CalculateSaleWaybillCostWithReturns(SaleWaybill sale, ReturnFromClientWaybill returnFromClientWaybillToExclude = null)
        {
            // Сумма всех возвратов по накладной реализации
            var returnedRow = returnFromClientWaybillRepository.Query<ReturnFromClientWaybillRow>();
            // TODO рефакторить, ну кто так через OneOf пишет? Надо выбрать в RetWaybillRow SaleWaybillRow, а в нем - SaleWaybill.Id !!! - Олег
            returnedRow.OneOf(x => x.SaleWaybillRow.Id, sale.Rows.Select(x => x.Id))
                .Restriction<ReturnFromClientWaybill>(x => x.ReturnFromClientWaybill)
                .Where(x => x.State == ReturnFromClientWaybillState.Receipted);

            if (returnFromClientWaybillToExclude != null)
            {
                returnedRow.Where(x => x.ReturnFromClientWaybill.Id != returnFromClientWaybillToExclude.Id);
            }

            var returnedSum = returnedRow.ToList<ReturnFromClientWaybillRow>()
                .Sum(x => x.SalePrice.Value * x.ReturnCount); // Отпускная цена уже должна быть установлена (иметь Value)

            // Вычисляем сумму накладной реализации
            decimal saleSum = sale.As<ExpenditureWaybill>().SalePriceSum;

            return saleSum - returnedSum;
        }        

        #endregion
    }
}
