using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    /// <summary>
    /// Служба разнесения оплат
    /// </summary>
    public class DealPaymentDocumentDistributionService : IDealPaymentDocumentDistributionService
    {
        #region Поля

        private readonly IDealPaymentDocumentRepository dealPaymentDocumentRepository;
        private readonly IReturnFromClientService returnFromClientService;
        private readonly ISaleWaybillRepository saleWaybillRepository;

        #endregion

        #region Конструкторы

        public DealPaymentDocumentDistributionService(IDealPaymentDocumentRepository dealPaymentDocumentRepository,
            IReturnFromClientService returnFromClientService,
            ISaleWaybillRepository saleWaybillRepository)
        {
            this.dealPaymentDocumentRepository = dealPaymentDocumentRepository;

            this.returnFromClientService = returnFromClientService;
            this.saleWaybillRepository = saleWaybillRepository;
        }

        #endregion

        #region Методы

        #region Разнесение платежного документа на список «сущность - сумма»

        /// <summary>
        /// Разнесение оплаты от клиента на список других сущностей. Создает разнесения платежного документа.
        /// Может остаться неразнесенный остаток.
        /// Если неразнесенной суммы оплаты от клиента недостаточно для разнесения, выбрасывается исключение.
        /// </summary>
        /// <param name="dealPaymentFromClient">Оплата от клиента для разнесения</param>
        /// <param name="dealPaymentDocumentDistributionInfoList">Список сущностей и сумм для разнесения</param>
        /// <param name="currentDate">Дата операции</param>
        public void DistributeDealPaymentFromClient(DealPaymentFromClient dealPaymentFromClient,
            IEnumerable<DealPaymentDocumentDistributionInfo> dealPaymentDocumentDistributionInfoList, DateTime currentDate)
        {
            DistributeDealPaymentDocument<DealPaymentFromClient>(dealPaymentFromClient, dealPaymentDocumentDistributionInfoList, currentDate);
        }

        /// <summary>
        /// Разнесение кредитовой корректировки сальдо на список других сущностей. Создает разнесения платежного документа.
        /// Может остаться неразнесенный остаток.
        /// Если неразнесенной суммы кредитовой корректировки сальдо недостаточно для разнесения, выбрасывается исключение.
        /// </summary>
        /// <param name="dealCreditInitialBalanceCorrection">Кредитовая корректировка сальдо для разнесения</param>
        /// <param name="dealPaymentDocumentDistributionInfoList">Список сущностей и сумм для разнесения</param>
        /// <param name="currentDate">Дата операции</param>
        public void DistributeDealCreditInitialBalanceCorrection(DealCreditInitialBalanceCorrection dealCreditInitialBalanceCorrection,
            IEnumerable<DealPaymentDocumentDistributionInfo> dealPaymentDocumentDistributionInfoList, DateTime currentDate)
        {
            DistributeDealPaymentDocument<DealCreditInitialBalanceCorrection>(dealCreditInitialBalanceCorrection, dealPaymentDocumentDistributionInfoList, currentDate);
        }

        /// <summary>
        /// Разнесение платежного документа на список других сущностей. Создает разнесения платежного документа.
        /// Может остаться неразнесенный остаток.
        /// Если неразнесенной суммы платежного документа недостаточно для разнесения, выбрасывается исключение.
        /// </summary>
        /// <param name="dealPaymentDocument">Платежный документ для разнесения</param>
        /// <param name="dealPaymentDocumentDistributionInfoList">Список сущностей и сумм для разнесения</param>
        /// <param name="currentDate">Дата операции</param>
        private void DistributeDealPaymentDocument<T>(T dealPaymentDocument, IEnumerable<DealPaymentDocumentDistributionInfo> dealPaymentDocumentDistributionInfoList, DateTime currentDate)
            where T : DealPaymentDocument
        {
            decimal undistributedSum = dealPaymentDocument.UndistributedSum;

            // информация о неразнесенных частях данного платежного документа
            var undistributedPartsInfo = GetDealPaymentUndistributedPartsInfoByDealPayment(dealPaymentDocument).ToList();

            foreach (var dealPaymentDocumentDistributionInfo in dealPaymentDocumentDistributionInfoList.OrderBy(x => x.OrdinalNumber))
            {
                if (!undistributedPartsInfo.Any()) break;
                
                if (dealPaymentDocumentDistributionInfo.Sum > 0)
                {
                    // Если поле SaleWaybill не null, разносим на SaleWaybill. Иначе не null должно быть поле DealDebitInitialBalanceCorrection
                    if (dealPaymentDocumentDistributionInfo.SaleWaybill != null)
                    {
                        PaySaleWaybill(undistributedPartsInfo, dealPaymentDocumentDistributionInfo.SaleWaybill, dealPaymentDocumentDistributionInfo.SaleWaybillDebtRemainder, 
                            dealPaymentDocumentDistributionInfo.Sum, currentDate);
                    }
                    else if(dealPaymentDocumentDistributionInfo.DealDebitInitialBalanceCorrection != null)
                    {
                        PayDealDebitInitialBalanceCorrection(undistributedPartsInfo, dealPaymentDocumentDistributionInfo.DealDebitInitialBalanceCorrection,
                            dealPaymentDocumentDistributionInfo.Sum, currentDate);
                    }
                    else 
                    {
                        throw new Exception("Неизвестный тип документа для разнесения оплаты.");
                    }
                }
            }

            ValidationUtils.Assert(dealPaymentDocument.UndistributedSum >= 0M, String.Format("Сумма для разнесения ({0} р.) превышает неразнесенную сумму оплаты ({1} р.).",
                (dealPaymentDocumentDistributionInfoList.Sum(x => x.Sum)).ForDisplay(ValueDisplayType.Money), undistributedSum.ForDisplay(ValueDisplayType.Money)));
        }

        #endregion

        #region Автоматическое разнесение подходящих платежных документов на создаваемые сущности, учитываемые со знаком «+» в сальдо по сделке

        /// <summary>
        /// Автоматическое разнесение подходящих платежных документов на создаваемый возврат оплаты клиенту
        /// </summary>
        /// <param name="dealPaymentToClient">Возврат оплаты клиенту для разнесения на него</param>
        /// <param name="currentDate">Дата операции</param>
        public void DistributeDealPaymentToClient(DealPaymentToClient dealPaymentToClient, DateTime currentDate)
        {
            // получение информации о всех неразнесенных частях оплат по сделке и команде
            var dealPaymentUndistributedPartsInfo = GetTotalDealPaymentUndistributedPartsInfo(dealPaymentToClient.Deal, dealPaymentToClient.Team);

            // неразнесенные части оплат на дату текущего платежого документа и их сумма
            var dealPaymentUndistributedPartsInfoOnDealPaymentDocumentDate = dealPaymentUndistributedPartsInfo.Where(x => x.AppearenceDate <= dealPaymentToClient.Date).ToList();
            var dealPaymentUndistributedPartsInfoOnDealPaymentDocumentDateSum = dealPaymentUndistributedPartsInfoOnDealPaymentDocumentDate.Sum(x => x.Sum);

            // сумма неразнесенных частей на дату должна быть не меньше неразнесенного остатка по текущему платежному документу
            ValidationUtils.Assert(dealPaymentUndistributedPartsInfoOnDealPaymentDocumentDateSum >= dealPaymentToClient.UndistributedSum,
                String.Format("Недостаточно средств на {0} для возврата оплаты клиенту. Доступно к возврату всего {1} руб.",
                    dealPaymentToClient.Date.ToFullDateTimeString(), dealPaymentUndistributedPartsInfoOnDealPaymentDocumentDateSum.ForDisplay(ValueDisplayType.Money)));

            // дата документа возврата оплаты и дебетовой корректировки должна быть больше даты освобождения денег на него.
            PayDealPaymentDocument(dealPaymentUndistributedPartsInfoOnDealPaymentDocumentDate, dealPaymentToClient, dealPaymentToClient.UndistributedSum, currentDate);
        }

        /// <summary>
        /// Автоматическое разнесение подходящих платежных документов на создаваемую дебетовую корректировку сальдо
        /// </summary>
        /// <param name="dealDebitInitialBalanceCorrection">Дебетовая корректировка сальдо для разнесения на него</param>
        /// <param name="currentDate">Дата операции</param>
        public void DistributeDealDebitInitialBalanceCorrection(DealDebitInitialBalanceCorrection dealDebitInitialBalanceCorrection, DateTime currentDate)
        {
            // получение информации о всех неразнесенных частях оплат по сделке и команде
            var dealPaymentUndistributedPartsInfo = GetTotalDealPaymentUndistributedPartsInfo(dealDebitInitialBalanceCorrection.Deal, dealDebitInitialBalanceCorrection.Team);

            // в данном случае нам не важна последовательность освобождения денег и полное разнесение денег на дебетовую корректировку
            PayDealPaymentDocument(dealPaymentUndistributedPartsInfo, dealDebitInitialBalanceCorrection, dealDebitInitialBalanceCorrection.UndistributedSum, currentDate);
        }

        #endregion

        #region Автоматическое разнесение подходящих платежных документов на накладные, меняющие свой статус

        #region На накладные реализации

        /// <summary>
        /// Попытаться оплатить накладную реализации из аванса (т.е. неразнесенных остатков платежных документов по сделке и команде).
        /// После разнесения может оставаться неоплаченный остаток у накладной или неразнесенные остатки платежных документов.
        /// Если накладная не имеет положительного остатка, расшифровки распределения оплаты не будут созданы.
        /// При полной оплате накладной реализации происходит установка признака того, что накладная полностью оплачена
        /// </summary>
        /// <param name="saleWaybill">Накладная реализации, на которую выполняется разнесение платежных документов</param>
        public void PaySaleWaybillOnAccept(SaleWaybill saleWaybill, DateTime currentDate)
        {
            // если сумма в ОЦ по накладной реализации = 0, выставляем признак полной оплаты и выходим
            if (saleWaybill.Is<ExpenditureWaybill>() && saleWaybill.As<ExpenditureWaybill>().SalePriceSum == 0)
            {
                saleWaybill.IsFullyPaid = true;
                return;
            }

            // При проводке данной накладной неоплаченный остаток равен сумме накладной в ОЦ
            var debtRemainder = saleWaybill.As<ExpenditureWaybill>().SalePriceSum;

            // получение информации о неразнесенных частях платежных документов
            var dealPaymentUndistributedPartsInfo = GetTotalDealPaymentUndistributedPartsInfo(saleWaybill.Deal, saleWaybill.Team);

            // разносим неоплаченные остатки на накладную
            PaySaleWaybill(dealPaymentUndistributedPartsInfo, saleWaybill, debtRemainder, debtRemainder, currentDate);
        }

        /// <summary>
        /// Отмена всех расшифровок распределения оплаты, разнесенных на данную накладную
        /// </summary>
        /// <param name="saleWaybill">Накладная реализации</param>
        public void CancelSaleWaybillPaymentDistribution(SaleWaybill saleWaybill)
        {
            // Если накладная имела нулевую сумму, ни одна расшифровка создана не была, но признак полной оплаты стоял. Сбрасываем его всегда
            saleWaybill.IsFullyPaid = false;

            // Получаем все расшифровки распределения оплаты по данной накладной реализации и кэшируем все их платежные документы и их коллекции расшифровок
            var dealPaymentDocumentDistributionList = dealPaymentDocumentRepository
                .GetDealPaymentDocumentDistributionsForDestinationSaleWaybills(saleWaybillRepository.GetSaleWaybillIQueryable(saleWaybill.Id));

            dealPaymentDocumentRepository.LoadSourceDealPaymentDocumentDistributions(dealPaymentDocumentDistributionList);

            // начинаем с удаления разнесения с максимальной суммой, т.е. отрицательные разнесения удаляем последними
            foreach (var dealPaymentDocumentDistribution in dealPaymentDocumentDistributionList.OrderByDescending(x => x.Sum))
            {
                dealPaymentDocumentDistribution.SourceDealPaymentDocument.RemoveDealPaymentDocumentDistribution(dealPaymentDocumentDistribution);
            }
        }

        #endregion

        #region На накладные возврата товаров

        /// <summary>
        /// Возврат оплаты по возвращенным позициям
        /// </summary>
        /// <param name="returnFromClientWaybill">Накладная возврата товаров от клиента</param>
        /// <param name="currentDate">Дата операции</param>
        public void ReturnPaymentToSales(ReturnFromClientWaybill returnFromClientWaybill, DateTime currentDate)
        {
            // Получаем идентификаторы всех накладных реализации, по которым сделан возврат
            var SQSalesIQueryable = saleWaybillRepository.GetRowsWithReturnsIQueryable(returnFromClientWaybill.Id);

            // Получаем все расшифровки распределения оплаты по данной накладной реализации и кэшируем все их платежные документы и их коллекции расшифровок
            var paymentDistributions = dealPaymentDocumentRepository.GetDealPaymentDocumentDistributionsForDestinationSaleWaybills(SQSalesIQueryable);
            dealPaymentDocumentRepository.LoadSourceDealPaymentDocumentDistributions(paymentDistributions);

            var saleWaybillList = saleWaybillRepository.GetSaleWaybillsByReturnFromClientWaybill(returnFromClientWaybill.Id);
            var returnedPaymentDistributions = dealPaymentDocumentRepository.GetDistributionsToReturnsFromClientWaybillsForDestinationSaleWaybills(SQSalesIQueryable);

            // Возвращаем оплаты по всем накладным реализации, создавая разнесения этих оплат с отрицательными суммами
            foreach (var saleWaybill in saleWaybillList)
            {
                // разнесения на текущую накладную реализации
                var paymentDistributionListBySale = paymentDistributions.Where(x => x.SaleWaybill == saleWaybill);

                // возвраты оплаты по возвратам товаров
                var returnedPaymentDistributionsBySale = returnedPaymentDistributions.Where(x => x.SaleWaybill == saleWaybill);

                // Получаем оплаты по накладной реализации
                var dealPaymentDocumentList = paymentDistributionListBySale.Where(x => x.SaleWaybill == saleWaybill).Select(x => x.SourceDealPaymentDocument).Distinct();

                // Стоимость накладной с учетом возвратов
                var saleWaybillCost = returnFromClientService.CalculateSaleWaybillCostWithReturns(saleWaybill);
                // Вычисляем сумму платежей по данной накладной реализации
                var paymentSum = paymentDistributionListBySale.Sum(x => x.Sum);
                // Вычисляем сумму ранее возвращенной оплаты по возвратам по данной накладной реализации (хранится отрицательной величиной)
                var returnedSum = returnedPaymentDistributionsBySale.Sum(x => -x.Sum);

                // Возвращаемая сумма оплат по накладной реализации. Она не может быть больше имеющейся оплаты
                var returnedPaid = (paymentSum - returnedSum > saleWaybillCost) ? paymentSum - returnedSum - saleWaybillCost : 0M;

                // Обновляем индикатор по оплатам накладной реализации
                var returnSumPerSale = returnFromClientWaybill.Rows.Where(x => x.SaleWaybillRow.SaleWaybill == saleWaybill).Sum(x => x.SalePrice * x.ReturnCount).Value;

                // Возвращаем средства начиная с последней оплаты (по времени создания)
                foreach (var dealPaymentDocument in dealPaymentDocumentList
                    .OrderByDescending(x => x.Date)
                    .OrderByDescending(x => x.CreationDate))
                {
                    if (returnedPaid == 0M) // Если вся сумма возвращена (или изначально равна нулю), то
                    {
                        break; // Завершаем цикл
                    }

                    // Вычисляем сумму, разнесенную на накладную реализации с данной оплаты
                    var paidSum = paymentDistributionListBySale
                        .Where(x => x.SourceDealPaymentDocument == dealPaymentDocument)
                        .Sum(x => x.Sum);
                    // Вычисляем уже возвращенную сумму по данной оплате (сумма возврата хранится отрицательной величиной)
                    var returnedPaidSum = returnedPaymentDistributionsBySale
                        .Where(x => x.SourceDealPaymentDocument == dealPaymentDocument)
                        .Sum(x => -x.Sum);

                    var rSum = Math.Min(paidSum - returnedPaidSum, returnedPaid);   //вычисляем сумму, которая должна быть возвращена по данной оплате

                    // Создаем возвратное разнесение оплаты. Возвращаемая сумма должна быть отрицательной.
                    var pd = new DealPaymentDocumentDistributionToReturnFromClientWaybill(dealPaymentDocument, returnFromClientWaybill, saleWaybill, -rSum,
                        returnFromClientWaybill.ReceiptDate.Value, currentDate);

                    // Добавляем разнесение в оплату
                    dealPaymentDocument.AddDealPaymentDocumentDistribution(pd);

                    returnedPaid -= rSum;   // Уменьшаем возвращаемую сумму на размер возвращенной оплаты (Вычисляем остаток возврата)
                }

                //---------- Проверяем наличие полной оплаты накладных реализации и устанавливаем им признак полной оплаты ------------

                // Оплачена ли накладная полностью? Устанавливаем признак полной оплаты (Имеющийся излишек оплаты был уже возвращен)
                saleWaybill.IsFullyPaid = (paymentSum - returnedSum - saleWaybillCost >= 0M);
            }
        }

        /// <summary>
        /// Отмена возврата оплаты по возвращенным позициям
        /// </summary>
        /// <param name="returnFromClientWaybill">Накладная возврата товаров от клиента</param>
        /// <param name="receiptDate">Дата приемки накладной реализации товаров</param>
        public void CancelPaymentReturnToSales(ReturnFromClientWaybill returnFromClientWaybill, DateTime receiptDate)
        {
            // Получаем все возвраты оплаты по данной накладной возврата
            var returnedDistributions = dealPaymentDocumentRepository.GetDistributionsToReturnFromClientWaybillsForDestinationReturnFromClientWaybills(returnFromClientWaybill.Id);

            foreach (var distribution in returnedDistributions)
            {
                // Проверяем возможность отмены разнесения
                ValidationUtils.Assert(-distribution.Sum <= distribution.SourceDealPaymentDocument.UndistributedSum,
                    "Невозможно отменить приемку накладной возврата, так как недостаточно средств для оплаты накладной реализации.");

                distribution.SourceDealPaymentDocument.RemoveDealPaymentDocumentDistribution(distribution); // Удаляем разнесение из оплаты
            }

            //---------- Проверяем наличие полной оплаты накладных реализации и устанавливаем им признак полной оплаты ------------

            // Получаем идентификаторы всех накладных реализации, по которым отменен возврат
            var SQSalesIQueryable = saleWaybillRepository.GetRowsWithReturnsIQueryable(returnFromClientWaybill.Id);

            // Получаем все расшифровки распределения оплаты по данной накладной реализации
            var paymentDistributions = dealPaymentDocumentRepository.GetDealPaymentDocumentDistributionsForDestinationSaleWaybills(SQSalesIQueryable);

            var saleWaybillList = saleWaybillRepository.GetSaleWaybillsByReturnFromClientWaybill(returnFromClientWaybill.Id);

            // Проверяем платежные документы по всем накладным реализации
            foreach (var saleWaybill in saleWaybillList)
            {
                // Стоимость накладной реализации с учетом накладных возврата товаров, но без отменяемой накладной возврата товаров
                var saleWaybillCost = returnFromClientService.CalculateSaleWaybillCostWithReturns(saleWaybill, returnFromClientWaybill);
                // Вычисляем сумму платежей по данной накладной реализации
                var paymentSum = paymentDistributions.Where(x => x.SaleWaybill.Id == saleWaybill.Id).Sum(x => x.Sum);
                // Вычисляем сумму возвращенной оплаты по возвратам по данной накладной реализации (хранится отрицательной величиной) без отменяемого возврата
                var returnedSum = returnedDistributions.Where(x => x.SaleWaybill.Id == saleWaybill.Id && x.ReturnFromClientWaybill.Id != returnFromClientWaybill.Id).Sum(x => -x.Sum);

                // Оплачена ли накладная полностью? Устанавливаем признак полной оплаты
                saleWaybill.IsFullyPaid = (paymentSum - returnedSum - saleWaybillCost == 0M);

                // Обновляем индикатор по оплатам накладной реализации
                var returnSumPerSale = returnFromClientWaybill.Rows.Where(x => x.SaleWaybillRow.SaleWaybill == saleWaybill).Sum(x => x.SalePrice * x.ReturnCount).Value;
                var returnedPaid = -returnedDistributions.Where(x => x.SaleWaybill == saleWaybill).Sum(x => x.Sum);
            }
        }

        #endregion

        #endregion

        #region Разнесение платежного документа на одну сущность конкретной суммой

        #region На накладную реализации

        /// <summary>
        /// Разнести неразнесенные части платежных документов на накладную реализации.
        /// После разнесения у накладной реализации может оставаться неоплаченный остаток.
        /// Если до разнесения накладная реализации не имеет положительного неоплаченного остатка, разнесение платежного документа не будет создано.
        /// При полной оплате накладной реализации происходит установка признака того, что накладная реализации полностью оплачена.
        /// </summary>
        /// <param name="dealPaymentUndistributedPartsInfo">Список неразнесенных частей платежных документов</param>
        /// <param name="saleWaybill">Накладная реализации (единственная), на которую выполняется разнесение платежных документов</param>
        /// <param name="debtRemainder">Текущий неоплаченный остаток по накладной</param>
        /// <param name="sumToDistribute">Сумма к разнесению</param>
        /// <param name="currentDate">Дата операции</param>
        private void PaySaleWaybill(List<DealPaymentUndistributedPartInfo> dealPaymentUndistributedPartsInfo, SaleWaybill saleWaybill, decimal debtRemainder, decimal sumToDistribute, DateTime currentDate)
        {
            // разносим неразнесенные части оплат
            foreach (var distributionPartInfo in dealPaymentUndistributedPartsInfo.OrderBy(x => x.AppearenceDate))
            {
                var dealPaymentDocument = distributionPartInfo.DealPaymentDocument;

                // cумма создаваемого разнесения платежного документа
                decimal sum = Math.Min(distributionPartInfo.Sum, sumToDistribute);
                // вычитаем разносимую сумму
                distributionPartInfo.Sum -= sum;
                sumToDistribute -= sum;
                
                // дата создаваемого разнесения
                var distributionDate = DateTimeUtils.GetMaxDate(distributionPartInfo.AppearenceDate, saleWaybill.AcceptanceDate.Value);

                ValidationUtils.Assert(dealPaymentDocument.Is<DealPaymentFromClient>() || dealPaymentDocument.Is<DealCreditInitialBalanceCorrection>(),
                    "Платежный документ имеет недопустимый тип.");

                ValidationUtils.Assert(sum > 0, "Сумма для разнесения должна быть положительной.");

                if (saleWaybill.Is<ExpenditureWaybill>()) // Если появятся еще типы накладных реализации, вместо "накладная реализации товаров" выводить их названия
                {
                    ExpenditureWaybill expenditureWaybill = saleWaybill.As<ExpenditureWaybill>();

                    ValidationUtils.Assert(expenditureWaybill.IsAccepted, String.Format("Невозможно разнести платежный документ на накладную реализации товаров {0} со статусом «{1}».",
                        expenditureWaybill.Name, expenditureWaybill.State.GetDisplayName()));
                    
                    ValidationUtils.Assert(sum <= debtRemainder, String.Format("Сумма для разнесения ({0} р.) превышает неоплаченный остаток накладной реализации товаров {1} ({2} р.)",
                        sum.ForDisplay(ValueDisplayType.Money), expenditureWaybill.Name, debtRemainder.ForDisplay(ValueDisplayType.Money)));
                    
                    ValidationUtils.Assert(dealPaymentDocument.Team == saleWaybill.Team, 
                        String.Format("Невозможно оплатить накладную реализации «{0}», т.к. она относится к другой команде.", saleWaybill.Name));

                    // формируем разнесение
                    var dealPaymentDocumentDistributionToSaleWaybill = new DealPaymentDocumentDistributionToSaleWaybill(dealPaymentDocument, expenditureWaybill, sum,
                        distributionDate, currentDate) { SourceDistributionToReturnFromClientWaybill = distributionPartInfo.DealPaymentDocumentDistributionToReturnFromClientWaybill };

                    dealPaymentDocument.AddDealPaymentDocumentDistribution(dealPaymentDocumentDistributionToSaleWaybill);

                    debtRemainder -= sum;

                    // Если накладная реализации товаров полностью оплачена, ставим ей признак полной оплаты
                    expenditureWaybill.IsFullyPaid = debtRemainder <= 0M;
                }

                // и выходим
                if (saleWaybill.IsFullyPaid)
                {
                    break;
                }
            }

            // удаляем полностью разнесенные части
            var undistributedPartsToDelete = dealPaymentUndistributedPartsInfo.Where(x => x.Sum == 0).ToList();

            foreach (var item in undistributedPartsToDelete)
            {
                dealPaymentUndistributedPartsInfo.Remove(item);
            }
        }

        #endregion

        #region На платежный документ по сделке

        /// <summary>
        /// Разнести платежный документ на дебетовую корректировку сальдо (заданной наперед суммой).
        /// После разнесения у дебетовой корректировки сальдо может оставаться неоплаченный остаток.
        /// Если до разнесения дебетовой корректировки сальдо не имеет положительного неоплаченного остатка, разнесение платежного документа не будет создано.
        /// При полной оплате дебетовой корректировки сальдо происходит установка признака того, что тот полностью оплачен.
        /// </summary>
        /// <param name="dealPaymentUndistributedPartsInfo">Информация о неразнесенных остатках платежных документов</param>
        /// <param name="dealDebitInitialBalanceCorrection">Дебетовая корректировка сальдо (единственный), на который выполняется разнесение платежного документа</param>
        /// <param name="sumToDistribute">Сумма для разнесения</param>
        /// <param name="currentDate">Дата операции</param>
        private void PayDealDebitInitialBalanceCorrection(List<DealPaymentUndistributedPartInfo> dealPaymentUndistributedPartsInfo, DealDebitInitialBalanceCorrection dealDebitInitialBalanceCorrection,
            decimal sumToDistribute, DateTime currentDate)
        {
            PayDealPaymentDocument(dealPaymentUndistributedPartsInfo, dealDebitInitialBalanceCorrection, sumToDistribute, currentDate);
        }

        /// <summary>
        /// Разнести неразнесенные части платежных документов на платежный документ-«назначение» (заданной наперед суммой).
        /// После разнесения у «источника» может оставаться неоплаченный остаток.
        /// Если до разнесения «назначение» не имеет положительного неоплаченного остатка, разнесение платежного документа не будет создано.
        /// При полной оплате «назначения» происходит установка признака того, что оно полностью оплачено.
        /// </summary>
        /// <param name="sourceDealPaymentUndistributedPartsInfo">Информация о неразнесенных остатках платежных документов</param>
        /// <param name="destinationDealPaymentDocument">Платежный документ, на который выполняется разнесение</param>
        /// <param name="sumToDistribute">Сумма для разнесения</param>
        /// <param name="currentDate">Дата операции</param>
        private void PayDealPaymentDocument(List<DealPaymentUndistributedPartInfo> sourceDealPaymentUndistributedPartsInfo, DealPaymentDocument destinationDealPaymentDocument,
            decimal sumToDistribute, DateTime currentDate)
        {
            // разносим неразнесенные части оплат
            foreach (var distributionPartInfo in sourceDealPaymentUndistributedPartsInfo.OrderBy(x => x.AppearenceDate))
            {
                var sourceDealPaymentDocument = distributionPartInfo.DealPaymentDocument;

                // cумма создаваемого разнесения платежного документа
                var sum = Math.Min(distributionPartInfo.Sum, sumToDistribute);
                // вычитаем разносимую сумму
                distributionPartInfo.Sum -= sum;
                sumToDistribute -= sum;

                // дата создаваемого разнесения
                var distributionDate = DateTimeUtils.GetMaxDate(distributionPartInfo.AppearenceDate, destinationDealPaymentDocument.Date);

                ValidationUtils.Assert(sourceDealPaymentDocument.Type.ContainsIn(DealPaymentDocumentType.DealPaymentFromClient, DealPaymentDocumentType.DealCreditInitialBalanceCorrection),
                    "Разносимый платежный документ имеет недопустимый тип.");
                ValidationUtils.Assert(destinationDealPaymentDocument.Type.ContainsIn(DealPaymentDocumentType.DealPaymentToClient, DealPaymentDocumentType.DealDebitInitialBalanceCorrection),
                    "Платежный документ, на который выполняется разнесение, имеет недопустимый тип.");

                ValidationUtils.Assert(sum > 0, "Сумма для разнесения должна быть положительной.");
                ValidationUtils.Assert(sourceDealPaymentDocument.UndistributedSum >= sum, "Неразнесенный остаток по платежному документу меньше суммы для разнесения.");

                ValidationUtils.Assert(sum <= destinationDealPaymentDocument.UndistributedSum,
                    String.Format("Сумма для разнесения ({0} р.) превышает неоплаченный остаток платежного документа, на который выполняется разнесение ({1} р.)",
                    sum.ForDisplay(ValueDisplayType.Money), destinationDealPaymentDocument.UndistributedSum.ForDisplay(ValueDisplayType.Money)));

                ValidationUtils.Assert(sourceDealPaymentDocument.Team == destinationDealPaymentDocument.Team,
                    "Разносимый платежный документ и платежный документ, на который выполняется разнесение, должны относиться к одной команде.");

                var dealPaymentDocumentDistributionToDealPaymentDocument = new DealPaymentDocumentDistributionToDealPaymentDocument(sourceDealPaymentDocument,
                    destinationDealPaymentDocument, sum, distributionDate, currentDate) 
                    { SourceDistributionToReturnFromClientWaybill = distributionPartInfo.DealPaymentDocumentDistributionToReturnFromClientWaybill };

                sourceDealPaymentDocument.AddDealPaymentDocumentDistribution(dealPaymentDocumentDistributionToDealPaymentDocument); // IsFullyDistributed у обоих ставится само

                if (destinationDealPaymentDocument.UndistributedSum == 0M) // Если вся сумма оплачена, завершаем цикл
                {
                    break;
                }
            }

            // удаляем полностью разнесенные части
            var undistributedPartsToDelete = sourceDealPaymentUndistributedPartsInfo.Where(x => x.Sum == 0).ToList();

            foreach (var item in undistributedPartsToDelete)
            {
                sourceDealPaymentUndistributedPartsInfo.Remove(item);
            }
        }

        #endregion

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Получение информации о всех неразнесенных частях оплаты по сделке и команде
        /// </summary>
        private List<DealPaymentUndistributedPartInfo> GetTotalDealPaymentUndistributedPartsInfo(Deal deal, Team team)
        {
            // получаем неполностью разнесенные платежные документы
            var undistributedDealPaymentDocumentList = dealPaymentDocumentRepository
                .GetUndistributedDealPaymentDocumentList(deal.Id, team.Id);

            // получение информации о неразнесенных частях платежных документов
            var dealPaymentUndistributedPartsInfo = new List<DealPaymentUndistributedPartInfo>();

            foreach (var undistributedDealPaymentDocument in undistributedDealPaymentDocumentList)
            {
                // получаем для платежных документов словарь с информацией о частях неразнесенной оплаты и датах их появления
                dealPaymentUndistributedPartsInfo.AddRange(GetDealPaymentUndistributedPartsInfoByDealPayment(undistributedDealPaymentDocument));
            }

            return dealPaymentUndistributedPartsInfo;
        }

        /// <summary>
        /// Получение для платежного документа информации о неразнесенных частях оплаты и датах их появления
        /// </summary>
        /// <param name="dealPaymentDocument">Платежный документ по сделке</param>
        private IEnumerable<DealPaymentUndistributedPartInfo> GetDealPaymentUndistributedPartsInfoByDealPayment(DealPaymentDocument dealPaymentDocument)
        {
            // информация о неразнесенных частях оплаты и датах их появления
            var paymentDistributionPartsInfo = new List<DealPaymentUndistributedPartInfo>();

            // добавляем все отрицательные разнесения (освободились после возврата оплаты от клиента)
            foreach (var distribution in dealPaymentDocument.Distributions.Where(x => x.Sum < 0 && x is DealPaymentDocumentDistributionToReturnFromClientWaybill))
            {
                // получаем сумму разнесений из данного разнесения на накладные реализации
                var distributionsToSaleWaybillSum = dealPaymentDocument.Distributions.Where(x => x.Sum > 0 && x is DealPaymentDocumentDistributionToSaleWaybill &&
                    (x as DealPaymentDocumentDistributionToSaleWaybill).SourceDistributionToReturnFromClientWaybill == distribution).Sum(x => x.Sum);
                
                // получаем сумму разнесений из данного разнесения на платежные документы
                var distributionsToDealPaymentDocumentSum = dealPaymentDocument.Distributions.Where(x => x.Sum > 0 && x is DealPaymentDocumentDistributionToDealPaymentDocument &&
                    (x as DealPaymentDocumentDistributionToDealPaymentDocument).SourceDistributionToReturnFromClientWaybill == distribution).Sum(x => x.Sum);

                // получаем чистый остаток для разнесения
                // для получения положительной суммы distribution.Sum берем с минусом
                var distributionSum = (-distribution.Sum) - distributionsToSaleWaybillSum - distributionsToDealPaymentDocumentSum;

                if (distributionSum != 0)
                {
                    paymentDistributionPartsInfo.Add(new DealPaymentUndistributedPartInfo(dealPaymentDocument, distribution.DistributionDate, distributionSum) 
                        { DealPaymentDocumentDistributionToReturnFromClientWaybill = (distribution as DealPaymentDocumentDistributionToReturnFromClientWaybill) });
                }
            }

            // добавляем в словарь сумму «чистого» неразнесенного остатка, т.е. который еще никогда никуда не разносился
            var clearRest = dealPaymentDocument.UndistributedSum - paymentDistributionPartsInfo.Sum(x => x.Sum);

            if (clearRest != 0)
            {
                paymentDistributionPartsInfo.Add(new DealPaymentUndistributedPartInfo(dealPaymentDocument, dealPaymentDocument.Date, clearRest));
            }

            return paymentDistributionPartsInfo;
        }

        #endregion

        #endregion
    }
}
