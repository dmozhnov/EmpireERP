using System;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface IAccountingPriceListWaybillTakingService
    {
        /// <summary>
        /// Создание связей между позициями РЦ и позициями входящих принятых накладных по точному наличию (Дельта 0)
        /// </summary>
        /// <param name="incomingRows">Список позиций входящих принятых накладных</param>
        /// <param name="priceList">РЦ</param>
        /// <param name="isOnAccountingPriceListStart">Признак: создается ли связь на начало периода действия РЦ</param>
        /// <param name="takingDate">Дата и время связи</param>
        void CreateTakingFromExactArticleAvailability(IEnumerable<IncomingWaybillRow> incomingRows, AccountingPriceList priceList, bool isOnAccountingPriceListStart, DateTime takingDate);

        /// <summary>
        /// Создание связей между позициями РЦ и позициями входящих проведенных, но не принятых накладных (Дельта 1+)
        /// </summary>
        /// <param name="incomingRows">Список позиций входящих проведенных, но не принятых накладных</param>
        /// <param name="priceList">РЦ</param>
        /// <param name="isOnAccountingPriceListStart">Признак: создается ли связь на начало периода действия РЦ</param>
        /// <param name="takingDate">Дата и время связи</param>
        void CreateTakingFromIncomingAcceptedArticleAvailability(IEnumerable<IncomingWaybillRow> incomingRows, AccountingPriceList priceList, bool isOnAccountingPriceListStart, DateTime takingDate);

        /// <summary>
        /// Создание связей между позициями РЦ и позициями входящих принятых с расхождениями накладных
        /// </summary>
        /// <param name="incomingRows">Список позиций входящих принятых с расхождениями накладных</param>
        /// <param name="priceList">РЦ</param>
        /// <param name="isOnAccountingPriceListStart">Признак: создается ли связь на начало периода действия РЦ</param>
        /// <param name="takingDate">Дата и время связи</param>
        void CreateTakingFromIncomingDivergenceArticleAvailability(IEnumerable<IncomingWaybillRow> incomingRows, AccountingPriceList priceList, bool isOnAccountingPriceListStart, DateTime takingDate);

        /// <summary>
        /// Создание связей между позициями РЦ и позициями исходящих проведенных, но не отгруженных/не принятых получателем накладных (Дельта 1-)
        /// </summary>
        /// <param name="outgoingRows">Список позиций исходящих проведенных, но не отгруженных/не принятых получателем накладных</param>
        /// <param name="priceList">РЦ</param>
        /// <param name="isOnAccountingPriceListStart">Признак: создается ли связь на начало периода действия РЦ</param>
        /// <param name="takingDate">Дата и время связи</param>
        void CreateTakingFromOutgoingAcceptedArticleAvailability(IEnumerable<OutgoingWaybillRow> outgoingRows, AccountingPriceList priceList, bool isOnAccountingPriceListStart, DateTime takingDate);
    }
}
