using ERP.Wholesale.Domain.Entities;
using System;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface IArticleRevaluationService
    {
        /// <summary>
        /// Поиск и расчет переоценки для РЦ, вступивших в силу или завершивших действие
        /// </summary>
        void CheckAccountingPriceListWithoutCalculatedRevaluation(DateTime currentDateTime);
        
        /// <summary>
        /// Проводка РЦ
        /// </summary>
        /// <param name="accountingPriceList">РЦ</param>
        void AccountingPriceListAccepted(AccountingPriceList accountingPriceList, DateTime currentDateTime);
        
        /// <summary>
        /// Отмена проводки РЦ
        /// </summary>
        /// <param name="accountingPriceList">РЦ</param>
        void AccountingPriceListAcceptanceCancelled(AccountingPriceList accountingPriceList, DateTime currentDateTime);
        
        #region Проводка накладной

        #region Проводка входящей накладной
        
        /// <summary>
        /// Проводка приходной накладной
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        void ReceiptWaybillAccepted(ReceiptWaybill waybill);

        #endregion

        #region Проводка исходящей накладной

        /// <summary>
        /// Проводка накладной реализации товаров
        /// </summary>
        /// <param name="waybill">Накладная реализации товаров</param>
        void ExpenditureWaybillAccepted(ExpenditureWaybill waybill);

        #endregion

        #endregion

        #region Отмена проводки накладной

        #region Отмена проводки входящей накладной

        /// <summary>
        /// Отмена проводки приходной накладной
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        void ReceiptWaybillAcceptanceCancelled(ReceiptWaybill waybill);

        /// <summary>
        /// Отмена проводки накладной возврата от клиента
        /// </summary>
        /// <param name="waybill">Накладная возврата от клиента</param>
        void ReturnFromClientWaybillAcceptanceCancelled(ReturnFromClientWaybill waybill);

        #endregion

        #region Отмена проводки исходящей накладной

        /// <summary>
        /// Отмена проводки накладной реализации товаров
        /// </summary>
        /// <param name="waybill">Накладная реализации товаров</param>
        void ExpenditureWaybillAcceptanceCancelled(ExpenditureWaybill waybill);

        /// <summary>
        /// Отмена проводки накладной списания
        /// </summary>
        /// <param name="waybill">Накладная списания</param>
        void WriteoffWaybillAcceptanceCancelled(WriteoffWaybill waybill);

        #endregion

        #region Отмена проводки входяще-исходящей накладной

        /// <summary>
        /// Отмена проводки накладной перемещения
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        void MovementWaybillAcceptanceCancelled(MovementWaybill waybill);
        
        /// <summary>
        /// Отмена проводки накладной смены собственника
        /// </summary>
        /// <param name="waybill">Накладная смены собственника</param>
        void ChangeOwnerWaybillAcceptanceCancelled(ChangeOwnerWaybill waybill);

        #endregion

        #endregion

        #region Перевод накладной в финальный статус

        /// <summary>
        /// Приемка приходной накладной
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        void ReceiptWaybillReceipted(ReceiptWaybill waybill);

        /// <summary>
        /// Согласование приходной накладной
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        void ReceiptWaybillApproved(ReceiptWaybill waybill);

        /// <summary>
        /// Приемка накладной перемещения
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        void MovementWaybillFinalized(MovementWaybill waybill);
        
        /// <summary>
        /// Приемка накладной смены собственника
        /// </summary>
        /// <param name="waybill">Накладная смены собственника</param>
        void ChangeOwnerWaybillFinalized(ChangeOwnerWaybill waybill);
        
        /// <summary>
        /// Приемка накладной возврата товаров от клиента
        /// </summary>
        /// <param name="waybill">Накладная возврата от клиента</param>
        void ReturnFromClientWaybillFinalized(ReturnFromClientWaybill waybill);

        /// <summary>
        /// Перевод накладной реализации товаров в финальный статус (приемка получателем)
        /// </summary>
        /// <param name="waybill">Накладная реализации товаров</param>
        void ExpenditureWaybillFinalized(ExpenditureWaybill waybill);

        /// <summary>
        /// Перевод накладной списания товаров в финальный статус (списано)
        /// </summary>
        /// <param name="waybill">Накладная списания товаров</param>
        void WriteoffWaybillFinalized(WriteoffWaybill waybill);
        
        #endregion

        #region Отмена перевода накладной в финальный статус

        /// <summary>
        /// Отмена приемки приходной накладной
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        void ReceiptWaybillReceiptCancelled(ReceiptWaybill waybill);

        /// <summary>
        /// Отмена согласования приходной накладной 
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        void ReceiptWaybillApprovementCancelled(ReceiptWaybill waybill);

        /// <summary>
        /// Отмена приемки накладной перемещения
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        void MovementWaybillReceiptCancelled(MovementWaybill waybill);

        /// <summary>
        /// Отмена приемки накладной смены собственника
        /// </summary>
        /// <param name="waybill">Накладная смены собственника</param>
        void ChangeOwnerWaybillReceiptCancelled(ChangeOwnerWaybill waybill);

        /// <summary>
        /// Отмена приемки накладной возврата от клиента
        /// </summary>
        /// <param name="waybill">Накладная возврата от клиента</param>
        void ReturnFromClientWaybillReceiptCancelled(ReturnFromClientWaybill waybill);

        /// <summary>
        /// Отмена перевода накладной реализации товаров в финальный статус
        /// </summary>
        /// <param name="waybill">Накладная реализации товаров</param>
        void ExpenditureWaybillFinalizationCancelled(ExpenditureWaybill waybill);

        /// <summary>
        /// Отмена перевода накладной списания товаров в финальный статус
        /// </summary>
        /// <param name="waybill">Накладная списания товаров</param>
        void WriteoffWaybillFinalizationCancelled(WriteoffWaybill waybill);

        #endregion
    }
}