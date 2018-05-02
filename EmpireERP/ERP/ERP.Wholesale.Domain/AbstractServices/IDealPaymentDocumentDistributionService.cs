using System;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Misc;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface IDealPaymentDocumentDistributionService
    {
        /// <summary>
        /// Разнесение оплаты от клиента на список других сущностей. Создает разнесения платежного документа.
        /// Может остаться неразнесенный остаток.
        /// Если неразнесенной суммы оплаты от клиента недостаточно для разнесения, выбрасывается исключение.
        /// </summary>
        /// <param name="dealPaymentFromClient">Оплата от клиента для разнесения</param>
        /// <param name="dealPaymentDocumentDistributionInfoList">Список сущностей и сумм для разнесения</param>
        /// <param name="currentDate">Дата операции</param>
        void DistributeDealPaymentFromClient(DealPaymentFromClient dealPaymentFromClient,
            IEnumerable<DealPaymentDocumentDistributionInfo> dealPaymentDocumentDistributionInfoList, DateTime currentDate);

        /// <summary>
        /// Разнесение кредитовой корректировки сальдо на список других сущностей. Создает разнесения платежного документа.
        /// Может остаться неразнесенный остаток.
        /// Если неразнесенной суммы кредитовой корректировки сальдо недостаточно для разнесения, выбрасывается исключение.
        /// </summary>
        /// <param name="dealCreditInitialBalanceCorrection">Кредитовая корректировка сальдо для разнесения</param>
        /// <param name="dealPaymentDocumentDistributionInfoList">Список сущностей и сумм для разнесения</param>
        /// <param name="currentDate">Дата операции</param>
        void DistributeDealCreditInitialBalanceCorrection(DealCreditInitialBalanceCorrection dealCreditInitialBalanceCorrection,
            IEnumerable<DealPaymentDocumentDistributionInfo> dealPaymentDocumentDistributionInfoList, DateTime currentDate);

        /// <summary>
        /// Автоматическое разнесение подходящих платежных документов на создаваемый возврат оплаты клиенту
        /// </summary>
        /// <param name="dealPaymentToClient">Возврат оплаты клиенту для разнесения на него</param>
        /// <param name="currentDate">Дата операции</param>
        void DistributeDealPaymentToClient(DealPaymentToClient dealPaymentToClient, DateTime currentDate);

        /// <summary>
        /// Автоматическое разнесение подходящих платежных документов на создаваемую дебетовую корректировку сальдо
        /// </summary>
        /// <param name="dealDebitInitialBalanceCorrection">Дебетовая корректировка сальдо для разнесения на него</param>
        /// <param name="currentDate">Дата операции</param>
        void DistributeDealDebitInitialBalanceCorrection(DealDebitInitialBalanceCorrection dealDebitInitialBalanceCorrection, DateTime currentDate);

        /// <summary>
        /// Попытаться оплатить накладную реализации из аванса (т.е. неразнесенных остатков платежных документов).
        /// После разнесения может оставаться неоплаченный остаток у накладной или неразнесенные остатки платежных документов.
        /// Если накладная не имеет положительного остатка, расшифровки распределения оплаты не будут созданы.
        /// При полной оплате накладной реализации происходит установка признака того, что накладная полностью оплачена
        /// </summary>
        /// <param name="saleWaybill">Накладная реализации, на которую выполняется разнесение платежных документов</param>
        void PaySaleWaybillOnAccept(SaleWaybill saleWaybill, DateTime currentDate);

        /// <summary>
        /// Отмена всех расшифровок распределения оплаты, разнесенных на данную накладную
        /// </summary>
        /// <param name="saleWaybill">Накладная реализации</param>
        void CancelSaleWaybillPaymentDistribution(SaleWaybill saleWaybill);

        /// <summary>
        /// Возврат оплаты по возвращенным позициям
        /// </summary>
        /// <param name="returnFromClientWaybill">Накладная возврата товаров от клиента</param>
        /// <param name="currentDate">Дата операции</param>
        void ReturnPaymentToSales(ReturnFromClientWaybill returnFromClientWaybill, DateTime currentDate);

        /// <summary>
        /// Отмена возврата оплаты по возвращенным позициям
        /// </summary>
        /// <param name="returnFromClientWaybill">Накладная возврата товаров от клиента</param>
        /// <param name="receiptDate">Дата приемки накладной реализации товаров</param>
        void CancelPaymentReturnToSales(ReturnFromClientWaybill returnFromClientWaybill, DateTime receiptDate);

    }
}
