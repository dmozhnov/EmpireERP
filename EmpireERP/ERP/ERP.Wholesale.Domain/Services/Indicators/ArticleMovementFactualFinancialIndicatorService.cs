using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    /// <summary>
    /// Служба расчета финансовых показателей товародвижения, изменения ТМЦ
    /// </summary>
    public class ArticleMovementFactualFinancialIndicatorService : IArticleMovementFactualFinancialIndicatorService
    {
        #region Поля

        private readonly IArticleMovementFactualFinancialIndicatorRepository articleMovementFactualFinancialIndicatorRepository;
        private readonly IReceiptWaybillRepository receiptWaybillRepository;
        private readonly IMovementWaybillRepository movementWaybillRepository;
        private readonly IWriteoffWaybillRepository writeoffWaybillRepository;
        private readonly IExpenditureWaybillRepository expenditureWaybillRepository;
        private readonly IReturnFromClientWaybillRepository returnFromClientWaybillRepository;

        #endregion

        #region Конструкторы

        public ArticleMovementFactualFinancialIndicatorService(IArticleMovementFactualFinancialIndicatorRepository articleMovementFactualFinancialIndicatorRepository,
            IReceiptWaybillRepository receiptWaybillRepository, IMovementWaybillRepository movementWaybillRepository, IWriteoffWaybillRepository writeoffWaybillRepository,
            IExpenditureWaybillRepository expenditureWaybillRepository, IReturnFromClientWaybillRepository returnFromClientWaybillRepository)
        {
            this.articleMovementFactualFinancialIndicatorRepository = articleMovementFactualFinancialIndicatorRepository;
            this.receiptWaybillRepository = receiptWaybillRepository;
            this.movementWaybillRepository = movementWaybillRepository;
            this.writeoffWaybillRepository = writeoffWaybillRepository;
            this.expenditureWaybillRepository = expenditureWaybillRepository;
            this.returnFromClientWaybillRepository = returnFromClientWaybillRepository;
        }

        #endregion

        #region Методы

        #region Методы сбора аналитики

        /// <summary>
        /// Получение списка показателей для списка МХ на определенную дату
        /// </summary>
        /// <param name="storageIDs"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public IList<ArticleMovementFactualFinancialIndicator> GetListOnDate(IEnumerable<short> storageIDs, DateTime date)
        {
            //Финансовые операции, соответствующие складам в роли получателей
            var incomingIndicators = articleMovementFactualFinancialIndicatorRepository.GetIncomingIndicatorsList(storageIDs, date);
                
            //Финансовые операции, соответствующие складам в роли отправителей
            var outgoingIndicators = articleMovementFactualFinancialIndicatorRepository.GetOutgoingIndicatorsList(storageIDs, date);
            
            var indicators = new List<ArticleMovementFactualFinancialIndicator>();
            indicators.AddRange(incomingIndicators);
            indicators.AddRange(outgoingIndicators);

            return indicators;
        }

        /// <summary>
        /// Метод производит вычитание из базового индикатора значений вычитаемого и возвращает измененный базовый индикатор
        /// </summary>
        /// <param name="baseIndicators"></param>
        /// <param name="subtracterIndicators"></param>
        /// <returns></returns>
        public IEnumerable<ArticleMovementFactualFinancialIndicator> IndicatorSubtraction(IList<ArticleMovementFactualFinancialIndicator> baseIndicators,
            IEnumerable<ArticleMovementFactualFinancialIndicator> subtracterIndicators)
        {
            foreach (var indicator in baseIndicators)
            {
                var substracterIndicator = subtracterIndicators
                    .Where(x =>
                        ((x.RecipientId == null && indicator.RecipientId == null) || (x.RecipientId == indicator.RecipientId)) &&
                        ((x.RecipientStorageId == null && indicator.RecipientStorageId == null) || (x.RecipientStorageId == indicator.RecipientStorageId)) &&
                        ((x.SenderId == null && indicator.SenderId == null) || (x.SenderId == indicator.SenderId)) &&
                        ((x.SenderStorageId == null && indicator.SenderStorageId == null) || (x.SenderStorageId == indicator.SenderStorageId)) &&
                        x.ArticleMovementOperationType == indicator.ArticleMovementOperationType).FirstOrDefault();

                if (substracterIndicator != null)
                {
                    indicator.PurchaseCostSum -= substracterIndicator.PurchaseCostSum;
                    indicator.AccountingPriceSum -= substracterIndicator.AccountingPriceSum;
                    indicator.SalePriceSum -= substracterIndicator.SalePriceSum;
                }
            }

            var indicatorsForDeletion = baseIndicators.Where(x => x.CountersAreZero).ToList();
            foreach (var indicator in indicatorsForDeletion)
            {
                baseIndicators.Remove(indicator);
            }

            return baseIndicators;
        }

        #endregion

        #region Методы пересчета индикатора

        /// <summary>
        /// Обновление значения показателя
        /// </summary>
        /// <param name="date"></param>
        /// <param name="senderId"></param>
        /// <param name="senderStorageId"></param>
        /// <param name="recipientId"></param>
        /// <param name="recipientStorageId"></param>
        /// <param name="articleMovementOperationType"></param>
        /// <param name="waybillId"></param>
        /// <param name="purchaseCostSumDelta">Сумма изменения в закупочных ценах</param>
        /// <param name="salePriceSumDelta">Сумма изменения в отпускных ценах</param>
        /// <param name="accountingPriceSumDelta">Сумма изменения в учетных ценах</param>
        public void Update(DateTime startDate, int? senderId, short? senderStorageId, int? recipientId, short? recipientStorageId, 
            ArticleMovementOperationType articleMovementOperationType, Guid waybillId, decimal purchaseCostSumDelta, decimal accountingPriceSumDelta, 
            decimal salePriceSumDelta)
        {
            var list = articleMovementFactualFinancialIndicatorRepository.GetFrom(startDate, senderId, senderStorageId, recipientId, recipientStorageId, articleMovementOperationType);

            // если нет показателя с датой окончания >= startDate или = null - добавляем его
            if (!list.Any())
            {
                var ind = new ArticleMovementFactualFinancialIndicator(startDate, senderId, senderStorageId, recipientId, recipientStorageId,
                    articleMovementOperationType, waybillId, purchaseCostSumDelta, accountingPriceSumDelta, salePriceSumDelta);

                articleMovementFactualFinancialIndicatorRepository.Save(ind);
            }
            else
            {
                // индикатор с минимальной датой начала среди индикаторов из list и сама эта дата
                var firstIndicator = list.OrderBy(x => x.StartDate).FirstOrDefault();
                var minimalStartDate = firstIndicator.StartDate;

                // если дата нового показателя совпадает с датой начала минимального показателя из list
                if (startDate == minimalStartDate)
                {
                    firstIndicator.PurchaseCostSum += purchaseCostSumDelta;         // меняем значение показателя
                    firstIndicator.AccountingPriceSum += accountingPriceSumDelta;
                    firstIndicator.SalePriceSum += salePriceSumDelta;                        
                }
                // если дата нового показателя меньше даты начала минимального показателя из list
                else if(startDate < minimalStartDate)
                {
                    // добавляем новый показатель
                    var _new = new ArticleMovementFactualFinancialIndicator(startDate, senderId, senderStorageId, recipientId, 
                        recipientStorageId, articleMovementOperationType, waybillId, 
                        purchaseCostSumDelta, accountingPriceSumDelta, salePriceSumDelta);
                        
                    _new.EndDate = firstIndicator.StartDate;
                    
                    articleMovementFactualFinancialIndicatorRepository.Save(_new);

                    firstIndicator.PreviousId = _new.Id;    // устанавливаем ссылку на добавленный показатель
                }
                // если дата нового показателя больше даты начала минимального показателя из list
                else
                {
                    firstIndicator.EndDate = startDate;  // завершаем действие текущего показателя

                    // ищем следующий после firstIndicator показатель
                    var secondIndicator = list.Where(x => x.StartDate > startDate).OrderBy(x => x.StartDate).FirstOrDefault();

                    // добавляем новый показатель после текущего
                    var _new = new ArticleMovementFactualFinancialIndicator(startDate, senderId, senderStorageId, recipientId,
                        recipientStorageId, articleMovementOperationType, waybillId,
                        firstIndicator.PurchaseCostSum + purchaseCostSumDelta,
                        firstIndicator.AccountingPriceSum + accountingPriceSumDelta,
                        firstIndicator.SalePriceSum + salePriceSumDelta);
                        
                    _new.PreviousId = firstIndicator.Id; // выставляем ссылку на предыдущую запись

                    articleMovementFactualFinancialIndicatorRepository.Save(_new);

                    // если есть следующий после firstIndicator показатель
                    if (secondIndicator != null)
                    {
                        _new.EndDate = secondIndicator.StartDate;
                        secondIndicator.PreviousId = _new.Id;
                    }
                }

                // изменяем значение показателей с датой начала > minimalStartDate
                foreach (var item in list.Where(x => x.StartDate > startDate))
                {
                    item.PurchaseCostSum += purchaseCostSumDelta;
                    item.AccountingPriceSum += accountingPriceSumDelta;
                    item.SalePriceSum += salePriceSumDelta;
                }
            }
        }

        /// <summary>
        /// Пересчитать суммы в закупочных ценах во всех затронутых индикаторах. Вызывается при установке цен в позициях данной приходной накладной (были нулевые)
        /// </summary>
        /// <param name="receiptWaybill">Приходная накладная (цены должны быть уже вычислены и записаны в нее)</param>
        public void SetPurchaseCosts(ReceiptWaybill receiptWaybill)
        {
            ChangePurchaseCosts(receiptWaybill, Decimal.One);
        }

        /// <summary>
        /// Пересчитать суммы в закупочных ценах во всех затронутых индикаторах. Вызывается при обнулении цен в позициях данной приходной накладной
        /// </summary>
        /// <param name="receiptWaybill">Приходная накладная</param>
        public void ResetPurchaseCosts(ReceiptWaybill receiptWaybill)
        {
            ChangePurchaseCosts(receiptWaybill, Decimal.MinusOne);
        }

        /// <summary>
        /// Пересчитать суммы в закупочных ценах во всех затронутых индикаторах.
        /// </summary>
        /// <param name="receiptWaybill">Приходная накладная, содержащая закупочные цены</param>
        /// <param name="sign">Знак (+1 или -1), с которым применяются данные закупочные цены (они возникают или обнуляются)</param>
        private void ChangePurchaseCosts(ReceiptWaybill receiptWaybill, decimal sign)
        {
            IEnumerable<Guid> movementWaybillIdList, writeoffWaybillIdList, expenditureWaybillIdList, returnFromClientWaybillIdList;
            IEnumerable<MovementWaybillRow> movementWaybillRowList;
            IEnumerable<WriteoffWaybillRow> writeoffWaybillRowList;
            IEnumerable<ExpenditureWaybillRow> expenditureWaybillRowList;
            IEnumerable<ReturnFromClientWaybillRow> returnFromClientWaybillRowList;
            DateTime startDate;

            // Получаем список индикаторов, непосредственно отражающих действия с накладными, имеющими позиции по заданному приходу
            var actionIndicatorList = articleMovementFactualFinancialIndicatorRepository.GetIndicatorListForReceiptWaybill(receiptWaybill.Id,
                out movementWaybillIdList, out movementWaybillRowList, out writeoffWaybillIdList, out writeoffWaybillRowList,
                out expenditureWaybillIdList, out expenditureWaybillRowList, out returnFromClientWaybillIdList, out returnFromClientWaybillRowList,
                out startDate);

            // Создаем словарь старых закупочных цен (ключ - код показателя, значение - старая ЗЦ, если нет такого элемента, ЗЦ не менялась)
            var oldPurchaseCostSumList = new Dictionary<Guid, decimal>();

            // 321 Так как проверяется только наличие показателя в данном списке (по id), можно бы сделать возвращаемое значение списком id - и облегчить запрос?
            // Если это не будет противоречить рефакторингу, о котором написано ниже (отобрать индикаторы во 2 список по одним ключам из 5-ти значений из списка actionIndicatorList)

            if (!actionIndicatorList.Any())
            {
                return;
            }

            // Приход всегда один. И операция по нему одна - приемка без расхождений. И первая по времени
            var receiptWaybillRowDictionary = receiptWaybill.Rows.ToDictionary(x => x.Id);

            // Вычисляем суммы в ЗЦ, на которые изменились суммы затронутых накладных, и помещаем в Dictionary
            var purchaseCostChangeSumList = new DynamicDictionary<Guid, decimal>();

            purchaseCostChangeSumList[receiptWaybill.Id] = receiptWaybillRowDictionary.Sum(x => Math.Round(x.Value.PendingCount * x.Value.PurchaseCost, 6));

            foreach (var movementWaybillRow in movementWaybillRowList)
            {
                purchaseCostChangeSumList[movementWaybillRow.MovementWaybill.Id] +=
                    Math.Round(movementWaybillRow.MovingCount * receiptWaybillRowDictionary[movementWaybillRow.ReceiptWaybillRow.Id].PurchaseCost, 6);
            }

            foreach (var writeoffWaybillRow in writeoffWaybillRowList)
            {
                purchaseCostChangeSumList[writeoffWaybillRow.WriteoffWaybill.Id] +=
                    Math.Round(writeoffWaybillRow.WritingoffCount * receiptWaybillRowDictionary[writeoffWaybillRow.ReceiptWaybillRow.Id].PurchaseCost, 6);
            }

            foreach (var expenditureWaybillRow in expenditureWaybillRowList)
            {
                purchaseCostChangeSumList[expenditureWaybillRow.SaleWaybill.Id] +=
                    Math.Round(expenditureWaybillRow.SellingCount * receiptWaybillRowDictionary[expenditureWaybillRow.ReceiptWaybillRow.Id].PurchaseCost, 6);
            }

            foreach (var returnFromClientWaybillRow in returnFromClientWaybillRowList)
            {
                purchaseCostChangeSumList[returnFromClientWaybillRow.ReturnFromClientWaybill.Id] +=
                    Math.Round(returnFromClientWaybillRow.ReturnCount *
                    receiptWaybillRowDictionary[returnFromClientWaybillRow.ReceiptWaybillRow.Id].PurchaseCost, 6);
            }

            // Сделана проверка только на получателя! (Ведь пересчитывать сумму в ЗЦ на конкретном месте хранения следует только с момента,
            // когда данное МХ появляется в получателе (RecipientStorageId) - т.е. когда на данное МХ приходит товар, приходом, перемещением или возвратом)
            var storageList = actionIndicatorList.Select(x => x.Value.RecipientStorageId).Distinct().Where(x => x != null).Select(x => x.Value);

            // Словарь дельт (изменений суммы) в ЗЦ по всем ключам по отношению к ситуации до пересчета
            // Ключ (см. сущность показателя) есть код организации-отправителя, код МХ-отправителя, код организации-получателя, код МХ-получателя, тип операции товародвижения
            var storagePurchaseCostChangeSumDictionary = new DynamicDictionary<int, DynamicDictionary<short, DynamicDictionary<int, DynamicDictionary<short,
                DynamicDictionary<ArticleMovementOperationType, decimal>>>>>();

            // Возможно, можно отбирать индикаторы во 2-й список по одним ключам из 5-ти значений из списка actionIndicatorList,
            // а не тянуть все для данного МХ. Но этот вопрос требует дополнительных исследований

            // Получаем список всех индикаторов, затронутых данными действиями (туда войдут и все из первого списка)
            var indicatorList = articleMovementFactualFinancialIndicatorRepository.GetIndicatorsListAfterDate(storageList, startDate);

            var indicatorDictionary = indicatorList.ToDictionary<ArticleMovementFactualFinancialIndicator, Guid>(x => x.Id);

            // Перебираем индикаторы по времени начала действия
            foreach (var indicator in indicatorList.OrderBy(x => x.StartDate))
            {
                // Из-за текущей системы проектирования показателей в БД остается строка даже для отмененной операции. При этом нет никакого признака, что операция отменена.
                // Поэтому можно брать разницу между данным показателем и предыдущим, и если она равна 0 - то игнорировать его.
                // Надо отметить, что при 0-х УЦ и ЗЦ может статься, что показатель не приращивает ни одно из значений, но не "отменен".
                // Этого тоже надо в будущем при перепроектировании избежать.
                decimal purchaseCostChangeSum, accountingPriceChangeSum, salePriceChangeSum;

                // Если показатель имеет предыдущий, вычисляем разность между ними во всех видах цен.
                if (indicator.PreviousId.HasValue)
                {
                    ArticleMovementFactualFinancialIndicator previousIndicator;

                    // Предыдущий может не попасть в выборку indicatorDictionary (если он был создан давно).
                    // Если же попадает, в indicatorDictionary быстро находим по ключу показатель с заданным Id.
                    if (indicatorDictionary.ContainsKey(indicator.PreviousId.Value))
                    {
                        previousIndicator = indicatorDictionary[indicator.PreviousId.Value];
                    }
                    else
                    {
                        previousIndicator = articleMovementFactualFinancialIndicatorRepository.GetById(indicator.PreviousId.Value);
                        ValidationUtils.NotNull(previousIndicator, "Индикатор не найден по своему идентификатору.");
                    }

                    decimal previousIndicatorOldPurchaseCostSum = oldPurchaseCostSumList.ContainsKey(previousIndicator.Id) ?
                        oldPurchaseCostSumList[previousIndicator.Id] : previousIndicator.PurchaseCostSum;
                    purchaseCostChangeSum = indicator.PurchaseCostSum - previousIndicatorOldPurchaseCostSum;
                    accountingPriceChangeSum = indicator.AccountingPriceSum - previousIndicator.AccountingPriceSum;
                    salePriceChangeSum = indicator.SalePriceSum - previousIndicator.SalePriceSum;
                }
                else
                {
                    purchaseCostChangeSum = indicator.PurchaseCostSum;
                    accountingPriceChangeSum = indicator.AccountingPriceSum;
                    salePriceChangeSum = indicator.SalePriceSum;
                }

                // Если показатель дает нулевое приращение - например, он остался на месте показателя по принятию накладной после отмены принятия - игнорируем его:
                // не изменяем из-за него дельту по его ключу (но, как и для всех, пересчитываем его сумму в ЗЦ по существующей дельте)
                if (purchaseCostChangeSum != 0M || accountingPriceChangeSum != 0M || salePriceChangeSum != 0M)
                {
                    // При нахождении данного показателя в списке непосредственно отражающих действия с накладными - изменяем дельту по данному ключу
                    if (actionIndicatorList.ContainsKey(indicator.Id))
                    {
                        decimal waybillPurchaseCostChangeSum = purchaseCostChangeSumList[indicator.WaybillId] * sign;

                        storagePurchaseCostChangeSumDictionary
                            [indicator.SenderId ?? 0]
                            [indicator.SenderStorageId ?? (short)0]
                            [indicator.RecipientId ?? 0]
                            [indicator.RecipientStorageId ?? (short)0]
                            [indicator.ArticleMovementOperationType] += waybillPurchaseCostChangeSum;
                    }
                }

                // Если дельта по данному ключу не равна 0, меняем значение индикатора
                decimal indicatorPurchaseCostChangeSum = storagePurchaseCostChangeSumDictionary[indicator.SenderId ?? 0]
                        [indicator.SenderStorageId ?? (short)0]
                        [indicator.RecipientId ?? 0]
                        [indicator.RecipientStorageId ?? (short)0]
                        [indicator.ArticleMovementOperationType];

                if (indicatorPurchaseCostChangeSum != 0M)
                {
                    oldPurchaseCostSumList[indicator.Id] = indicator.PurchaseCostSum;
                    indicator.PurchaseCostSum += indicatorPurchaseCostChangeSum;
                }
            }
        } 

        #endregion

        #endregion
    }
}