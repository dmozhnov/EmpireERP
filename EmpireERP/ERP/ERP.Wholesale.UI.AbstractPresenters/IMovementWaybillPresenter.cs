using System;
using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.ViewModels.MovementWaybill;
using ERP.Wholesale.UI.ViewModels.OutgoingWaybill;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common.TORG12;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common.Т1;
using ERP.Wholesale.UI.ViewModels.PrintingForm.MovementWaybill;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IMovementWaybillPresenter : IBaseWaybillPresenter<MovementWaybill>
    {
        MovementWaybillListViewModel List(UserInfo currentUser);
        GridData GetShippingPendingGrid(GridState state, UserInfo currentUser);
        GridData GetShippedGrid(GridState state, UserInfo currentUser);
        GridData GetReceiptedGrid(GridState state, UserInfo currentUser);

        MovementWaybillSelectViewModel SelectWaybill(int articleId, UserInfo currentUser);
        GridData GetWaybillSelectGrid(GridState state, UserInfo currentUser);

        MovementWaybillEditViewModel Create(string backURL, UserInfo currentUser);
        MovementWaybillEditViewModel Edit(Guid id, string backURL, UserInfo currentUser);

        object GetAccountOrganizationsForRecipientStorage(short storageId, UserInfo currentUser);
        object GetAccountOrganizationsForSenderStorage(short storageId, UserInfo currentUser);
                
        string Save(MovementWaybillEditViewModel model, UserInfo currentUser);

        MovementWaybillDetailsViewModel Details(Guid id, string backURL, UserInfo currentUser);

        GridData GetMovementWaybillRowGrid(GridState state, UserInfo currentUser);
        /// <summary>
        /// Получить грид с группами товаров
        /// </summary>
        GridData GetMovementWaybillArticleGroupGrid(GridState state, UserInfo currentUser);

        MovementWaybillRowEditViewModel AddRow(Guid movementWaybillId, UserInfo currentUser);
        OutgoingWaybillAddRowsByListViewModel AddRowsByList(Guid waybillId, string backURL, UserInfo currentUser);
        void AddRowSimply(Guid waybillId, int articleId, decimal count, UserInfo currentUser);
        GridData GetArticlesForWaybillRowsAdditionByListGrid(GridState state, UserInfo currentUser);
        MovementWaybillRowEditViewModel EditRow(Guid movementWaybillId, Guid movementWaybillRowId, UserInfo currentUser);
        object GetRowInfo(Guid waybillId, Guid batchId, UserInfo currentUser);
        object SaveRow(MovementWaybillRowEditViewModel model, UserInfo currentUser);
        object DeleteRow(Guid movementWaybillId, Guid movementWaybillRowId, UserInfo currentUser);

        GridData GetDocGrid(GridState state, UserInfo currentUser);

        object PrepareToAccept(Guid id, UserInfo currentUser);
        object CancelReadinessToAccept(Guid id, UserInfo currentUser);
        object Ship(Guid id, UserInfo currentUser);
        object CancelShipping(Guid id, UserInfo currentUser);
        object Accept(Guid id, UserInfo currentUser);
        object CancelAcceptance(Guid id, UserInfo currentUser);
        object Receipt(Guid id, UserInfo currentUser);
        object CancelReceipt(Guid id, UserInfo currentUser);
        void Delete(Guid id, UserInfo currentUser);

        /// <summary>
        /// Получение модели параметров ПФ "Товарный чек"
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        CashMemoPrintingFormSettingsViewModel GetCashMemoPrintingFormSettings(Guid waybillId, UserInfo currentUser);
        CashMemoPrintingFormViewModel GetCashMemoPrintingForm(CashMemoPrintingFormSettingsViewModel settings, UserInfo currentUser);
        
        MovementWaybillPrintingFormSettingsViewModel GetPrintingFormSettings(bool PrintSenderPrice, bool PrintRecepientPrice, bool PrintMarkup, UserInfo currentUser);
        MovementWaybillPrintingFormViewModel GetPrintingForm(MovementWaybillPrintingFormSettingsViewModel settings, UserInfo currentUser);

        /// <summary>
        /// Получение модели параметров ПФ счет фактуры
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        InvoicePrintingFormSettingsViewModel GetInvoicePrintingFormSettings(Guid waybillId, UserInfo currentUser);
        
        /// <summary>
        /// Получение модели ПФ счет фактура
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        InvoicePrintingFormViewModel GetInvoicePrintingForm(InvoicePrintingFormSettingsViewModel settings, UserInfo currentUser);
        
        /// <summary>
        /// Получение информации о строках печатной формы
        /// </summary>
        InvoicePrintingFormRowsViewModel GetInvoicePrintingFormRows(InvoicePrintingFormSettingsViewModel settings, UserInfo currentUser);

        /// <summary>
        /// Экспорт счет-фактуры в Excel
        /// </summary>
        byte[] ExportInvoicePrintingFormToExcel(InvoicePrintingFormSettingsViewModel settings, UserInfo currentUser);

        /// <summary>
        /// Получение модели печатной формы ТОРГ 12
        /// </summary>
        /// <param name="settings">Параметры печатной формы</param>
        /// <returns>Модель печатной формы</returns>
        TORG12PrintingFormViewModel GetTORG12PrintingForm(TORG12PrintingFormSettingsViewModel settings, UserInfo currentUser);

        /// <summary>
        /// Получение модели параметров ПФ ТОРГ12
        /// </summary>        
        TORG12PrintingFormSettingsViewModel GetTORG12PrintingFormSettings(Guid waybillId, UserInfo currentUser);

        /// <summary>
        /// Получение информации о строках печатной формы
        /// </summary>        
        TORG12PrintingFormRowsViewModel GetTORG12PrintingFormRows(TORG12PrintingFormSettingsViewModel settings, UserInfo currentUser);
        
        /// <summary>
        /// Экспорт ТОРГ - 12 в Excel
        /// </summary>
        byte[] ExportTORG12PrintingFormToExcel(TORG12PrintingFormSettingsViewModel settings, UserInfo currentUser);

        #region T1 (TTH)

        /// <summary>
        /// Форма настройки печати Т1 (ТТН)
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns>Модель настроек</returns>
        T1PrintingFormSettingsViewModel GetT1PrintingFormSettings(Guid waybillId, UserInfo currentUser);
        
        /// <summary>
        /// Формирование общей части формы Т1
        /// </summary>
        /// <param name="settings">Настройки</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns>Модель</returns>
        T1ProductSectionPrintingFormViewModel ShowT1ProductSectionPrintingForm(T1PrintingFormSettingsViewModel settings, UserInfo currentUser);        

        /// <summary>
        /// Получение позиций для формы Т1
        /// </summary>
        /// <param name="settings">Настройки</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns>Модель позиций</returns>
        T1ProductSectionPrintingFormRowsViewModel ShowT1ProductSectionPrintingFormRows(T1PrintingFormSettingsViewModel settings, UserInfo currentUser);

        #endregion
    }
}
